using SqlServerPersistence.Data;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.Features.ChecksumExport;

public static class ChecksumQueryBuilder
{
    public static string Build(TableInfo table, IReadOnlyList<ColumnInfo> columns)
    {
        if (columns.Count == 0)
            throw new InvalidOperationException($"No visible columns found for {table}.");
        var keys = table.PrimaryKeys.OrderBy(x => x, StringComparer.Ordinal).Select(SqlIdentifier.Quote).ToArray();
        var expressions = columns.Select(c => c.TypeName.ToLowerInvariant() switch
        {
            "text" or "ntext" or "xml" => $"CAST({SqlIdentifier.Quote(c.Name)} AS nvarchar(max))",
            "image" or "geography" or "geometry" or "hierarchyid" => $"CAST({SqlIdentifier.Quote(c.Name)} AS varbinary(max))",
            _ => SqlIdentifier.Quote(c.Name)
        });
        // ORDER BY ordinal avoids ambiguity if a primary key is itself named Checksum.
        var order = Enumerable.Range(1, keys.Length + 1);
        return SqlScripts.Render("ChecksumExport.SelectRowChecksums", new Dictionary<string, string>
        {
            ["KeyColumns"] = string.Concat(keys.Select(k => k + ", ")),
            ["ChecksumExpressions"] = string.Join(", ", expressions),
            ["TableName"] = $"{SqlIdentifier.Quote(table.Schema)}.{SqlIdentifier.Quote(table.Name)}",
            ["OrderBy"] = string.Join(", ", order)
        });
    }
}
