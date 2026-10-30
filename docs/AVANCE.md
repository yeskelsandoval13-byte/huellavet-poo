# HuellaVet

## Sistema de gestión clínica veterinaria

**Avance del proyecto final**

---

| Campo | Detalle |
|---|---|
| **Estudiante** | Yeskel Sandoval |
| **Carrera** | Ingeniería en Sistemas y Computación |
| **Asignatura** | Programación Orientada a Objetos (FI-134) |
| **Docente** | Ing. Gustavo Acevedo Martínez |
| **Periodo** | III Cuatrimestre 2026 |
| **Fecha de entrega** | Octubre de 2026 |
| **Tecnología** | C# 14 · .NET 10 · Spectre.Console · xUnit |
| **Repositorio** | `huellavet-poo` |

> «Cuidamos cada huella.»

---

## Resumen

HuellaVet es el avance de un sistema de gestión para una clínica veterinaria. El proyecto resuelve, mediante programación orientada a objetos, los procesos de registro de clientes y mascotas, expediente clínico, agenda de citas, atención, facturación, cálculo de nómina, inventario y reportes.

La solución se organiza en seis proyectos: dominio, aplicación, infraestructura, interfaz de consola, laboratorio de la Clase 03 y pruebas automatizadas. El dominio concentra las reglas del negocio y las jerarquías de clases; la aplicación orquesta los casos de uso; la infraestructura persiste el estado en JSON; la consola ofrece una experiencia guiada con Spectre.Console.

El diseño aplica de forma explícita **herencia, polimorfismo y sobrecarga de métodos**. Además, reproduce con nombres y miembros exactos las dos ilustraciones solicitadas en la Clase 03: la jerarquía `SeleccionFutbol` → `Futbolista`, `Entrenador`, `Masajista`, y la jerarquía `Forma` → `Circulo`, `Rectangulo`, `Cuadrado`.

El avance incluye **32 pruebas automatizadas aprobadas**, una demostración no interactiva (`--demo`), persistencia local resistente a archivos dañados y documentación de arquitectura, diagramas, guion de defensa y rúbrica.

**Palabras clave:** programación orientada a objetos, C#, herencia, polimorfismo, sobrecarga, arquitectura por capas, veterinaria, pruebas automatizadas.

---

## 1. Introducción

Una clínica veterinaria administra diariamente información que cambia con rapidez: llegan pacientes nuevos, se reprograman citas, se actualizan pesos y diagnósticos, se generan facturas y se controla el inventario de medicamentos. Cuando esos datos viven en cuadernos, hojas de cálculo separadas o mensajes de texto, la operación pierde trazabilidad y aumentan los errores.

HuellaVet nace como respuesta académica a ese escenario. No pretende sustituir un sistema comercial ni una base de datos empresarial en esta etapa; pretende demostrar que un diseño orientado a objetos bien separado puede resolver el problema central de la clínica y crecer hacia una interfaz gráfica o una base de datos sin reescribir sus reglas.

El proyecto se desarrolló con C# sobre .NET 10 y una interfaz de consola. La consola no es una limitación del diseño: es una decisión deliberada para que la demostración sea portable, rápida y verificable en cualquier equipo con el SDK instalado. La lógica de negocio no depende de ella.

### 1.1 Motivación personal

El tema veterinario permite combinar dos intereses: el cuidado de los animales y la construcción de software útil para personas reales. La clínica ficticia permite modelar situaciones concretas —una mascota con alergias, una cirugía de alto riesgo, un cliente con puntos de fidelidad— y obliga a que la programación orientada a objetos resuelva problemas, no solo ejemplos abstractos.

---

## 2. Planteamiento del problema

### 2.1 Contexto

La clínica HuellaVet atiende a clientes con una o varias mascotas. Cada visita genera información clínica y administrativa:

- Datos del dueño y de la mascota.
- Motivo de la consulta, diagnóstico, peso y observaciones.
- Veterinario responsable, servicio y horario.
- Cargos, descuentos, impuesto, pago y puntos de fidelidad.
- Medicamentos e insumos disponibles.

### 2.2 Situación actual

En un escenario manual o disperso se identifican estos problemas:

1. **Información duplicada o incompleta.** El mismo cliente puede registrarse varias veces con documentos escritos de forma distinta.
2. **Choques de agenda.** Dos citas se asignan al mismo veterinario o a la misma mascota en horarios cercanos.
3. **Facturación inconsistente.** Los precios se calculan con criterios distintos según quién atiende, y se olvidan descuentos, impuestos o puntos.
4. **Poca visibilidad.** La dirección no tiene un resumen inmediato de ingresos, pacientes, stock bajo o vacunas próximas.
5. **Expedientes difíciles de consultar.** El historial de una mascota queda repartido entre hojas y facturas.

### 2.3 Formulación del problema

¿Cómo diseñar e implementar un avance funcional en C# que modele la operación de una clínica veterinaria aplicando herencia, polimorfismo y sobrecarga, con reglas de negocio verificables, persistencia y una interfaz que permita demostrar los flujos principales?

---

## 3. Justificación

El valor del proyecto se puede explicar en cuatro dimensiones.

**Académica.** El curso exige comprender los pilares de la programación orientada a objetos. HuellaVet los utiliza en un dominio realista: no hay una clase suelta para “probar herencia”, sino una jerarquía de personas, mascotas y servicios que necesita esas herramientas para funcionar.

**Técnica.** La separación por capas permite cambiar la interfaz de consola por WinForms, WPF o web, y el almacenamiento JSON por SQL Server, sin tocar las entidades. Las pruebas automatizadas reducen el riesgo de romper reglas al ampliar el sistema.

**Operativa.** El prototipo ya permite registrar clientes y mascotas, programar y atender citas, facturar, consultar reportes y revisar inventario. Es una base demostrable, no una maqueta vacía.

**Personal.** El proyecto obliga a tomar decisiones de diseño, justificar nombres, manejar errores y documentar. Esas habilidades se acercan más al trabajo profesional que resolver ejercicios aislados.

---

## 4. Objetivos

### 4.1 Objetivo general

Desarrollar un avance funcional de un sistema de gestión clínica veterinaria en C#, aplicando los principios de la programación orientada a objetos —en especial herencia, polimorfismo y sobrecarga— con una arquitectura por capas, persistencia local, pruebas automatizadas y documentación de entrega.

### 4.2 Objetivos específicos

1. Modelar las entidades principales de la clínica: personas, mascotas, servicios, citas, facturas, productos y el agregado que las contiene.
2. Implementar jerarquías con clases abstractas, propiedades abstractas, métodos virtuales y métodos sobrescritos.
3. Aplicar sobrecarga de métodos en el cálculo de precios, el detalle de factura y la generación de facturas.
4. Construir casos de uso para clientes, pacientes, agenda, facturación, personal, inventario y reportes.
5. Persistir la información en un archivo JSON con guardado atómico y recuperación ante archivos dañados.
6. Ofrecer una interfaz de consola clara, con menús, tablas, colores y un modo de demostración automática.
7. Reproducir con exactitud las clases y miembros de las ilustraciones de la Clase 03.
8. Validar las reglas principales con pruebas automatizadas y una integración continua.
9. Documentar la arquitectura, el mapeo de la Clase 03, el guion de defensa y el cumplimiento de requisitos.

---

## 5. Alcance

### 5.1 Incluido en esta entrega

| Área | Alcance |
|---|---|
| Clientes | Registro, búsqueda por nombre, documento, teléfono o correo, actualización de contacto y puntos de fidelidad. |
| Mascotas | Registro de perros, gatos, aves y conejos; ficha, edad calculada, alergias, peso y expediente. |
| Agenda | Programación, confirmación, inicio de atención, atención, cancelación, inasistencia y detección de choques. |
| Servicios | Consulta general, vacunación, cirugía, peluquería y hospitalización, con precios polimórficos. |
| Facturación | Factura desde cita atendida, descuento, IVA, pago, puntos y prevención de duplicados. |
| Personal | Veterinarios, recepcionistas y auxiliares; nómina calculada con reglas por cargo. |
| Inventario | Stock, mínimos, reposición, descuento y alertas. |
| Reportes | Dashboard, ingresos por servicio, pacientes por especie, servicios populares, vacunas próximas e ingresos mensuales. |
| Persistencia | JSON local con escritura atómica y copia de respaldo si el archivo está dañado. |
| Windows Forms | Interfaz gráfica conectada a SQL Server (avance). |
| Base de datos | Diseño físico y script SQL Server ejecutado correctamente. |
| Laboratorio | Clase 03 exacta: herencia, polimorfismo y sobrecarga. |
| Calidad | 32 pruebas xUnit, documento de arquitectura, diagramas y CI. |
| Base de datos | Diseño relacional completo en SQL Server: 26 tablas, vistas, índices y normalización 3NF. |

### 5.2 Fuera de alcance en esta entrega

- Interfaz gráfica completa de escritorio o web.
- CRUD completo de todas las entidades.
- Autenticación, roles y auditoría.
- Concurrencia multiusuario y transacciones distribuidas.
- Pasarela de pagos real.
- Envío de correos, recordatorios o mensajes.
- Gestión completa de medicamentos, recetas e imágenes médicas.

Estas exclusiones no son carencias ocultas: son la siguiente fase del proyecto. La arquitectura fue preparada para incorporarlas.

---

## 6. Marco teórico

### 6.1 Programación orientada a objetos

La programación orientada a objetos organiza el software alrededor de objetos que combinan estado y comportamiento. Un objeto es una instancia de una clase; la clase define atributos, métodos, propiedades y reglas. En HuellaVet, `Mascota` no es solo una estructura de datos: conoce su edad, su especie, su ficha y cómo calcular una dosis.

Los pilares aplicados son:

- **Abstracción:** se modela lo relevante para la clínica y se oculta lo accidental.
- **Encapsulamiento:** el estado se modifica mediante operaciones controladas.
- **Herencia:** se reutiliza y especializa comportamiento.
- **Polimorfismo:** un mismo mensaje produce comportamientos distintos según el objeto real.

### 6.2 Encapsulamiento

`Factura` es un buen ejemplo. Su total no se asigna desde fuera; se deriva de los detalles:

```csharp
public decimal Subtotal => Math.Round(Detalles.Sum(d => d.Subtotal), 2);
public decimal Descuento => Math.Round(Subtotal * DescuentoPorcentaje / 100m, 2);
public decimal Impuesto => Math.Round((Subtotal - Descuento) * ImpuestoPorcentaje / 100m, 2);
public decimal Total => Math.Round(Subtotal - Descuento + Impuesto, 2);
```

Además, `Pagada`, `MetodoPago` y `FechaPago` tienen setters privados. La única forma de pagar es `RegistrarPago(...)`, que valida que existan detalles y que la factura no esté pagada. Así se evita un estado imposible.

### 6.3 Herencia

La herencia permite que una clase derivada reciba miembros de una clase base y agregue o especialice comportamiento.

```csharp
public abstract class Persona
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public abstract string Rol { get; }
}

public abstract class Empleado : Persona
{
    public decimal SalarioBase { get; set; }
    public abstract string Cargo { get; }
    public virtual decimal CalcularSalario() => SalarioBase;
}

public sealed class Veterinario : Empleado
{
    public override string Cargo => "Veterinario";

    public override decimal CalcularSalario()
    {
        var bonoEspecialidad = Especialidad.Contains("Cirug") ? 3500m : 2000m;
        return SalarioBase + bonoEspecialidad + (AniosExperiencia * 250m);
    }
}
```

`Veterinario` **es un** `Empleado` y **es una** `Persona`. Hereda nombre, apellido, documento y teléfono; agrega especialidad, colegiatura y experiencia; redefine el cálculo del salario.

### 6.4 Polimorfismo

El polimorfismo permite tratar objetos de tipos distintos a través de una referencia común. En el proyecto se usa de dos formas complementarias:

**Polimorfismo de subtipo (dinámico).** Una lista de `ServicioVeterinario` contiene consultas, vacunaciones, cirugías, peluquerías y hospitalizaciones. El código que factura solo invoca:

```csharp
var precio = servicio.CalcularPrecio(mascota);
```

Cada clase concreta decide la fórmula. No hay una cadena de `if` por tipo.

**Polimorfismo en tiempo de ejecución con métodos virtuales.** `SeleccionFutbol.Concentrarse()` se declara `virtual`; `Futbolista`, `Entrenador` y `Masajista` lo sobrescriben. Una lista `List<SeleccionFutbol>` ejecuta la versión correcta de cada objeto.

### 6.5 Sobrecarga

La sobrecarga ocurre cuando una clase tiene varios métodos con el mismo nombre y distinta lista de parámetros. El compilador elige la versión según los argumentos.

```csharp
public decimal Calcular(decimal precioBase);
public decimal Calcular(decimal precioBase, decimal descuentoPorcentaje);
public decimal Calcular(decimal precioBase, int cantidad);
public decimal Calcular(decimal precioBase, decimal descuentoPorcentaje, int cantidad);
```

Es una forma de polimorfismo en tiempo de compilación. No debe confundirse con la sobrescritura (`override`), que ocurre en tiempo de ejecución y requiere herencia.

| Concepto | Momento | Requiere herencia | Ejemplo |
|---|---|---|---|
| Sobrecarga | Compilación | No | `Calcular(500, 10m)` |
| Sobrescritura | Ejecución | Sí | `Veterinario.CalcularSalario()` |
| Ocultamiento (`new`) | Compilación | Sí | No se usa en el proyecto |

### 6.6 Clases abstractas e interfaces

Una clase abstracta representa una idea incompleta que no debe instanciarse. `Persona`, `Mascota`, `Empleado`, `ServicioVeterinario` y `Forma` son abstractas porque no existe una “persona genérica” ni una “mascota genérica” en el modelo.

La interfaz `IAlmacenClinica` define el contrato de persistencia:

```csharp
public interface IAlmacenClinica
{
    ClinicaData Datos { get; }
    void Guardar();
    void Restablecer(ClinicaData datos);
}
```

La aplicación depende de esa abstracción, no de un archivo concreto. Hoy existen `JsonAlmacenClinica` y `AlmacenMemoria`; mañana puede existir `SqlAlmacenClinica`.

### 6.7 Arquitectura por capas

La arquitectura por capas separa responsabilidades y dirige las dependencias hacia el dominio. La interfaz no contiene reglas de negocio; la infraestructura no decide precios; el dominio no imprime mensajes.

### 6.8 Persistencia y serialización polimórfica

`System.Text.Json` serializa el agregado `ClinicaData`. Las jerarquías usan `[JsonPolymorphic]` y `[JsonDerivedType]` para conservar el tipo concreto:

```csharp
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(Perro), "perro")]
[JsonDerivedType(typeof(Gato), "gato")]
[JsonDerivedType(typeof(Ave), "ave")]
[JsonDerivedType(typeof(Conejo), "conejo")]
public abstract class Mascota : EntidadBase { ... }
```

Sin ese discriminador, al deserializar se perdería si un paciente es perro, gato, ave o conejo.

### 6.9 Pruebas automatizadas

xUnit permite verificar reglas sin abrir la consola. Las pruebas usan `AlmacenMemoria` para ser rápidas y deterministas; la persistencia JSON tiene una prueba de ida y vuelta con un archivo temporal.

---

## 7. Metodología

Se utilizó un desarrollo iterativo e incremental:

1. **Análisis del enunciado.** Se identificaron los requisitos de la Clase 03 y el alcance del avance.
2. **Diseño del dominio.** Se definieron jerarquías, relaciones, invariantes y excepciones.
3. **Construcción por capas.** Primero dominio, luego aplicación, infraestructura y consola.
4. **Datos de demostración.** Se creó un conjunto coherente de clientes, mascotas, citas, facturas y productos.
5. **Pruebas.** Se escribieron pruebas de reglas, cálculos, reportes, persistencia y Clase 03.
6. **Refinamiento de la experiencia.** Se añadieron tablas, colores, ayudas y el modo `--demo`.
7. **Documentación y preparación de la defensa.** Se elaboraron diagramas, guion y checklist.

La verificación se apoya en tres evidencias: `dotnet test`, la salida de `--demo` y la revisión manual de los flujos de la consola.

---

## 8. Análisis y diseño

### 8.1 Actores

| Actor | Necesidad principal |
|---|---|
| Recepción | Registrar clientes y mascotas, programar citas, cobrar. |
| Veterinario | Consultar expediente, atender cita, registrar diagnóstico y peso. |
| Administración | Revisar ingresos, nómina, servicios e inventario. |
| Docente / evaluador | Verificar herencia, polimorfismo, sobrecarga y funcionamiento. |

### 8.2 Requisitos funcionales

| Código | Requisito | Evidencia |
|---|---|---|
| RF-01 | Registrar clientes con documento único. | `ClinicaService.RegistrarCliente` |
| RF-02 | Registrar mascotas de cuatro especies. | `ClinicaService.RegistrarMascota` |
| RF-03 | Consultar expediente con citas y facturas. | `ClinicaService.ObtenerExpediente` |
| RF-04 | Programar citas sin choques de horario. | `ClinicaService.ProgramarCita` |
| RF-05 | Cambiar estado de una cita de forma controlada. | `Cita.Confirmar`, `Atender`, `Cancelar` |
| RF-06 | Calcular precios distintos por servicio y paciente. | `ServicioVeterinario.CalcularPrecio` |
| RF-07 | Generar factura con descuento, IVA y pago. | `FacturacionService` |
| RF-08 | Acumular puntos de fidelidad. | `Cliente.AcumularPuntos` |
| RF-09 | Calcular nómina por cargo. | `Empleado.CalcularSalario` |
| RF-10 | Alertar stock bajo y reponer productos. | `Producto`, `ClinicaService` |
| RF-11 | Mostrar reportes operativos. | `ReporteService` |
| RF-12 | Persistir y recuperar los datos. | `JsonAlmacenClinica` |

### 8.3 Requisitos no funcionales

| Código | Requisito | Cumplimiento |
|---|---|---|
| RNF-01 | Portabilidad | .NET 10; ejecutable en Windows, Linux y macOS. |
| RNF-02 | Mantenibilidad | Separación en capas y nombres del dominio. |
| RNF-03 | Confiabilidad | Guardado atómico y respaldo ante JSON corrupto. |
| RNF-04 | Claridad | Interfaz guiada, mensajes en español y ayuda `--help`. |
| RNF-05 | Verificabilidad | 32 pruebas automatizadas y CI. |
| RNF-06 | Extensibilidad | Contratos e interfaces para sustituir persistencia e interfaz. |
| RNF-07 | Rendimiento | Operación local sobre colecciones en memoria; adecuada para el volumen académico. |

### 8.4 Arquitectura

```text
HuellaVet.Console · presentación
   │
   ├──> HuellaVet.Application · casos de uso y reportes
   │        └──> HuellaVet.Domain · entidades y reglas
   │        └──> IAlmacenClinica · contrato de persistencia
   │
   ├──> HuellaVet.Infrastructure · JSON / memoria
   │        └──> implementa IAlmacenClinica
   │        └──> huellavet.json
   │
   └──> HuellaVet.Laboratorio · Clase 03
```

| Proyecto | Responsabilidad | Depende de |
|---|---|---|
| `HuellaVet.Domain` | Entidades, jerarquías, invariantes y excepciones. | Nada |
| `HuellaVet.Application` | Casos de uso, facturación y reportes. | Domain |
| `HuellaVet.Infrastructure` | JSON, memoria y datos de demostración. | Application, Domain |
| `HuellaVet.Console` | Menús, tablas, flujos y presentación. | Application, Infrastructure, Laboratorio |
| `HuellaVet.Laboratorio` | Clases exactas de la Clase 03. | Nada |
| `HuellaVet.Tests` | Pruebas de dominio, aplicación, persistencia y laboratorio. | Todos los anteriores |

La regla central es que **el dominio no conoce la consola, el archivo JSON ni la base de datos**.

### 8.5 Modelo de clases

El detalle completo, con diagramas Mermaid, está en [`DIAGRAMA_CLASES.md`](DIAGRAMA_CLASES.md). Las jerarquías principales son:

```text
Persona
├── Cliente
└── Empleado
    ├── Veterinario
    ├── Recepcionista
    └── AuxiliarVeterinario

Mascota
├── Perro
├── Gato
├── Ave
└── Conejo

ServicioVeterinario
├── ConsultaGeneral
├── Vacunacion
├── Cirugia
├── Peluqueria
└── Hospitalizacion
```

### 8.6 Decisiones de diseño relevantes

1. **El agregado `ClinicaData` se serializa completo.** Simplifica el avance y evita ciclos de navegación; las relaciones se guardan por identificadores.
2. **Las reglas viven en los objetos.** `Cita` controla sus transiciones; `Factura` calcula y protege su estado; `Producto` valida el stock.
3. **La factura se genera desde una cita atendida.** No se permite facturar una cita programada ni duplicar la factura de una cita.
4. **El precio es polimórfico.** `Factura.AgregarDetalle(servicio, mascota)` invoca `servicio.CalcularPrecio(mascota)` sin preguntar el tipo.
5. **La persistencia es reemplazable.** `IAlmacenClinica` separa la aplicación del mecanismo concreto.
6. **La consola se adapta al contexto.** Cuando la salida está redirigida, no intenta limpiar la pantalla; así funciona `--demo` en CI o en una terminal capturada.
7. **Los errores esperados son de negocio.** `ReglaNegocioException` transporta un mensaje comprensible; la consola lo muestra sin cerrar la aplicación.
8. **La persistencia es intercambiable.** `IAlmacenClinica` permite usar JSON o SQL Server sin cambiar los servicios. La interfaz Windows Forms consume los mismos casos de uso.

### 8.7 Flujo principal: atender y facturar

```text
Recepción programa la cita
  → ClinicaService.ProgramarCita(...)
      valida cliente, mascota, veterinario, servicio y fecha
      detecta choque de veterinario y de mascota (30 min)
      guarda la cita

Veterinario inicia y atiende
  → ClinicaService.IniciarAtencion(citaId)
  → ClinicaService.AtenderCita(citaId, diagnóstico, peso, observaciones)
      Cita.Atender(...) cambia el estado a Atendida
      actualiza peso y última visita de la mascota

Recepción cobra
  → FacturacionService.GenerarFacturaParaCita(citaId, pago, descuento)
      exige cita atendida y ausencia de factura previa
      agrega el detalle con el precio polimórfico
      aplica descuento (máximo 50 %) e IVA (15 %)
      registra el pago y acumula puntos
```

### 8.8 Diseño de base de datos

Además del avance de aplicación, el repositorio incluye el diseño relacional completo de HuellaVet en `docs/bd/`. El modelo sigue las seis fases de la guía del profesor y contiene 26 tablas, 28 relaciones, 5 catálogos, 6 vistas, índices, restricciones y un script ejecutable para SQL Server 2022.

| Fase | Evidencia |
|---|---|
| 1. Requisitos y análisis | `docs/bd/01_requisitos.md` |
| 2. Modelo conceptual | `docs/bd/02_modelo_conceptual.drawio` |
| 3. Modelo lógico | `docs/bd/03_modelo_logico.drawio` |
| 4. Normalización | `docs/bd/04_normalizacion.md` |
| 5. Modelo físico | `docs/bd/05_modelo_fisico.drawio` y `docs/bd/07_diccionario_datos.md` |
| 6. Implementación | `docs/bd/06_huellavet_sqlserver.sql` |

Decisiones principales:

- **Table-per-type** para personas, mascotas y servicios.
- **GUID** como clave primaria, coincidiendo con `Guid` del dominio.
- **Totales no almacenados**; se calculan en `vw_FacturasResumen`.
- **Precio histórico** en `DetallesFactura`.
- **Índice único filtrado** `UX_Facturas_Cita` para garantizar una factura por cita.
- **CHECK** para enumeraciones y rangos.

La aplicación sigue usando JSON en esta entrega; el script SQL es el modelo físico listo para la migración de la Unidad 3.

---

## 9. Implementación

### 9.1 Tamaño y organización

El código fuente comprende **61 archivos C#** y aproximadamente **4,500 líneas** (sin contar `bin` ni `obj`), distribuidos en seis proyectos. La solución se compila con `dotnet build` y se ejecuta con `dotnet run`.

### 9.2 Funcionalidades demostrables

| Módulo | Qué se puede hacer |
|---|---|
| Dashboard | Ver pacientes, citas del día, ingresos del mes, stock bajo, agenda y vacunas próximas. |
| Clientes y mascotas | Registrar, buscar, actualizar contacto, crear mascota y abrir expediente. |
| Agenda | Ver agenda por fecha, programar, confirmar, iniciar, atender, cancelar y marcar inasistencia. |
| Facturación | Listar facturas, ver detalle y facturar una cita atendida con pago y descuento. |
| Servicios | Listar el catálogo, simular precios y actualizar tarifas. |
| Personal | Ver fichas y nómina calculada con polimorfismo. |
| Reportes | Ingresos por servicio, especies, servicios populares, vacunas e ingresos mensuales. |
| Laboratorio | Ejecutar las demostraciones de herencia, polimorfismo y sobrecarga. |
| Configuración | Ver ruta y estado del almacén, exportar una copia del JSON. |

### 9.3 Interfaz de consola

La interfaz usa Spectre.Console para tablas, paneles, reglas y colores. Se fijó un ancho de 120 columnas cuando la terminal lo permite, se configuró la cultura `es-NI` y se muestra la moneda en córdobas (C$).

El modo automático permite evaluar el proyecto sin navegar:

```powershell
dotnet run --project src/HuellaVet.Console -- --demo --reset
```

Opciones disponibles:

```text
--demo           Ejecuta una demostración automática y termina.
--reset          Restablece los datos de demostración.
--data <ruta>    Usa un archivo JSON distinto.
--help, -h       Muestra la ayuda.
--version        Muestra la versión.
```

### 9.4 Persistencia

La primera ejecución crea:

```text
%LOCALAPPDATA%\HuellaVet\huellavet.json
```

El guardado escribe primero un archivo `.tmp` y luego lo mueve sobre el original. Si la carga falla por JSON inválido, se conserva una copia `.corrupto-<fecha>.json` y se informa al usuario. Esta decisión evita perder el archivo anterior ante una escritura interrumpida.

### 9.5 Manejo de errores

Los errores de negocio se lanzan como `ReglaNegocioException` y la consola los presenta como mensajes claros. Entre las validaciones implementadas están:

- Documento de cliente duplicado.
- Correo con formato básico inválido.
- Fecha de nacimiento futura o poco razonable.
- Peso fuera de rango.
- Especie no permitida.
- Cita en el pasado.
- Choque de horario del veterinario o de la mascota.
- Intento de cancelar una cita atendida.
- Factura de cita no atendida o duplicada.
- Descuento superior al 50 %.
- Pago doble o factura sin detalles.
- Stock insuficiente.

---

## 10. Cumplimiento de la Clase 03

La actividad solicitada se implementó en `src/HuellaVet.Laboratorio/Clase03/` con los nombres y miembros exactos. El detalle pedagógico está en [`CLASE_03.md`](CLASE_03.md).

### 10.1 Herencia: selección de fútbol

| Clase | Miembros exigidos | Implementación |
|---|---|---|
| `SeleccionFutbol` | `id`, `Nombre`, `Apellidos`, `Edad`; `Concentrarse()`, `Viajar()` | `Id`, `Nombre`, `Apellidos`, `Edad`; métodos `virtual`. |
| `Futbolista` | `dorsal`, `demarcacion`; `JugarPartido()`, `Entrenar()` | `Dorsal`, `Demarcacion`; métodos propios. |
| `Entrenador` | `idFederacion`; `DirigirPartido()`, `DirigirEntrenamiento()` | `IdFederacion`; métodos propios. |
| `Masajista` | `titulacion`, `aniosExperiencia`; `DarMasaje()` | `Titulacion`, `AniosExperiencia`; método propio. |

```csharp
var equipo = new List<SeleccionFutbol>
{
    new Futbolista { Nombre = "Diego", Apellidos = "Martínez", Dorsal = 10 },
    new Entrenador { Nombre = "Gustavo", Apellidos = "Acevedo", IdFederacion = "FED-2026-018" },
    new Masajista  { Nombre = "Karla", Apellidos = "Ríos", Titulacion = "Fisioterapia" }
};

foreach (var miembro in equipo)
{
    Console.WriteLine(miembro.Concentrarse());
}
```

La lista es de la clase base, pero cada objeto ejecuta su propia versión de `Concentrarse()`.

### 10.2 Polimorfismo: formas geométricas

| Clase | Miembros | Fórmula |
|---|---|---|
| `Forma` | `Area`, `CalculaArea()` | Contrato abstracto. |
| `Circulo` | `Radio` | π × radio² |
| `Rectangulo` | `Base`, `Altura` | base × altura |
| `Cuadrado` | `Lado` | lado² |

```csharp
var formas = new List<Forma>
{
    new Circulo { Radio = 5 },
    new Rectangulo { Base = 6, Altura = 4 },
    new Cuadrado { Lado = 4 }
};

var areas = formas.Select(f => f.CalculaArea()).ToList(); // 78.54, 24.00, 16.00
```

`Cuadrado` hereda directamente de `Forma`, tal como en la ilustración del docente; no se hizo heredar de `Rectangulo` para respetar el diseño solicitado.

### 10.3 Sobrecarga

`CalculadoraPrecios` ofrece cuatro versiones de `Calcular`:

| Llamada | Resultado | Regla |
|---|---:|---|
| `Calcular(500)` | C$ 500.00 | Precio base. |
| `Calcular(500, 10m)` | C$ 450.00 | Descuento del 10 %. |
| `Calcular(500, 3)` | C$ 1,500.00 | Tres unidades. |
| `Calcular(500, 10m, 3)` | C$ 1,350.00 | Descuento y cantidad. |

La misma idea se aplica en el dominio con `ServicioVeterinario.CalcularPrecio`, `Factura.AgregarDetalle` y `FacturacionService.GenerarFacturaParaCita`.

### 10.4 Salida real de la demostración

```text
• Futbolista → Diego Martínez (Edad: 24)
    Concentrarse(): Diego Martínez se concentra para el partido como Mediocampista.
    Viajar():       Diego Martínez viaja en el autobús del equipo.
    JugarPartido(): Diego Martínez juega el partido con el dorsal 10.
    Entrenar():     Diego Martínez entrena la definición y la resistencia.

Tipo          Radio/Base/Lado       Área
────────────────────────────────────────────
Circulo      radio = 5.0                78.54
Rectangulo   6.0 × 4.0                  24.00
Cuadrado     lado = 4.0                 16.00
```

La captura completa está en [`capturas/demo.txt`](capturas/demo.txt).

---

## 11. Pruebas y resultados

### 11.1 Ejecución

```powershell
dotnet test
```

Resultado:

```text
Correctas! - Con error: 0, Superado: 32, Omitido: 0, Total: 32
```

### 11.2 Distribución

| Conjunto | Casos | Qué verifica |
|---|---:|---|
| `ClinicaServiceTests` | 8 | Registro, duplicados, agenda, choques, atención, cancelación, especie y nómina. |
| `FacturacionServiceTests` | 5 | Factura de cita atendida, rechazo de no atendida, duplicados, descuento y puntos. |
| `ReporteServiceTests` | 5 | Resumen, especies, ingresos, vacunas y últimos seis meses. |
| `FacturaTests` | 5 | Subtotal, descuento, IVA, total, pago doble y detalle manual. |
| `Clase03Tests` | 8 | Herencia, polimorfismo, áreas y sobrecarga (incluye dos casos teóricos de círculo). |
| `JsonAlmacenClinicaTests` | 1 | Ida y vuelta conservando tipos derivados y relaciones. |
| **Total** | **32** | **0 fallos** |

### 11.3 Matriz de trazabilidad

| Requisito | Clase / método | Prueba |
|---|---|---|
| Documento único | `ClinicaService.RegistrarCliente` | `RegistrarCliente_ConDocumentoDuplicado_LanzaReglaDeNegocio` |
| Fecha futura | `ClinicaService.ProgramarCita` | `ProgramarCita_ConFechaPasada_LanzaReglaDeNegocio` |
| Choque de agenda | `ClinicaService.ProgramarCita` | `ProgramarCita_ConChoqueDeVeterinario_LanzaReglaDeNegocio` |
| Transición de cita | `Cita.Atender` | `AtenderCita_CambiaEstadoYActualizaPeso` |
| Cancelación inválida | `Cita.Cancelar` | `CancelarCita_Atendida_LanzaReglaDeNegocio` |
| Especie válida | `ClinicaService.RegistrarMascota` | `RegistrarMascota_ConEspecieInvalida_LanzaReglaDeNegocio` |
| Nómina polimórfica | `Empleado.CalcularSalario` | `CalcularNominaTotal_SumaElSalarioPolimorfico` |
| Factura desde cita | `FacturacionService` | `GenerarFacturaParaCita_Atendida_GeneraFacturaPagada` |
| Factura duplicada | `FacturacionService` | `GenerarFacturaParaCita_DosVeces_LanzaReglaDeNegocio` |
| Puntos | `Cliente.AcumularPuntos` | `GenerarFactura_AcumulaPuntosAlCliente` |
| Reportes | `ReporteService` | cinco pruebas de reportes |
| Persistencia polimórfica | `JsonAlmacenClinica` | `GuardarYCargar_ConservaTiposDerivadosYRelaciones` |
| Clase 03 | `SeleccionFutbol`, `Forma`, `CalculadoraPrecios` | ocho casos de `Clase03Tests` |

### 11.4 Integración continua

El flujo `.github/workflows/ci.yml` restaura, compila en Release y ejecuta las pruebas en Ubuntu con .NET 10. Esto permite verificar cada push y cada pull request.

### 11.5 Pruebas manuales complementarias

Además de xUnit, se ejecutó el modo `--demo` y se recorrieron los menús de dashboard, clientes, agenda, facturación, servicios, personal, reportes y configuración. La salida de la demostración se conserva como evidencia en `docs/capturas/demo.txt`.

---

## 12. Resultados

La ejecución de `--demo --reset` produjo, entre otros, estos resultados:

- **7 pacientes** registrados y **4 empleados activos**.
- **4 citas** en la agenda del día y **6 citas pendientes**.
- **C$ 9,325.06** de ingresos del mes.
- **2 productos** con stock bajo.
- Factura `HV-2026-0001` con subtotal C$ 650.00, IVA C$ 97.50 y total C$ 747.50.
- Cliente con **91 puntos** de fidelidad.
- Laboratorio Clase 03 con las tres demostraciones y resultados correctos.

Estos valores provienen de los datos de ejemplo y pueden cambiar si el usuario modifica la información. El sistema los recalcula a partir del estado persistido, no de valores fijos en la interfaz.

### 12.1 Logros frente a los objetivos

| Objetivo | Resultado |
|---|---|
| Modelar el dominio | Cumplido: personas, mascotas, servicios, citas, facturas, productos y agregado. |
| Aplicar herencia | Cumplido en personas, mascotas, servicios y Clase 03. |
| Aplicar polimorfismo | Cumplido con métodos abstractos, virtuales y listas de la base. |
| Aplicar sobrecarga | Cumplido en precios, detalles de factura y facturación. |
| Casos de uso | Cumplido para clientes, agenda, atención, cobro, personal, inventario y reportes. |
| Persistencia | Cumplido con JSON atómico y respaldo. |
| Interfaz | Cumplido con consola guiada y modo automático. |
| Clase 03 exacta | Cumplido con nombres y miembros solicitados. |
| Pruebas | Cumplido: 32 pruebas aprobadas. |
| Documentación | Cumplido: avance, arquitectura, diagramas, Clase 03, guion y rúbrica. |

---

## 13. Conclusiones

El avance demuestra que la programación orientada a objetos no consiste en memorizar definiciones, sino en distribuir responsabilidades. La jerarquía de servicios permite agregar una nueva tarifa sin reescribir la facturación; la jerarquía de mascotas permite calcular dosis y fichas sin preguntar la especie; la jerarquía de empleados permite calcular la nómina con reglas distintas desde una sola llamada.

La separación por capas fue decisiva. El dominio se puede probar sin consola; la aplicación se puede probar sin archivos; la persistencia se puede sustituir. Esa independencia es la principal lección técnica del proyecto.

También se aprendió que un sistema académico debe cuidar la experiencia: mensajes claros, datos de ejemplo coherentes, una demostración automática y documentación que permita al docente encontrar cada requisito. La calidad no está solo en que compile, sino en que otra persona pueda entenderlo, ejecutarlo y evaluarlo.

El resultado es un avance funcional, defendible y extensible. No está terminado como producto comercial, pero sí cumple el objetivo del curso: aplicar herencia, polimorfismo y sobrecarga en un problema con sentido.

---

## 14. Trabajo futuro

1. **Interfaz gráfica completa.** Ampliar Windows Forms con CRUD completo, validaciones visuales y reportes exportables.
2. **SQL Server y ADO.NET.** Ya existe `SqlAlmacenClinica`; falta completar CRUD, transacciones y repositorios por entidad.
3. **CRUD completo.** Altas, bajas y cambios de todas las entidades, con auditoría.
4. **Autenticación y roles.** Recepción, veterinario y administrador, con permisos diferenciados.
5. **Reportes exportables.** PDF y Excel, además de gráficos históricos.
6. **Recordatorios.** Correo o SMS para vacunas y citas.
7. **Recetas y medicamentos.** Catálogo, dosis, inventario y prescripción.
8. **Pruebas de interfaz y de integración.** Flujos completos de extremo a extremo.
9. **Accesibilidad y localización.** Mensajes y formatos configurables.
10. **Despliegue.** Instalador o publicación autocontenida para equipos de la clínica.

---

## 15. Bibliografía

Gamma, E., Helm, R., Johnson, R. y Vlissides, J. (1994). *Design Patterns: Elements of Reusable Object-Oriented Software*. Addison-Wesley. ISBN 0-201-63361-2.

Martin, R. C. (2017). *Clean Architecture: A Craftsman's Guide to Software Structure and Design*. Prentice Hall. ISBN 978-0-13-449416-6.

Microsoft. (2026). *C# documentation*. https://learn.microsoft.com/dotnet/csharp/

Microsoft. (2026). *Inheritance in C#*. https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/inheritance

Microsoft. (2026). *Polymorphism in C#*. https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/polymorphism

Microsoft. (2026). *Member overloading*. https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/members

Microsoft. (2026). *How to serialize properties of derived classes with System.Text.Json*. https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/polymorphism

Microsoft. (2026). *Unit testing C# with xUnit*. https://learn.microsoft.com/dotnet/core/testing/unit-testing-with-dotnet-test

Pressman, R. S. y Maxim, B. R. (2020). *Software Engineering: A Practitioner's Approach* (9.ª ed.). McGraw-Hill. ISBN 978-1-259-87297-6.

Sommerville, I. (2016). *Software Engineering* (10.ª ed.). Pearson. ISBN 978-0-13-394303-0.

Spectre.Console. (2026). *Spectre.Console documentation*. https://spectreconsole.net/

---

## 16. Anexos

- [`README.md`](../README.md) - presentación, instalación y comandos.
- [`bd/README.md`](bd/README.md) - diseño de base de datos, fases 1 a 6.
- [`ARQUITECTURA.md`](ARQUITECTURA.md) - capas, dependencias y decisiones.
- [`DIAGRAMA_CLASES.md`](DIAGRAMA_CLASES.md) — diagramas Mermaid del dominio y la Clase 03.
- [`CLASE_03.md`](CLASE_03.md) — correspondencia exacta con las ilustraciones del docente.
- [`GUION_DEFENSA.md`](GUION_DEFENSA.md) — recorrido de exposición y preguntas probables.
- [`RUBRICA.md`](RUBRICA.md) — checklist de cumplimiento.
- [`capturas/demo.txt`](capturas/demo.txt) — salida real de la demostración.

---

**Declaración de originalidad.** El código, los datos de demostración y la documentación fueron elaborados como parte del proyecto académico de la asignatura. Las fuentes externas consultadas se citan en la bibliografía.
