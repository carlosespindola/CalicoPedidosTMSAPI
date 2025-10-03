using System;
using System.Data;
using System.Data.SqlClient;

namespace Calico.PedidosTMS.DAL.DapperContexts
{
    public class CalicoInterfazDapperContext
    {
        private readonly string _connectionString;

        public CalicoInterfazDapperContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}