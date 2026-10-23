namespace SqlServerPersistence.Features.ChecksumExport;

public sealed record ExportResult(string Directory, int Exported, int Excluded, int Failed);
