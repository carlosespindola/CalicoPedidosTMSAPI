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
using Asp.Versioning;

namespace BrainSystem.Pedidos.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidosWMSController : ControllerBase
    {
        private BrainSystemDBContextFactory _factory;
        readonly IDBContextFactory _dapperDBContextFactory;
        private string _cliente;
        BrainSystemDBContext _brainDBContext;

        public PedidosWMSController(IHttpContextAccessor contextAccessor, BrainSystemDBContextFactory factory, IDBContextFactory dapperDBContextFactory)
        {
            ClaimsPrincipal currentUser = contextAccessor.HttpContext.User;

            _cliente = currentUser.FindFirstValue("id");

            _factory = factory;

            _dapperDBContextFactory = dapperDBContextFactory;

            _brainDBContext = _factory.Create();
        }


        // POST: api/Pedidos/Alta
        /// <summary>
        /// Alta de pedidos
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Pedidos/Alta
        ///     [
        ///         {
        ///     	            "emplazamiento": "001",
        ///             	    "almacen": "206",
        ///             	    "tipoPedido": "206",
        ///             	    "categoriaPedido": "R",
        ///             	    "sucursalPedido": "0073",
        ///             	    "numeroPedido": "00002568",
        ///             	    "fechaPedido": "2019-04-17", // Formato de fecha: yyyy-MM-dd
        ///             	    "cliente": "0000000206",
        ///             	    "subcliente": "B000424",
        ///             	    "razonSocial": "MATTIACCI LAURA",
        ///             	    "condicionIVA": "1",
        ///             	    "cuit":"30123456789",
        ///             	    "domicilio": "HAEDO 1962",
        ///             	    "localidad": "CAPITAL FEDERAL",
        ///             	    "provincia": "BUE",
        ///             	    "codigoPostal": "1000",
        ///             	    "email": "tanavalentina@hotmail.com",
        ///             	    "telefonos": "1549382344//",
        ///             	    "particiona": "N",
        ///             	    "fechaEntrega": "2019-04-24", // Formato de fecha: yyyy-MM-dd
        ///             	    "refA": "",
        ///             	    "refB": "VALOR DECLARADO $460 ABONA WAM",
        ///             	    "valorFactura": 8374.00,
        ///             	    "valorContrareembolso": null,
        ///             	    "pesoTotal": null,
        ///             	    "centroDeCosto": "",
        ///             	    "detalleProductos":
        ///                     [
        ///             		    {
        ///     			            "linea": 1,
        ///             			    "compania": "WAM",
        ///             			    "producto": "00000CL78061053",
        ///             			    "cantidad": 1,
        ///             			    "loteUnico": "",
        ///             			    "despachoParcial": "0",
        ///             			    "estadoProducto": "",
        ///             			    "lote": "",
        ///             			    "serie": ""
        ///                     },
        ///     		        {
        ///     			    	    "linea": 2,
        ///     			    	    "compania": "WAM",
        ///     			    	    "producto": "000SOPPE080200E",
        ///     			    	    "cantidad": 2,
        ///     			    	    "loteUnico": "",
        ///     			    	    "despachoParcial": "0",
        ///     			    	    "estadoProducto": "",
        ///     			    	    "lote": "",
        ///     			    	    "serie": ""
        ///     			    }
        ///     		   ]
        ///     	}
        ///     ]
        ///
        /// </remarks>
        /// <param name="pedidos"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de el alta de cada pedido</response>
        [Produces("application/json")]
        [HttpPost("Alta")]
        public async Task<IActionResult> Post([FromBody] Pedido[] pedidos)
        {
            DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();
            BrainSystemDBContext _brainDBContext = _factory.Create();

            try
            {
                List<ResultadoPedido> resultadoPedidos = await ProcesaPedidos("A", pedidos, _brainDBContext, _dapperDBContext);

                return Ok(resultadoPedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }


        // PUT: api/Pedidos/Modificar
        /// <summary>
        /// Modifica pedidos existentes.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Pedidos/Modificar
        ///     [
        ///         {
        ///     	            "emplazamiento": "001",
        ///             	    "almacen": "206",
        ///             	    "tipoPedido": "206",
        ///             	    "categoriaPedido": "R",
        ///             	    "sucursalPedido": "0073",
        ///             	    "numeroPedido": "00002568",
        ///             	    "fechaPedido": "2019-04-17", // Formato de fecha: yyyy-MM-dd
        ///             	    "cliente": "0000000206",
        ///             	    "subcliente": "B000424",
        ///             	    "razonSocial": "MATTIACCI LAURA",
        ///             	    "condicionIVA": "1",
        ///             	    "cuit":"30123456789",
        ///             	    "domicilio": "HAEDO 1962",
        ///             	    "localidad": "CAPITAL FEDERAL",
        ///             	    "provincia": "BUE",
        ///             	    "codigoPostal": "1000",
        ///             	    "email": "tanavalentina@hotmail.com",
        ///             	    "telefonos": "1549382344//",
        ///             	    "particiona": "N",
        ///             	    "fechaEntrega": "2019-04-24", // Formato de fecha: yyyy-MM-dd
        ///             	    "refA": "",
        ///             	    "refB": "VALOR DECLARADO $460 ABONA WAM",
        ///             	    "valorFactura": 8374.00,
        ///             	    "valorContrareembolso": null,
        ///             	    "pesoTotal": null,
        ///             	    "centroDeCosto": "",
        ///             	    "detalleProductos":
        ///                     [
        ///             		    {
        ///     			            "linea": 1,
        ///             			    "compania": "WAM",
        ///             			    "producto": "00000CL78061053",
        ///             			    "cantidad": 1,
        ///             			    "loteUnico": "",
        ///             			    "despachoParcial": "0",
        ///             			    "estadoProducto": "",
        ///             			    "lote": "",
        ///             			    "serie": ""
        ///                     },
        ///     		        {
        ///     			    	    "linea": 2,
        ///     			    	    "compania": "WAM",
        ///     			    	    "producto": "000SOPPE080200E",
        ///     			    	    "cantidad": 2,
        ///     			    	    "loteUnico": "",
        ///     			    	    "despachoParcial": "0",
        ///     			    	    "estadoProducto": "",
        ///     			    	    "lote": "",
        ///     			    	    "serie": ""
        ///     			    }
        ///     		   ]
        ///     	}
        ///     ]
        ///
        /// </remarks>
        /// <param name="pedido"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de el alta de cada pedido</response>         
        [Produces("application/json")]
        [HttpPut("Modificar")]
        public async Task<IActionResult> Put([FromBody] Pedido[] pedidos)
        {
            DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();
            BrainSystemDBContext _brainDBContext = _factory.Create();

            try
            {
                List<ResultadoPedido> resultadoPedidos = await ProcesaPedidos("M", pedidos, _brainDBContext, _dapperDBContext);

                return Ok(resultadoPedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }


        // DELETE: api/Pedidos/Eliminar
        /// <summary>
        /// Elimina pedidos.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/Pedidos/Eliminar
        ///     [
        ///         {
        ///     	           "emplazamiento": "001",
        ///             	    "almacen": "206",
        ///             	    "tipoPedido": "206",
        ///             	    "categoriaPedido": "R",
        ///             	    "sucursalPedido": "0073",
        ///             	    "numeroPedido": "00002568"
        ///     	  }
        ///     ]
        ///
        /// </remarks>
        /// <param name="pedidos"></param>
        /// <returns></returns>
        /// <response code="200">Elimina los comprobantes</response>
        /// <response code="400">Si hubo un error al intentar eliminar los comprobantes</response>            
        [Produces("application/json")]
        [HttpDelete("Eliminar")]
        public async Task<IActionResult> Delete([FromBody] IdentificadorPedido[] pedidos)
        {
            DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();
            BrainSystemDBContext _brainDBContext = _factory.Create();

            try
            {
                List<ResultadoPedido> resultadoPedidos = new List<ResultadoPedido>();

                foreach (IdentificadorPedido item in pedidos)
                {
                    ResultadoEliminacionPedido resultadoValidacion = new ResultadoEliminacionPedido();

                    IDbConnection connection = _dapperDBContext.GetConnection();

                    string sqlText = "spEliminarPedido @emplazamiento, @almacen, @tip_pedido, @cat_pedido, @suc_pedido, @nro_pedido";

                    IEnumerable<ResultadoEliminacionPedido> t = await connection.QueryAsync<ResultadoEliminacionPedido>(sqlText, new { emplazamiento = item.Emplazamiento, almacen = item.Almacen, tip_pedido = item.TipoPedido, cat_pedido = item.CategoriaPedido, suc_pedido = item.SucursalPedido, nro_pedido = item.NumeroPedido });

                    resultadoValidacion = t.FirstOrDefault<ResultadoEliminacionPedido>();

                    ResultadoPedido resultado = new ResultadoPedido(item.Emplazamiento, item.Almacen, item.TipoPedido, item.CategoriaPedido, item.SucursalPedido, decimal.Parse(item.NumeroPedido));

                    if (!string.IsNullOrEmpty(resultadoValidacion.Resultado))
                    {

                        resultado.Resultado = "ERROR";
                        resultado.Mensaje = resultadoValidacion.Resultado;
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


        // GET: api/Pedidos/GetRubros
        /// <summary>
        /// Consulta de Rubros
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Pedidos/ConsultaStock
        ///
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Retorna Los rubros y subrubros del cliente</response>
        [Produces("application/json")]
        [HttpGet("GetRubros")]
        public async Task<IActionResult> GetRubros()
        {
            DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();
            BrainSystemDBContext _brainDBContext = _factory.Create();

            try
            {
                List<Rubro> rubros = new List<Rubro>();

                CLIENTESACCESOAPI api = await _brainDBContext.CLIENTESACCESOAPIs.AsNoTracking().Where(a => a.CODIGOCLIENTE == _cliente).FirstAsync();

                using (IDbConnection connection = _dapperDBContext.GetConnection())
                {
                    string sqlText = "SELECT DISTINCT LTRIM(RTRIM(r.CODIGO)) AS CodigoRubro, LTRIM(RTRIM(r.DESCRIP)) AS Rubro, LTRIM(RTRIM(s.CODIGO)) AS CodigoSubrubro, LTRIM(RTRIM(s.DESCRIP)) AS Subrubro" +
                                 " FROM dbo.MATITEC p" +
                                 " INNER JOIN dbo.rubros r ON r.CODIGO = p.RUBRO" +
                                 " INNER JOIN dbo.SUBRUBRO s ON s.RUBRO = r.CODIGO" +
                                 " WHERE ITCIA = '" + api.CODIGO_CIA + "'" +
                                 " ORDER BY Rubro, Subrubro";

                    IEnumerable<RubrosData> data = await connection.QueryAsync<RubrosData>(sqlText);

                    rubros = (from n
                                            in (from c in data select new { RubroCd = c.CodigoRubro, RubroDs = c.Rubro }).Distinct()
                                                              select new Rubro
                                                              (
                                                                  n.RubroCd, n.RubroDs,
                                                                  (from j
                                                                   in data
                                                                   where j.CodigoRubro == n.RubroCd
                                                                   select new Subrubro(j.CodigoSubrubro, j.Subrubro)).ToList<Subrubro>()
                                                              )
                                        ).ToList<Rubro>();
                }

                return Ok(rubros);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }


        // GET: api/Pedidos/GetEstadosComprobantes
        /// <summary>
        /// Consulta de Estados de Comprobantes SAADIS
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Pedidos/GetEstadosComprobantes
        ///
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Retorna Los rubros y subrubros del cliente</response>
        [Produces("application/json")]
        [HttpGet("GetEstadosComprobantes")]
        public async Task<IActionResult> GetEstadosComprobantes()
        {
            List<KeyValuePair<string, string>> estados = new List<KeyValuePair<string, string>>();

            estados.Add(new KeyValuePair<string, string>("P", "En deposito"));
            estados.Add(new KeyValuePair<string, string>("1", "En diagramacion de despacho"));
            estados.Add(new KeyValuePair<string, string>("2", "En diagramacion de despacho"));
            estados.Add(new KeyValuePair<string, string>("4", "En Reparto"));
            estados.Add(new KeyValuePair<string, string>("E", "En Reparto"));
            estados.Add(new KeyValuePair<string, string>("R", "En Reparto"));
            estados.Add(new KeyValuePair<string, string>("5", "Entregado Destinatario"));
            estados.Add(new KeyValuePair<string, string>("C", "Entregado Destinatario"));
            estados.Add(new KeyValuePair<string, string>("6", "Operación Anulada por remitente"));
            estados.Add(new KeyValuePair<string, string>("7", "Pendiente de Resolucion remitente"));
            estados.Add(new KeyValuePair<string, string>("D", "Documentacion Rendida al remitente"));
            estados.Add(new KeyValuePair<string, string>("T", "Documentacion Rendida al remitente"));
            estados.Add(new KeyValuePair<string, string>("A", "Anulados"));

            return Ok(estados);
        }

        // POST: api/Pedidos/ConsultaStock
        /// <summary>
        /// Consulta de Stock
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Pedidos/ConsultaStock
        ///
        /// </remarks>
        /// <param name="filtroConsultaStock"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de la conslta de stock de productos del cliente</response>
        [Produces("application/json")]
        [HttpPost("ConsultaStock")]
        public async Task<IActionResult> ConsultaStock([FromBody] FiltroConsultaStock filtroConsultaStock)
        {
            DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();
            BrainSystemDBContext _brainDBContext = _factory.Create();

            try
            {
                CLIENTESACCESOAPI api = await _brainDBContext.CLIENTESACCESOAPIs.AsNoTracking().Where(a => a.CODIGOCLIENTE == _cliente).FirstAsync();

                string sqlText = "SELECT ubcia as Compania,ubprod as Producto, LTRIM(RTRIM(itdesc)) as Descripcion, sum(ubcfisi) as Cantidad, ubiest+' - '+LTRIM(RTRIM(eidesc)) as Estado " +
                             "FROM maaubic (NOLOCK) join .mateinc (NOLOCK) ON ubiest = eiesin " +
                             "left join matitec (NOLOCK) ON (ubprod=itprod AND ubcia=itcia ) " +
                             "WHERE ubcia <> '' AND ubcia='" + api.CODIGO_CIA + "' " +
                             (string.IsNullOrEmpty(filtroConsultaStock.CodigoProducto) ? string.Empty : " AND ubprod='" + filtroConsultaStock.CodigoProducto.PadLeft(15, '0') + "' ") +
                             (string.IsNullOrEmpty(filtroConsultaStock.CodigoRubro) ? string.Empty : " AND rubro='" + filtroConsultaStock.CodigoRubro + "' ") +
                             (string.IsNullOrEmpty(filtroConsultaStock.CodigoSubrubro) ? string.Empty : " AND subrub='" + filtroConsultaStock.CodigoSubrubro + "' ") +
                             "GROUP BY ubcia,ubprod,itdesc,ubiest, eidesc " +
                             "ORDER BY ubprod";

                IEnumerable<Stock> data;

                using (IDbConnection connection = _dapperDBContext.GetConnection())
                {
                    data = await connection.QueryAsync<Stock>(sqlText);
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }

        // POST: api/Pedidos/ConsultaKardex
        /// <summary>
        /// Consulta e Kardex
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Pedidos/ConsultaKardex
        /// </remarks>
        /// <param name="filtroConsultaKardex"></param>
        /// <returns></returns>
        /// <response code="200">Retorna el resultado de la consulta de Kardex del cliente</response>
        [Produces("application/json")]
        [HttpPost("ConsultaKardex")]
        public async Task<IActionResult> ConsultaKardex([FromBody] FiltroConsultaKardex filtroConsultaKardex)
        {
            DapperDBContext _dapperDBContext = _dapperDBContextFactory.CreateDBContext();
            BrainSystemDBContext _brainDBContext = _factory.Create();

            try
            {
                CLIENTESACCESOAPI api = await _brainDBContext.CLIENTESACCESOAPIs.AsNoTracking().Where(a => a.CODIGOCLIENTE == _cliente).FirstAsync();

                string sqlText = "SELECT trcia AS compania, trprod AS producto, " +
                                 "LTRIM(RTRIM(matitec.itdesc)) as descripcion, " +
                                 "trcant AS Cantidad, trtipope AS TipoOperacion, " +
                                 "CASE trtipope WHEN 'PIC' THEN LTRIM(RTRIM(MATTPED.TPDESC)) " +
                                 "WHEN 'REC' THEN LTRIM(RTRIM(MATTREC.TRDESC)) " +
                                 "ELSE LTRIM(RTRIM(MATTREC.TRDESC)) END AS DescOp, " +
                                 "CONVERT(VARCHAR, trfetr, 103) AS FechaComprobante, " +
                                 "CASE trtipope WHEN 'AJU' THEN '' ELSE trcomp END AS NroComprobante " +
                                 "FROM mactrac(NOLOCK) " +
                                 "INNER JOIN matitec ON itcia = trcia and itprod = trprod " +
                                 "LEFT JOIN MATTPED ON MACTRAC.TRCTRC = MATTPED.TPTIPE " +
                                 "LEFT JOIN MATTREC ON MACTRAC.TRCTRC = MATTREC.TRTIRE  " +
                                 "WHERE trcia='" + api.CODIGO_CIA + "' " +
                                 (string.IsNullOrEmpty(filtroConsultaKardex.CodigoProducto) ? string.Empty : " AND trprod='" + filtroConsultaKardex.CodigoProducto.PadLeft(15, '0') + "' ") +
                                 "ORDER BY trprod, trfetr";

                IEnumerable<Kardex> data;

                using (IDbConnection connection = _dapperDBContext.GetConnection())
                {
                    data = await connection.QueryAsync<Kardex>(sqlText);
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(551, ex.Message);
                throw ex;
            }
        }

        private async Task<List<ResultadoPedido>> ProcesaPedidos(string _operacion, Pedido[] pedidos, BrainSystemDBContext _brainDBContext, DapperDBContext _dapperDBContext)
        {
            List<ResultadoPedido> resultadoPedidos = new List<ResultadoPedido>();

            foreach (Pedido item in pedidos)
            {
                ResultadoValidacionPedido resultadoValidacion = new ResultadoValidacionPedido();

                IDbConnection connection = _dapperDBContext.GetConnection();

                string sqlText = "spCheckCabeceraPedido @operacion, @emplazamiento, @almacen, @tip_pedido, @cat_pedido, @suc_pedido, @nro_pedido, @cliente, @subcliente, @razon_social, @domicilio, @localidad, @codigoPostal, @telefono, @contacto";

                IEnumerable<ResultadoValidacionPedido> t = await connection.QueryAsync<ResultadoValidacionPedido>(sqlText, new { operacion = _operacion, emplazamiento = item.Emplazamiento, almacen = item.Almacen, tip_pedido = item.TipoPedido, cat_pedido = item.CategoriaPedido, suc_pedido = item.SucursalPedido, nro_pedido = item.NumeroPedido, cliente = item.Cliente, subcliente = item.Subcliente.PadLeft(10, '0'), razon_social = item.RazonSocial, domicilio = item.Domicilio, localidad = item.Localidad, codigoPostal = item.CodigoPostal, telefono = item.Telefonos, contacto = item.Email });

                resultadoValidacion = t.FirstOrDefault<ResultadoValidacionPedido>();

                ResultadoPedido resultado;

                if (!string.IsNullOrEmpty(resultadoValidacion.ResultadoValidacion))
                {
                    resultado = new ResultadoPedido(item.Emplazamiento, item.Almacen, item.TipoPedido, item.CategoriaPedido, item.SucursalPedido, decimal.Parse(item.NumeroPedido));

                    resultado.Resultado = "ERROR";
                    resultado.Mensaje = resultadoValidacion.ResultadoValidacion;
                }
                else
                {
                    TMCABPED cabecera = new TMCABPED();

                    cabecera.EMPLAZA = item.Emplazamiento;
                    cabecera.ALMACEN = item.Almacen;
                    cabecera.TIPPEDIDO = item.TipoPedido;
                    cabecera.CATPEDIDO = item.CategoriaPedido;
                    cabecera.SUCPEDIDO = item.SucursalPedido;
                    cabecera.NROPEDIDO = decimal.Parse(item.NumeroPedido);
                    cabecera.FECHA = item.FechaPedido;// new DateTime(int.Parse(item.FechaPedido.Substring(6, 4)), int.Parse(item.FechaPedido.Substring(3, 2)), int.Parse(item.FechaPedido.Substring(0, 2)));
                    cabecera.CLIENTE = item.Cliente;
                    cabecera.SUBCLIENTE = item.Subcliente.PadLeft(10, '0');
                    cabecera.RAZONSOC = item.RazonSocial;

                    cabecera.DOMICILIO = item.Domicilio;
                    cabecera.LOCALIDAD = item.Localidad;
                    cabecera.PROVINCIA = item.Provincia;
                    cabecera.CP = item.CodigoPostal;

                    cabecera.PARTICIONA = (item.Particiona == "S");
                    if (item.FechaEntrega != null)
                    {
                        cabecera.FENTREGA = item.FechaEntrega; // (string.IsNullOrEmpty(item.FechaEntrega) ? DateTime.Today : new DateTime(int.Parse(item.FechaEntrega.Substring(6, 4)), int.Parse(item.FechaEntrega.Substring(3, 2)), int.Parse(item.FechaEntrega.Substring(0, 2))));}
                    }
                    cabecera.REFA = item.RefA;
                    cabecera.REFB = item.RefB;
                    cabecera.VALFACTUR = item.ValorFactura;
                    cabecera.VALCONTRA = (item.ValorContrareembolso != null ? item.ValorContrareembolso : null);
                    cabecera.UNIDADES = 0;
                    cabecera.ITEMSPED = 0;
                    cabecera.PESOTOT = (item.PesoTotal != null ? item.PesoTotal : null);
                    cabecera.CENCOS = item.CentroDeCosto;
                    cabecera.AREAMUELLE = resultadoValidacion.AreaMuelle;

                    _brainDBContext.TMCABPEDs.Add(cabecera);

                    foreach (var lineaProducto in item.DetalleProductos)
                    {
                        TMDETPED lineaPedido = new TMDETPED();

                        lineaPedido.EMPLAZA = cabecera.EMPLAZA;
                        lineaPedido.ALMACEN = cabecera.ALMACEN;
                        lineaPedido.TIPPEDIDO = cabecera.TIPPEDIDO;
                        lineaPedido.CATPEDIDO = cabecera.CATPEDIDO;
                        lineaPedido.SUCPEDIDO = cabecera.SUCPEDIDO;
                        lineaPedido.NROPEDIDO = cabecera.NROPEDIDO;

                        lineaPedido.LINEA = lineaProducto.Linea;
                        lineaPedido.CIA = lineaProducto.Compania;
                        lineaPedido.PRODUCTO = lineaProducto.Producto.PadLeft(15, '0');
                        lineaPedido.CANTIDAD = lineaProducto.Cantidad;
                        lineaPedido.DESPPARCI = lineaProducto.DespachoParcial;
                        lineaPedido.LOTEUNICO = lineaProducto.LoteUnico;
                        lineaPedido.ESTPROD = lineaProducto.EstadoProducto;
                        lineaPedido.LOTE = lineaProducto.Lote;
                        lineaPedido.SERIE = lineaProducto.Serie;

                        //ver is manejar el tema de la cantidad si serie <> ""

                        _brainDBContext.TMDETPEDs.Add(lineaPedido);

                    }

                    //actualiza datos subcliente, solo si cambio algun dato
                    if (resultadoValidacion.SubclienteNuevo || resultadoValidacion.Domicilio != item.Domicilio || resultadoValidacion.Localidad != item.Localidad || resultadoValidacion.CodigoPostal != item.CodigoPostal || resultadoValidacion.CambioDeAreaMuelle == "S" || resultadoValidacion.Telefonos != item.Telefonos || resultadoValidacion.Contacto != item.Email)
                    {
                        SUBCLIEN subcliente = await _brainDBContext.SUBCLIENs.Where(s => s.CLIENTE == cabecera.CLIENTE && s.CODIGO == cabecera.SUBCLIENTE).FirstOrDefaultAsync();

                        if (subcliente == null)
                        {
                            subcliente = new SUBCLIEN();
                            subcliente.CLIENTE = cabecera.CLIENTE;
                            subcliente.CODIGO = cabecera.SUBCLIENTE;
                            subcliente.RAZONSOC = cabecera.RAZONSOC;
                            subcliente.PROVINCIA = string.Empty;
                            subcliente.TARVIS = string.Empty;
                            subcliente.METVAL = string.Empty;
                            subcliente.METFAC = string.Empty;
                            subcliente.CONCOM = string.Empty;
                            subcliente.METAFO = string.Empty;
                            subcliente.CODVAL = string.Empty;
                            subcliente.TARCAR = string.Empty;
                            subcliente.FAX = string.Empty;
                            subcliente.ZONA = string.Empty;
                            subcliente.VENDEDOR = string.Empty;
                            subcliente.DIASVTO = 0;
                            subcliente.PAIS = string.Empty;
                            subcliente.RESERVA = string.Empty;
                            subcliente.PPICK = 0;
                            subcliente.HEDESDE = string.Empty;
                            subcliente.HEHASTA = string.Empty;
                            subcliente.NRORUC = string.Empty;
                            subcliente.FEDESDE = string.Empty;
                            subcliente.FEHASTA = string.Empty;
                            subcliente.AREAMUELLE = resultadoValidacion.AreaMuelle;

                            _brainDBContext.Add(subcliente);
                        }

                        if (item.CondicionIVA != null)
                        {
                            subcliente.IVA = item.CondicionIVA.ToString();
                        }
                        if (!string.IsNullOrEmpty(item.CUIT))
                        {
                            subcliente.CUIT = item.CUIT;
                        }
                        subcliente.DOMICILIO = item.Domicilio;
                        subcliente.LOCALIDAD = item.Localidad;
                        subcliente.CP = item.CodigoPostal;
                        subcliente.TELEFONO = item.Telefonos;
                        subcliente.CONTACTO = item.Email;
                        
                    }

                    resultado = new ResultadoPedido(cabecera.EMPLAZA, cabecera.ALMACEN, cabecera.TIPPEDIDO, cabecera.CATPEDIDO, cabecera.SUCPEDIDO, cabecera.NROPEDIDO);

                    try
                    {
                        _brainDBContext.SaveChanges();

                        //generó bien en TMCABPED/TEDETPED. Ahora llama a un sp q resuelve los pesos y cantidades del pedido 
                        //y copia el pedido a MPOPEDC y MPOPEDD
                        sqlText = "spSumarizaYCopiaPedidoA_MP @emplazamiento, @almacen, @tip_pedido, @cat_pedido, @suc_pedido, @nro_pedido";

                        await connection.QueryAsync(sqlText, new { emplazamiento = item.Emplazamiento, almacen = item.Almacen, tip_pedido = item.TipoPedido, cat_pedido = item.CategoriaPedido, suc_pedido = item.SucursalPedido, nro_pedido = item.NumeroPedido });

                        resultadoValidacion = t.FirstOrDefault<ResultadoValidacionPedido>();

                        resultado.Resultado = "OK";
                        resultado.Mensaje = string.Empty;
                    }
                    catch (DbUpdateException e)
                    {

                        resultado.Resultado = "ERROR";
                        resultado.Mensaje = e.InnerException.Message;
                    }
                }

                resultadoPedidos.Add(resultado);
            }

            return resultadoPedidos;
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
                                                                                 new
                                                                                 {
                                                                                     cliente = api.CODIGOCLIENTE,
                                                                                     cia = api.CODIGO_CIA,
                                                                                     centroEmisor = filtroConsultaPedidosWMS.CentroEmisor,
                                                                                     nroComprobante = filtroConsultaPedidosWMS.NroComprobante,
                                                                                     letraComprobante = filtroConsultaPedidosWMS.LetraComprobante,
                                                                                     codigoDestinatario = filtroConsultaPedidosWMS.CodigoDestinatario,
                                                                                     destinatario = filtroConsultaPedidosWMS.Destinatario,
                                                                                     estadoComprobante = filtroConsultaPedidosWMS.EstadoComprobante,
                                                                                     fechaEmisionCbteDesde = filtroConsultaPedidosWMS.FechaEmisionCbteDesde,
                                                                                     fechaEmisionCbteHasta = filtroConsultaPedidosWMS.FechaEmisionCbteHasta
                                                                                 });
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

                    if (data.Count() == 0)
                    {
                        throw new Exception("Pedido no encontrado");
                    }

                    foreach (ResultadoDetallePedidoWMS item in data)
                    {
                        if (pedido == null)
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

                        if (string.IsNullOrEmpty(item.CantidadLote) && string.IsNullOrEmpty(item.CantidadSerie))
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