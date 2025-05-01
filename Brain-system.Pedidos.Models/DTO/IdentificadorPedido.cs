using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class IdentificadorPedido
    {

        [MaxLength(3, ErrorMessage = "Tamaño excedido")]
        public virtual string Emplazamiento
        {
            get;
            set;
        }


        [MaxLength(3, ErrorMessage = "Tamaño excedido")]
        public virtual string Almacen
        {
            get;
            set;
        }


        [MaxLength(3, ErrorMessage = "Tamaño excedido")]
        public virtual string TipoPedido
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string CategoriaPedido
        {
            get;
            set;
        }

        [MaxLength(4, ErrorMessage = "Tamaño excedido")]
        public virtual string SucursalPedido
        {
            get;
            set;
        }

        [MaxLength(8, ErrorMessage = "Tamaño excedido")]
        public virtual string NumeroPedido
        {
            get;
            set;
        }
    }
}
