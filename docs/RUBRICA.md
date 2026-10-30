# Rúbrica de verificación · HuellaVet

Esta checklist permite comprobar el avance contra los requisitos del proyecto final y de la Clase 03. Cada fila indica **dónde** está la evidencia para que el docente pueda revisarla sin buscar a ciegas.

**Leyenda:** ✅ implementado y verificable · 🟡 parcial o manual · 🔜 planificado para la siguiente entrega

---

## 1. Entregables

| Requisito | Evidencia | Estado |
|---|---|---|
| Repositorio con el código fuente | `HuellaVet.sln` y carpetas `src/`, `tests/` | ✅ |
| Documento del avance | `docs/AVANCE.md` | ✅ |
| README de instalación y uso | `README.md` | ✅ |
| Diagramas de clases | `docs/DIAGRAMA_CLASES.md` | ✅ |
| Arquitectura y decisiones | `docs/ARQUITECTURA.md` | ✅ |
| Actividad de la Clase 03 | `src/HuellaVet.Laboratorio/Clase03/` y `docs/CLASE_03.md` | ✅ |
| Guion de defensa | `docs/GUION_DEFENSA.md` | ✅ |
| Evidencia de ejecución | `docs/capturas/demo.txt` | ✅ |
| Pruebas automatizadas | `tests/HuellaVet.Tests/` | ✅ |
| Integración continua | `.github/workflows/ci.yml` | ✅ |
| Historial de Git con commits | Repositorio local listo para publicar | ✅ |

---

## 2. Programación orientada a objetos

| Concepto | Evidencia | Estado |
|---|---|---|
| Clases y objetos | `Persona`, `Mascota`, `Cita`, `Factura`, etc. | ✅ |
| Propiedades y métodos | Todas las entidades del dominio | ✅ |
| Encapsulamiento | `Factura.Total` calculado; setters privados en `Pagada`, `MetodoPago`, `FechaPago` | ✅ |
| Abstracción | `Persona`, `Empleado`, `Mascota`, `ServicioVeterinario`, `Forma` | ✅ |
| Herencia | `Persona` → `Empleado` → cargos; `Mascota` → especies; `ServicioVeterinario` → servicios | ✅ |
| Herencia múltiple de niveles | `Persona` → `Empleado` → `Veterinario` | ✅ |
| Polimorfismo con `virtual`/`override` | `Concentrarse`, `Viajar`, `CalcularSalario`, `Ficha`, `Descripcion` | ✅ |
| Polimorfismo con `abstract` | `Forma.CalculaArea`, `Mascota.Sonido`, `ServicioVeterinario.CalcularPrecio` | ✅ |
| Sobrecarga | `CalcularPrecio` (4), `AgregarDetalle` (2), `GenerarFacturaParaCita` (2), `Calcular` (4) | ✅ |
| Interface | `IAlmacenClinica` | ✅ |
| Composición | `Factura` contiene `DetalleFactura`; `ClinicaData` contiene las colecciones | ✅ |
| Colecciones genéricas | `List<T>`, `Dictionary<TKey,TValue>` | ✅ |
| LINQ | Consultas de reportes, búsquedas y cálculos | ✅ |
| Excepciones personalizadas | `ReglaNegocioException` | ✅ |
| Enumeraciones | `EstadoCita`, `MetodoPago`, `SexoMascota`, `NivelRiesgo`, `TipoCorte` | ✅ |

---

## 3. Actividad de la Clase 03

### 3.1 Herencia

| Requisito | Evidencia | Estado |
|---|---|---|
| Clase `SeleccionFutbol` | `SeleccionFutbol.cs` | ✅ |
| Datos `id`, `Nombre`, `Apellidos`, `Edad` | Propiedades `Id`, `Nombre`, `Apellidos`, `Edad` | ✅ |
| `Concentrarse()` | Método virtual | ✅ |
| `Viajar()` | Método virtual | ✅ |
| `Futbolista` hereda de `SeleccionFutbol` | `public class Futbolista : SeleccionFutbol` | ✅ |
| Datos `dorsal`, `demarcacion` | `Dorsal`, `Demarcacion` | ✅ |
| `JugarPartido()` | Método propio | ✅ |
| `Entrenar()` | Método propio | ✅ |
| `Entrenador` hereda de `SeleccionFutbol` | `public class Entrenador : SeleccionFutbol` | ✅ |
| Dato `idFederacion` | `IdFederacion` | ✅ |
| `DirigirPartido()` | Método propio | ✅ |
| `DirigirEntrenamiento()` | Método propio | ✅ |
| `Masajista` hereda de `SeleccionFutbol` | `public class Masajista : SeleccionFutbol` | ✅ |
| Datos `titulacion`, `aniosExperiencia` | `Titulacion`, `AniosExperiencia` | ✅ |
| `DarMasaje()` | Método propio | ✅ |
| Demostración con lista de la base | `DemostracionClase03.Herencia()` | ✅ |

### 3.2 Polimorfismo

| Requisito | Evidencia | Estado |
|---|---|---|
| Clase `Forma` | `Formas.cs` | ✅ |
| Atributo `Area` | Propiedad con `protected set` | ✅ |
| Método `CalculaArea()` | Método abstracto | ✅ |
| `Circulo` hereda de `Forma` | `public sealed class Circulo : Forma` | ✅ |
| `Rectangulo` hereda de `Forma` | `public sealed class Rectangulo : Forma` | ✅ |
| `Cuadrado` hereda de `Forma` | `public sealed class Cuadrado : Forma` | ✅ |
| Cálculo correcto de áreas | 78.54, 24.00 y 16.00 | ✅ |
| Uso polimórfico | `List<Forma>` y `DemostracionClase03.Polimorfismo()` | ✅ |

### 3.3 Sobrecarga

| Requisito | Evidencia | Estado |
|---|---|---|
| Mismo nombre, distintos parámetros | `CalculadoraPrecios.Calcular` (4 versiones) | ✅ |
| Resultados correctos | 500, 450, 1,500 y 1,350 | ✅ |
| Aplicación en el proyecto | `ServicioVeterinario`, `Factura`, `FacturacionService` | ✅ |
| Prueba automatizada | `Sobrecarga_ResuelveSegunLosArgumentos` | ✅ |

---

## 4. Funcionalidades del sistema

| Módulo | Requisito | Evidencia | Estado |
|---|---|---|---|
| Clientes | Registrar con documento único | `ClinicaService.RegistrarCliente` | ✅ |
| Clientes | Buscar por nombre, documento, teléfono o correo | `BuscarClientes` | ✅ |
| Clientes | Actualizar contacto | `ActualizarContactoCliente` | ✅ |
| Mascotas | Registrar perro, gato, ave o conejo | `RegistrarMascota` | ✅ |
| Mascotas | Calcular edad y ficha | `Mascota.EdadAnios`, `Ficha()` | ✅ |
| Mascotas | Expediente con citas y facturas | `ObtenerExpediente` | ✅ |
| Agenda | Programar cita | `ProgramarCita` | ✅ |
| Agenda | Evitar choques de 30 minutos | Validación en `ProgramarCita` | ✅ |
| Agenda | Confirmar, iniciar, atender, cancelar, marcar inasistencia | `Cita` y `ClinicaService` | ✅ |
| Servicios | Catálogo de cinco servicios | `ConsultaGeneral`, `Vacunacion`, `Cirugia`, `Peluqueria`, `Hospitalizacion` | ✅ |
| Servicios | Precio distinto por servicio y mascota | `CalcularPrecio(Mascota)` | ✅ |
| Servicios | Simular precio y actualizar tarifa | `ServiciosPage` | ✅ |
| Facturación | Generar factura desde cita atendida | `FacturacionService` | ✅ |
| Facturación | Descuento máximo 50 % | `Factura.AplicarDescuento` | ✅ |
| Facturación | IVA 15 % | `Factura.Impuesto` | ✅ |
| Facturación | Evitar factura duplicada | Validación por `CitaId` | ✅ |
| Facturación | Registrar pago y puntos | `RegistrarPago`, `AcumularPuntos` | ✅ |
| Personal | Nómina por cargo | `CalcularSalario` | ✅ |
| Inventario | Stock, mínimo, reposición y descuento | `Producto` y `ClinicaService` | ✅ |
| Reportes | Dashboard | `ReporteService.ObtenerResumen` | ✅ |
| Reportes | Ingresos por servicio | `IngresosPorServicio` | ✅ |
| Reportes | Pacientes por especie | `PacientesPorEspecie` | ✅ |
| Reportes | Servicios más solicitados | `ServiciosMasSolicitados` | ✅ |
| Reportes | Vacunas próximas | `ProximasVacunas` | ✅ |
| Reportes | Ingresos mensuales | `IngresosUltimosMeses` | ✅ |
| Persistencia | Guardar y cargar JSON | `JsonAlmacenClinica` | ✅ |
| Persistencia | Conservar tipos derivados | `[JsonPolymorphic]` y `$tipo` | ✅ |
| Persistencia | Guardado atómico | Archivo `.tmp` + `File.Move` | ✅ |
| Persistencia | Recuperar archivo dañado | Copia `.corrupto-<fecha>.json` | ✅ |

---

## 5. Interfaz y experiencia

| Requisito | Evidencia | Estado |
|---|---|---|
| Menú principal claro | `MenuPrincipal.cs` | ✅ |
| Dashboard con indicadores | `DashboardPage.cs` | ✅ |
| Tablas y colores | Spectre.Console | ✅ |
| Mensajes en español | Toda la interfaz | ✅ |
| Formato de moneda local | Cultura `es-NI`, C$ | ✅ |
| Ayuda y versión | `--help`, `--version` | ✅ |
| Demostración automática | `--demo` | ✅ |
| Reinicio de datos | `--reset` | ✅ |
| Ruta de datos configurable | `--data` | ✅ |
| Manejo de errores sin cerrar la app | `MenuPrincipal` captura excepciones | ✅ |
| Adaptación a salida redirigida | `UI.Limpiar()` verifica `Console.IsOutputRedirected` | ✅ |

---

## 6. Calidad y pruebas

| Requisito | Evidencia | Estado |
|---|---|---|
| Pruebas unitarias | 32 casos xUnit | ✅ |
| Pruebas de reglas de negocio | `ClinicaServiceTests` | ✅ |
| Pruebas de facturación | `FacturacionServiceTests`, `FacturaTests` | ✅ |
| Pruebas de reportes | `ReporteServiceTests` | ✅ |
| Pruebas de persistencia | `JsonAlmacenClinicaTests` | ✅ |
| Pruebas de la Clase 03 | `Clase03Tests` | ✅ |
| Resultado | 32 correctas, 0 errores | ✅ |
| CI | GitHub Actions: restore, build, test | ✅ |
| Código compilable en Release | `dotnet build -c Release` | ✅ |
| Sin advertencias | `dotnet build` reporta 0 advertencias y 0 errores | ✅ |

> Verificado con `dotnet build HuellaVet.sln --configuration Release`: compilación correcta, 0 advertencias, 0 errores.

---

## 7. Documentación

| Requisito | Evidencia | Estado |
|---|---|---|
| Portada e identificación | `AVANCE.md` | ✅ |
| Resumen | `AVANCE.md` | ✅ |
| Problema y justificación | `AVANCE.md` | ✅ |
| Objetivos general y específicos | `AVANCE.md` | ✅ |
| Alcance y limitaciones | `AVANCE.md` | ✅ |
| Marco teórico | `AVANCE.md` | ✅ |
| Metodología | `AVANCE.md` | ✅ |
| Análisis y diseño | `AVANCE.md`, `ARQUITECTURA.md` | ✅ |
| Diagramas | `DIAGRAMA_CLASES.md` | ✅ |
| Mapeo de Clase 03 | `CLASE_03.md` | ✅ |
| Pruebas y resultados | `AVANCE.md` | ✅ |
| Conclusiones | `AVANCE.md` | ✅ |
| Trabajo futuro | `AVANCE.md` | ✅ |
| Bibliografía | `AVANCE.md` | ✅ |
| Guion de defensa | `GUION_DEFENSA.md` | ✅ |
| Captura de ejecución | `capturas/demo.txt` | ✅ |

---

## 8. Requisitos no funcionales

| Requisito | Cómo se cumple | Estado |
|---|---|---|
| Portabilidad | .NET 10; sin dependencias nativas | ✅ |
| Mantenibilidad | Capas, nombres claros, archivos pequeños | ✅ |
| Confiabilidad | Guardado atómico y respaldo | ✅ |
| Usabilidad | Menús, ayudas, mensajes y demo | ✅ |
| Rendimiento | Colecciones en memoria para el volumen académico | ✅ |
| Seguridad | No hay credenciales ni datos reales; JSON local | 🟡 |
| Escalabilidad | Contratos e interfaces para sustituir persistencia | 🟡 |
| Accesibilidad | Texto claro; pendiente de interfaz gráfica | 🔜 |

> “Seguridad” y “escalabilidad” quedan parciales porque el avance no incluye autenticación ni base de datos multiusuario. No se presentan como terminadas.

---

## 9. Lo que falta para la entrega final

| Tema | Estado | Próximo paso |
|---|---|---|
| Interfaz gráfica | 🔜 | WinForms/WPF o web. |
| SQL Server + ADO.NET | 🔜 | Tablas, repositorios y transacciones. |
| CRUD completo | 🔜 | Altas, bajas y cambios de todas las entidades. |
| Autenticación y roles | 🔜 | Login, permisos y auditoría. |
| Reportes PDF/Excel | 🔜 | Exportación y gráficos. |
| Recordatorios | 🔜 | Correo o SMS. |
| Pruebas de integración | 🔜 | Flujos completos de UI. |

---

## 10. Verificación rápida

Copiar y ejecutar desde la raíz:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet run --project src/HuellaVet.Console -- --demo --reset
```

Criterios de aceptación:

- [ ] La solución restaura sin errores.
- [ ] La compilación termina con 0 errores.
- [ ] Las 32 pruebas pasan.
- [ ] La demo muestra dashboard, factura y las tres secciones de la Clase 03.
- [ ] El menú interactivo abre y permite navegar.
- [ ] El archivo JSON se crea en `%LOCALAPPDATA%\HuellaVet\huellavet.json`.
- [ ] El repositorio no incluye `bin`, `obj` ni archivos de usuario.

---

## 11. Autoevaluación

| Criterio | Peso relativo | Nota cualitativa |
|---|---|---|
| Dominio y POO | Alto | Sólido: jerarquías claras y reglas en los objetos. |
| Clase 03 exacta | Alto | Completa: nombres, miembros y demostración. |
| Funcionalidad | Alto | Avance amplio y navegable. |
| Pruebas | Medio | 32 casos, 0 fallos, CI. |
| Documentación | Medio | Completa y con evidencia. |
| Interfaz | Medio | Consola cuidada; gráfica pendiente. |
| Persistencia | Medio | JSON robusto; SQL pendiente. |

**Conclusión:** el avance cumple los objetivos de la Clase 03 y presenta una base funcional, documentada y verificable para la entrega final.
