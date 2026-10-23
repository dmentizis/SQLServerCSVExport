using Microsoft.Data.SqlClient;
using SqlServerPersistence.Models;

namespace SqlServerPersistence.Data;

public static class SqlConnectionFactory
{
    public static SqlConnection Create(ConnectionSettings settings)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = settings.Server,
            InitialCatalog = settings.Database,
            IntegratedSecurity = settings.IntegratedSecurity,
            Encrypt = SqlConnectionEncryptOption.Mandatory,
            TrustServerCertificate = settings.TrustServerCertificate,
            ConnectTimeout = 15,
            ApplicationName = "SQL Server Persistence"
        };
        if (!settings.IntegratedSecurity)
        {
            builder.UserID = settings.Username;
            builder.Password = settings.Password;
        }
        return new SqlConnection(builder.ConnectionString);
    }
}
