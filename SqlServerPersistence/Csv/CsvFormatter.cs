using System.Globalization;

namespace SqlServerPersistence.Csv;

public static class CsvFormatter
{
    public static string Field(object? value)
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

    public static string Row(IEnumerable<object?> values) => string.Join(",", values.Select(Field));
}
