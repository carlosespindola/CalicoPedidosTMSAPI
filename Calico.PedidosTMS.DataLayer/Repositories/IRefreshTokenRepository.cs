using System.Threading.Tasks;
using Calico.PedidosTMS.Model;

namespace Calico.PedidosTMS.DAL.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<int> DeleteByClienteAndUsuarioAsync(string cliente, string usuario);
        Task<int> InsertAsync(RefreshToken token);
    }
}