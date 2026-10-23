namespace SQLServerCSVExport.Objects;

public sealed class Table
{
    public int ObjectId { get; init; }
    public required string Schema { get; init; }
    public required string Name { get; init; }
    public long RowCount { get; init; }
    public List<string> PrimaryKeys { get; } = [];
    public override string ToString() => $"{Schema}.{Name}";
}

public sealed record Column(string Name, string TypeName);

public sealed record ExclusionTable(string Schema, string Table)
{
    public override string ToString() => $"{Schema}.{Table}";
}
