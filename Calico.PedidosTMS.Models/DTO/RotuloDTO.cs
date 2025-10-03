public class RotuloECommerceDto
{
    public string Remitente { get; set; }
    public string DomicilioRemitente { get; set; }
    public string LocalidadRemitente { get; set; }
    public string CpRemitente { get; set; }
    public string Destinatario { get; set; }
    public string DomicilioDestinatario { get; set; }
    public string LocalidadDestinatario { get; set; }
    public string CpDestinatario { get; set; }
    public string TelefonoDestinatario { get; set; }

    public string Destinatario2 { get; set; }
    public string TelefonoDestinatario2 { get; set; }
    public string DniDestinatario2 { get; set; }
    public string DatosEntrega { get; set; }

    public string Remito { get; set; }
    public string Nro_Bulto { get; set; }

    public int BultosTotales { get; set; }
}
