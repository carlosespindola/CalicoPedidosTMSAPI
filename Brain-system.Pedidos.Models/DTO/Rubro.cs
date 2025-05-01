using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class RubrosData
    {
        public string CodigoRubro { get; set; }

        public string Rubro { get; set; }

        public string CodigoSubrubro { get; set; }

        public string Subrubro { get; set; }
    }

    public class Subrubro
    {
        public string SubrubroCd { get; set; }

        public string SubrubroDs { get; set; }

        public Subrubro(string codigo, string descripcion)
        {
            SubrubroCd = codigo;
            SubrubroDs = descripcion;
        }
    }


    public class Rubro
    {
        public string RubroCd { get; set; }

        public string RubroDs { get; set; }

        public List<Subrubro> Subrubros { get; set; }

        public Rubro(string codigo, string descripcion, List<Subrubro> subrubros)
        {
            RubroCd = codigo;
            RubroDs = descripcion;
            Subrubros = subrubros;
        }
    }
}
