using HuellaVet.Application.Services;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class AgendaPage
{
    public static void Ejecutar(ClinicaService clinica, FacturacionService facturacion)
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Agenda de citas", "Programa, confirma, atiende y cancela citas.");

            var opcion = UI.SeleccionarTexto("¿Qué deseas hacer?", [
                "Ver agenda de hoy",
                "Ver agenda de mañana",
                "Ver todas las citas",
                "Programar nueva cita",
                "Gestionar cita existente",
                "Volver al menú principal"
            ]);

            try
            {
                switch (opcion)
                {
                    case "Ver agenda de hoy":
                        VerAgenda(clinica, DateTime.Today, "Agenda de hoy");
                        break;
                    case "Ver agenda de mañana":
                        VerAgenda(clinica, DateTime.Today.AddDays(1), "Agenda de mañana");
                        break;
                    case "Ver todas las citas":
                        VerAgenda(clinica, null, "Todas las citas");
                        break;
                    case "Programar nueva cita":
                        ProgramarCita(clinica);
                        break;
                    case "Gestionar cita existente":
                        GestionarCita(clinica, facturacion);
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

    private static void VerAgenda(ClinicaService clinica, DateTime? fecha, string titulo)
    {
        UI.Titulo(titulo);
        var citas = clinica.ObtenerCitas(fecha);

        if (citas.Count == 0)
        {
            UI.Info("No hay citas para mostrar.");
            UI.Pausa();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        if (fecha is null)
        {
            tabla.AddColumn("Fecha");
        }

        tabla.AddColumn("Hora");
        tabla.AddColumn("Paciente");
        tabla.AddColumn("Dueño");
        tabla.AddColumn("Servicio");
        tabla.AddColumn("Veterinario");
        tabla.AddColumn("Estado");
        tabla.AddColumn("Motivo");

        foreach (var cita in citas)
        {
            var mascota = clinica.ObtenerMascota(cita.MascotaId);
            var dueno = clinica.ObtenerCliente(cita.ClienteId);
            var servicio = clinica.ObtenerServicio(cita.ServicioId);
            var veterinario = clinica.ObtenerEmpleados().FirstOrDefault(e => e.Id == cita.VeterinarioId);

            var fila = new List<string>();
            if (fecha is null)
            {
                fila.Add(cita.FechaHora.ToString("dd/MM/yyyy"));
            }

            fila.Add(cita.FechaHora.ToString("HH:mm"));
            fila.Add(mascota?.Nombre ?? "—");
            fila.Add(dueno?.NombreCompleto ?? "—");
            fila.Add(servicio?.Nombre ?? "—");
            fila.Add(veterinario?.NombreCompleto ?? "—");
            fila.Add(UI.Estado(cita.Estado));
            fila.Add(UI.Truncar(cita.Motivo, 28));

            tabla.AddRow(fila.ToArray());
        }

        AnsiConsole.Write(tabla);
        UI.Pausa();
    }

    private static void ProgramarCita(ClinicaService clinica)
    {
        var clientes = clinica.ObtenerClientes();
        if (clientes.Count == 0)
        {
            UI.Advertencia("Primero registra un cliente y su mascota.");
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

        var servicios = clinica.ObtenerServicios();
        var servicio = UI.Seleccionar(
            "Selecciona el servicio",
            servicios,
            s => $"{s.Nombre} · {s.Categoria} · {UI.Moneda(s.PrecioBase)}");

        var veterinarios = clinica.ObtenerVeterinarios();
        var veterinario = UI.Seleccionar(
            "Selecciona el veterinario",
            veterinarios,
            v => $"{v.NombreCompleto} · {v.Especialidad}");

        var fechaHora = UI.PedirFechaHora(
            "Fecha y hora de la cita",
            DateTime.Today.AddDays(1).AddHours(9));

        var motivo = UI.PedirTexto("Motivo de la consulta");

        var cita = clinica.ProgramarCita(
            cliente.Id,
            mascota.Id,
            veterinario.Id,
            servicio.Id,
            fechaHora,
            motivo);

        UI.Exito(
            $"Cita programada para {mascota.Nombre} el {cita.FechaHora:dd/MM/yyyy} a las {cita.FechaHora:HH:mm}.");
        UI.Pausa();
    }

    private static void GestionarCita(ClinicaService clinica, FacturacionService facturacion)
    {
        var citas = clinica.ObtenerCitasPendientes();
        if (citas.Count == 0)
        {
            UI.Info("No hay citas activas para gestionar.");
            UI.Pausa();
            return;
        }

        var cita = UI.Seleccionar(
            "Selecciona la cita",
            citas,
            c => DescribirCita(c, clinica));

        MostrarCita(cita, clinica);

        var opcion = UI.SeleccionarTexto("¿Qué deseas hacer con esta cita?", [
            "Confirmar cita",
            "Iniciar atención",
            "Atender y facturar",
            "Cancelar cita",
            "Marcar como no asistió",
            "Volver"
        ]);

        switch (opcion)
        {
            case "Confirmar cita":
                clinica.ConfirmarCita(cita.Id);
                UI.Exito($"Cita confirmada para {cita.FechaHora:dd/MM/yyyy HH:mm}.");
                break;
            case "Iniciar atención":
                clinica.IniciarAtencion(cita.Id);
                UI.Exito($"Atención iniciada para la cita de {cita.FechaHora:HH:mm}.");
                break;
            case "Atender y facturar":
                AtenderYFacturar(cita, clinica, facturacion);
                break;
            case "Cancelar cita":
                var motivo = UI.PedirTexto("Motivo de cancelación");
                clinica.CancelarCita(cita.Id, motivo);
                UI.Exito("Cita cancelada correctamente.");
                break;
            case "Marcar como no asistió":
                if (UI.Confirmar("¿Confirmas que el paciente no asistió?", false))
                {
                    clinica.MarcarNoAsistio(cita.Id);
                    UI.Exito("La cita se marcó como no asistida.");
                }
                break;
            default:
                return;
        }

        UI.Pausa();
    }

    private static void AtenderYFacturar(
        Cita cita,
        ClinicaService clinica,
        FacturacionService facturacion)
    {
        if (cita.Estado is EstadoCita.Programada or EstadoCita.Confirmada)
        {
            clinica.IniciarAtencion(cita.Id);
        }

        var mascota = clinica.ObtenerMascota(cita.MascotaId);
        var peso = UI.PedirDecimal(
            "Peso registrado (kg)",
            0.01m,
            200m,
            mascota?.PesoKg ?? 5m);

        var diagnostico = UI.PedirTexto("Diagnóstico");
        var observaciones = UI.PedirTexto("Observaciones", permitirVacio: true);

        clinica.AtenderCita(cita.Id, diagnostico, peso, observaciones);
        UI.Exito($"Atención registrada para {mascota?.Nombre ?? "el paciente"}.");

        if (!UI.Confirmar("¿Generar la factura ahora?", true))
        {
            return;
        }

        var metodo = UI.Seleccionar(
            "Método de pago",
            Enum.GetValues<MetodoPago>(),
            m => m.ToString());

        var descuento = UI.PedirDecimal("Descuento (%)", 0m, 50m, 0m);
        var factura = facturacion.GenerarFacturaParaCita(cita.Id, metodo, descuento);

        UI.Exito($"Factura {factura.Numero} generada por {UI.Moneda(factura.Total)}.");
        FacturacionPage.MostrarFactura(factura, clinica, pausar: false);
    }

    private static void MostrarCita(Cita cita, ClinicaService clinica)
    {
        var mascota = clinica.ObtenerMascota(cita.MascotaId);
        var dueno = clinica.ObtenerCliente(cita.ClienteId);
        var servicio = clinica.ObtenerServicio(cita.ServicioId);
        var veterinario = clinica.ObtenerEmpleados().FirstOrDefault(e => e.Id == cita.VeterinarioId);

        var panel = new Panel(
                new Markup(
                    $"[bold]{mascota?.Nombre.EscapeMarkup() ?? "—"}[/] · {mascota?.Especie.EscapeMarkup() ?? "—"} · {mascota?.Raza.EscapeMarkup() ?? "—"}\n" +
                    $"[grey]Dueño:[/] {dueno?.NombreCompleto.EscapeMarkup() ?? "—"}   [grey]Teléfono:[/] {dueno?.Telefono.EscapeMarkup() ?? "—"}\n" +
                    $"[grey]Fecha:[/] {cita.FechaHora:dd/MM/yyyy HH:mm}   [grey]Estado:[/] {UI.Estado(cita.Estado)}\n" +
                    $"[grey]Servicio:[/] {servicio?.Nombre.EscapeMarkup() ?? "—"}   [grey]Veterinario:[/] {veterinario?.NombreCompleto.EscapeMarkup() ?? "—"}\n" +
                    $"[grey]Motivo:[/] {cita.Motivo.EscapeMarkup()}\n" +
                    (string.IsNullOrWhiteSpace(cita.Diagnostico)
                        ? string.Empty
                        : $"[grey]Diagnóstico:[/] {cita.Diagnostico.EscapeMarkup()}\n")))
            .Header($"[bold]Cita {UI.IdCorto(cita.Id)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        AnsiConsole.Write(panel);
    }

    private static string DescribirCita(Cita cita, ClinicaService clinica)
    {
        var mascota = clinica.ObtenerMascota(cita.MascotaId);
        var servicio = clinica.ObtenerServicio(cita.ServicioId);
        return $"{cita.FechaHora:dd/MM HH:mm} · {mascota?.Nombre ?? "—"} · {servicio?.Nombre ?? "—"} · {cita.EstadoTexto}";
    }
}
