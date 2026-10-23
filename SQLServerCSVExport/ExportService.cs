using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using SQLServerCSVExport.Objects;

namespace SQLServerCSVExport;

public sealed record ExportProgress(string Message, int Percent);
public sealed record ExportResult(string Directory, int Exported, int Excluded, int Failed);

public sealed class ExportService
{
    public async Task<ExportResult> ExportAsync(Database database, string destination,
        IReadOnlyList<ExclusionTable> exclusions, IProgress<ExportProgress> progress, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(database.Server) || string.IsNullOrWhiteSpace(database.Name))
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
            var tables = await DataAccess.GetDatabaseDataAsync(database, token);
            IEnumerable<object?>[] header = [["Schema", "Name", "RowCount"]];
            await Helper.WriteCsvAsync(Path.Combine(directory, "_TableData.csv"),
                header.Concat(tables.Select(t => new object?[] { t.Schema, t.Name, t.RowCount })), token);

            for (var i = 0; i < tables.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                var table = tables[i];
                if (Helper.IsInExclusionList(table.Schema, table.Name, exclusions))
                {
                    excluded++;
                    entries.Add(new { table.Schema, table.Name, Status = "Excluded" });
                    progress.Report(new($"Excluded {table}.", (i + 1) * 100 / tables.Count));
                    continue;
                }
                var filename = Helper.TableFileName(table);
                try
                {
                    progress.Report(new($"Exporting {table}…", i * 100 / tables.Count));
                    var count = await ExportTableAsync(database, table, Path.Combine(directory, filename), token);
                    exported++;
                    entries.Add(new { table.Schema, table.Name, File = filename, Rows = count, Status = "Completed" });
                    progress.Report(new($"Exported {table}: {count:N0} rows.", (i + 1) * 100 / tables.Count));
                }
                catch (Exception) when (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
                catch (Exception ex)
                {
                    failed++;
                    entries.Add(new { table.Schema, table.Name, File = filename, Status = "Failed", Error = ex.Message });
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
            var manifest = new { database.Server, Database = database.Name, Status = status,
                Algorithm = "SQL Server CHECKSUM", Exported = exported, Excluded = excluded, Failed = failed, Tables = entries };
            await File.WriteAllTextAsync(Path.Combine(directory, "manifest.json"),
                JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    private static async Task<long> ExportTableAsync(Database database, Table table, string path, CancellationToken token)
    {
        await using var connection = DataAccess.CreateConnection(database);
        await connection.OpenAsync(token);
        var columns = await DataAccess.GetTableColumnsAsync(connection, table, database.CommandTimeout, token);
        using var command = new SqlCommand(ChecksumQuery.Build(table, columns), connection) { CommandTimeout = database.CommandTimeout };
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, token);
        long count = 0;
        await using (var writer = new StreamWriter(path + ".partial", false, new UTF8Encoding(false)))
        {
            writer.NewLine = "\r\n";
            await writer.WriteLineAsync(Helper.CsvRow(Enumerable.Range(0, reader.FieldCount).Select(reader.GetName)).AsMemory(), token);
            var values = new object[reader.FieldCount];
            while (await reader.ReadAsync(token))
            {
                reader.GetValues(values);
                await writer.WriteLineAsync(Helper.CsvRow(values).AsMemory(), token);
                count++;
            }
        }
        token.ThrowIfCancellationRequested();
        File.Move(path + ".partial", path);
        return count;
    }
}
