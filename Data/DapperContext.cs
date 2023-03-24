using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace GrpcNet7.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _config;

        public DapperContext(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connect() => new MySqlConnection(_config.GetConnectionString("connDef"));
    }
}
