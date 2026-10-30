using HuellaVet.Application.Services;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class FacturacionPage
{
    public static void Ejecutar(FacturacionService facturacion, ClinicaService clinica)
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Atención y facturación", "Cobros, descuentos e historial de ingresos.");

            var opcion = UI.SeleccionarTexto("¿Qué deseas hacer?", [
                "Ver todas las facturas",
                "Ver detalle de una factura",
                "Facturar una cita atendida",
                "Volver al menú principal"
            ]);

            try
            {
                switch (opcion)
                {
                    case "Ver todas las facturas":
                        Listar(facturacion, clinica);
                        break;
                    case "Ver detalle de una factura":
                        VerDetalle(facturacion, clinica);
                        break;
                    case "Facturar una cita atendida":
                        FacturarCita(facturacion, clinica);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex) when (ex is HuellaVet.Domain.Common.ReglaNegocioException)
            {
                UI.Error(ex.Message);
                UI.Pausa();
            }
        }
    }

    private static void Listar(FacturacionService facturacion, ClinicaService clinica)
    {
        var facturas = facturacion.ObtenerFacturas();

        if (facturas.Count == 0)
        {
            UI.Info("Todavía no hay facturas registradas.");
            UI.Pausa();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        tabla.AddColumn("Número");
        tabla.AddColumn("Fecha");
        tabla.AddColumn("Cliente");
        tabla.AddColumn("Subtotal");
        tabla.AddColumn("IVA");
        tabla.AddColumn("Total");
        tabla.AddColumn("Estado");

        foreach (var factura in facturas)
        {
            var cliente = clinica.ObtenerCliente(factura.ClienteId);
            tabla.AddRow(
                factura.Numero,
                factura.Fecha.ToString("dd/MM/yyyy"),
                cliente?.NombreCompleto ?? "—",
                UI.Moneda(factura.Subtotal),
                UI.Moneda(factura.Impuesto),
                UI.Moneda(factura.Total),
                factura.Pagada ? "[green]Pagada[/]" : "[yellow]Pendiente[/]");
        }

        AnsiConsole.Write(tabla);
        UI.Pausa();
    }

    private static void VerDetalle(FacturacionService facturacion, ClinicaService clinica)
    {
        var facturas = facturacion.ObtenerFacturas();
        if (facturas.Count == 0)
        {
            UI.Info("No hay facturas registradas.");
            UI.Pausa();
            return;
        }

        var factura = UI.Seleccionar(
            "Selecciona la factura",
            facturas,
            f =>
            {
                var cliente = clinica.ObtenerCliente(f.ClienteId);
                return $"{f.Numero} · {f.Fecha:dd/MM/yyyy} · {cliente?.NombreCompleto ?? "—"} · {UI.Moneda(f.Total)}";
            });

        MostrarFactura(factura, clinica);
    }

    private static void FacturarCita(
        FacturacionService facturacion,
        ClinicaService clinica)
    {
        var citasPendientes = clinica.ObtenerCitas()
            .Where(c => c.Estado == EstadoCita.Atendida)
            .Where(c => facturacion.ObtenerFacturaDeCita(c.Id) is null)
            .ToList();

        if (citasPendientes.Count == 0)
        {
            UI.Info("No hay citas atendidas pendientes de facturar.");
            UI.Pausa();
            return;
        }

        var cita = UI.Seleccionar(
            "Selecciona la cita atendida",
            citasPendientes,
            c =>
            {
                var mascota = clinica.ObtenerMascota(c.MascotaId);
                var servicio = clinica.ObtenerServicio(c.ServicioId);
                return $"{c.FechaHora:dd/MM/yyyy HH:mm} · {mascota?.Nombre ?? "—"} · {servicio?.Nombre ?? "—"}";
            });

        var metodo = UI.Seleccionar(
            "Método de pago",
            Enum.GetValues<MetodoPago>(),
            m => m.ToString());

        var descuento = UI.PedirDecimal("Descuento (%)", 0m, 50m, 0m);

        var factura = facturacion.GenerarFacturaParaCita(cita.Id, metodo, descuento);

        UI.Exito($"Factura {factura.Numero} generada por {UI.Moneda(factura.Total)}.");
        MostrarFactura(factura, clinica, pausar: false);
        UI.Pausa();
    }

    public static void MostrarFactura(
        Factura factura,
        ClinicaService clinica,
        bool pausar = true)
    {
        var cliente = clinica.ObtenerCliente(factura.ClienteId);
        var estado = factura.Pagada ? "[green]PAGADA[/]" : "[yellow]PENDIENTE[/]";

        var encabezado = new Panel(
                new Markup(
                    $"[bold]Cliente:[/] {cliente?.NombreCompleto.EscapeMarkup() ?? "—"}\n" +
                    $"[grey]Documento:[/] {cliente?.Documento.EscapeMarkup() ?? "—"}   " +
                    $"[grey]Fecha:[/] {factura.Fecha:dd/MM/yyyy HH:mm}\n" +
                    $"[grey]Estado:[/] {estado}   " +
                    $"[grey]Pago:[/] {(factura.MetodoPago?.ToString() ?? "—")}"))
            .Header($"[bold]Factura {factura.Numero.EscapeMarkup()}[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        AnsiConsole.Write(encabezado);
        AnsiConsole.WriteLine();

        if (factura.Detalles.Count == 0)
        {
            UI.Info("La factura no tiene detalles.");
        }
        else
        {
            var tabla = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .Expand();

            tabla.AddColumn("Descripción");
            tabla.AddColumn("Cant.");
            tabla.AddColumn("Precio unit.");
            tabla.AddColumn("Subtotal");

            foreach (var detalle in factura.Detalles)
            {
                tabla.AddRow(
                    detalle.Descripcion,
                    detalle.Cantidad.ToString(),
                    UI.Moneda(detalle.PrecioUnitario),
                    UI.Moneda(detalle.Subtotal));
            }

            AnsiConsole.Write(tabla);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[grey]Subtotal:[/] {UI.Moneda(factura.Subtotal),24}");
        AnsiConsole.MarkupLine($"[grey]Descuento ({factura.DescuentoPorcentaje:N0} %):[/] {UI.Moneda(factura.Descuento),24}");
        AnsiConsole.MarkupLine($"[grey]IVA ({factura.ImpuestoPorcentaje:N0} %):[/] {UI.Moneda(factura.Impuesto),24}");
        AnsiConsole.Write(new Rule().RuleStyle("grey").LeftJustified());
        AnsiConsole.MarkupLine($"[bold green]TOTAL:[/] [bold green]{UI.Moneda(factura.Total),24}[/]");

        if (cliente is not null)
        {
            AnsiConsole.MarkupLine($"[grey]Puntos de fidelidad del cliente:[/] {cliente.PuntosFidelidad}");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Gracias por confiar en HuellaVet. Cuidamos cada huella.[/]");

        if (pausar)
        {
            UI.Pausa();
        }
    }
}
