using HuellaVet.Application.Services;
using HuellaVet.Domain.Citas;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class DashboardPage
{
    public static void Mostrar(ClinicaService clinica, ReporteService reportes, bool pausar = true)
    {
        UI.Titulo("Dashboard", "Una mirada rápida a la operación de la clínica.");

        var resumen = reportes.ObtenerResumen();

        var grid = new Grid();
        for (var i = 0; i < 4; i++)
        {
            grid.AddColumn();
        }

        grid.AddRow(
            Metrica("Pacientes activos", resumen.Pacientes.ToString(), Color.Teal),
            Metrica("Citas de hoy", resumen.CitasHoy.ToString(), Color.Blue),
            Metrica("Ingresos del mes", UI.Moneda(resumen.IngresosMes), Color.Green),
            Metrica("Stock bajo", resumen.StockBajo.ToString(), Color.Orange1));

        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine(
            $"[grey]Citas pendientes:[/] [bold]{resumen.CitasPendientes}[/]   " +
            $"[grey]Empleados activos:[/] [bold]{resumen.EmpleadosActivos}[/]");

        AnsiConsole.WriteLine();
        MostrarAgendaHoy(clinica);

        AnsiConsole.WriteLine();
        MostrarVacunas(reportes);

        AnsiConsole.WriteLine();
        MostrarStockBajo(clinica);

        if (pausar)
        {
            UI.Pausa();
        }
    }

    private static Panel Metrica(string titulo, string valor, Color color)
    {
        return new Panel(
                new Markup($"[bold]{valor.EscapeMarkup()}[/]\n[grey]{titulo.EscapeMarkup()}[/]"))
            .Border(BoxBorder.Rounded)
            .BorderColor(color)
            .Expand();
    }

    private static void MostrarAgendaHoy(ClinicaService clinica)
    {
        AnsiConsole.Write(
            new Rule("[bold]Agenda de hoy[/]")
                .LeftJustified()
                .RuleStyle("grey"));

        var citas = clinica.ObtenerCitasHoy();

        if (citas.Count == 0)
        {
            UI.Info("No hay citas registradas para hoy.");
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .Expand();

        tabla.AddColumn("Hora");
        tabla.AddColumn("Paciente");
        tabla.AddColumn("Dueño");
        tabla.AddColumn("Servicio");
        tabla.AddColumn("Veterinario");
        tabla.AddColumn("Estado");

        foreach (var cita in citas)
        {
            var mascota = clinica.ObtenerMascota(cita.MascotaId);
            var dueno = clinica.ObtenerCliente(cita.ClienteId);
            var servicio = clinica.ObtenerServicio(cita.ServicioId);
            var veterinario = clinica.ObtenerEmpleados().FirstOrDefault(e => e.Id == cita.VeterinarioId);

            tabla.AddRow(
                cita.FechaHora.ToString("HH:mm"),
                mascota?.Nombre ?? "—",
                dueno?.NombreCompleto ?? "—",
                servicio?.Nombre ?? "—",
                veterinario?.NombreCompleto ?? "—",
                UI.Estado(cita.Estado));
        }

        AnsiConsole.Write(tabla);
    }

    private static void MostrarVacunas(ReporteService reportes)
    {
        AnsiConsole.Write(
            new Rule("[bold]Próximas vacunas (45 días)[/]")
                .LeftJustified()
                .RuleStyle("grey"));

        var vacunas = reportes.ProximasVacunas(45);

        if (vacunas.Count == 0)
        {
            UI.Info("No hay vacunas próximas en los siguientes 45 días.");
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .Expand();

        tabla.AddColumn("Fecha");
        tabla.AddColumn("Paciente");
        tabla.AddColumn("Dueño");
        tabla.AddColumn("Vacuna");
        tabla.AddColumn("Días");

        foreach (var vacuna in vacunas)
        {
            tabla.AddRow(
                vacuna.Fecha.ToString("dd/MM/yyyy"),
                vacuna.Mascota,
                vacuna.Dueno,
                vacuna.Vacuna,
                vacuna.Dias == 0 ? "[orange1]Hoy[/]" : vacuna.Dias.ToString());
        }

        AnsiConsole.Write(tabla);
    }

    private static void MostrarStockBajo(ClinicaService clinica)
    {
        AnsiConsole.Write(
            new Rule("[bold]Inventario con stock bajo[/]")
                .LeftJustified()
                .RuleStyle("grey"));

        var productos = clinica.ObtenerProductosStockBajo();

        if (productos.Count == 0)
        {
            UI.Exito("Inventario sin alertas.");
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Orange1)
            .Expand();

        tabla.AddColumn("Producto");
        tabla.AddColumn("Stock");
        tabla.AddColumn("Mínimo");
        tabla.AddColumn("Precio");

        foreach (var producto in productos)
        {
            tabla.AddRow(
                producto.Nombre,
                $"[red]{producto.Stock}[/]",
                producto.StockMinimo.ToString(),
                UI.Moneda(producto.Precio));
        }

        AnsiConsole.Write(tabla);
    }
}
