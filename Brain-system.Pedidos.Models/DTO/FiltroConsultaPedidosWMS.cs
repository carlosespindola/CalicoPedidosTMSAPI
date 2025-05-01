using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class FiltroConsultaDetallePedidoWMS
    {
        public string CentroEmisor { get; set; }

        public int? NroComprobante { get; set; }

        public string LetraComprobante { get; set; }
    }
    public class FiltroConsultaPedidosWMS : FiltroConsultaDetallePedidoWMS
    {
        public string CodigoDestinatario { get; set; }

        public string Destinatario { get; set; }

        public string EstadoComprobante { get; set; }

        public DateTime? FechaEmisionCbteDesde { get; set; }

        public DateTime? FechaEmisionCbteHasta { get; set; }
    }
}
