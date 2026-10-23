#nullable enable
namespace SQLServerCSVExport;

partial class Form1
{
    private System.ComponentModel.IContainer? components;
    private TextBox txtSourceServer = null!, txtSourceUsername = null!, txtSourcePassword = null!;
    private TextBox TxtExcludeSchema = null!, TxtExcludeTable = null!;
    private CheckBox chkWindows = null!, chkTrustCertificate = null!;
    private Button BtnConnect = null!, BtnChoose = null!, BtnExport = null!, BtnCancel = null!;
    private Button BtnAdd = null!, BtnClear = null!, BtnRemove = null!;
    private ComboBox cbDatabases = null!;
    private Label lblConnected = null!, lblPath = null!;
    private ListBox lbExclusions = null!;
    private RichTextBox logger = null!;
    private ProgressBar progressBar = null!;
    private NumericUpDown timeout = null!;
    private GroupBox sourceGroup = null!, exclusionGroup = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        Font = new Font("Segoe UI", 10F);
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1180, 690);
        MinimumSize = new Size(1050, 690);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Database Checksums Export";
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 2 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        Controls.Add(root);

        sourceGroup = new GroupBox { Text = "Source", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var source = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 9 };
        source.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        source.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < 9; i++) source.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 9));
        txtSourceServer = new TextBox { Name = "Server", Dock = DockStyle.Fill, PlaceholderText = @"Server or server\instance" };
        txtSourceUsername = new TextBox { Name = "Username", Dock = DockStyle.Fill };
        txtSourcePassword = new TextBox { Name = "Password", Dock = DockStyle.Fill, UseSystemPasswordChar = true };
        chkWindows = new CheckBox { Name = "WindowsAuthentication", Text = "Use Windows Authentication", AutoSize = true, Checked = true };
        chkTrustCertificate = new CheckBox { Text = "Trust server certificate (self-signed servers)", AutoSize = true };
        BtnConnect = MakeButton("Check Connection", "Connect");
        lblConnected = new Label { Text = "NONE", Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
        cbDatabases = new ComboBox { Name = "Databases", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        source.Controls.Add(MakeLabel("Server"), 0, 0); source.Controls.Add(txtSourceServer, 1, 0);
        source.Controls.Add(MakeLabel("Username"), 0, 1); source.Controls.Add(txtSourceUsername, 1, 1);
        source.Controls.Add(MakeLabel("Password"), 0, 2); source.Controls.Add(txtSourcePassword, 1, 2);
        source.Controls.Add(chkWindows, 1, 3);
        source.Controls.Add(chkTrustCertificate, 1, 4);
        source.Controls.Add(BtnConnect, 1, 5);
        source.Controls.Add(MakeLabel("Connected:"), 0, 6); source.Controls.Add(lblConnected, 1, 6);
        source.Controls.Add(MakeLabel("Database"), 0, 7); source.Controls.Add(cbDatabases, 1, 7);
        source.Controls.Add(new Label { Text = "Select a database after checking the connection.", AutoSize = true }, 1, 8);
        sourceGroup.Controls.Add(source);
        root.Controls.Add(sourceGroup, 0, 0);

        exclusionGroup = new GroupBox { Text = "Exclude Tables", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var exclusions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        exclusions.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        exclusions.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        exclusions.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
        exclusions.RowStyles.Add(new RowStyle(SizeType.Absolute, 37));
        lbExclusions = new ListBox { Name = "Exclusions", Dock = DockStyle.Fill, IntegralHeight = false, HorizontalScrollbar = true };
        exclusions.Controls.Add(lbExclusions, 0, 0);
        var excludeButtons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        BtnClear = MakeButton("Clear", "Clear");
        BtnRemove = MakeButton("Remove selected", "Remove");
        excludeButtons.Controls.AddRange([BtnClear, BtnRemove]);
        exclusions.Controls.Add(excludeButtons, 0, 1);
        exclusions.Controls.Add(MakeLabel("Schema / Table name (exact names)"), 0, 2);
        var addRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
        addRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
        addRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
        addRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        TxtExcludeSchema = new TextBox { Name = "ExcludeSchema", Text = "dbo", Dock = DockStyle.Fill };
        TxtExcludeTable = new TextBox { Name = "ExcludeTable", Dock = DockStyle.Fill };
        BtnAdd = MakeButton("Add", "Add");
        addRow.Controls.Add(TxtExcludeSchema, 0, 0); addRow.Controls.Add(TxtExcludeTable, 1, 0); addRow.Controls.Add(BtnAdd, 2, 0);
        exclusions.Controls.Add(addRow, 0, 3);
        exclusionGroup.Controls.Add(exclusions);
        root.Controls.Add(exclusionGroup, 1, 0);

        var exportGroup = new GroupBox { Text = "Export", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var export = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        export.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        BtnChoose = MakeButton("Choose Export Location", "Choose");
        lblPath = new Label { Text = "Export location: NONE", Dock = DockStyle.Fill, AutoEllipsis = true };
        var timeoutRow = new FlowLayoutPanel { Dock = DockStyle.Fill };
        timeout = new NumericUpDown { Minimum = 1, Maximum = 86400, Value = 300, Width = 85 };
        timeoutRow.Controls.AddRange([new Label { Text = "Query timeout (seconds):", AutoSize = true, Padding = new Padding(0, 4, 0, 0) }, timeout]);
        var actionRow = new FlowLayoutPanel { Dock = DockStyle.Fill };
        BtnExport = MakeButton("Export", "Export");
        BtnCancel = MakeButton("Cancel", "Cancel");
        actionRow.Controls.AddRange([BtnExport, BtnCancel]);
        progressBar = new ProgressBar { Dock = DockStyle.Fill };
        export.Controls.Add(BtnChoose, 0, 0); export.Controls.Add(lblPath, 0, 1);
        export.Controls.Add(timeoutRow, 0, 2); export.Controls.Add(actionRow, 0, 3); export.Controls.Add(progressBar, 0, 4);
        exportGroup.Controls.Add(export);
        root.Controls.Add(exportGroup, 0, 1);
        var loggerGroup = new GroupBox { Text = "Logger", Dock = DockStyle.Fill, Padding = new Padding(12) };
        logger = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = SystemColors.Window, DetectUrls = false, Font = new Font("Consolas", 9F) };
        loggerGroup.Controls.Add(logger);
        root.Controls.Add(loggerGroup, 1, 1);
        ResumeLayout(true);
    }

    private static Label MakeLabel(string text) => new() { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
    private static Button MakeButton(string text, string name) => new() { Text = text, Name = name, AutoSize = true, MinimumSize = new Size(75, 30), Anchor = AnchorStyles.Left };
}
