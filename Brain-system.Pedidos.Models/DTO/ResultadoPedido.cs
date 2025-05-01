using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class ResultadoPedido
    {
        public ResultadoPedido(string _emplazamiento, string _almacen, string _tipoPedido, string _categoriaPedido, string _sucursalPedido, decimal _numeroPedido)
        {
            this.Emplazamiento = _emplazamiento;
            this.Almacen = _almacen;
            this.TipoPedido = _tipoPedido;
            this.CategoriaPedido = _categoriaPedido;
            this.SucursalPedido = _sucursalPedido;
            this.NumeroPedido = _numeroPedido;

        }

        public virtual string Emplazamiento
        {
            get;
            set;
        }

        public virtual string Almacen
        {
            get;
            set;
        }

        public virtual string TipoPedido
        {
            get;
            set;
        }

        public virtual string CategoriaPedido
        {
            get;
            set;
        }

        public virtual string SucursalPedido
        {
            get;
            set;
        }

        public virtual decimal NumeroPedido
        {
            get;
            set;
        }

        public virtual string Resultado
        {
            get;
            set;
        }

        public virtual string Mensaje
        {
            get;
            set;
        }
    }
}
