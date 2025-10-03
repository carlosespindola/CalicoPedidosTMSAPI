using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Calico.PedidosTMS.Models.DTO
{
    public class PedidoTMSDesdeAPI
    {
        [MaxLength(4, ErrorMessage = "Tamaño excedido")]
        public virtual string PuntoDeVentaComprobante //CBTCENEMI
        {
            get;
            set;
        }

        [MaxLength(8, ErrorMessage = "Tamaño excedido")]
        public virtual string NumeroComprobante //CBTNROCBT
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string LetraComprobante //CBTLETCBT
        {
            get;
            set;
        }

        [Required]
        public virtual DateTime FechaComprobante //CBTFECCBT
        {
            get;
            set;
        }

        [MaxLength(15, ErrorMessage = "Tamaño excedido")]
        public virtual string NumeroRemito //CBTREMITO
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> Bultos //CBTBULTOS
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> KilosNetos //CBTKILOSN
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> KilosAforados //CBTKILOSA
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> MetrosCubicos //CBTMETRCB
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string NombreRemitente //CBTNOMRTE
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string LocalidadRemitente //CBTLOCRTE
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string NombreDestinatario //CBTNOMDES
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string DomicilioDestinatario //CBTDOMDES
        {
            get;
            set;
        }

        [MaxLength(5, ErrorMessage = "Tamaño excedido")]
        public virtual string NumeroCalleDestinatario //CBTNRODES
        {
            get;
            set;
        }

        [MaxLength(5, ErrorMessage = "Tamaño excedido")]
        public virtual string PisoDeptoDestinatario //CBTDTODES
        {
            get;
            set;
        }

        [MaxLength(20, ErrorMessage = "Tamaño excedido")]
        public virtual string LocalidadDestinatario //CBTLOCDES
        {
            get;
            set;
        }

        [MaxLength(10, ErrorMessage = "Tamaño excedido")]
        public virtual string CodigoPostalDestinatario //LCLCPOSTA
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> ValorDeclarado //CBTVADESE
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> ImporteContrareembolso //CBTIMPCRE
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string TipoIvaRemitente //IVACODIGO
        {
            get;
            set;
        }

        [MaxLength(13, ErrorMessage = "Tamaño excedido")]
        public virtual string CuitRemitente //CLTCUIT
        {
            get;
            set;
        }

        [MaxLength(10, ErrorMessage = "Tamaño excedido")]
        public virtual string NumeroClienteDestino //CBTSUBCLI
        {
            get;
            set;
        }

        [MaxLength(30, ErrorMessage = "Tamaño excedido")]
        public virtual string ObservacionEnvio //CBTDESCRI
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> CantidadPallets //CBTPALLET
        {
            get;
            set;
        }

        public virtual System.Nullable<decimal> CantidadUnidades //CBTUNIDAD
        {
            get;
            set;
        }

        public virtual System.Nullable<System.DateTime> FechaPosibleEntrega //CBTFEPOEN
        {
            get;
            set;
        }

        [MaxLength(33, ErrorMessage = "Tamaño excedido")]
        public virtual string ObservacionEnvio_2 //CBTDESCR2
        {
            get;
            set;
        }

        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string TipoIvasDestino //CbtTivade
        {
            get;
            set;
        }

        [MaxLength(13, ErrorMessage = "Tamaño excedido")]
        public virtual string CuitDestino //CbtCuitde
        {
            get;
            set;
        }

        [MaxLength(60, ErrorMessage = "Tamaño excedido")]
        public virtual string ObservacionAdicionalEnvio //CBTDETALL
        {
            get;
            set;
        }

        [MaxLength(4, ErrorMessage = "Tamaño excedido")]
        public virtual string SucursalOrigen //CBTSUCORI
        {
            get;
            set;
        }

        [MaxLength(100, ErrorMessage = "Tamaño excedido")]
        public virtual string Email //CBTEMAIL
        {
            get;
            set;
        }
        
        [MaxLength(70, ErrorMessage = "Tamaño excedido")]
        public virtual string Telefono //CbtTelefo
        {
            get;
            set;
        }
        [MaxLength(80, ErrorMessage = "Tamaño excedido")]
        public virtual string CampoAdicional1 //Cbt_adic1
        {
            get;
            set;
        }
        
        [MaxLength(80, ErrorMessage = "Tamaño excedido")]
        public virtual string CampoAdicional2 //Cbt_adic2
        {
            get;
            set;
        }
        
        [MaxLength(80, ErrorMessage = "Tamaño excedido")]
        public virtual string CampoAdicional3 //Cbt_adic3
        {
            get;
            set;
        }
        
        [MaxLength(80, ErrorMessage = "Tamaño excedido")]
        public virtual string CampoAdicional4 //Cbt_adic4
        {
            get;
            set;
        }
        
        [MaxLength(80, ErrorMessage = "Tamaño excedido")]
        public virtual string CampoAdicional5 //Cbt_adic5
        {
            get;
            set;
        }


        [MaxLength(1, ErrorMessage = "Tamaño excedido")]
        public virtual string Senasa //CbtSenasa
        {
            get;
            set;
        }

        [MaxLength(3, ErrorMessage = "Tamaño excedido")]
        public virtual string TipoCarga //Cbt_tipcar
        {
            get;
            set;
        }
        
        [MaxLength(40, ErrorMessage = "Tamaño excedido")]
        public virtual string NombreDestinatarioAlternativo //Cbt_nomdes_alter
        {
            get;
            set;
        }
        
        [MaxLength(4, ErrorMessage = "Tamaño excedido")]
        public virtual string TipoIvasDestinoAlternativo //Cbt_tivade_alter
        {
            get;
            set;
        }
        
        [MaxLength(13, ErrorMessage = "Tamaño excedido")]
        public virtual string CuitDestinoAlternativo //Cbt_cuitde_alter
        {
            get;
            set;
        }
        
        [MaxLength(70, ErrorMessage = "Tamaño excedido")]
        public virtual string TelefonoDestinoAlternativo //Cbt_telefo_alter
        {
            get;
            set;
        }
        
        [MaxLength(500, ErrorMessage = "Tamaño excedido")]
        public virtual string CodigoVerificadorEntrega //Cbt_codVerif
        {
            get;
            set;
        }

        [MaxLength(2000, ErrorMessage = "Tamaño excedido")]
        public virtual string DatosEntrega //Cbt_DatosEntrega
        {
            get;
            set;
        }
    }
}
