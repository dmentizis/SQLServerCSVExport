using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace SqlServerPersistence.Data;

/// <summary>
/// Loads the .sql files under Sql/, which are compiled in as embedded resources.
/// Names are relative to Sql/ with dots instead of folder separators, e.g. "Metadata.GetTables".
/// </summary>
public static partial class SqlScripts
{
    private const string ResourcePrefix = "SqlServerPersistence.Sql.";
    private static readonly ConcurrentDictionary<string, string> Cache = new();

    public static string Get(string name) => Cache.GetOrAdd(name, Load);

    /// <summary>
    /// Fills {{Token}} placeholders in a single pass, so substituted values are never re-scanned.
    /// </summary>
    public static string Render(string name, IReadOnlyDictionary<string, string> values) =>
        PlaceholderRegex().Replace(Get(name), match => values.TryGetValue(match.Groups[1].Value, out var value)
            ? value
            : throw new InvalidOperationException($"SQL script '{name}' has no value for placeholder {match.Value}."));

    private static string Load(string name)
    {
        using var stream = typeof(SqlScripts).Assembly.GetManifestResourceStream(ResourcePrefix + name + ".sql")
            ?? throw new InvalidOperationException($"SQL script '{name}' is not an embedded resource.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd().TrimEnd();
    }

    [GeneratedRegex(@"\{\{(\w+)\}\}")]
    private static partial Regex PlaceholderRegex();
}
