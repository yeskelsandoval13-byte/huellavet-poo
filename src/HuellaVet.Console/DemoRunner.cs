using HuellaVet.Application.Services;
using HuellaVet.Laboratorio.Clase03;
using Spectre.Console;

namespace HuellaVet.ConsoleUI;

/// <summary>
/// Recorrido automático para demostrar el avance sin escribir en el menú.
/// Ideal para una exposición o para revisar que todo funciona.
/// </summary>
public static class DemoRunner
{
    public static void Ejecutar(
        ClinicaService clinica,
        FacturacionService facturacion,
        ReporteService reportes)
    {
        UI.Limpiar();
        UI.Cabecera();

        AnsiConsole.MarkupLine("[bold]Demostración automática del avance[/]");
        AnsiConsole.MarkupLine("[grey]Datos de ejemplo: pacientes, agenda, facturas e inventario.[/]");
        AnsiConsole.WriteLine();

        UI.Titulo("1. Dashboard", "Resumen operativo y agenda del día.");
        Pages.DashboardPage.Mostrar(clinica, reportes, pausar: false);

        UI.Titulo("2. Factura de ejemplo", "Cálculo de subtotal, descuento, IVA y total.");
        var factura = facturacion.ObtenerFacturas().FirstOrDefault();
        if (factura is null)
        {
            UI.Info("No hay facturas en los datos actuales.");
        }
        else
        {
            Pages.FacturacionPage.MostrarFactura(factura, clinica, pausar: false);
        }

        UI.Titulo("3. Laboratorio Clase 03 · Herencia", "SeleccionFutbol y sus clases hijas.");
        UI.MostrarLineas(DemostracionClase03.Herencia());

        UI.Titulo("4. Laboratorio Clase 03 · Polimorfismo", "Forma y sus clases hijas.");
        UI.MostrarLineas(DemostracionClase03.Polimorfismo());

        UI.Titulo("5. Laboratorio Clase 03 · Sobrecarga", "Mismo método, distintos parámetros.");
        UI.MostrarLineas(DemostracionClase03.Sobrecarga());

        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new Rule("[bold green]Fin de la demostración[/]")
                .RuleStyle("green")
                .LeftJustified());

        AnsiConsole.MarkupLine(
            "[grey]Ejecuta sin --demo para abrir el menú interactivo y probar los flujos completos.[/]");
    }
}
