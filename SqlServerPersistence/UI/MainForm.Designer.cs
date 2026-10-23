#nullable enable
using SqlServerPersistence.UI.Controls;

namespace SqlServerPersistence.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer? components;
    private TabControl tabFeatures = null!;
    private LogControl logControl = null!;

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
        Text = "SQL Server Persistence";
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 28));
        Controls.Add(root);
        tabFeatures = new TabControl { Name = nameof(tabFeatures), Dock = DockStyle.Fill };
        root.Controls.Add(tabFeatures, 0, 0);
        logControl = new LogControl { Name = nameof(logControl), Dock = DockStyle.Fill };
        root.Controls.Add(logControl, 0, 1);
        ResumeLayout(true);
    }
}
