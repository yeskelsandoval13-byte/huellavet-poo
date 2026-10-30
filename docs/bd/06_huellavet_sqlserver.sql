/* ============================================================
   HuellaVet · Script de implementación para SQL Server 2022
   Generado desde docs/bd/modelo-bd.mjs
   Ejecutar en una instancia de SQL Server con permisos de CREATE DATABASE.
   ============================================================ */

IF DB_ID(N'HuellaVet') IS NULL
BEGIN
    CREATE DATABASE HuellaVet;
END
GO

USE HuellaVet;
GO

SET NOCOUNT ON;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* -- Especies ------------------------------------------- */
CREATE TABLE dbo.Especies (
    [EspecieId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(30) NOT NULL,
    CONSTRAINT [PK_Especies] PRIMARY KEY CLUSTERED ([EspecieId] ASC),
    CONSTRAINT [UQ_Especies_Nombre] UNIQUE ([Nombre])
);
GO

/* -- EstadosCita ------------------------------------------- */
CREATE TABLE dbo.EstadosCita (
    [EstadoCitaId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_EstadosCita] PRIMARY KEY CLUSTERED ([EstadoCitaId] ASC),
    CONSTRAINT [UQ_EstadosCita_Nombre] UNIQUE ([Nombre])
);
GO

/* -- MetodosPago ------------------------------------------- */
CREATE TABLE dbo.MetodosPago (
    [MetodoPagoId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(30) NOT NULL,
    CONSTRAINT [PK_MetodosPago] PRIMARY KEY CLUSTERED ([MetodoPagoId] ASC),
    CONSTRAINT [UQ_MetodosPago_Nombre] UNIQUE ([Nombre])
);
GO

/* -- CategoriasProducto ------------------------------------------- */
CREATE TABLE dbo.CategoriasProducto (
    [CategoriaProductoId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(40) NOT NULL,
    CONSTRAINT [PK_CategoriasProducto] PRIMARY KEY CLUSTERED ([CategoriaProductoId] ASC),
    CONSTRAINT [UQ_CategoriasProducto_Nombre] UNIQUE ([Nombre])
);
GO

/* -- CategoriasServicio ------------------------------------------- */
CREATE TABLE dbo.CategoriasServicio (
    [CategoriaServicioId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(40) NOT NULL,
    CONSTRAINT [PK_CategoriasServicio] PRIMARY KEY CLUSTERED ([CategoriaServicioId] ASC),
    CONSTRAINT [UQ_CategoriasServicio_Nombre] UNIQUE ([Nombre])
);
GO

/* -- Personas ------------------------------------------- */
CREATE TABLE dbo.Personas (
    [PersonaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Nombre] NVARCHAR(80) NOT NULL,
    [Apellido] NVARCHAR(80) NOT NULL,
    [Documento] NVARCHAR(30) NOT NULL,
    [Telefono] NVARCHAR(25) NOT NULL,
    [Email] NVARCHAR(120) NULL,
    [TipoPersona] CHAR(1) NOT NULL DEFAULT 'C',
    [CreadoEn] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [ActualizadoEn] DATETIME2(0) NULL,
    CONSTRAINT [PK_Personas] PRIMARY KEY CLUSTERED ([PersonaId] ASC),
    CONSTRAINT [UQ_Personas_Documento] UNIQUE ([Documento]),
    CONSTRAINT [CK_Personas_TipoPersona] CHECK (TipoPersona IN ('C','E'))
);
GO

/* -- Clientes ------------------------------------------- */
CREATE TABLE dbo.Clientes (
    [ClienteId] UNIQUEIDENTIFIER NOT NULL,
    [Direccion] NVARCHAR(200) NULL,
    [FechaRegistro] DATE NOT NULL DEFAULT CAST(SYSDATETIME() AS date),
    [PuntosFidelidad] INT NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([ClienteId] ASC),
    CONSTRAINT [CK_Clientes_PuntosFidelidad] CHECK (PuntosFidelidad >= 0),
    CONSTRAINT [FK_Clientes_Personas] FOREIGN KEY ([ClienteId]) REFERENCES dbo.Personas ([PersonaId])
);
GO

/* -- Empleados ------------------------------------------- */
CREATE TABLE dbo.Empleados (
    [EmpleadoId] UNIQUEIDENTIFIER NOT NULL,
    [SalarioBase] DECIMAL(12,2) NOT NULL,
    [FechaIngreso] DATE NOT NULL,
    [Activo] BIT NOT NULL DEFAULT (1),
    [TipoEmpleado] CHAR(1) NOT NULL DEFAULT 'V',
    CONSTRAINT [PK_Empleados] PRIMARY KEY CLUSTERED ([EmpleadoId] ASC),
    CONSTRAINT [CK_Empleados_SalarioBase] CHECK (SalarioBase > 0),
    CONSTRAINT [CK_Empleados_TipoEmpleado] CHECK (TipoEmpleado IN ('V','R','A')),
    CONSTRAINT [FK_Empleados_Personas] FOREIGN KEY ([EmpleadoId]) REFERENCES dbo.Personas ([PersonaId])
);
GO

/* -- Veterinarios ------------------------------------------- */
CREATE TABLE dbo.Veterinarios (
    [VeterinarioId] UNIQUEIDENTIFIER NOT NULL,
    [Especialidad] NVARCHAR(80) NOT NULL,
    [NumeroColegiatura] NVARCHAR(30) NOT NULL,
    [AniosExperiencia] TINYINT NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Veterinarios] PRIMARY KEY CLUSTERED ([VeterinarioId] ASC),
    CONSTRAINT [UQ_Veterinarios_NumeroColegiatura] UNIQUE ([NumeroColegiatura]),
    CONSTRAINT [CK_Veterinarios_AniosExperiencia] CHECK (AniosExperiencia BETWEEN 0 AND 70),
    CONSTRAINT [FK_Veterinarios_Empleados] FOREIGN KEY ([VeterinarioId]) REFERENCES dbo.Empleados ([EmpleadoId])
);
GO

/* -- Recepcionistas ------------------------------------------- */
CREATE TABLE dbo.Recepcionistas (
    [RecepcionistaId] UNIQUEIDENTIFIER NOT NULL,
    [Turno] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_Recepcionistas] PRIMARY KEY CLUSTERED ([RecepcionistaId] ASC),
    CONSTRAINT [CK_Recepcionistas_Turno] CHECK (Turno IN ('Matutino','Vespertino','Nocturno')),
    CONSTRAINT [FK_Recepcionistas_Empleados] FOREIGN KEY ([RecepcionistaId]) REFERENCES dbo.Empleados ([EmpleadoId])
);
GO

/* -- AuxiliaresVeterinarios ------------------------------------------- */
CREATE TABLE dbo.AuxiliaresVeterinarios (
    [AuxiliarId] UNIQUEIDENTIFIER NOT NULL,
    [Certificacion] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_AuxiliaresVeterinarios] PRIMARY KEY CLUSTERED ([AuxiliarId] ASC),
    CONSTRAINT [FK_AuxiliaresVeterinarios_Empleados] FOREIGN KEY ([AuxiliarId]) REFERENCES dbo.Empleados ([EmpleadoId])
);
GO

/* -- Mascotas ------------------------------------------- */
CREATE TABLE dbo.Mascotas (
    [MascotaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ClienteId] UNIQUEIDENTIFIER NOT NULL,
    [EspecieId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(60) NOT NULL,
    [Raza] NVARCHAR(60) NOT NULL,
    [FechaNacimiento] DATE NOT NULL,
    [PesoKg] DECIMAL(5,2) NOT NULL,
    [Sexo] CHAR(1) NOT NULL DEFAULT 'D',
    [Alergias] NVARCHAR(300) NULL,
    [Notas] NVARCHAR(500) NULL,
    [Esterilizado] BIT NOT NULL DEFAULT (0),
    [UltimaVisita] DATETIME2(0) NULL,
    [CreadoEn] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [ActualizadoEn] DATETIME2(0) NULL,
    CONSTRAINT [PK_Mascotas] PRIMARY KEY CLUSTERED ([MascotaId] ASC),
    CONSTRAINT [CK_Mascotas_PesoKg] CHECK (PesoKg > 0 AND PesoKg <= 200),
    CONSTRAINT [CK_Mascotas_Sexo] CHECK (Sexo IN ('M','H','D')),
    CONSTRAINT [FK_Mascotas_Clientes] FOREIGN KEY ([ClienteId]) REFERENCES dbo.Clientes ([ClienteId]),
    CONSTRAINT [FK_Mascotas_Especies] FOREIGN KEY ([EspecieId]) REFERENCES dbo.Especies ([EspecieId])
);
GO

/* -- Perros ------------------------------------------- */
CREATE TABLE dbo.Perros (
    [MascotaId] UNIQUEIDENTIFIER NOT NULL,
    [Talla] NVARCHAR(10) NOT NULL DEFAULT 'Mediana',
    [Entrenado] BIT NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Perros] PRIMARY KEY CLUSTERED ([MascotaId] ASC),
    CONSTRAINT [CK_Perros_Talla] CHECK (Talla IN ('Pequena','Mediana','Grande','Gigante')),
    CONSTRAINT [FK_Perros_Mascotas] FOREIGN KEY ([MascotaId]) REFERENCES dbo.Mascotas ([MascotaId])
);
GO

/* -- Gatos ------------------------------------------- */
CREATE TABLE dbo.Gatos (
    [MascotaId] UNIQUEIDENTIFIER NOT NULL,
    [EsIndoor] BIT NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Gatos] PRIMARY KEY CLUSTERED ([MascotaId] ASC),
    CONSTRAINT [FK_Gatos_Mascotas] FOREIGN KEY ([MascotaId]) REFERENCES dbo.Mascotas ([MascotaId])
);
GO

/* -- Aves ------------------------------------------- */
CREATE TABLE dbo.Aves (
    [MascotaId] UNIQUEIDENTIFIER NOT NULL,
    [PuedeVolar] BIT NOT NULL DEFAULT (1),
    [ColorPlumaje] NVARCHAR(40) NOT NULL DEFAULT 'Verde',
    CONSTRAINT [PK_Aves] PRIMARY KEY CLUSTERED ([MascotaId] ASC),
    CONSTRAINT [FK_Aves_Mascotas] FOREIGN KEY ([MascotaId]) REFERENCES dbo.Mascotas ([MascotaId])
);
GO

/* -- Conejos ------------------------------------------- */
CREATE TABLE dbo.Conejos (
    [MascotaId] UNIQUEIDENTIFIER NOT NULL,
    [LargoOrejasCm] DECIMAL(4,1) NOT NULL DEFAULT (8.0),
    CONSTRAINT [PK_Conejos] PRIMARY KEY CLUSTERED ([MascotaId] ASC),
    CONSTRAINT [CK_Conejos_LargoOrejasCm] CHECK (LargoOrejasCm > 0),
    CONSTRAINT [FK_Conejos_Mascotas] FOREIGN KEY ([MascotaId]) REFERENCES dbo.Mascotas ([MascotaId])
);
GO

/* -- Servicios ------------------------------------------- */
CREATE TABLE dbo.Servicios (
    [ServicioId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Codigo] NVARCHAR(20) NOT NULL,
    [Nombre] NVARCHAR(80) NOT NULL,
    [PrecioBase] DECIMAL(12,2) NOT NULL,
    [DuracionMinutos] SMALLINT NOT NULL DEFAULT (30),
    [RequiereAyuno] BIT NOT NULL DEFAULT (0),
    [Activo] BIT NOT NULL DEFAULT (1),
    [CategoriaServicioId] TINYINT NOT NULL,
    [TipoServicio] CHAR(1) NOT NULL DEFAULT 'C',
    [CreadoEn] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [ActualizadoEn] DATETIME2(0) NULL,
    CONSTRAINT [PK_Servicios] PRIMARY KEY CLUSTERED ([ServicioId] ASC),
    CONSTRAINT [UQ_Servicios_Codigo] UNIQUE ([Codigo]),
    CONSTRAINT [CK_Servicios_PrecioBase] CHECK (PrecioBase > 0),
    CONSTRAINT [CK_Servicios_DuracionMinutos] CHECK (DuracionMinutos > 0),
    CONSTRAINT [CK_Servicios_TipoServicio] CHECK (TipoServicio IN ('C','V','G','P','H')),
    CONSTRAINT [FK_Servicios_CategoriasServicio] FOREIGN KEY ([CategoriaServicioId]) REFERENCES dbo.CategoriasServicio ([CategoriaServicioId])
);
GO

/* -- ConsultasGenerales ------------------------------------------- */
CREATE TABLE dbo.ConsultasGenerales (
    [ServicioId] UNIQUEIDENTIFIER NOT NULL,
    [IncluyeReceta] BIT NOT NULL DEFAULT (1),
    CONSTRAINT [PK_ConsultasGenerales] PRIMARY KEY CLUSTERED ([ServicioId] ASC),
    CONSTRAINT [FK_ConsultasGenerales_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId])
);
GO

/* -- Vacunaciones ------------------------------------------- */
CREATE TABLE dbo.Vacunaciones (
    [ServicioId] UNIQUEIDENTIFIER NOT NULL,
    [Vacuna] NVARCHAR(80) NOT NULL,
    [ProximaDosisMeses] TINYINT NOT NULL DEFAULT (12),
    CONSTRAINT [PK_Vacunaciones] PRIMARY KEY CLUSTERED ([ServicioId] ASC),
    CONSTRAINT [CK_Vacunaciones_ProximaDosisMeses] CHECK (ProximaDosisMeses >= 1),
    CONSTRAINT [FK_Vacunaciones_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId])
);
GO

/* -- Cirugias ------------------------------------------- */
CREATE TABLE dbo.Cirugias (
    [ServicioId] UNIQUEIDENTIFIER NOT NULL,
    [NivelRiesgo] CHAR(1) NOT NULL DEFAULT 'M',
    [RequiereHospitalizacion] BIT NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Cirugias] PRIMARY KEY CLUSTERED ([ServicioId] ASC),
    CONSTRAINT [CK_Cirugias_NivelRiesgo] CHECK (NivelRiesgo IN ('B','M','A')),
    CONSTRAINT [FK_Cirugias_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId])
);
GO

/* -- Peluquerias ------------------------------------------- */
CREATE TABLE dbo.Peluquerias (
    [ServicioId] UNIQUEIDENTIFIER NOT NULL,
    [TipoCorte] NVARCHAR(20) NOT NULL DEFAULT 'Completo',
    [IncluyeBano] BIT NOT NULL DEFAULT (1),
    CONSTRAINT [PK_Peluquerias] PRIMARY KEY CLUSTERED ([ServicioId] ASC),
    CONSTRAINT [CK_Peluquerias_TipoCorte] CHECK (TipoCorte IN ('Higienico','Completo','Raza','Creativo')),
    CONSTRAINT [FK_Peluquerias_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId])
);
GO

/* -- Hospitalizaciones ------------------------------------------- */
CREATE TABLE dbo.Hospitalizaciones (
    [ServicioId] UNIQUEIDENTIFIER NOT NULL,
    [Dias] SMALLINT NOT NULL DEFAULT (1),
    [CostoPorDia] DECIMAL(12,2) NOT NULL DEFAULT (850),
    CONSTRAINT [PK_Hospitalizaciones] PRIMARY KEY CLUSTERED ([ServicioId] ASC),
    CONSTRAINT [CK_Hospitalizaciones_Dias] CHECK (Dias >= 1),
    CONSTRAINT [CK_Hospitalizaciones_CostoPorDia] CHECK (CostoPorDia > 0),
    CONSTRAINT [FK_Hospitalizaciones_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId])
);
GO

/* -- Citas ------------------------------------------- */
CREATE TABLE dbo.Citas (
    [CitaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ClienteId] UNIQUEIDENTIFIER NOT NULL,
    [MascotaId] UNIQUEIDENTIFIER NOT NULL,
    [VeterinarioId] UNIQUEIDENTIFIER NOT NULL,
    [ServicioId] UNIQUEIDENTIFIER NOT NULL,
    [EstadoCitaId] TINYINT NOT NULL,
    [FechaHora] DATETIME2(0) NOT NULL,
    [Motivo] NVARCHAR(300) NOT NULL,
    [Diagnostico] NVARCHAR(1000) NULL,
    [PesoRegistrado] DECIMAL(5,2) NULL,
    [FechaAtencion] DATETIME2(0) NULL,
    [Observaciones] NVARCHAR(1000) NULL,
    [MotivoCancelacion] NVARCHAR(300) NULL,
    [CreadoEn] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [ActualizadoEn] DATETIME2(0) NULL,
    CONSTRAINT [PK_Citas] PRIMARY KEY CLUSTERED ([CitaId] ASC),
    CONSTRAINT [CK_Citas_PesoRegistrado] CHECK (PesoRegistrado IS NULL OR (PesoRegistrado > 0 AND PesoRegistrado <= 200)),
    CONSTRAINT [FK_Citas_Clientes] FOREIGN KEY ([ClienteId]) REFERENCES dbo.Clientes ([ClienteId]),
    CONSTRAINT [FK_Citas_Mascotas] FOREIGN KEY ([MascotaId]) REFERENCES dbo.Mascotas ([MascotaId]),
    CONSTRAINT [FK_Citas_Veterinarios] FOREIGN KEY ([VeterinarioId]) REFERENCES dbo.Veterinarios ([VeterinarioId]),
    CONSTRAINT [FK_Citas_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId]),
    CONSTRAINT [FK_Citas_EstadosCita] FOREIGN KEY ([EstadoCitaId]) REFERENCES dbo.EstadosCita ([EstadoCitaId])
);
GO

/* -- Facturas ------------------------------------------- */
CREATE TABLE dbo.Facturas (
    [FacturaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Numero] NVARCHAR(20) NOT NULL,
    [ClienteId] UNIQUEIDENTIFIER NOT NULL,
    [CitaId] UNIQUEIDENTIFIER NULL,
    [Fecha] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [DescuentoPorcentaje] DECIMAL(5,2) NOT NULL DEFAULT (0),
    [ImpuestoPorcentaje] DECIMAL(5,2) NOT NULL DEFAULT (15),
    [Pagada] BIT NOT NULL DEFAULT (0),
    [MetodoPagoId] TINYINT NULL,
    [FechaPago] DATETIME2(0) NULL,
    [CreadoEn] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [ActualizadoEn] DATETIME2(0) NULL,
    CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([FacturaId] ASC),
    CONSTRAINT [UQ_Facturas_Numero] UNIQUE ([Numero]),
    CONSTRAINT [CK_Facturas_DescuentoPorcentaje] CHECK (DescuentoPorcentaje BETWEEN 0 AND 50),
    CONSTRAINT [CK_Facturas_ImpuestoPorcentaje] CHECK (ImpuestoPorcentaje BETWEEN 0 AND 100),
    CONSTRAINT [CK_Facturas_Pago_Coherente] CHECK ((Pagada = 0 AND MetodoPagoId IS NULL AND FechaPago IS NULL) OR (Pagada = 1 AND MetodoPagoId IS NOT NULL AND FechaPago IS NOT NULL)),
    CONSTRAINT [FK_Facturas_Clientes] FOREIGN KEY ([ClienteId]) REFERENCES dbo.Clientes ([ClienteId]),
    CONSTRAINT [FK_Facturas_Citas] FOREIGN KEY ([CitaId]) REFERENCES dbo.Citas ([CitaId]),
    CONSTRAINT [FK_Facturas_MetodosPago] FOREIGN KEY ([MetodoPagoId]) REFERENCES dbo.MetodosPago ([MetodoPagoId])
);
GO

/* -- DetallesFactura ------------------------------------------- */
CREATE TABLE dbo.DetallesFactura (
    [DetalleFacturaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [FacturaId] UNIQUEIDENTIFIER NOT NULL,
    [ServicioId] UNIQUEIDENTIFIER NULL,
    [Descripcion] NVARCHAR(200) NOT NULL,
    [Cantidad] INT NOT NULL DEFAULT (1),
    [PrecioUnitario] DECIMAL(12,2) NOT NULL,
    CONSTRAINT [PK_DetallesFactura] PRIMARY KEY CLUSTERED ([DetalleFacturaId] ASC),
    CONSTRAINT [CK_DetallesFactura_Cantidad] CHECK (Cantidad > 0),
    CONSTRAINT [CK_DetallesFactura_PrecioUnitario] CHECK (PrecioUnitario >= 0),
    CONSTRAINT [FK_DetallesFactura_Facturas] FOREIGN KEY ([FacturaId]) REFERENCES dbo.Facturas ([FacturaId]),
    CONSTRAINT [FK_DetallesFactura_Servicios] FOREIGN KEY ([ServicioId]) REFERENCES dbo.Servicios ([ServicioId])
);
GO

/* -- Productos ------------------------------------------- */
CREATE TABLE dbo.Productos (
    [ProductoId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CategoriaProductoId] TINYINT NOT NULL,
    [Nombre] NVARCHAR(100) NOT NULL,
    [Precio] DECIMAL(12,2) NOT NULL,
    [Stock] INT NOT NULL DEFAULT (0),
    [StockMinimo] INT NOT NULL DEFAULT (5),
    [Vencimiento] DATE NULL,
    [CreadoEn] DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
    [ActualizadoEn] DATETIME2(0) NULL,
    CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED ([ProductoId] ASC),
    CONSTRAINT [CK_Productos_Precio] CHECK (Precio > 0),
    CONSTRAINT [CK_Productos_Stock] CHECK (Stock >= 0),
    CONSTRAINT [CK_Productos_StockMinimo] CHECK (StockMinimo >= 0),
    CONSTRAINT [FK_Productos_CategoriasProducto] FOREIGN KEY ([CategoriaProductoId]) REFERENCES dbo.CategoriasProducto ([CategoriaProductoId])
);
GO

/* -- Índices -------------------------------------------------- */
CREATE INDEX [IX_Personas_Apellido_Nombre] ON dbo.Personas ([Apellido], [Nombre]);
CREATE INDEX [IX_Personas_Email] ON dbo.Personas ([Email]);
CREATE INDEX [IX_Clientes_FechaRegistro] ON dbo.Clientes ([FechaRegistro]);
CREATE INDEX [IX_Empleados_Activo_Tipo] ON dbo.Empleados ([Activo], [TipoEmpleado]);
CREATE INDEX [IX_Mascotas_Cliente] ON dbo.Mascotas ([ClienteId]);
CREATE INDEX [IX_Mascotas_Especie] ON dbo.Mascotas ([EspecieId]);
CREATE INDEX [IX_Mascotas_Nombre] ON dbo.Mascotas ([Nombre]);
CREATE INDEX [IX_Servicios_Categoria_Activo] ON dbo.Servicios ([CategoriaServicioId], [Activo]);
CREATE INDEX [IX_Servicios_Nombre] ON dbo.Servicios ([Nombre]);
CREATE INDEX [IX_Citas_Veterinario_Fecha] ON dbo.Citas ([VeterinarioId], [FechaHora]);
CREATE INDEX [IX_Citas_Mascota_Fecha] ON dbo.Citas ([MascotaId], [FechaHora]);
CREATE INDEX [IX_Citas_Fecha_Estado] ON dbo.Citas ([FechaHora], [EstadoCitaId]);
CREATE INDEX [IX_Citas_Cliente_Fecha] ON dbo.Citas ([ClienteId], [FechaHora]);
CREATE UNIQUE INDEX [UX_Facturas_Cita] ON dbo.Facturas ([CitaId]) WHERE CitaId IS NOT NULL;
CREATE INDEX [IX_Facturas_Cliente_Fecha] ON dbo.Facturas ([ClienteId], [Fecha]);
CREATE INDEX [IX_Facturas_Pagada_Fecha] ON dbo.Facturas ([Pagada], [Fecha]);
CREATE INDEX [IX_DetallesFactura_Factura] ON dbo.DetallesFactura ([FacturaId]);
CREATE INDEX [IX_DetallesFactura_Servicio] ON dbo.DetallesFactura ([ServicioId]);
CREATE INDEX [IX_Productos_Categoria] ON dbo.Productos ([CategoriaProductoId]);
CREATE INDEX [IX_Productos_Stock] ON dbo.Productos ([Stock]);
GO

/* -- Datos de catálogo ---------------------------------------- */
INSERT INTO dbo.Especies ([EspecieId], [Nombre]) VALUES
    (1, N'Perro'),
    (2, N'Gato'),
    (3, N'Ave'),
    (4, N'Conejo');
GO

INSERT INTO dbo.EstadosCita ([EstadoCitaId], [Nombre]) VALUES
    (1, N'Programada'),
    (2, N'Confirmada'),
    (3, N'EnAtencion'),
    (4, N'Atendida'),
    (5, N'Cancelada'),
    (6, N'NoAsistio');
GO

INSERT INTO dbo.MetodosPago ([MetodoPagoId], [Nombre]) VALUES
    (1, N'Efectivo'),
    (2, N'Tarjeta'),
    (3, N'Transferencia');
GO

INSERT INTO dbo.CategoriasProducto ([CategoriaProductoId], [Nombre]) VALUES
    (1, N'Medicamento'),
    (2, N'Alimento'),
    (3, N'Insumo'),
    (4, N'Higiene');
GO

INSERT INTO dbo.CategoriasServicio ([CategoriaServicioId], [Nombre]) VALUES
    (1, N'Consulta'),
    (2, N'Vacunación'),
    (3, N'Cirugía'),
    (4, N'Estética'),
    (5, N'Hospitalización');
GO

/* -- Vistas de consulta --------------------------------------- */
CREATE OR ALTER VIEW dbo.vw_Empleados
AS
SELECT
    p.PersonaId AS EmpleadoId,
    p.Nombre + N' ' + p.Apellido AS NombreCompleto,
    p.Documento,
    p.Telefono,
    p.Email,
    e.TipoEmpleado,
    CASE e.TipoEmpleado
        WHEN 'V' THEN N'Veterinario'
        WHEN 'R' THEN N'Recepcionista'
        ELSE N'Auxiliar veterinario'
    END AS Cargo,
    e.SalarioBase,
    e.FechaIngreso,
    e.Activo,
    v.Especialidad,
    v.NumeroColegiatura,
    v.AniosExperiencia,
    r.Turno,
    a.Certificacion
FROM dbo.Personas p
INNER JOIN dbo.Empleados e ON e.EmpleadoId = p.PersonaId
LEFT JOIN dbo.Veterinarios v ON v.VeterinarioId = e.EmpleadoId
LEFT JOIN dbo.Recepcionistas r ON r.RecepcionistaId = e.EmpleadoId
LEFT JOIN dbo.AuxiliaresVeterinarios a ON a.AuxiliarId = e.EmpleadoId;
GO

CREATE OR ALTER VIEW dbo.vw_Mascotas
AS
SELECT
    m.MascotaId,
    m.Nombre,
    esp.Nombre AS Especie,
    m.Raza,
    m.FechaNacimiento,
    DATEDIFF(YEAR, m.FechaNacimiento, CAST(SYSDATETIME() AS date))
      - CASE
          WHEN DATEADD(YEAR, DATEDIFF(YEAR, m.FechaNacimiento, CAST(SYSDATETIME() AS date)), m.FechaNacimiento) > CAST(SYSDATETIME() AS date)
          THEN 1 ELSE 0
        END AS EdadAnios,
    m.PesoKg,
    m.Sexo,
    m.Alergias,
    m.Esterilizado,
    m.UltimaVisita,
    p.Nombre + N' ' + p.Apellido AS Dueno
FROM dbo.Mascotas m
INNER JOIN dbo.Especies esp ON esp.EspecieId = m.EspecieId
INNER JOIN dbo.Clientes c ON c.ClienteId = m.ClienteId
INNER JOIN dbo.Personas p ON p.PersonaId = c.ClienteId;
GO

CREATE OR ALTER VIEW dbo.vw_Agenda
AS
SELECT
    c.CitaId,
    CAST(c.FechaHora AS date) AS Fecha,
    c.FechaHora,
    p.Nombre + N' ' + p.Apellido AS Cliente,
    m.Nombre AS Mascota,
    esp.Nombre AS Especie,
    vp.Nombre + N' ' + vp.Apellido AS Veterinario,
    s.Nombre AS Servicio,
    ec.Nombre AS Estado,
    c.Motivo
FROM dbo.Citas c
INNER JOIN dbo.Clientes cl ON cl.ClienteId = c.ClienteId
INNER JOIN dbo.Personas p ON p.PersonaId = cl.ClienteId
INNER JOIN dbo.Mascotas m ON m.MascotaId = c.MascotaId
INNER JOIN dbo.Especies esp ON esp.EspecieId = m.EspecieId
INNER JOIN dbo.Veterinarios v ON v.VeterinarioId = c.VeterinarioId
INNER JOIN dbo.Personas vp ON vp.PersonaId = v.VeterinarioId
INNER JOIN dbo.Servicios s ON s.ServicioId = c.ServicioId
INNER JOIN dbo.EstadosCita ec ON ec.EstadoCitaId = c.EstadoCitaId;
GO

CREATE OR ALTER VIEW dbo.vw_FacturasResumen
AS
WITH Base AS (
    SELECT
        f.FacturaId,
        f.Numero,
        f.ClienteId,
        f.CitaId,
        f.Fecha,
        f.Pagada,
        f.DescuentoPorcentaje,
        f.ImpuestoPorcentaje,
        CAST(COALESCE(SUM(d.Cantidad * d.PrecioUnitario), 0) AS DECIMAL(12,2)) AS Subtotal
    FROM dbo.Facturas f
    LEFT JOIN dbo.DetallesFactura d ON d.FacturaId = f.FacturaId
    GROUP BY f.FacturaId, f.Numero, f.ClienteId, f.CitaId, f.Fecha, f.Pagada,
             f.DescuentoPorcentaje, f.ImpuestoPorcentaje
),
Calculada AS (
    SELECT
        *,
        CAST(Subtotal * DescuentoPorcentaje / 100.0 AS DECIMAL(12,2)) AS Descuento,
        CAST((Subtotal - (Subtotal * DescuentoPorcentaje / 100.0)) * ImpuestoPorcentaje / 100.0 AS DECIMAL(12,2)) AS Impuesto
    FROM Base
)
SELECT
    FacturaId,
    Numero,
    ClienteId,
    CitaId,
    Fecha,
    Pagada,
    Subtotal,
    Descuento,
    Impuesto,
    CAST(Subtotal - Descuento + Impuesto AS DECIMAL(12,2)) AS Total
FROM Calculada;
GO

CREATE OR ALTER VIEW dbo.vw_StockBajo
AS
SELECT
    p.ProductoId,
    p.Nombre,
    cp.Nombre AS Categoria,
    p.Stock,
    p.StockMinimo,
    p.Precio,
    p.Vencimiento
FROM dbo.Productos p
INNER JOIN dbo.CategoriasProducto cp ON cp.CategoriaProductoId = p.CategoriaProductoId
WHERE p.Stock <= p.StockMinimo;
GO

CREATE OR ALTER VIEW dbo.vw_VacunasProximas
AS
WITH Proximas AS (
    SELECT
        m.Nombre AS Mascota,
        p.Nombre + N' ' + p.Apellido AS Dueno,
        vac.Vacuna,
        DATEADD(MONTH, vac.ProximaDosisMeses, COALESCE(c.FechaAtencion, c.FechaHora)) AS ProximaDosis
    FROM dbo.Citas c
    INNER JOIN dbo.EstadosCita ec ON ec.EstadoCitaId = c.EstadoCitaId
    INNER JOIN dbo.Vacunaciones vac ON vac.ServicioId = c.ServicioId
    INNER JOIN dbo.Mascotas m ON m.MascotaId = c.MascotaId
    INNER JOIN dbo.Clientes cl ON cl.ClienteId = c.ClienteId
    INNER JOIN dbo.Personas p ON p.PersonaId = cl.ClienteId
    WHERE ec.Nombre = N'Atendida'
)
SELECT
    Mascota,
    Dueno,
    Vacuna,
    ProximaDosis,
    DATEDIFF(DAY, CAST(SYSDATETIME() AS date), CAST(ProximaDosis AS date)) AS Dias
FROM Proximas
WHERE CAST(ProximaDosis AS date) BETWEEN CAST(SYSDATETIME() AS date) AND DATEADD(DAY, 45, CAST(SYSDATETIME() AS date));
GO

PRINT N'Base HuellaVet creada correctamente.';
GO
