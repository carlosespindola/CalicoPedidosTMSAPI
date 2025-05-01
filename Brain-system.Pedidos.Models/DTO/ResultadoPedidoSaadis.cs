using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class ResultadoPedidoSaadis
    {
        public ResultadoPedidoSaadis() { }

        public ResultadoPedidoSaadis(string _centroEmisorComprobante, string _numeroComprobante, string _letraComprobante)
        {
            this.CentroEmisor = _centroEmisorComprobante;
            this.NumeroComprobante = _numeroComprobante;
            this.Letra = _letraComprobante;
        }

        public virtual string CentroEmisor
        {
            get;
            set;
        }

        public virtual string NumeroComprobante
        {
            get;
            set;
        }

        public virtual string Letra
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
