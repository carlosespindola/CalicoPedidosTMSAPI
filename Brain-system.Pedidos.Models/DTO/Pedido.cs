using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class Pedido : IdentificadorPedido
    {
        [Required]
        public virtual DateTime FechaPedido
        {
            get;
            set;
        }

        [MaxLength(10, ErrorMessage = "Tamaño excedido")]
        public virtual string Cliente
        {
            get;
            set;
        }

        [MaxLength(10, ErrorMessage = "Tamaño excedido")]
        public virtual string Subcliente
        {
            get;
            set;
        }

        public virtual int? CondicionIVA
        {
            get;
            set;
        }

        [MaxLength(11, ErrorMessage = "Tamaño excedido")]
        public virtual string CUIT
        {
            get;
            set;
        }

        [MaxLength(60, ErrorMessage = "Tamaño excedido")]
        public virtual string RazonSocial
        {
            get;
            set;
        }

        [MaxLength(250, ErrorMessage = "Tamaño excedido")]
        public virtual string Domicilio
        {
            get;
            set;
        }

        [MaxLength(40, ErrorMessage = "Tamaño excedido")]
        public virtual string Localidad
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string Provincia
        {
            get;
            set;
        }

        [MaxLength(10, ErrorMessage = "Tamaño excedido")]
        public virtual string CodigoPostal
        {
            get;
            set;
        }

        [MaxLength(200, ErrorMessage = "Tamaño excedido")]
        public virtual string Email
        {
            get;
            set;
        }

        [MaxLength(25, ErrorMessage = "Tamaño excedido")]
        public virtual string Telefonos
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string Particiona
        {
            get;
            set;
        }

        public virtual DateTime FechaEntrega
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string RefA
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string RefB
        {
            get;
            set;
        }

        public virtual decimal? ValorFactura
        {
            get;
            set;
        }

        public virtual decimal? ValorContrareembolso
        {
            get;
            set;
        }

        public virtual decimal? PesoTotal
        {
            get;
            set;
        }

        [MaxLength(10, ErrorMessage = "Tamaño excedido")]
        public virtual string CentroDeCosto
        {
            get;
            set;
        }

        public List<PedidoItem> DetalleProductos
        {
            get;
            set;
        }
    }
}
