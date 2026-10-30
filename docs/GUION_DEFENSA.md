# Guion de defensa · HuellaVet

**Duración objetivo:** 7 minutos  
**Formato:** demostración en vivo + explicación breve del diseño  
**Regla de oro:** mostrar, no recitar. Cada concepto debe verse funcionando.

---

## 1. Antes de empezar (checklist de 2 minutos)

- [ ] Abrir una terminal en la carpeta `HuellaVet`.
- [ ] Ejecutar `dotnet build` y confirmar que compila.
- [ ] Ejecutar `dotnet test` y confirmar **32 pruebas correctas**.
- [ ] Ejecutar `dotnet run --project src/HuellaVet.Console -- --demo --reset` una vez.
- [ ] Cerrar la aplicación y dejarla lista para abrir el menú interactivo.
- [ ] Tener abiertos en el editor:
  - `src/HuellaVet.Laboratorio/Clase03/SeleccionFutbol.cs`
  - `src/HuellaVet.Laboratorio/Clase03/Formas.cs`
  - `src/HuellaVet.Domain/Servicios/ServicioVeterinario.cs`
  - `src/HuellaVet.Domain/Facturacion/Factura.cs`
- [ ] Tener a mano `docs/capturas/demo.txt` por si falla la terminal.
- [ ] Fuente de la terminal legible (16–18 pt) y ventana maximizada.
- [ ] Cerrar notificaciones y programas que puedan robar el foco.

> Si el docente pide repositorio: mostrar el `README.md` y la pestaña de Actions con el CI en verde.

---

## 2. Estructura de los 7 minutos

| Tiempo | Bloque | Qué se muestra | Idea que debe quedar |
|---|---|---|---|
| 0:00–0:40 | Apertura | Portada del README | Problema y nombre del proyecto. |
| 0:40–1:20 | Problema y objetivos | README / AVANCE | La clínica necesita ordenar su operación. |
| 1:20–2:10 | Arquitectura | Diagrama del README | Capas y separación de responsabilidades. |
| 2:10–3:20 | Demo de gestión | Dashboard, clientes, agenda | El sistema funciona de punta a punta. |
| 3:20–4:10 | Facturación | Factura de ejemplo | Cálculo y reglas de negocio. |
| 4:10–5:40 | Clase 03 | Laboratorio | Herencia, polimorfismo y sobrecarga exactos. |
| 5:40–6:25 | Calidad | `dotnet test`, CI | 32 pruebas y arquitectura verificable. |
| 6:25–7:00 | Cierre | Conclusiones | Lo logrado y lo que sigue. |

---

## 3. Guion palabra por palabra

### 0:00–0:40 · Apertura

> “Buenos días. Mi proyecto se llama **HuellaVet** y es un sistema de gestión para una clínica veterinaria. La frase que lo resume es ‘Cuidamos cada huella’. No es solo un CRUD: es un avance funcional construido alrededor de objetos que representan personas, mascotas, servicios, citas y facturas.”

Mostrar el README con el logo y la tabla de módulos.

### 0:40–1:20 · Problema y objetivos

> “La clínica trabaja con información dispersa: dueños, mascotas, vacunas, citas, pagos e inventario. Eso produce duplicados, choques de agenda y facturas inconsistentes. El objetivo fue modelar ese problema con programación orientada a objetos, aplicando herencia, polimorfismo y sobrecarga, y dejarlo demostrable con una consola, persistencia y pruebas.”

Señalar en el README:

- Módulos implementados.
- Sección de la Clase 03.
- Estado del proyecto.

### 1:20–2:10 · Arquitectura

> “La solución tiene seis proyectos. El dominio no conoce la consola ni el archivo JSON: solo contiene entidades y reglas. La aplicación orquesta los casos de uso. La infraestructura guarda en JSON. La consola presenta. El laboratorio contiene la Clase 03 y las pruebas verifican todo.”

Mostrar el diagrama de arquitectura del README.

> “La dependencia importante apunta hacia el dominio. Si mañana cambio la consola por WinForms o el JSON por SQL Server, las reglas no se reescriben.”

### 2:10–3:20 · Demo de gestión

Ejecutar:

```powershell
dotnet run --project src/HuellaVet.Console
```

Entrar a **Dashboard**.

> “El dashboard resume la operación: pacientes, citas del día, ingresos del mes, stock bajo, agenda y próximas vacunas. Todo se calcula desde los datos, no está escrito a mano.”

Entrar a **Clientes y mascotas → Ver clientes**.

> “Cada cliente tiene sus mascotas y su expediente. El expediente reúne citas y facturas.”

Entrar a **Agenda de citas → Ver agenda de hoy**.

> “La agenda muestra hora, paciente, dueño, servicio, veterinario y estado. Si intento programar dos citas con el mismo veterinario a menos de 30 minutos, el sistema lanza una regla de negocio y no permite el choque.”

No es necesario completar un registro en vivo si el tiempo aprieta; basta con entrar a la pantalla y señalar la validación.

### 3:20–4:10 · Facturación

Volver al menú y entrar a **Atención y facturación → Ver facturas → primera factura**.

> “La factura se genera desde una cita ya atendida. Calcula subtotal, descuento, IVA del 15 % y total. No se puede facturar dos veces la misma cita ni pagar una factura sin detalles. El total no se asigna desde fuera: se deriva de los detalles.”

Señalar:

- `HV-2026-0001`.
- Subtotal C$ 650.00.
- IVA C$ 97.50.
- Total C$ 747.50.
- Puntos de fidelidad.

> “El detalle usa el precio polimórfico del servicio: la factura no pregunta si es consulta, cirugía o vacunación; llama a `CalcularPrecio(mascota)`.”

### 4:10–5:40 · Clase 03

Entrar a **Laboratorio POO (Clase 03)** o ejecutar:

```powershell
dotnet run --project src/HuellaVet.Console -- --demo
```

Mostrar la sección de **herencia**.

> “`SeleccionFutbol` tiene id, nombre, apellidos y edad, con `Concentrarse` y `Viajar`. `Futbolista`, `Entrenador` y `Masajista` heredan y agregan sus datos. La lista es de tipo `SeleccionFutbol`, pero cada objeto ejecuta su propia versión. Eso demuestra herencia y polimorfismo al mismo tiempo.”

Mostrar la sección de **polimorfismo**.

> “`Forma` declara `Area` y `CalculaArea()` como abstracto. `Circulo`, `Rectangulo` y `Cuadrado` implementan su fórmula. El bucle solo conoce `Forma`; no hay un `if` por tipo. El resultado es 78.54, 24 y 16.”

Mostrar la sección de **sobrecarga**.

> “`CalculadoraPrecios` tiene cuatro versiones de `Calcular`: precio base, con descuento, con cantidad y con ambas. El compilador elige según los argumentos. En el dominio se repite con `CalcularPrecio`, `AgregarDetalle` y `GenerarFacturaParaCita`.”

Abrir `SeleccionFutbol.cs` y `Formas.cs` si el docente pide ver el código:

> “Los nombres y miembros coinciden con la ilustración. Solo se aplicó PascalCase de C# a las propiedades.”

### 5:40–6:25 · Calidad

Ejecutar:

```powershell
dotnet test
```

> “Son 32 pruebas: dominio, facturación, reportes, persistencia y Clase 03. Todas pasan. La persistencia tiene una prueba de ida y vuelta que conserva los tipos derivados con el discriminador `$tipo`. El repositorio incluye un flujo de GitHub Actions que compila y prueba en cada push.”

Mostrar la salida:

```text
Correctas! - Con error: 0, Superado: 32, Omitido: 0, Total: 32
```

### 6:25–7:00 · Cierre

> “En resumen: HuellaVet demuestra herencia con personas, mascotas y servicios; polimorfismo con precios, salarios y formas; y sobrecarga con cálculos y facturas. La arquitectura separa dominio, aplicación, infraestructura y consola. Lo que sigue es la interfaz gráfica, SQL Server y autenticación. Gracias.”

---

## 4. Recorrido alternativo de 3 minutos

Si el docente interrumpe o el tiempo se reduce:

1. **0:00–0:30:** problema y arquitectura (diagrama).
2. **0:30–1:15:** `--demo` completo; señalar dashboard, factura y Clase 03.
3. **1:15–2:15:** abrir `SeleccionFutbol.cs`, `Formas.cs` y `CalculadoraPrecios.cs`; explicar `virtual`/`override`, abstracto y sobrecarga.
4. **2:15–2:45:** `dotnet test` y salida de 32 pruebas.
5. **2:45–3:00:** conclusión y siguiente fase.

---

## 5. Preguntas probables y respuestas

### 5.1 Sobre los conceptos

**¿Qué es herencia?**
Es el mecanismo por el cual una clase deriva de otra, recibe sus miembros y puede especializarlos. En el proyecto, `Veterinario` hereda de `Empleado` y `Empleado` de `Persona`.

**¿Qué es polimorfismo?**
Es la capacidad de tratar objetos de tipos distintos mediante una referencia común y obtener el comportamiento del objeto real. `List<ServicioVeterinario>` y `List<Forma>` son los ejemplos más claros.

**¿Diferencia entre sobrecarga y sobrescritura?**
La sobrecarga tiene el mismo nombre con distintos parámetros y se resuelve al compilar. La sobrescritura redefine un método virtual en una clase derivada y se resuelve al ejecutar.

**¿Qué es una clase abstracta?**
Una clase que no se instancia y que puede declarar miembros abstractos. Sirve como contrato parcial. `Forma` obliga a implementar `CalculaArea()`.

**¿Qué diferencia hay entre `abstract` y `virtual`?**
`abstract` no tiene implementación y obliga a sobrescribir; `virtual` tiene implementación y permite sobrescribir. `SeleccionFutbol.Concentrarse()` es virtual; `Forma.CalculaArea()` es abstracto.

**¿Para qué sirve `override`?**
Para reemplazar la implementación virtual de la base y participar en el despacho dinámico. Sin `override`, el método no se ejecutaría de forma polimórfica.

**¿Qué pasa si olvido `override`?**
El compilador advierte que el método oculta al de la base. Si se usa `new`, el resultado depende del tipo de la variable, no del objeto real. Es un error conceptual frecuente.

**¿Por qué `Cuadrado` no hereda de `Rectangulo`?**
Porque la ilustración lo muestra como hijo directo de `Forma`. Además, un cuadrado con base y altura independientes podría quedar en un estado inconsistente. Se respetó el diseño del docente.

### 5.2 Sobre el proyecto

**¿Por qué una consola y no WinForms?**
Porque el objetivo del avance es demostrar el dominio y la lógica, no la interfaz. La consola es portable, rápida y permite enseñar todos los flujos sin depender de un entorno gráfico. La arquitectura ya separa la presentación para cambiarla después.

**¿Por qué JSON y no SQL Server?**
Porque la unidad de acceso a datos se trabaja más adelante. JSON permite ejecutar el proyecto en cualquier equipo sin instalar un motor. La interfaz `IAlmacenClinica` deja preparada la sustitución por SQL Server.

**¿Cómo se evita que se dupliquen las facturas?**
`FacturacionService` verifica si ya existe una factura con el mismo `CitaId` y lanza `ReglaNegocioException` antes de crear otra.

**¿Cómo se calcula el total de la factura?**
`Subtotal` suma los detalles; `Descuento` aplica el porcentaje; `Impuesto` aplica el 15 % sobre el subtotal menos descuento; `Total` suma subtotal menos descuento más impuesto. Son propiedades calculadas.

**¿Cómo se detecta un choque de agenda?**
Se comparan las citas activas del veterinario y de la mascota; si la diferencia absoluta es menor a 30 minutos, se rechaza.

**¿Qué pasa si el JSON se daña?**
Se copia el archivo a `.corrupto-<fecha>.json`, se informa al usuario y se inicia con datos nuevos. El guardado usa un archivo temporal para no dejar el original a medias.

**¿Cómo se conservan los tipos derivados al guardar?**
Con `[JsonPolymorphic]` y `[JsonDerivedType]`. El JSON incluye `$tipo`, por ejemplo `"perro"`, y al cargar se reconstruye `Perro`, no `Mascota`.

**¿Qué pasa si el cliente no existe?**
Los servicios lanzan `ReglaNegocioException` con un mensaje claro. La consola lo captura y lo muestra sin cerrar la aplicación.

**¿Cuántas pruebas hay y qué cubren?**
32 pruebas: 8 de Clínica, 5 de Facturación, 5 de Reportes, 5 de Factura, 8 de Clase 03 y 1 de persistencia.

**¿Qué falta para que sea un sistema comercial?**
Interfaz gráfica, base de datos, autenticación, auditoría, respaldo, concurrencia, reportes exportables y pruebas de integración.

**¿Qué mejorarías si tuvieras más tiempo?**
Separaría `IAlmacenClinica` en repositorios por entidad, agregaría SQL Server, una interfaz web y pruebas de extremo a extremo. También agregaría recordatorios de vacunas.

**¿Por qué el proyecto se llama HuellaVet?**
Porque “huella” representa la identidad de cada mascota y el lema “Cuidamos cada huella” resume la idea de un sistema centrado en el paciente.

---

## 6. Preguntas que conviene no improvisar

Si no sabes una respuesta, no inventes. Usa una de estas salidas:

- “Ese caso no está implementado en esta entrega; está en la lista de trabajo futuro.”
- “La decisión fue X por Y; si el requisito cambiara, lo ajustaría en Z.”
- “Puedo mostrarte dónde está en el código y qué prueba lo cubre.”

El docente valora más la honestidad y la capacidad de ubicar el código que una respuesta inventada.

---

## 7. Plan de contingencia

### Si la consola no abre

1. Ejecutar `dotnet run --project src/HuellaVet.Console -- --demo`.
2. Si falla, abrir `docs/capturas/demo.txt` y explicar la salida.
3. Si falla la compilación, ejecutar `dotnet build` y leer el error; no entrar en pánico.
4. Tener el repositorio abierto en GitHub como respaldo.

### Si no hay internet

El proyecto no necesita internet para ejecutarse. Solo se necesita para clonar o para ver GitHub Actions; la demo y las pruebas son locales.

### Si el docente pide modificar algo en vivo

Cambios seguros y demostrables:

- Agregar un servicio nuevo (por ejemplo, `ConsultaEspecializada`) heredando de `ServicioVeterinario`.
- Agregar una especie nueva (por ejemplo, `Hurón`) heredando de `Mascota`.
- Agregar una sobrecarga a `CalculadoraPrecios`.
- Cambiar el precio base de un servicio desde la pantalla de servicios.

No intentes refactorizaciones grandes en vivo.

---

## 8. Frases que conviene memorizar

- “El dominio no conoce la consola ni el archivo: esa es la separación clave.”
- “La lista es de la clase base, pero el objeto real decide el comportamiento.”
- “No hay un `if` por tipo: el polimorfismo resuelve la fórmula.”
- “La sobrecarga se decide al compilar; la sobrescritura, al ejecutar.”
- “El total no se asigna: se deriva de los detalles.”
- “32 pruebas, 0 fallos.”
- “Esto es un avance funcional, no una maqueta.”

---

## 9. Cierre de una frase

> “HuellaVet demuestra que un buen diseño orientado a objetos permite que el código hable el lenguaje de la clínica: cada mascota sabe su especie, cada servicio su precio y cada empleado su salario.”
