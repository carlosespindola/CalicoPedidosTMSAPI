using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{

    public class PedidoItem
    {
        public virtual decimal Linea
        {
            get;
            set;
        }

        [MaxLength(3, ErrorMessage = "Tamaño excedido")]
        public virtual string Compania
        {
            get;
            set;
        }

        [MaxLength(15, ErrorMessage = "Tamaño excedido")]
        public virtual string Producto
        {
            get;
            set;
        }

        public virtual decimal? Cantidad
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string LoteUnico
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string DespachoParcial
        {
            get;
            set;
        }

        [MaxLength(3, ErrorMessage = "Tamaño excedido")]
        public virtual string EstadoProducto
        {
            get;
            set;
        }

        [MaxLength(15, ErrorMessage = "Tamaño excedido")]
        public virtual string Lote
        {
            get;
            set;
        }

        [MaxLength(15, ErrorMessage = "Tamaño excedido")]
        public virtual string Serie
        {
            get;
            set;
        }
    }

}
