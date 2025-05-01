using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
  [Serializable]
  public class PedidoItemImportado
  {
    private int _nroLinea;
    private string _cia;
    private string _codigoArticulo;
    private string _descArticulo;
    private double _cantidad;
    private double _precioUnitario;
    private string _despParcial;
    private string _loteUnico;
    private string _digitoProd;
    private string _lote;
    private string _serie;
    private string _estadoProducto;
    private double _peso;
    private double _volumen;
    private int _idProducto;
    private bool _valido;
    private string _causa;

    public PedidoItemImportado() 
    {
      _valido = false;
      _causa = "No chequeado";
    }

    public int NroLinea
    {
      get
      {
        return _nroLinea;
      }
      set
      {
        _nroLinea = value;
      }
    }

    public string Cia
    {
      get
      {
        return _cia;
      }
      set
      {
        _cia = value.Trim(); ;
      }
    }

    public string CodigoArticulo
    {
      get
      {
        return _codigoArticulo;
      }
      set
      {
        _codigoArticulo = value.Trim().PadLeft(15, '0');
      }
    }

    public string DescArticulo
    {
      get { return _descArticulo; }
      set { _descArticulo = value; }
    }

    public double Cantidad
    {
      get
      {
        return _cantidad;
      }
      set
      {
        _cantidad = value;
      }
    }

    public double PrecioUnitario
    {
      get
      {
        return _precioUnitario;
      }
      set
      {
        _precioUnitario = value;
      }
    }

    public string DespParcial
    {
      get
      {
        return _despParcial;
      }
      set
      {
        _despParcial = value;
      }
    }

    public string LoteUnico
    {
      get
      {
        return _loteUnico;
      }
      set
      {
        _loteUnico = value;
      }
    }

    public string DigitoProd
    {
      get
      {
        return _digitoProd;
      }
      set
      {
        _digitoProd = value;
      }
    }

    public string Lote
    {
      get
      {
        return _lote;
      }
      set
      {
        _lote = value.Trim();
      }
    }

    public string Serie
    {
      get
      {
        return _serie;
      }
      set
      {
        _serie = value.Trim();
      }
    }

    public string EstadoProducto
    {
      get
      {
        return _estadoProducto;
      }
      set
      {
        _estadoProducto = value.Trim();
      }
    }

    public double Peso
    {
      get { return _peso; }
      set { _peso = value; }
    }

    public double VolumenUnitario
    {
      set { _volumen = value; }
    }

    public double VolumenTotal
    {
      get
      {
        return _volumen * _cantidad;
      }
    }

    public bool Valido
    {
      get
      {
        return _valido;
      }
    }

    public string Causa
    {
      get { return _causa; }
      set
      {
        _causa = value;
        _valido = (_causa.Length == 0);
      }
    }

    public int idProducto
    {
      get
      {
        return _idProducto;
      }
      set
      {
        _idProducto = value;
      }
    }
    
    //Implementaciones para pedidos standard. En pedidos diferentes al standard deben override estos metodos
    public virtual bool ValidarItem()
    {
      _valido = false;
      _causa = "Funcion no implementada";
      return false;
    }

    public virtual bool Importar(string Emplazamiento , string Almacen , string TipoPedido , string Letra , string CentroEmisor , string NroPedido)
    {
      return false;
    }
  }
}
