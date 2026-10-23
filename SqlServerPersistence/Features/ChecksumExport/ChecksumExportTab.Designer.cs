#nullable enable
using SqlServerPersistence.UI.Controls;

namespace SqlServerPersistence.Features.ChecksumExport;

partial class ChecksumExportTab
{
    private System.ComponentModel.IContainer? components;
    private ConnectionControl connectionControl = null!;
    private GroupBox grpExclusions = null!, grpExport = null!;
    private ListBox lstExclusions = null!;
    private TextBox txtExcludeSchema = null!, txtExcludeTable = null!;
    private Button btnAddExclusion = null!, btnClearExclusions = null!, btnRemoveExclusion = null!;
    private Button btnChooseExportLocation = null!, btnExport = null!, btnCancel = null!;
    private Label lblExportPath = null!;
    private NumericUpDown nudQueryTimeout = null!;
    private ProgressBar prgExport = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _operation?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
        Controls.Add(root);

        connectionControl = new ConnectionControl { Name = nameof(connectionControl), Title = "Source", Dock = DockStyle.Fill };
        root.Controls.Add(connectionControl, 0, 0);
        root.SetRowSpan(connectionControl, 2);

        grpExclusions = new GroupBox { Name = nameof(grpExclusions), Text = "Exclude Tables", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var exclusions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        exclusions.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        exclusions.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        exclusions.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
        exclusions.RowStyles.Add(new RowStyle(SizeType.Absolute, 37));
        lstExclusions = new ListBox { Name = nameof(lstExclusions), Dock = DockStyle.Fill, IntegralHeight = false, HorizontalScrollbar = true };
        exclusions.Controls.Add(lstExclusions, 0, 0);
        var exclusionButtons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        btnClearExclusions = ControlFactory.CreateButton("Clear", nameof(btnClearExclusions));
        btnRemoveExclusion = ControlFactory.CreateButton("Remove selected", nameof(btnRemoveExclusion));
        exclusionButtons.Controls.AddRange([btnClearExclusions, btnRemoveExclusion]);
        exclusions.Controls.Add(exclusionButtons, 0, 1);
        exclusions.Controls.Add(ControlFactory.CreateLabel("Schema / Table name (exact names)"), 0, 2);
        var addRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
        addRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
        addRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72));
        addRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        txtExcludeSchema = new TextBox { Name = nameof(txtExcludeSchema), Text = "dbo", Dock = DockStyle.Fill };
        txtExcludeTable = new TextBox { Name = nameof(txtExcludeTable), Dock = DockStyle.Fill };
        btnAddExclusion = ControlFactory.CreateButton("Add", nameof(btnAddExclusion));
        addRow.Controls.Add(txtExcludeSchema, 0, 0); addRow.Controls.Add(txtExcludeTable, 1, 0); addRow.Controls.Add(btnAddExclusion, 2, 0);
        exclusions.Controls.Add(addRow, 0, 3);
        grpExclusions.Controls.Add(exclusions);
        root.Controls.Add(grpExclusions, 1, 0);

        grpExport = new GroupBox { Name = nameof(grpExport), Text = "Export", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var export = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        export.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        export.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        btnChooseExportLocation = ControlFactory.CreateButton("Choose Export Location", nameof(btnChooseExportLocation));
        lblExportPath = new Label { Name = nameof(lblExportPath), Text = "Export location: NONE", Dock = DockStyle.Fill, AutoEllipsis = true };
        var timeoutRow = new FlowLayoutPanel { Dock = DockStyle.Fill };
        nudQueryTimeout = new NumericUpDown { Name = nameof(nudQueryTimeout), Minimum = 1, Maximum = 86400, Value = 300, Width = 85 };
        timeoutRow.Controls.AddRange([new Label { Text = "Query timeout (seconds):", AutoSize = true, Padding = new Padding(0, 4, 0, 0) }, nudQueryTimeout]);
        var actionRow = new FlowLayoutPanel { Dock = DockStyle.Fill };
        btnExport = ControlFactory.CreateButton("Export", nameof(btnExport));
        btnCancel = ControlFactory.CreateButton("Cancel", nameof(btnCancel));
        actionRow.Controls.AddRange([btnExport, btnCancel]);
        prgExport = new ProgressBar { Name = nameof(prgExport), Dock = DockStyle.Fill };
        export.Controls.Add(btnChooseExportLocation, 0, 0); export.Controls.Add(lblExportPath, 0, 1);
        export.Controls.Add(timeoutRow, 0, 2); export.Controls.Add(actionRow, 0, 3); export.Controls.Add(prgExport, 0, 4);
        grpExport.Controls.Add(export);
        root.Controls.Add(grpExport, 1, 1);
        ResumeLayout(true);
    }
}
