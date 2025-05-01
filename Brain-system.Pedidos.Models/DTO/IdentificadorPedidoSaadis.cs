using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class IdentificadorPedidoSaadis
    {
        [MaxLength(4, ErrorMessage = "Tamaño excedido")]
        public virtual string PuntoDeVentaComprobante //CBTCENEMI
        {
            get;
            set;
        }

        [MaxLength(8, ErrorMessage = "Tamaño excedido")]
        public virtual string NumeroComprobante //CBTNROCBT
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string LetraComprobante //CBTLETCBT
        {
            get;
            set;
        }
    }
}
