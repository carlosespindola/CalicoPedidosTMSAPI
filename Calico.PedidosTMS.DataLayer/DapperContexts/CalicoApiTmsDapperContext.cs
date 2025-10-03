using System;
using System.Data;
using System.Data.SqlClient;

namespace Calico.PedidosTMS.DAL.DapperContexts
{
    public class CalicoApiTmsDapperContext
    {
        private readonly string _connectionString;

        public CalicoApiTmsDapperContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}