using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class ResultadoValidacionPedido
    {
        public virtual string ResultadoValidacion
        {
            get;
            set;
        }

        public bool SubclienteNuevo
        {
            get;
            set;
        }

        public virtual string Domicilio
        {
            get;
            set;
        }

        public virtual string Localidad
        {
            get;
            set;
        }

        public virtual string CodigoPostal
        {
            get;
            set;
        }

        public virtual string AreaMuelle
        {
            get;
            set;
        }

        public string CambioDeAreaMuelle
        {
            get;
            set;
        }

        public virtual string Telefonos
        {
            get;
            set;
        }

        public virtual string Contacto
        {
            get;
            set;
        }
    }
}
