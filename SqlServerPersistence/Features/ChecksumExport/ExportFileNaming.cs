using System.Security.Cryptography;
using System.Text;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.Features.ChecksumExport;

public static class ExportFileNaming
{
    public static string TableFileName(TableInfo table)
    {
        // Keep the original schema_table.csv convention where possible. A stable suffix
        // prevents collisions from underscores, sanitization, case and long identifiers.
        var raw = table.Schema + "_" + table.Name;
        var safe = new string(raw.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c).ToArray());
        if (safe.Length > 100) safe = safe[..100];
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(table.Schema + "\0" + table.Name)))[..16];
        return $"{safe.TrimEnd('.', ' ')}_{hash}.csv";
    }
}
