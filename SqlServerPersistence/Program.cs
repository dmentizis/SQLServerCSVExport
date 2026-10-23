using SqlServerPersistence.Data;
using SqlServerPersistence.Features.ChecksumExport;
using SqlServerPersistence.UI;

namespace SqlServerPersistence;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Composition root: create shared services once and register one tab per feature.
        var metadata = new MetadataRepository();
        var mainForm = new MainForm();
        mainForm.AddTab(new ChecksumExportTab(metadata, new ChecksumExportService(metadata), mainForm.LogSink));
        Application.Run(mainForm);
    }
}
