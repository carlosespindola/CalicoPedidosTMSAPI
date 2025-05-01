// Decompiled with JetBrains decompiler
// Type: ImportacionPedidos.DALImportacionPedidos
// Assembly: PedidosStandard, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E1405162-9855-4914-8873-B7CD0EBBE173
// Assembly location: C:\Trabajo\brainsys\SaadisNET_Stock\SaadisNet_Prueba\AssembliesInterfases\PedidosStandard.dll

using DALSaadNET;
using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ImportacionPedidos
{
  public class DALImportacionPedidos
  {
    public virtual bool CheckDatosCabecera(PedidoImportado p)
    {
      SqlParameter[] sqlParameterArray = new SqlParameter[27]
      {
        new SqlParameter("@CodigoEmplazamiento", (object) p.get_Emplazamiento()),
        new SqlParameter("@CodigoAlmacen", (object) p.get_Almacen()),
        new SqlParameter("@CodigoTipoPedido", (object) p.get_TipoPedido()),
        new SqlParameter("@LetraPedido", (object) p.get_Letra()),
        new SqlParameter("@NroSucPedido", (object) p.get_CentroEmisor()),
        new SqlParameter("@NroPedido", (object) p.get_NroPedido()),
        new SqlParameter("@CodigoCliente", (object) p.get_Cliente()),
        new SqlParameter("@CodigoSubCliente", (object) p.get_Subcliente()),
        new SqlParameter("@RazonSocial", (object) p.get_RazonSocial()),
        new SqlParameter("@Domicilio", (object) p.get_Domicilio()),
        new SqlParameter("@CP", (object) p.get_CodigoPostal()),
        new SqlParameter("@Localidad", (object) p.get_Localidad()),
        new SqlParameter("@Provincia", (object) p.get_Provincia()),
        new SqlParameter("@CodigoCentroDeCosto", (object) p.get_CentroDeCosto()),
        new SqlParameter("@IdPedido", SqlDbType.Int),
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
      };
      sqlParameterArray[14].Value = (object) DBNull.Value;
      sqlParameterArray[14].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[15] = new SqlParameter("@IdEmplazamiento", SqlDbType.Int);
      sqlParameterArray[15].Value = (object) DBNull.Value;
      sqlParameterArray[15].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[16] = new SqlParameter("@IdAlmacen", SqlDbType.Int);
      sqlParameterArray[16].Value = (object) DBNull.Value;
      sqlParameterArray[16].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[17] = new SqlParameter("@IdTipoPedido", SqlDbType.Int);
      sqlParameterArray[17].Value = (object) DBNull.Value;
      sqlParameterArray[17].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[18] = new SqlParameter("@IdCliente", SqlDbType.Int);
      sqlParameterArray[18].Value = (object) DBNull.Value;
      sqlParameterArray[18].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[19] = new SqlParameter("@IdSubCliente", SqlDbType.Int);
      sqlParameterArray[19].Value = (object) DBNull.Value;
      sqlParameterArray[19].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[20] = new SqlParameter("@IdLocalidad", SqlDbType.Int);
      sqlParameterArray[20].Value = (object) DBNull.Value;
      sqlParameterArray[20].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[21] = new SqlParameter("@IdProvincia", SqlDbType.Int);
      sqlParameterArray[21].Value = (object) DBNull.Value;
      sqlParameterArray[21].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[22] = new SqlParameter("@IdPais", SqlDbType.Int);
      sqlParameterArray[22].Value = (object) DBNull.Value;
      sqlParameterArray[22].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[23] = new SqlParameter("@IdCentroDeCosto", SqlDbType.Int);
      sqlParameterArray[23].Value = (object) DBNull.Value;
      sqlParameterArray[23].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[24] = new SqlParameter("@AreaMuelle", SqlDbType.VarChar, 6);
      sqlParameterArray[24].Value = (object) new string(' ', 6);
      sqlParameterArray[24].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[25] = new SqlParameter("@IdAreaMuelle", SqlDbType.Int);
      sqlParameterArray[25].Value = (object) DBNull.Value;
      sqlParameterArray[25].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[26] = new SqlParameter("@Resultado", SqlDbType.VarChar, 1000);
      sqlParameterArray[26].Value = (object) "";
      sqlParameterArray[26].Direction = ParameterDirection.InputOutput;
      try
      {
        SQLHelper.ExecuteNonQuery((string) SQLHelper.CONN_STRING_NON_DTC, CommandType.StoredProcedure, "spCheckCabeceraPedido", sqlParameterArray);
        p.set_IdPedido(Convert.ToInt32(sqlParameterArray[14].Value == DBNull.Value ? (object) 0 : sqlParameterArray[14].Value));
        p.set_IdEmplazamaiento(Convert.ToInt32(sqlParameterArray[15].Value == DBNull.Value ? (object) 0 : sqlParameterArray[15].Value));
        p.set_IdAlmacen(Convert.ToInt32(sqlParameterArray[16].Value == DBNull.Value ? (object) 0 : sqlParameterArray[16].Value));
        p.set_IdTipoPedido(Convert.ToInt32(sqlParameterArray[17].Value == DBNull.Value ? (object) 0 : sqlParameterArray[17].Value));
        p.set_IdCliente(Convert.ToInt32(sqlParameterArray[18].Value == DBNull.Value ? (object) 0 : sqlParameterArray[18].Value));
        p.set_IdSubcliente(Convert.ToInt32(sqlParameterArray[19].Value == DBNull.Value ? (object) 0 : sqlParameterArray[19].Value));
        p.set_IdLocalidad(Convert.ToInt32(sqlParameterArray[20].Value == DBNull.Value ? (object) 0 : sqlParameterArray[20].Value));
        p.set_IdProvincia(Convert.ToInt32(sqlParameterArray[21].Value == DBNull.Value ? (object) 0 : sqlParameterArray[21].Value));
        p.set_IdPais(Convert.ToInt32(sqlParameterArray[22].Value == DBNull.Value ? (object) 0 : sqlParameterArray[22].Value));
        p.set_IdCentroDeCosto(Convert.ToInt32(sqlParameterArray[23].Value == DBNull.Value ? (object) 0 : sqlParameterArray[23].Value));
        p.set_AreaMuelle(Convert.ToString(sqlParameterArray[24].Value == DBNull.Value ? (object) string.Empty : sqlParameterArray[24].Value));
        p.set_IdAreaMuelle(Convert.ToInt32(sqlParameterArray[25].Value == DBNull.Value ? (object) 0 : sqlParameterArray[25].Value));
        PedidoImportado pedidoImportado = p;
        pedidoImportado.set_Causa(pedidoImportado.get_Causa() + Convert.ToString(sqlParameterArray[26].Value == DBNull.Value ? (object) string.Empty : sqlParameterArray[26].Value));
        return true;
      }
      catch
      {
        return false;
      }
    }

    public virtual bool CheckDatosItem(int IdPedido, PedidoItemImportado i)
    {
      SqlParameter[] sqlParameterArray = new SqlParameter[12]
      {
        new SqlParameter("@IdPedido", (object) IdPedido),
        new SqlParameter("@NroLinea", (object) i.get_NroLinea()),
        new SqlParameter("@Cia", (object) i.get_Cia()),
        new SqlParameter("@CodigoProducto", (object) i.get_CodigoArticulo()),
        new SqlParameter("@CodigoEstadoProducto", (object) i.get_EstadoProducto()),
        new SqlParameter("@IdProducto", SqlDbType.Int),
        null,
        null,
        null,
        null,
        null,
        null
      };
      sqlParameterArray[5].Value = (object) DBNull.Value;
      sqlParameterArray[5].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[6] = new SqlParameter("@DescProducto", SqlDbType.VarChar, 50);
      sqlParameterArray[6].Value = (object) new string(' ', 50);
      sqlParameterArray[6].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[7] = new SqlParameter("@Peso", SqlDbType.Decimal);
      sqlParameterArray[7].Value = (object) DBNull.Value;
      sqlParameterArray[7].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[8] = new SqlParameter("@Largo", SqlDbType.Decimal);
      sqlParameterArray[8].Value = (object) DBNull.Value;
      sqlParameterArray[8].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[9] = new SqlParameter("@Ancho", SqlDbType.Decimal);
      sqlParameterArray[9].Value = (object) DBNull.Value;
      sqlParameterArray[9].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[10] = new SqlParameter("@Alto", SqlDbType.Decimal);
      sqlParameterArray[10].Value = (object) DBNull.Value;
      sqlParameterArray[10].Direction = ParameterDirection.InputOutput;
      sqlParameterArray[11] = new SqlParameter("@Resultado", SqlDbType.VarChar, 1000);
      sqlParameterArray[11].Value = (object) "";
      sqlParameterArray[11].Direction = ParameterDirection.InputOutput;
      try
      {
        SQLHelper.ExecuteNonQuery((string) SQLHelper.CONN_STRING_NON_DTC, CommandType.StoredProcedure, "spCheckItemPedido", sqlParameterArray);
        if (sqlParameterArray[5].Value != DBNull.Value)
        {
          i.set_idProducto(Convert.ToInt32(sqlParameterArray[5].Value));
          i.set_DescArticulo(sqlParameterArray[6].Value.ToString());
          i.set_Peso(Convert.ToDouble(sqlParameterArray[7].Value));
          i.set_VolumenUnitario(Convert.ToDouble(sqlParameterArray[8].Value) * Convert.ToDouble(sqlParameterArray[9].Value) * Convert.ToDouble(sqlParameterArray[10].Value));
          i.set_Causa(sqlParameterArray[11].Value.ToString());
        }
        else
          i.set_Causa("<BR>Producto no encontrado");
        return true;
      }
      catch
      {
        return false;
      }
    }

    public virtual bool ImportarPedido(PedidoImportado p)
    {
      StringBuilder stringBuilder = new StringBuilder();
      using (IEnumerator<PedidoItemImportado> enumerator = ((IEnumerable<PedidoItemImportado>) p.get_items()).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          PedidoItemImportado current = enumerator.Current;
          if (current.get_Valido())
          {
            stringBuilder.Append(current.get_NroLinea().ToString() + "|");
            stringBuilder.Append(current.get_idProducto().ToString() + "|");
            stringBuilder.Append(current.get_Cantidad().ToString() + "|");
            stringBuilder.Append(current.get_LoteUnico() + "|");
            stringBuilder.Append(current.get_Lote() + "|");
            stringBuilder.Append(current.get_Serie() + "|");
            stringBuilder.Append(current.get_DespParcial() + "|");
            stringBuilder.Append(current.get_EstadoProducto() + "#");
          }
        }
      }
      SqlParameter[] sqlParameterArray = new SqlParameter[25]
      {
        new SqlParameter("@IdPedidoExistente", (object) p.get_IdPedido()),
        new SqlParameter("@IdEmplazamiento", (object) p.get_IdEmplazamaiento()),
        new SqlParameter("@IdAlmacen", (object) p.get_IdAlmacen()),
        new SqlParameter("@IdTipoPedido", (object) p.get_IdTipoPedido()),
        new SqlParameter("@LetraPedido", (object) p.get_Letra()),
        new SqlParameter("@NroSucPedido", (object) p.get_CentroEmisor()),
        new SqlParameter("@NroPedido", (object) p.get_NroPedido()),
        new SqlParameter("@FechaPedido", (object) p.get_FechaPedido()),
        new SqlParameter("@FechaEntrega", (object) p.get_FechaEntrega()),
        new SqlParameter("@IdCliente", (object) p.get_IdCliente()),
        new SqlParameter("@IdSubCliente", (object) p.get_IdSubcliente()),
        new SqlParameter("@RazonSocial", (object) p.get_RazonSocial()),
        new SqlParameter("@Domicilio", (object) p.get_Domicilio()),
        new SqlParameter("@CP", (object) p.get_CodigoPostal()),
        p.get_IdLocalidad() != 0 ? new SqlParameter("@IdLocalidad", (object) p.get_IdLocalidad()) : new SqlParameter("@IdLocalidad", (object) DBNull.Value),
        p.get_IdProvincia() != 0 ? new SqlParameter("@IdProvincia", (object) p.get_IdProvincia()) : new SqlParameter("@IdProvincia", (object) DBNull.Value),
        p.get_IdPais() != 0 ? new SqlParameter("@IdPais", (object) p.get_IdPais()) : new SqlParameter("@IdPais", (object) DBNull.Value),
        p.get_IdAreaMuelle() != 0 ? new SqlParameter("@IdAreaMuelle", (object) p.get_IdAreaMuelle()) : new SqlParameter("@IdAreaMuelle", (object) DBNull.Value),
        new SqlParameter("@RefA", (object) p.get_ReferenciaA()),
        new SqlParameter("@RefB", (object) p.get_ReferenciaB()),
        p.get_IdCentroDeCosto() != 0 ? new SqlParameter("@IdCentroDeCosto", (object) p.get_IdCentroDeCosto()) : new SqlParameter("@IdCentroDeCosto", (object) DBNull.Value),
        new SqlParameter("@EntregaParcial", (object) p.get_PedidoParticionado()),
        new SqlParameter("@ImporteFactura", (object) p.get_ValorDeclarado()),
        new SqlParameter("@ContraReembolso", (object) p.get_ValorContraReembolso()),
        new SqlParameter("@Items", (object) stringBuilder.ToString())
      };
      try
      {
        SQLHelper.ExecuteNonQuery((string) SQLHelper.CONN_STRING_NON_DTC, CommandType.StoredProcedure, "spImportarPedido", sqlParameterArray);
        return true;
      }
      catch (SqlException ex)
      {
        throw BSExceptionManager.HandleException((Exception) ex, "Importación de pedidos");
      }
      catch (Exception ex)
      {
        throw new ApplicationException("Error inesperado en base de datos", ex);
      }
    }
  }
}
