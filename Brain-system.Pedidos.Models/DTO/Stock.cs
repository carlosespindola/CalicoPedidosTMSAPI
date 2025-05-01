using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class Stock
    {
        public string Compania { get; set; }

        public string Producto { get; set; }

        public string Descripcion { get; set; }

        public decimal Cantidad { get; set; }

        public string Estado { get; set; }
    }
}
