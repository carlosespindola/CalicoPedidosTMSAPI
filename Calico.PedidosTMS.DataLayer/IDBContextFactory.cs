using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Data;


namespace Calico.PedidosTMS.DAL
{
	public class IDBContextFactory
	{
		private string connectionString;

		public IDBContextFactory(string connectionString)
		{
			this.connectionString = connectionString;
		}

        public DapperDBContext CreateDBContext()
        {
            return new DapperDBContext(this.connectionString);
        }
	}

	public class ISaadisDBContextFactory
	{
		private string connectionString;

		public ISaadisDBContextFactory(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public DapperDBContext CreateDBContext()
		{
			return new DapperDBContext(this.connectionString);
		}
	}

	public class DapperDBContext
	{
		private string _connectionString;

		internal DapperDBContext(string connectionString)
		{
			this._connectionString = connectionString;
		}

		public IDbConnection GetConnection()
		{
			return new System.Data.SqlClient.SqlConnection(this._connectionString);
		}
	}
}
