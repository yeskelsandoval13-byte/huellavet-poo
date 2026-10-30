namespace HuellaVet.Domain.Facturacion;

/// <summary>
/// Línea de una factura. Guarda una copia del precio cobrado,
/// para que un cambio futuro de tarifa no altere facturas antiguas.
/// </summary>
public sealed class DetalleFactura
{
    public Guid ServicioId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Math.Round(Cantidad * PrecioUnitario, 2);
}
