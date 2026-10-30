# Diccionario de datos · HuellaVet

Generado desde `modelo-bd.mjs` con `node docs/bd/generar_diagramas.mjs`.

**DBMS:** SQL Server 2022 · **Base:** `HuellaVet` · **Esquema:** `dbo`

Convenciones: `PK` = clave primaria, `FK` = clave foránea, `PK/FK` = clave compartida con la tabla base, `UQ` = restricción única. La columna **Nulo** indica si se permite `NULL`.

---

## Especies

Normaliza la especie de cada mascota y evita repetir el texto en cada fila.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **EspecieId** | `TINYINT` | No | PK |  |  | Identificador de la especie. |
| **Nombre** | `NVARCHAR(30)` | No | UQ |  |  | Perro, Gato, Ave o Conejo. |

---

## EstadosCita

Catálogo de estados para conservar la integridad del ciclo de la cita.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **EstadoCitaId** | `TINYINT` | No | PK |  |  | Identificador del estado. |
| **Nombre** | `NVARCHAR(20)` | No | UQ |  |  | Programada, Confirmada, EnAtencion, Atendida, Cancelada o NoAsistio. |

---

## MetodosPago

Catálogo de métodos de pago; la factura solo lo usa cuando está pagada.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **MetodoPagoId** | `TINYINT` | No | PK |  |  | Identificador del método. |
| **Nombre** | `NVARCHAR(30)` | No | UQ |  |  | Efectivo, Tarjeta o Transferencia. |

---

## CategoriasProducto

Clasifica los productos para reportes y filtros de inventario.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **CategoriaProductoId** | `TINYINT` | No | PK |  |  | Identificador de la categoría. |
| **Nombre** | `NVARCHAR(40)` | No | UQ |  |  | Medicamento, Alimento, Insumo o Higiene. |

---

## CategoriasServicio

Agrupa los servicios para el catálogo y los reportes de ingresos.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **CategoriaServicioId** | `TINYINT` | No | PK |  |  | Identificador de la categoría. |
| **Nombre** | `NVARCHAR(40)` | No | UQ |  |  | Consulta, Vacunación, Cirugía, Estética u Hospitalización. |

---

## Personas

Superclase de clientes y empleados. Evita duplicar los datos personales.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **PersonaId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador único de la persona. |
| **Nombre** | `NVARCHAR(80)` | No |  |  |  | Nombre de pila. |
| **Apellido** | `NVARCHAR(80)` | No |  |  |  | Apellidos. |
| **Documento** | `NVARCHAR(30)` | No | UQ |  |  | Documento de identidad; no se repite. |
| **Telefono** | `NVARCHAR(25)` | No |  |  |  | Teléfono de contacto. |
| **Email** | `NVARCHAR(120)` | Sí |  |  |  | Correo electrónico; puede ser nulo. |
| **TipoPersona** | `CHAR(1)` | No |  |  | DEFAULT 'C' · TipoPersona IN ('C','E') | C = cliente, E = empleado. |
| **CreadoEn** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de creación. |
| **ActualizadoEn** | `DATETIME2(0)` | Sí |  |  |  | Última modificación. |

**Índices:**

- `IX_Personas_Apellido_Nombre`: (Apellido, Nombre)
- `IX_Personas_Email`: (Email)

---

## Clientes

Datos comerciales del dueño de una o varias mascotas.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ClienteId** | `UNIQUEIDENTIFIER` | No | PK/FK | Personas(PersonaId) |  | Misma clave que Personas. |
| **Direccion** | `NVARCHAR(200)` | Sí |  |  |  | Dirección de contacto. |
| **FechaRegistro** | `DATE` | No |  |  | DEFAULT CAST(SYSDATETIME() AS date) | Fecha de alta como cliente. |
| **PuntosFidelidad** | `INT` | No |  |  | DEFAULT (0) · PuntosFidelidad >= 0 | Saldo de puntos acumulados. |

**Índices:**

- `IX_Clientes_FechaRegistro`: (FechaRegistro)

---

## Empleados

Datos laborales comunes; el cargo específico vive en las tablas de subtipo.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **EmpleadoId** | `UNIQUEIDENTIFIER` | No | PK/FK | Personas(PersonaId) |  | Misma clave que Personas. |
| **SalarioBase** | `DECIMAL(12,2)` | No |  |  | SalarioBase > 0 | Salario base mensual en córdobas. |
| **FechaIngreso** | `DATE` | No |  |  |  | Fecha de ingreso. |
| **Activo** | `BIT` | No |  |  | DEFAULT (1) | Indica si el empleado sigue laborando. |
| **TipoEmpleado** | `CHAR(1)` | No |  |  | DEFAULT 'V' · TipoEmpleado IN ('V','R','A') | V = veterinario, R = recepcionista, A = auxiliar. |

**Índices:**

- `IX_Empleados_Activo_Tipo`: (Activo, TipoEmpleado)

---

## Veterinarios

Datos profesionales del veterinario que atiende las citas.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **VeterinarioId** | `UNIQUEIDENTIFIER` | No | PK/FK | Empleados(EmpleadoId) |  | Misma clave que Empleados. |
| **Especialidad** | `NVARCHAR(80)` | No |  |  |  | Medicina general, cirugía, etc. |
| **NumeroColegiatura** | `NVARCHAR(30)` | No | UQ |  |  | Registro profesional único. |
| **AniosExperiencia** | `TINYINT` | No |  |  | DEFAULT (0) · AniosExperiencia BETWEEN 0 AND 70 | Años de experiencia. |

---

## Recepcionistas

Datos del turno de recepción.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **RecepcionistaId** | `UNIQUEIDENTIFIER` | No | PK/FK | Empleados(EmpleadoId) |  | Misma clave que Empleados. |
| **Turno** | `NVARCHAR(20)` | No |  |  | Turno IN ('Matutino','Vespertino','Nocturno') | Matutino, Vespertino o Nocturno. |

---

## AuxiliaresVeterinarios

Datos del personal de apoyo clínico.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **AuxiliarId** | `UNIQUEIDENTIFIER` | No | PK/FK | Empleados(EmpleadoId) |  | Misma clave que Empleados. |
| **Certificacion** | `NVARCHAR(100)` | No |  |  |  | Certificación o área de apoyo. |

---

## Mascotas

Datos comunes de todos los pacientes; la especie se normaliza en un catálogo.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **MascotaId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador único del paciente. |
| **ClienteId** | `UNIQUEIDENTIFIER` | No | FK | Clientes(ClienteId) |  | Dueño de la mascota. |
| **EspecieId** | `TINYINT` | No | FK | Especies(EspecieId) |  | Especie del paciente. |
| **Nombre** | `NVARCHAR(60)` | No |  |  |  | Nombre de la mascota. |
| **Raza** | `NVARCHAR(60)` | No |  |  |  | Raza o “Criollo”. |
| **FechaNacimiento** | `DATE` | No |  |  |  | Fecha de nacimiento. |
| **PesoKg** | `DECIMAL(5,2)` | No |  |  | PesoKg > 0 AND PesoKg <= 200 | Peso en kilogramos. |
| **Sexo** | `CHAR(1)` | No |  |  | DEFAULT 'D' · Sexo IN ('M','H','D') | M = macho, H = hembra, D = desconocido. |
| **Alergias** | `NVARCHAR(300)` | Sí |  |  |  | Alergias conocidas. |
| **Notas** | `NVARCHAR(500)` | Sí |  |  |  | Observaciones clínicas. |
| **Esterilizado** | `BIT` | No |  |  | DEFAULT (0) | Indica si está esterilizado. |
| **UltimaVisita** | `DATETIME2(0)` | Sí |  |  |  | Última visita registrada. |
| **CreadoEn** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de creación. |
| **ActualizadoEn** | `DATETIME2(0)` | Sí |  |  |  | Última modificación. |

**Índices:**

- `IX_Mascotas_Cliente`: (ClienteId)
- `IX_Mascotas_Especie`: (EspecieId)
- `IX_Mascotas_Nombre`: (Nombre)

---

## Perros

Atributos exclusivos de los perros.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **MascotaId** | `UNIQUEIDENTIFIER` | No | PK/FK | Mascotas(MascotaId) |  | Misma clave que Mascotas. |
| **Talla** | `NVARCHAR(10)` | No |  |  | DEFAULT 'Mediana' · Talla IN ('Pequena','Mediana','Grande','Gigante') | Pequena, Mediana, Grande o Gigante. |
| **Entrenado** | `BIT` | No |  |  | DEFAULT (0) | Indica si tiene entrenamiento. |

---

## Gatos

Atributos exclusivos de los gatos.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **MascotaId** | `UNIQUEIDENTIFIER` | No | PK/FK | Mascotas(MascotaId) |  | Misma clave que Mascotas. |
| **EsIndoor** | `BIT` | No |  |  | DEFAULT (0) | Indica si vive dentro de la casa. |

---

## Aves

Atributos exclusivos de las aves.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **MascotaId** | `UNIQUEIDENTIFIER` | No | PK/FK | Mascotas(MascotaId) |  | Misma clave que Mascotas. |
| **PuedeVolar** | `BIT` | No |  |  | DEFAULT (1) | Indica si puede volar. |
| **ColorPlumaje** | `NVARCHAR(40)` | No |  |  | DEFAULT 'Verde' | Color predominante. |

---

## Conejos

Atributos exclusivos de los conejos.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **MascotaId** | `UNIQUEIDENTIFIER` | No | PK/FK | Mascotas(MascotaId) |  | Misma clave que Mascotas. |
| **LargoOrejasCm** | `DECIMAL(4,1)` | No |  |  | DEFAULT (8.0) · LargoOrejasCm > 0 | Largo de orejas en centímetros. |

---

## Servicios

Datos comunes de los servicios; cada subtipo guarda sus atributos y tarifa.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ServicioId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador único del servicio. |
| **Codigo** | `NVARCHAR(20)` | No | UQ |  |  | Código legible, por ejemplo CON-001. |
| **Nombre** | `NVARCHAR(80)` | No |  |  |  | Nombre comercial. |
| **PrecioBase** | `DECIMAL(12,2)` | No |  |  | PrecioBase > 0 | Precio base en córdobas. |
| **DuracionMinutos** | `SMALLINT` | No |  |  | DEFAULT (30) · DuracionMinutos > 0 | Duración estimada. |
| **RequiereAyuno** | `BIT` | No |  |  | DEFAULT (0) | Indica si requiere ayuno. |
| **Activo** | `BIT` | No |  |  | DEFAULT (1) | Indica si se puede agendar. |
| **CategoriaServicioId** | `TINYINT` | No | FK | CategoriasServicio(CategoriaServicioId) |  | Categoría comercial. |
| **TipoServicio** | `CHAR(1)` | No |  |  | DEFAULT 'C' · TipoServicio IN ('C','V','G','P','H') | C = consulta, V = vacunación, G = cirugía, P = peluquería, H = hospitalización. |
| **CreadoEn** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de creación. |
| **ActualizadoEn** | `DATETIME2(0)` | Sí |  |  |  | Última modificación. |

**Índices:**

- `IX_Servicios_Categoria_Activo`: (CategoriaServicioId, Activo)
- `IX_Servicios_Nombre`: (Nombre)

---

## ConsultasGenerales

Atributos de una consulta general.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ServicioId** | `UNIQUEIDENTIFIER` | No | PK/FK | Servicios(ServicioId) |  | Misma clave que Servicios. |
| **IncluyeReceta** | `BIT` | No |  |  | DEFAULT (1) | Indica si incluye receta. |

---

## Vacunaciones

Datos de la vacuna y el intervalo de la próxima dosis.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ServicioId** | `UNIQUEIDENTIFIER` | No | PK/FK | Servicios(ServicioId) |  | Misma clave que Servicios. |
| **Vacuna** | `NVARCHAR(80)` | No |  |  |  | Nombre de la vacuna. |
| **ProximaDosisMeses** | `TINYINT` | No |  |  | DEFAULT (12) · ProximaDosisMeses >= 1 | Meses hasta la próxima dosis. |

---

## Cirugias

Datos de riesgo y hospitalización de una cirugía.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ServicioId** | `UNIQUEIDENTIFIER` | No | PK/FK | Servicios(ServicioId) |  | Misma clave que Servicios. |
| **NivelRiesgo** | `CHAR(1)` | No |  |  | DEFAULT 'M' · NivelRiesgo IN ('B','M','A') | B = bajo, M = medio, A = alto. |
| **RequiereHospitalizacion** | `BIT` | No |  |  | DEFAULT (0) | Indica si requiere hospitalización. |

---

## Peluquerias

Datos del corte y del baño.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ServicioId** | `UNIQUEIDENTIFIER` | No | PK/FK | Servicios(ServicioId) |  | Misma clave que Servicios. |
| **TipoCorte** | `NVARCHAR(20)` | No |  |  | DEFAULT 'Completo' · TipoCorte IN ('Higienico','Completo','Raza','Creativo') | Higienico, Completo, Raza o Creativo. |
| **IncluyeBano** | `BIT` | No |  |  | DEFAULT (1) | Indica si incluye baño. |

---

## Hospitalizaciones

Datos de duración y costo diario de una hospitalización.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ServicioId** | `UNIQUEIDENTIFIER` | No | PK/FK | Servicios(ServicioId) |  | Misma clave que Servicios. |
| **Dias** | `SMALLINT` | No |  |  | DEFAULT (1) · Dias >= 1 | Cantidad de días. |
| **CostoPorDia** | `DECIMAL(12,2)` | No |  |  | DEFAULT (850) · CostoPorDia > 0 | Costo por día en córdobas. |

---

## Citas

Relaciona cliente, mascota, veterinario, servicio y estado en un horario.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **CitaId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador único de la cita. |
| **ClienteId** | `UNIQUEIDENTIFIER` | No | FK | Clientes(ClienteId) |  | Cliente que agenda. |
| **MascotaId** | `UNIQUEIDENTIFIER` | No | FK | Mascotas(MascotaId) |  | Paciente atendido. |
| **VeterinarioId** | `UNIQUEIDENTIFIER` | No | FK | Veterinarios(VeterinarioId) |  | Veterinario responsable. |
| **ServicioId** | `UNIQUEIDENTIFIER` | No | FK | Servicios(ServicioId) |  | Servicio solicitado. |
| **EstadoCitaId** | `TINYINT` | No | FK | EstadosCita(EstadoCitaId) |  | Estado actual. |
| **FechaHora** | `DATETIME2(0)` | No |  |  |  | Fecha y hora de la cita. |
| **Motivo** | `NVARCHAR(300)` | No |  |  |  | Motivo de la consulta. |
| **Diagnostico** | `NVARCHAR(1000)` | Sí |  |  |  | Diagnóstico registrado al atender. |
| **PesoRegistrado** | `DECIMAL(5,2)` | Sí |  |  | PesoRegistrado IS NULL OR (PesoRegistrado > 0 AND PesoRegistrado <= 200) | Peso tomado durante la atención. |
| **FechaAtencion** | `DATETIME2(0)` | Sí |  |  |  | Fecha real de atención. |
| **Observaciones** | `NVARCHAR(1000)` | Sí |  |  |  | Observaciones clínicas. |
| **MotivoCancelacion** | `NVARCHAR(300)` | Sí |  |  |  | Motivo si fue cancelada. |
| **CreadoEn** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de creación. |
| **ActualizadoEn** | `DATETIME2(0)` | Sí |  |  |  | Última modificación. |

**Índices:**

- `IX_Citas_Veterinario_Fecha`: (VeterinarioId, FechaHora)
- `IX_Citas_Mascota_Fecha`: (MascotaId, FechaHora)
- `IX_Citas_Fecha_Estado`: (FechaHora, EstadoCitaId)
- `IX_Citas_Cliente_Fecha`: (ClienteId, FechaHora)

---

## Facturas

Cabecera de la factura; el total se calcula desde los detalles y no se almacena.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **FacturaId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador único de la factura. |
| **Numero** | `NVARCHAR(20)` | No | UQ |  |  | Número legible, por ejemplo HV-2026-0001. |
| **ClienteId** | `UNIQUEIDENTIFIER` | No | FK | Clientes(ClienteId) |  | Cliente que paga. |
| **CitaId** | `UNIQUEIDENTIFIER` | Sí | FK | Citas(CitaId) |  | Cita facturada; puede ser nula en una venta manual. |
| **Fecha** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de emisión. |
| **DescuentoPorcentaje** | `DECIMAL(5,2)` | No |  |  | DEFAULT (0) · DescuentoPorcentaje BETWEEN 0 AND 50 | Descuento entre 0 y 50 %. |
| **ImpuestoPorcentaje** | `DECIMAL(5,2)` | No |  |  | DEFAULT (15) · ImpuestoPorcentaje BETWEEN 0 AND 100 | IVA configurado; por defecto 15 %. |
| **Pagada** | `BIT` | No |  |  | DEFAULT (0) | Indica si ya se pagó. |
| **MetodoPagoId** | `TINYINT` | Sí | FK | MetodosPago(MetodoPagoId) |  | Método usado al pagar. |
| **FechaPago** | `DATETIME2(0)` | Sí |  |  |  | Fecha y hora del pago. |
| **CreadoEn** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de creación. |
| **ActualizadoEn** | `DATETIME2(0)` | Sí |  |  |  | Última modificación. |

**Restricciones de tabla:**

- `CK_Facturas_Pago_Coherente`: `(Pagada = 0 AND MetodoPagoId IS NULL AND FechaPago IS NULL) OR (Pagada = 1 AND MetodoPagoId IS NOT NULL AND FechaPago IS NOT NULL)`

**Índices:**

- `UX_Facturas_Cita`: UNIQUE (CitaId) WHERE CitaId IS NOT NULL
- `IX_Facturas_Cliente_Fecha`: (ClienteId, Fecha)
- `IX_Facturas_Pagada_Fecha`: (Pagada, Fecha)

---

## DetallesFactura

Cada línea conserva la descripción y el precio cobrado, aunque la tarifa cambie después.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **DetalleFacturaId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador de la línea. |
| **FacturaId** | `UNIQUEIDENTIFIER` | No | FK | Facturas(FacturaId) |  | Factura a la que pertenece. |
| **ServicioId** | `UNIQUEIDENTIFIER` | Sí | FK | Servicios(ServicioId) |  | Servicio cobrado; nulo si es un concepto manual. |
| **Descripcion** | `NVARCHAR(200)` | No |  |  |  | Texto cobrado al cliente. |
| **Cantidad** | `INT` | No |  |  | DEFAULT (1) · Cantidad > 0 | Unidades cobradas. |
| **PrecioUnitario** | `DECIMAL(12,2)` | No |  |  | PrecioUnitario >= 0 | Precio histórico por unidad. |

**Índices:**

- `IX_DetallesFactura_Factura`: (FacturaId)
- `IX_DetallesFactura_Servicio`: (ServicioId)

---

## Productos

Existencias de medicamentos, alimentos, insumos e higiene.

| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |
|---|---|---|---|---|---|---|
| **ProductoId** | `UNIQUEIDENTIFIER` | No | PK |  | DEFAULT NEWSEQUENTIALID() | Identificador del producto. |
| **CategoriaProductoId** | `TINYINT` | No | FK | CategoriasProducto(CategoriaProductoId) |  | Categoría del producto. |
| **Nombre** | `NVARCHAR(100)` | No |  |  |  | Nombre comercial. |
| **Precio** | `DECIMAL(12,2)` | No |  |  | Precio > 0 | Precio de venta en córdobas. |
| **Stock** | `INT` | No |  |  | DEFAULT (0) · Stock >= 0 | Unidades disponibles. |
| **StockMinimo** | `INT` | No |  |  | DEFAULT (5) · StockMinimo >= 0 | Umbral de reposición. |
| **Vencimiento** | `DATE` | Sí |  |  |  | Fecha de vencimiento, si aplica. |
| **CreadoEn** | `DATETIME2(0)` | No |  |  | DEFAULT SYSDATETIME() | Fecha de creación. |
| **ActualizadoEn** | `DATETIME2(0)` | Sí |  |  |  | Última modificación. |

**Índices:**

- `IX_Productos_Categoria`: (CategoriaProductoId)
- `IX_Productos_Stock`: (Stock)

---

## Resumen

- Tablas: **26**
- Columnas: **127**
- Diagramas: `02_modelo_conceptual.drawio`, `03_modelo_logico.drawio`, `05_modelo_fisico.drawio`
- Script: `06_huellavet_sqlserver.sql`
