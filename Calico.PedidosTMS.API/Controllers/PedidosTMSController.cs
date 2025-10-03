using Asp.Versioning;
using Calico.PedidosTMS.API.Models;
using Calico.PedidosTMS.Models.DTO;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Calico.PedidosTMS.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosTMSController : ControllerBase
    {
        private readonly API.Services.PedidosTMSService _pedidosTMSService;
        private readonly API.Services.RotuloService _rotuloService;
        private string _cliente;

        public PedidosTMSController(
            IHttpContextAccessor contextAccessor,
            API.Services.PedidosTMSService pedidosTMSService,
            Services.RotuloService rotuloService)
        {
            ClaimsPrincipal currentUser = contextAccessor.HttpContext.User;

            _cliente = currentUser.FindFirstValue("id");

            _pedidosTMSService = pedidosTMSService;
            _rotuloService = rotuloService;
        }


        // POST: api/v1/PedidosSaadis/Alta
        /// <summary>
        /// Alta de pedidos en Saadis
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/PedidosSaadis/Alta
        ///            [
        ///              {
        ///                "puntoDeVentaComprobante": "0001",
        ///                "numeroComprobante": "1234",
        ///                "letraComprobante": "A",
        ///                "fechaComprobante": "2021-04-19T18:49:39.901Z",
        ///                "numeroRemito": "",
        ///                "bultos": 2,
        ///                "kilosNetos": 3,
        ///                "kilosAforados": 4,
        ///                "metrosCubicos": 5,
        ///                "nombreRemitente": "Juan",
        ///                "localidadRemitente": "loc juan",
        ///                "nombreDestinatario": "Pedro",
        ///                "domicilioDestinatario": "calle pedro",
        ///                "numeroCalleDestinatario": "987",
        ///                "pisoDeptoDestinatario": "",
        ///                "localidadDestinatario": "san antonio",
        ///                "codigoPostalDestinatario": "9876",
        ///                "valorDeclarado": 0,
        ///                "importeContrareembolso": 0,
        ///                "tipoIvaRemitente": "1",
        ///                "cuitRemitente": "1234567890123",
        ///                "numeroClienteDestino": "10",
        ///                "observacionEnvio": "1er obsv",
        ///                "cantidadPallets": 1,
        ///                "cantidadUnidades": 1,
        ///                "fechaPosibleEntrega": null,
        ///                "observacionEnvio_2": "2da obsv",
        ///                "tipoIvasDestino": "2",
        ///                "cuitDestino": "23127137099",
        ///                "observacionAdicionalEnvio": "sarasa",
        ///                "SucursalOrigen": "",
        ///                "Email": "",
        ///                "Telefono": "",
        ///                "CampoAdicional1": "",
        ///                "CampoAdicional2": "",
        ///                "CampoAdicional3": "",
        ///                "CampoAdicional4": "",
        ///                "CampoAdicional5": "",
        ///                "Senasa"!: "",
        ///                "TipoCarga": "",
        ///                "NombreDestinatarioAlternativo": "",
        ///                "TipoIvasDestinoAlternativo": "",
        ///                "CuitDestinoAlternativo": "",
        ///                "TelefonoDestinoAlternativo": "",
        ///                "CodigoVerificadorEntrega": "",
        ///                "DatosEntrega": ""
        ///              },
        ///              {
        ///                "puntoDeVentaComprobante": "0001",
        ///                "numeroComprobante": "1235",
        ///                "letraComprobante": "A",
        ///                "fechaComprobante": "2021-04-19T18:49:39.901Z",
        ///                "numeroRemito": "",
        ///                "bultos": 2,
        ///                "kilosNetos": 3,
        ///                "kilosAforados": 4,
        ///                "metrosCubicos": 5,
        ///                "nombreRemitente": "Juan",
        ///                "localidadRemitente": "loc juan",
        ///                "nombreDestinatario": "Pedro",
        ///                "domicilioDestinatario": "calle pedro",
        ///                "numeroCalleDestinatario": "987",
        ///                "pisoDeptoDestinatario": "",
        ///                "localidadDestinatario": "san antonio",
        ///                "codigoPostalDestinatario": "9876",
        ///                "valorDeclarado": 0,
        ///                "importeContrareembolso": 0,
        ///                "tipoIvaRemitente": "1",
        ///                "cuitRemitente": "1234567890123",
        ///                "numeroClienteDestino": "10",
        ///                "observacionEnvio": "1er obsv",
        ///                "cantidadPallets": 1,
        ///                "cantidadUnidades": 1,
        ///                "fechaPosibleEntrega": null,
        ///                "observacionEnvio_2": "2da obsv",
        ///                "tipoIvasDestino": "2",
        ///                "cuitDestino": "23127137099",
        ///                "observacionAdicionalEnvio": "sarasa"
        ///                "SucursalOrigen": "",
        ///                "Email": "",
        ///                "Telefono": "",
        ///                "CampoAdicional1": "",
        ///                "CampoAdicional2": "",
        ///                "CampoAdicional3": "",
        ///                "CampoAdicional4": "",
        ///                "CampoAdicional5": "",
        ///                "Senasa"!: "",
        ///                "TipoCarga": "",
        ///                "NombreDestinatarioAlternativo": "",
        ///                "TipoIvasDestinoAlternativo": "",
        ///                "CuitDestinoAlternativo": "",
        ///                "TelefonoDestinoAlternativo": "",
        ///                "CodigoVerificadorEntrega": "",
        ///                "DatosEntrega": ""
        ///              }
        ///            ]
        ///
        /// </remarks>
        /// <param name="pedidos"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de el alta de cada pedido</response>
        [Produces("application/json")]
        [HttpPost("Alta")]
        public async Task<IActionResult> Post([FromBody] PedidoTMSDesdeAPI[] pedidos)
        {
            try
            {
                List<ResultadoPedidoTMS> resultadoPedidos = await _pedidosTMSService.AltaPedidosTMS(_cliente, pedidos);

                return Ok(resultadoPedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
            }
        }

        /// <summary>
        /// Genera un rótulo en formato PDF a partir del ID del comprobante especificado.
        /// </summary>
        /// <param name="idComprobante">Identificador del comprobante para el cual se generará el rótulo.</param>
        /// <returns>
        /// Devuelve un string en base64 representando el archivo PDF del rótulo generado.
        /// </returns>
        /// <response code="200">PDF generado correctamente en base64.</response>
        /// <response code="409">Conflicto: los datos necesarios están incompletos o se han generado todos los rótulos posibles.</response>
        /// <response code="500">Error interno al generar el rótulo.</response>
        [HttpPost("GenerarRotulo")]
        public async Task<IActionResult> Generar(string idComprobante, [FromServices] IOptions<ApplicationSettings> settings)
        {
            try
            {
                string URL_QR_OptimoCamino = settings.Value.URL_QR_OptimoCamino;

                var base64Pdf = await _rotuloService.GenerarRotuloPdfAsync(_cliente, idComprobante, URL_QR_OptimoCamino);
                return Ok(base64Pdf);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al generar el rótulo.", detail = ex.Message });
            }
        }





        /***************************************************************************************************************************

                // DELETE: api/PedidosSaadis/Eliminar
                /// <summary>
                /// Elimina pedidos en Saadis.
                /// </summary>
                /// <remarks>
                /// Sample request:
                ///
                ///     DELETE /api/PedidosSaadis/Eliminar
                ///     [
                ///       {
                ///         "puntoDeVentaComprobante": "0001",
                ///         "numeroComprobante": "1234",
                ///         "letraComprobante": "A",
                ///       }
                ///     ]
                ///
                /// </remarks>
                /// <param name="pedidos"></param>
                /// <returns></returns>
                /// <response code="200">Elimina los pedidos</response>
                /// <response code="400">Si los pedidos no pudieron ser eliminados</response>            
                [Produces("application/json")]
                [HttpDelete("Eliminar")]
                public async Task<IActionResult> Delete([FromBody] IdentificadorPedidoSaadis[] pedidos)
                {
                    DapperDBContext _dapperSaadisDBContext = _dapperSaadisDBContextFactory.CreateDBContext();
                    SaadisDBContext _saadisDBContext = _saadisFactory.Create();

                    try
                    {
                        List<ResultadoPedidoSaadis> resultadoPedidos = new List<ResultadoPedidoSaadis>();

                        foreach (IdentificadorPedidoSaadis item in pedidos)
                        {
                            IDbConnection connection = _dapperSaadisDBContext.GetConnection();

                            string sqlText = "spEliminarPedidoSaadis @cbt_cenemi, @cbt_nrocbt, @cbt_letcbt";

                            IEnumerable<string> t = await connection.QueryAsync<string>(sqlText, new { cbt_cenemi = item.PuntoDeVentaComprobante.PadLeft(4, '0'), cbt_nrocbt = item.NumeroComprobante.PadLeft(8, '0'), cbt_letcbt = item.LetraComprobante });

                            string resultadoEliminacion = t.FirstOrDefault<string>();

                            ResultadoPedidoSaadis resultado = new ResultadoPedidoSaadis(item.PuntoDeVentaComprobante, item.NumeroComprobante, item.LetraComprobante, _cliente, "05");

                            if (!string.IsNullOrEmpty(resultadoEliminacion))
                            {
                                resultado.Resultado = "ERROR";
                                resultado.Mensaje = resultadoEliminacion;
                            }
                            else
                            {
                                resultado.Resultado = "OK";
                                resultado.Mensaje = string.Empty;
                            }

                            resultadoPedidos.Add(resultado);
                        }

                        return Ok(resultadoPedidos);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(551, ex.Message);
                        throw ex;
                    }
                }

                private async Task<List<ResultadoPedidoSaadis>> AltaPedidosSaadis(string cliente, PedidoSaadis[] pedidos, SaadisDBContext _saadisDBContext, DapperDBContext _dapperSaadisDBContext)
                {
                    string sqlText;

                    List<ResultadoPedidoSaadis> resultadoPedidos = new List<ResultadoPedidoSaadis>();

                    IDbConnection connection = _dapperSaadisDBContext.GetConnection();

                    sqlText = "spCreaCabeceraPedidoSaadis";

                    var p = new DynamicParameters();
                    p.Add("NumeroProcesoAPI", dbType: DbType.String, direction: ParameterDirection.Output, size: 12);

                    await connection.QueryAsync(sqlText, p, commandType: CommandType.StoredProcedure);

                    string rhr_nrohru = p.Get<string>("NumeroProcesoAPI");

                    foreach (PedidoSaadis item in pedidos)
                    {
                        ResultadoPedidoSaadis resultado = new ResultadoPedidoSaadis(item.PuntoDeVentaComprobante, item.NumeroComprobante, item.LetraComprobante, _cliente, "05");

                        if (string.IsNullOrEmpty(item.PuntoDeVentaComprobante) || string.IsNullOrEmpty(item.NumeroComprobante) || string.IsNullOrEmpty(item.LetraComprobante))
                        {
                            resultado.Resultado = "ERROR";
                            resultado.Mensaje = "Punto de venta, numero y letra del comprobante son obligatorios";
                        }
                        else
                        {
                            sqlText = "spValidaExistenciaPedidoSaadis @cbt_cenemi, @cbt_nrocbt, @cbt_letcbt";

                            IEnumerable<string> t = await connection.QueryAsync<string>(sqlText, new { cbt_cenemi = item.PuntoDeVentaComprobante.PadLeft(4,'0'), cbt_nrocbt = item.NumeroComprobante.PadLeft(8, '0'), cbt_letcbt = item.LetraComprobante });

                            string cbteOK = t.FirstOrDefault<string>();

                            if (cbteOK != "OK")
                            {
                                resultado.Resultado = "ERROR";
                                resultado.Mensaje = cbteOK;
                            }
                            else
                            {
                                RECHRDE cbte = new RECHRDE();

                                cbte.CBTCENEMI = item.PuntoDeVentaComprobante.PadLeft(4, '0');
                                cbte.CBTNROCBT = item.NumeroComprobante.PadLeft(8, '0');
                                cbte.CBTLETCBT = item.LetraComprobante;
                                cbte.RHDNROCLI = cliente;
                                cbte.CBTCODCBT = "05";
                                cbte.CBTFECCBT = item.FechaComprobante;
                                cbte.CBTREMITO = item.NumeroRemito;
                                cbte.CBTBULTOS = item.Bultos;
                                cbte.CBTKILOSN = item.KilosNetos;
                                cbte.CBTKILOSA = item.KilosAforados;
                                cbte.CBTMETRCB = item.MetrosCubicos;
                                cbte.CBTNOMRTE = item.NombreRemitente;
                                cbte.CBTLOCRTE = item.LocalidadRemitente;
                                cbte.CBTNOMDES = item.NombreDestinatario;
                                cbte.CBTDOMDES = item.DomicilioDestinatario;
                                cbte.CBTLOCDES = item.LocalidadDestinatario;
                                cbte.CBTNRODES = item.NumeroCalleDestinatario;
                                cbte.CBTDTODES = item.PisoDeptoDestinatario;
                                cbte.LCLCPOSTA = item.CodigoPostalDestinatario;
                                cbte.CBTNROHRU = rhr_nrohru;
                                cbte.CBTVADESE = item.ValorDeclarado;
                                cbte.CBTIMPCRE = item.ImporteContrareembolso;
                                cbte.IVACODIGO = item.TipoIvaRemitente;
                                cbte.CLTCUIT = item.CuitRemitente;
                                cbte.CBTSUBCLI = item.NumeroClienteDestino;
                                cbte.CBTDESCRI = item.ObservacionEnvio;
                                cbte.CBTPALLET = item.CantidadPallets;
                                cbte.CBTUNIDAD = item.CantidadUnidades;
                                if (item.FechaPosibleEntrega.HasValue)
                                {
                                    cbte.CBTFEPOEN = item.FechaPosibleEntrega;
                                }
                                else
                                {
                                    cbte.CBTFEPOEN = new DateTime(1900, 1, 1);
                                }
                                cbte.CBTDESCR2 = item.ObservacionEnvio_2;
                                cbte.CbtTivade = item.TipoIvasDestino;
                                cbte.CbtCuitde = item.CuitDestino;

                                cbte.CBTSUCORI = item.SucursalOrigen ?? "";
                                cbte.CBTEMAIL = item.Email;
                                cbte.CbtTelefo = item.Telefono;

                                cbte.CbtAdic1 = item.CampoAdicional1 ?? "";
                                cbte.CbtAdic2 = item.CampoAdicional2 ?? "";
                                cbte.CbtAdic3 = item.CampoAdicional3 ?? "";
                                cbte.CbtAdic4 = item.CampoAdicional4 ?? "";
                                cbte.CbtAdic5 = item.CampoAdicional5 ?? "";

                                cbte.CbtSenasa = item.Senasa;
                                cbte.CbtTipcar = item.TipoCarga;

                                cbte.CbtNomdesAlter = item.NombreDestinatarioAlternativo ?? ""; 
                                cbte.CbtTivadeAlter = item.TipoIvasDestinoAlternativo ?? "";
                                cbte.CbtCuitdeAlter = item.CuitDestinoAlternativo ?? "";
                                cbte.CbtTelefoAlter = item.TelefonoDestinoAlternativo ?? "";
                                cbte.CbtCodVerif = item.CodigoVerificadorEntrega ?? "";
                                cbte.CbtDatosEntrega = item.DatosEntrega ?? "";

                                _saadisDBContext.RECHRDEs.Add(cbte);

                                resultado.Resultado = "OK";
                            }
                        }

                        resultadoPedidos.Add(resultado);
                    }

                    try
                    {
                        _saadisDBContext.SaveChanges();

                        //generó bien en TMCABPED/TEDETPED. Ahora llama a un sp q resuelve los pesos y cantidades del pedido 
                        //y copia el pedido a MPOPEDC y MPOPEDD
                        sqlText = "spCompletaCabeceraPedidoSaadis @nroProceso";

                        await connection.QueryAsync(sqlText, new { nroProceso = rhr_nrohru });

                    }
                    catch (DbUpdateException e)
                    {
                        ResultadoPedidoSaadis resultado = new ResultadoPedidoSaadis();

                        resultado.Resultado = "ERROR global al guardar comprobantes o al completar cabecera de pedido Nro. " + rhr_nrohru;
                        resultado.Mensaje = e.InnerException.Message;
                    }

                    return resultadoPedidos;
                }


                // POST: api/PedidosSaadis/ConsultaComprobantes
                /// <summary>
                /// Consulta de Comprobantes SAADIS
                /// </summary>
                /// <remarks>
                /// Sample request:
                ///
                ///     POST /api/PedidosSaadis/ConsultaComprobantes
                ///     {
                ///       "centroEmisor": "",
                ///       "nroComprobante": "",
                ///       "letraComprobante": "",
                ///       "codigoDestinatario": "",
                ///       "destinatario": "",
                ///       "localidad": "",
                ///       "estadoComprobante": "",     // Puede indicar mas de un estado con los valores separados por coma ( , )
                ///       "fechaConformeDesde": "",    // Formato de fecha: yyyy-MM-dd o null para no definir valor en este campo
                ///       "fechaConformeHasta": "",    // Formato de fecha: yyyy-MM-dd o null para no definir valor en este campo
                ///       "fechaEmisionCbteDesde": "", // Formato de fecha: yyyy-MM-dd o null para no definir valor en este campo
                ///       "fechaEmisionCbteHasta": "", // Formato de fecha: yyyy-MM-dd o null para no definir valor en este campo
                ///       "cbtesObservados": true      // Valores permitidos: false o true
                ///     }
                ///
                /// </remarks>
                /// <param name="filtroConsultaCbtes"></param>
                /// <returns></returns>
                /// <response code="200">Retorna el resultado de la consulta de comprobantes del cliente</response>
                [Produces("application/json")]
                [HttpPost("ConsultaComprobantes")]
                public async Task<IActionResult> ConsultaComprobantes([FromBody] FiltroConsultaComprobantes filtroConsultaCbtes, [FromServices] IOptions<ApplicationSettings> settings)
                {
                    DapperDBContext _dapperDBContext = _dapperSaadisDBContextFactory.CreateDBContext();

                    try
                    {
                        string saadisDBName = settings.Value.SAADIS_DBName;

                        string sqlText = "SELECT DISTINCT CBT_REMITO, cbt_cenemi, cbt_nrocli, cbt_nrocbt, cbt_letcbt, cbt_codcbt, " +
                                         "cbt_letcbt + '-' + cbt_cenemi + '-' + cbt_nrocbt AS NumeroComprobante," +
                                         "LTRIM(RTRIM(substring(Ltrim(cbt_nomrte),1,15))) AS Remitente, " +
                                         "LTRIM(RTRIM(cbt_nomdes)) as NombreDestinatario, " +
                                         "LTRIM(RTRIM(cbt_domdes)) as DomicilioDestinatario, " +
                                         "LTRIM(RTRIM(cbt_locdes)) + ' - ' + LTRIM(RTRIM(lcl_cposta)) as LocalidadDestinatario, " +
                                         "CASE WHEN cbt_feccbt <> '19000101' THEN CONVERT(VARCHAR, cbt_feccbt, 103) ELSE '' END as FechaComprobante, " +
                                         "CASE WHEN cbt_feentr <> '19000101' THEN CONVERT(VARCHAR, cbt_feentr, 103) ELSE '' END AS FechaEntrega, " +
                                         "CASE WHEN cbt_fecrec <> '19000101' THEN CONVERT(VARCHAR, cbt_fecrec, 103) ELSE '' END AS FechaRecepcion, " +
                                         "CASE cbt_estado " +
                                         " WHEN '1' THEN 'En diagramacion de despacho'" +
                                         " WHEN '2' THEN 'En diagramacion de despacho'" +
                                         " WHEN '4' THEN 'En Viaje'" +
                                         " WHEN '5' THEN 'Entregado Destinatario'" +
                                         " WHEN 'C' THEN 'Entregado Destinatario'" +
                                         " WHEN '6' THEN 'Operación Anulada por remitente'" +
                                         " WHEN '7' THEN 'Pendiente de Resolucion remitente'" +
                                         " WHEN 'P' THEN 'En Deposito'" +
                                         " WHEN 'T' THEN 'Documentacion Rendida al remitente'" +
                                         " WHEN 'D' THEN 'Documentacion Rendida al remitente'" +
                                         " WHEN 'A' THEN 'Anulado'" +
                                         " END AS EstadoComprobante, " +
                                         "cbt_bultos AS Bultos, cbt_kilosn AS KilosNetos, cbt_vadese AS ValorDeclarado, cbt_kilosa AS KilosAforados, cbt_metrcb AS MetrosCubicos, cbt_feccbt ";


                        if (filtroConsultaCbtes.CbtesObservados)
                        {
                            sqlText = sqlText + "FROM " + saadisDBName + "..comprob c (NOLOCK) ";

                            //join solo para DoubleStar
                            //inner join " + saadisDBName + "..competap e (NOLOCK) on c.cbt_letcbt = e.ETA_LETCBT COLLATE Modern_Spanish_CI_AI " +
                            //                " and c.cbt_CENEMI = e.ETA_CENEMI  COLLATE Modern_Spanish_CI_AI and " +
                            //                " c.cbt_NROCBT = e.ETA_NROCBT COLLATE Modern_Spanish_CI_AI and c.cbt_NROCLI = e.ETA_NROCLI COLLATE Modern_Spanish_CI_AI and " +
                            //                " c.cbt_CODCBT = e. ETA_CODCBT COLLATE Modern_Spanish_CI_AI ";
                        }
                        else
                        {
                            sqlText = sqlText + "FROM " + saadisDBName + "..comprob c (NOLOCK)";

                        }

                        //Siempre comprobante 5														
                        string estados = string.Empty;
                        if (!string.IsNullOrEmpty(filtroConsultaCbtes.EstadoComprobante))
                        {
                            estados = filtroConsultaCbtes.EstadoComprobante.Insert(filtroConsultaCbtes.EstadoComprobante.Length, "'")
                                                                           .Insert(0, "'")
                                                                           .Replace(",", "','");
                        }

                        sqlText = sqlText + " WHERE cbt_nrocli='" + _cliente + "'  " +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.CodigoDestinatario) ? string.Empty : "AND cbt_subcli = '" + filtroConsultaCbtes.CodigoDestinatario.PadLeft(10, '0') + "'  ") +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.CentroEmisor) ? string.Empty : "AND cbt_cenemi = '" + filtroConsultaCbtes.CentroEmisor.PadLeft(4, '0') + "' ") +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.NroComprobante) ? string.Empty : "AND cbt_nrocbt = '" + filtroConsultaCbtes.NroComprobante.PadLeft(8, '0') + "' ") +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.LetraComprobante) ? string.Empty : "AND cbt_letcbt = '" + filtroConsultaCbtes.LetraComprobante + "' ") +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.Localidad) ? string.Empty : "AND cbt_locdes LIKE '%" + filtroConsultaCbtes.Localidad + "' ") +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.Destinatario) ? string.Empty : "AND cbt_nomdes LIKE '%" + filtroConsultaCbtes.Destinatario + "' ") +
                                            (string.IsNullOrEmpty(filtroConsultaCbtes.EstadoComprobante) ? string.Empty : "AND cbt_estado IN (" + estados + ") ");

                        //+ (filtroConsultaCbtes.CbtesObservados ? " AND eta_tipetap = '002'  " : string.Empty);

                        if (filtroConsultaCbtes.FechaConformeDesde != null)
                        {
                            sqlText = sqlText + " AND cbt_feentr >= '" + filtroConsultaCbtes.FechaConformeDesde.Value.ToString("yyyyMMdd") + "'  ";
                        }
                        if (filtroConsultaCbtes.FechaConformeHasta != null)
                        {
                            sqlText = sqlText + " AND cbt_feentr < DATEADD(d, 1, '" + filtroConsultaCbtes.FechaConformeHasta.Value.ToString("yyyyMMdd") + "')  ";

                            sqlText = sqlText + " AND cbt_feentr != '19000101' ";
                        }

                        if (filtroConsultaCbtes.FechaEmisionCbteDesde != null)
                        {
                            sqlText = sqlText + " AND cbt_feccbt >= '" + filtroConsultaCbtes.FechaEmisionCbteDesde.Value.ToString("yyyyMMdd") + "'  ";
                        }
                        if (filtroConsultaCbtes.FechaEmisionCbteHasta != null)
                        {
                            sqlText = sqlText + " AND cbt_feccbt < DATEADD(d, 1, '" + filtroConsultaCbtes.FechaEmisionCbteHasta.Value.ToString("yyyyMMdd") + "')  ";

                            sqlText = sqlText + " AND cbt_feccbt != '19000101' ";
                        }

                        //agregado para cliente que NO SON DoubleStar xq trabajan de una manera diferente, generando un cbte con otro codigo para el redespacho
                        sqlText = sqlText + " AND cbt_fecrec = (SELECT MAX(cbt_fecrec) FROM " + saadisDBName + "..comprob c1(NOLOCK)  WHERE c1.cbt_nrocli = c.cbt_nrocli  AND c1.cbt_cenemi = c.cbt_cenemi AND c1.CBT_NROCBT = c.CBT_NROCBT AND c1.CBT_LETCBT = c.CBT_LETCBT) ";

                        sqlText = sqlText + " ORDER BY cbt_feccbt asc";

                        IEnumerable<Comprobante> data;

                        using (IDbConnection connection = _dapperDBContext.GetConnection())
                        {
                            data = await connection.QueryAsync<Comprobante>(sqlText);

                            foreach (Comprobante cbte in data)
                            {
                                string sqlEtapas = "SELECT DISTINCT eta_tipetap + '-' + CASE eta_tipetap " +
                                    "WHEN '001' THEN 'En Distribución' " +
                                    "WHEN '002' THEN 'Obsv. en la entrega' " +
                                    "WHEN '004' THEN 'Resolvió Cliente Remitente' " +
                                    "WHEN '005' THEN 'Conformado' " +
                                    "WHEN '013' THEN 'Obsv. en la entrega' " +
                                    "WHEN '019' THEN 'Rendido en Planilla al Cliente' " +
                                    "WHEN '020' THEN 'Desconformación de cbte' " +
                                    "WHEN '024' THEN 'Rendido en Planilla al Cliente Contra reembolso' " +
                                    "WHEN '029' THEN 'Desconformación Planilla de rendición' " +
                                    "WHEN '035' THEN 'Facturado' " +
                                    "WHEN '039' THEN 'Entregado Espera de Conformar Remito' " +

                                    "WHEN '055' THEN 'Generacion de turno' " +
                                    "WHEN '056' THEN 'Generacion de turno' " +
                                    "WHEN '072' THEN 'Cambio fecha posible entrega' " +

                                    "WHEN '083' THEN 'App - En camino a destinatario' " +
                                    "WHEN '084' THEN 'App - En destino' " +
                                    "WHEN '085' THEN 'App - Entregado a redespacho' " +
                                    "WHEN '086' THEN 'App - Entregado' " +
                                    "WHEN '087' THEN 'App - No entrega' " +
                                    "WHEN '088' THEN 'App - Entrega con diferencia de bultos' " +

                                    "WHEN '093' THEN 'App - Obsrevaciones adicionales' " +
                                    "WHEN '094' THEN 'App - Asigno un turno' " +
                                    "WHEN '095' THEN 'App - Se encuentra en expreso' " +
                                    "WHEN '096' THEN 'App - En camino a suc expreso' " +
                                    "WHEN '097' THEN 'App - En cabecera expreso' " +

                                    "WHEN '011' THEN 'Informacion' " +
                                    "END AS DescEtapa, " +
                                    "CONVERT(VARCHAR, ETA_FECETA, 103) AS FechaEtapa, " +
                                    "LTRIM(RTRIM(eta_nrohr)) AS HojaDeRuta, " +
                                    "CASE WHEN eta_tipetap = '002' OR eta_tipetap = '013' THEN LTRIM(RTRIM(eta_justi)) + ' - ' + LTRIM(RTRIM(ISNULL(obs.COB_DESCRI,'')))  + ' - ' + LTRIM(RTRIM(CONVERT(VARCHAR(MAX),eta_obsv))) " +
                                    " ELSE" +
                                    "   CASE " +
                                    "     WHEN eta_tipetap = '019' THEN 'PLANILLA NRO: ' + LTRIM(RTRIM(CONVERT(VARCHAR(MAX),eta_obsv)))" +
                                    "     WHEN eta_tipetap IN ('011','004','005','019','035','055','056','072','083','084','085','086','087','088','093','094','095','096','097') THEN LTRIM(RTRIM(CONVERT(VARCHAR(MAX),eta_obsv)))" +
                                    "     ELSE ''" +
                                    "   END" +
                                    " END AS Observacion, ce.eta_feceta " +
                                                   //"ETA_LETCBT, ETA_CENEMI, ETA_NROCBT, ETA_NROCLI, ETA_CODCBT, ETA_ETAPA, ETA_TIPETAP, ETA_JUSTI, ETA_FECETA, " +
                                                   //"ETA_HORAETA, ETA_USUAETA, ETA_NROHR, cast(ETA_OBSV as varchar(500)) as eta_obsv, " +
                                                   //"ETA_FECTRA, ETA_COMDEV, ETA_EMIDEV, ETA_NRODEV, ETA_TIPDEV, " +
                                                   //"ETA_ENVIADO, ETA_PLAREC, ETA_ORIGEN, ETA_PLREMA, ETA_FECIER,  " +
                                                   //"c.CBT_FERECD AS CBT_FERECD, c.cbt_feentr as cbt_feentr, ce.eta_feceta " +
                                                   "FROM " + saadisDBName + "..competap ce (NOLOCK)" +
                                                   "inner join " + saadisDBName + "..etapas et (NOLOCK) on ce.eta_tipetap collate Modern_Spanish_CI_AI=et.codigo " +
                                                   "INNER JOIN " + saadisDBName + "..COMPROB c (NOLOCK) ON c.CBT_LETCBT collate Modern_Spanish_CI_AI= ce.ETA_LETCBT AND c.CBT_CENEMI collate Modern_Spanish_CI_AI= ce.ETA_CENEMI AND c.CBT_NROCBT collate Modern_Spanish_CI_AI= ce.ETA_NROCBT AND " +
                                                   "c.CBT_CODCBT collate Modern_Spanish_CI_AI = ce.ETA_CODCBT AND c.CBT_NROCLI collate Modern_Spanish_CI_AI= ce.ETA_NROCLI " +
                                                   "LEFT JOIN " + saadisDBName + "..CODOBSE obs (NOLOCK) ON obs.COB_CODIGO = ce.eta_justi " +
                                                   "WHERE eta_codcbt='" + cbte.cbt_codcbt + "' and eta_cenemi='" + cbte.cbt_cenemi + "' AND " +
                                                   " eta_letcbt='" + cbte.cbt_letcbt + "' AND eta_nrocbt='" + cbte.cbt_nrocbt + "' AND eta_nrocli='" + cbte.cbt_nrocli + "' " +
                                                   " AND ce.eta_tipetap IN ('001','002','004','005','011','013','019','020','024','029','035','039','055','056','072','083','084','085','086','087','088','093','094','095','096','097') " +
                                                   " ORDER BY ce.eta_feceta ASC";

                                IEnumerable<Etapa> etapas = await connection.QueryAsync<Etapa>(sqlEtapas);

                                cbte.EtapasComprobante = etapas.ToList();
                            }

                        }

                        return Ok(data);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(551, ex.Message);
                        throw ex;
                    }
                }

        *****************************************************************************************************************************************/


    }
}