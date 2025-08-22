using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MonthlyRevenueApi.Infrastructure.Database
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;
        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateDbConnection(string connectionName)
        {
            var connStr = _config.GetConnectionString(connectionName);
            return new SqlConnection(connStr);
        }
    }
}
