using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class Kardex
    {
        public string Compania { get; set; }

        public string Producto { get; set; }

        public string Descripcion { get; set; }

        public decimal Cantidad { get; set; }

        public string TipoOperacion { get; set; }

        public string DescOperacion { get; set; }

        public string FechaComprobante { get; set; }

        public string NroComprobante { get; set; }
    }
}
