using System.ComponentModel;
using SqlServerPersistence.Data;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.UI.Controls;

/// <summary>
/// Server, credentials and database selection. The owning tab runs <see cref="ConnectAsync"/>
/// under its own operation token, so its Cancel button also interrupts a connection check.
/// </summary>
public partial class ConnectionControl : UserControl
{
    private ConnectionSettings? _connection;
    private bool _busy;

    public ConnectionControl()
    {
        InitializeComponent();
        btnCheckConnection.Click += (_, _) => CheckConnectionRequested?.Invoke(this, EventArgs.Empty);
        foreach (var text in new[] { txtServer, txtUsername, txtPassword })
            text.TextChanged += (_, _) => ResetConnection();
        chkWindowsAuthentication.CheckedChanged += (_, _) => ResetConnection();
        chkTrustServerCertificate.CheckedChanged += (_, _) => ResetConnection();
        cboDatabases.SelectedIndexChanged += (_, _) => DatabaseSelectionChanged?.Invoke(this, EventArgs.Empty);
        UpdateControls();
    }

    public event EventHandler? CheckConnectionRequested;
    public event EventHandler? ConnectionInvalidated;
    public event EventHandler? DatabaseSelectionChanged;

    [DefaultValue("Source")]
    public string Title
    {
        get => grpConnection.Text;
        set => grpConnection.Text = value;
    }

    /// <summary>Server-level settings of the last successful check; <see cref="ConnectionSettings.Name"/> is empty.</summary>
    public ConnectionSettings? Connection => _connection;

    public string? SelectedDatabase => cboDatabases.SelectedItem as string;

    public void SetBusy(bool busy)
    {
        _busy = busy;
        UpdateControls();
    }

    /// <summary>Returns a validation message for the current input, or null when it can be checked.</summary>
    public string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtServer.Text)) return "Enter a SQL Server name.";
        if (!chkWindowsAuthentication.Checked && string.IsNullOrWhiteSpace(txtUsername.Text))
            return "Enter a username or select Windows Authentication.";
        return null;
    }

    public async Task ConnectAsync(MetadataRepository metadata, ILogSink log, CancellationToken token)
    {
        var candidate = new ConnectionSettings
        {
            Server = txtServer.Text.Trim(), Username = txtUsername.Text.Trim(), Password = txtPassword.Text,
            IntegratedSecurity = chkWindowsAuthentication.Checked, TrustServerCertificate = chkTrustServerCertificate.Checked
        };
        ResetConnection();
        try
        {
            log.Log($"Connecting to {candidate.Server}…");
            var databases = await metadata.GetDatabasesAsync(candidate, token);
            token.ThrowIfCancellationRequested();
            _connection = candidate;
            cboDatabases.Items.AddRange(databases.Cast<object>().ToArray());
            lblConnectedServer.Text = candidate.Server;
            log.Log($"Connected. {databases.Count} accessible databases found. Select a database to export.");
        }
        catch (Exception) when (token.IsCancellationRequested) { log.Log("Connection cancelled."); }
        catch (Exception ex) { log.Log("Connection failed: " + ex.Message); }
        finally { UpdateControls(); }
    }

    private void ResetConnection()
    {
        _connection = null;
        cboDatabases.Items.Clear();
        lblConnectedServer.Text = "NONE";
        UpdateControls();
        ConnectionInvalidated?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateControls()
    {
        grpConnection.Enabled = !_busy;
        txtUsername.Enabled = !chkWindowsAuthentication.Checked;
        txtPassword.Enabled = !chkWindowsAuthentication.Checked;
        cboDatabases.Enabled = _connection is not null;
    }
}
