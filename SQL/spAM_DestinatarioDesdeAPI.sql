-- =============================================
-- Stored Procedure: spAM_DestinatarioDesdeAPI
-- Descripción: Gestiona el alta/modificación de destinatarios desde la API
-- Base de datos: DB_Master (CalicoMasterDapperContext)
-- Fecha creación: 2025-09-30
-- Fecha modificación: 2025-09-30 - Agregado parámetro @Cliente
-- =============================================

ALTER PROCEDURE [dbo].[spAM_DestinatarioDesdeAPI]
    @Cliente NVARCHAR(10),
    @NumeroClienteDestino NVARCHAR(10),
    @NombreDestinatario NVARCHAR(30),
    @DomicilioDestinatario NVARCHAR(30),
    @NumeroCalleDestinatario NVARCHAR(5),
    @PisoDeptoDestinatario NVARCHAR(5),
    @LocalidadDestinatario NVARCHAR(20),
    @CodigoPostalDestinatario NVARCHAR(10),
    @TipoIvasDestino NVARCHAR(1),
    @CuitDestino NVARCHAR(13)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        -- Validaciones de datos obligatorios
        IF @Cliente IS NULL OR LTRIM(RTRIM(@Cliente)) = ''
        BEGIN
            RAISERROR('El código de cliente es obligatorio', 16, 1);
            RETURN;
        END

        IF @NumeroClienteDestino IS NULL OR LTRIM(RTRIM(@NumeroClienteDestino)) = ''
        BEGIN
            RAISERROR('El número de cliente destino es obligatorio', 16, 1);
            RETURN;
        END

        IF @NombreDestinatario IS NULL OR LTRIM(RTRIM(@NombreDestinatario)) = ''
        BEGIN
            RAISERROR('El nombre del destinatario es obligatorio', 16, 1);
            RETURN;
        END

        -- Verificar si el destinatario ya existe para este cliente
        IF EXISTS (SELECT 1 FROM dbo.tblDestinatario WHERE dest_clie_codigo = @Cliente AND dest_codigo = @NumeroClienteDestino)
        BEGIN
            -- Actualizar destinatario existente
            UPDATE dbo.tblDestinatario
            SET
                dest_razonSocial = LTRIM(RTRIM(@NombreDestinatario)),
                dest_nombreCalle = LTRIM(RTRIM(ISNULL(@DomicilioDestinatario, ''))),
                dest_numeroCalle = LTRIM(RTRIM(ISNULL(@NumeroCalleDestinatario, ''))),
                dest_piso = LTRIM(RTRIM(ISNULL(@PisoDeptoDestinatario, ''))),
				dest_domicilio = LTRIM(RTRIM(ISNULL(@DomicilioDestinatario, ''))) + ' ' + LTRIM(RTRIM(ISNULL(@NumeroCalleDestinatario, ''))) + ', ' + LTRIM(RTRIM(ISNULL(@LocalidadDestinatario, ''))),
                dest_loca_codigoPostal = LTRIM(RTRIM(ISNULL(@CodigoPostalDestinatario, ''))),
                dest_tiva_codigo = LTRIM(RTRIM(ISNULL(@TipoIvasDestino, ''))),
                dest_numeroCuit = LTRIM(RTRIM(ISNULL(@CuitDestino, '')))
            WHERE dest_clie_codigo = @Cliente
              AND dest_codigo = @NumeroClienteDestino;
        END
        ELSE
        BEGIN
            -- Insertar nuevo destinatario
            INSERT INTO dbo.tblDestinatario
			(
				dest_id,
                dest_clie_codigo,
                dest_codigo,
                dest_razonSocial,
                dest_nombreCalle,
                dest_numeroCalle,
                dest_piso,
                dest_domicilio,
                dest_loca_codigoPostal,
                dest_tiva_codigo,
                dest_numeroCuit
            )
            VALUES (
				(SELECT MAX(dest_id) + 1 FROM dbo.tblDestinatario),
                @Cliente,
                @NumeroClienteDestino,
                LTRIM(RTRIM(@NombreDestinatario)),
                LTRIM(RTRIM(ISNULL(@DomicilioDestinatario, ''))),
                LTRIM(RTRIM(ISNULL(@NumeroCalleDestinatario, ''))),
                LTRIM(RTRIM(ISNULL(@PisoDeptoDestinatario, ''))),
                LTRIM(RTRIM(ISNULL(@DomicilioDestinatario, ''))) + ' ' + LTRIM(RTRIM(ISNULL(@NumeroCalleDestinatario, ''))) + ', ' + LTRIM(RTRIM(ISNULL(@LocalidadDestinatario, ''))),
                LTRIM(RTRIM(ISNULL(@CodigoPostalDestinatario, ''))),
                LTRIM(RTRIM(ISNULL(@TipoIvasDestino, ''))),
                LTRIM(RTRIM(ISNULL(@CuitDestino, '')))
            );
        END

    END TRY
    BEGIN CATCH
        SELECT
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        -- Registrar el error o manejarlo según la lógica de negocio
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
