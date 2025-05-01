using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brainsystem.Pedidos.Models.DTO
{
    public class Comprobante
    {
        [JsonIgnore]
        public string cbt_cenemi { get; set; }
        [JsonIgnore]
        public string cbt_nrocli { get; set; }
        [JsonIgnore]
        public string cbt_nrocbt { get; set; }
        [JsonIgnore]
        public string cbt_letcbt { get; set; }
        [JsonIgnore]
        public string cbt_codcbt { get; set; }
        public string NumeroComprobante { get; set; }
        public string Remitente { get; set; }
        public string NombreDestinatario { get; set; }
        public string DomicilioDestinatario { get; set; }
        public string LocalidadDestinatario { get; set; }
        public string FechaComprobante { get; set; }
        public string FechaEntrega { get; set; }
        public string FechaRecepcion { get; set; }
        public string EstadoComprobante { get; set; }
        public int Bultos { get; set; }
        public decimal KilosNetos { get; set; }
        public decimal ValorDeclarado { get; set; }
        public decimal KilosAforados { get; set; }
        public decimal MetrosCubicos { get; set; }

        public List<Etapa> EtapasComprobante { get; set; }
    }

    public class Etapa
    {
        public string DescEtapa { get; set; }
        public string FechaEtapa { get; set; }
        public string HojaDeRuta { get; set; }
        public string Observacion { get; set; }
    }
}
