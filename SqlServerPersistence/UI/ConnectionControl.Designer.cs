namespace SqlServerPersistence.UI;

partial class ConnectionControl
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        grpConnection = new GroupBox();
        lblServer = new Label();
        txtServer = new TextBox();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        chkWindowsAuthentication = new CheckBox();
        chkTrustServerCertificate = new CheckBox();
        btnCheckConnection = new Button();
        lblConnected = new Label();
        lblConnectedServer = new Label();
        lblDatabase = new Label();
        cboDatabases = new ComboBox();
        lblDatabaseHint = new Label();
        grpConnection.SuspendLayout();
        SuspendLayout();
        //
        // grpConnection
        //
        grpConnection.Controls.Add(lblServer);
        grpConnection.Controls.Add(txtServer);
        grpConnection.Controls.Add(lblUsername);
        grpConnection.Controls.Add(txtUsername);
        grpConnection.Controls.Add(lblPassword);
        grpConnection.Controls.Add(txtPassword);
        grpConnection.Controls.Add(chkWindowsAuthentication);
        grpConnection.Controls.Add(chkTrustServerCertificate);
        grpConnection.Controls.Add(btnCheckConnection);
        grpConnection.Controls.Add(lblConnected);
        grpConnection.Controls.Add(lblConnectedServer);
        grpConnection.Controls.Add(lblDatabase);
        grpConnection.Controls.Add(cboDatabases);
        grpConnection.Controls.Add(lblDatabaseHint);
        grpConnection.Dock = DockStyle.Fill;
        grpConnection.Location = new Point(0, 0);
        grpConnection.Name = "grpConnection";
        grpConnection.Size = new Size(600, 400);
        grpConnection.TabIndex = 0;
        grpConnection.TabStop = false;
        grpConnection.Text = "Source";
        //
        // lblServer
        //
        lblServer.AutoSize = true;
        lblServer.Location = new Point(15, 35);
        lblServer.Name = "lblServer";
        lblServer.Size = new Size(49, 19);
        lblServer.TabIndex = 0;
        lblServer.Text = "Server";
        //
        // txtServer
        //
        txtServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtServer.Location = new Point(115, 32);
        txtServer.Name = "txtServer";
        txtServer.PlaceholderText = "Server or server\\instance";
        txtServer.Size = new Size(465, 25);
        txtServer.TabIndex = 1;
        txtServer.TextChanged += input_Changed;
        //
        // lblUsername
        //
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(15, 72);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(72, 19);
        lblUsername.TabIndex = 2;
        lblUsername.Text = "Username";
        //
        // txtUsername
        //
        txtUsername.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtUsername.Location = new Point(115, 69);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(465, 25);
        txtUsername.TabIndex = 3;
        txtUsername.TextChanged += input_Changed;
        //
        // lblPassword
        //
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(15, 109);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(67, 19);
        lblPassword.TabIndex = 4;
        lblPassword.Text = "Password";
        //
        // txtPassword
        //
        txtPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtPassword.Location = new Point(115, 106);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new Size(465, 25);
        txtPassword.TabIndex = 5;
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.TextChanged += input_Changed;
        //
        // chkWindowsAuthentication
        //
        chkWindowsAuthentication.AutoSize = true;
        chkWindowsAuthentication.Checked = true;
        chkWindowsAuthentication.CheckState = CheckState.Checked;
        chkWindowsAuthentication.Location = new Point(115, 142);
        chkWindowsAuthentication.Name = "chkWindowsAuthentication";
        chkWindowsAuthentication.Size = new Size(194, 23);
        chkWindowsAuthentication.TabIndex = 6;
        chkWindowsAuthentication.Text = "Use Windows Authentication";
        chkWindowsAuthentication.UseVisualStyleBackColor = true;
        chkWindowsAuthentication.CheckedChanged += input_Changed;
        //
        // chkTrustServerCertificate
        //
        chkTrustServerCertificate.AutoSize = true;
        chkTrustServerCertificate.Location = new Point(115, 174);
        chkTrustServerCertificate.Name = "chkTrustServerCertificate";
        chkTrustServerCertificate.Size = new Size(290, 23);
        chkTrustServerCertificate.TabIndex = 7;
        chkTrustServerCertificate.Text = "Trust server certificate (self-signed servers)";
        chkTrustServerCertificate.UseVisualStyleBackColor = true;
        chkTrustServerCertificate.CheckedChanged += input_Changed;
        //
        // btnCheckConnection
        //
        btnCheckConnection.Location = new Point(115, 208);
        btnCheckConnection.Name = "btnCheckConnection";
        btnCheckConnection.Size = new Size(140, 30);
        btnCheckConnection.TabIndex = 8;
        btnCheckConnection.Text = "Check Connection";
        btnCheckConnection.UseVisualStyleBackColor = true;
        btnCheckConnection.Click += btnCheckConnection_Click;
        //
        // lblConnected
        //
        lblConnected.AutoSize = true;
        lblConnected.Location = new Point(15, 255);
        lblConnected.Name = "lblConnected";
        lblConnected.Size = new Size(77, 19);
        lblConnected.TabIndex = 9;
        lblConnected.Text = "Connected:";
        //
        // lblConnectedServer
        //
        lblConnectedServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblConnectedServer.AutoEllipsis = true;
        lblConnectedServer.Location = new Point(115, 255);
        lblConnectedServer.Name = "lblConnectedServer";
        lblConnectedServer.Size = new Size(465, 20);
        lblConnectedServer.TabIndex = 10;
        lblConnectedServer.Text = "NONE";
        //
        // lblDatabase
        //
        lblDatabase.AutoSize = true;
        lblDatabase.Location = new Point(15, 292);
        lblDatabase.Name = "lblDatabase";
        lblDatabase.Size = new Size(65, 19);
        lblDatabase.TabIndex = 11;
        lblDatabase.Text = "Database";
        //
        // cboDatabases
        //
        cboDatabases.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cboDatabases.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDatabases.Location = new Point(115, 288);
        cboDatabases.Name = "cboDatabases";
        cboDatabases.Size = new Size(465, 25);
        cboDatabases.TabIndex = 12;
        cboDatabases.SelectedIndexChanged += cboDatabases_SelectedIndexChanged;
        //
        // lblDatabaseHint
        //
        lblDatabaseHint.AutoSize = true;
        lblDatabaseHint.Location = new Point(115, 322);
        lblDatabaseHint.Name = "lblDatabaseHint";
        lblDatabaseHint.Size = new Size(306, 19);
        lblDatabaseHint.TabIndex = 13;
        lblDatabaseHint.Text = "Select a database after checking the connection.";
        //
        // ConnectionControl
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(grpConnection);
        Font = new Font("Segoe UI", 10F);
        Name = "ConnectionControl";
        Size = new Size(600, 400);
        grpConnection.ResumeLayout(false);
        grpConnection.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private GroupBox grpConnection;
    private Label lblServer;
    private TextBox txtServer;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPassword;
    private TextBox txtPassword;
    private CheckBox chkWindowsAuthentication;
    private CheckBox chkTrustServerCertificate;
    private Button btnCheckConnection;
    private Label lblConnected;
    private Label lblConnectedServer;
    private Label lblDatabase;
    private ComboBox cboDatabases;
    private Label lblDatabaseHint;
}
