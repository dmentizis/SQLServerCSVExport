using SqlServerPersistence.Data;
using SqlServerPersistence.Models;
using SqlServerPersistence.UI;

namespace SqlServerPersistence.Features.ChecksumExport;

public partial class ChecksumExportTab : UserControl, IFeatureTab
{
    private readonly MetadataRepository _metadata;
    private readonly ChecksumExportService _exportService;
    private readonly ILogSink _log;
    private readonly List<TableExclusion> _exclusions = [];
    private string? _exportPath;
    private CancellationTokenSource? _operation;

    public ChecksumExportTab(MetadataRepository metadata, ChecksumExportService exportService, ILogSink log)
    {
        _metadata = metadata;
        _exportService = exportService;
        _log = log;
        InitializeComponent();
        connectionControl.CheckConnectionRequested += connectionControl_CheckConnectionRequested;
        connectionControl.ConnectionInvalidated += (_, _) => UpdateControls();
        connectionControl.DatabaseSelectionChanged += (_, _) => UpdateControls();
        btnChooseExportLocation.Click += btnChooseExportLocation_Click;
        btnExport.Click += btnExport_Click;
        btnCancel.Click += (_, _) => CancelOperation();
        btnAddExclusion.Click += btnAddExclusion_Click;
        btnClearExclusions.Click += (_, _) => { _exclusions.Clear(); RefreshExclusions(); };
        btnRemoveExclusion.Click += (_, _) =>
        {
            if (lstExclusions.SelectedItem is TableExclusion item) _exclusions.Remove(item);
            RefreshExclusions();
        };
        UpdateControls();
    }

    public string Title => "Checksum Export";

    public bool IsBusy => _operation is not null;

    public event EventHandler? OperationFinished;

    public void CancelOperation()
    {
        if (_operation is null || _operation.IsCancellationRequested) return;
        _log.Log("Cancelling…");
        _operation.Cancel();
        UpdateControls();
    }

    private void UpdateControls()
    {
        var busy = _operation is not null;
        connectionControl.SetBusy(busy);
        grpExclusions.Enabled = !busy;
        btnChooseExportLocation.Enabled = !busy;
        nudQueryTimeout.Enabled = !busy;
        btnExport.Enabled = !busy && connectionControl.Connection is not null && connectionControl.SelectedDatabase is not null && _exportPath is not null;
        btnCancel.Enabled = busy && !_operation!.IsCancellationRequested;
    }

    private CancellationToken BeginOperation()
    {
        _operation = new CancellationTokenSource();
        UpdateControls();
        return _operation.Token;
    }

    private void FinishOperation()
    {
        _operation?.Dispose();
        _operation = null;
        UpdateControls();
        OperationFinished?.Invoke(this, EventArgs.Empty);
    }

    private async void connectionControl_CheckConnectionRequested(object? sender, EventArgs e)
    {
        if (_operation is not null) return;
        if (connectionControl.ValidateInput() is { } error) { ShowError(error); return; }
        var token = BeginOperation();
        try { await connectionControl.ConnectAsync(_metadata, _log, token); }
        finally { FinishOperation(); }
    }

    private void btnChooseExportLocation_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog { Description = "Choose a folder for CSV exports", UseDescriptionForTitle = true };
        if (_exportPath is not null) dialog.SelectedPath = _exportPath;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _exportPath = dialog.SelectedPath;
        lblExportPath.Text = _exportPath;
        UpdateControls();
    }

    private async void btnExport_Click(object? sender, EventArgs e)
    {
        if (_operation is not null || connectionControl.Connection is not { } connection
            || connectionControl.SelectedDatabase is not { } databaseName || _exportPath is null) return;
        var settings = connection with { Name = databaseName, CommandTimeout = (int)nudQueryTimeout.Value };
        var exclusions = _exclusions.ToArray();
        var destination = _exportPath;
        var token = BeginOperation();
        prgExport.Value = 0;
        _log.Log("##### EXPORT STARTED #####");
        var progress = new Progress<ExportProgress>(p => { _log.Log(p.Message); prgExport.Value = Math.Clamp(p.Percent, 0, 100); });
        try
        {
            var result = await Task.Run(() => _exportService.ExportAsync(settings, destination, exclusions, progress, token), token);
            prgExport.Value = 100;
            _log.Log($"##### {(result.Failed == 0 ? "EXPORT COMPLETED" : "EXPORT COMPLETED WITH ERRORS")} #####");
            _log.Log($"Exported: {result.Exported}; excluded: {result.Excluded}; failed: {result.Failed}.");
            _log.Log($"Files: {result.Directory}");
        }
        catch (OperationCanceledException) { _log.Log("##### EXPORT CANCELLED — completed files retained; incomplete files have .partial suffix #####"); }
        catch (Exception ex) { _log.Log("##### EXPORT FAILED ##### " + ex.Message); }
        finally { FinishOperation(); }
    }

    private void btnAddExclusion_Click(object? sender, EventArgs e)
    {
        var schema = txtExcludeSchema.Text.Trim();
        var table = txtExcludeTable.Text.Trim();
        if (schema.Length == 0 || table.Length == 0) { ShowError("Enter both a schema and a table name."); return; }
        if (!_exclusions.Any(x => x.Matches(schema, table))) _exclusions.Add(new(schema, table));
        RefreshExclusions();
        txtExcludeTable.Clear();
        txtExcludeTable.Focus();
    }

    private void RefreshExclusions()
    {
        lstExclusions.Items.Clear();
        lstExclusions.Items.AddRange(_exclusions.Cast<object>().ToArray());
    }

    private void ShowError(string message) =>
        MessageBox.Show(this, message, FindForm()?.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
