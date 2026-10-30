# Diagramas de clases

Los diagramas usan [Mermaid](https://mermaid.js.org/). GitHub y Visual Studio Code los renderizan directamente.

---

## 1. Personas

```mermaid
classDiagram
    direction TB

    class Persona {
        <<abstract>>
        +Guid Id
        +string Nombre
        +string Apellido
        +string Documento
        +string Telefono
        +string Email
        +string NombreCompleto
        +string Rol
        +Presentacion() string
    }

    class Cliente {
        +string Direccion
        +DateTime FechaRegistro
        +int PuntosFidelidad
        +string Rol
        +AcumularPuntos(decimal) int
    }

    class Empleado {
        <<abstract>>
        +decimal SalarioBase
        +DateTime FechaIngreso
        +bool Activo
        +string Cargo
        +CalcularSalario() decimal
        +FichaTecnica() string
    }

    class Veterinario {
        +string Especialidad
        +string NumeroColegiatura
        +int AniosExperiencia
        +string Cargo
        +CalcularSalario() decimal
        +AtenderPaciente(string) string
    }

    class Recepcionista {
        +string Turno
        +string Cargo
        +CalcularSalario() decimal
    }

    class AuxiliarVeterinario {
        +string Certificacion
        +string Cargo
        +CalcularSalario() decimal
    }

    Persona <|-- Cliente
    Persona <|-- Empleado
    Empleado <|-- Veterinario
    Empleado <|-- Recepcionista
    Empleado <|-- AuxiliarVeterinario
```

**Conceptos:** herencia, clase abstracta, propiedad abstracta (`Rol`), método virtual (`CalcularSalario`) y polimorfismo.

---

## 2. Pacientes

```mermaid
classDiagram
    direction TB

    class Mascota {
        <<abstract>>
        +Guid Id
        +string Nombre
        +string Raza
        +DateTime FechaNacimiento
        +decimal PesoKg
        +SexoMascota Sexo
        +Guid ClienteId
        +string Alergias
        +string Notas
        +bool Esterilizado
        +DateTime? UltimaVisita
        +string Especie
        +string Sonido()
        +decimal FactorDosis
        +int EdadAnios
        +string EdadTexto
        +string Ficha() string
        +decimal CalcularDosis(decimal) decimal
        +string EstadoClinico() string
    }

    class Perro {
        +TallaPerro Talla
        +bool Entrenado
        +string Especie
        +decimal FactorDosis
        +string Sonido() string
        +string Ficha() string
    }

    class Gato {
        +bool EsIndoor
        +string Especie
        +decimal FactorDosis
        +string Sonido() string
        +string Ficha() string
    }

    class Ave {
        +bool PuedeVolar
        +string ColorPlumaje
        +string Especie
        +decimal FactorDosis
        +string Sonido() string
        +string Ficha() string
    }

    class Conejo {
        +decimal LargoOrejasCm
        +string Especie
        +decimal FactorDosis
        +string Sonido() string
        +string Ficha() string
    }

    Mascota <|-- Perro
    Mascota <|-- Gato
    Mascota <|-- Ave
    Mascota <|-- Conejo
```

**Conceptos:** propiedad abstracta (`Especie`, `FactorDosis`), método abstracto (`Sonido`), método virtual (`Ficha`) y polimorfismo en `CalcularDosis`.

---

## 3. Servicios

```mermaid
classDiagram
    direction TB

    class ServicioVeterinario {
        <<abstract>>
        +Guid Id
        +string Codigo
        +string Nombre
        +decimal PrecioBase
        +int DuracionMinutos
        +bool RequiereAyuno
        +bool Activo
        +string Categoria
        +CalcularPrecio(Mascota) decimal
        +CalcularPrecio(Mascota, decimal) decimal
        +CalcularPrecio(Mascota, int) decimal
        +CalcularPrecio(Mascota, decimal, int) decimal
        +Descripcion() string
    }

    class ConsultaGeneral {
        +bool IncluyeReceta
        +string Categoria
        +CalcularPrecio(Mascota) decimal
    }

    class Vacunacion {
        +string Vacuna
        +int ProximaDosisMeses
        +string Categoria
        +CalcularPrecio(Mascota) decimal
    }

    class Cirugia {
        +NivelRiesgo Riesgo
        +bool RequiereHospitalizacion
        +string Categoria
        +CalcularPrecio(Mascota) decimal
    }

    class Peluqueria {
        +TipoCorte Tipo
        +bool IncluyeBano
        +string Categoria
        +CalcularPrecio(Mascota) decimal
    }

    class Hospitalizacion {
        +int Dias
        +decimal CostoPorDia
        +string Categoria
        +CalcularPrecio(Mascota) decimal
    }

    ServicioVeterinario <|-- ConsultaGeneral
    ServicioVeterinario <|-- Vacunacion
    ServicioVeterinario <|-- Cirugia
    ServicioVeterinario <|-- Peluqueria
    ServicioVeterinario <|-- Hospitalizacion
```

**Conceptos:** método abstracto (`CalcularPrecio`), sobrecarga (cuatro versiones) y polimorfismo puro: una lista de `ServicioVeterinario` calcula precios distintos sin preguntar el tipo.

---

## 4. Citas y facturación

```mermaid
classDiagram
    direction LR

    class Cita {
        +Guid Id
        +Guid ClienteId
        +Guid MascotaId
        +Guid VeterinarioId
        +Guid ServicioId
        +DateTime FechaHora
        +string Motivo
        +EstadoCita Estado
        +string? Diagnostico
        +decimal? PesoRegistrado
        +DateTime? FechaAtencion
        +bool EstaActiva
        +bool PuedeFacturarse
        +Confirmar() void
        +IniciarAtencion() void
        +Atender(string, decimal, string?, DateTime?) void
        +Cancelar(string) void
        +MarcarNoAsistio() void
        +Reprogramar(DateTime) void
    }

    class Factura {
        +Guid Id
        +string Numero
        +Guid ClienteId
        +Guid? CitaId
        +DateTime Fecha
        +List~DetalleFactura~ Detalles
        +decimal DescuentoPorcentaje
        +decimal ImpuestoPorcentaje
        +bool Pagada
        +MetodoPago? MetodoPago
        +decimal Subtotal
        +decimal Descuento
        +decimal Impuesto
        +decimal Total
        +AgregarDetalle(ServicioVeterinario, Mascota, int) void
        +AgregarDetalle(string, decimal, int) void
        +AplicarDescuento(decimal) void
        +RegistrarPago(MetodoPago) void
    }

    class DetalleFactura {
        +Guid ServicioId
        +string Descripcion
        +int Cantidad
        +decimal PrecioUnitario
        +decimal Subtotal
    }

    class ClinicaData {
        +List~Cliente~ Clientes
        +List~Empleado~ Empleados
        +List~Mascota~ Mascotas
        +List~ServicioVeterinario~ Servicios
        +List~Cita~ Citas
        +List~Factura~ Facturas
        +List~Producto~ Productos
    }

    ClinicaData *-- "0..*" Cita
    ClinicaData *-- "0..*" Factura
    Cita --> Factura : genera
    Factura *-- "1..*" DetalleFactura
```

**Conceptos:** encapsulación del estado (`Estado` solo cambia con métodos), composición (`Factura` contiene detalles), cálculos derivados (`Total` no se asigna) y sobrecarga (`AgregarDetalle`).

---

## 5. Laboratorio Clase 03 · Herencia

```mermaid
classDiagram
    direction TB

    class SeleccionFutbol {
        +int Id
        +string Nombre
        +string Apellidos
        +int Edad
        +Concentrarse() string
        +Viajar() string
    }

    class Futbolista {
        +int Dorsal
        +string Demarcacion
        +Concentrarse() string
        +Viajar() string
        +JugarPartido() string
        +Entrenar() string
    }

    class Entrenador {
        +string IdFederacion
        +Concentrarse() string
        +Viajar() string
        +DirigirPartido() string
        +DirigirEntrenamiento() string
    }

    class Masajista {
        +string Titulacion
        +int AniosExperiencia
        +Concentrarse() string
        +Viajar() string
        +DarMasaje() string
    }

    SeleccionFutbol <|-- Futbolista
    SeleccionFutbol <|-- Entrenador
    SeleccionFutbol <|-- Masajista
```

---

## 6. Laboratorio Clase 03 · Polimorfismo

```mermaid
classDiagram
    direction TB

    class Forma {
        <<abstract>>
        +double Area
        +CalculaArea() double
    }

    class Circulo {
        +double Radio
        +CalculaArea() double
    }

    class Rectangulo {
        +double Base
        +double Altura
        +CalculaArea() double
    }

    class Cuadrado {
        +double Lado
        +CalculaArea() double
    }

    Forma <|-- Circulo
    Forma <|-- Rectangulo
    Forma <|-- Cuadrado
```

> `Cuadrado` hereda directamente de `Forma`, tal como aparece en la ilustración del profesor. No se hizo heredar de `Rectangulo` para respetar el diseño solicitado.
