#nullable enable
namespace SqlServerPersistence.UI.Controls;

partial class ConnectionControl
{
    private System.ComponentModel.IContainer? components;
    private GroupBox grpConnection = null!;
    private TextBox txtServer = null!, txtUsername = null!, txtPassword = null!;
    private CheckBox chkWindowsAuthentication = null!, chkTrustServerCertificate = null!;
    private Button btnCheckConnection = null!;
    private Label lblConnectedServer = null!;
    private ComboBox cboDatabases = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        grpConnection = new GroupBox { Name = nameof(grpConnection), Text = "Source", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 9 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < 9; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 9));
        txtServer = new TextBox { Name = nameof(txtServer), Dock = DockStyle.Fill, PlaceholderText = @"Server or server\instance" };
        txtUsername = new TextBox { Name = nameof(txtUsername), Dock = DockStyle.Fill };
        txtPassword = new TextBox { Name = nameof(txtPassword), Dock = DockStyle.Fill, UseSystemPasswordChar = true };
        chkWindowsAuthentication = new CheckBox { Name = nameof(chkWindowsAuthentication), Text = "Use Windows Authentication", AutoSize = true, Checked = true };
        chkTrustServerCertificate = new CheckBox { Name = nameof(chkTrustServerCertificate), Text = "Trust server certificate (self-signed servers)", AutoSize = true };
        btnCheckConnection = ControlFactory.CreateButton("Check Connection", nameof(btnCheckConnection));
        lblConnectedServer = new Label { Name = nameof(lblConnectedServer), Text = "NONE", Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
        cboDatabases = new ComboBox { Name = nameof(cboDatabases), DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        layout.Controls.Add(ControlFactory.CreateLabel("Server"), 0, 0); layout.Controls.Add(txtServer, 1, 0);
        layout.Controls.Add(ControlFactory.CreateLabel("Username"), 0, 1); layout.Controls.Add(txtUsername, 1, 1);
        layout.Controls.Add(ControlFactory.CreateLabel("Password"), 0, 2); layout.Controls.Add(txtPassword, 1, 2);
        layout.Controls.Add(chkWindowsAuthentication, 1, 3);
        layout.Controls.Add(chkTrustServerCertificate, 1, 4);
        layout.Controls.Add(btnCheckConnection, 1, 5);
        layout.Controls.Add(ControlFactory.CreateLabel("Connected:"), 0, 6); layout.Controls.Add(lblConnectedServer, 1, 6);
        layout.Controls.Add(ControlFactory.CreateLabel("Database"), 0, 7); layout.Controls.Add(cboDatabases, 1, 7);
        layout.Controls.Add(new Label { Text = "Select a database after checking the connection.", AutoSize = true }, 1, 8);
        grpConnection.Controls.Add(layout);
        Controls.Add(grpConnection);
        ResumeLayout(true);
    }
}
