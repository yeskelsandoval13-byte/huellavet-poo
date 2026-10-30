using HuellaVet.Application.Services;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Common;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Infrastructure.Persistencia;
using Xunit;

namespace HuellaVet.Tests.Application;

public sealed class FacturacionServiceTests
{
    private static (ClinicaService Clinica, FacturacionService Facturacion, HuellaVet.Domain.ClinicaData Data) CrearServicios()
    {
        var data = DatosDemo.Crear();
        var almacen = new AlmacenMemoria(data);
        return (new ClinicaService(almacen), new FacturacionService(almacen), data);
    }

    private static Cita CrearCitaAtendida(ClinicaService clinica, HuellaVet.Domain.ClinicaData data)
    {
        var cliente = data.Clientes.First();
        var mascota = data.Mascotas.First(m => m.ClienteId == cliente.Id);
        var veterinario = clinica.ObtenerVeterinarios().First();
        var servicio = clinica.ObtenerServicios().First();

        var cita = clinica.ProgramarCita(
            cliente.Id,
            mascota.Id,
            veterinario.Id,
            servicio.Id,
            DateTime.Today.AddDays(4).AddHours(14),
            "Atención de prueba");

        clinica.IniciarAtencion(cita.Id);
        clinica.AtenderCita(cita.Id, "Diagnóstico de prueba", mascota.PesoKg, "Observación");
        return cita;
    }

    [Fact]
    public void GenerarFacturaParaCita_Atendida_GeneraFacturaPagada()
    {
        var (clinica, facturacion, data) = CrearServicios();
        var cita = CrearCitaAtendida(clinica, data);

        var factura = facturacion.GenerarFacturaParaCita(cita.Id, MetodoPago.Efectivo);

        Assert.True(factura.Pagada);
        Assert.Equal(MetodoPago.Efectivo, factura.MetodoPago);
        Assert.Single(factura.Detalles);
        Assert.True(factura.Total > 0);
        Assert.NotNull(facturacion.ObtenerFacturaDeCita(cita.Id));
    }

    [Fact]
    public void GenerarFacturaParaCita_NoAtendida_LanzaReglaDeNegocio()
    {
        var (clinica, facturacion, data) = CrearServicios();
        var cita = data.Citas.First(c => c.EstaActiva);

        Assert.Throws<ReglaNegocioException>(() =>
            facturacion.GenerarFacturaParaCita(cita.Id, MetodoPago.Tarjeta));
    }

    [Fact]
    public void GenerarFacturaParaCita_DosVeces_LanzaReglaDeNegocio()
    {
        var (clinica, facturacion, data) = CrearServicios();
        var cita = CrearCitaAtendida(clinica, data);
        facturacion.GenerarFacturaParaCita(cita.Id, MetodoPago.Efectivo);

        Assert.Throws<ReglaNegocioException>(() =>
            facturacion.GenerarFacturaParaCita(cita.Id, MetodoPago.Tarjeta));
    }

    [Fact]
    public void GenerarFacturaConDescuento_AplicaDescuento()
    {
        var (clinica, facturacion, data) = CrearServicios();
        var cita = CrearCitaAtendida(clinica, data);

        var factura = facturacion.GenerarFacturaParaCita(cita.Id, MetodoPago.Transferencia, 10m);

        Assert.Equal(10m, factura.DescuentoPorcentaje);
        Assert.True(factura.Descuento > 0);
        Assert.Equal(factura.Subtotal - factura.Descuento + factura.Impuesto, factura.Total);
    }

    [Fact]
    public void GenerarFactura_AcumulaPuntosAlCliente()
    {
        var (clinica, facturacion, data) = CrearServicios();
        var cita = CrearCitaAtendida(clinica, data);
        var cliente = data.Clientes.First(c => c.Id == cita.ClienteId);
        var puntosAntes = cliente.PuntosFidelidad;

        var factura = facturacion.GenerarFacturaParaCita(cita.Id, MetodoPago.Efectivo);

        Assert.True(cliente.PuntosFidelidad > puntosAntes);
        Assert.Equal((int)Math.Floor(factura.Total / 100m), cliente.PuntosFidelidad - puntosAntes);
    }
}
