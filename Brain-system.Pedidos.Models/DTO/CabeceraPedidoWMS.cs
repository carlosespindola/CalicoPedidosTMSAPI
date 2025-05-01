using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class CabeceraPedidoWMS
    {
        public string Letra { get; set; }

        public string CentroEmisor { get; set; }

        public string NroPedido { get; set; }

        public string EstadoPedido { get; set; }

        public string Subcliente { get; set; }

        public string RazonSocial { get; set; }
    }
}
