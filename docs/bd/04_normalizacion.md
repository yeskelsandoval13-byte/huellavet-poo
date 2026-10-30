# Fase 4 · Normalización

## 1. Objetivo

Demostrar que el modelo lógico de HuellaVet se encuentra en **primera, segunda y tercera forma normal (1NF, 2NF y 3NF)**, y documentar las pocas redundancias controladas que se mantienen por motivos de negocio.

## 2. Primera forma normal (1NF)

Una tabla está en 1NF cuando todos sus atributos son atómicos, no hay grupos repetitivos y cada fila es única.

- No existen columnas como `Telefonos`, `Alergias1`, `Alergias2`.
- Las líneas de factura están en `DetallesFactura`, no como columnas repetidas en `Facturas`.
- Los subtipos de mascota, empleado y servicio están en tablas separadas.
- Cada tabla tiene clave primaria.

**Ejemplo antes de normalizar:**

| Factura | Cliente | Detalle1 | Detalle2 | Detalle3 |
|---|---|---|---|---|
| F-001 | Ana | Consulta x1 | Vacuna x2 | — |

**Ejemplo normalizado:**

| FacturaId | ClienteId | Descripción | Cantidad |
|---|---|---|---:|
| F-001 | C-001 | Consulta | 1 |
| F-001 | C-001 | Vacuna | 2 |

## 3. Segunda forma normal (2NF)

Una tabla está en 2NF cuando está en 1NF y todo atributo no clave depende de la clave primaria completa. El caso típico son las tablas con clave compuesta.

- `DetallesFactura` usa una clave sustituta (`DetalleFacturaId`), por lo que cada atributo depende de la línea completa y no solo de `FacturaId`.
- Las tablas de subtipo (`Perros`, `Gatos`, `Aves`, `Conejos`, `Veterinarios`, etc.) tienen como clave primaria la clave de la tabla base; sus atributos dependen de esa clave completa.
- `Citas` no almacena nombre del cliente, nombre de la mascota ni precio del servicio; esos datos se obtienen por joins.

**Dependencias funcionales principales:**

| Tabla | Dependencias |
|---|---|
| `Personas` | `PersonaId → Nombre, Apellido, Documento, Telefono, Email, TipoPersona` |
| `Clientes` | `ClienteId → Direccion, FechaRegistro, PuntosFidelidad` |
| `Mascotas` | `MascotaId → ClienteId, EspecieId, Nombre, Raza, FechaNacimiento, PesoKg, Sexo` |
| `Servicios` | `ServicioId → Codigo, Nombre, PrecioBase, DuracionMinutos, CategoriaServicioId, TipoServicio` |
| `Citas` | `CitaId → ClienteId, MascotaId, VeterinarioId, ServicioId, FechaHora, EstadoCitaId` |
| `Facturas` | `FacturaId → Numero, ClienteId, CitaId, Fecha, DescuentoPorcentaje, ImpuestoPorcentaje` |
| `DetallesFactura` | `DetalleFacturaId → FacturaId, ServicioId, Descripcion, Cantidad, PrecioUnitario` |
| `Productos` | `ProductoId → CategoriaProductoId, Nombre, Precio, Stock, StockMinimo, Vencimiento` |

## 4. Tercera forma normal (3NF)

Una tabla está en 3NF cuando está en 2NF y no existen dependencias transitivas: ningún atributo no clave depende de otro atributo no clave.

- `Clientes` no repite `Nombre`, `Documento` ni `Telefono`; esos datos viven en `Personas`.
- `Mascotas` no repite el nombre del dueño; se obtiene desde `Clientes`/`Personas`.
- `Citas` no repite el nombre del veterinario, la mascota ni el servicio.
- `Facturas` no almacena subtotal, descuento, impuesto ni total; se calculan en `vw_FacturasResumen`.
- `Mascotas` no almacena la edad; se calcula desde `FechaNacimiento`.
- `Productos` no almacena la categoría en texto; usa `CategoriaProductoId`.
- `Servicios` no almacena el nombre de la categoría; usa `CategoriaServicioId`.

## 5. Redundancias controladas

Algunas columnas parecen redundantes, pero se mantienen de forma intencional y se documentan.

| Columna | Motivo |
|---|---|
| `Personas.TipoPersona` | Discriminador de subtipo para consultas rápidas. No determina otros atributos no clave; los atributos específicos viven en `Clientes` o `Empleados`. |
| `Empleados.TipoEmpleado` | Discriminador de subtipo (`Veterinario`, `Recepcionista`, `Auxiliar`). |
| `Mascotas.EspecieId` | Discriminador y clave foránea al catálogo; permite filtrar sin unir con las tablas de subtipo. |
| `Servicios.TipoServicio` | Discriminador de subtipo para reportes y facturación. |
| `DetallesFactura.PrecioUnitario` | Snapshot del precio cobrado. Evita que un cambio de tarifa altere facturas históricas. |
| `Facturas.Numero` | Identificador legible y único para el usuario; no es un atributo derivado de otra tabla. |
| `Clientes.PuntosFidelidad` | Saldo mutable de un programa de fidelidad. Se acumula desde facturas, pero puede usarse para canjes futuros, por lo que no es una simple suma derivable en todo momento. |
| `Veterinarios.NumeroColegiatura` | Identificador profesional único. |

Si se quisiera una normalización aún más estricta, `PuntosFidelidad` podría moverse a una tabla `MovimientosPuntos` con un saldo calculado. Se deja como saldo por simplicidad operativa y se documenta como denormalización controlada.

## 6. Forma normal de Boyce-Codd (BCNF)

Se revisó que todo determinante sea clave candidata:

- `Personas.Documento` es único y determina una persona.
- `Servicios.Codigo` es único.
- `Veterinarios.NumeroColegiatura` es único.
- `Facturas.Numero` es único.
- `Especies.Nombre`, `EstadosCita.Nombre`, `MetodosPago.Nombre`, `CategoriasProducto.Nombre` y `CategoriasServicio.Nombre` son únicos.

Por lo tanto, el modelo cumple BCNF en sus tablas principales.

## 7. Ejemplo de factura normalizada

**Tabla `Facturas`:**

| FacturaId | Numero | ClienteId | CitaId | Fecha | DescuentoPorcentaje | ImpuestoPorcentaje | Pagada |
|---|---|---|---|---|---:|---:|---|
| F-001 | HV-2026-0001 | C-001 | CIT-001 | 2026-10-15 | 10 | 15 | 1 |

**Tabla `DetallesFactura`:**

| DetalleFacturaId | FacturaId | ServicioId | Descripción | Cantidad | PrecioUnitario |
|---|---|---|---|---:|---:|
| D-001 | F-001 | S-001 | Consulta general - Luna | 1 | 850.00 |
| D-002 | F-001 | S-002 | Vacunación - Luna | 1 | 450.00 |

**Vista `vw_FacturasResumen`:**

| FacturaId | Subtotal | Descuento | Impuesto | Total |
|---|---:|---:|---:|---:|
| F-001 | 1300.00 | 130.00 | 175.50 | 1345.50 |

## 8. Conclusión

El modelo de HuellaVet cumple 1NF, 2NF y 3NF. Las únicas redundancias son discriminadores de subtipo, claves alternativas únicas, precios históricos y un saldo de fidelidad; todas están justificadas y documentadas. Los totales y la edad no se almacenan, lo que evita dependencias transitivas y mantiene la integridad de los datos.
