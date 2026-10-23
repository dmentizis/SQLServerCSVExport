namespace SqlServerPersistence.UI;

/// <summary>
/// A feature hosted as a tab in <see cref="MainForm"/>. Implementations are UserControls
/// registered in Program.cs through <see cref="MainForm.AddTab{TTab}"/>.
/// </summary>
public interface IFeatureTab
{
    string Title { get; }
    bool IsBusy { get; }
    void CancelOperation();

    /// <summary>Raised on the UI thread after a running operation has finished and cleaned up.</summary>
    event EventHandler? OperationFinished;
}
