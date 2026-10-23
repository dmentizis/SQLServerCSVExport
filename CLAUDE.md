# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A .NET 10 Windows Forms utility (`net10.0-windows`, nullable + implicit usings enabled) that connects to SQL Server and exports, per user table, the primary-key values plus SQL Server `CHECKSUM` of each row to CSV, so two databases' exports can be compared. It is a reconstruction of an older app known only from photos; README.md's "Reconstruction decisions" section records which behaviours were recovered vs. deliberately changed — consult it before "fixing" something that looks odd (e.g. the duplicate `Checksum` header when a PK is named `Checksum`, case-sensitive exact-match exclusions, `_TableData.csv` listing excluded tables too).

## Commands

```powershell
dotnet build SQLServerPersistence.slnx -c Release
dotnet run --project SQLServerCSVExport
```

Naming mismatch to be aware of: the folder and C# namespace are `SQLServerCSVExport`, but the project file is `SQLServerCSVExport/SQLServerPersistence.csproj` and the solution is `SQLServerPersistence.slnx`. README still refers to `SQLServerCSVExport.slnx` and a `SQLServerCSVExport.Tests` project (with `-- preview.png [--integration]`); neither is in the repository, so there are currently no tests to run.

Only package dependency: `Microsoft.Data.SqlClient` 7.1.1 (not `System.Data.SqlClient`; no CsvHelper — CSV writing is hand-rolled in `Helper`).

## Architecture

All code is in `SQLServerCSVExport/`, namespace `SQLServerCSVExport` (models in `SQLServerCSVExport.Objects`).

- `Form1` — UI only. Builds an immutable `Database` record from the inputs, runs operations as async tasks with a `CancellationTokenSource`, and receives progress via `IProgress<ExportProgress>`. Editing connection fields invalidates the checked connection; closing mid-run cancels and waits for cleanup before closing. Layout lives in `Form1.Designer.cs` (layout panels, hand-written).
- `DataAccess` — all metadata SQL. `CreateConnection` always uses `Encrypt = Mandatory` (TrustServerCertificate is opt-in). Lists databases via `master`, loads tables + estimated row counts (sum of heap/clustered-index partitions) + ordered PK column lists, and per-table column metadata (parameterized). Targets SQL Server 2016+.
- `ChecksumQuery.Build` — generates the per-table `SELECT <pk cols sorted ordinally>, CHECKSUM(<all cols>) AS [Checksum] ... ORDER BY <ordinals>`. Identifiers must go through `Quote` (bracket-escaping). text/ntext/xml are cast to `nvarchar(max)`; image/geography/geometry/hierarchyid to `varbinary(max)`.
- `ExportService.ExportAsync` — orchestrates a run: creates a unique `Export_<timestamp>_<guid>` folder, writes `_TableData.csv`, then exports each non-excluded table on its own connection with `SequentialAccess` streaming. Per-table failures are recorded and the run continues; cancellation aborts the whole run. `manifest.json` is always written in `finally` (deliberately without the token) with the final status.
- `Helper` — CSV formatting (invariant culture, ISO round-trip dates, `0x` hex for binary, RFC-style quoting), safe filename generation (`<schema>_<table>_<sha256 prefix>.csv`), exclusion matching.

Output-file invariant: every CSV is written to `<name>.partial` and only renamed to its final name after completing without cancellation. Preserve this pattern (and UTF-8 without BOM, CRLF) in any new writer — a `.partial` file must never be mistaken for a completed export.
