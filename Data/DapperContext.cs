using MySqlConnector;
using System.Data;

namespace GrpcNet7.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _config;

        public DapperContext(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_config.GetConnectionString("connDef"));
    }
}
