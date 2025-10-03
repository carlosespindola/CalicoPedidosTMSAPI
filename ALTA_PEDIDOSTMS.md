# Plan de Trabajo: Refactorización de PedidosTMSController

## Objetivo
Refactorizar el `PedidosTMSController` para:
1. Eliminar DBContexts no utilizados
2. Implementar inyección de dependencias para los contextos `DefaultConnection` y `DBMasterConnection`
3. Crear el método de servicio `AltaPedidosTMS` que llame a stored procedures
4. Generar los archivos SQL de definición de stored procedures

---

## Tareas

### 1. Análisis del Estado Actual
**Estado:** ✅ COMPLETADO

**Hallazgos:**
- El controller actualmente usa `SaadisDBContextFactory` y `ISaadisDBContextFactory`
- Necesita reemplazarse con los contextos Dapper:
  - `CalicoInterfazDapperContext` (DefaultConnection)
  - `CalicoMasterDapperContext` (DBMasterConnection)
- El método `AltaPedidosTMS` actualmente no existe pero hay una referencia en el código comentado antiguo
- Ya existe la clase `ResultadoPedidoTMS` en `/Calico.PedidosTMS.Models/DTO/ResultadoPedidoSaadis.cs`
- Ya existe la clase `PedidoTMSDesdeAPI` con todos los campos necesarios

---

### 2. Modificar PedidosTMSController - Inyección de Dependencias
**Archivos a modificar:**
- `/Calico.PedidosTMS.API/Controllers/PedidosTMSController.cs`

**Cambios:**
1. Eliminar las dependencias:
   - `SaadisDBContextFactory _saadisFactory`
   - `ISaadisDBContextFactory _dapperSaadisDBContextFactory`

2. Agregar las nuevas dependencias por constructor:
   - `CalicoInterfazDapperContext` (para DefaultConnection)
   - `CalicoMasterDapperContext` (para DBMasterConnection)

3. Actualizar el constructor del controller

---

### 3. Implementar el Método de Servicio AltaPedidosTMS
**Ubicación:** `PedidosTMSController.cs`

**Firma del método:**
```csharp
private async Task<List<ResultadoPedidoTMS>> AltaPedidosTMS(
    string cliente,
    PedidoTMSDesdeAPI[] pedidos,
    CalicoInterfazDapperContext defaultContext,
    CalicoMasterDapperContext masterContext)
```

**Lógica del método:**
1. Inicializar lista de resultados: `List<ResultadoPedidoTMS>`
2. Para cada pedido en el array:

   a. **Llamar al SP de destinatario (DBMasterConnection):**
   - SP: `spAM_DestinatarioDesdeAPI`
   - Parámetros:
     - `NumeroClienteDestino`
     - `NombreDestinatario`
     - `DomicilioDestinatario`
     - `NumeroCalleDestinatario`
     - `PisoDeptoDestinatario`
     - `LocalidadDestinatario`
     - `CodigoPostalDestinatario`
     - `TipoIvasDestino`
     - `CuitDestino`

   b. **Llamar al SP de alta de pedido (DefaultConnection):**
   - SP: `spAltaPedidoTMSDesdeAPI`
   - Parámetros: Todos los campos de `PedidoTMSDesdeAPI` + `cliente`
   - Recibir resultado del procesamiento

   c. **Crear objeto ResultadoPedidoTMS:**
   - Mapear el resultado del SP a `ResultadoPedidoTMS`
   - Agregar a la lista de resultados

3. Retornar la lista completa de resultados

---

### 4. Actualizar el Endpoint POST Alta
**Ubicación:** `PedidosTMSController.cs` línea 164

**Cambios:**
1. Eliminar la creación manual de contextos:
   ```csharp
   // ELIMINAR:
   DapperDBContext _dapperSaadisDBContext = _dapperSaadisDBContextFactory.CreateDBContext();
   SaadisDBContext _saadisDBContext = _saadisFactory.Create();
   ```

2. Actualizar la llamada al método:
   ```csharp
   List<ResultadoPedidoTMS> resultadoPedidos = await AltaPedidosTMS(
       _cliente,
       pedidos,
       _calicoInterfazContext,
       _calicoMasterContext);
   ```

---

### 5. Crear Stored Procedure: spAM_DestinatarioDesdeAPI
**Archivo:** `/Calico.PedidosTMS.API.Projects/SQL/spAM_DestinatarioDesdeAPI.sql`

**Propósito:**
Gestionar el alta/modificación de destinatarios en la base de datos Master

**Parámetros de entrada:**
- `@NumeroClienteDestino NVARCHAR(10)`
- `@NombreDestinatario NVARCHAR(30)`
- `@DomicilioDestinatario NVARCHAR(30)`
- `@NumeroCalleDestinatario NVARCHAR(5)`
- `@PisoDeptoDestinatario NVARCHAR(5)`
- `@LocalidadDestinatario NVARCHAR(20)`
- `@CodigoPostalDestinatario NVARCHAR(10)`
- `@TipoIvasDestino NVARCHAR(1)`
- `@CuitDestino NVARCHAR(13)`

**Parámetros de salida:**
- Ninguno (o según lógica de negocio)

**Lógica:**
- Validar existencia del destinatario
- Si existe: actualizar datos
- Si no existe: crear nuevo destinatario
- Realizar validaciones de datos
- Manejo de errores con TRY-CATCH

---

### 6. Crear Stored Procedure: spAltaPedidoTMSDesdeAPI
**Archivo:** `/Calico.PedidosTMS.API.Projects/SQL/spAltaPedidoTMSDesdeAPI.sql`

**Propósito:**
Procesar el alta de un pedido TMS desde la API

**Parámetros de entrada:** (TODOS los campos de PedidoTMSDesdeAPI)
- `@Cliente NVARCHAR(10)`
- `@PuntoDeVentaComprobante NVARCHAR(4)`
- `@NumeroComprobante NVARCHAR(8)`
- `@LetraComprobante NVARCHAR(1)`
- `@FechaComprobante DATETIME`
- `@NumeroRemito NVARCHAR(15)`
- `@Bultos DECIMAL(18,2)`
- `@KilosNetos DECIMAL(18,2)`
- `@KilosAforados DECIMAL(18,2)`
- `@MetrosCubicos DECIMAL(18,2)`
- `@NombreRemitente NVARCHAR(30)`
- `@LocalidadRemitente NVARCHAR(30)`
- `@NombreDestinatario NVARCHAR(30)`
- `@DomicilioDestinatario NVARCHAR(30)`
- `@NumeroCalleDestinatario NVARCHAR(5)`
- `@PisoDeptoDestinatario NVARCHAR(5)`
- `@LocalidadDestinatario NVARCHAR(20)`
- `@CodigoPostalDestinatario NVARCHAR(10)`
- `@ValorDeclarado DECIMAL(18,2)`
- `@ImporteContrareembolso DECIMAL(18,2)`
- `@TipoIvaRemitente NVARCHAR(1)`
- `@CuitRemitente NVARCHAR(13)`
- `@NumeroClienteDestino NVARCHAR(10)`
- `@ObservacionEnvio NVARCHAR(30)`
- `@CantidadPallets DECIMAL(18,2)`
- `@CantidadUnidades DECIMAL(18,2)`
- `@FechaPosibleEntrega DATETIME`
- `@ObservacionEnvio_2 NVARCHAR(33)`
- `@TipoIvasDestino NVARCHAR(1)`
- `@CuitDestino NVARCHAR(13)`
- `@ObservacionAdicionalEnvio NVARCHAR(60)`
- `@SucursalOrigen NVARCHAR(4)`
- `@Email NVARCHAR(100)`
- `@Telefono NVARCHAR(70)`
- `@CampoAdicional1 NVARCHAR(80)`
- `@CampoAdicional2 NVARCHAR(80)`
- `@CampoAdicional3 NVARCHAR(80)`
- `@CampoAdicional4 NVARCHAR(80)`
- `@CampoAdicional5 NVARCHAR(80)`
- `@Senasa NVARCHAR(1)`
- `@TipoCarga NVARCHAR(3)`
- `@NombreDestinatarioAlternativo NVARCHAR(40)`
- `@TipoIvasDestinoAlternativo NVARCHAR(4)`
- `@CuitDestinoAlternativo NVARCHAR(13)`
- `@TelefonoDestinoAlternativo NVARCHAR(70)`
- `@CodigoVerificadorEntrega NVARCHAR(500)`
- `@DatosEntrega NVARCHAR(2000)`

**Parámetros de salida:**
- `@Resultado NVARCHAR(10)` OUTPUT -- 'OK' o 'ERROR'
- `@Mensaje NVARCHAR(500)` OUTPUT -- Mensaje descriptivo del resultado

**Lógica:**
- Validar existencia del comprobante (evitar duplicados)
- Validar datos obligatorios
- Insertar el pedido en las tablas correspondientes
- Retornar resultado del procesamiento
- Manejo de errores con TRY-CATCH

---

### 7. Actualizar appsettings.json (Si es necesario)
**Archivo:** `/Calico.PedidosTMS.API/appsettings.json`

**Validar que existan:**
- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:DBMasterConnection`

---

### 8. Actualizar Startup.cs (Si es necesario)
**Archivo:** `/Calico.PedidosTMS.API/Startup.cs`

**Validar que estén registrados:**
```csharp
services.AddScoped(sp => new Calico.PedidosTMS.DAL.DapperContexts.CalicoInterfazDapperContext(
    Configuration.GetConnectionString("DefaultConnection")));

services.AddScoped(sp => new Calico.PedidosTMS.DAL.DapperContexts.CalicoMasterDapperContext(
    Configuration.GetConnectionString("DBMasterConnection")));
```
(Ya están registrados según líneas 173-177)

---

### 9. Testing y Validación
**Tareas:**
1. Compilar el proyecto
2. Verificar que no hay errores de dependencias
3. Probar el endpoint con Swagger
4. Validar que los stored procedures funcionan correctamente
5. Verificar los resultados en la base de datos

---

## Archivos a Crear

1. **SQL Scripts:**
   - `/SQL/spAM_DestinatarioDesdeAPI.sql`
   - `/SQL/spAltaPedidoTMSDesdeAPI.sql`

---

## Archivos a Modificar

1. **Controller:**
   - `/Calico.PedidosTMS.API/Controllers/PedidosTMSController.cs`
     - Líneas 32-48: Constructor y dependencias
     - Líneas 164-180: Método POST Alta
     - Agregar nuevo método privado: `AltaPedidosTMS`

---

## Notas Importantes

- ⚠️ **NO MODIFICAR** `AuthController.cs`
- ⚠️ El método `AltaPedidosTMS` debe ser **privado** y ser llamado desde el endpoint POST
- ⚠️ Los stored procedures deben tener manejo de errores robusto con TRY-CATCH
- ⚠️ Usar `Dapper` para la ejecución de los stored procedures
- ⚠️ El resultado debe ser una lista de `ResultadoPedidoTMS`, no `ResultadoPedidoSaadis`
- ⚠️ Mantener la estructura de respuesta existente para compatibilidad con clientes

---

## Orden de Ejecución

1. ✅ Crear archivo `ALTA_PEDIDOSTMS.md` (este documento)
2. ⏳ Crear archivo SQL `spAM_DestinatarioDesdeAPI.sql`
3. ⏳ Crear archivo SQL `spAltaPedidoTMSDesdeAPI.sql`
4. ⏳ Modificar `PedidosTMSController.cs` - Constructor y dependencias
5. ⏳ Implementar método privado `AltaPedidosTMS` en `PedidosTMSController.cs`
6. ⏳ Actualizar endpoint POST Alta en `PedidosTMSController.cs`
7. ⏳ Compilar y probar

---

## Checklist Final

- [ ] Archivo `spAM_DestinatarioDesdeAPI.sql` creado
- [ ] Archivo `spAltaPedidoTMSDesdeAPI.sql` creado
- [ ] Constructor de `PedidosTMSController` actualizado
- [ ] Método `AltaPedidosTMS` implementado
- [ ] Endpoint POST Alta actualizado
- [ ] Proyecto compila sin errores
- [ ] Stored procedures ejecutados en la base de datos
- [ ] Testing con Swagger exitoso
- [ ] Validación de datos en la base de datos

---

**Fecha de creación:** 2025-09-30
**Estado:** Plan completado - Listo para ejecución
