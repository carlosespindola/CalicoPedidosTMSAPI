using Calico.PedidosTMS.Models.DTO;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Calico.PedidosTMS.API.Services
{
    public class PedidosTMSService
    {
        private readonly Calico.PedidosTMS.DAL.DapperContexts.CalicoInterfazDapperContext _calicoInterfazContext;
        private readonly Calico.PedidosTMS.DAL.DapperContexts.CalicoMasterDapperContext _calicoMasterContext;

        public PedidosTMSService(
            Calico.PedidosTMS.DAL.DapperContexts.CalicoInterfazDapperContext calicoInterfazContext,
            Calico.PedidosTMS.DAL.DapperContexts.CalicoMasterDapperContext calicoMasterContext)
        {
            _calicoInterfazContext = calicoInterfazContext;
            _calicoMasterContext = calicoMasterContext;
        }

        /// <summary>
        /// Procesa el alta de pedidos TMS llamando a los stored procedures correspondientes
        /// </summary>
        /// <param name="cliente">Código del cliente</param>
        /// <param name="pedidos">Array de pedidos a procesar</param>
        /// <returns>Lista de resultados del procesamiento de cada pedido</returns>
        public async Task<List<ResultadoPedidoTMS>> AltaPedidosTMS(
            string cliente,
            PedidoTMSDesdeAPI[] pedidos)
        {
            List<ResultadoPedidoTMS> resultadoPedidos = new List<ResultadoPedidoTMS>();

            IDbConnection connectionDefault = _calicoInterfazContext.CreateConnection();
            IDbConnection connectionMaster = _calicoMasterContext.CreateConnection();

            foreach (PedidoTMSDesdeAPI item in pedidos)
            {
                ResultadoPedidoTMS resultado = new ResultadoPedidoTMS(
                    item.PuntoDeVentaComprobante,
                    item.NumeroComprobante,
                    item.LetraComprobante);

                try
                {
                    // Validar campos obligatorios
                    if (string.IsNullOrEmpty(item.PuntoDeVentaComprobante) ||
                        string.IsNullOrEmpty(item.NumeroComprobante) ||
                        string.IsNullOrEmpty(item.LetraComprobante))
                    {
                        resultado.Resultado = "ERROR";
                        resultado.Mensaje = "Punto de venta, número y letra del comprobante son obligatorios";
                        resultadoPedidos.Add(resultado);
                        continue;
                    }

                    // Paso 1: Llamar al SP de alta/modificación de destinatario en DBMaster
                    if (!string.IsNullOrEmpty(item.NumeroClienteDestino))
                    {
                        var parametersDestinatario = new DynamicParameters();
                        parametersDestinatario.Add("@Cliente", cliente);
                        parametersDestinatario.Add("@NumeroClienteDestino", item.NumeroClienteDestino);
                        parametersDestinatario.Add("@NombreDestinatario", item.NombreDestinatario);
                        parametersDestinatario.Add("@DomicilioDestinatario", item.DomicilioDestinatario);
                        parametersDestinatario.Add("@NumeroCalleDestinatario", item.NumeroCalleDestinatario);
                        parametersDestinatario.Add("@PisoDeptoDestinatario", item.PisoDeptoDestinatario);
                        parametersDestinatario.Add("@LocalidadDestinatario", item.LocalidadDestinatario);
                        parametersDestinatario.Add("@CodigoPostalDestinatario", item.CodigoPostalDestinatario);
                        parametersDestinatario.Add("@TipoIvasDestino", item.TipoIvasDestino);
                        parametersDestinatario.Add("@CuitDestino", item.CuitDestino);

                        await connectionMaster.ExecuteAsync(
                            "spAM_DestinatarioDesdeAPI",
                            parametersDestinatario,
                            commandType: CommandType.StoredProcedure);
                    }

                    // Paso 2: Llamar al SP de alta de pedido en DefaultConnection
                    var parametersPedido = new DynamicParameters();
                    parametersPedido.Add("@Cliente", cliente);
                    parametersPedido.Add("@PuntoDeVentaComprobante", item.PuntoDeVentaComprobante);
                    parametersPedido.Add("@NumeroComprobante", item.NumeroComprobante);
                    parametersPedido.Add("@LetraComprobante", item.LetraComprobante);
                    parametersPedido.Add("@FechaComprobante", item.FechaComprobante);
                    parametersPedido.Add("@NumeroRemito", item.NumeroRemito);
                    parametersPedido.Add("@Bultos", item.Bultos);
                    parametersPedido.Add("@KilosNetos", item.KilosNetos);
                    parametersPedido.Add("@KilosAforados", item.KilosAforados);
                    parametersPedido.Add("@MetrosCubicos", item.MetrosCubicos);
                    parametersPedido.Add("@NombreRemitente", item.NombreRemitente);
                    parametersPedido.Add("@LocalidadRemitente", item.LocalidadRemitente);
                    parametersPedido.Add("@NombreDestinatario", item.NombreDestinatario);
                    parametersPedido.Add("@DomicilioDestinatario", item.DomicilioDestinatario);
                    parametersPedido.Add("@NumeroCalleDestinatario", item.NumeroCalleDestinatario);
                    parametersPedido.Add("@PisoDeptoDestinatario", item.PisoDeptoDestinatario);
                    parametersPedido.Add("@LocalidadDestinatario", item.LocalidadDestinatario);
                    parametersPedido.Add("@CodigoPostalDestinatario", item.CodigoPostalDestinatario);
                    parametersPedido.Add("@ValorDeclarado", item.ValorDeclarado);
                    parametersPedido.Add("@ImporteContrareembolso", item.ImporteContrareembolso);
                    parametersPedido.Add("@TipoIvaRemitente", item.TipoIvaRemitente);
                    parametersPedido.Add("@CuitRemitente", item.CuitRemitente);
                    parametersPedido.Add("@NumeroClienteDestino", item.NumeroClienteDestino);
                    parametersPedido.Add("@ObservacionEnvio", item.ObservacionEnvio);
                    parametersPedido.Add("@CantidadPallets", item.CantidadPallets);
                    parametersPedido.Add("@CantidadUnidades", item.CantidadUnidades);
                    parametersPedido.Add("@FechaPosibleEntrega", item.FechaPosibleEntrega);
                    parametersPedido.Add("@ObservacionEnvio_2", item.ObservacionEnvio_2);
                    parametersPedido.Add("@TipoIvasDestino", item.TipoIvasDestino);
                    parametersPedido.Add("@CuitDestino", item.CuitDestino);
                    parametersPedido.Add("@ObservacionAdicionalEnvio", item.ObservacionAdicionalEnvio);
                    parametersPedido.Add("@SucursalOrigen", item.SucursalOrigen);
                    parametersPedido.Add("@Email", item.Email);
                    parametersPedido.Add("@Telefono", item.Telefono);
                    parametersPedido.Add("@CampoAdicional1", item.CampoAdicional1);
                    parametersPedido.Add("@CampoAdicional2", item.CampoAdicional2);
                    parametersPedido.Add("@CampoAdicional3", item.CampoAdicional3);
                    parametersPedido.Add("@CampoAdicional4", item.CampoAdicional4);
                    parametersPedido.Add("@CampoAdicional5", item.CampoAdicional5);
                    parametersPedido.Add("@Senasa", item.Senasa);
                    parametersPedido.Add("@TipoCarga", item.TipoCarga);
                    parametersPedido.Add("@NombreDestinatarioAlternativo", item.NombreDestinatarioAlternativo);
                    parametersPedido.Add("@TipoIvasDestinoAlternativo", item.TipoIvasDestinoAlternativo);
                    parametersPedido.Add("@CuitDestinoAlternativo", item.CuitDestinoAlternativo);
                    parametersPedido.Add("@TelefonoDestinoAlternativo", item.TelefonoDestinoAlternativo);
                    parametersPedido.Add("@CodigoVerificadorEntrega", item.CodigoVerificadorEntrega);
                    parametersPedido.Add("@DatosEntrega", item.DatosEntrega);

                    // Parámetros de salida
                    parametersPedido.Add("@Resultado", dbType: DbType.String, direction: ParameterDirection.Output, size: 10);
                    parametersPedido.Add("@Mensaje", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

                    await connectionDefault.ExecuteAsync(
                        "spAltaPedidoTMSDesdeAPI",
                        parametersPedido,
                        commandType: CommandType.StoredProcedure);

                    // Obtener los valores de salida
                    resultado.Resultado = parametersPedido.Get<string>("@Resultado");
                    resultado.Mensaje = parametersPedido.Get<string>("@Mensaje");
                }
                catch (Exception ex)
                {
                    resultado.Resultado = "ERROR";
                    resultado.Mensaje = $"Error al procesar el pedido: {ex.Message}";
                }

                resultadoPedidos.Add(resultado);
            }

            return resultadoPedidos;
        }
    }
}
