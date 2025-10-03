using System;
using System.Data;
using System.Threading.Tasks;
using Calico.PedidosTMS.DAL.DapperContexts;
using Calico.PedidosTMS.Model;
using Dapper;

namespace Calico.PedidosTMS.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly CalicoApiTmsDapperContext _context;

        public RefreshTokenRepository(CalicoApiTmsDapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> DeleteByClienteAndUsuarioAsync(string cliente, string usuario)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    DELETE FROM RefreshToken
                    WHERE CODIGO_CLIENTE = @Cliente
                      AND USUARIO_API = @Usuario";

                return await connection.ExecuteAsync(sql, new { Cliente = cliente, Usuario = usuario });
            }
        }

        public async Task<int> InsertAsync(RefreshToken token)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    INSERT INTO RefreshToken (IssuedUtc, ExpiresUtc, Token, CODIGO_CLIENTE, USUARIO_API)
                    VALUES (@IssuedUtc, @ExpiresUtc, @Token, @CODIGOCLIENTE, @USUARIOAPI)";

                return await connection.ExecuteAsync(sql, token);
            }
        }
    }
}