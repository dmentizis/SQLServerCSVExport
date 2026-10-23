# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A .NET 10 Windows Forms utility (`net10.0-windows`, nullable + implicit usings enabled) that connects to SQL Server and exports, per user table, the primary-key values plus SQL Server `CHECKSUM` of each row to CSV, so two databases' exports can be compared. It is a reconstruction of an older app known only from photos; README.md's "Reconstruction decisions" section records which behaviours were recovered vs. deliberately changed — consult it before "fixing" something that looks odd (e.g. the duplicate `Checksum` header when a PK is named `Checksum`, case-sensitive exact-match exclusions, `_TableData.csv` listing excluded tables too).

## Commands

```powershell
dotnet build SqlServerPersistence.slnx -c Release
dotnet run --project SqlServerPersistence
```

There is no test project in the repository (README describes one that was never committed). Only package dependency: `Microsoft.Data.SqlClient` 7.1.1 (not `System.Data.SqlClient`; no CsvHelper).

## Architecture

Single project `SqlServerPersistence/`, organised by folder; namespaces mirror folders (`SqlServerPersistence.Models`, `.Data`, `.Csv`, `.Features.ChecksumExport`, `.UI`, `.UI.Controls`).

- **Extending:** features are tabs. `Program.cs` is the composition root: it creates services (plain classes, constructor-injected, no DI container) and calls `MainForm.AddTab` for each `UserControl` implementing `UI/IFeatureTab`. `MainForm` is only a tab host plus the shared `LogControl` (`ILogSink`); on close it cancels busy tabs and closes after each raises `OperationFinished`. A new feature (e.g. schema comparison) = `Features/<Name>/` (service + tab) + `Sql/<Name>/*.sql` + one `AddTab` line. `UI/Controls/ConnectionControl` is reusable (settable `Title`, e.g. two instances for source/target); the owning tab runs its `ConnectAsync` under the tab's own `CancellationTokenSource` so Cancel also interrupts connection checks.
- **SQL lives only in `Sql/**/*.sql`**, compiled as embedded resources. Load via `SqlScripts.Get("Folder.Name")`; templates use `{{Token}}` placeholders filled by `SqlScripts.Render` in a single pass (so substituted identifiers are never re-scanned). Identifiers must go through `SqlIdentifier.Quote`; values go through parameters (`@PascalCase`).
- `Data/` — `SqlConnectionFactory.Create` always uses `Encrypt = Mandatory` (TrustServerCertificate opt-in). `MetadataRepository` lists databases (via `master`), tables + estimated row counts + ordered PK columns, and per-table columns. Targets SQL Server 2016+.
- `Features/ChecksumExport/` — `ChecksumQueryBuilder` renders `SelectRowChecksums.sql` (PKs sorted ordinally, text/ntext/xml → `nvarchar(max)`, image/spatial/hierarchyid → `varbinary(max)`, `ORDER BY` ordinals). `ChecksumExportService` creates a unique `Export_<timestamp>_<guid>` folder, writes `_TableData.csv`, streams each non-excluded table on its own connection; per-table failures are recorded and the run continues, cancellation aborts it, and `manifest.json` is always written in `finally` without the token.
- `Csv/` — `CsvFormatter` (invariant culture, round-trip dates, `0x` hex, RFC quoting) and `CsvFileWriter`, which owns the output invariant: write to `<name>.partial` (UTF-8 without BOM, CRLF) and rename only on completion without cancellation. Use it for any new CSV output — a `.partial` file must never look like a completed export.

## Conventions

- One type per file, file named after the type; private fields `_camelCase`; async methods end in `Async`.
- UI is built in hand-written `*.Designer.cs` files (layout panels, `#nullable enable` at the top, `AutoScaleMode.Dpi`). Control fields use lowercase type prefixes — `txt`, `chk`, `btn`, `cbo`, `lbl`, `lst`, `grp`, `nud`, `prg`, `rtb`, `tab` — and handlers are `<control>_<Event>`. Shared styling helpers are in `UI/Controls/ControlFactory`.
- SQL files are PascalCase verb–noun names in a folder per feature.
