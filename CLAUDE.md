# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A .NET 10 Windows Forms utility (`net10.0-windows`, nullable + implicit usings enabled). It connects to SQL Server and, for every user table, exports the primary-key values plus the SQL Server `CHECKSUM` of each row to CSV, so exports of two databases can be compared.

It is a reconstruction of an older app known only from photos. README.md's "Reconstruction decisions" section records which behaviours were recovered and which were deliberately changed. Read it before "fixing" something that looks odd, for example: the duplicate `Checksum` header when a PK is named `Checksum`, case-sensitive exact-match exclusions, or `_TableData.csv` also listing excluded tables. `preview.png` shows the intended window layout.

## Commands

```powershell
dotnet build SqlServerPersistence.slnx -c Release
dotnet run --project SqlServerPersistence
```

There is no test project (README mentions one that was never committed), so verify UI changes by building and running the app. The only package is `Microsoft.Data.SqlClient` 7.1.1 (not `System.Data.SqlClient`; no CsvHelper).

## Code map

Namespaces mirror folders under `SqlServerPersistence/`.

| Path | What it holds |
| --- | --- |
| `Program.cs` | Entry point; just runs `MainForm`. |
| `UI/MainForm` | The only window: Source (a `ConnectionControl`), Exclude Tables, Export and Logger group boxes in a 2×2 `TableLayoutPanel`. All event handlers and the busy/cancel state live in `MainForm.cs`. |
| `UI/ConnectionControl` | Reusable server/credentials/database UserControl. Pure UI: it raises `CheckConnectionClick`, the form runs the query and calls `SetConnected`; editing an input calls `ClearConnection`. `ConnectionChanged` fires on connect, disconnect and database selection. |
| `Features/ChecksumExport/` | `ChecksumExportService` (the export run), `ChecksumQueryBuilder`, `ExportFileNaming`, and the `ExportProgress`/`ExportResult` records. No UI. |
| `Data/` | `SqlConnectionFactory`, `MetadataRepository` (databases, tables + estimated row counts + ordered PKs, columns), `SqlScripts` (loads embedded SQL), `SqlIdentifier`. |
| `Sql/<Feature>/*.sql` | Every SQL statement, compiled as embedded resources. |
| `Csv/` | `CsvFormatter` and `CsvFileWriter`. |
| `Models/` | `ConnectionSettings` (`Database` is the database name), `TableInfo`, `ColumnInfo`, `TableExclusion`. |

### How things work

- **Operations:** `MainForm` runs one long operation at a time (connection check or export) under `_operation` (`BeginOperation` / `CancelOperation` / `EndOperation`). `UpdateControls` derives every enabled state from it. Closing while busy cancels and keeps the window open until `EndOperation` closes it.
- **Export:** `ChecksumExportService.ExportAsync` runs on `Task.Run`. It creates a unique `Export_<timestamp>_<guid>` folder, writes `_TableData.csv`, then exports each non-excluded table on its own connection. A failing table is recorded and the run continues; cancellation aborts the run. `manifest.json` is always written in `finally`, without the cancellation token.
- **SQL:** load scripts with `SqlScripts.Get("Folder.Name")`. Templates use `{{Token}}` placeholders filled by `SqlScripts.Render` in a single pass, so substituted identifiers are never re-scanned. Quote identifiers with `SqlIdentifier.Quote`; pass values as `@PascalCase` parameters. Target SQL Server 2016+.
- **Checksum query:** `ChecksumQueryBuilder` sorts PKs ordinally, casts text/ntext/xml to `nvarchar(max)` and image/spatial/hierarchyid to `varbinary(max)`, and orders by column ordinals.
- **Connections** always use `Encrypt = Mandatory`; `TrustServerCertificate` is opt-in from the UI.
- **CSV output:** always write through `CsvFileWriter`. It writes `<name>.partial` (UTF-8 without BOM, CRLF) and renames only when completed without cancellation, so a `.partial` file never looks like a finished export. `CsvFormatter` uses invariant culture, round-trip dates, `0x` hex for binary and RFC quoting.

## Windows Forms designer rules

Every `*.Designer.cs` must stay in the format the Visual Studio designer generates, or the designer will not open it:

- One field per control (labels and layout panels included), declared at the bottom of the file without initializers.
- `InitializeComponent` creates every control with `new X()` first, then sets properties one statement at a time in `// controlName` sections, then `SuspendLayout`/`ResumeLayout(false)`/`PerformLayout`.
- No object initializers, local variables, loops, lambdas, collection expressions or helper-method calls in `InitializeComponent`. Wire events there as `btnExport.Click += btnExport_Click;` and put the handler in the `.cs` file.
- Do not add `#nullable enable` or `= null!`: `*.Designer.cs` is treated as generated code, so nullable warnings do not apply.
- Controls placed in the designer need a parameterless constructor. Public settable properties on UserControls need `[DefaultValue]` or `[DesignerSerializationVisibility]` (analyzer WFO1000).
- Layout: `AutoScaleMode.Dpi` with `AutoScaleDimensions = new SizeF(96F, 96F)` and Segoe UI 10pt. Use absolute `Location`/`Size` with `Anchor` inside group boxes, and a `TableLayoutPanel` only where columns or rows must share space.

## Conventions

- One type per file, named after the type. Private fields are `_camelCase`. Async methods end in `Async`.
- Control fields use lowercase type prefixes: `txt`, `chk`, `btn`, `cbo`, `lbl`, `lst`, `grp`, `nud`, `prg`, `rtb`, `tlp`. Handlers are named `<control>_<Event>`; a handler shared by several controls is named after what they have in common (e.g. `input_Changed`).
- SQL files use PascalCase verb–noun names, one folder per feature.
- New features: put the logic in `Features/<Name>/` and its SQL in `Sql/<Name>/`, keeping UI out of the service. If the window needs to host several features, move each one's controls into a UserControl and put them on `TabPage`s in `MainForm`.
