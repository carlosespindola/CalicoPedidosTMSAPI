using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using iText.Layout.Element;
using iText.Layout.Borders;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using QRCoder;

using iText.Barcodes;
using iText.Kernel.Pdf.Xobject;
using iText.Barcodes.Qrcode;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using Dapper;
using System.Xml;
using static iText.IO.Image.Jpeg2000ImageData;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using System;


namespace Calico.PedidosTMS.API.Services
{
    

    public class RotuloService //: IRotuloService
    {
        private readonly Calico.PedidosTMS.DAL.DapperContexts.CalicoInterfazDapperContext _calicoInterfazContext;
        private readonly Calico.PedidosTMS.DAL.DapperContexts.CalicoMasterDapperContext _calicoMasterContext;

        public RotuloService(
            Calico.PedidosTMS.DAL.DapperContexts.CalicoInterfazDapperContext calicoInterfazContext,
            Calico.PedidosTMS.DAL.DapperContexts.CalicoMasterDapperContext calicoMasterContext)
        {
            _calicoInterfazContext = calicoInterfazContext;
            _calicoMasterContext = calicoMasterContext;
        }

        private string razonSocialRemitente;
        private string domicilioRemitente;
        private string localidadRemitente;
        private string cpRemitente;

        private string destinatario;
        private string domicilioDestinatario;
        private string localidadDestinatario;
        private string cpDestinatario;
        private string telefonoDestinatario;

        private string destinatario2;
        private string telefonoDestinatario2;
        private string dniDestinatario2;
        private string datosEntrega;

        private string remito;
        private int bulto;
        private int bultosTotales;

        public async Task<string> GenerarRotuloPdfAsync(string cliente, string idComprobante, string URL_QR_OptimoCamino)
        {
            try
            {
                // Asignación de valores desde la base de datos
                AssignValuesFromSaadis(cliente, idComprobante);

                // Validación de entrada
                if (string.IsNullOrEmpty(cliente))
                {
                    throw new ArgumentException("El cliente no puede estar vacío.", nameof(cliente));
                }

                if (string.IsNullOrEmpty(idComprobante))
                {
                    throw new ArgumentException("El ID de comprobante no puede estar vacío.", nameof(idComprobante));
                }              

                // Validaciones adicionales después de la asignación
                if (string.IsNullOrEmpty(razonSocialRemitente) )//|| string.IsNullOrEmpty(domicilioRemitente))
                {
                    throw new InvalidOperationException("Los datos del remitente están incompletos.");
                }

                if (string.IsNullOrEmpty(destinatario) || string.IsNullOrEmpty(domicilioDestinatario))
                {
                    throw new InvalidOperationException("Los datos del destinatario están incompletos.");
                }

                bultosTotales = int.Parse(idComprobante.Substring(idComprobante.Length - 3));

                using var ms = new MemoryStream();
                using var writer = new PdfWriter(ms);
                using var pdf = new PdfDocument(writer);

                // Tamaño 10 x 15 cm en puntos
                var pageSize = new PageSize(283.465f, 425.197f);

                float marginPoints = 2f * 2.83465f; // 2 mm en puntos

                // Fuentes (declarar una sola vez fuera del bucle)
                var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Datos que se extraen una sola vez
                string letra = idComprobante.Substring(0, 1);
                string centroEmisor = idComprobante.Substring(1, 4);
                string comprobante = idComprobante.Substring(5, 8);
                string valorQR = $"{URL_QR_OptimoCamino}{cliente}-05-{letra}-{centroEmisor}-{comprobante}";

                // Generar QR con el valor de IdSeguimiento (una sola vez)
                var qrGenerator = new QRCodeGenerator();
                var qrData = qrGenerator.CreateQrCode(idComprobante, QRCodeGenerator.ECCLevel.Q);
                var pngQrCode = new PngByteQRCode(qrData);
                byte[] qrBytes = pngQrCode.GetGraphic(20);
                var qrImageData = ImageDataFactory.Create(qrBytes);

                // Imagen del logo (cargar una sola vez)
                string imagePath = "Resources\\Calico.png";
                var imageData = ImageDataFactory.Create(imagePath);

                var remitoCorto = remito.Length > 3 ? remito.Substring(0, remito.Length - 3) : remito;

                // Crear el documento UNA SOLA VEZ fuera del bucle
                var doc = new iText.Layout.Document(pdf, pageSize);
                doc.SetMargins(marginPoints, marginPoints, marginPoints, marginPoints);

                // BUCLE PARA GENERAR CADA RÓTULO
                for (int bulto = 1; bulto <= bultosTotales; bulto++)
                {
                    // Crear nueva página para cada rótulo
                    var page = pdf.AddNewPage(pageSize);
                    var canvas = new PdfCanvas(page);

                    // Marco de la página
                    float x = marginPoints;
                    float y = marginPoints;
                    float width = pageSize.GetWidth() - 2 * marginPoints;
                    float height = pageSize.GetHeight() - 2 * marginPoints;
                    canvas.SetLineWidth(0.75f);
                    canvas.Rectangle(x, y, width, height);
                    canvas.Stroke();

                    // Contenedor interior con margen adicional
                    float innerMargin = 2f * 2.83465f;
                    var contentContainer = new Div().SetMargin(innerMargin);

                    // Cabecera
                    var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1 })).UseAllAvailableWidth();
                    var image = new Image(imageData).SetWidth(100);

                    var qrImage = new Image(qrImageData).SetWidth(70).SetHeight(60);

                    // Crear un Div para contener la imagen y el texto del QR
                    var qrContainer = new Div()
                        .SetWidth(70)
                        .SetHorizontalAlignment(HorizontalAlignment.RIGHT)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                    qrContainer.Add(qrImage);

                    var qrText = new Paragraph(idComprobante.Substring(0, idComprobante.Length - 3))
                        .SetFontSize(5)
                        .SetMarginTop(1);

                    qrContainer.Add(qrText);

                    var qrCell = new Cell()
                        .Add(qrContainer)
                        .SetBorder(Border.NO_BORDER)
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                    headerTable.AddCell(new Cell().Add(image).SetBorder(Border.NO_BORDER));
                    headerTable.AddCell(qrCell);

                    contentContainer.Add(headerTable);

                    // Remitente
                    contentContainer.Add(new Paragraph("REMITENTE").SetFont(boldFont).SetFontSize(10));
                    contentContainer.Add(new Paragraph($"{razonSocialRemitente}\n{domicilioRemitente}\n{localidadRemitente}\n{cpRemitente}").SetFont(normalFont).SetFontSize(9));

                    // Destinatario y Bultos en la misma tabla
                    var destinatarioTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1 })).UseAllAvailableWidth();

                    // Celda para la información del destinatario
                    var destinatarioCell = new Cell().Add(new Paragraph("DESTINATARIO").SetFont(boldFont).SetFontSize(10))
                        .Add(new Paragraph($"{destinatario}\n{domicilioDestinatario}\n{localidadDestinatario}\n{cpDestinatario} - {telefonoDestinatario}").SetFont(normalFont).SetFontSize(9))
                        .SetBorder(Border.NO_BORDER);
                    destinatarioTable.AddCell(destinatarioCell);

                    // AQUÍ ES DONDE CAMBIA CADA RÓTULO - La información de bultos
                    var bultosCell = new Cell().Add(new Paragraph("BULTOS:").SetFont(boldFont).SetFontSize(9))
                        .Add(new Paragraph($"{bulto}/{bultosTotales}").SetFont(boldFont).SetFontSize(9))
                        .SetBorder(Border.NO_BORDER)
                        .SetVerticalAlignment(VerticalAlignment.TOP)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                    destinatarioTable.AddCell(bultosCell);

                    contentContainer.Add(destinatarioTable);

                    // Autorizado
                    contentContainer.Add(new Paragraph("Autorizado:").SetFont(boldFont).SetFontSize(9));
                    contentContainer.Add(new Paragraph($"{destinatario2}\n{telefonoDestinatario2}\n{dniDestinatario2}\n{datosEntrega}").SetFont(normalFont).SetFontSize(9));

                    // Remito
                    contentContainer.Add(new Paragraph($"Remito: {remitoCorto}").SetFont(normalFont).SetFontSize(9));

                    // Generar código de barras 3of9
                    var barcode = new Barcode39(pdf);
                    barcode.SetCode(remito);
                    barcode.SetStartStopText(false);
                    barcode.SetBarHeight(40f);
                    barcode.SetX(1.2f);

                    var barcodeObject = barcode.CreateFormXObject(ColorConstants.BLACK, ColorConstants.WHITE, pdf);
                    var barcodeImage = new Image(barcodeObject)
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                        .ScaleToFit(180, 40)
                        .SetMarginBottom(0)
                        .SetPaddingBottom(0);

                    var barcodeText = new Paragraph(remito)
                        .SetFont(normalFont)
                        .SetFontSize(9)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetMarginTop(0)
                        .SetMarginBottom(0);

                    var barcodeContainer = new Div()
                        .SetMarginTop(5)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                    barcodeContainer.Add(barcodeImage);
                    barcodeContainer.Add(barcodeText);

                    contentContainer.Add(barcodeContainer);

                    // Añadir el contenedor al documento
                    doc.Add(contentContainer);
                }

                // Cerrar el documento UNA SOLA VEZ al final
                doc.Close();

                return Convert.ToBase64String(ms.ToArray());




                //using var ms = new MemoryStream();
                //using var writer = new PdfWriter(ms);
                //using var pdf = new PdfDocument(writer);

                //// Tamaño 10 x 15 cm en puntos
                //var pageSize = new PageSize(283.465f, 425.197f);
                //var doc = new iText.Layout.Document(pdf, pageSize);

                //float marginPoints = 2f * 2.83465f; // 2 mm en puntos
                //doc.SetMargins(marginPoints, marginPoints, marginPoints, marginPoints);

                //// Página y marco
                //var page = pdf.AddNewPage(pageSize);
                //var canvas = new PdfCanvas(page);
                //float x = marginPoints;
                //float y = marginPoints;
                //float width = pageSize.GetWidth() - 2 * marginPoints;
                //float height = pageSize.GetHeight() - 2 * marginPoints;
                //canvas.SetLineWidth(0.75f);
                //canvas.Rectangle(x, y, width, height);
                //canvas.Stroke();

                //// Contenedor interior con margen adicional (otros 2 mm = 5.67 pt)
                //float innerMargin = 2f * 2.83465f;
                //var contentContainer = new Div().SetMargin(innerMargin);

                //// Fuentes
                //var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                //var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                //// Cabecera
                //var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1 })).UseAllAvailableWidth();
                //string imagePath = "Resources\\Calico.png";
                //var imageData = ImageDataFactory.Create(imagePath);
                //var image = new Image(imageData).SetWidth(100);


                //string letra = idComprobante.Substring(0, 1); 
                //string centroEmisor = idComprobante.Substring(1, 4); 
                //string comprobante = idComprobante.Substring(5, 8);                

                //string valorQR = $"{URL_QR_OptimoCamino}{cliente}-05-{letra}-{centroEmisor}-{comprobante}";

                //// Generar QR con el valor de IdSeguimiento
                //var qrGenerator = new QRCodeGenerator();
                //var qrData = qrGenerator.CreateQrCode(idComprobante, QRCodeGenerator.ECCLevel.Q);
                //var pngQrCode = new PngByteQRCode(qrData);
                //byte[] qrBytes = pngQrCode.GetGraphic(20); // Escala 20, ajustá según sea necesario

                //// Crear la imagen desde los bytes del código QR
                //var qrImageData = ImageDataFactory.Create(qrBytes);
                //var qrImage = new Image(qrImageData).SetWidth(70).SetHeight(60); // Ajusta el tamaño según sea necesario

                //// Crear un Div para contener la imagen y el texto del QR
                //var qrContainer = new Div()
                //    .SetWidth(70) // Establecer el ancho del Div igual al ancho de la imagen QR
                //    .SetHorizontalAlignment(HorizontalAlignment.RIGHT) // Alinea el contenedor a la derecha
                //    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER); // Centra el texto dentro del Div

                //// Agregar la imagen del QR al contenedor
                //qrContainer.Add(qrImage);

                //// Agregar el texto del IdSeguimiento debajo de la imagen
                //var qrText = new Paragraph(idComprobante.Substring(0, idComprobante.Length - 3))
                //    .SetFontSize(5)
                //    .SetMarginTop(1);

                //qrContainer.Add(qrText);

                //// Crear una nueva celda y centrar el contenido dentro de ella
                //var qrCell = new Cell()
                //    .Add(qrContainer)
                //    .SetBorder(Border.NO_BORDER)
                //    .SetHorizontalAlignment(HorizontalAlignment.CENTER); // Centrar el contenido de la celda

                //// Añadir la imagen del logo a la primera celda
                //headerTable.AddCell(new Cell().Add(image).SetBorder(Border.NO_BORDER));
                //// Añadir la celda que contiene el QR centrado
                //headerTable.AddCell(qrCell);

                //contentContainer.Add(headerTable);

                //// Remitente
                //contentContainer.Add(new Paragraph("REMITENTE").SetFont(boldFont).SetFontSize(10));
                //contentContainer.Add(new Paragraph($"{razonSocialRemitente}\n{domicilioRemitente}\n{localidadRemitente}\n{cpRemitente}").SetFont(normalFont).SetFontSize(9));

                //// Destinatario y Bultos en la misma tabla
                //var destinatarioTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1 })).UseAllAvailableWidth();

                //// Celda para la información del destinatario
                //var destinatarioCell = new Cell().Add(new Paragraph("DESTINATARIO").SetFont(boldFont).SetFontSize(10))
                //    .Add(new Paragraph($"{destinatario}\n{domicilioDestinatario}\n{localidadDestinatario}\n{cpDestinatario} - {telefonoDestinatario}").SetFont(normalFont).SetFontSize(9))
                //    .SetBorder(Border.NO_BORDER);
                //destinatarioTable.AddCell(destinatarioCell);

                //// Celda para la información de los bultos
                //var bultosCell = new Cell().Add(new Paragraph("BULTOS:").SetFont(boldFont).SetFontSize(9))
                //    .Add(new Paragraph($"{bulto}/{bultosTotales}").SetFont(boldFont).SetFontSize(9))
                //    .SetBorder(Border.NO_BORDER)
                //    .SetVerticalAlignment(VerticalAlignment.TOP) // Alinea los bultos en la parte superior
                //    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT); // Alinea el texto a la derecha
                //destinatarioTable.AddCell(bultosCell);

                //contentContainer.Add(destinatarioTable);

                //// Autorizado
                //contentContainer.Add(new Paragraph("Autorizado:").SetFont(boldFont).SetFontSize(9));
                //contentContainer.Add(new Paragraph($"{destinatario2}\n{telefonoDestinatario2}\n{dniDestinatario2}\n{datosEntrega}").SetFont(normalFont).SetFontSize(9));

                //var remitoCorto = remito.Length > 3 ? remito.Substring(0, remito.Length - 3) : remito;

                //// Remito
                //contentContainer.Add(new Paragraph($"Remito: {remitoCorto}").SetFont(normalFont).SetFontSize(9));

                //// Generar código de barras 3of9
                //var barcode = new Barcode39(pdf);
                //barcode.SetCode(remito);
                //barcode.SetStartStopText(false); // Sin asteriscos visibles
                //barcode.SetBarHeight(40f);       // Aumentamos la altura
                //barcode.SetX(1.2f);              // Aumentamos el ancho de barras

                //var barcodeObject = barcode.CreateFormXObject(ColorConstants.BLACK, ColorConstants.WHITE, pdf);
                //var barcodeImage = new Image(barcodeObject)
                //    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                //    .ScaleToFit(180, 40) // Escalar para un ancho más grande pero contenido
                //    .SetMarginBottom(0)
                //    .SetPaddingBottom(0);

                //var barcodeText = new Paragraph(remito)
                //    .SetFont(normalFont)
                //    .SetFontSize(9) // Tamaño de fuente un poco mayor
                //    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                //    .SetMarginTop(0)
                //    .SetMarginBottom(0);

                //var barcodeContainer = new Div()
                //    .SetMarginTop(5)
                //    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                //barcodeContainer.Add(barcodeImage);
                //barcodeContainer.Add(barcodeText);

                //contentContainer.Add(barcodeContainer);

                //// Añadir el contenedor al documento
                //doc.Add(contentContainer);

                //doc.Close();

                //return Convert.ToBase64String(ms.ToArray());
            }
            catch (ArgumentException ex)
            {
                // Manejo de errores de validación
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                // Manejo de errores específicos de lógica de negocio
                throw ex;
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                throw ex;
            }
        }

        private void AssignValuesFromSaadis(string cliente, string idComprobante)
        {
            try
            {
                using var connection = _calicoInterfazContext.CreateConnection();

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@CBT_NROCLI", cliente);
                parameters.Add("@IdComprobante", idComprobante);

                RotuloECommerceDto result = connection.QueryFirstOrDefault<RotuloECommerceDto>(
                    "spGetDataRotuloECommerce",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    throw new InvalidOperationException("No se encontraron datos para el comprobante proporcionado.");
                }

                razonSocialRemitente = result.Remitente;
                domicilioRemitente = result.DomicilioRemitente;
                localidadRemitente = result.LocalidadRemitente;
                cpRemitente = result.CpRemitente;

                destinatario = result.Destinatario;
                domicilioDestinatario = result.DomicilioDestinatario;
                localidadDestinatario = result.LocalidadDestinatario;
                cpDestinatario = result.CpDestinatario;
                telefonoDestinatario = result.TelefonoDestinatario;

                destinatario2 = result.Destinatario2;
                telefonoDestinatario2 = result.TelefonoDestinatario2;
                dniDestinatario2 = result.DniDestinatario2;
                datosEntrega = result.DatosEntrega;

                remito = result.Remito;
                bulto = int.TryParse(result.Nro_Bulto, out int b) ? b : 0;
                bultosTotales = result.BultosTotales;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al asignar los valores desde la base de datos.", ex);
            }
        }
    }

}
