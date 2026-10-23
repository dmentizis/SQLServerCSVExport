# SQL Server Persistence

Reconstruction of the Windows Forms `DatabaseCompare_Utility` application photographed in `Utilities_Solutions`, implemented as the SqlServerPersistence solution. Targets .NET 10 for Windows.

## Run

Open `SqlServerPersistence.slnx` in Visual Studio with the .NET desktop development workload and choose SqlServerPersistence as the startup project, or run:

```powershell
dotnet run --project SqlServerPersistence
```

A Release build is available at `SqlServerPersistence/bin/Release/net10.0-windows/SqlServerPersistence.exe`. Keep the accompanying DLLs and runtime files beside the executable. Requires the .NET 10 Windows Desktop Runtime.

## Project structure

```
SqlServerPersistence/
  Program.cs                 entry point: runs MainForm
  UI/                        MainForm (the whole window) and the reusable ConnectionControl
  Features/ChecksumExport/   export service, checksum query builder and file naming (no UI)
  Data/                      connection factory, identifier quoting, metadata repository, SqlScripts loader
  Sql/                       every SQL statement as an embedded .sql file (Metadata/, ChecksumExport/)
  Csv/                       CSV formatting and the .partial-then-rename file writer
  Models/                    connection settings, table/column metadata, exclusions
```

All forms and controls open in the Visual Studio Windows Forms designer. A new feature goes in its own `Features/<Name>/` folder, with its SQL under `Sql/<Name>/`.

1. Enter a server name (for example `localhost` or `server\instance`).
2. Choose Windows Authentication or enter SQL credentials.
3. Click **Check Connection**, then select a database.
4. Optionally add exact schema/table names to **Exclude Tables**. Matching is case-sensitive, as in the photos; no wildcard syntax is implied.
5. Choose an export folder and click **Export**. **Cancel** also interrupts active database reads. Closing during a run requests cancellation and waits for cleanup.

Connections are encrypted. For a known development server with a self-signed certificate, explicitly select **Trust server certificate** if certificate validation fails. Credentials are held only in memory and never saved to the export manifest. The account needs database access, metadata visibility and SELECT permission on exported tables. The reconstructed metadata query targets SQL Server 2016 or newer.

## Output

Each run creates a unique `Export_<timestamp>_<id>` subfolder:

- `_TableData.csv`: `Schema,Name,RowCount` for every visible user table, including excluded tables, matching the photographed summary behavior. Row counts are metadata estimates from heap/clustered-index partitions.
- `<schema>_<table>_<stable-suffix>.csv`: primary-key fields followed by SQL Server's integer `Checksum` for every row. Composite keys are sorted by name; rows are ordered by these keys and checksum. Tables without keys export sorted checksums, preserving duplicate rows. Empty tables still have a header.
- `manifest.json`: server/database identity, overall result and each table's file/status/error. Use it to map filenames to exact SQL names. The suffix avoids collisions and invalid Windows filenames.
- `*.partial`: unfinished output from cancellation or failure; never treat these as completed exports.

CSV uses UTF-8 without BOM, comma delimiters, CRLF records and standard quoted-field escaping. Numbers use invariant culture; date/time keys use round-trip formatting and binary keys use hexadecimal. A primary key named `Checksum` results in two columns with that header, preserving the photographed checksum header; their positions distinguish them.

This is a checksum exporter, not a database synchronization tool. SQL Server `CHECKSUM` has collisions and can ignore some textual differences; equal checksums do not prove data equality. Compare exports from equivalent schemas, column order and collations. Reads use the server's normal isolation level, so a changing database is not a single consistent snapshot. For comparisons, use quiescent databases or independently prepared snapshots. Exported key values are literal CSV data; import them as text when using spreadsheet software.

## Reconstruction decisions

Reviewed all 59 photos. The 34 files from `20240612_202412.jpg` through `20240612_202940.jpg` belong to this WinForms utility. The 25 later photos show an unrelated `5_7_Login` Blazor project and are not part of this application.

Recovered from photos:

- Database, table, column and exclusion models.
- Source connection controls, database selection, export folder, exclusions and logger; the layout follows `20240612_202531.jpg`.
- Table summary CSV mapping and per-row `CHECKSUM` query, including primary keys and conversions for text/XML/binary/spatial values.
- Background export, per-table status messages and cancellation workflow.

Reconstructed or corrected:

- The connection-check and folder-selection handlers were collapsed; the column-metadata query was never expanded. These are implemented from the visible callers and data models.
- The designer source and project dependencies were not photographed. The `*.Designer.cs` files were rebuilt in standard Visual Studio designer format, following the photographed layout (Source, Exclude Tables, Export and Logger in a 2×2 grid; see `preview.png`).
- Uses Microsoft.Data.SqlClient 7.1.1 instead of the older System.Data.SqlClient API, and a small tested CSV writer instead of CsvHelper. Package reference: https://www.nuget.org/packages/Microsoft.Data.SqlClient/7.1.1
- Uses async tasks and cancellation tokens instead of BackgroundWorker, and streams query results instead of buffering entire tables in a DataTable.
- Fixes partition row counts (SUM of base-table partitions, not MAX across indexes), escapes SQL identifiers, parameterizes metadata lookup, preserves complete binary values with `varbinary(max)`, and keeps primary keys as a list instead of splitting names on semicolons.
- Adds validation, stale-connection invalidation after credential edits, certificate options, query timeout, progress, exclusion removal and distinct failure/cancellation/completion states.
- Uses unique run folders, safe filenames, a manifest and partial-file promotion to avoid overwriting earlier exports or reporting incomplete CSV files as complete.

These repairs intentionally change some outputs from the photographed implementation (especially binary values beyond its implicit cast length). Byte-for-byte compatibility with the old buggy implementation is not promised.

## Verification

```powershell
dotnet build SqlServerPersistence.slnx -c Release
```

The repository does not contain a test project. The `SQLServerCSVExport.Tests` project used during reconstruction (27 checks for CSV formatting, special identifiers, composite/keyless queries, filename safety, file completion/cancellation, connection settings and UI state, plus an optional `--integration` run against a temporary `localhost` database) was never committed. Live database export behavior remains unverified.
