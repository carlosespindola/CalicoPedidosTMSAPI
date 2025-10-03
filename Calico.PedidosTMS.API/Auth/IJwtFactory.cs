
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Calico.PedidosTMS.API.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(string codigoCliente, string usuario);
    }
}
