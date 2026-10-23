namespace SqlServerPersistence.UI.Controls;

/// <summary>Consistent styling for controls created in code-built layouts.</summary>
internal static class ControlFactory
{
    public static Label CreateLabel(string text) =>
        new() { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

    public static Button CreateButton(string text, string name) =>
        new() { Text = text, Name = name, AutoSize = true, MinimumSize = new Size(75, 30), Anchor = AnchorStyles.Left };
}
