using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
  public interface IPedidosImportacion
  {
    int CantidadDeArchivos
    {
      get;
      set;
    }

    string ArchivoCabecera
    {
      get;
      set;
    }

    string ArchivoDetalle
    {
      get;
      set;
    }

    string Estado
    {
      get;
    }

    string ErrorMsg
    {
      get;
    }

    IList<PedidoImportado> CargarArchivos();

    bool ImportarPedido(PedidoImportado p);

  }
}
