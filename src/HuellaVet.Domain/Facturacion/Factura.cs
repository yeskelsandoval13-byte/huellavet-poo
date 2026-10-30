using System.Text.Json.Serialization;
using HuellaVet.Domain.Common;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Servicios;

namespace HuellaVet.Domain.Facturacion;

/// <summary>
/// Factura con subtotal, descuento, IVA y total calculados.
/// El total nunca se asigna desde fuera: se deriva de sus detalles.
/// </summary>
public sealed class Factura : EntidadBase
{
    public string Numero { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public Guid? CitaId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public List<DetalleFactura> Detalles { get; set; } = [];

    [JsonInclude]
    public decimal DescuentoPorcentaje { get; private set; }

    public decimal ImpuestoPorcentaje { get; set; } = 15m;

    [JsonInclude]
    public bool Pagada { get; private set; }

    [JsonInclude]
    public MetodoPago? MetodoPago { get; private set; }

    [JsonInclude]
    public DateTime? FechaPago { get; private set; }

    public decimal Subtotal => Math.Round(Detalles.Sum(d => d.Subtotal), 2);
    public decimal Descuento => Math.Round(Subtotal * DescuentoPorcentaje / 100m, 2);
    public decimal Impuesto => Math.Round((Subtotal - Descuento) * ImpuestoPorcentaje / 100m, 2);
    public decimal Total => Math.Round(Subtotal - Descuento + Impuesto, 2);

    /// <summary>
    /// Agrega un servicio usando el precio que corresponda a la mascota.
    /// El precio es polimórfico: depende del tipo real de servicio.
    /// </summary>
    public void AgregarDetalle(ServicioVeterinario servicio, Mascota mascota, int cantidad = 1)
    {
        if (Pagada)
        {
            throw new ReglaNegocioException("No se pueden agregar líneas a una factura pagada.");
        }

        if (!servicio.Activo)
        {
            throw new ReglaNegocioException($"El servicio «{servicio.Nombre}» no está activo.");
        }

        if (cantidad < 1)
        {
            throw new ReglaNegocioException("La cantidad debe ser al menos 1.");
        }

        Detalles.Add(new DetalleFactura
        {
            ServicioId = servicio.Id,
            Descripcion = $"{servicio.Nombre} · {mascota.Nombre}",
            Cantidad = cantidad,
            PrecioUnitario = servicio.CalcularPrecio(mascota)
        });
    }

    /// <summary>
    /// Sobrecarga para cobrar un concepto manual (por ejemplo, un medicamento).
    /// </summary>
    public void AgregarDetalle(string descripcion, decimal precioUnitario, int cantidad = 1)
    {
        if (Pagada)
        {
            throw new ReglaNegocioException("No se pueden agregar líneas a una factura pagada.");
        }

        Validador.TextoRequerido(descripcion, "descripción");
        Validador.Rango(precioUnitario, 0.01m, 1_000_000m, "precio");

        if (cantidad < 1)
        {
            throw new ReglaNegocioException("La cantidad debe ser al menos 1.");
        }

        Detalles.Add(new DetalleFactura
        {
            Descripcion = descripcion.Trim(),
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario
        });
    }

    public void AplicarDescuento(decimal porcentaje)
    {
        if (Pagada)
        {
            throw new ReglaNegocioException("No se puede modificar una factura ya pagada.");
        }

        Validador.Rango(porcentaje, 0m, 50m, "descuento");
        DescuentoPorcentaje = porcentaje;
    }

    public void RegistrarPago(MetodoPago metodoPago)
    {
        if (Pagada)
        {
            throw new ReglaNegocioException("La factura ya fue pagada.");
        }

        if (Detalles.Count == 0)
        {
            throw new ReglaNegocioException("No se puede pagar una factura sin detalles.");
        }

        Pagada = true;
        MetodoPago = metodoPago;
        FechaPago = DateTime.Now;
    }
}
