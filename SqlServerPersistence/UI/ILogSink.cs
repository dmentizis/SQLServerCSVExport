namespace SqlServerPersistence.UI;

/// <summary>Shared application log. Call from the UI thread.</summary>
public interface ILogSink
{
    void Log(string message);
}
