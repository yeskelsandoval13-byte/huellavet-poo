# Clase 03 · Herencia, polimorfismo y sobrecarga

Este documento explica cómo HuellaVet resuelve **exactamente** la actividad de la Clase 03 y cómo los mismos conceptos se aplican al dominio de la clínica.

- Código: `src/HuellaVet.Laboratorio/Clase03/`
- Demostración: menú **Laboratorio POO (Clase 03)** o `dotnet run --project src/HuellaVet.Console -- --demo`
- Pruebas: `tests/HuellaVet.Tests/Laboratorio/Clase03Tests.cs`

---

## 1. Requerimiento del docente

La actividad pide trasladar a C# dos ilustraciones:

### 1.1 Herencia

```text
SeleccionFutbol
├── Futbolista
├── Entrenador
└── Masajista
```

| Clase | Datos | Acciones |
|---|---|---|
| `SeleccionFutbol` | `id`, `Nombre`, `Apellidos`, `Edad` | `Concentrarse()`, `Viajar()` |
| `Futbolista` | `dorsal`, `demarcacion` | `JugarPartido()`, `Entrenar()` |
| `Entrenador` | `idFederacion` | `DirigirPartido()`, `DirigirEntrenamiento()` |
| `Masajista` | `titulacion`, `aniosExperiencia` | `DarMasaje()` |

### 1.2 Polimorfismo

```text
Forma
├── Circulo
├── Rectangulo
└── Cuadrado
```

| Clase | Datos | Método |
|---|---|---|
| `Forma` | `Area` | `CalculaArea()` |
| `Circulo` | radio | `CalculaArea()` |
| `Rectangulo` | base, altura | `CalculaArea()` |
| `Cuadrado` | lado | `CalculaArea()` |

### 1.3 Sobrecarga

La actividad también trabaja la sobrecarga de métodos: varias versiones del mismo método con distintos parámetros. En el laboratorio se resuelve con `CalculadoraPrecios.Calcular(...)`.

---

## 2. Correspondencia exacta

| Requerimiento | Archivo | Implementación |
|---|---|---|
| `SeleccionFutbol` con datos comunes | `SeleccionFutbol.cs` | Clase base con `Id`, `Nombre`, `Apellidos`, `Edad`. |
| `Concentrarse()` y `Viajar()` | `SeleccionFutbol.cs` | Métodos `virtual` para permitir especialización. |
| `Futbolista` | `SeleccionFutbol.cs` | `Dorsal`, `Demarcacion`, `JugarPartido()`, `Entrenar()`. |
| `Entrenador` | `SeleccionFutbol.cs` | `IdFederacion`, `DirigirPartido()`, `DirigirEntrenamiento()`. |
| `Masajista` | `SeleccionFutbol.cs` | `Titulacion`, `AniosExperiencia`, `DarMasaje()`. |
| `Forma` | `Formas.cs` | Clase abstracta con `Area` y `CalculaArea()`. |
| `Circulo` | `Formas.cs` | `Radio` y fórmula π × radio². |
| `Rectangulo` | `Formas.cs` | `Base`, `Altura` y fórmula base × altura. |
| `Cuadrado` | `Formas.cs` | `Lado` y fórmula lado². |
| Sobrecarga | `CalculadoraPrecios.cs` | Cuatro versiones de `Calcular`. |

> **Nota sobre los nombres:** la ilustración usa `id`, `dorsal`, `demarcacion`, etc. En C# las propiedades públicas se escriben en PascalCase (`Id`, `Dorsal`, `Demarcacion`). Es la misma información con la convención del lenguaje; no se cambió ningún miembro ni se agregó otro obligatorio.

---

## 3. Herencia

### 3.1 Clase base

```csharp
public class SeleccionFutbol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int Edad { get; set; }

    public virtual string Concentrarse() =>
        $"{Nombre} {Apellidos} se concentra con la selección.";

    public virtual string Viajar() =>
        $"{Nombre} {Apellidos} viaja con la selección.";
}
```

`SeleccionFutbol` concentra lo común: identificación, nombre y edad. Los métodos son `virtual` porque cada tipo de miembro se concentra y viaja de forma distinta.

### 3.2 Clases derivadas

```csharp
public class Futbolista : SeleccionFutbol
{
    public int Dorsal { get; set; }
    public string Demarcacion { get; set; } = string.Empty;

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} se concentra para el partido como {Demarcacion}.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja en el autobús del equipo.";

    public string JugarPartido() =>
        $"{Nombre} {Apellidos} juega el partido con el dorsal {Dorsal}.";

    public string Entrenar() =>
        $"{Nombre} {Apellidos} entrena la definición y la resistencia.";
}
```

`Entrenador` y `Masajista` siguen el mismo patrón. Cada uno hereda los cuatro datos comunes y agrega los suyos.

### 3.3 ¿Qué demuestra?

1. **Reutilización:** no se repiten `Nombre`, `Apellidos`, `Edad` ni los métodos comunes.
2. **Especialización:** cada clase derivada agrega datos y acciones propias.
3. **Sobrescritura:** `Concentrarse()` y `Viajar()` cambian según el rol.
4. **Relación “es un”:** un `Futbolista` **es un** `SeleccionFutbol`.

### 3.4 Uso polimórfico

```csharp
var equipo = new List<SeleccionFutbol>
{
    new Futbolista { Nombre = "Diego", Apellidos = "Martínez", Dorsal = 10, Demarcacion = "Mediocampista" },
    new Entrenador { Nombre = "Gustavo", Apellidos = "Acevedo", IdFederacion = "FED-2026-018" },
    new Masajista  { Nombre = "Karla", Apellidos = "Ríos", Titulacion = "Fisioterapia deportiva", AniosExperiencia = 7 }
};

foreach (var miembro in equipo)
{
    Console.WriteLine(miembro.Concentrarse());
}
```

Aunque la lista es `List<SeleccionFutbol>`, en tiempo de ejecución se invoca la versión de `Futbolista`, `Entrenador` o `Masajista` según el objeto real. Eso es **polimorfismo de subtipo**.

---

## 4. Polimorfismo

### 4.1 Clase abstracta

```csharp
public abstract class Forma
{
    public double Area { get; protected set; }

    public abstract double CalculaArea();
}
```

`Forma` no se puede instanciar porque no existe una forma “genérica” con área conocida. `CalculaArea()` es abstracto: obliga a cada clase derivada a dar su fórmula.

`Area` tiene `protected set` para que solo la clase base o sus derivadas puedan asignarla. El usuario de una forma lee el resultado, pero no lo falsifica.

### 4.2 Implementaciones

```csharp
public sealed class Circulo : Forma
{
    public double Radio { get; set; }

    public override double CalculaArea()
    {
        Area = Math.Round(Math.PI * Radio * Radio, 2);
        return Area;
    }
}

public sealed class Rectangulo : Forma
{
    public double Base { get; set; }
    public double Altura { get; set; }

    public override double CalculaArea()
    {
        Area = Math.Round(Base * Altura, 2);
        return Area;
    }
}

public sealed class Cuadrado : Forma
{
    public double Lado { get; set; }

    public override double CalculaArea()
    {
        Area = Math.Round(Lado * Lado, 2);
        return Area;
    }
}
```

`Cuadrado` hereda directamente de `Forma` porque así aparece en la ilustración. Aunque geométricamente un cuadrado es un rectángulo, hacerlo heredar de `Rectangulo` habría cambiado el diseño solicitado y introducido una discusión sobre sustitución de Liskov que no era el objetivo de la clase.

### 4.3 Uso polimórfico

```csharp
var formas = new List<Forma>
{
    new Circulo { Radio = 5 },
    new Rectangulo { Base = 6, Altura = 4 },
    new Cuadrado { Lado = 4 }
};

foreach (var forma in formas)
{
    Console.WriteLine($"{forma.GetType().Name}: {forma.CalculaArea():N2}");
}
```

Salida:

```text
Circulo: 78.54
Rectangulo: 24.00
Cuadrado: 16.00
```

El bucle solo conoce `Forma.CalculaArea()`. No hay `if (forma is Circulo)` para calcular. Cada objeto decide su fórmula; eso es polimorfismo.

### 4.4 Diferencia con la sobrecarga

| Pregunta | Polimorfismo (sobrescritura) | Sobrecarga |
|---|---|---|
| ¿Dónde se decide? | En ejecución, según el objeto real. | En compilación, según los argumentos. |
| ¿Requiere herencia? | Sí. | No. |
| ¿Firma del método? | Igual nombre y parámetros; cambia la implementación. | Igual nombre; cambian los parámetros. |
| Ejemplo | `forma.CalculaArea()` | `Calcular(500, 10m)` |

---

## 5. Sobrecarga

```csharp
public sealed class CalculadoraPrecios
{
    public decimal Calcular(decimal precioBase);
    public decimal Calcular(decimal precioBase, decimal descuentoPorcentaje);
    public decimal Calcular(decimal precioBase, int cantidad);
    public decimal Calcular(decimal precioBase, decimal descuentoPorcentaje, int cantidad);
}
```

El compilador distingue las versiones por la cantidad y el tipo de argumentos:

| Llamada | Versión elegida | Resultado |
|---|---|---:|
| `Calcular(500m)` | Una cifra. | 500.00 |
| `Calcular(500m, 10m)` | Precio y descuento `decimal`. | 450.00 |
| `Calcular(500m, 3)` | Precio y cantidad `int`. | 1,500.00 |
| `Calcular(500m, 10m, 3)` | Precio, descuento y cantidad. | 1,350.00 |

### 5.1 Cuidado con la ambigüedad

`Calcular(500m, 10)` y `Calcular(500m, 10m)` son llamadas distintas: la segunda usa la versión de descuento y la primera la de cantidad. El sufijo `m` indica `decimal`. Esta es una decisión de diseño que debe explicarse en la defensa.

En el dominio de la clínica se aplica la misma idea:

```csharp
servicio.CalcularPrecio(mascota);
servicio.CalcularPrecio(mascota, 10m);       // descuento
servicio.CalcularPrecio(mascota, 3);         // cantidad
servicio.CalcularPrecio(mascota, 10m, 3);    // ambas
```

---

## 6. Demostración en vivo

### 6.1 Desde el menú

```text
Menú principal → Laboratorio POO (Clase 03)
```

El módulo muestra un mapa de conceptos y permite ejecutar:

1. Herencia: selección de fútbol.
2. Polimorfismo: formas geométricas.
3. Sobrecarga: calculadora de precios.

### 6.2 Desde la terminal

```powershell
dotnet run --project src/HuellaVet.Console -- --demo
```

La sección 3, 4 y 5 de la salida corresponde a la Clase 03. La captura completa está en [`capturas/demo.txt`](capturas/demo.txt).

### 6.3 Salida esperada de la herencia

```text
List<SeleccionFutbol> equipo = new() { Futbolista, Entrenador, Masajista };

• Futbolista → Diego Martínez (Edad: 24)
    Concentrarse(): Diego Martínez se concentra para el partido como Mediocampista.
    Viajar():       Diego Martínez viaja en el autobús del equipo.
    JugarPartido(): Diego Martínez juega el partido con el dorsal 10.
    Entrenar():     Diego Martínez entrena la definición y la resistencia.

• Entrenador → Gustavo Acevedo (Edad: 45)
    Concentrarse(): Gustavo Acevedo revisa la estrategia con el cuerpo técnico.
    Viajar():       Gustavo Acevedo viaja con el cuerpo técnico.
    DirigirPartido():       Gustavo Acevedo dirige el partido desde la banca.
    DirigirEntrenamiento(): Gustavo Acevedo dirige el entrenamiento táctico.

• Masajista → Karla Ríos (Edad: 31)
    Concentrarse(): Karla Ríos prepara la camilla y los vendajes.
    Viajar():       Karla Ríos viaja con el equipo médico.
    DarMasaje(): Karla Ríos da un masaje de recuperación (Fisioterapia deportiva, 7 años de experiencia).
```

---

## 7. Pruebas de la Clase 03

Archivo: `tests/HuellaVet.Tests/Laboratorio/Clase03Tests.cs`.

| Prueba | Qué comprueba |
|---|---|
| `Futbolista_HeredaDeSeleccionFutbol` | La relación de herencia y los métodos propios. |
| `ListaDeSeleccionFutbol_EjecutaComportamientoPolimorfico` | Tres objetos de la base producen tres mensajes distintos. |
| `Circulo_CalculaArea` (2 casos) | Área con radio 5 y 10, y asignación de `Area`. |
| `Rectangulo_CalculaArea` | 6 × 4 = 24. |
| `Cuadrado_CalculaArea` | 4² = 16. |
| `ListaDeFormas_CalculaAreaSinConocerElTipoConcreto` | La lista de `Forma` devuelve 78.54, 24 y 16. |
| `Sobrecarga_ResuelveSegunLosArgumentos` | Las cuatro versiones de `Calcular`. |

Resultado: **8 casos aprobados** dentro de las 32 pruebas del proyecto.

---

## 8. Preguntas probables del docente

**¿Por qué `SeleccionFutbol` no es abstracta?**
La ilustración la presenta como una clase concreta con datos y comportamiento comunes. En el dominio de la clínica sí se usan clases abstractas (`Persona`, `Mascota`, `ServicioVeterinario`) porque allí no tiene sentido instanciar la base. Se respetó la ilustración en el laboratorio y se aplicó un diseño más rico en el proyecto.

**¿Qué pasaría si `Concentrarse()` no fuera `virtual`?**
La lista ejecutaría siempre la versión de `SeleccionFutbol`; se perdería el comportamiento específico. Si se usara `new` en las clases hijas, se ocultaría el método y el resultado dependería del tipo de la variable, no del objeto.

**¿Por qué `CalculaArea()` es abstracto y no virtual?**
Porque no existe una fórmula base razonable para “forma genérica”. La clase base solo puede definir el contrato; cada forma concreta debe implementarlo.

**¿Por qué `Area` no es una propiedad calculada?**
La ilustración la presenta como un atributo. Se implementó como propiedad con `protected set`, asignada por `CalculaArea()`, para conservar el miembro y evitar que se modifique desde fuera.

**¿Por qué `Cuadrado` no hereda de `Rectangulo`?**
Porque la ilustración muestra las tres clases como hijas directas de `Forma`. Además, un cuadrado con setters independientes de base y altura podría quedar en un estado inválido; el diseño solicitado evita ese problema.

**¿La sobrecarga es polimorfismo?**
Sí, es polimorfismo en tiempo de compilación. La sobrescritura es polimorfismo en tiempo de ejecución. Conviene explicar ambos y no confundirlos.

**¿Dónde se usa esto en HuellaVet?**
Herencia: `Persona` → `Empleado` → `Veterinario`; `Mascota` → `Perro`, `Gato`, `Ave`, `Conejo`; `ServicioVeterinario` → cinco servicios. Polimorfismo: `CalcularSalario()`, `CalcularPrecio(mascota)`, `Sonido()`, `Ficha()`. Sobrecarga: `CalcularPrecio`, `AgregarDetalle`, `GenerarFacturaParaCita`.

---

## 9. Cómo compilar y probar solo esta parte

```powershell
# Compilar la solución
dotnet build

# Ejecutar todas las pruebas
dotnet test

# Ejecutar solo las pruebas de la Clase 03
dotnet test --filter "FullyQualifiedName~Clase03Tests"

# Ver la demostración
dotnet run --project src/HuellaVet.Console -- --demo
```

---

## 10. Resumen de una frase

**La herencia comparte lo común, el polimorfismo decide en tiempo de ejecución y la sobrecarga decide en tiempo de compilación; HuellaVet usa los tres para que el código exprese el negocio en lugar de llenarse de condicionales.**
