using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using SqlServerPersistence.Csv;
using SqlServerPersistence.Data;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.Features.ChecksumExport;

public sealed class ChecksumExportService(MetadataRepository metadata)
{
    public async Task<ExportResult> ExportAsync(ConnectionSettings settings, string destination,
        IReadOnlyList<TableExclusion> exclusions, IProgress<ExportProgress> progress, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(settings.Server) || string.IsNullOrWhiteSpace(settings.Database))
            throw new ArgumentException("Choose a connected server and database first.");
        if (!Directory.Exists(destination)) throw new DirectoryNotFoundException("Choose an existing export folder.");

        var directory = Path.Combine(destination, $"Export_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        progress.Report(new($"Export folder: {directory}", 0));
        var entries = new List<object>();
        var status = "Running";
        int exported = 0, excluded = 0, failed = 0;
        try
        {
            progress.Report(new("Reading table metadata…", 0));
            var tables = await metadata.GetTablesAsync(settings, token);
            IEnumerable<object?>[] header = [["Schema", "Name", "RowCount"]];
            await CsvFileWriter.WriteAsync(Path.Combine(directory, "_TableData.csv"),
                header.Concat(tables.Select(t => new object?[] { t.Schema, t.Name, t.RowCount })), token);

            for (var i = 0; i < tables.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                var table = tables[i];
                if (exclusions.Any(e => e.Matches(table.Schema, table.Name)))
                {
                    excluded++;
                    entries.Add(new { table.Schema, table.Name, Status = "Excluded" });
                    progress.Report(new($"Excluded {table}.", (i + 1) * 100 / tables.Count));
                    continue;
                }
                var fileName = ExportFileNaming.TableFileName(table);
                try
                {
                    progress.Report(new($"Exporting {table}…", i * 100 / tables.Count));
                    var count = await ExportTableAsync(settings, table, Path.Combine(directory, fileName), token);
                    exported++;
                    entries.Add(new { table.Schema, table.Name, File = fileName, Rows = count, Status = "Completed" });
                    progress.Report(new($"Exported {table}: {count:N0} rows.", (i + 1) * 100 / tables.Count));
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
                catch (Exception ex)
                {
                    failed++;
                    entries.Add(new { table.Schema, table.Name, File = fileName, Status = "Failed", Error = ex.Message });
                    progress.Report(new($"ERROR — {table}: {ex.Message}", (i + 1) * 100 / tables.Count));
                }
            }
            token.ThrowIfCancellationRequested();
            status = failed == 0 ? "Completed" : "CompletedWithErrors";
            return new(directory, exported, excluded, failed);
        }
        catch (Exception) when (token.IsCancellationRequested)
        {
            status = "Cancelled";
            throw new OperationCanceledException(token);
        }
        catch
        {
            status = "Failed";
            throw;
        }
        finally
        {
            // Use no cancellation token so the outcome is recorded even after Cancel.
            var manifest = new { settings.Server, settings.Database, Status = status,
                Algorithm = "SQL Server CHECKSUM", Exported = exported, Excluded = excluded, Failed = failed, Tables = entries };
            await File.WriteAllTextAsync(Path.Combine(directory, "manifest.json"),
                JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    private async Task<long> ExportTableAsync(ConnectionSettings settings, TableInfo table, string path, CancellationToken token)
    {
        await using var connection = SqlConnectionFactory.Create(settings);
        await connection.OpenAsync(token);
        var columns = await metadata.GetTableColumnsAsync(connection, table, settings.CommandTimeout, token);
        using var command = new SqlCommand(ChecksumQueryBuilder.Build(table, columns), connection) { CommandTimeout = settings.CommandTimeout };
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, token);
        long count = 0;
        await using var writer = new CsvFileWriter(path);
        await writer.WriteRowAsync(Enumerable.Range(0, reader.FieldCount).Select(reader.GetName), token);
        var values = new object[reader.FieldCount];
        while (await reader.ReadAsync(token))
        {
            reader.GetValues(values);
            await writer.WriteRowAsync(values, token);
            count++;
        }
        await writer.CompleteAsync(token);
        return count;
    }
}
