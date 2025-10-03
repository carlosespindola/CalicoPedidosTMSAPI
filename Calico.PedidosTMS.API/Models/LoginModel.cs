using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calico.PedidosTMS.API.Models
{
    /// <summary>
    /// Parameters used for login.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Codigo de cliente.
        /// </summary>
        public string Cliente { get; set; }
        /// <summary>
        /// Usuario API.
        /// </summary>
        public string Usuario { get; set; }
        /// <summary>
        /// Usuario password
        /// </summary>
        public string Password { get; set; }
    }
}
