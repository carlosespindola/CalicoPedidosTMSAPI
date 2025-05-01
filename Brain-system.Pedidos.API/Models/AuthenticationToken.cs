using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainSystem.Pedidos.API.Models
{
    public class AuthenticationToken
    {
        public string id { get; set; }
        public string auth_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string usuario { get; set; }
    }
}
