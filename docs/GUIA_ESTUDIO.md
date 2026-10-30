# Guía de estudio y defensa · HuellaVet

## 1. Resumen en una frase

HuellaVet es un avance de sistema para clínica veterinaria en C#/.NET 10, con arquitectura por capas, POO aplicada, interfaz Windows Forms conectada a SQL Server, pruebas automatizadas y documentación completa.

---

## 2. Qué debo decir primero

> "El proyecto demuestra herencia, polimorfismo y sobrecarga en un dominio real de clínica veterinaria. La lógica está separada de la interfaz, la persistencia puede ser JSON o SQL Server, y el sistema se ejecuta y se prueba."

---

## 3. Estructura del proyecto

| Proyecto | Responsabilidad |
|---|---|
| `HuellaVet.Domain` | Entidades, reglas, jerarquías y excepciones. No depende de nadie. |
| `HuellaVet.Application` | Casos de uso, validaciones, facturación y reportes. |
| `HuellaVet.Infrastructure` | Persistencia JSON y SQL Server (`SqlAlmacenClinica`). |
| `HuellaVet.Console` | Interfaz de consola y demo. |
| `HuellaVet.WinForms` | Interfaz gráfica conectada a SQL Server. |
| `HuellaVet.Laboratorio` | Solución exacta de la Clase 03. |
| `HuellaVet.Tests` | 32 pruebas automatizadas. |

---

## 4. POO aplicado

### Herencia
- `Persona` → `Cliente`, `Empleado`
- `Empleado` → `Veterinario`, `Recepcionista`, `AuxiliarVeterinario`
- `Mascota` → `Perro`, `Gato`, `Ave`, `Conejo`
- `ServicioVeterinario` → `ConsultaGeneral`, `Vacunacion`, `Cirugia`, `Peluqueria`, `Hospitalizacion`

### Polimorfismo
- `ServicioVeterinario.CalcularPrecio(Mascota)` decide el precio según el tipo real.
- `Factura.AgregarDetalle(servicio, mascota)` llama a `CalcularPrecio` sin `if` por tipo.
- `Empleado.CalcularSalario()` cambia según el cargo.

### Sobrecarga
- `ServicioVeterinario.CalcularPrecio(Mascota, descuento, cantidad)`
- `Factura.AgregarDetalle(servicio, mascota, cantidad)`
- `Factura.AgregarDetalle(descripcion, precio, cantidad)`
- `FacturacionService.GenerarFacturaParaCita(...)` con y sin descuento

### Encapsulamiento
- `Factura.Total` se calcula, no se asigna.
- `Pagada`, `MetodoPago` y `FechaPago` tienen setter privado.
- `Cita` controla sus transiciones de estado.
- `Producto` valida stock antes de descontar.

---

## 5. Clase 03 exacta

### Herencia: fútbol
- `SeleccionFutbol` → `Futbolista`, `Entrenador`, `Masajista`
- Archivo: `src/HuellaVet.Laboratorio/Clase03/SeleccionFutbol.cs`

### Polimorfismo: formas
- `Forma` → `Circulo`, `Rectangulo`, `Cuadrado`
- Archivo: `src/HuellaVet.Laboratorio/Clase03/Formas.cs`

### Sobrecarga
- `CalculadoraPrecios.Calcular` con 4 variantes
- Archivo: `src/HuellaVet.Laboratorio/Clase03/CalculadoraPrecios.cs`

---

## 6. Base de datos SQL Server

### Tablas principales
- `Personas`, `Clientes`, `Empleados`
- `Mascotas`, `Perros`, `Gatos`, `Aves`, `Conejos`
- `Servicios`, `ConsultasGenerales`, `Vacunaciones`, `Cirugias`, `Peluquerias`, `Hospitalizaciones`
- `Citas`, `Facturas`, `DetallesFactura`, `Productos`
- Catálogos: `Especies`, `EstadosCita`, `MetodosPago`, `CategoriasProducto`, `CategoriasServicio`

### Decisiones importantes
- **Table-per-type**: atributos específicos en tablas hijas.
- **GUID** como clave primaria.
- **Totales no almacenados**: se calculan en `vw_FacturasResumen`.
- **Precio histórico** en `DetallesFactura`.
- **Índice único filtrado** `UX_Facturas_Cita`.
- **CHECK** para enumeraciones.
- Normalización en **3NF**.

### Vistas
- `vw_Empleados`
- `vw_Mascotas`
- `vw_Agenda`
- `vw_FacturasResumen`
- `vw_StockBajo`
- `vw_VacunasProximas`

---

## 7. Persistencia

### JSON
- Archivo: `%LOCALAPPDATA%\HuellaVet\huellavet.json`
- Escritura atómica con `.tmp` + `File.Move`.
- Si se daña, se crea copia `.corrupto-<fecha>.json`.

### SQL Server
- `SqlAlmacenClinica` implementa `IAlmacenClinica`.
- Carga y guarda el agregado completo.
- Usa transacciones ADO.NET.
- La app Windows Forms consume los mismos servicios.

---

## 8. Flujo principal

```text
Recepción registra cliente y mascota
Veterinario atiende cita
Recepción genera factura
Factura calcula precio polimórfico
Se aplica descuento e IVA
Se registra pago y puntos
Se guarda en JSON o SQL Server
```

---

## 9. Pruebas

- 32 pruebas automatizadas.
- 0 errores.
- Cubren:
  - Clase 03
  - Facturación
  - Agenda
  - Reportes
  - Persistencia JSON

Comandos:

```powershell
dotnet build HuellaVet.sln --configuration Release
dotnet test HuellaVet.sln --configuration Release
dotnet run --project src/HuellaVet.WinForms
```

---

## 10. Preguntas probables del profesor

### ¿Dónde está la herencia?
En `Persona → Empleado → Veterinario`, `Mascota → Perro`, `ServicioVeterinario → Cirugia`, y en el laboratorio de Clase 03.

### ¿Dónde está el polimorfismo?
En `ServicioVeterinario.CalcularPrecio(Mascota)` y en `Factura.AgregarDetalle`.

### ¿Dónde está la sobrecarga?
En `CalcularPrecio`, `AgregarDetalle` y `GenerarFacturaParaCita`.

### ¿Por qué usas interfaz?
`IAlmacenClinica` permite cambiar JSON por SQL Server sin tocar los servicios.

### ¿Cómo se guarda en SQL?
`SqlAlmacenClinica` carga y guarda todas las entidades con ADO.NET y transacciones.

### ¿Por qué los nombres no están en Clientes?
Porque están en `Personas`; es normalización table-per-type.

### ¿Qué falta para la entrega final?
CRUD completo, interfaz más completa, autenticación, reportes PDF/Excel y pruebas de integración.

---

## 11. Checklist antes de presentar

- [ ] `dotnet build` sin errores.
- [ ] `dotnet test` con 32 correctas.
- [ ] Windows Forms abre y carga datos.
- [ ] SQL Server tiene la base `HuellaVet`.
- [ ] Demo `--demo` funciona.
- [ ] Tengo abierto `docs/GUION_DEFENSA.md`.
- [ ] Tengo abierto `docs/CLASE_03.md`.
- [ ] Tengo abierto `docs/RUBRICA.md`.

---

## 12. Archivos clave para la defensa

| Archivo | Uso |
|---|---|
| `src/HuellaVet.Domain/Servicios/ServicioVeterinario.cs` | Polimorfismo central. |
| `src/HuellaVet.Domain/Facturacion/Factura.cs` | Encapsulamiento y cálculo. |
| `src/HuellaVet.Laboratorio/Clase03/SeleccionFutbol.cs` | Herencia exacta. |
| `src/HuellaVet.Laboratorio/Clase03/Formas.cs` | Polimorfismo exacto. |
| `src/HuellaVet.Infrastructure/Persistencia/SqlAlmacenClinica.cs` | SQL Server real. |
| `docs/bd/06_huellavet_sqlserver.sql` | Script de base de datos. |
| `docs/GUION_DEFENSA.md` | Guion de 7 minutos. |
