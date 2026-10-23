using SQLServerCSVExport.Objects;

namespace SQLServerCSVExport;

public partial class Form1 : Form
{
    private Database? _database;
    private string? _exportPath;
    private CancellationTokenSource? _operation;
    private readonly List<ExclusionTable> _exclusions = [];
    private bool _closeAfterCancel;

    public Form1()
    {
        InitializeComponent();
        BtnConnect.Click += CheckConnection_Click;
        BtnChoose.Click += ChooseExportLocation_Click;
        BtnExport.Click += Export_Click;
        BtnCancel.Click += (_, _) => CancelOperation();
        BtnAdd.Click += AddExclusion_Click;
        BtnClear.Click += (_, _) => { _exclusions.Clear(); RefreshExclusions(); };
        BtnRemove.Click += (_, _) =>
        {
            if (lbExclusions.SelectedItem is ExclusionTable item) _exclusions.Remove(item);
            RefreshExclusions();
        };
        foreach (var text in new[] { txtSourceServer, txtSourceUsername, txtSourcePassword })
            text.TextChanged += (_, _) => InvalidateConnection();
        chkWindows.CheckedChanged += (_, _) => InvalidateConnection();
        chkTrustCertificate.CheckedChanged += (_, _) => InvalidateConnection();
        cbDatabases.SelectedIndexChanged += (_, _) => UpdateControls();
        FormClosing += (_, e) =>
        {
            if (_operation is null) return;
            e.Cancel = true;
            _closeAfterCancel = true;
            CancelOperation();
        };
        UpdateControls();
    }

    private void InvalidateConnection()
    {
        _database = null;
        cbDatabases.Items.Clear();
        lblConnected.Text = "NONE";
        UpdateControls();
    }

    private void UpdateControls()
    {
        var busy = _operation is not null;
        sourceGroup.Enabled = !busy;
        exclusionGroup.Enabled = !busy;
        txtSourceUsername.Enabled = !chkWindows.Checked;
        txtSourcePassword.Enabled = !chkWindows.Checked;
        cbDatabases.Enabled = _database is not null;
        BtnChoose.Enabled = !busy;
        timeout.Enabled = !busy;
        BtnExport.Enabled = !busy && _database is not null && cbDatabases.SelectedItem is string && _exportPath is not null;
        BtnCancel.Enabled = busy && !_operation!.IsCancellationRequested;
    }

    private void Log(string message)
    {
        if (logger.TextLength > 200_000) logger.Clear();
        logger.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        logger.SelectionStart = logger.TextLength;
        logger.ScrollToCaret();
    }

    private async void CheckConnection_Click(object? sender, EventArgs e)
    {
        if (_operation is not null) return;
        if (string.IsNullOrWhiteSpace(txtSourceServer.Text)) { ShowError("Enter a SQL Server name."); return; }
        if (!chkWindows.Checked && string.IsNullOrWhiteSpace(txtSourceUsername.Text))
        { ShowError("Enter a username or select Windows Authentication."); return; }
        var candidate = new Database
        {
            Server = txtSourceServer.Text.Trim(), Username = txtSourceUsername.Text.Trim(), Password = txtSourcePassword.Text,
            IntegratedSecurity = chkWindows.Checked, TrustServerCertificate = chkTrustCertificate.Checked
        };
        InvalidateConnection();
        _operation = new CancellationTokenSource();
        UpdateControls();
        try
        {
            Log($"Connecting to {candidate.Server}…");
            var databases = await DataAccess.GetDatabasesAsync(candidate, _operation.Token);
            _operation.Token.ThrowIfCancellationRequested();
            _database = candidate;
            cbDatabases.Items.AddRange(databases.Cast<object>().ToArray());
            lblConnected.Text = candidate.Server;
            Log($"Connected. {databases.Count} accessible databases found. Select a database to export.");
        }
        catch (Exception) when (_operation.IsCancellationRequested) { Log("Connection cancelled."); }
        catch (Exception ex) { Log("Connection failed: " + ex.Message); }
        finally { FinishOperation(); }
    }

    private void ChooseExportLocation_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog { Description = "Choose a folder for CSV exports", UseDescriptionForTitle = true };
        if (_exportPath is not null) dialog.SelectedPath = _exportPath;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _exportPath = dialog.SelectedPath;
        lblPath.Text = _exportPath;
        UpdateControls();
    }

    private async void Export_Click(object? sender, EventArgs e)
    {
        if (_operation is not null || _database is null || cbDatabases.SelectedItem is not string name || _exportPath is null) return;
        var database = _database with { Name = name, CommandTimeout = (int)timeout.Value };
        var exclusions = _exclusions.ToArray();
        var destination = _exportPath;
        _operation = new CancellationTokenSource();
        var token = _operation.Token;
        progressBar.Value = 0;
        UpdateControls();
        Log("##### EXPORT STARTED #####");
        var progress = new Progress<ExportProgress>(p => { Log(p.Message); progressBar.Value = Math.Clamp(p.Percent, 0, 100); });
        try
        {
            var result = await Task.Run(() => new ExportService().ExportAsync(database, destination, exclusions, progress, token), token);
            progressBar.Value = 100;
            Log($"##### {(result.Failed == 0 ? "EXPORT COMPLETED" : "EXPORT COMPLETED WITH ERRORS")} #####");
            Log($"Exported: {result.Exported}; excluded: {result.Excluded}; failed: {result.Failed}.");
            Log($"Files: {result.Directory}");
        }
        catch (OperationCanceledException) { Log("##### EXPORT CANCELLED — completed files retained; incomplete files have .partial suffix #####"); }
        catch (Exception ex) { Log("##### EXPORT FAILED ##### " + ex.Message); }
        finally { FinishOperation(); }
    }

    private void CancelOperation()
    {
        if (_operation is null || _operation.IsCancellationRequested) return;
        Log("Cancelling…");
        _operation.Cancel();
        UpdateControls();
    }

    private void FinishOperation()
    {
        _operation?.Dispose();
        _operation = null;
        UpdateControls();
        if (_closeAfterCancel) Close();
    }

    private void AddExclusion_Click(object? sender, EventArgs e)
    {
        var schema = TxtExcludeSchema.Text.Trim();
        var table = TxtExcludeTable.Text.Trim();
        if (schema.Length == 0 || table.Length == 0) { ShowError("Enter both a schema and a table name."); return; }
        if (!Helper.IsInExclusionList(schema, table, _exclusions)) _exclusions.Add(new(schema, table));
        RefreshExclusions();
        TxtExcludeTable.Clear();
        TxtExcludeTable.Focus();
    }

    private void RefreshExclusions()
    {
        lbExclusions.Items.Clear();
        lbExclusions.Items.AddRange(_exclusions.Cast<object>().ToArray());
    }

    private void ShowError(string message) => MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
}
