using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Entities
{
  [Serializable]
  public class PedidoImportado
  {
    IList<PedidoItemImportado> _items;
    private string _emplazamiento;
    private string _almacen;
    private string _tipoPedido;
    private string _letra;
    private string _centroEmisor;
    private string _nroPedido;
    private string _fechaPedido;
    private string _cliente;
    private string _subCliente;
    private string _razonSocial;
    private string _domicilio;
    private string _localidad;
    private string _provincia;
    private string _CP;
    private string _particiona;
    private string _fechaEntrega;
    private string _refA;
    private string _refB;
    private double _valorDeclarado;
    private double _valorContrareembolso;
    private double _unidades;
    private string _centroDeCosto;
    private string _areaMuelle;
    private string _operador;
    private string _fechaTransac;
    private string _ordenPrioridad;
    private string _otros;
    private int _cantidadItems;
    private double _pesoTotal;
    private double _volumenTotal;
    private bool _importar;
    private string _causa;
    private bool _existeEnMSOPEDC;
    private bool _importado;

    private int _IdPedido;
    private int _IdEmplazamaiento;
    private int _IdAlmacen;
    private int _IdTipoPedido;
    private int _IdCliente;
    private int _IdSubcliente; // si no existe lo agrega
    private int _IdLocalidad;
    private int _IdProvincia;
    private int _IdPais;
    private int _IdAreaMuelle;
    private int _IdCentroDeCosto;

    public PedidoImportado()
    {
      _items = (IList<PedidoItemImportado>)new List<PedidoItemImportado>();
      _cantidadItems = 0;
      _unidades = 0;
    }

    public void AgregarItem(PedidoItemImportado i)
    {
      _items.Add(i);
      _cantidadItems += 1;
      _unidades += i.Cantidad;
    }

    public IList<PedidoItemImportado> items
    {
      get { return _items; }
    }

    public string Emplazamiento
    {
      get { return _emplazamiento; }
      set { _emplazamiento = value; }
    }

    public string Almacen
    {
      get { return _almacen; }
      set { _almacen = value; }
    }

    public string TipoPedido
    {
      get { return _tipoPedido; }
      set { _tipoPedido = value; }
    }

    public string Letra
    {
      get { return _letra; }
      set { _letra = value; }
    }

    public string CentroEmisor
    {
      get { return _centroEmisor; }
      set { _centroEmisor = value.Trim().PadLeft( 4, '0'); }
    }

    public string NroPedido
    {
      get { return _nroPedido; }
      set { _nroPedido = value.Trim().PadLeft( 8, '0'); }
    }

    public string NroPedidoCompleto
    {
      get { return _letra + "-" + _centroEmisor + "-" + _nroPedido; }
    }

    public string FechaPedido
    {
      get { return _fechaPedido; }
      set
      {
        try
        {
          DateTime d = new DateTime(Convert.ToInt32(value.Substring(0, 4)), Convert.ToInt32(value.Substring(4, 2)), Convert.ToInt32(value.Substring(6, 2)));

          _fechaPedido = value;
        }
        catch
        {
          _fechaPedido = string.Empty;
        }
      }
    }

    public string Cliente
    {
      get { return _cliente; }
      set { _cliente = value; }
    }

    public string Subcliente
    {
      get { return _subCliente; }
      set { _subCliente = value; }
    }

    public string RazonSocial
    {
      get { return _razonSocial; }
      set { _razonSocial = value.Trim(); ; }
    }

    public string Domicilio
    {
      get { return _domicilio; }
      set { _domicilio = value.Trim(); }
    }

    public string Localidad
    {
      get { return _localidad; }
      set { _localidad = value.Trim(); }
    }

    public string Provincia 
    {
      get { return _provincia; }
      set { _provincia = value.Trim(); }
    }

    public string CodigoPostal
    {
      get { return _CP; }
      set { _CP = value.Trim(); }
    }

    public string PedidoParticionado
    {
      get { return _particiona; }
      set { _particiona = value; }
    }

    public string FechaEntrega
    {
      get { return _fechaEntrega; }
      set {
        try
        {
          DateTime d = new DateTime(Convert.ToInt32(value.Substring(0, 4)), Convert.ToInt32(value.Substring(4, 2)), Convert.ToInt32(value.Substring(6, 2)));

          _fechaEntrega = value;
        }
        catch
        {
          _fechaEntrega = string.Empty;
        }
      }
    }

    public string ReferenciaA
    {
      get { return _refA; }
      set { _refA = value; }
    }

    public string ReferenciaB
    {
      get { return _refB; }
      set { _refB = value; }
    }

    public double Unidades
    {
      get { return _unidades; }
    }

    public int CantidadLineas
    {
      get { return _cantidadItems; }
    }

    public double ValorDeclarado
    {
      get { return _valorDeclarado; }
      set { _valorDeclarado = value; }
    }

    public double ValorContraReembolso
    {
      get { return _valorContrareembolso; }
      set { _valorContrareembolso = value; }
    }

    public string AreaMuelle
    {
      get { return _areaMuelle; }
      set { _areaMuelle = value; }
    }

    public string CentroDeCosto
    {
      get { return _centroDeCosto; }
      set { _centroDeCosto = value.Trim(); }
    }

    public double PesoTotal
    {
      get { return _pesoTotal; }
      set { _pesoTotal = value; }
    }

    public double VolumenTotal
    {
      get { return _volumenTotal; }
      set { _volumenTotal = value; }
    }

    public int IdPedido
    {
      get { return _IdPedido; }
      set { _IdPedido = value; }
    }

    public int IdEmplazamaiento
    {
      get { return _IdEmplazamaiento;}
      set { _IdEmplazamaiento = value; }
    }
    
    public int IdAlmacen
    {
      get { return _IdAlmacen;}
      set { _IdAlmacen = value; }
    }
    
    public int IdTipoPedido
    {
      get { return _IdTipoPedido;}
      set { _IdTipoPedido = value; }
    }
    
    public int IdCliente
    {
      get { return _IdCliente;}
      set { _IdCliente = value; }
    }
    
    public int IdSubcliente
    {
      get { return _IdSubcliente;}
      set { _IdSubcliente = value; }
    }
    
    public int IdLocalidad
    {
      get { return _IdLocalidad;}
      set { _IdLocalidad = value; }
    }
    
    public int IdProvincia
    {
      get { return _IdProvincia;}
      set { _IdProvincia = value; }
    }

    public int IdPais
    {
      get { return _IdPais; }
      set { _IdPais = value; }
    }

    public int IdAreaMuelle
    {
      get { return _IdAreaMuelle; }
      set { _IdAreaMuelle = value; }
    }

    public int IdCentroDeCosto
    {
      get { return _IdCentroDeCosto;}
      set { _IdCentroDeCosto = value; }
    }

    //
    public bool Importable
    {
      get { return _importar; }
    }

    public string Causa
    {
      get { return _causa; }
      set { 
        _causa = value;
        _importar = (_causa.Length == 0) ;
      }
    }

    public bool Importado
    {
      get { return _importado; }
    }


    //Implementaciones para pedidos standard. En pedidos diferentes al standard deben override estos metodos
    public virtual void ValidarPedido()
    {
      _causa = "Funcion no implementada";
      _importar = false;
    }

    public virtual bool Importar()
    {
      _causa = "Funcion no implementada";
      _importado = false;
      return false;
    }


  }
}
