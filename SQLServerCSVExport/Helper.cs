using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SQLServerCSVExport.Objects;

namespace SQLServerCSVExport;

public static class Helper
{
    public static bool IsInExclusionList(string schema, string table, IEnumerable<ExclusionTable> exclusions) =>
        exclusions.Any(e => e.Schema == schema && e.Table == table);

    public static string CsvField(object? value)
    {
        var text = value switch
        {
            null or DBNull => "",
            byte[] bytes => "0x" + Convert.ToHexString(bytes),
            DateTime date => date.ToString("O", CultureInfo.InvariantCulture),
            DateTimeOffset date => date.ToString("O", CultureInfo.InvariantCulture),
            IFormattable formatted => formatted.ToString(null, CultureInfo.InvariantCulture) ?? "",
            _ => value.ToString() ?? ""
        };
        return text.IndexOfAny([',', '"', '\r', '\n']) >= 0 ? "\"" + text.Replace("\"", "\"\"") + "\"" : text;
    }

    public static string CsvRow(IEnumerable<object?> values) => string.Join(",", values.Select(CsvField));

    public static string TableFileName(Table table)
    {
        // Keep the original schema_table.csv convention where possible. A stable suffix
        // prevents collisions from underscores, sanitization, case and long identifiers.
        var raw = table.Schema + "_" + table.Name;
        var safe = new string(raw.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c).ToArray());
        if (safe.Length > 100) safe = safe[..100];
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(table.Schema + "\0" + table.Name)))[..16];
        return $"{safe.TrimEnd('.', ' ')}_{hash}.csv";
    }

    public static async Task WriteCsvAsync(string path, IEnumerable<IEnumerable<object?>> rows, CancellationToken token)
    {
        var partial = path + ".partial";
        await using (var writer = new StreamWriter(partial, false, new UTF8Encoding(false)))
        {
            writer.NewLine = "\r\n";
            foreach (var row in rows)
            {
                token.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(CsvRow(row).AsMemory(), token);
            }
        }
        token.ThrowIfCancellationRequested();
        File.Move(partial, path);
    }
}
