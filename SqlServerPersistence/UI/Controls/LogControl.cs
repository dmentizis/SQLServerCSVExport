namespace SqlServerPersistence.UI.Controls;

public partial class LogControl : UserControl, ILogSink
{
    public LogControl()
    {
        InitializeComponent();
    }

    public void Log(string message)
    {
        if (rtbLog.TextLength > 200_000) rtbLog.Clear();
        rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        rtbLog.SelectionStart = rtbLog.TextLength;
        rtbLog.ScrollToCaret();
    }
}
