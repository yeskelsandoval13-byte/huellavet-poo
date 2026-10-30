using HuellaVet.Application.Services;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class ServiciosPage
{
    public static void Ejecutar(ClinicaService clinica)
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Servicios y precios", "Tarifas que se adaptan al paciente y al tipo de servicio.");

            var opcion = UI.SeleccionarTexto("¿Qué deseas hacer?", [
                "Ver catálogo de servicios",
                "Simular precio para una mascota",
                "Actualizar precio base",
                "Volver al menú principal"
            ]);

            try
            {
                switch (opcion)
                {
                    case "Ver catálogo de servicios":
                        Listar(clinica);
                        break;
                    case "Simular precio para una mascota":
                        Simular(clinica);
                        break;
                    case "Actualizar precio base":
                        Actualizar(clinica);
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

    private static void Listar(ClinicaService clinica)
    {
        var servicios = clinica.ObtenerServicios(soloActivos: false);

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        tabla.AddColumn("Código");
        tabla.AddColumn("Servicio");
        tabla.AddColumn("Categoría");
        tabla.AddColumn("Precio base");
        tabla.AddColumn("Duración");
        tabla.AddColumn("Activo");

        foreach (var servicio in servicios)
        {
            tabla.AddRow(
                servicio.Codigo,
                servicio.Nombre,
                servicio.Categoria,
                UI.Moneda(servicio.PrecioBase),
                $"{servicio.DuracionMinutos} min",
                servicio.Activo ? "[green]Sí[/]" : "[red]No[/]");
        }

        AnsiConsole.Write(tabla);
        UI.Pausa();
    }

    private static void Simular(ClinicaService clinica)
    {
        var mascotas = clinica.ObtenerMascotas();
        if (mascotas.Count == 0)
        {
            UI.Advertencia("No hay mascotas registradas para simular.");
            UI.Pausa();
            return;
        }

        var servicios = clinica.ObtenerServicios();
        var servicio = UI.Seleccionar(
            "Selecciona el servicio",
            servicios,
            s => $"{s.Nombre} · {s.Categoria} · base {UI.Moneda(s.PrecioBase)}");

        var mascota = UI.Seleccionar(
            "Selecciona la mascota",
            mascotas,
            m => $"{m.Nombre} · {m.Especie} · {m.Raza} · {m.PesoKg:N2} kg");

        var precio = servicio.CalcularPrecio(mascota);
        var conDescuento = servicio.CalcularPrecio(mascota, 10m);
        var porDos = servicio.CalcularPrecio(mascota, 2);
        var conDescuentoYDos = servicio.CalcularPrecio(mascota, 10m, 2);

        var panel = new Panel(
                new Markup(
                    $"[bold]{servicio.Nombre.EscapeMarkup()}[/] · {servicio.Categoria.EscapeMarkup()}\n" +
                    $"[grey]Paciente:[/] {mascota.Ficha().EscapeMarkup()}\n" +
                    $"[grey]Precio base:[/] {UI.Moneda(servicio.PrecioBase)}\n\n" +
                    $"[bold]Precio calculado:[/] [bold green]{UI.Moneda(precio)}[/]\n" +
                    $"[grey]Con 10 % de descuento:[/] {UI.Moneda(conDescuento)}\n" +
                    $"[grey]Para 2 servicios:[/] {UI.Moneda(porDos)}\n" +
                    $"[grey]Con descuento y cantidad:[/] {UI.Moneda(conDescuentoYDos)}\n\n" +
                    "[grey]El método CalcularPrecio se resolvió según el tipo real de servicio y la sobrecarga elegida.[/]"))
            .Header("[bold]Simulación de precio[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        AnsiConsole.Write(panel);
        UI.Pausa();
    }

    private static void Actualizar(ClinicaService clinica)
    {
        var servicios = clinica.ObtenerServicios(soloActivos: false);
        var servicio = UI.Seleccionar(
            "Selecciona el servicio",
            servicios,
            s => $"{s.Nombre} · actual {UI.Moneda(s.PrecioBase)}");

        var nuevoPrecio = UI.PedirDecimal(
            "Nuevo precio base",
            1m,
            1_000_000m,
            servicio.PrecioBase);

        clinica.ActualizarPrecioServicio(servicio.Id, nuevoPrecio);
        UI.Exito($"Precio actualizado: {servicio.Nombre} ahora cuesta {UI.Moneda(nuevoPrecio)}.");
        UI.Pausa();
    }
}
