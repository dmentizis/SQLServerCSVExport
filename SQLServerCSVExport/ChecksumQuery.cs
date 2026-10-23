using SQLServerCSVExport.Objects;

namespace SQLServerCSVExport;

public static class ChecksumQuery
{
    public static string Quote(string identifier) => "[" + identifier.Replace("]", "]]") + "]";

    public static string Build(Table table, IReadOnlyList<Column> columns)
    {
        if (columns.Count == 0)
            throw new InvalidOperationException($"No visible columns found for {table}.");
        var keys = table.PrimaryKeys.OrderBy(x => x, StringComparer.Ordinal).Select(Quote).ToArray();
        var expressions = columns.Select(c => c.TypeName.ToLowerInvariant() switch
        {
            "text" or "ntext" or "xml" => $"CAST({Quote(c.Name)} AS nvarchar(max))",
            "image" or "geography" or "geometry" or "hierarchyid" => $"CAST({Quote(c.Name)} AS varbinary(max))",
            _ => Quote(c.Name)
        });
        // ORDER BY ordinal avoids ambiguity if a primary key is itself named Checksum.
        var select = keys.Append($"CHECKSUM({string.Join(", ", expressions)}) AS [Checksum]");
        var order = Enumerable.Range(1, keys.Length + 1);
        return $"SELECT {string.Join(", ", select)} FROM {Quote(table.Schema)}.{Quote(table.Name)} ORDER BY {string.Join(", ", order)};";
    }
}
