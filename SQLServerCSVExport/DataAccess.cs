using System.Data;
using Microsoft.Data.SqlClient;
using SQLServerCSVExport.Objects;

namespace SQLServerCSVExport;

public static class DataAccess
{
    public static SqlConnection CreateConnection(Database database)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = database.Server,
            InitialCatalog = database.Name,
            IntegratedSecurity = database.IntegratedSecurity,
            Encrypt = SqlConnectionEncryptOption.Mandatory,
            TrustServerCertificate = database.TrustServerCertificate,
            ConnectTimeout = 15,
            ApplicationName = "SQL Server Persistence"
        };
        if (!database.IntegratedSecurity)
        {
            builder.UserID = database.Username;
            builder.Password = database.Password;
        }
        return new SqlConnection(builder.ConnectionString);
    }

    public static async Task<List<string>> GetDatabasesAsync(Database database, CancellationToken token)
    {
        await using var connection = CreateConnection(database with { Name = "master" });
        await connection.OpenAsync(token);
        using var command = new SqlCommand("SELECT name FROM sys.databases WHERE state = 0 AND HAS_DBACCESS(name) = 1 ORDER BY name;", connection);
        await using var reader = await command.ExecuteReaderAsync(token);
        List<string> names = [];
        while (await reader.ReadAsync(token)) names.Add(reader.GetString(0));
        return names;
    }

    public static async Task<List<Table>> GetDatabaseDataAsync(Database database, CancellationToken token)
    {
        await using var connection = CreateConnection(database);
        await connection.OpenAsync(token);
        const string sql = """
            SELECT t.object_id, s.name, t.name,
                COALESCE((SELECT SUM(p.rows) FROM sys.partitions p
                          WHERE p.object_id = t.object_id AND p.index_id IN (0, 1)), 0)
            FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id
            WHERE t.is_ms_shipped = 0 ORDER BY s.name, t.name;

            SELECT i.object_id, c.name
            FROM sys.indexes i
            JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
            WHERE i.is_primary_key = 1 AND ic.key_ordinal > 0
            ORDER BY i.object_id, ic.key_ordinal;
            """;
        using var command = new SqlCommand(sql, connection) { CommandTimeout = database.CommandTimeout };
        await using var reader = await command.ExecuteReaderAsync(token);
        List<Table> tables = [];
        while (await reader.ReadAsync(token))
            tables.Add(new Table { ObjectId = reader.GetInt32(0), Schema = reader.GetString(1), Name = reader.GetString(2), RowCount = reader.GetInt64(3) });
        var byId = tables.ToDictionary(t => t.ObjectId);
        await reader.NextResultAsync(token);
        while (await reader.ReadAsync(token))
            if (byId.TryGetValue(reader.GetInt32(0), out var table)) table.PrimaryKeys.Add(reader.GetString(1));
        return tables;
    }

    public static async Task<List<Column>> GetTableColumnsAsync(SqlConnection connection, Table table, int timeout, CancellationToken token)
    {
        // Use the underlying system type for SQL alias types, and the named type for CLR types.
        const string sql = """
            SELECT c.name, COALESCE(bt.name, ut.name)
            FROM sys.columns c
            JOIN sys.types ut ON ut.user_type_id = c.user_type_id
            LEFT JOIN sys.types bt ON bt.user_type_id = c.system_type_id AND bt.system_type_id = c.system_type_id
            WHERE c.object_id = @id AND c.is_hidden = 0
            ORDER BY c.column_id;
            """;
        using var command = new SqlCommand(sql, connection) { CommandTimeout = timeout };
        command.Parameters.Add("@id", SqlDbType.Int).Value = table.ObjectId;
        await using var reader = await command.ExecuteReaderAsync(token);
        List<Column> columns = [];
        while (await reader.ReadAsync(token)) columns.Add(new Column(reader.GetString(0), reader.GetString(1)));
        return columns;
    }
}
