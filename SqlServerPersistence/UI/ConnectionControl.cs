using System.ComponentModel;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.UI;

/// <summary>
/// Server, credentials and database selection. The control only holds UI state: the owning form
/// runs the connection check when <see cref="CheckConnectionClick"/> fires and reports the result
/// back through <see cref="SetConnected"/>. Editing any input discards the previous connection.
/// </summary>
public partial class ConnectionControl : UserControl
{
    private ConnectionSettings? _connection;

    public ConnectionControl()
    {
        InitializeComponent();
        UpdateControls();
    }

    /// <summary>Raised when the Check Connection button is clicked.</summary>
    public event EventHandler? CheckConnectionClick;

    /// <summary>Raised when the connection is set or discarded, or another database is selected.</summary>
    public event EventHandler? ConnectionChanged;

    [DefaultValue("Source")]
    public string Title
    {
        get => grpConnection.Text;
        set => grpConnection.Text = value;
    }

    /// <summary>Settings of the last successful check, without a database; null when not connected.</summary>
    public ConnectionSettings? Connection => _connection;

    public string? SelectedDatabase => cboDatabases.SelectedItem as string;

    /// <summary>Returns a validation message for the current input, or null when it can be checked.</summary>
    public string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(txtServer.Text)) return "Enter a SQL Server name.";
        if (!chkWindowsAuthentication.Checked && string.IsNullOrWhiteSpace(txtUsername.Text))
            return "Enter a username or select Windows Authentication.";
        return null;
    }

    public ConnectionSettings ReadSettings() => new()
    {
        Server = txtServer.Text.Trim(),
        Username = txtUsername.Text.Trim(),
        Password = txtPassword.Text,
        IntegratedSecurity = chkWindowsAuthentication.Checked,
        TrustServerCertificate = chkTrustServerCertificate.Checked
    };

    public void SetConnected(ConnectionSettings connection, IEnumerable<string> databases)
    {
        _connection = connection;
        cboDatabases.Items.Clear();
        cboDatabases.Items.AddRange(databases.ToArray<object>());
        lblConnectedServer.Text = connection.Server;
        UpdateControls();
        ConnectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearConnection()
    {
        _connection = null;
        cboDatabases.Items.Clear();
        lblConnectedServer.Text = "NONE";
        UpdateControls();
        ConnectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateControls()
    {
        txtUsername.Enabled = !chkWindowsAuthentication.Checked;
        txtPassword.Enabled = !chkWindowsAuthentication.Checked;
        cboDatabases.Enabled = _connection is not null;
    }

    private void btnCheckConnection_Click(object? sender, EventArgs e) =>
        CheckConnectionClick?.Invoke(this, EventArgs.Empty);

    // Shared by every server/credential input.
    private void input_Changed(object? sender, EventArgs e) => ClearConnection();

    private void cboDatabases_SelectedIndexChanged(object? sender, EventArgs e) =>
        ConnectionChanged?.Invoke(this, EventArgs.Empty);
}
