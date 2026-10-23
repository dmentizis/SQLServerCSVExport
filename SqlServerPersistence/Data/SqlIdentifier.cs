namespace SqlServerPersistence.Data;

public static class SqlIdentifier
{
    public static string Quote(string identifier) => "[" + identifier.Replace("]", "]]") + "]";
}
