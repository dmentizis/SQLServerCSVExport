#nullable enable
namespace SqlServerPersistence.UI.Controls;

partial class LogControl
{
    private System.ComponentModel.IContainer? components;
    private GroupBox grpLog = null!;
    private RichTextBox rtbLog = null!;

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
        grpLog = new GroupBox { Name = nameof(grpLog), Text = "Logger", Dock = DockStyle.Fill, Padding = new Padding(12) };
        rtbLog = new RichTextBox { Name = nameof(rtbLog), Dock = DockStyle.Fill, ReadOnly = true, BackColor = SystemColors.Window, DetectUrls = false, Font = new Font("Consolas", 9F) };
        grpLog.Controls.Add(rtbLog);
        Controls.Add(grpLog);
        ResumeLayout(true);
    }
}
