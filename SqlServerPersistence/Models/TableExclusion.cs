namespace SqlServerPersistence.Models;

public sealed record TableExclusion(string Schema, string Table)
{
    // Exact, case-sensitive match; no wildcard syntax.
    public bool Matches(string schema, string table) => Schema == schema && Table == table;

    public override string ToString() => $"{Schema}.{Table}";
}
