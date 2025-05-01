using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class FiltroConsultaComprobantes
    {
        public string CentroEmisor { get; set; }

        public string NroComprobante { get; set; }

        public string LetraComprobante { get; set; }

        public string CodigoDestinatario { get; set; }

        public string Destinatario { get; set; }

        public string Localidad { get; set; }

        public string EstadoComprobante { get; set; }

        public DateTime? FechaConformeDesde { get; set; }

        public DateTime? FechaConformeHasta { get; set; }

        public DateTime? FechaEmisionCbteDesde { get; set; }

        public DateTime? FechaEmisionCbteHasta { get; set; }

        public bool CbtesObservados { get; set; }
    }
}
