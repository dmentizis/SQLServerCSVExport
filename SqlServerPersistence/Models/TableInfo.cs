namespace SqlServerPersistence.Models;

public sealed class TableInfo
{
    public int ObjectId { get; init; }
    public required string Schema { get; init; }
    public required string Name { get; init; }
    public long RowCount { get; init; }
    public List<string> PrimaryKeys { get; } = [];
    public override string ToString() => $"{Schema}.{Name}";
}
