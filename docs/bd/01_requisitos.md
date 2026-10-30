# Fase 1 · Requisitos y análisis de la base de datos

## 1. Objetivo

Diseñar la base de datos relacional que respalde la operación de la clínica veterinaria HuellaVet. Esta base de datos es el modelo objetivo de persistencia; el avance actual de la aplicación sigue usando JSON local, pero el esquema queda listo para migrarse a SQL Server en la Unidad 3.

## 2. Alcance

### Incluye

- Clientes y sus mascotas.
- Expediente clínico básico (diagnóstico, peso, observaciones).
- Personal de la clínica y sus especializaciones.
- Catálogo de servicios veterinarios.
- Agenda de citas y estados.
- Facturación, descuentos, impuestos y puntos de fidelidad.
- Inventario de productos.
- Catálogos de apoyo (especies, estados, métodos de pago, categorías).

### No incluye en esta fase

- Autenticación, roles y permisos.
- Nómina contable o bancaria.
- Compras, proveedores y órdenes de compra.
- Historial de movimientos de inventario.
- Sucursales o multi-tenant.
- Integración ADO.NET/Entity Framework con la aplicación C#.

## 3. Actores

| Actor | Responsabilidad principal |
|---|---|
| Recepcionista | Registra clientes, mascotas, agenda citas y cobra facturas. |
| Veterinario | Atiende citas, registra diagnóstico y peso. |
| Auxiliar veterinario | Apoya en hospitalización y laboratorio. |
| Administrador | Configura servicios, personal, inventario y reportes. |
| Cliente | Es el dueño de una o varias mascotas; no opera la base directamente. |

## 4. Requisitos funcionales

| Código | Requisito |
|---|---|
| RF-01 | Registrar clientes con documento único. |
| RF-02 | Registrar mascotas asociadas a un cliente. |
| RF-03 | Registrar empleados y su especialización (veterinario, recepcionista o auxiliar). |
| RF-04 | Mantener un catálogo de servicios con precio base, duración y categoría. |
| RF-05 | Programar citas para un cliente, mascota, veterinario y servicio. |
| RF-06 | Consultar citas por fecha, cliente, mascota y veterinario. |
| RF-07 | Cambiar el estado de la cita (programada, confirmada, en atención, atendida, cancelada, no asistió). |
| RF-08 | Atender una cita y registrar diagnóstico, peso y observaciones. |
| RF-09 | Generar una factura a partir de una cita atendida. |
| RF-10 | Cobrar servicios con precio histórico y descuentos opcionales. |
| RF-11 | Registrar el método de pago y la fecha de pago. |
| RF-12 | Acumular puntos de fidelidad al cliente. |
| RF-13 | Consultar inventario, stock mínimo y productos vencidos. |
| RF-14 | Generar reportes de agenda, facturación, vacunas próximas y stock bajo. |

## 5. Reglas de negocio

| Código | Regla | Aplicación |
|---|---|---|
| RN-01 | Un cliente puede tener una o varias mascotas. | Base de datos + aplicación. |
| RN-02 | Una mascota pertenece a un solo cliente. | Base de datos. |
| RN-03 | La especie de la mascota se normaliza en un catálogo. | Base de datos. |
| RN-04 | Una persona es cliente o empleado, no ambos. | Aplicación + `TipoPersona`. |
| RN-05 | Un empleado es de un solo cargo especializado. | Aplicación + subtablas. |
| RN-06 | Un servicio es de un solo subtipo. | Aplicación + subtablas. |
| RN-07 | La fecha de la cita debe ser futura. | Aplicación. |
| RN-08 | El veterinario y el servicio deben estar activos. | Aplicación. |
| RN-09 | La mascota debe pertenecer al cliente que agenda. | Aplicación. |
| RN-10 | No puede haber dos citas activas del mismo veterinario con menos de 30 minutos de diferencia. | Aplicación + índice. |
| RN-11 | No puede haber dos citas activas de la misma mascota con menos de 30 minutos de diferencia. | Aplicación + índice. |
| RN-12 | Solo una cita atendida puede facturarse. | Aplicación. |
| RN-13 | Una cita tiene como máximo una factura. | Base de datos (`UX_Facturas_Cita`). |
| RN-14 | El número de factura es único. | Base de datos. |
| RN-15 | El descuento está entre 0 y 50 %. | Base de datos + aplicación. |
| RN-16 | El impuesto está entre 0 y 100 %. | Base de datos + aplicación. |
| RN-17 | Una factura pagada exige método y fecha de pago. | Base de datos + aplicación. |
| RN-18 | No se pueden modificar las líneas de una factura pagada. | Aplicación. |
| RN-19 | El precio de la línea se guarda como histórico. | Base de datos + aplicación. |
| RN-20 | El stock no puede ser negativo. | Base de datos + aplicación. |
| RN-21 | Un producto con stock menor o igual al mínimo se considera bajo. | Vista/aplicación. |
| RN-22 | Se acumula un punto por cada C$ 100 facturados. | Aplicación. |
| RN-23 | El documento, el código del servicio, el número de colegiatura y el número de factura son únicos. | Base de datos. |
| RN-24 | La fecha de nacimiento de una mascota no puede ser futura. | Aplicación. |

## 6. Entidades y relaciones

| Grupo | Tablas | Relación clave |
|---|---|---|
| Personas | `Personas`, `Clientes`, `Empleados`, `Veterinarios`, `Recepcionistas`, `AuxiliaresVeterinarios` | `Personas` 1:0..1 `Clientes`; `Personas` 1:0..1 `Empleados`; `Empleados` 1:0..1 subtipo. |
| Pacientes | `Especies`, `Mascotas`, `Perros`, `Gatos`, `Aves`, `Conejos` | `Especies` 1:N `Mascotas`; `Clientes` 1:N `Mascotas`; `Mascotas` 1:0..1 subtipo. |
| Servicios | `CategoriasServicio`, `Servicios`, `ConsultasGenerales`, `Vacunaciones`, `Cirugias`, `Peluquerias`, `Hospitalizaciones` | `CategoriasServicio` 1:N `Servicios`; `Servicios` 1:0..1 subtipo. |
| Operación | `EstadosCita`, `Citas`, `MetodosPago`, `Facturas`, `DetallesFactura`, `CategoriasProducto`, `Productos` | `Clientes` 1:N `Citas`; `Mascotas` 1:N `Citas`; `Veterinarios` 1:N `Citas`; `Servicios` 1:N `Citas`; `Citas` 0..1:1 `Facturas`; `Facturas` 1:N `DetallesFactura`. |

## 7. Requisitos no funcionales

| Código | Requisito | Cómo se cumple |
|---|---|---|
| RNF-01 | Integridad referencial | Claves foráneas entre todas las tablas relacionadas. |
| RNF-02 | Validación de dominio | `CHECK` para enumeraciones y rangos. |
| RNF-03 | Rendimiento | Índices por fecha, cliente, mascota y veterinario. |
| RNF-04 | Consistencia de pagos | `CK_Facturas_Pago_Coherente`. |
| RNF-05 | Trazabilidad | `CreadoEn` y `ActualizadoEn` en tablas mutables. |
| RNF-06 | Portabilidad | SQL Server 2022, sin características exclusivas de una edición. |
| RNF-07 | Mantenibilidad | Vistas para reportes y totales. |
| RNF-08 | Seguridad futura | Preparado para roles y RowVersion sin cambiar la estructura base. |

## 8. Volumen estimado

| Entidad | Estimado inicial | Estimado a 5 años |
|---|---:|---:|
| Clientes | 1 000 | 5 000 |
| Mascotas | 1 500 | 8 000 |
| Empleados | 20 | 50 |
| Servicios | 25 | 60 |
| Citas | 3 000/año | 15 000/año |
| Facturas | 2 500/año | 12 000/año |
| Productos | 120 | 300 |

Estos valores son académicos y sirven para justificar los índices por fecha y por cliente.

## 9. Supuestos

- Una sola clínica y una sola moneda (córdobas).
- Un veterinario por cita.
- Una factura por cita, aunque también se permiten ventas manuales sin cita.
- No se modela nómina contable ni compras.
- No se registra historial de movimientos de inventario; solo existencias actuales.
- El huso horario de referencia es `America/Managua`.

## 10. Trazabilidad con el dominio C#

| Clase C# | Tabla o tablas |
|---|---|
| `Persona` | `Personas` |
| `Cliente` | `Clientes` |
| `Empleado` | `Empleados` |
| `Veterinario` | `Veterinarios` |
| `Recepcionista` | `Recepcionistas` |
| `AuxiliarVeterinario` | `AuxiliaresVeterinarios` |
| `Mascota` | `Mascotas` |
| `Perro`, `Gato`, `Ave`, `Conejo` | `Perros`, `Gatos`, `Aves`, `Conejos` |
| `ServicioVeterinario` | `Servicios` |
| `ConsultaGeneral`, `Vacunacion`, `Cirugia`, `Peluqueria`, `Hospitalizacion` | `ConsultasGenerales`, `Vacunaciones`, `Cirugias`, `Peluquerias`, `Hospitalizaciones` |
| `Cita` | `Citas` |
| `Factura` | `Facturas` |
| `DetalleFactura` | `DetallesFactura` |
| `Producto` | `Productos` |
| `SexoMascota`, `NivelRiesgo`, `TipoCorte`, `EstadoCita`, `MetodoPago`, `CategoriaProducto` | Catálogos o restricciones `CHECK` |
| `ClinicaData` | Transacción de aplicación, no una tabla |

## 11. Criterios de aceptación

- El modelo cubre todas las operaciones del dominio C#.
- Las 26 tablas están normalizadas al menos en 3NF.
- El script se ejecuta completo en SQL Server 2022.
- Los diagramas se abren en draw.io sin errores.
- El diccionario describe cada tabla y columna.
- La documentación enlaza cada fase con su evidencia.
