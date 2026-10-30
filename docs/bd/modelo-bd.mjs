// Modelo de datos de HuellaVet.
// Fuente única para los diagramas .drawio, el diccionario y el script SQL.
// Ejecutar: node docs/bd/generar_diagramas.mjs

export const palette = {
  personas: { fill: '#F0FDFA', stroke: '#0F766E', title: '#0F766E', section: '#CCFBF1' },
  mascotas: { fill: '#FFFBEB', stroke: '#B45309', title: '#B45309', section: '#FEF3C7' },
  servicios: { fill: '#EFF6FF', stroke: '#1D4ED8', title: '#1D4ED8', section: '#DBEAFE' },
  operacion: { fill: '#FFF1F2', stroke: '#BE123C', title: '#BE123C', section: '#FFE4E6' },
  catalogos: { fill: '#F8FAFC', stroke: '#475569', title: '#475569', section: '#E2E8F0' }
};

const f = (name, type, key = '', nullable = false, desc = '', fk = '', defaultValue = '', check = '') => ({
  name,
  type,
  key,
  nullable,
  desc,
  fk,
  defaultValue,
  check
});

export const tables = [
  // ── Catálogos ──────────────────────────────────────────────────
  {
    id: 'especies',
    title: 'Especies',
    section: 'catalogos',
    subtitle: 'Catálogo de especies atendidas',
    purpose: 'Normaliza la especie de cada mascota y evita repetir el texto en cada fila.',
    x: 70,
    y: 140,
    w: 260,
    fields: [
      f('EspecieId', 'TINYINT', 'PK', false, 'Identificador de la especie.'),
      f('Nombre', 'NVARCHAR(30)', 'UQ', false, 'Perro, Gato, Ave o Conejo.')
    ],
    indexes: []
  },
  {
    id: 'estadosCita',
    title: 'EstadosCita',
    section: 'catalogos',
    subtitle: 'Ciclo de vida de una cita',
    purpose: 'Catálogo de estados para conservar la integridad del ciclo de la cita.',
    x: 1920,
    y: 140,
    w: 240,
    fields: [
      f('EstadoCitaId', 'TINYINT', 'PK', false, 'Identificador del estado.'),
      f('Nombre', 'NVARCHAR(20)', 'UQ', false, 'Programada, Confirmada, EnAtencion, Atendida, Cancelada o NoAsistio.')
    ],
    indexes: []
  },
  {
    id: 'metodosPago',
    title: 'MetodosPago',
    section: 'catalogos',
    subtitle: 'Formas de pago aceptadas',
    purpose: 'Catálogo de métodos de pago; la factura solo lo usa cuando está pagada.',
    x: 1920,
    y: 520,
    w: 240,
    fields: [
      f('MetodoPagoId', 'TINYINT', 'PK', false, 'Identificador del método.'),
      f('Nombre', 'NVARCHAR(30)', 'UQ', false, 'Efectivo, Tarjeta o Transferencia.')
    ],
    indexes: []
  },
  {
    id: 'categoriasProducto',
    title: 'CategoriasProducto',
    section: 'catalogos',
    subtitle: 'Clasificación del inventario',
    purpose: 'Clasifica los productos para reportes y filtros de inventario.',
    x: 1580,
    y: 1080,
    w: 280,
    fields: [
      f('CategoriaProductoId', 'TINYINT', 'PK', false, 'Identificador de la categoría.'),
      f('Nombre', 'NVARCHAR(40)', 'UQ', false, 'Medicamento, Alimento, Insumo o Higiene.')
    ],
    indexes: []
  },
  {
    id: 'categoriasServicio',
    title: 'CategoriasServicio',
    section: 'catalogos',
    subtitle: 'Agrupación comercial de servicios',
    purpose: 'Agrupa los servicios para el catálogo y los reportes de ingresos.',
    x: 800,
    y: 140,
    w: 260,
    fields: [
      f('CategoriaServicioId', 'TINYINT', 'PK', false, 'Identificador de la categoría.'),
      f('Nombre', 'NVARCHAR(40)', 'UQ', false, 'Consulta, Vacunación, Cirugía, Estética u Hospitalización.')
    ],
    indexes: []
  },

  // ── Personas ───────────────────────────────────────────────────
  {
    id: 'personas',
    title: 'Personas',
    section: 'personas',
    subtitle: 'Datos comunes de clientes y empleados',
    purpose: 'Superclase de clientes y empleados. Evita duplicar los datos personales.',
    x: 70,
    y: 140,
    w: 280,
    fields: [
      f('PersonaId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador único de la persona.', '', 'NEWSEQUENTIALID()'),
      f('Nombre', 'NVARCHAR(80)', '', false, 'Nombre de pila.'),
      f('Apellido', 'NVARCHAR(80)', '', false, 'Apellidos.'),
      f('Documento', 'NVARCHAR(30)', 'UQ', false, 'Documento de identidad; no se repite.'),
      f('Telefono', 'NVARCHAR(25)', '', false, 'Teléfono de contacto.'),
      f('Email', 'NVARCHAR(120)', '', true, 'Correo electrónico; puede ser nulo.'),
      f('TipoPersona', 'CHAR(1)', '', false, 'C = cliente, E = empleado.', '', "'C'", "TipoPersona IN ('C','E')"),
      f('CreadoEn', 'DATETIME2(0)', '', false, 'Fecha de creación.', '', 'SYSDATETIME()'),
      f('ActualizadoEn', 'DATETIME2(0)', '', true, 'Última modificación.')
    ],
    indexes: [
      { name: 'IX_Personas_Apellido_Nombre', columns: ['Apellido', 'Nombre'] },
      { name: 'IX_Personas_Email', columns: ['Email'] }
    ]
  },
  {
    id: 'clientes',
    title: 'Clientes',
    section: 'personas',
    subtitle: 'Extensión de Personas',
    purpose: 'Datos comerciales del dueño de una o varias mascotas.',
    x: 390,
    y: 140,
    w: 260,
    fields: [
      f('ClienteId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Personas.', 'personas'),
      f('Direccion', 'NVARCHAR(200)', '', true, 'Dirección de contacto.'),
      f('FechaRegistro', 'DATE', '', false, 'Fecha de alta como cliente.', '', 'CAST(SYSDATETIME() AS date)'),
      f('PuntosFidelidad', 'INT', '', false, 'Saldo de puntos acumulados.', '', '(0)', 'PuntosFidelidad >= 0')
    ],
    indexes: [{ name: 'IX_Clientes_FechaRegistro', columns: ['FechaRegistro'] }]
  },
  {
    id: 'empleados',
    title: 'Empleados',
    section: 'personas',
    subtitle: 'Extensión de Personas',
    purpose: 'Datos laborales comunes; el cargo específico vive en las tablas de subtipo.',
    x: 390,
    y: 330,
    w: 260,
    fields: [
      f('EmpleadoId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Personas.', 'personas'),
      f('SalarioBase', 'DECIMAL(12,2)', '', false, 'Salario base mensual en córdobas.', '', '', 'SalarioBase > 0'),
      f('FechaIngreso', 'DATE', '', false, 'Fecha de ingreso.'),
      f('Activo', 'BIT', '', false, 'Indica si el empleado sigue laborando.', '', '(1)'),
      f('TipoEmpleado', 'CHAR(1)', '', false, 'V = veterinario, R = recepcionista, A = auxiliar.', '', "'V'", "TipoEmpleado IN ('V','R','A')")
    ],
    indexes: [{ name: 'IX_Empleados_Activo_Tipo', columns: ['Activo', 'TipoEmpleado'] }]
  },
  {
    id: 'veterinarios',
    title: 'Veterinarios',
    section: 'personas',
    subtitle: 'Especialización de Empleados',
    purpose: 'Datos profesionales del veterinario que atiende las citas.',
    x: 70,
    y: 420,
    w: 280,
    fields: [
      f('VeterinarioId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Empleados.', 'empleados'),
      f('Especialidad', 'NVARCHAR(80)', '', false, 'Medicina general, cirugía, etc.'),
      f('NumeroColegiatura', 'NVARCHAR(30)', 'UQ', false, 'Registro profesional único.'),
      f('AniosExperiencia', 'TINYINT', '', false, 'Años de experiencia.', '', '(0)', 'AniosExperiencia BETWEEN 0 AND 70')
    ],
    indexes: []
  },
  {
    id: 'recepcionistas',
    title: 'Recepcionistas',
    section: 'personas',
    subtitle: 'Especialización de Empleados',
    purpose: 'Datos del turno de recepción.',
    x: 390,
    y: 500,
    w: 260,
    fields: [
      f('RecepcionistaId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Empleados.', 'empleados'),
      f('Turno', 'NVARCHAR(20)', '', false, 'Matutino, Vespertino o Nocturno.', '', '', "Turno IN ('Matutino','Vespertino','Nocturno')")
    ],
    indexes: []
  },
  {
    id: 'auxiliares',
    title: 'AuxiliaresVeterinarios',
    section: 'personas',
    subtitle: 'Especialización de Empleados',
    purpose: 'Datos del personal de apoyo clínico.',
    x: 390,
    y: 630,
    w: 260,
    fields: [
      f('AuxiliarId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Empleados.', 'empleados'),
      f('Certificacion', 'NVARCHAR(100)', '', false, 'Certificación o área de apoyo.')
    ],
    indexes: []
  },

  // ── Mascotas ───────────────────────────────────────────────────
  {
    id: 'mascotas',
    title: 'Mascotas',
    section: 'mascotas',
    subtitle: 'Pacientes de la clínica',
    purpose: 'Datos comunes de todos los pacientes; la especie se normaliza en un catálogo.',
    x: 390,
    y: 1040,
    w: 300,
    fields: [
      f('MascotaId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador único del paciente.', '', 'NEWSEQUENTIALID()'),
      f('ClienteId', 'UNIQUEIDENTIFIER', 'FK', false, 'Dueño de la mascota.', 'clientes'),
      f('EspecieId', 'TINYINT', 'FK', false, 'Especie del paciente.', 'especies'),
      f('Nombre', 'NVARCHAR(60)', '', false, 'Nombre de la mascota.'),
      f('Raza', 'NVARCHAR(60)', '', false, 'Raza o “Criollo”.'),
      f('FechaNacimiento', 'DATE', '', false, 'Fecha de nacimiento.'),
      f('PesoKg', 'DECIMAL(5,2)', '', false, 'Peso en kilogramos.', '', '', 'PesoKg > 0 AND PesoKg <= 200'),
      f('Sexo', 'CHAR(1)', '', false, 'M = macho, H = hembra, D = desconocido.', '', "'D'", "Sexo IN ('M','H','D')"),
      f('Alergias', 'NVARCHAR(300)', '', true, 'Alergias conocidas.'),
      f('Notas', 'NVARCHAR(500)', '', true, 'Observaciones clínicas.'),
      f('Esterilizado', 'BIT', '', false, 'Indica si está esterilizado.', '', '(0)'),
      f('UltimaVisita', 'DATETIME2(0)', '', true, 'Última visita registrada.'),
      f('CreadoEn', 'DATETIME2(0)', '', false, 'Fecha de creación.', '', 'SYSDATETIME()'),
      f('ActualizadoEn', 'DATETIME2(0)', '', true, 'Última modificación.')
    ],
    indexes: [
      { name: 'IX_Mascotas_Cliente', columns: ['ClienteId'] },
      { name: 'IX_Mascotas_Especie', columns: ['EspecieId'] },
      { name: 'IX_Mascotas_Nombre', columns: ['Nombre'] }
    ]
  },
  {
    id: 'perros',
    title: 'Perros',
    section: 'mascotas',
    subtitle: 'Especialización de Mascotas',
    purpose: 'Atributos exclusivos de los perros.',
    x: 70,
    y: 1180,
    w: 260,
    fields: [
      f('MascotaId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Mascotas.', 'mascotas'),
      f('Talla', 'NVARCHAR(10)', '', false, 'Pequena, Mediana, Grande o Gigante.', '', "'Mediana'", "Talla IN ('Pequena','Mediana','Grande','Gigante')"),
      f('Entrenado', 'BIT', '', false, 'Indica si tiene entrenamiento.', '', '(0)')
    ],
    indexes: []
  },
  {
    id: 'gatos',
    title: 'Gatos',
    section: 'mascotas',
    subtitle: 'Especialización de Mascotas',
    purpose: 'Atributos exclusivos de los gatos.',
    x: 70,
    y: 1320,
    w: 260,
    fields: [
      f('MascotaId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Mascotas.', 'mascotas'),
      f('EsIndoor', 'BIT', '', false, 'Indica si vive dentro de la casa.', '', '(0)')
    ],
    indexes: []
  },
  {
    id: 'aves',
    title: 'Aves',
    section: 'mascotas',
    subtitle: 'Especialización de Mascotas',
    purpose: 'Atributos exclusivos de las aves.',
    x: 70,
    y: 1440,
    w: 260,
    fields: [
      f('MascotaId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Mascotas.', 'mascotas'),
      f('PuedeVolar', 'BIT', '', false, 'Indica si puede volar.', '', '(1)'),
      f('ColorPlumaje', 'NVARCHAR(40)', '', false, 'Color predominante.', '', "'Verde'")
    ],
    indexes: []
  },
  {
    id: 'conejos',
    title: 'Conejos',
    section: 'mascotas',
    subtitle: 'Especialización de Mascotas',
    purpose: 'Atributos exclusivos de los conejos.',
    x: 70,
    y: 1580,
    w: 260,
    fields: [
      f('MascotaId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Mascotas.', 'mascotas'),
      f('LargoOrejasCm', 'DECIMAL(4,1)', '', false, 'Largo de orejas en centímetros.', '', '(8.0)', 'LargoOrejasCm > 0')
    ],
    indexes: []
  },

  // ── Servicios ──────────────────────────────────────────────────
  {
    id: 'servicios',
    title: 'Servicios',
    section: 'servicios',
    subtitle: 'Catálogo de servicios veterinarios',
    purpose: 'Datos comunes de los servicios; cada subtipo guarda sus atributos y tarifa.',
    x: 1110,
    y: 140,
    w: 300,
    fields: [
      f('ServicioId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador único del servicio.', '', 'NEWSEQUENTIALID()'),
      f('Codigo', 'NVARCHAR(20)', 'UQ', false, 'Código legible, por ejemplo CON-001.'),
      f('Nombre', 'NVARCHAR(80)', '', false, 'Nombre comercial.'),
      f('PrecioBase', 'DECIMAL(12,2)', '', false, 'Precio base en córdobas.', '', '', 'PrecioBase > 0'),
      f('DuracionMinutos', 'SMALLINT', '', false, 'Duración estimada.', '', '(30)', 'DuracionMinutos > 0'),
      f('RequiereAyuno', 'BIT', '', false, 'Indica si requiere ayuno.', '', '(0)'),
      f('Activo', 'BIT', '', false, 'Indica si se puede agendar.', '', '(1)'),
      f('CategoriaServicioId', 'TINYINT', 'FK', false, 'Categoría comercial.', 'categoriasServicio'),
      f('TipoServicio', 'CHAR(1)', '', false, 'C = consulta, V = vacunación, G = cirugía, P = peluquería, H = hospitalización.', '', "'C'", "TipoServicio IN ('C','V','G','P','H')"),
      f('CreadoEn', 'DATETIME2(0)', '', false, 'Fecha de creación.', '', 'SYSDATETIME()'),
      f('ActualizadoEn', 'DATETIME2(0)', '', true, 'Última modificación.')
    ],
    indexes: [
      { name: 'IX_Servicios_Categoria_Activo', columns: ['CategoriaServicioId', 'Activo'] },
      { name: 'IX_Servicios_Nombre', columns: ['Nombre'] }
    ]
  },
  {
    id: 'consultasGenerales',
    title: 'ConsultasGenerales',
    section: 'servicios',
    subtitle: 'Especialización de Servicios',
    purpose: 'Atributos de una consulta general.',
    x: 800,
    y: 400,
    w: 260,
    fields: [
      f('ServicioId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Servicios.', 'servicios'),
      f('IncluyeReceta', 'BIT', '', false, 'Indica si incluye receta.', '', '(1)')
    ],
    indexes: []
  },
  {
    id: 'vacunaciones',
    title: 'Vacunaciones',
    section: 'servicios',
    subtitle: 'Especialización de Servicios',
    purpose: 'Datos de la vacuna y el intervalo de la próxima dosis.',
    x: 800,
    y: 520,
    w: 260,
    fields: [
      f('ServicioId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Servicios.', 'servicios'),
      f('Vacuna', 'NVARCHAR(80)', '', false, 'Nombre de la vacuna.'),
      f('ProximaDosisMeses', 'TINYINT', '', false, 'Meses hasta la próxima dosis.', '', '(12)', 'ProximaDosisMeses >= 1')
    ],
    indexes: []
  },
  {
    id: 'cirugias',
    title: 'Cirugias',
    section: 'servicios',
    subtitle: 'Especialización de Servicios',
    purpose: 'Datos de riesgo y hospitalización de una cirugía.',
    x: 800,
    y: 650,
    w: 260,
    fields: [
      f('ServicioId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Servicios.', 'servicios'),
      f('NivelRiesgo', 'CHAR(1)', '', false, 'B = bajo, M = medio, A = alto.', '', "'M'", "NivelRiesgo IN ('B','M','A')"),
      f('RequiereHospitalizacion', 'BIT', '', false, 'Indica si requiere hospitalización.', '', '(0)')
    ],
    indexes: []
  },
  {
    id: 'peluquerias',
    title: 'Peluquerias',
    section: 'servicios',
    subtitle: 'Especialización de Servicios',
    purpose: 'Datos del corte y del baño.',
    x: 800,
    y: 780,
    w: 260,
    fields: [
      f('ServicioId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Servicios.', 'servicios'),
      f('TipoCorte', 'NVARCHAR(20)', '', false, 'Higienico, Completo, Raza o Creativo.', '', "'Completo'", "TipoCorte IN ('Higienico','Completo','Raza','Creativo')"),
      f('IncluyeBano', 'BIT', '', false, 'Indica si incluye baño.', '', '(1)')
    ],
    indexes: []
  },
  {
    id: 'hospitalizaciones',
    title: 'Hospitalizaciones',
    section: 'servicios',
    subtitle: 'Especialización de Servicios',
    purpose: 'Datos de duración y costo diario de una hospitalización.',
    x: 800,
    y: 910,
    w: 260,
    fields: [
      f('ServicioId', 'UNIQUEIDENTIFIER', 'PK/FK', false, 'Misma clave que Servicios.', 'servicios'),
      f('Dias', 'SMALLINT', '', false, 'Cantidad de días.', '', '(1)', 'Dias >= 1'),
      f('CostoPorDia', 'DECIMAL(12,2)', '', false, 'Costo por día en córdobas.', '', '(850)', 'CostoPorDia > 0')
    ],
    indexes: []
  },

  // ── Operación ──────────────────────────────────────────────────
  {
    id: 'citas',
    title: 'Citas',
    section: 'operacion',
    subtitle: 'Agenda clínica',
    purpose: 'Relaciona cliente, mascota, veterinario, servicio y estado en un horario.',
    x: 1580,
    y: 140,
    w: 320,
    fields: [
      f('CitaId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador único de la cita.', '', 'NEWSEQUENTIALID()'),
      f('ClienteId', 'UNIQUEIDENTIFIER', 'FK', false, 'Cliente que agenda.', 'clientes'),
      f('MascotaId', 'UNIQUEIDENTIFIER', 'FK', false, 'Paciente atendido.', 'mascotas'),
      f('VeterinarioId', 'UNIQUEIDENTIFIER', 'FK', false, 'Veterinario responsable.', 'veterinarios'),
      f('ServicioId', 'UNIQUEIDENTIFIER', 'FK', false, 'Servicio solicitado.', 'servicios'),
      f('EstadoCitaId', 'TINYINT', 'FK', false, 'Estado actual.', 'estadosCita'),
      f('FechaHora', 'DATETIME2(0)', '', false, 'Fecha y hora de la cita.'),
      f('Motivo', 'NVARCHAR(300)', '', false, 'Motivo de la consulta.'),
      f('Diagnostico', 'NVARCHAR(1000)', '', true, 'Diagnóstico registrado al atender.'),
      f('PesoRegistrado', 'DECIMAL(5,2)', '', true, 'Peso tomado durante la atención.', '', '', 'PesoRegistrado IS NULL OR (PesoRegistrado > 0 AND PesoRegistrado <= 200)'),
      f('FechaAtencion', 'DATETIME2(0)', '', true, 'Fecha real de atención.'),
      f('Observaciones', 'NVARCHAR(1000)', '', true, 'Observaciones clínicas.'),
      f('MotivoCancelacion', 'NVARCHAR(300)', '', true, 'Motivo si fue cancelada.'),
      f('CreadoEn', 'DATETIME2(0)', '', false, 'Fecha de creación.', '', 'SYSDATETIME()'),
      f('ActualizadoEn', 'DATETIME2(0)', '', true, 'Última modificación.')
    ],
    indexes: [
      { name: 'IX_Citas_Veterinario_Fecha', columns: ['VeterinarioId', 'FechaHora'] },
      { name: 'IX_Citas_Mascota_Fecha', columns: ['MascotaId', 'FechaHora'] },
      { name: 'IX_Citas_Fecha_Estado', columns: ['FechaHora', 'EstadoCitaId'] },
      { name: 'IX_Citas_Cliente_Fecha', columns: ['ClienteId', 'FechaHora'] }
    ]
  },
  {
    id: 'facturas',
    title: 'Facturas',
    section: 'operacion',
    subtitle: 'Documento de cobro',
    purpose: 'Cabecera de la factura; el total se calcula desde los detalles y no se almacena.',
    x: 1580,
    y: 520,
    w: 300,
    fields: [
      f('FacturaId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador único de la factura.', '', 'NEWSEQUENTIALID()'),
      f('Numero', 'NVARCHAR(20)', 'UQ', false, 'Número legible, por ejemplo HV-2026-0001.'),
      f('ClienteId', 'UNIQUEIDENTIFIER', 'FK', false, 'Cliente que paga.', 'clientes'),
      f('CitaId', 'UNIQUEIDENTIFIER', 'FK', true, 'Cita facturada; puede ser nula en una venta manual.', 'citas'),
      f('Fecha', 'DATETIME2(0)', '', false, 'Fecha de emisión.', '', 'SYSDATETIME()'),
      f('DescuentoPorcentaje', 'DECIMAL(5,2)', '', false, 'Descuento entre 0 y 50 %.', '', '(0)', 'DescuentoPorcentaje BETWEEN 0 AND 50'),
      f('ImpuestoPorcentaje', 'DECIMAL(5,2)', '', false, 'IVA configurado; por defecto 15 %.', '', '(15)', 'ImpuestoPorcentaje BETWEEN 0 AND 100'),
      f('Pagada', 'BIT', '', false, 'Indica si ya se pagó.', '', '(0)'),
      f('MetodoPagoId', 'TINYINT', 'FK', true, 'Método usado al pagar.', 'metodosPago'),
      f('FechaPago', 'DATETIME2(0)', '', true, 'Fecha y hora del pago.'),
      f('CreadoEn', 'DATETIME2(0)', '', false, 'Fecha de creación.', '', 'SYSDATETIME()'),
      f('ActualizadoEn', 'DATETIME2(0)', '', true, 'Última modificación.')
    ],
    checks: [
      {
        name: 'CK_Facturas_Pago_Coherente',
        expr: "(Pagada = 0 AND MetodoPagoId IS NULL AND FechaPago IS NULL) OR (Pagada = 1 AND MetodoPagoId IS NOT NULL AND FechaPago IS NOT NULL)"
      }
    ],
    indexes: [
      { name: 'UX_Facturas_Cita', columns: ['CitaId'], unique: true, filter: 'CitaId IS NOT NULL' },
      { name: 'IX_Facturas_Cliente_Fecha', columns: ['ClienteId', 'Fecha'] },
      { name: 'IX_Facturas_Pagada_Fecha', columns: ['Pagada', 'Fecha'] }
    ]
  },
  {
    id: 'detallesFactura',
    title: 'DetallesFactura',
    section: 'operacion',
    subtitle: 'Líneas de la factura',
    purpose: 'Cada línea conserva la descripción y el precio cobrado, aunque la tarifa cambie después.',
    x: 1580,
    y: 850,
    w: 300,
    fields: [
      f('DetalleFacturaId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador de la línea.', '', 'NEWSEQUENTIALID()'),
      f('FacturaId', 'UNIQUEIDENTIFIER', 'FK', false, 'Factura a la que pertenece.', 'facturas'),
      f('ServicioId', 'UNIQUEIDENTIFIER', 'FK', true, 'Servicio cobrado; nulo si es un concepto manual.', 'servicios'),
      f('Descripcion', 'NVARCHAR(200)', '', false, 'Texto cobrado al cliente.'),
      f('Cantidad', 'INT', '', false, 'Unidades cobradas.', '', '(1)', 'Cantidad > 0'),
      f('PrecioUnitario', 'DECIMAL(12,2)', '', false, 'Precio histórico por unidad.', '', '', 'PrecioUnitario >= 0')
    ],
    indexes: [
      { name: 'IX_DetallesFactura_Factura', columns: ['FacturaId'] },
      { name: 'IX_DetallesFactura_Servicio', columns: ['ServicioId'] }
    ]
  },
  {
    id: 'productos',
    title: 'Productos',
    section: 'operacion',
    subtitle: 'Inventario de la clínica',
    purpose: 'Existencias de medicamentos, alimentos, insumos e higiene.',
    x: 1920,
    y: 1080,
    w: 300,
    fields: [
      f('ProductoId', 'UNIQUEIDENTIFIER', 'PK', false, 'Identificador del producto.', '', 'NEWSEQUENTIALID()'),
      f('CategoriaProductoId', 'TINYINT', 'FK', false, 'Categoría del producto.', 'categoriasProducto'),
      f('Nombre', 'NVARCHAR(100)', '', false, 'Nombre comercial.'),
      f('Precio', 'DECIMAL(12,2)', '', false, 'Precio de venta en córdobas.', '', '', 'Precio > 0'),
      f('Stock', 'INT', '', false, 'Unidades disponibles.', '', '(0)', 'Stock >= 0'),
      f('StockMinimo', 'INT', '', false, 'Umbral de reposición.', '', '(5)', 'StockMinimo >= 0'),
      f('Vencimiento', 'DATE', '', true, 'Fecha de vencimiento, si aplica.'),
      f('CreadoEn', 'DATETIME2(0)', '', false, 'Fecha de creación.', '', 'SYSDATETIME()'),
      f('ActualizadoEn', 'DATETIME2(0)', '', true, 'Última modificación.')
    ],
    indexes: [
      { name: 'IX_Productos_Categoria', columns: ['CategoriaProductoId'] },
      { name: 'IX_Productos_Stock', columns: ['Stock'] }
    ]
  }
];

// Relaciones del modelo lógico/físico.
export const relationships = [
  { from: 'personas', to: 'clientes', kind: 'inherit' },
  { from: 'personas', to: 'empleados', kind: 'inherit' },
  { from: 'empleados', to: 'veterinarios', kind: 'inherit' },
  { from: 'empleados', to: 'recepcionistas', kind: 'inherit' },
  { from: 'empleados', to: 'auxiliares', kind: 'inherit' },
  { from: 'especies', to: 'mascotas', kind: 'one-to-many' },
  { from: 'clientes', to: 'mascotas', kind: 'one-to-many' },
  { from: 'mascotas', to: 'perros', kind: 'inherit' },
  { from: 'mascotas', to: 'gatos', kind: 'inherit' },
  { from: 'mascotas', to: 'aves', kind: 'inherit' },
  { from: 'mascotas', to: 'conejos', kind: 'inherit' },
  { from: 'categoriasServicio', to: 'servicios', kind: 'one-to-many' },
  { from: 'servicios', to: 'consultasGenerales', kind: 'inherit' },
  { from: 'servicios', to: 'vacunaciones', kind: 'inherit' },
  { from: 'servicios', to: 'cirugias', kind: 'inherit' },
  { from: 'servicios', to: 'peluquerias', kind: 'inherit' },
  { from: 'servicios', to: 'hospitalizaciones', kind: 'inherit' },
  { from: 'clientes', to: 'citas', kind: 'one-to-many' },
  { from: 'mascotas', to: 'citas', kind: 'one-to-many' },
  { from: 'veterinarios', to: 'citas', kind: 'one-to-many' },
  { from: 'servicios', to: 'citas', kind: 'one-to-many' },
  { from: 'estadosCita', to: 'citas', kind: 'one-to-many' },
  { from: 'clientes', to: 'facturas', kind: 'one-to-many' },
  { from: 'citas', to: 'facturas', kind: 'zero-to-one' },
  { from: 'metodosPago', to: 'facturas', kind: 'zero-to-many' },
  { from: 'facturas', to: 'detallesFactura', kind: 'one-to-many' },
  { from: 'servicios', to: 'detallesFactura', kind: 'zero-to-many' },
  { from: 'categoriasProducto', to: 'productos', kind: 'one-to-many' }
];

// Entidades del modelo conceptual: menos detalle, más intuición.
export const conceptual = {
  entities: [
    {
      id: 'persona',
      title: 'Persona',
      subtitle: 'Cliente o empleado',
      section: 'personas',
      x: 80,
      y: 110,
      w: 230,
      fields: ['PersonaId (PK)', 'Nombre', 'Documento (UQ)', 'TipoPersona']
    },
    {
      id: 'cliente',
      title: 'Cliente',
      subtitle: 'Dueño de mascotas',
      section: 'personas',
      x: 80,
      y: 340,
      w: 230,
      fields: ['ClienteId (PK/FK)', 'Direccion', 'PuntosFidelidad']
    },
    {
      id: 'empleado',
      title: 'Empleado',
      subtitle: 'Veterinario / Recepcionista / Auxiliar',
      section: 'personas',
      x: 390,
      y: 340,
      w: 260,
      fields: ['EmpleadoId (PK/FK)', 'SalarioBase', 'TipoEmpleado']
    },
    {
      id: 'mascota',
      title: 'Mascota',
      subtitle: 'Perro / Gato / Ave / Conejo',
      section: 'mascotas',
      x: 740,
      y: 110,
      w: 250,
      fields: ['MascotaId (PK)', 'Nombre', 'Especie', 'PesoKg']
    },
    {
      id: 'cita',
      title: 'Cita',
      subtitle: 'Agenda clínica',
      section: 'operacion',
      x: 740,
      y: 390,
      w: 250,
      fields: ['CitaId (PK)', 'FechaHora', 'Estado', 'Diagnostico']
    },
    {
      id: 'servicio',
      title: 'Servicio',
      subtitle: 'Consulta / Vacunación / Cirugía / Peluquería / Hospitalización',
      section: 'servicios',
      x: 1100,
      y: 110,
      w: 270,
      fields: ['ServicioId (PK)', 'Nombre', 'PrecioBase', 'DuracionMinutos']
    },
    {
      id: 'factura',
      title: 'Factura',
      subtitle: 'Documento de cobro',
      section: 'operacion',
      x: 1100,
      y: 390,
      w: 250,
      fields: ['FacturaId (PK)', 'Numero (UQ)', 'Fecha', 'Total (derivado)']
    },
    {
      id: 'detalle',
      title: 'DetalleFactura',
      subtitle: 'Línea de cobro',
      section: 'operacion',
      x: 1430,
      y: 390,
      w: 250,
      fields: ['DetalleId (PK)', 'Descripcion', 'Cantidad', 'PrecioUnitario']
    },
    {
      id: 'producto',
      title: 'Producto',
      subtitle: 'Inventario',
      section: 'catalogos',
      x: 1100,
      y: 780,
      w: 250,
      fields: ['ProductoId (PK)', 'Nombre', 'Stock', 'Precio']
    }
  ],
  relationships: [
    { from: 'cliente', to: 'persona', kind: 'inherit' },
    { from: 'empleado', to: 'persona', kind: 'inherit' },
    {
      from: 'cliente',
      to: 'mascota',
      kind: 'one-to-many',
      waypoints: [[360, 392], [360, 300], [865, 300]],
      style: 'exitX=1;exitY=0.5;entryX=0.5;entryY=1;'
    },
    {
      from: 'mascota',
      to: 'cita',
      kind: 'one-to-many',
      style: 'exitX=0.5;exitY=1;entryX=0.35;entryY=0;'
    },
    {
      from: 'empleado',
      to: 'cita',
      kind: 'one-to-many',
      style: 'exitX=1;exitY=0.5;entryX=0;entryY=0.5;'
    },
    {
      from: 'servicio',
      to: 'cita',
      kind: 'one-to-many',
      waypoints: [[1235, 232], [1235, 300], [902, 300]],
      style: 'exitX=0.5;exitY=1;entryX=0.65;entryY=0;'
    },
    {
      from: 'cita',
      to: 'factura',
      kind: 'zero-to-one',
      style: 'exitX=1;exitY=0.5;entryX=0;entryY=0.5;'
    },
    {
      from: 'factura',
      to: 'detalle',
      kind: 'one-to-many',
      style: 'exitX=1;exitY=0.5;entryX=0;entryY=0.5;'
    },
    {
      from: 'servicio',
      to: 'detalle',
      kind: 'zero-to-many',
      waypoints: [[1430, 171], [1430, 330], [1555, 330]],
      style: 'exitX=1;exitY=0.5;entryX=0.5;entryY=0;'
    }
  ]
};

export const catalogSeeds = {
  especies: [
    [1, 'Perro'],
    [2, 'Gato'],
    [3, 'Ave'],
    [4, 'Conejo']
  ],
  estadosCita: [
    [1, 'Programada'],
    [2, 'Confirmada'],
    [3, 'EnAtencion'],
    [4, 'Atendida'],
    [5, 'Cancelada'],
    [6, 'NoAsistio']
  ],
  metodosPago: [
    [1, 'Efectivo'],
    [2, 'Tarjeta'],
    [3, 'Transferencia']
  ],
  categoriasProducto: [
    [1, 'Medicamento'],
    [2, 'Alimento'],
    [3, 'Insumo'],
    [4, 'Higiene']
  ],
  categoriasServicio: [
    [1, 'Consulta'],
    [2, 'Vacunación'],
    [3, 'Cirugía'],
    [4, 'Estética'],
    [5, 'Hospitalización']
  ]
};

export const createOrder = [
  'especies',
  'estadosCita',
  'metodosPago',
  'categoriasProducto',
  'categoriasServicio',
  'personas',
  'clientes',
  'empleados',
  'veterinarios',
  'recepcionistas',
  'auxiliares',
  'mascotas',
  'perros',
  'gatos',
  'aves',
  'conejos',
  'servicios',
  'consultasGenerales',
  'vacunaciones',
  'cirugias',
  'peluquerias',
  'hospitalizaciones',
  'citas',
  'facturas',
  'detallesFactura',
  'productos'
];

export const tableById = Object.fromEntries(tables.map(t => [t.id, t]));
