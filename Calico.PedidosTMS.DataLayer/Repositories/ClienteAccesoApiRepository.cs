using System;
using System.Data;
using System.Threading.Tasks;
using Calico.PedidosTMS.DAL.DapperContexts;
using Calico.PedidosTMS.Model;
using Dapper;

namespace Calico.PedidosTMS.DAL.Repositories
{
    public class ClienteAccesoApiRepository : IClienteAccesoApiRepository
    {
        private readonly CalicoApiTmsDapperContext _context;

        public ClienteAccesoApiRepository(CalicoApiTmsDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CLIENTESACCESOAPI> GetByClienteAndUsuarioAsync(string cliente, string usuario)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT
                        CODIGO_CLIENTE AS CODIGOCLIENTE,
                        USUARIO_API AS USUARIOAPI,
                        PASSWORD_API AS PASSWORDAPI
                    FROM CLIENTES_ACCESO_API
                    WHERE CODIGO_CLIENTE = @Cliente
                      AND USUARIO_API = @Usuario";

                return await connection.QueryFirstOrDefaultAsync<CLIENTESACCESOAPI>(sql, new { Cliente = cliente, Usuario = usuario });
            }
        }

        public async Task<CLIENTESACCESOAPI> ValidateCredentialsAsync(string cliente, string usuario, string password)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT
                        CODIGO_CLIENTE AS CODIGOCLIENTE,
                        USUARIO_API AS USUARIOAPI,
                        PASSWORD_API AS PASSWORDAPI
                    FROM CLIENTES_ACCESO_API
                    WHERE CODIGO_CLIENTE = @Cliente
                      AND USUARIO_API = @Usuario
                      AND PASSWORD_API = @Password";

                return await connection.QueryFirstOrDefaultAsync<CLIENTESACCESOAPI>(sql, new { Cliente = cliente, Usuario = usuario, Password = password });
            }
        }
    }
}