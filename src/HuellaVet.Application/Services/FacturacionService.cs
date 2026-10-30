using HuellaVet.Application.Abstractions;
using HuellaVet.Domain.Common;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Citas;

namespace HuellaVet.Application.Services;

/// <summary>
/// Genera y consulta facturas. La factura se crea a partir de una cita
/// ya atendida, aplicando el precio polimórfico del servicio.
/// </summary>
public sealed class FacturacionService
{
    private readonly IAlmacenClinica _almacen;

    public FacturacionService(IAlmacenClinica almacen)
    {
        _almacen = almacen;
    }

    private Domain.ClinicaData Datos => _almacen.Datos;

    public IReadOnlyList<Factura> ObtenerFacturas() =>
        Datos.Facturas.OrderByDescending(f => f.Fecha).ToList();

    public Factura? ObtenerFactura(Guid facturaId) =>
        Datos.Facturas.FirstOrDefault(f => f.Id == facturaId);

    public Factura? ObtenerFacturaDeCita(Guid citaId) =>
        Datos.Facturas.FirstOrDefault(f => f.CitaId == citaId);

    /// <summary>
    /// Sobrecarga sin descuento: delega en la versión completa.
    /// </summary>
    public Factura GenerarFacturaParaCita(Guid citaId, MetodoPago metodoPago) =>
        GenerarFacturaParaCita(citaId, metodoPago, 0m);

    /// <summary>
    /// Genera la factura de una cita atendida, con descuento opcional.
    /// </summary>
    public Factura GenerarFacturaParaCita(
        Guid citaId,
        MetodoPago metodoPago,
        decimal descuentoPorcentaje)
    {
        var cita = Datos.Citas.FirstOrDefault(c => c.Id == citaId)
            ?? throw new ReglaNegocioException("La cita no existe.");

        if (cita.Estado != EstadoCita.Atendida)
        {
            throw new ReglaNegocioException(
                "Solo se puede facturar una cita que ya fue atendida.");
        }

        if (Datos.Facturas.Any(f => f.CitaId == citaId))
        {
            throw new ReglaNegocioException("Esta cita ya tiene una factura registrada.");
        }

        var cliente = Datos.Clientes.FirstOrDefault(c => c.Id == cita.ClienteId)
            ?? throw new ReglaNegocioException("El cliente de la cita no existe.");

        var mascota = Datos.Mascotas.FirstOrDefault(m => m.Id == cita.MascotaId)
            ?? throw new ReglaNegocioException("La mascota de la cita no existe.");

        var servicio = Datos.Servicios.FirstOrDefault(s => s.Id == cita.ServicioId)
            ?? throw new ReglaNegocioException("El servicio de la cita no existe.");

        var factura = new Factura
        {
            Numero = SiguienteNumero(),
            ClienteId = cliente.Id,
            CitaId = cita.Id,
            Fecha = cita.FechaAtencion ?? DateTime.Now
        };

        factura.AgregarDetalle(servicio, mascota);

        if (descuentoPorcentaje > 0)
        {
            factura.AplicarDescuento(descuentoPorcentaje);
        }

        factura.RegistrarPago(metodoPago);

        Datos.Facturas.Add(factura);
        cliente.AcumularPuntos(factura.Total);
        _almacen.Guardar();

        return factura;
    }

    private string SiguienteNumero()
    {
        var anio = DateTime.Today.Year;
        var consecutivo = Datos.Facturas.Count(f => f.Fecha.Year == anio) + 1;

        while (true)
        {
            var numero = $"HV-{anio}-{consecutivo:0000}";
            if (Datos.Facturas.All(f => !f.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase)))
            {
                return numero;
            }

            consecutivo++;
        }
    }
}
