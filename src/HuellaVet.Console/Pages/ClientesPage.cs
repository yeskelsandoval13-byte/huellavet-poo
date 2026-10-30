using HuellaVet.Application.Services;
using HuellaVet.Domain.Mascotas;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class ClientesPage
{
    public static void Ejecutar(ClinicaService clinica)
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Clientes y mascotas", "Registro, búsqueda y expediente clínico.");

            var opcion = UI.SeleccionarTexto("¿Qué deseas hacer?", [
                "Ver todos los clientes",
                "Buscar cliente",
                "Registrar cliente",
                "Registrar mascota",
                "Ver expediente de una mascota",
                "Volver al menú principal"
            ]);

            try
            {
                switch (opcion)
                {
                    case "Ver todos los clientes":
                        VerClientes(clinica);
                        break;
                    case "Buscar cliente":
                        BuscarCliente(clinica);
                        break;
                    case "Registrar cliente":
                        RegistrarCliente(clinica);
                        break;
                    case "Registrar mascota":
                        RegistrarMascota(clinica);
                        break;
                    case "Ver expediente de una mascota":
                        VerExpediente(clinica);
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

    private static void VerClientes(ClinicaService clinica)
    {
        var clientes = clinica.ObtenerClientes();

        if (clientes.Count == 0)
        {
            UI.Info("Todavía no hay clientes registrados.");
            UI.Pausa();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        tabla.AddColumn("ID");
        tabla.AddColumn("Cliente");
        tabla.AddColumn("Documento");
        tabla.AddColumn("Teléfono");
        tabla.AddColumn("Mascotas");
        tabla.AddColumn("Puntos");

        foreach (var cliente in clientes)
        {
            tabla.AddRow(
                UI.IdCorto(cliente.Id),
                cliente.NombreCompleto,
                cliente.Documento,
                cliente.Telefono,
                clinica.MascotasDeCliente(cliente.Id).Count.ToString(),
                cliente.PuntosFidelidad.ToString());
        }

        AnsiConsole.Write(tabla);
        UI.Pausa();
    }

    private static void BuscarCliente(ClinicaService clinica)
    {
        var termino = UI.PedirTexto("Nombre, documento, teléfono o correo");
        var resultados = clinica.BuscarClientes(termino);

        if (resultados.Count == 0)
        {
            UI.Info($"No se encontraron clientes con «{termino}».");
            UI.Pausa();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        tabla.AddColumn("Cliente");
        tabla.AddColumn("Documento");
        tabla.AddColumn("Teléfono");
        tabla.AddColumn("Correo");
        tabla.AddColumn("Mascotas");

        foreach (var cliente in resultados)
        {
            tabla.AddRow(
                cliente.NombreCompleto,
                cliente.Documento,
                cliente.Telefono,
                string.IsNullOrWhiteSpace(cliente.Email) ? "—" : cliente.Email,
                clinica.MascotasDeCliente(cliente.Id).Count.ToString());
        }

        AnsiConsole.Write(tabla);
        UI.Pausa();
    }

    private static void RegistrarCliente(ClinicaService clinica)
    {
        UI.Titulo("Nuevo cliente", "Los campos con * son obligatorios.");

        var nombre = UI.PedirTexto("Nombre *");
        var apellido = UI.PedirTexto("Apellido *");
        var documento = UI.PedirTexto("Documento de identidad *");
        var telefono = UI.PedirTexto("Teléfono *");
        var email = UI.PedirTexto("Correo electrónico (opcional)", permitirVacio: true);
        var direccion = UI.PedirTexto("Dirección", permitirVacio: true);

        var cliente = clinica.RegistrarCliente(nombre, apellido, documento, telefono, email, direccion);

        UI.Exito($"Cliente registrado: {cliente.NombreCompleto} ({UI.IdCorto(cliente.Id)}).");
        UI.Pausa();
    }

    private static void RegistrarMascota(ClinicaService clinica)
    {
        var clientes = clinica.ObtenerClientes();
        if (clientes.Count == 0)
        {
            UI.Advertencia("Primero registra un cliente.");
            UI.Pausa();
            return;
        }

        UI.Titulo("Nueva mascota", "El paciente quedará asociado a un dueño.");

        var cliente = UI.Seleccionar(
            "Selecciona al dueño",
            clientes,
            c => $"{c.NombreCompleto} · {c.Documento}");

        var nombre = UI.PedirTexto("Nombre de la mascota *");
        var especie = UI.SeleccionarTexto("Especie *", ["Perro", "Gato", "Ave", "Conejo"]);
        var raza = UI.PedirTexto("Raza", "Criollo", permitirVacio: true);
        var nacimiento = UI.PedirFecha(
            "Fecha de nacimiento *",
            DateTime.Today.AddYears(-2),
            debeSerFutura: false);
        var peso = UI.PedirDecimal("Peso en kg *", 0.01m, 200m, 5m);

        var sexo = UI.Seleccionar(
            "Sexo",
            new[] { SexoMascota.Macho, SexoMascota.Hembra, SexoMascota.Desconocido },
            s => s switch
            {
                SexoMascota.Macho => "Macho",
                SexoMascota.Hembra => "Hembra",
                _ => "Desconocido"
            });

        var alergias = UI.PedirTexto("Alergias", "Ninguna", permitirVacio: true);
        var notas = UI.PedirTexto("Notas clínicas", permitirVacio: true);

        var mascota = clinica.RegistrarMascota(
            cliente.Id,
            nombre,
            especie,
            raza,
            nacimiento,
            peso,
            sexo,
            alergias,
            notas);

        UI.Exito($"Mascota registrada: {mascota.Nombre} ({mascota.Especie}) para {cliente.NombreCompleto}.");
        UI.Pausa();
    }

    private static void VerExpediente(ClinicaService clinica)
    {
        var clientes = clinica.ObtenerClientes();
        if (clientes.Count == 0)
        {
            UI.Info("No hay clientes registrados.");
            UI.Pausa();
            return;
        }

        var cliente = UI.Seleccionar(
            "Selecciona al dueño",
            clientes,
            c => $"{c.NombreCompleto} · {c.Documento}");

        var mascotas = clinica.MascotasDeCliente(cliente.Id);
        if (mascotas.Count == 0)
        {
            UI.Advertencia($"{cliente.NombreCompleto} no tiene mascotas registradas.");
            UI.Pausa();
            return;
        }

        var mascota = UI.Seleccionar(
            "Selecciona la mascota",
            mascotas,
            m => $"{m.Nombre} · {m.Especie} · {m.Raza}");

        var expediente = clinica.ObtenerExpediente(mascota.Id);
        var m = expediente.Mascota;

        var panel = new Panel(
                new Markup(
                    $"[bold]{m.Nombre.EscapeMarkup()}[/] · {m.Especie.EscapeMarkup()} · {m.Raza.EscapeMarkup()}\n" +
                    $"[grey]Dueño:[/] {expediente.Dueno.NombreCompleto.EscapeMarkup()} · {expediente.Dueno.Telefono.EscapeMarkup()}\n" +
                    $"[grey]Edad:[/] {m.EdadTexto}   [grey]Peso:[/] {m.PesoKg:N2} kg   [grey]Sexo:[/] {m.Sexo}\n" +
                    $"[grey]Estado:[/] {m.EstadoClinico().EscapeMarkup()}\n" +
                    $"[grey]Sonido:[/] {m.Sonido().EscapeMarkup()}\n" +
                    $"[grey]Dosis de referencia (5 mg/kg):[/] {m.CalcularDosis(5m):N2} mg · factor {m.FactorDosis:N1}\n" +
                    $"[grey]Notas:[/] {(string.IsNullOrWhiteSpace(m.Notas) ? "—" : m.Notas.EscapeMarkup())}\n\n" +
                    "[grey]Ejemplo académico de polimorfismo; no sustituye el criterio clínico.[/]"))
            .Header($"[bold]Expediente {UI.IdCorto(m.Id)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        AnsiConsole.Write(panel);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold]Historial de citas[/]").LeftJustified().RuleStyle("grey"));

        if (expediente.Citas.Count == 0)
        {
            UI.Info("Sin citas registradas.");
        }
        else
        {
            var tabla = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .Expand();

            tabla.AddColumn("Fecha");
            tabla.AddColumn("Servicio");
            tabla.AddColumn("Veterinario");
            tabla.AddColumn("Estado");
            tabla.AddColumn("Diagnóstico");

            foreach (var cita in expediente.Citas)
            {
                var servicio = clinica.ObtenerServicio(cita.ServicioId);
                var veterinario = clinica.ObtenerEmpleados().FirstOrDefault(e => e.Id == cita.VeterinarioId);
                tabla.AddRow(
                    cita.FechaHora.ToString("dd/MM/yyyy HH:mm"),
                    servicio?.Nombre ?? "—",
                    veterinario?.NombreCompleto ?? "—",
                    UI.Estado(cita.Estado),
                    string.IsNullOrWhiteSpace(cita.Diagnostico) ? "—" : cita.Diagnostico);
            }

            AnsiConsole.Write(tabla);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[bold]Facturas[/]").LeftJustified().RuleStyle("grey"));

        if (expediente.Facturas.Count == 0)
        {
            UI.Info("Sin facturas registradas.");
        }
        else
        {
            var tabla = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .Expand();

            tabla.AddColumn("Número");
            tabla.AddColumn("Fecha");
            tabla.AddColumn("Total");
            tabla.AddColumn("Estado");

            foreach (var factura in expediente.Facturas)
            {
                tabla.AddRow(
                    factura.Numero,
                    factura.Fecha.ToString("dd/MM/yyyy"),
                    UI.Moneda(factura.Total),
                    factura.Pagada ? "[green]Pagada[/]" : "[yellow]Pendiente[/]");
            }

            AnsiConsole.Write(tabla);
        }

        UI.Pausa();
    }
}
