using HuellaVet.Domain.Common;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Servicios;
using HuellaVet.Infrastructure.Persistencia;
using Xunit;

namespace HuellaVet.Tests.Domain;

public sealed class FacturaTests
{
    private static (Factura Factura, Mascota Mascota, ServicioVeterinario Servicio) CrearFactura()
    {
        var data = DatosDemo.Crear();
        var cliente = data.Clientes.First();
        var mascota = data.Mascotas.First(m => m.ClienteId == cliente.Id);
        var servicio = data.Servicios.OfType<ConsultaGeneral>().First();

        var factura = new Factura
        {
            Numero = "TEST-0001",
            ClienteId = cliente.Id,
            Fecha = DateTime.Today
        };

        return (factura, mascota, servicio);
    }

    [Fact]
    public void Factura_CalculaSubtotalDescuentoImpuestoTotal()
    {
        var (factura, mascota, servicio) = CrearFactura();

        factura.AgregarDetalle(servicio, mascota);
        factura.AplicarDescuento(10m);

        var subtotal = servicio.CalcularPrecio(mascota);

        Assert.Equal(subtotal, factura.Subtotal);
        Assert.Equal(Math.Round(subtotal * 0.10m, 2), factura.Descuento);
        Assert.Equal(Math.Round((subtotal - factura.Descuento) * 0.15m, 2), factura.Impuesto);
        Assert.Equal(subtotal - factura.Descuento + factura.Impuesto, factura.Total);
    }

    [Fact]
    public void Factura_NoPermitePagarSinDetalles()
    {
        var (factura, _, _) = CrearFactura();

        Assert.Throws<ReglaNegocioException>(() => factura.RegistrarPago(MetodoPago.Efectivo));
    }

    [Fact]
    public void Factura_NoPermitePagoDoble()
    {
        var (factura, mascota, servicio) = CrearFactura();
        factura.AgregarDetalle(servicio, mascota);
        factura.RegistrarPago(MetodoPago.Efectivo);

        Assert.True(factura.Pagada);
        Assert.Throws<ReglaNegocioException>(() => factura.RegistrarPago(MetodoPago.Tarjeta));
    }

    [Fact]
    public void Factura_AgregaDetalleManualConSobrecarga()
    {
        var (factura, _, _) = CrearFactura();

        factura.AgregarDetalle("Medicamento de prueba", 250m, 2);

        Assert.Single(factura.Detalles);
        Assert.Equal(500m, factura.Subtotal);
        Assert.Equal(2, factura.Detalles[0].Cantidad);
    }

    [Fact]
    public void Factura_NoPermiteDescuentoMayorA50PorCiento()
    {
        var (factura, _, _) = CrearFactura();

        Assert.Throws<ReglaNegocioException>(() => factura.AplicarDescuento(51m));
    }
}
