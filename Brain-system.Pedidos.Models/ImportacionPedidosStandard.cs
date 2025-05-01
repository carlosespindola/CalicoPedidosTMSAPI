// Decompiled with JetBrains decompiler
// Type: ImportacionPedidos.ImportacionPedidosStandard
// Assembly: PedidosStandard, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E1405162-9855-4914-8873-B7CD0EBBE173
// Assembly location: C:\Trabajo\brainsys\SaadisNET_Stock\SaadisNet_Prueba\AssembliesInterfases\PedidosStandard.dll

using DALSaadNET;
using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ImportacionPedidos
{
  public class ImportacionPedidosStandard : IPedidosImportacion
  {
    private int _cantidadDeArchivos = 0;
    private const int PosInicialEmplazamiento = 0;
    private const int TamanioEmplazamiento = 3;
    private const int PosInicialAlmacen = 3;
    private const int TamanioAlmacen = 3;
    private const int PosInicialTipoPedido = 6;
    private const int TamanioTipoPedido = 3;
    private const int PosInicialLetra = 9;
    private const int TamanioLetra = 1;
    private const int PosInicialCentroEmisor = 10;
    private const int TamanioCentroEmisor = 4;
    private const int PosInicialNroPedido = 14;
    private const int TamanioNroPedido = 10;
    private const int PosInicialFechaPedido = 24;
    private const int TamanioFechaPedido = 8;
    private const int PosInicialCliente = 32;
    private const int TamanioCliente = 10;
    private const int PosInicialSubcliente = 42;
    private const int TamanioSubcliente = 10;
    private const int PosInicialRazonSocial = 52;
    private const int TamanioRazonSocial = 30;
    private const int PosInicialDomicilio = 82;
    private const int TamanioDomicilio = 30;
    private const int PosInicialLocalidad = 112;
    private const int TamanioLocalidad = 30;
    private const int PosInicialProvincia = 142;
    private const int TamanioProvincia = 30;
    private const int PosInicialCodigoPostal = 172;
    private const int TamanioCodigoPostal = 10;
    private const int PosInicialPedidoParticionado = 182;
    private const int TamanioPedidoParticionado = 1;
    private const int PosInicialFechaEntrega = 183;
    private const int TamanioFechaEntrega = 8;
    private const int PosInicialRefA = 191;
    private const int TamanioRefA = 30;
    private const int PosInicialRefB = 221;
    private const int TamanioRefB = 30;
    private const int PosInicialValorDeclarado = 251;
    private const int TamaniovalorDeclarado = 13;
    private const int PosInicialValorContraReembolso = 264;
    private const int TamanioValorContraReembolso = 13;
    private const int PosInicialCentroDeCosto = 277;
    private const int TamanioCentroDeCosto = 3;
    private const int PosInicialPesoTotal = 281;
    private const int TamanioPesoTotal = 7;
    private const int PosInicialNroLinea = 24;
    private const int TamanioNroLinea = 10;
    private const int PosInicialCompania = 34;
    private const int TamanioCompania = 3;
    private const int PosInicialProducto = 37;
    private const int TamanioProducto = 15;
    private const int PosInicialCantidad = 52;
    private const int TamanioCantidad = 13;
    private const int PosInicialPrecioUnitario = 65;
    private const int TamanioPrecioUnitario = 13;
    private const int PosInicialDespachoParcial = 78;
    private const int TamanioDespachoParcial = 1;
    private const int PosInicialLoteUnico = 79;
    private const int TamanioLoteUnico = 1;
    private const int PosInicialDigitoProd = 80;
    private const int TamanioDigitoProd = 2;
    private const int PosInicialEstadoProducto = 82;
    private const int TamanioEstadoProducto = 3;
    private const int PosInicialLote = 85;
    private const int TamanioLote = 15;
    private const int PosInicialSerie = 100;
    private const int TamanioSerie = 15;
    private string _nombreArchivoCabecera;
    private string _nombreArchivoDetalle;
    private string _estado;
    private string _errorMsg;

    public int CantidadDeArchivos
    {
      get
      {
        return this._cantidadDeArchivos;
      }
      set
      {
        this._cantidadDeArchivos = value;
      }
    }

    public string ArchivoCabecera
    {
      get
      {
        return this._nombreArchivoCabecera;
      }
      set
      {
        this._nombreArchivoCabecera = value;
      }
    }

    public string ArchivoDetalle
    {
      get
      {
        return this._nombreArchivoDetalle;
      }
      set
      {
        this._nombreArchivoDetalle = value;
      }
    }

    public string Estado
    {
      get
      {
        return this._estado;
      }
    }

    public string ErrorMsg
    {
      get
      {
        return this._errorMsg;
      }
    }

    public IList<PedidoImportado> CargarArchivos()
    {
      if (this._cantidadDeArchivos == 0 || this._cantidadDeArchivos > 2 || this._nombreArchivoCabecera.Length == 0 || this._cantidadDeArchivos == 2 && this._nombreArchivoDetalle.Length == 0)
      {
        this._estado = "I";
        this._errorMsg = "Debe definir el/los archivo(s) de entrada";
        return (IList<PedidoImportado>) null;
      }
      if (!File.Exists(this._nombreArchivoCabecera))
      {
        this._estado = "I";
        this._errorMsg = "No se encontró el archivo cabecera";
        return (IList<PedidoImportado>) null;
      }
      if (this._cantidadDeArchivos == 2 && !File.Exists(this._nombreArchivoDetalle))
      {
        this._estado = "I";
        this._errorMsg = "No se encontró el archivo detalle";
        return (IList<PedidoImportado>) null;
      }
      IList<PedidoImportado> pedidos = (IList<PedidoImportado>) new List<PedidoImportado>();
      Encoding encoding1 = Encoding.GetEncoding(1252);
      FileStream fileStream1 = File.Open(this._nombreArchivoCabecera, FileMode.Open, FileAccess.Read, FileShare.Read);
      using (StreamReader streamReader = new StreamReader((Stream) fileStream1, encoding1))
      {
        while (streamReader.Peek() >= 0)
          ((ICollection<PedidoImportado>) pedidos).Add(this.ProcesarLineaPedido(streamReader.ReadLine()));
      }
      fileStream1.Close();
      Encoding encoding2 = Encoding.GetEncoding(1252);
      FileStream fileStream2 = File.Open(this._nombreArchivoDetalle, FileMode.Open, FileAccess.Read, FileShare.Read);
      using (StreamReader streamReader = new StreamReader((Stream) fileStream2, encoding2))
      {
        while (streamReader.Peek() >= 0)
          this.CargarLineaDetalle(pedidos, streamReader.ReadLine());
      }
      fileStream2.Close();
      File.Delete(this._nombreArchivoCabecera);
      if (this._cantidadDeArchivos == 2)
        File.Delete(this._nombreArchivoDetalle);
      using (IEnumerator<PedidoImportado> enumerator1 = ((IEnumerable<PedidoImportado>) pedidos).GetEnumerator())
      {
        while (((IEnumerator) enumerator1).MoveNext())
        {
          PedidoImportado current1 = enumerator1.Current;
          DALImportacionPedidos importacionPedidos = new DALImportacionPedidos();
          if (!importacionPedidos.CheckDatosCabecera(current1))
          {
            current1.set_Causa("No se pudo verificar la cabecera del pedido");
          }
          else
          {
            using (IEnumerator<PedidoItemImportado> enumerator2 = ((IEnumerable<PedidoItemImportado>) current1.get_items()).GetEnumerator())
            {
              while (((IEnumerator) enumerator2).MoveNext())
              {
                PedidoItemImportado current2 = enumerator2.Current;
                importacionPedidos.CheckDatosItem(current1.get_IdPedido(), current2);
              }
            }
          }
        }
      }
      using (IEnumerator<PedidoImportado> enumerator1 = ((IEnumerable<PedidoImportado>) pedidos).GetEnumerator())
      {
        while (((IEnumerator) enumerator1).MoveNext())
        {
          PedidoImportado current1 = enumerator1.Current;
          bool flag = false;
          using (IEnumerator<PedidoItemImportado> enumerator2 = ((IEnumerable<PedidoItemImportado>) current1.get_items()).GetEnumerator())
          {
            while (((IEnumerator) enumerator2).MoveNext())
            {
              PedidoItemImportado current2 = enumerator2.Current;
              if (!flag && current2.get_Valido())
                flag = true;
              if (!current2.get_Valido() && current2.get_Causa() != "<BR>La línea ya existe en el pedido")
              {
                PedidoImportado pedidoImportado = current1;
                pedidoImportado.set_Causa(pedidoImportado.get_Causa() + "<BR>El pedido tiene al menos un item inválido");
                break;
              }
            }
          }
          if (!flag)
          {
            PedidoImportado pedidoImportado = current1;
            pedidoImportado.set_Causa(pedidoImportado.get_Causa() + "<BR>El pedido tiene al menos un item inválido");
          }
        }
      }
      return pedidos;
    }

    private PedidoImportado ProcesarLineaPedido(string sLineaPedido)
    {
      PedidoImportado pedidoImportado1 = new PedidoImportado();
      pedidoImportado1.set_Emplazamiento(sLineaPedido.Substring(0, 3));
      pedidoImportado1.set_Almacen(sLineaPedido.Substring(3, 3));
      pedidoImportado1.set_TipoPedido(sLineaPedido.Substring(6, 3));
      pedidoImportado1.set_Letra(sLineaPedido.Substring(9, 1));
      pedidoImportado1.set_CentroEmisor(sLineaPedido.Substring(10, 4));
      pedidoImportado1.set_NroPedido(sLineaPedido.Substring(14, 10));
      pedidoImportado1.set_FechaPedido(sLineaPedido.Substring(24, 8));
      pedidoImportado1.set_Cliente(sLineaPedido.Substring(32, 10));
      pedidoImportado1.set_Subcliente(sLineaPedido.Substring(42, 10));
      pedidoImportado1.set_RazonSocial(sLineaPedido.Substring(52, 30));
      pedidoImportado1.set_Domicilio(sLineaPedido.Substring(82, 30));
      pedidoImportado1.set_Localidad(sLineaPedido.Substring(112, 30));
      pedidoImportado1.set_Provincia(sLineaPedido.Substring(142, 30));
      pedidoImportado1.set_CodigoPostal(sLineaPedido.Substring(172, 10));
      pedidoImportado1.set_PedidoParticionado(sLineaPedido.Substring(182, 1));
      pedidoImportado1.set_FechaEntrega(sLineaPedido.Substring(183, 8));
      pedidoImportado1.set_ReferenciaA(sLineaPedido.Substring(191, 30));
      pedidoImportado1.set_ReferenciaB(sLineaPedido.Substring(221, 30));
      pedidoImportado1.set_Causa("");
      try
      {
        pedidoImportado1.set_ValorDeclarado(Convert.ToDouble(sLineaPedido.Substring(251, 13)));
        pedidoImportado1.set_ValorDeclarado(pedidoImportado1.get_ValorDeclarado() / 100.0);
      }
      catch
      {
        this._estado = "I";
        this._errorMsg = "Valor declarado inválido en archivo";
      }
      try
      {
        pedidoImportado1.set_ValorContraReembolso(Convert.ToDouble(sLineaPedido.Substring(264, 13)));
        PedidoImportado pedidoImportado2 = pedidoImportado1;
        pedidoImportado2.set_ValorContraReembolso(pedidoImportado2.get_ValorContraReembolso() / 100.0);
      }
      catch
      {
        this._estado = "I";
        this._errorMsg = "Valor contra reembolso inválido en archivo";
      }
      pedidoImportado1.set_CentroDeCosto(sLineaPedido.Substring(277, 3));
      try
      {
        pedidoImportado1.set_PesoTotal(Convert.ToDouble(sLineaPedido.Substring(281, 7)));
      }
      catch
      {
        this._estado = "I";
        this._errorMsg = "Peso inválido en archivo";
      }
      return pedidoImportado1;
    }

    private void CargarLineaDetalle(IList<PedidoImportado> pedidos, string sLineaDetalle)
    {
      string str1 = sLineaDetalle.Substring(0, 3);
      string str2 = sLineaDetalle.Substring(3, 3);
      string str3 = sLineaDetalle.Substring(6, 3);
      string str4 = sLineaDetalle.Substring(9, 1);
      string str5 = sLineaDetalle.Substring(10, 4);
      string str6 = sLineaDetalle.Substring(14, 10);
      PedidoItemImportado pedidoItemImportado1 = new PedidoItemImportado();
      try
      {
        pedidoItemImportado1.set_NroLinea(Convert.ToInt32(sLineaDetalle.Substring(24, 10)));
      }
      catch
      {
        pedidoItemImportado1.set_NroLinea(0);
      }
      pedidoItemImportado1.set_Cia(sLineaDetalle.Substring(34, 3));
      pedidoItemImportado1.set_CodigoArticulo(sLineaDetalle.Substring(37, 15));
      try
      {
        pedidoItemImportado1.set_Cantidad(Convert.ToDouble(sLineaDetalle.Substring(52, 13)));
        PedidoItemImportado pedidoItemImportado2 = pedidoItemImportado1;
        pedidoItemImportado2.set_Cantidad(pedidoItemImportado2.get_Cantidad() / 100.0);
      }
      catch
      {
        pedidoItemImportado1.set_Cantidad(0.0);
      }
      try
      {
        pedidoItemImportado1.set_PrecioUnitario(Convert.ToDouble(sLineaDetalle.Substring(65, 13)));
        PedidoItemImportado pedidoItemImportado2 = pedidoItemImportado1;
        pedidoItemImportado2.set_PrecioUnitario(pedidoItemImportado2.get_PrecioUnitario() / 100.0);
      }
      catch
      {
        pedidoItemImportado1.set_PrecioUnitario(0.0);
      }
      pedidoItemImportado1.set_DespParcial(sLineaDetalle.Substring(78, 1));
      pedidoItemImportado1.set_LoteUnico(sLineaDetalle.Substring(79, 1));
      pedidoItemImportado1.set_DigitoProd(sLineaDetalle.Substring(80, 2));
      pedidoItemImportado1.set_EstadoProducto(sLineaDetalle.Substring(82, 3));
      pedidoItemImportado1.set_Lote(sLineaDetalle.Substring(85, 15));
      pedidoItemImportado1.set_Serie(sLineaDetalle.Substring(100, 15));
      using (IEnumerator<PedidoImportado> enumerator = ((IEnumerable<PedidoImportado>) pedidos).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          PedidoImportado current = enumerator.Current;
          if (current.get_Emplazamiento() == str1 && current.get_Almacen() == str2 && (current.get_TipoPedido() == str3 && current.get_Letra() == str4) && current.get_CentroEmisor() == str5 && current.get_NroPedido() == str6)
          {
            current.AgregarItem(pedidoItemImportado1);
            break;
          }
          if (((ICollection<PedidoItemImportado>) current.get_items()).Count == 0)
            current.set_Causa("<BR>El pedido debe contener al menos un producto");
        }
      }
    }

    public bool ImportarPedido(PedidoImportado p)
    {
      DALImportacionPedidos importacionPedidos = new DALImportacionPedidos();
      try
      {
        importacionPedidos.ImportarPedido(p);
        return true;
      }
      catch (ControlledException ex)
      {
        this._errorMsg = ((Exception) ex).Message;
        return false;
      }
      catch (Exception ex)
      {
        this._errorMsg = ex.Message;
        return false;
      }
    }
  }
}
