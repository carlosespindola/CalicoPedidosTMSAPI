
using Microsoft.AspNetCore.Identity;

namespace Calico.PedidosTMS.API.Models.Entities
{
    // Add profile data for application users by adding properties to this class
  public class AppUser : IdentityUser
  {
    // Extended Properties
    public string Cliente { get; set; }

    public string Usuario { get; set; }
  }
}
