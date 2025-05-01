using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using BrainSystem.Pedidos.DAL;
using Brainsystem.Pedidos.Models.DTO;
using BrainSystem.Pedidos.Model;
using Brainsystem.Pedidos.API.Models;
using Microsoft.Extensions.Options;
using System.Text;
using Asp.Versioning;

namespace Brainsystem.Pedidos.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidosWMS_OldController : ControllerBase
    {
        private BrainSystemDBContextFactory _factory;
        readonly IDBContextFactory _dapperDBContextFactory;
        private string _cliente;
        BrainSystemDBContext _brainDBContext;

        public PedidosWMS_OldController(IHttpContextAccessor contextAccessor, BrainSystemDBContextFactory factory, IDBContextFactory dapperDBContextFactory)
        {
            ClaimsPrincipal currentUser = contextAccessor.HttpContext.User;

            _cliente = currentUser.FindFirstValue("id");

            _factory = factory;

            _dapperDBContextFactory = dapperDBContextFactory;

            _brainDBContext = _factory.Create();
        }


        // GET: api/PedidosWMS/GetEstadosPedidosWMS
        /// <summary>
        /// Consulta de Estados de Pedidos WMS
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/PedidosWMS/GetEstadosPedidosWMS
        ///
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Retorna estados posibles de los pedidos WMS</response>
        [Produces("application/json")]
        [HttpGet("GetEstadosPedidosWMS")]
        public async Task<IActionResult> GetEstadosPedidosWMS()
        {
            List<KeyValuePair<string, string>> estados = new List<KeyValuePair<string, string>>();

            estados.Add(new KeyValuePair<string, string>("P", "Pendiente de procesar"));
            estados.Add(new KeyValuePair<string, string>("Z", "Pendiente de procesar con error"));
            estados.Add(new KeyValuePair<string, string>("E", "Procesado con error"));
            estados.Add(new KeyValuePair<string, string>("K", "Procesado"));
            estados.Add(new KeyValuePair<string, string>("A", "Anulado"));
            estados.Add(new KeyValuePair<string, string>("X", "Procesado con anulaciones"));

            return Ok(estados);
        }

        // POST: api/Pedidos/ConsultaCabeceraPedidosWMS
        /// <summary>
        /// Consulta de cabecera de Pedidos WMS
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Pedidos/ConsultaCabeceraPedidosWMS
        ///     {
        ///       "centroEmisor": "",
        ///       "nroComprobante": "",
        ///       "letraComprobante": "",
        ///       "codigoDestinatario": "",
        ///       "destinatario": "",
        ///       "estadoComprobante": "",     // Puede indicar mas de un estado con los valores separados por coma ( , )
        ///       "fechaEmisionCbteDesde": "", // Formato de fecha: yyyy-MM-dd o null para no definir valor en este campo
        ///       "fechaEmisionCbteHasta": ""  // Formato de fecha: yyyy-MM-dd o null para no definir valor en este campo
        ///     }
        ///
        /// </remarks>
        /// <param name="filtroConsultaPedidosWMS"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de la consulta de cabeceras de pedidos WMS</response>
        [Produces("application/json")]
        [HttpPost("ConsultaCabeceraPedidosWMS")]
        public async Task<IActionResult> ConsultaCabeceraPedidosWMS([FromBody] FiltroConsultaPedidosWMS filtroConsultaPedidosWMS)
        {
            try
            {
                CLIENTESACCESOAPI api = await _brainDBContext.CLIENTESACCESOAPIs.AsNoTracking().Where(a => a.CODIGOCLIENTE == _cliente).FirstAsync();

                if ((string.IsNullOrEmpty(filtroConsultaPedidosWMS.CentroEmisor) ||
                     string.IsNullOrEmpty(filtroConsultaPedidosWMS.LetraComprobante) ||
                     filtroConsultaPedidosWMS.NroComprobante == null) &&
                     (!string.IsNullOrEmpty(filtroConsultaPedidosWMS.CentroEmisor) ||
                     !string.IsNullOrEmpty(filtroConsultaPedidosWMS.LetraComprobante) ||
                     filtroConsultaPedidosWMS.NroComprobante != null))
                {
                    throw new Exception("Cuando ingresa Numero de Pedido, Centro Emisor y/o Letra, debe definir los 3 campos identificatorios del pedido");
                }

                if (string.IsNullOrEmpty(filtroConsultaPedidosWMS.CodigoDestinatario)
                    || string.IsNullOrEmpty(filtroConsultaPedidosWMS.Destinatario)
                    || string.IsNullOrEmpty(filtroConsultaPedidosWMS.EstadoComprobante))
                {
                    if (filtroConsultaPedidosWMS.FechaEmisionCbteDesde == null || filtroConsultaPedidosWMS.FechaEmisionCbteHasta == null)
                    {
                        throw new Exception("Debe ingresar rango de fecha");
                    }
                    else
                    {
                        if (((TimeSpan)((DateTime)filtroConsultaPedidosWMS.FechaEmisionCbteHasta - ((DateTime)filtroConsultaPedidosWMS.FechaEmisionCbteDesde).AddDays(1))).TotalDays > 10)
                        {
                            throw new Exception("El rango de fecha no puede ser superior a 10 días");
                        }
                    }
                }

                DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();

                IEnumerable<CabeceraPedidoWMS> data;
                using (IDbConnection connection = _dapperDBContext.GetConnection())
                {
                    data = await connection.QueryAsync<CabeceraPedidoWMS>("dbo.spGetCabecerasPedidosWMS @CLIENTE, @CIA, @centroEmisor, @nroComprobante, @letraComprobante, @codigoDestinatario, @destinatario, @estadoComprobante, @fechaEmisionCbteDesde, @fechaEmisionCbteHasta", 
                                                                                 new { cliente = api.CODIGOCLIENTE,
                                                                                       cia = api.CODIGO_CIA,
                                                                                       centroEmisor = filtroConsultaPedidosWMS.CentroEmisor,
                                                                                       nroComprobante = filtroConsultaPedidosWMS.NroComprobante,
                                                                                       letraComprobante = filtroConsultaPedidosWMS.LetraComprobante,
                                                                                       codigoDestinatario = filtroConsultaPedidosWMS.CodigoDestinatario,
                                                                                       destinatario = filtroConsultaPedidosWMS.Destinatario,
                                                                                       estadoComprobante = filtroConsultaPedidosWMS.EstadoComprobante,
                                                                                       fechaEmisionCbteDesde = filtroConsultaPedidosWMS.FechaEmisionCbteDesde,
                                                                                       fechaEmisionCbteHasta = filtroConsultaPedidosWMS.FechaEmisionCbteHasta});
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }

        // POST: api/Pedidos/ConsultaDetallePedidoWMS
        /// <summary>
        /// Consulta de detalle de Pedido WMS
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Pedidos/ConsultaDetallePedidoWMS
        ///     {
        ///       "centroEmisor": "",
        ///       "nroComprobante": "",
        ///       "letraComprobante": ""
        ///     }
        ///
        /// </remarks>
        /// <param name="filtroConsultaPedidoWMS"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de la consulta de cabeceras de pedidos WMS</response>
        [Produces("application/json")]
        [HttpPost("ConsultaDetallePedidoWMS")]
        public async Task<IActionResult> ConsultaDetallePedidoWMS([FromBody] FiltroConsultaDetallePedidoWMS filtroConsultaPedidoWMS)
        {
            try
            {
                CLIENTESACCESOAPI api = await _brainDBContext.CLIENTESACCESOAPIs.AsNoTracking().Where(a => a.CODIGOCLIENTE == _cliente).FirstAsync();
                if ((string.IsNullOrEmpty(filtroConsultaPedidoWMS.CentroEmisor) ||
                     string.IsNullOrEmpty(filtroConsultaPedidoWMS.LetraComprobante) ||
                     filtroConsultaPedidoWMS.NroComprobante == null))
                {
                    throw new Exception("Debe ingresar Numero de Pedido, Centro Emisor y Letra del pedido");
                }

                DetallePedidoWMS pedido = null;

                DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();

                IEnumerable<ResultadoDetallePedidoWMS> data;
                using (IDbConnection connection = _dapperDBContext.GetConnection())
                {
                    data = await connection.QueryAsync<ResultadoDetallePedidoWMS>("dbo.spGetDetallePedidoWMS @CLIENTE, @CIA, @letraComprobante, @centroEmisor, @nroComprobante",
                                                                                 new
                                                                                 {
                                                                                     cliente = api.CODIGOCLIENTE,
                                                                                     cia = api.CODIGO_CIA,
                                                                                     letraComprobante = filtroConsultaPedidoWMS.LetraComprobante,
                                                                                     centroEmisor = filtroConsultaPedidoWMS.CentroEmisor,
                                                                                     nroComprobante = filtroConsultaPedidoWMS.NroComprobante
                                                                                 });

                    if(data.Count() == 0)
                    {
                        throw new Exception("Pedido no encontrado");
                    }

                    foreach (ResultadoDetallePedidoWMS item in data)
                    {
                        if(pedido == null)
                        {
                            pedido = new DetallePedidoWMS();
                            pedido.DetalleLinea = new List<DetLinea>();

                            pedido.Letra = item.CAT_PEDIDO;
                            pedido.CentroEmisor = item.SUC_PEDIDO;
                            pedido.NroPedido = item.NRO_PEDIDO;
                            pedido.EstadoPedido = item.ESTADOPEDIDO;
                        }

                        DetLinea linea = new DetLinea();
                        linea.Linea = item.LINEA;
                        linea.Cia = item.CIA;
                        linea.Producto = item.PRODUCTO;
                        linea.Cantidad = item.CANTIDAD;

                        if(string.IsNullOrEmpty(item.CantidadLote) && string.IsNullOrEmpty(item.CantidadSerie))
                        {
                            linea.Pickeado = item.CantidadPickeada;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.CantidadLote))
                            {
                                //tiene lote
                                List<L> l = new List<L>();
                                
                                string[] lotes = item.CantidadLote.Split('|');
                                for (int i = 0; i < lotes.Length; i++)
                                {
                                    string[] lote = lotes[i].Split('~');

                                    L l0 = new L();
                                    l0.Lote = lote[0];
                                    l0.Cantidad = decimal.Parse(lote[1]);

                                    l.Add(l0);
                                }

                                linea.Pickeado = l;
                            }
                            else
                            {
                                //es con serie
                                List<S> s = new List<S>();

                                string[] series = item.CantidadLote.Split('|');
                                for (int i = 0; i < series.Length; i++)
                                {
                                    S s0 = new S();
                                    s0.Serie = series[0];

                                    s.Add(s0);
                                }
                                linea.Pickeado = s;
                            }
                        }

                        linea.ConError = item.CantidadConError;
                        linea.Cancelado = item.CantidadCancelada;
                        linea.EnProcesoPicking = item.CantidadEnProcesoDePicking;

                        pedido.DetalleLinea.Add(linea);
                    }
                }

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }
    }
}
