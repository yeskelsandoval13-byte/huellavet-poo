using HuellaVet.Domain.Common;

namespace HuellaVet.Domain.Inventario;

/// <summary>
/// Producto de inventario. El stock solo se modifica con
/// Descontar o Reponer, que validan existencias.
/// </summary>
public sealed class Producto : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public CategoriaProducto Categoria { get; set; } = CategoriaProducto.Insumo;
    public int Stock { get; set; }
    public int StockMinimo { get; set; } = 5;
    public decimal Precio { get; set; }
    public DateTime? Vencimiento { get; set; }

    public bool StockBajo => Stock <= StockMinimo;

    public bool Vencido => Vencimiento.HasValue && Vencimiento.Value.Date < DateTime.Today;

    public void Descontar(int cantidad)
    {
        if (cantidad < 1)
        {
            throw new ReglaNegocioException("La cantidad a descontar debe ser al menos 1.");
        }

        if (cantidad > Stock)
        {
            throw new ReglaNegocioException(
                $"Stock insuficiente de «{Nombre}». Disponible: {Stock}.");
        }

        Stock -= cantidad;
    }

    public void Reponer(int cantidad)
    {
        if (cantidad < 1)
        {
            throw new ReglaNegocioException("La cantidad a reponer debe ser al menos 1.");
        }

        Stock += cantidad;
    }

    public string EstadoStock => StockBajo ? "BAJO" : "OK";
}
