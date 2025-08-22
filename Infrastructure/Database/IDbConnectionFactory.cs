using System.Data;

namespace MonthlyRevenueApi.Infrastructure.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection(string connectionName);
    }
}
