using SqlServerPersistence.Data;
using SqlServerPersistence.Features.ChecksumExport;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.UI;

/// <summary>
/// The application window: connect to a server, pick a database, exclude tables and export each
/// table's primary keys and row checksums to CSV.
/// </summary>
/// <remarks>
/// One long-running operation (connection check or export) runs at a time, tracked by
/// <see cref="_operation"/>. Cancel cancels it; closing the window cancels it and closes once it
/// has finished cleaning up.
/// </remarks>
public partial class MainForm : Form
{
    private readonly MetadataRepository _metadata = new();
    private readonly ChecksumExportService _exportService;
    private string? _exportPath;
    private CancellationTokenSource? _operation;
    private bool _closeWhenFinished;

    public MainForm()
    {
        InitializeComponent();
        _exportService = new ChecksumExportService(_metadata);
        UpdateControls();
    }

    private bool IsBusy => _operation is not null;

    // ---- Connection ----

    private async void connectionControl_CheckConnectionClick(object? sender, EventArgs e)
    {
        if (IsBusy) return;
        if (connectionControl.ValidateInput() is { } error)
        {
            ShowMessage(error);
            return;
        }

        var settings = connectionControl.ReadSettings();
        connectionControl.ClearConnection();
        var token = BeginOperation();
        try
        {
            Log($"Connecting to {settings.Server}…");
            var databases = await _metadata.GetDatabasesAsync(settings, token);
            token.ThrowIfCancellationRequested();
            connectionControl.SetConnected(settings, databases);
            Log($"Connected. {databases.Count} accessible databases found. Select a database to export.");
        }
        catch (Exception) when (token.IsCancellationRequested) { Log("Connection cancelled."); }
        catch (Exception ex) { Log("Connection failed: " + ex.Message); }
        finally { EndOperation(); }
    }

    private void connectionControl_ConnectionChanged(object? sender, EventArgs e) => UpdateControls();

    // ---- Exclusions (the list box items are the TableExclusion records) ----

    private void btnAddExclusion_Click(object? sender, EventArgs e)
    {
        var schema = txtExcludeSchema.Text.Trim();
        var table = txtExcludeTable.Text.Trim();
        if (schema.Length == 0 || table.Length == 0)
        {
            ShowMessage("Enter both a schema and a table name.");
            return;
        }

        var exclusion = new TableExclusion(schema, table);
        if (!lstExclusions.Items.Contains(exclusion)) lstExclusions.Items.Add(exclusion);
        txtExcludeTable.Clear();
        txtExcludeTable.Focus();
    }

    private void btnRemoveExclusion_Click(object? sender, EventArgs e)
    {
        if (lstExclusions.SelectedItem is { } item) lstExclusions.Items.Remove(item);
    }

    private void btnClearExclusions_Click(object? sender, EventArgs e) => lstExclusions.Items.Clear();

    // ---- Export ----

    private void btnChooseExportLocation_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog { Description = "Choose a folder for CSV exports", UseDescriptionForTitle = true };
        if (_exportPath is not null) dialog.SelectedPath = _exportPath;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _exportPath = dialog.SelectedPath;
        lblExportPath.Text = "Export location: " + _exportPath;
        UpdateControls();
    }

    private async void btnExport_Click(object? sender, EventArgs e)
    {
        if (IsBusy || connectionControl.Connection is not { } connection
            || connectionControl.SelectedDatabase is not { } database || _exportPath is null) return;

        // Capture everything the background task needs before it starts.
        var settings = connection with { Database = database, CommandTimeout = (int)nudQueryTimeout.Value };
        var exclusions = lstExclusions.Items.Cast<TableExclusion>().ToArray();
        var destination = _exportPath;
        var progress = new Progress<ExportProgress>(p =>
        {
            Log(p.Message);
            prgExport.Value = Math.Clamp(p.Percent, 0, 100);
        });

        var token = BeginOperation();
        prgExport.Value = 0;
        Log("##### EXPORT STARTED #####");
        try
        {
            var result = await Task.Run(() => _exportService.ExportAsync(settings, destination, exclusions, progress, token), token);
            prgExport.Value = 100;
            Log($"##### {(result.Failed == 0 ? "EXPORT COMPLETED" : "EXPORT COMPLETED WITH ERRORS")} #####");
            Log($"Exported: {result.Exported}; excluded: {result.Excluded}; failed: {result.Failed}.");
            Log($"Files: {result.Directory}");
        }
        catch (OperationCanceledException) { Log("##### EXPORT CANCELLED — completed files retained; incomplete files have .partial suffix #####"); }
        catch (Exception ex) { Log("##### EXPORT FAILED ##### " + ex.Message); }
        finally { EndOperation(); }
    }

    private void btnCancel_Click(object? sender, EventArgs e) => CancelOperation();

    // ---- Closing ----

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!IsBusy) return;
        // Keep the window open until the running operation has cleaned up; EndOperation closes it.
        e.Cancel = true;
        _closeWhenFinished = true;
        CancelOperation();
    }

    // ---- Operation state ----

    private CancellationToken BeginOperation()
    {
        _operation = new CancellationTokenSource();
        UpdateControls();
        return _operation.Token;
    }

    private void CancelOperation()
    {
        if (_operation is null || _operation.IsCancellationRequested) return;
        Log("Cancelling…");
        _operation.Cancel();
        UpdateControls();
    }

    private void EndOperation()
    {
        _operation?.Dispose();
        _operation = null;
        UpdateControls();
        if (_closeWhenFinished) Close();
    }

    private void UpdateControls()
    {
        var busy = IsBusy;
        connectionControl.Enabled = !busy;
        grpExclusions.Enabled = !busy;
        btnChooseExportLocation.Enabled = !busy;
        nudQueryTimeout.Enabled = !busy;
        btnExport.Enabled = !busy && connectionControl.Connection is not null
            && connectionControl.SelectedDatabase is not null && _exportPath is not null;
        btnCancel.Enabled = busy && !_operation!.IsCancellationRequested;
    }

    // ---- Helpers ----

    private void Log(string message)
    {
        if (rtbLog.TextLength > 200_000) rtbLog.Clear();
        rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        rtbLog.SelectionStart = rtbLog.TextLength;
        rtbLog.ScrollToCaret();
    }

    private void ShowMessage(string message) =>
        MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
