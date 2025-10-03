using System;
using System.Collections.Generic;
using System.Text;

namespace Calico.PedidosTMS.Models.DTO
{
    public class ResultadoPedidoTMS
    {
        public ResultadoPedidoTMS() { }

        public ResultadoPedidoTMS(string _centroEmisorComprobante, string _numeroComprobante, string _letraComprobante)
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
