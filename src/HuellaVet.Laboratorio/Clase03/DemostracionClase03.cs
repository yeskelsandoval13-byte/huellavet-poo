using System.Globalization;

namespace HuellaVet.Laboratorio.Clase03;

/// <summary>
/// Guiones de demostración para la defensa. Devuelven líneas de texto
/// para que la capa de consola las presente con su propio formato.
/// </summary>
public static class DemostracionClase03
{
    public static IReadOnlyList<string> Herencia()
    {
        var equipo = new List<SeleccionFutbol>
        {
            new Futbolista
            {
                Id = 1,
                Nombre = "Diego",
                Apellidos = "Martínez",
                Edad = 24,
                Dorsal = 10,
                Demarcacion = "Mediocampista"
            },
            new Entrenador
            {
                Id = 2,
                Nombre = "Gustavo",
                Apellidos = "Acevedo",
                Edad = 45,
                IdFederacion = "FED-2026-018"
            },
            new Masajista
            {
                Id = 3,
                Nombre = "Karla",
                Apellidos = "Ríos",
                Edad = 31,
                Titulacion = "Fisioterapia deportiva",
                AniosExperiencia = 7
            }
        };

        var lineas = new List<string>
        {
            "List<SeleccionFutbol> equipo = new() { Futbolista, Entrenador, Masajista };",
            string.Empty
        };

        foreach (var miembro in equipo)
        {
            lineas.Add($"• {miembro.GetType().Name} → {miembro.Nombre} {miembro.Apellidos} (Edad: {miembro.Edad})");
            lineas.Add($"    Concentrarse(): {miembro.Concentrarse()}");
            lineas.Add($"    Viajar():       {miembro.Viajar()}");

            switch (miembro)
            {
                case Futbolista futbolista:
                    lineas.Add($"    JugarPartido(): {futbolista.JugarPartido()}");
                    lineas.Add($"    Entrenar():     {futbolista.Entrenar()}");
                    break;
                case Entrenador entrenador:
                    lineas.Add($"    DirigirPartido():       {entrenador.DirigirPartido()}");
                    lineas.Add($"    DirigirEntrenamiento(): {entrenador.DirigirEntrenamiento()}");
                    break;
                case Masajista masajista:
                    lineas.Add($"    DarMasaje(): {masajista.DarMasaje()}");
                    break;
            }

            lineas.Add(string.Empty);
        }

        lineas.Add("La lista es de tipo SeleccionFutbol, pero cada objeto conserva su comportamiento real.");
        lineas.Add("Eso es herencia + polimorfismo trabajando juntos.");
        return lineas;
    }

    public static IReadOnlyList<string> Polimorfismo()
    {
        var formas = new List<Forma>
        {
            new Circulo { Radio = 5 },
            new Rectangulo { Base = 6, Altura = 4 },
            new Cuadrado { Lado = 4 }
        };

        var lineas = new List<string>
        {
            "List<Forma> formas = new() { Circulo(5), Rectangulo(6×4), Cuadrado(4) };",
            string.Empty,
            "Tipo          Radio/Base/Lado       Área",
            "────────────────────────────────────────────"
        };

        foreach (var forma in formas)
        {
            var area = forma.CalculaArea();
            var detalle = forma switch
            {
                Circulo c => $"radio = {c.Radio:N1}",
                Rectangulo r => $"{r.Base:N1} × {r.Altura:N1}",
                Cuadrado c => $"lado = {c.Lado:N1}",
                _ => "—"
            };

            lineas.Add($"{forma.GetType().Name,-12} {detalle,-21} {area,10:N2}");
        }

        lineas.Add(string.Empty);
        lineas.Add("El bucle solo conoce Forma.CalculaArea(); cada clase decide su fórmula.");
        lineas.Add("No hay if/switch por tipo: el polimorfismo resuelve el comportamiento.");
        return lineas;
    }

    public static IReadOnlyList<string> Sobrecarga()
    {
        var calculadora = new CalculadoraPrecios();
        var cultura = CultureInfo.GetCultureInfo("es-NI");

        return
        [
            "La sobrecarga ocurre en tiempo de compilación: mismo nombre, distintos parámetros.",
            string.Empty,
            $"Calcular(500)              → C$ {calculadora.Calcular(500m).ToString("N2", cultura)}",
            $"Calcular(500, 10m)         → C$ {calculadora.Calcular(500m, 10m).ToString("N2", cultura)}  (descuento del 10 %)",
            $"Calcular(500, 3)           → C$ {calculadora.Calcular(500m, 3).ToString("N2", cultura)}  (3 servicios)",
            $"Calcular(500, 10m, 3)      → C$ {calculadora.Calcular(500m, 10m, 3).ToString("N2", cultura)}  (descuento + cantidad)",
            string.Empty,
            "En HuellaVet se usa sobrecarga en:",
            "  • ServicioVeterinario.CalcularPrecio(mascota), (mascota, descuento), (mascota, cantidad).",
            "  • Factura.AgregarDetalle(servicio, mascota) y AgregarDetalle(descripción, precio, cantidad).",
            "  • FacturacionService.GenerarFacturaParaCita(cita, pago) y (cita, pago, descuento)."
        ];
    }
}
