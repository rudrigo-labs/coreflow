using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CoreFlow.Infrastructure.DbContexts
{
    /// <summary>
    /// Contexto para utilizar Dapper
    /// </summary>
    public abstract class DapperConnection
    {
        public IConfiguration Configuration { get; }

        protected DapperConnection(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IDbConnection GetConnection(string connectionName) { return new SqlConnection(Configuration.GetConnectionString(connectionName)); }
    }
}
