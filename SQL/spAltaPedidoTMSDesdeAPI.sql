/***************************************************************************
-- Stored Procedure: spAltaPedidoTMSDesdeAPI
-- Descripción: Procesa el alta de un pedido TMS desde la API
-- Base de datos: DefaultConnection (CalicoInterfazDapperContext)
-- Fecha creación: 2025-09-30
***************************************************************************
* 03/10/25 CEspindola Diego Accion definió que servicio y sector van fijos (al menos x ahora). Valores: servicio = 'A', sector = 'API'
***************************************************************************/
ALTER PROCEDURE [dbo].[spAltaPedidoTMSDesdeAPI]
    -- Parámetros del cliente y comprobante
    @Cliente NVARCHAR(10),
    @PuntoDeVentaComprobante NVARCHAR(4),
    @NumeroComprobante NVARCHAR(8),
    @LetraComprobante NVARCHAR(1),
    @FechaComprobante DATETIME,
    @NumeroRemito NVARCHAR(15),

    -- Parámetros de cantidades y medidas
    @Bultos DECIMAL(18,2),
    @KilosNetos DECIMAL(18,2),
    @KilosAforados DECIMAL(18,2),
    @MetrosCubicos DECIMAL(18,2),
    @CantidadPallets DECIMAL(18,2),
    @CantidadUnidades DECIMAL(18,2),

    -- Parámetros del remitente
    @NombreRemitente NVARCHAR(30),
    @LocalidadRemitente NVARCHAR(30),
    @TipoIvaRemitente NVARCHAR(1),
    @CuitRemitente NVARCHAR(13),

    -- Parámetros del destinatario
    @NombreDestinatario NVARCHAR(30),
    @DomicilioDestinatario NVARCHAR(30),
    @NumeroCalleDestinatario NVARCHAR(5),
    @PisoDeptoDestinatario NVARCHAR(5),
    @LocalidadDestinatario NVARCHAR(20),
    @CodigoPostalDestinatario NVARCHAR(10),
    @NumeroClienteDestino NVARCHAR(10),
    @TipoIvasDestino NVARCHAR(1),
    @CuitDestino NVARCHAR(13),

    -- Parámetros financieros
    @ValorDeclarado DECIMAL(18,2),
    @ImporteContrareembolso DECIMAL(18,2),

    -- Parámetros de observaciones
    @ObservacionEnvio NVARCHAR(30),
    @ObservacionEnvio_2 NVARCHAR(33),
    @ObservacionAdicionalEnvio NVARCHAR(60),

    -- Parámetros adicionales
    @FechaPosibleEntrega DATETIME,
    @SucursalOrigen NVARCHAR(4),
    @Email NVARCHAR(100),
    @Telefono NVARCHAR(70),

    -- Campos adicionales personalizables
    @CampoAdicional1 NVARCHAR(80),
    @CampoAdicional2 NVARCHAR(80),
    @CampoAdicional3 NVARCHAR(80),
    @CampoAdicional4 NVARCHAR(80),
    @CampoAdicional5 NVARCHAR(80),

    -- Parámetros especiales
    @Senasa NVARCHAR(1),
    @TipoCarga NVARCHAR(3),

    -- Destinatario alternativo
    @NombreDestinatarioAlternativo NVARCHAR(40),
    @TipoIvasDestinoAlternativo NVARCHAR(4),
    @CuitDestinoAlternativo NVARCHAR(13),
    @TelefonoDestinoAlternativo NVARCHAR(70),

    -- Parámetros de entrega
    @CodigoVerificadorEntrega NVARCHAR(500),
    @DatosEntrega NVARCHAR(2000),

    -- Parámetros de salida
    @Resultado NVARCHAR(10) OUTPUT,
    @Mensaje NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    DECLARE @ComprobanteExiste INT = 0;

    -- Inicializar parámetros de salida
    SET @Resultado = 'ERROR';
    SET @Mensaje = '';

    BEGIN TRY
        -- ========================================
        -- VALIDACIONES DE DATOS OBLIGATORIOS
        -- ========================================

        IF @Cliente IS NULL OR LTRIM(RTRIM(@Cliente)) = ''
        BEGIN
            SET @Mensaje = 'El código de cliente es obligatorio';
            RETURN;
        END

        IF @PuntoDeVentaComprobante IS NULL OR LTRIM(RTRIM(@PuntoDeVentaComprobante)) = ''
        BEGIN
            SET @Mensaje = 'El punto de venta del comprobante es obligatorio';
            RETURN;
        END

        IF @NumeroComprobante IS NULL OR LTRIM(RTRIM(@NumeroComprobante)) = ''
        BEGIN
            SET @Mensaje = 'El número de comprobante es obligatorio';
            RETURN;
        END

        IF @LetraComprobante IS NULL OR LTRIM(RTRIM(@LetraComprobante)) = ''
        BEGIN
            SET @Mensaje = 'La letra del comprobante es obligatoria';
            RETURN;
        END

        IF @FechaComprobante IS NULL
        BEGIN
            SET @Mensaje = 'La fecha del comprobante es obligatoria';
            RETURN;
        END

		CREATE TABLE #CambioEstado
		(
			[cliente] CHAR(20) NOT NULL,
			[letra] CHAR(1) NOT NULL,
			[centroEmisor] CHAR(4) NOT NULL,
			[numero] NUMERIC(10, 0) NOT NULL,
			[accion] INT NOT NULL
		)

		CREATE TABLE #Agrupamiento
		(
			[cliente] CHAR(20) NOT NULL,
			[letra] CHAR(1) NOT NULL,
			[centroEmisor] CHAR(4) NOT NULL,
			[numero] NUMERIC(10, 0) NOT NULL,
			[concepto] CHAR(4) NOT NULL,
			[valor] CHAR(50) NOT NULL
		)

		CREATE TABLE #AsignacionMuelle
		(
			[cliente] CHAR(20) NOT NULL,
			[letra] CHAR(1) NOT NULL,
			[centroEmisor] CHAR(4) NOT NULL,
			[numero] NUMERIC(10, 0) NOT NULL
		)

        -- ========================================
        -- NORMALIZAR DATOS
        -- ========================================

        -- Normalizar punto de venta y número de comprobante (padding con ceros)
        SET @PuntoDeVentaComprobante = RIGHT('0000' + LTRIM(RTRIM(@PuntoDeVentaComprobante)), 4);
        SET @NumeroComprobante = RIGHT('00000000' + LTRIM(RTRIM(@NumeroComprobante)), 8);
--        SET @Cliente = RIGHT('0000000000' + LTRIM(RTRIM(@Cliente)), 10);

        IF @NumeroClienteDestino IS NOT NULL
        BEGIN
            SET @NumeroClienteDestino = RIGHT('0000000000' + LTRIM(RTRIM(@NumeroClienteDestino)), 10);
        END

		DECLARE @importar BIT, @confirmar BIT, @recibir BIT, @entregar BIT, @asignarAgrupamiento BIT,
			@asignarMuelle BIT, @informarDestinatario BIT

		SET @importar = 0
		SET @confirmar = 0
		SET @recibir = 0
		SET @entregar = 0
		SET @asignarAgrupamiento = 0
		SET @asignarMuelle = 0
		SET @informarDestinatario = 0

		SELECT 
			@importar = parametrizacion.pcom_importar,
			@confirmar = parametrizacion.pcom_confirmar,
			@recibir = parametrizacion.pcom_recibir,
			@entregar = parametrizacion.pcom_entregar,
			@asignarAgrupamiento = parametrizacion.pcom_asignarAgrupamiento,
			@asignarMuelle = parametrizacion.pcom_asignarMuelle,
			@informarDestinatario = parametrizacion.pcom_informarDestinatario
		FROM tblParametrizacionComprobante parametrizacion (NOLOCK)
		WHERE parametrizacion.pcom_estado = 'LIS'
			AND parametrizacion.pcom_cliente = RIGHT('0000000000' + LTRIM(RTRIM(@Cliente)), 10)


		------INICIO CARGA CAMBIOS ESTADO-----
		DECLARE @accion INT

		-- Asigna accion de confirmación
		SET @accion = 1

		IF (@confirmar = 1)
		BEGIN
			IF NOT EXISTS
				(
					SELECT TOP 1 1
					FROM tblCambioEstadoComprobante existente
					WHERE existente.ceco_cliente = @cliente COLLATE DATABASE_DEFAULT
						AND existente.ceco_letra = @LetraComprobante COLLATE DATABASE_DEFAULT
						AND existente.ceco_centroEmisor = @PuntoDeVentaComprobante COLLATE DATABASE_DEFAULT
						AND existente.ceco_numero = @NumeroComprobante COLLATE DATABASE_DEFAULT
						AND existente.ceco_accion = @accion
				)
			BEGIN
				INSERT INTO #CambioEstado
				(
					cliente,
					letra,
					centroEmisor,
					numero,
					accion
				)
				VALUES
				(@cliente, @LetraComprobante, @PuntoDeVentaComprobante, @NumeroComprobante, @accion)
			END
		END

		-- Asigna accion de recepcion
		SET @accion = 2

		IF (@recibir = 1)
		BEGIN
			IF NOT EXISTS
				(
					SELECT TOP 1 1
					FROM tblCambioEstadoComprobante existente
					WHERE existente.ceco_cliente = @cliente COLLATE DATABASE_DEFAULT
						AND existente.ceco_letra = @LetraComprobante COLLATE DATABASE_DEFAULT
						AND existente.ceco_centroEmisor = @PuntoDeVentaComprobante COLLATE DATABASE_DEFAULT
						AND existente.ceco_numero = @NumeroComprobante COLLATE DATABASE_DEFAULT
						AND existente.ceco_accion = @accion
				)
			BEGIN
				INSERT INTO #CambioEstado
				(
					cliente,
					letra,
					centroEmisor,
					numero,
					accion
				)
				VALUES
				(@cliente, @LetraComprobante, @PuntoDeVentaComprobante, @NumeroComprobante, @accion)
			END
		END

		-- Asigna accion de entrega
		SET @accion = 3

		IF (@entregar = 1)
		BEGIN
			INSERT INTO #CambioEstado
			(
				cliente,
				letra,
				centroEmisor,
				numero,
				accion
			)
			VALUES
			(@cliente, @LetraComprobante, @PuntoDeVentaComprobante, @NumeroComprobante, @accion)
		END

		------FIN CARGA CAMBIOS ESTADO-----

		------INICIO CARGA AGRUPAMIENTOS-----
		/*************** Mirgor no define Agrupamientos!! ********************

		IF (@asignarAgrupamiento = 1)
		BEGIN
		
			DECLARE @concepto CHAR(4), @campo SYSNAME, @desde INT, @hasta INT

			DECLARE Parametrizaciones INSENSITIVE CURSOR FOR
				SELECT
					parametrizacion.pagr_concepto AS concepto,
					parametrizacion.pagr_campo AS campo,
					parametrizacion.pagr_desde AS desde,
					parametrizacion.pagr_hasta AS hasta
				FROM tblParametrizacionAgrupamiento parametrizacion
				WHERE parametrizacion.pagr_cliente = @Cliente

			OPEN Parametrizaciones
	
			FETCH NEXT FROM Parametrizaciones INTO @concepto, @campo, @desde, @hasta
		
			WHILE (@@FETCH_STATUS = 0)
			BEGIN
				IF (@campo = 'TIP_PEDIDO')
				BEGIN
					INSERT INTO #Agrupamiento
					(
						cliente,
						letra,
						centroEmisor,
						numero,
						concepto,
						valor
					)
					VALUES
					(@cliente, @LetraComprobante, @PuntoDeVentaComprobante, @NumeroComprobante, @concepto,
					SUBSTRING(temporal.TIP_PEDIDO, @desde, @hasta))
				END
				ELSE IF (@campo = 'NRO_RECOGI')
				BEGIN
					INSERT INTO #Agrupamiento
					(
						cliente,
						letra,
						centroEmisor,
						numero,
						concepto,
						valor
					)
					SELECT
						@cliente AS cliente,
						temporal.CAT_PEDIDO AS letra,
						temporal.SUC_PEDIDO AS centroEmisor,
						temporal.NRO_PEDIDO AS numero,
						@concepto AS concepto,
						SUBSTRING(CAST(temporal.NRO_RECOGI AS NVARCHAR(MAX)), @desde, @hasta) AS valor
					FROM #Pedido temporal
					WHERE temporal.ID = @id
						AND temporal.NRO_RECOGI <> 0
						AND NOT EXISTS
						(
							SELECT TOP 1 1
							FROM #Agrupamiento existente
							WHERE existente.cliente = @cliente
								AND existente.letra = temporal.CAT_PEDIDO
								AND existente.centroEmisor = temporal.SUC_PEDIDO
								AND existente.numero = temporal.NRO_PEDIDO
								AND existente.concepto = @concepto
						)

					UPDATE #Agrupamiento
					SET valor = SUBSTRING(CAST(temporal.NRO_RECOGI AS NVARCHAR(MAX)), @desde, @hasta)
					FROM #Pedido temporal
					WHERE temporal.ID = @id
						AND temporal.NRO_RECOGI <> 0
						AND #Agrupamiento.cliente = @cliente
						AND #Agrupamiento.letra = temporal.CAT_PEDIDO
						AND #Agrupamiento.centroEmisor = temporal.SUC_PEDIDO
						AND #Agrupamiento.numero = temporal.NRO_PEDIDO
						AND #Agrupamiento.concepto = @concepto
				END
				ELSE IF (@campo = 'REF_A')
				BEGIN
					INSERT INTO #Agrupamiento
					(
						cliente,
						letra,
						centroEmisor,
						numero,
						concepto,
						valor
					)
					SELECT
						@cliente AS cliente,
						temporal.CAT_PEDIDO AS letra,
						temporal.SUC_PEDIDO AS centroEmisor,
						temporal.NRO_PEDIDO AS numero,
						@concepto AS concepto,
						SUBSTRING(temporal.REF_A, @desde, @hasta) AS valor
					FROM #Pedido temporal
					WHERE temporal.ID = @id
						AND SUBSTRING(temporal.REF_A, @desde, @hasta) <> ''
						AND NOT EXISTS
						(
							SELECT TOP 1 1
							FROM #Agrupamiento existente
							WHERE existente.cliente = @cliente
								AND existente.letra = temporal.CAT_PEDIDO
								AND existente.centroEmisor = temporal.SUC_PEDIDO
								AND existente.numero = temporal.NRO_PEDIDO
								AND existente.concepto = @concepto
						)

					UPDATE #Agrupamiento
					SET valor = SUBSTRING(temporal.REF_A, @desde, @hasta)
					FROM #Pedido temporal
					WHERE temporal.ID = @id
						AND SUBSTRING(temporal.REF_A, @desde, @hasta) <> ''
						AND #Agrupamiento.cliente = @cliente
						AND #Agrupamiento.letra = temporal.CAT_PEDIDO
						AND #Agrupamiento.centroEmisor = temporal.SUC_PEDIDO
						AND #Agrupamiento.numero = temporal.NRO_PEDIDO
						AND #Agrupamiento.concepto = @concepto
				END
				ELSE IF (@campo = 'REF_B')
				BEGIN
					INSERT INTO #Agrupamiento
					(
						cliente,
						letra,
						centroEmisor,
						numero,
						concepto,
						valor
					)
					SELECT
						@cliente AS cliente,
						temporal.CAT_PEDIDO AS letra,
						temporal.SUC_PEDIDO AS centroEmisor,
						temporal.NRO_PEDIDO AS numero,
						@concepto AS concepto,
						SUBSTRING(temporal.REF_B, @desde, @hasta) AS valor
					FROM #Pedido temporal
					WHERE temporal.ID = @id
						AND SUBSTRING(temporal.REF_B, @desde, @hasta) <> ''
						AND NOT EXISTS
						(
							SELECT TOP 1 1
							FROM #Agrupamiento existente
							WHERE existente.cliente = @cliente
								AND existente.letra = temporal.CAT_PEDIDO
								AND existente.centroEmisor = temporal.SUC_PEDIDO
								AND existente.numero = temporal.NRO_PEDIDO
								AND existente.concepto = @concepto
						)

					UPDATE #Agrupamiento
					SET valor = SUBSTRING(temporal.REF_B, @desde, @hasta)
					FROM #Pedido temporal
					WHERE temporal.ID = @id
						AND SUBSTRING(temporal.REF_B, @desde, @hasta) <> ''
						AND #Agrupamiento.cliente = @cliente
						AND #Agrupamiento.letra = temporal.CAT_PEDIDO
						AND #Agrupamiento.centroEmisor = temporal.SUC_PEDIDO
						AND #Agrupamiento.numero = temporal.NRO_PEDIDO
						AND #Agrupamiento.concepto = @concepto
				END

				FETCH NEXT FROM Parametrizaciones INTO @concepto, @campo, @desde, @hasta
			END
			CLOSE Parametrizaciones
			DEALLOCATE Parametrizaciones
		END
		------FIN CARGA AGRUPAMIENTOS-----
		**************************************************************************/

		------INICIO CARGA ASIGNACION MUELLES-----
		IF (@asignarMuelle = 1)
		BEGIN
			INSERT INTO #AsignacionMuelle
			(
				cliente,
				letra,
				centroEmisor,
				numero
			)
			VALUES
			(@cliente, @LetraComprobante, @PuntoDeVentaComprobante, @NumeroComprobante)
		END
		------FIN CARGA ASIGNACION MUELLES-----

		-- resolvemos sucursal segun codigo postal destino
		DECLARE @sucursal VARCHAR(6)

		SELECT @sucursal = loca_sucu_codigo
		FROM Calico_DB_Master.dbo.tblLocalidad
		WHERE loca_codigoPostal=@CodigoPostalDestinatario

		DECLARE @cantidadComprobantes INT, @cantidadCambiosEstado INT, @cantidadAgrupamientos INT,
			@cantidadAsignacionesMuelle INT
	
		SET @cantidadComprobantes = 1
		SET @cantidadCambiosEstado = 0
		SET @cantidadAgrupamientos = 0
		SET @cantidadAsignacionesMuelle = 0
	
		SELECT @cantidadCambiosEstado = COUNT(1)
		FROM #CambioEstado temporal
		WHERE NOT EXISTS
			(
				SELECT TOP 1 1
				FROM tblCambioEstadoComprobante existente
				WHERE existente.ceco_cliente = temporal.cliente COLLATE DATABASE_DEFAULT
					AND existente.ceco_letra = temporal.letra COLLATE DATABASE_DEFAULT
					AND existente.ceco_centroEmisor = temporal.centroEmisor COLLATE DATABASE_DEFAULT
					AND existente.ceco_numero = temporal.numero
					AND existente.ceco_accion = temporal.accion
			)

		SELECT @cantidadAgrupamientos = COUNT(1)
		FROM #Agrupamiento temporal
	
		SELECT @cantidadAsignacionesMuelle = COUNT(1)
		FROM #AsignacionMuelle temporal
		WHERE NOT EXISTS
			(
				SELECT TOP 1 1
				FROM tblAsignacionMuelle existente
				WHERE existente.amue_cliente = temporal.cliente COLLATE DATABASE_DEFAULT
					AND existente.amue_letra = temporal.letra COLLATE DATABASE_DEFAULT
					AND existente.amue_centroEmisor = temporal.centroEmisor COLLATE DATABASE_DEFAULT
					AND existente.amue_numero = temporal.numero
			)	

		DECLARE @idProceso INT, @tipoProceso INT

		SET @idProceso = 0
		SET @tipoProceso = 23

		EXEC INTERFAZ_CrearProcesos @tipoProceso, @cantidadComprobantes, @idProceso OUTPUT

		INSERT INTO dbo.tblComprobante
		(
			comp_proc_id,
			comp_cliente,
			comp_sucursal,
			comp_sector,
			comp_servicio,
			comp_fechaEmision,
			comp_fechaEstimada,
			comp_horaEstimadaDesde,
			comp_horaEstimadaHasta,
			comp_fechaTurno,
			comp_horaTurnoDesde,
			comp_horaTurnoHasta,
			comp_letra,
			comp_centroEmisor,
			comp_numero,
			comp_tipo,
			comp_remitente,
			comp_razonSocialOrigen,
			comp_codigoPostalOrigen,
			comp_nombreCalleOrigen,
			comp_numeroCalleOrigen,
			comp_pisoOrigen,
			comp_departamentoOrigen,
			comp_domicilioOrigen,
			comp_telefonoOrigen,
			comp_correoElectronicoOrigen,
			comp_observacionesOrigen,
			comp_destinatario,
			comp_razonSocialDestino,
			comp_codigoPostalDestino,
			comp_nombreCalleDestino,
			comp_numeroCalleDestino,
			comp_pisoDestino,
			comp_departamentoDestino,
			comp_domicilioDestino,
			comp_telefonoDestino,
			comp_correoElectronicoDestino,
			comp_observacionesDestino,
			comp_paletas,
			comp_bultos,
			comp_kilogramos,
			comp_metrosCubicos,
			comp_unidades,
			comp_valorDeclarado,
			comp_importe,
			comp_observacion
		)
		VALUES
			(@idProceso, -- + ROW_NUMBER() OVER(ORDER BY @Cliente ASC)) - 1 AS comp_proc_id,
			@Cliente,-- AS comp_clie_codigo,
			@sucursal, -- AS comp_sucu_codigo, 
			'A', -- revisar esto temporal.sector AS comp_sect_codigo,
			'API', -- revisar esto temporal.servicio AS comp_serv_codigo,
			@FechaComprobante,-- AS comp_fechaEmision,
			@FechaPosibleEntrega, -- AS comp_fechaEstimada,
			'19730101',-- AS comp_horaEstimadaDesde,
			'19730101',-- AS comp_horaEstimadaHasta,
			'19730101',-- AS comp_fechaTurno,
			'19730101',-- AS comp_horaTurnoDesde,
			'19730101',-- AS comp_horaTurnoHasta,
			@LetraComprobante,-- AS comp_letra,
			@PuntoDeVentaComprobante,-- AS comp_centroEmisor,
			@NumeroComprobante,-- AS comp_numero,
			5,-- AS comp_tipo,
			'',-- AS comp_remi_codigo,
			@NombreRemitente,-- AS comp_razonSocialOrigen,
			'',-- AS comp_loca_codigoPostalOrigen,
			'',-- AS comp_nombreCalleOrigen,
			'',-- AS comp_numeroCalleOrigen,
			'',-- AS comp_pisoOrigen,
			'',-- AS comp_departamentoOrigen,
			'',-- AS comp_domicilioOrigen,
			'',-- AS comp_telefonoOrigen,
			'',-- AS comp_correoElectronicoOrigen,
			'',-- AS comp_observacionesOrigen,
			@NumeroClienteDestino,-- AS comp_dest_codigo,
			@NombreDestinatario,-- AS comp_razonSocialDestino,
			@CodigoPostalDestinatario,-- AS comp_loca_codigoPostalDestino,
			@DomicilioDestinatario,-- AS comp_nombreCalleDestino,
			@NumeroCalleDestinatario,-- AS comp_numeroCalleDestino,
			'',-- AS comp_pisoDestino,
			'',-- AS comp_departamentoDestino,
			LTRIM(RTRIM(ISNULL(@DomicilioDestinatario, ''))) + ' ' + LTRIM(RTRIM(ISNULL(@NumeroCalleDestinatario, ''))) + ' ' + LTRIM(RTRIM(ISNULL(',Depto. ' + @PisoDeptoDestinatario, ''))) + ', ' + LTRIM(RTRIM(ISNULL(@LocalidadDestinatario, ''))), -- AS comp_domicilioDestino,
			'', -- AS comp_telefonoDestino,
			'', -- AS comp_correoElectronicoDestino,
			'', -- AS comp_observacionesDestino,
			0, -- AS comp_paletas,
			@Bultos, -- AS comp_bultos,
			@KilosAforados, -- AS comp_kilogramos,
			@MetrosCubicos, -- AS comp_metrosCubicos,
			@CantidadUnidades, -- AS comp_unidades,
			@ValorDeclarado, -- AS comp_valorDeclarado,
			@ImporteContrareembolso, -- AS comp_importe,
			@ObservacionEnvio -- AS comp_observacion
			)
	
		UPDATE tblProceso
		SET proc_identificador =
			LTRIM(RTRIM(proceso.comp_cliente)) + '-' +
			proceso.comp_letra + '-' +
			proceso.comp_centroEmisor + '-' +
			RIGHT(REPLICATE('0', 10) + CAST(proceso.comp_numero AS VARCHAR(MAX)), 10)
		FROM tblComprobante proceso
		WHERE proceso.comp_proc_id = tblProceso.proc_id

		IF (@cantidadCambiosEstado > 0)
		BEGIN
			SET @idProceso = 0
			SET @tipoProceso = 24

			EXEC INTERFAZ_CrearProcesos @tipoProceso, @cantidadCambiosEstado, @idProceso OUTPUT

			INSERT INTO tblCambioEstadoComprobante
			(
				ceco_proc_id,
				ceco_cliente,
				ceco_letra,
				ceco_centroEmisor,
				ceco_numero,
				ceco_accion
			)
			SELECT
				(@idProceso + ROW_NUMBER() OVER(ORDER BY temporal.cliente ASC)) - 1 AS ceco_proc_id,
				temporal.cliente AS ceco_cliente,
				temporal.letra AS ceco_letra,
				temporal.centroEmisor AS ceco_centroEmisor,
				temporal.numero AS ceco_numero,
				temporal.accion AS ceco_accion
			FROM #CambioEstado temporal
			WHERE NOT EXISTS
				(
					SELECT TOP 1 1
					FROM tblCambioEstadoComprobante existente
					WHERE existente.ceco_cliente = temporal.cliente COLLATE DATABASE_DEFAULT
						AND existente.ceco_letra = temporal.letra COLLATE DATABASE_DEFAULT
						AND existente.ceco_centroEmisor = temporal.centroEmisor COLLATE DATABASE_DEFAULT
						AND existente.ceco_numero = temporal.numero
						AND existente.ceco_accion = temporal.accion
				)
		END

		UPDATE tblProceso
		SET proc_identificador =
			LTRIM(RTRIM(proceso.ceco_cliente)) + '-' +
			proceso.ceco_letra + '-' +
			proceso.ceco_centroEmisor + '-' +
			RIGHT(REPLICATE('0', 10) + CAST(proceso.ceco_numero AS VARCHAR(MAX)), 10)
		FROM tblCambioEstadoComprobante proceso
		WHERE proceso.ceco_proc_id = tblProceso.proc_id
			AND EXISTS
			(
				SELECT TOP 1 1
				FROM #CambioEstado existente
				WHERE existente.cliente = proceso.ceco_cliente COLLATE DATABASE_DEFAULT
					AND existente.letra = proceso.ceco_letra COLLATE DATABASE_DEFAULT
					AND existente.centroEmisor = proceso.ceco_centroEmisor COLLATE DATABASE_DEFAULT
					AND existente.numero = proceso.ceco_numero
			)

		IF (@cantidadAgrupamientos > 0)
		BEGIN
			SET @idProceso = 0
			SET @tipoProceso = 25

			EXEC INTERFAZ_CrearProcesos @tipoProceso, @cantidadAgrupamientos, @idProceso OUTPUT

			INSERT INTO tblAgrupamiento
			(
				agru_proc_id,
				agru_cliente,
				agru_letra,
				agru_centroEmisor,
				agru_numero,
				agru_concepto,
				agru_valor
			)
			SELECT
				(@idProceso + ROW_NUMBER() OVER(ORDER BY temporal.cliente ASC)) - 1 AS agru_proc_id,
				temporal.cliente AS agru_cliente,
				temporal.letra AS agru_letra,
				temporal.centroEmisor AS agru_centroEmisor,
				temporal.numero AS agru_numero,
				temporal.concepto AS agru_concepto,
				temporal.valor AS agru_valor
			FROM #Agrupamiento temporal
		END
	
		UPDATE tblProceso
		SET proc_identificador =
			LTRIM(RTRIM(proceso.agru_cliente)) + '-' +
			proceso.agru_letra + '-' +
			proceso.agru_centroEmisor + '-' +
			RIGHT(REPLICATE('0', 10) + CAST(proceso.agru_numero AS VARCHAR(MAX)), 10)
		FROM tblAgrupamiento proceso
		WHERE proceso.agru_proc_id = tblProceso.proc_id
			AND EXISTS
			(
				SELECT TOP 1 1
				FROM #Agrupamiento existente
				WHERE existente.cliente = proceso.agru_cliente COLLATE DATABASE_DEFAULT
					AND existente.letra = proceso.agru_letra COLLATE DATABASE_DEFAULT
					AND existente.centroEmisor = proceso.agru_centroEmisor COLLATE DATABASE_DEFAULT
					AND existente.numero = proceso.agru_numero
			)

		IF (@cantidadAsignacionesMuelle > 0)
		BEGIN
			SET @idProceso = 0
			SET @tipoProceso = 26

			EXEC INTERFAZ_CrearProcesos @tipoProceso, @cantidadAsignacionesMuelle, @idProceso OUTPUT

			INSERT INTO tblAsignacionMuelle
			(
				amue_proc_id,
				amue_cliente,
				amue_letra,
				amue_centroEmisor,
				amue_numero
			)
			SELECT
				(@idProceso + ROW_NUMBER() OVER(ORDER BY temporal.cliente ASC)) - 1 AS amue_proc_id,
				temporal.cliente AS amue_cliente,
				temporal.letra AS amue_letra,
				temporal.centroEmisor AS amue_centroEmisor,
				temporal.numero AS amue_numero
			FROM #AsignacionMuelle temporal
			WHERE NOT EXISTS
			(
				SELECT TOP 1 1
				FROM tblAsignacionMuelle existente
				WHERE existente.amue_cliente = temporal.cliente COLLATE DATABASE_DEFAULT
					AND existente.amue_letra = temporal.letra COLLATE DATABASE_DEFAULT
					AND existente.amue_centroEmisor = temporal.centroEmisor COLLATE DATABASE_DEFAULT
					AND existente.amue_numero = temporal.numero
			)
		END

		UPDATE tblProceso
		SET proc_identificador =
			LTRIM(RTRIM(proceso.amue_cliente)) + '-' +
			proceso.amue_letra + '-' +
			proceso.amue_centroEmisor + '-' +
			RIGHT(REPLICATE('0', 10) + CAST(proceso.amue_numero AS VARCHAR(MAX)), 10)
		FROM tblAsignacionMuelle proceso
		WHERE proceso.amue_proc_id = tblProceso.proc_id
			AND EXISTS
			(
				SELECT TOP 1 1
				FROM #AsignacionMuelle existente
				WHERE existente.cliente = proceso.amue_cliente COLLATE DATABASE_DEFAULT
					AND existente.letra = proceso.amue_letra COLLATE DATABASE_DEFAULT
					AND existente.centroEmisor = proceso.amue_centroEmisor COLLATE DATABASE_DEFAULT
					AND existente.numero = proceso.amue_numero
			)

		-- guarda los campos adicionales de la API no contemplados en tblComprobante
		INSERT INTO Calico_DB_API_TMS.dbo.tblDatosAdicionalesAPI
		(
		    Cliente,
		    Letra,
		    CentroEmisor,
		    Numero,
		    Remito,
		    KilosNetos,
		    CantidadPallets,
		    FechaPosibleEntrega,
		    SucursalOrigen,
		    Email,
		    Telefono,
		    LocalidadRemitente,
		    TipoIvaRemitente,
		    CuitRemitente,
		    PisoDeptoDestinatario,
		    TipoIvasDestino,
		    CuitDestino,
		    ObservacionEnvio_2,
		    ObservacionAdicionalEnvio,
		    CampoAdicional1,
		    CampoAdicional2,
		    CampoAdicional3,
		    CampoAdicional4,
		    CampoAdicional5,
		    Senasa,
		    TipoCarga,
		    TipoIvasDestinoAlternativo,
		    CuitDestinoAlternativo,
		    TelefonoDestinoAlternativo,
		    CodigoVerificadorEntrega,
		    DatosEntrega
		)
		VALUES
		(   @Cliente,
		    @LetraComprobante,
		    @PuntoDeVentaComprobante,
		    @NumeroComprobante,
		    @NumeroRemito,
		    @KilosNetos,
		    @CantidadPallets,
		    @FechaPosibleEntrega,
		    @SucursalOrigen,
		    @Email,
		    @Telefono,
		    @LocalidadRemitente,
		    @TipoIvaRemitente,
		    @CuitRemitente,
		    @PisoDeptoDestinatario,
		    @TipoIvasDestino,
		    @CuitDestino,
		    @ObservacionEnvio_2,
		    @ObservacionAdicionalEnvio,
		    @CampoAdicional1,
		    @CampoAdicional2,
		    @CampoAdicional3,
		    @CampoAdicional4,
		    @CampoAdicional5,
		    @Senasa,
		    @TipoCarga,
		    @TipoIvasDestinoAlternativo,
		    @CuitDestinoAlternativo,
		    @TelefonoDestinoAlternativo,
		    @CodigoVerificadorEntrega,
		    @DatosEntrega
		    )

        -- Si llegamos aquí, el pedido se insertó correctamente
        SET @Resultado = 'OK';
        SET @Mensaje = 'Pedido creado correctamente';

		DROP TABLE #AsignacionMuelle
		DROP TABLE #Agrupamiento
		DROP TABLE #CambioEstado

    END TRY
    BEGIN CATCH
        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        SET @Resultado = 'ERROR';
        SET @Mensaje = 'Error al procesar el pedido: ' + @ErrorMessage;

        -- Re-lanzar el error si es crítico
        IF @ErrorSeverity > 10
        BEGIN
            RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        END
    END CATCH

END
GO
