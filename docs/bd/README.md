# Diseño de base de datos · HuellaVet

Este directorio contiene la **Fase 1 a Fase 6 del diseño de base de datos** de HuellaVet, siguiendo la guía de la imagen compartida por el profesor:

1. Requisitos y análisis.
2. Modelo conceptual.
3. Modelo lógico.
4. Normalización.
5. Modelo físico.
6. Implementación.

El diseño se deriva del dominio C# de `src/HuellaVet.Domain`, pero **no sustituye** la persistencia JSON del avance actual: la aplicación sigue usando JSON. Este directorio deja el modelo relacional listo para migrar a SQL Server en la Unidad 3.

---

## Archivos

| Archivo | Fase | Contenido |
|---|---|---|
| `01_requisitos.md` | 1 | Requisitos, reglas de negocio, entidades, volumen y trazabilidad con el dominio C#. |
| `02_modelo_conceptual.drawio` | 2 | Diagrama entidad-relación de alto nivel. |
| `03_modelo_logico.drawio` | 3 | Esquema relacional completo con claves, tipos y relaciones. |
| `04_normalizacion.md` | 4 | Análisis de 1NF, 2NF, 3NF y decisiones de normalización. |
| `05_modelo_fisico.drawio` | 5 | Modelo físico con SQL Server 2022, índices y restricciones. |
| `06_huellavet_sqlserver.sql` | 6 | Script de creación, índices, catálogos, vistas y verificación. |
| `07_diccionario_datos.md` | 5/6 | Diccionario de las 26 tablas y 244 columnas. |
| `generar_diagramas.mjs` | Soporte | Fuente única de los diagramas y del diccionario. |
| `modelo-bd.mjs` | Soporte | Modelo de tablas, campos, relaciones y semillas. |

---

## Resumen del modelo

| Grupo | Tablas |
|---|---|
| Catálogos | `Especies`, `EstadosCita`, `MetodosPago`, `CategoriasProducto`, `CategoriasServicio` (5) |
| Personas | `Personas`, `Clientes`, `Empleados`, `Veterinarios`, `Recepcionistas`, `AuxiliaresVeterinarios` (6) |
| Pacientes | `Mascotas`, `Perros`, `Gatos`, `Aves`, `Conejos` (5) |
| Servicios | `Servicios`, `ConsultasGenerales`, `Vacunaciones`, `Cirugias`, `Peluquerias`, `Hospitalizaciones` (6) |
| Operación | `Citas`, `Facturas`, `DetallesFactura`, `Productos` (4) |
| **Total** | **26 tablas** |

Relaciones: 28. Incluyen 14 herencias (tabla hija → tabla base) y 14 relaciones de negocio.

---

## Cómo abrir los diagramas `.drawio`

### Opción 1: aplicación web

1. Abrir [https://app.diagrams.net](https://app.diagrams.net).
2. **File → Open from → Device**.
3. Seleccionar `02_modelo_conceptual.drawio`, `03_modelo_logico.drawio` o `05_modelo_fisico.drawio`.
4. Usar el selector de páginas inferior para navegar por las vistas.

### Opción 2: VS Code

1. Instalar la extensión **Draw.io Integration** de Henning Dieterichs.
2. Abrir el archivo `.drawio` con doble clic.
3. Editar y guardar.

### Opción 3: exportar a PDF o PNG

Desde draw.io: **File → Export as → PDF/PNG**, seleccionando la página que se desea entregar.

---

## Decisiones de diseño

| Decisión | Justificación |
|---|---|
| **Table-per-type** para personas, mascotas y servicios | Mantiene los atributos específicos en tablas hijas y evita columnas nulas. |
| **GUID (`UNIQUEIDENTIFIER`)** como clave primaria | Coincide con `Guid` del dominio C# y facilita migraciones. |
| **Catálogos** para especies, estados, métodos de pago y categorías | Evita repetir texto y permite reportes. |
| **Totales no almacenados** | `Subtotal`, `Descuento`, `Impuesto` y `Total` se calculan en `vw_FacturasResumen`. |
| **Precio histórico** en `DetallesFactura` | Un cambio futuro de tarifa no altera facturas antiguas. |
| **Índice único filtrado** `UX_Facturas_Cita` | Garantiza una sola factura por cita sin bloquear ventas manuales. |
| **`CHECK` para enumeraciones** | Valida `Sexo`, `TipoPersona`, `TipoEmpleado`, `TipoServicio`, `NivelRiesgo`, `TipoCorte`, etc. |
| **Edad y stock bajo no se almacenan** | Se calculan en vistas o en la aplicación. |
| **`PuntosFidelidad` como saldo** | Es un estado de negocio mutable; se explica como denormalización controlada en `04_normalizacion.md`. |

---

## Cómo ejecutar el script SQL

Requisito: SQL Server 2022 o superior.

### SSMS / Azure Data Studio

1. Abrir `06_huellavet_sqlserver.sql`.
2. Ejecutar con **F5**.
3. El script crea la base `HuellaVet`, las 26 tablas, índices, catálogos y vistas.

### sqlcmd

```powershell
"C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn\SQLCMD.EXE" -S localhost -E -i "docs\bd\06_huellavet_sqlserver.sql" -b -C
```

### Verificación realizada

El script se ejecutó correctamente en SQL Server local con `sqlcmd`:

- 26 tablas creadas.
- 6 vistas creadas.
- 5 catálogos con datos iniciales.
- 0 errores de sintaxis.

> El script asume una base nueva. Si ya existe `HuellaVet`, cree una base vacía o ajuste el script antes de ejecutarlo.

---

## Regenerar los diagramas

Los diagramas y el diccionario se generan desde una fuente única:

```powershell
node docs/bd/generar_diagramas.mjs
```

Requisito: Node.js 18 o superior.

---

## Relación con el resto del proyecto

- La aplicación actual persiste en `%LOCALAPPDATA%\HuellaVet\huellavet.json`.
- Este directorio documenta el modelo relacional objetivo y deja el script físico listo.
- La migración a ADO.NET / Entity Framework queda para la Unidad 3, como se indica en `README.md` y `docs/AVANCE.md`.
- El dominio C# sigue siendo la fuente de verdad de las reglas; el script SQL replica la estructura y las restricciones, no la lógica de precios.
- La documentación académica principal sigue estando en `docs/AVANCE.md`.

---

## Checklist de la guía del profesor

| Fase | Evidencia |
|---|---|
| 1. Requisitos y análisis | `01_requisitos.md` |
| 2. Modelo conceptual | `02_modelo_conceptual.drawio` |
| 3. Modelo lógico | `03_modelo_logico.drawio` |
| 4. Normalización | `04_normalizacion.md` |
| 5. Modelo físico | `05_modelo_fisico.drawio` |
| 6. Implementación | `06_huellavet_sqlserver.sql`, `07_diccionario_datos.md` |
| Documentación | `docs/AVANCE.md`, `docs/ARQUITECTURA.md`, `docs/DIAGRAMA_CLASES.md`, `docs/RUBRICA.md` |
