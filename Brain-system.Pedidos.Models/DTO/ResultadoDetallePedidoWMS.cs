using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class ResultadoDetallePedidoWMS
    {
        public string CAT_PEDIDO { get; set; }
        public string SUC_PEDIDO { get; set; }
        public string NRO_PEDIDO { get; set; }
        public string ESTADOPEDIDO { get; set; }
        public int LINEA { get; set; }
        public string CIA { get; set; }
        public string PRODUCTO { get; set; }
        public decimal CANTIDAD { get; set; }
        public decimal CantidadPickeada { get; set; }
        public string CantidadLote { get; set; }
        public string CantidadSerie { get; set; }
        public decimal CantidadConError { get; set; }
        public decimal CantidadCancelada { get; set; }
        public decimal CantidadEnProcesoDePicking { get; set; }
    }

    public class L
    {
        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
    }

    public class S
    {
        public string Serie { get; set; }
    }

    public class DetLinea
    {
        public int Linea { get; set; }
        public string Cia { get; set; }
        public string Producto { get; set; }
        public decimal Cantidad { get; set; }
        public object Pickeado { get; set; }
        public decimal ConError { get; set; }
        public decimal Cancelado { get; set; }
        public decimal EnProcesoPicking { get; set; }
    }

    public class DetallePedidoWMS
    {
        public string Letra { get; set; }
        public string CentroEmisor { get; set; }
        public string NroPedido { get; set; }
        public string EstadoPedido { get; set; }
        public List<DetLinea> DetalleLinea { get; set; }

    }
}
