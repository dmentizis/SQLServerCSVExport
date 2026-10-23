namespace SQLServerCSVExport.Objects;

public sealed record Database
{
    public string Server { get; init; } = "";
    public string Name { get; init; } = "";
    public string Username { get; init; } = "";
    public string Password { get; init; } = "";
    public bool IntegratedSecurity { get; init; }
    public bool TrustServerCertificate { get; init; }
    public int CommandTimeout { get; init; } = 300;

    // Do not expose credentials through the record's generated ToString().
    public override string ToString() => $"{Server} / {Name}";
}
