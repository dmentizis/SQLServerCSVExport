namespace SqlServerPersistence.UI;

/// <summary>Application shell: one tab per feature plus a shared log.</summary>
public partial class MainForm : Form
{
    private readonly List<IFeatureTab> _tabs = [];
    private bool _closeAfterCancel;

    public MainForm()
    {
        InitializeComponent();
        FormClosing += MainForm_FormClosing;
    }

    public ILogSink LogSink => logControl;

    public void AddTab<TTab>(TTab tab) where TTab : UserControl, IFeatureTab
    {
        tab.Dock = DockStyle.Fill;
        var page = new TabPage(tab.Title) { Name = "tab" + tab.GetType().Name, UseVisualStyleBackColor = true };
        page.Controls.Add(tab);
        tabFeatures.TabPages.Add(page);
        tab.OperationFinished += (_, _) =>
        {
            if (_closeAfterCancel && !_tabs.Any(t => t.IsBusy)) Close();
        };
        _tabs.Add(tab);
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        var busy = _tabs.Where(t => t.IsBusy).ToArray();
        if (busy.Length == 0) return;
        // Cancel running work and close once every tab has cleaned up.
        e.Cancel = true;
        _closeAfterCancel = true;
        foreach (var tab in busy) tab.CancelOperation();
    }
}
