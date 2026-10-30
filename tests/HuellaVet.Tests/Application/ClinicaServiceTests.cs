using HuellaVet.Application.Services;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Common;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;
using HuellaVet.Infrastructure.Persistencia;
using Xunit;

namespace HuellaVet.Tests.Application;

public sealed class ClinicaServiceTests
{
    private static (ClinicaService Service, HuellaVet.Domain.ClinicaData Data) CrearServicio()
    {
        var data = DatosDemo.Crear();
        return (new ClinicaService(new AlmacenMemoria(data)), data);
    }

    [Fact]
    public void RegistrarCliente_ConDocumentoDuplicado_LanzaReglaDeNegocio()
    {
        var (service, data) = CrearServicio();
        var existente = data.Clientes.First();

        Assert.Throws<ReglaNegocioException>(() =>
            service.RegistrarCliente(
                "Otro",
                "Cliente",
                existente.Documento,
                "8000-0000",
                "otro@email.com",
                "Managua"));
    }

    [Fact]
    public void RegistrarCliente_Valido_SeAgregaAlAlmacen()
    {
        var (service, _) = CrearServicio();
        var antes = service.ObtenerClientes().Count;

        var cliente = service.RegistrarCliente(
            "Yeskel",
            "Sandoval",
            "001-010101-9999Z",
            "7777-0000",
            "yeskel@email.com",
            "Managua");

        Assert.Equal(antes + 1, service.ObtenerClientes().Count);
        Assert.Contains(cliente, service.ObtenerClientes());
    }

    [Fact]
    public void ProgramarCita_ConFechaPasada_LanzaReglaDeNegocio()
    {
        var (service, data) = CrearServicio();
        var cliente = data.Clientes.First();
        var mascota = data.Mascotas.First(m => m.ClienteId == cliente.Id);
        var veterinario = service.ObtenerVeterinarios().First();
        var servicio = service.ObtenerServicios().First();

        Assert.Throws<ReglaNegocioException>(() =>
            service.ProgramarCita(
                cliente.Id,
                mascota.Id,
                veterinario.Id,
                servicio.Id,
                DateTime.Now.AddHours(-1),
                "Cita en el pasado"));
    }

    [Fact]
    public void ProgramarCita_ConChoqueDeVeterinario_LanzaReglaDeNegocio()
    {
        var (service, data) = CrearServicio();
        var citaExistente = data.Citas.First(c => c.FechaHora > DateTime.Now && c.EstaActiva);

        Assert.Throws<ReglaNegocioException>(() =>
            service.ProgramarCita(
                citaExistente.ClienteId,
                citaExistente.MascotaId,
                citaExistente.VeterinarioId,
                citaExistente.ServicioId,
                citaExistente.FechaHora.AddMinutes(10),
                "Choque de horario"));
    }

    [Fact]
    public void AtenderCita_CambiaEstadoYActualizaPeso()
    {
        var (service, data) = CrearServicio();
        var cliente = data.Clientes.First();
        var mascota = data.Mascotas.First(m => m.ClienteId == cliente.Id);
        var veterinario = service.ObtenerVeterinarios().First();
        var servicio = service.ObtenerServicios().First();

        var cita = service.ProgramarCita(
            cliente.Id,
            mascota.Id,
            veterinario.Id,
            servicio.Id,
            DateTime.Today.AddDays(2).AddHours(16),
            "Control de prueba");

        service.IniciarAtencion(cita.Id);
        service.AtenderCita(cita.Id, "Paciente estable", 9.5m, "Sin novedad");

        Assert.Equal(EstadoCita.Atendida, cita.Estado);
        Assert.Equal("Paciente estable", cita.Diagnostico);
        Assert.Equal(9.5m, cita.PesoRegistrado);
        Assert.Equal(9.5m, mascota.PesoKg);
    }

    [Fact]
    public void CancelarCita_Atendida_LanzaReglaDeNegocio()
    {
        var (service, data) = CrearServicio();
        var cliente = data.Clientes.First();
        var mascota = data.Mascotas.First(m => m.ClienteId == cliente.Id);
        var veterinario = service.ObtenerVeterinarios().First();
        var servicio = service.ObtenerServicios().First();

        var cita = service.ProgramarCita(
            cliente.Id,
            mascota.Id,
            veterinario.Id,
            servicio.Id,
            DateTime.Today.AddDays(3).AddHours(10),
            "Control");

        service.IniciarAtencion(cita.Id);
        service.AtenderCita(cita.Id, "Atendido", 8m, null);

        Assert.Throws<ReglaNegocioException>(() => service.CancelarCita(cita.Id, "Ya no puede"));
    }

    [Fact]
    public void RegistrarMascota_ConEspecieInvalida_LanzaReglaDeNegocio()
    {
        var (service, data) = CrearServicio();
        var cliente = data.Clientes.First();

        Assert.Throws<ReglaNegocioException>(() =>
            service.RegistrarMascota(
                cliente.Id,
                "Pepito",
                "Dinosaurio",
                "Criollo",
                DateTime.Today.AddYears(-2),
                10m,
                SexoMascota.Macho,
                "Ninguna",
                string.Empty));
    }

    [Fact]
    public void CalcularNominaTotal_SumaElSalarioPolimorfico()
    {
        var (service, data) = CrearServicio();

        var esperado = data.Empleados.Where(e => e.Activo).Sum(e => e.CalcularSalario());
        var total = service.CalcularNominaTotal();

        Assert.Equal(esperado, total);
        Assert.Equal(4, data.Empleados.Count);
        Assert.Contains(data.Empleados, e => e is Veterinario);
        Assert.Contains(data.Empleados, e => e is Recepcionista);
        Assert.Contains(data.Empleados, e => e is AuxiliarVeterinario);
    }
}
