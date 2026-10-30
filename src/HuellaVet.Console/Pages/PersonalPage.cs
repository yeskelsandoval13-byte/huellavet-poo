using HuellaVet.Application.Services;
using HuellaVet.Domain.Personas;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class PersonalPage
{
    public static void Mostrar(ClinicaService clinica)
    {
        UI.Titulo("Personal y nómina", "Cada cargo calcula su salario con su propia regla.");

        var empleados = clinica.ObtenerEmpleados();

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        tabla.AddColumn("Colaborador");
        tabla.AddColumn("Cargo");
        tabla.AddColumn("Detalle");
        tabla.AddColumn("Ingreso");
        tabla.AddColumn("Salario calculado");
        tabla.AddColumn("Activo");

        foreach (var empleado in empleados)
        {
            var detalle = empleado switch
            {
                Veterinario v => $"{v.Especialidad} · {v.AniosExperiencia} años",
                Recepcionista r => $"Turno {r.Turno}",
                AuxiliarVeterinario a => a.Certificacion,
                _ => "—"
            };

            tabla.AddRow(
                empleado.NombreCompleto,
                empleado.Cargo,
                detalle,
                empleado.FechaIngreso.ToString("MM/yyyy"),
                UI.Moneda(empleado.CalcularSalario()),
                empleado.Activo ? "[green]Sí[/]" : "[red]No[/]");
        }

        AnsiConsole.Write(tabla);

        var total = clinica.CalcularNominaTotal();
        var panel = new Panel(
                new Markup(
                    $"[bold]Nómina mensual estimada:[/] [bold green]{UI.Moneda(total)}[/]\n" +
                    "[grey]El total se obtiene recorriendo la lista de Empleado y llamando CalcularSalario().[/]\n" +
                    "[grey]Veterinario, Recepcionista y Auxiliar aplican bonos distintos: eso es polimorfismo.[/]"))
            .Header("[bold]Resumen de nómina[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Green)
            .Expand();

        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel);
        UI.Pausa();
    }
}
