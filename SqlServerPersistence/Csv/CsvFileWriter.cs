using System.Text;

namespace SqlServerPersistence.Csv;

/// <summary>
/// Writes UTF-8 (no BOM), CRLF CSV to "&lt;path&gt;.partial" and renames it to the final path only
/// when <see cref="CompleteAsync"/> runs without cancellation, so a .partial file is never a finished export.
/// </summary>
public sealed class CsvFileWriter : IAsyncDisposable
{
    private readonly string _path;
    private readonly StreamWriter _writer;
    private bool _closed;

    public CsvFileWriter(string path)
    {
        _path = path;
        _writer = new StreamWriter(PartialPath(path), false, new UTF8Encoding(false)) { NewLine = "\r\n" };
    }

    public static string PartialPath(string path) => path + ".partial";

    public static async Task WriteAsync(string path, IEnumerable<IEnumerable<object?>> rows, CancellationToken token)
    {
        await using var writer = new CsvFileWriter(path);
        foreach (var row in rows)
        {
            token.ThrowIfCancellationRequested();
            await writer.WriteRowAsync(row, token);
        }
        await writer.CompleteAsync(token);
    }

    public async Task WriteRowAsync(IEnumerable<object?> values, CancellationToken token) =>
        await _writer.WriteLineAsync(CsvFormatter.Row(values).AsMemory(), token);

    public async Task CompleteAsync(CancellationToken token)
    {
        await DisposeAsync();
        token.ThrowIfCancellationRequested();
        File.Move(PartialPath(_path), _path);
    }

    public async ValueTask DisposeAsync()
    {
        if (_closed) return;
        _closed = true;
        await _writer.DisposeAsync();
    }
}
