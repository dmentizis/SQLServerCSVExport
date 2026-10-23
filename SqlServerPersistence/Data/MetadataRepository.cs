using System.Data;
using Microsoft.Data.SqlClient;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.Data;

public sealed class MetadataRepository
{
    public async Task<List<string>> GetDatabasesAsync(ConnectionSettings settings, CancellationToken token)
    {
        await using var connection = SqlConnectionFactory.Create(settings with { Name = "master" });
        await connection.OpenAsync(token);
        using var command = new SqlCommand(SqlScripts.Get("Metadata.GetDatabases"), connection);
        await using var reader = await command.ExecuteReaderAsync(token);
        List<string> names = [];
        while (await reader.ReadAsync(token)) names.Add(reader.GetString(0));
        return names;
    }

    public async Task<List<TableInfo>> GetTablesAsync(ConnectionSettings settings, CancellationToken token)
    {
        await using var connection = SqlConnectionFactory.Create(settings);
        await connection.OpenAsync(token);
        using var command = new SqlCommand(SqlScripts.Get("Metadata.GetTables"), connection) { CommandTimeout = settings.CommandTimeout };
        await using var reader = await command.ExecuteReaderAsync(token);
        List<TableInfo> tables = [];
        while (await reader.ReadAsync(token))
            tables.Add(new TableInfo { ObjectId = reader.GetInt32(0), Schema = reader.GetString(1), Name = reader.GetString(2), RowCount = reader.GetInt64(3) });
        var byId = tables.ToDictionary(t => t.ObjectId);
        await reader.NextResultAsync(token);
        while (await reader.ReadAsync(token))
            if (byId.TryGetValue(reader.GetInt32(0), out var table)) table.PrimaryKeys.Add(reader.GetString(1));
        return tables;
    }

    public async Task<List<ColumnInfo>> GetTableColumnsAsync(SqlConnection connection, TableInfo table, int commandTimeout, CancellationToken token)
    {
        using var command = new SqlCommand(SqlScripts.Get("Metadata.GetTableColumns"), connection) { CommandTimeout = commandTimeout };
        command.Parameters.Add("@ObjectId", SqlDbType.Int).Value = table.ObjectId;
        await using var reader = await command.ExecuteReaderAsync(token);
        List<ColumnInfo> columns = [];
        while (await reader.ReadAsync(token)) columns.Add(new ColumnInfo(reader.GetString(0), reader.GetString(1)));
        return columns;
    }
}
