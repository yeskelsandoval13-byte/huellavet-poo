using System.Globalization;
using HuellaVet.Application.Services;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class ReportesPage
{
    public static void Mostrar(ReporteService reportes, ClinicaService clinica)
    {
        UI.Titulo("Reportes", "Indicadores para tomar decisiones sobre la clínica.");

        var resumen = reportes.ObtenerResumen();
        var grid = new Grid();
        for (var i = 0; i < 4; i++)
        {
            grid.AddColumn();
        }

        grid.AddRow(
            Metrica("Pacientes", resumen.Pacientes.ToString()),
            Metrica("Citas pendientes", resumen.CitasPendientes.ToString()),
            Metrica("Ingresos del mes", UI.Moneda(resumen.IngresosMes)),
            Metrica("Stock bajo", resumen.StockBajo.ToString()));

        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();

        AnsiConsole.Write(new Rule("[bold]Ingresos por servicio[/]").LeftJustified().RuleStyle("grey"));
        var ingresos = reportes.IngresosPorServicio();
        if (ingresos.Count == 0)
        {
            UI.Info("Sin facturas pagadas todavía.");
        }
        else
        {
            var tabla = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey).Expand();
            tabla.AddColumn("Servicio");
            tabla.AddColumn("Cantidad");
            tabla.AddColumn("Ingreso");

            foreach (var item in ingresos)
            {
                tabla.AddRow(item.Servicio, item.Cantidad.ToString(), UI.Moneda(item.Total));
            }

            AnsiConsole.Write(tabla);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold]Pacientes por especie[/]").LeftJustified().RuleStyle("grey"));
        var especies = reportes.PacientesPorEspecie();
        if (especies.Count == 0)
        {
            UI.Info("Sin pacientes registrados.");
        }
        else
        {
            var tabla = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey).Expand();
            tabla.AddColumn("Especie");
            tabla.AddColumn("Pacientes");

            foreach (var item in especies)
            {
                tabla.AddRow(item.Especie, item.Cantidad.ToString());
            }

            AnsiConsole.Write(tabla);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold]Servicios más solicitados[/]").LeftJustified().RuleStyle("grey"));
        var populares = reportes.ServiciosMasSolicitados();
        if (populares.Count == 0)
        {
            UI.Info("Sin citas registradas.");
        }
        else
        {
            var tabla = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey).Expand();
            tabla.AddColumn("Servicio");
            tabla.AddColumn("Citas");
            tabla.AddColumn("Ingresos");

            foreach (var item in populares)
            {
                tabla.AddRow(item.Servicio, item.Citas.ToString(), UI.Moneda(item.Ingresos));
            }

            AnsiConsole.Write(tabla);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold]Próximas vacunas (45 días)[/]").LeftJustified().RuleStyle("grey"));
        var vacunas = reportes.ProximasVacunas(45);
        if (vacunas.Count == 0)
        {
            UI.Info("No hay vacunas próximas.");
        }
        else
        {
            var tabla = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey).Expand();
            tabla.AddColumn("Fecha");
            tabla.AddColumn("Paciente");
            tabla.AddColumn("Dueño");
            tabla.AddColumn("Vacuna");
            tabla.AddColumn("Días");

            foreach (var item in vacunas)
            {
                tabla.AddRow(
                    item.Fecha.ToString("dd/MM/yyyy"),
                    item.Mascota,
                    item.Dueno,
                    item.Vacuna,
                    item.Dias.ToString());
            }

            AnsiConsole.Write(tabla);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold]Ingresos de los últimos 6 meses[/]").LeftJustified().RuleStyle("grey"));
        var meses = reportes.IngresosUltimosMeses();
        var tablaMeses = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey).Expand();
        tablaMeses.AddColumn("Mes");
        tablaMeses.AddColumn("Ingresos");

        foreach (var (mes, total) in meses)
        {
            tablaMeses.AddRow(
                mes.ToString("MMMM yyyy", CultureInfo.CurrentCulture),
                UI.Moneda(total));
        }

        AnsiConsole.Write(tablaMeses);

        UI.Pausa();
    }

    private static Panel Metrica(string titulo, string valor) =>
        new Panel(new Markup($"[bold]{valor.EscapeMarkup()}[/]\n[grey]{titulo.EscapeMarkup()}[/]"))
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();
}
