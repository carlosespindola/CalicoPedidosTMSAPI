using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class FiltroConsultaStock : FiltroConsultaKardex
    {
        public string CodigoRubro { get; set; }

        public string CodigoSubrubro { get; set; }
    }
}
