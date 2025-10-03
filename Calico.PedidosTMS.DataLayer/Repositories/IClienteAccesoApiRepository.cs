using Calico.PedidosTMS.Model;
using System.Threading.Tasks;

namespace Calico.PedidosTMS.DAL.Repositories
{
    public interface IClienteAccesoApiRepository
    {
        Task<CLIENTESACCESOAPI> GetByClienteAndUsuarioAsync(string cliente, string usuario);
        Task<CLIENTESACCESOAPI> ValidateCredentialsAsync(string cliente, string usuario, string password);
    }
}