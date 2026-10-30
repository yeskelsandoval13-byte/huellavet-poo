namespace HuellaVet.Laboratorio.Clase03;

/// <summary>
/// Ejemplo de sobrecarga de métodos: el mismo nombre, distintos
/// parámetros y distintas reglas de cálculo.
/// </summary>
public sealed class CalculadoraPrecios
{
    public decimal Calcular(decimal precioBase)
    {
        Validar(precioBase);
        return Math.Round(precioBase, 2);
    }

    public decimal Calcular(decimal precioBase, decimal descuentoPorcentaje)
    {
        Validar(precioBase);

        if (descuentoPorcentaje is < 0m or > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(descuentoPorcentaje));
        }

        return Math.Round(precioBase * (1m - descuentoPorcentaje / 100m), 2);
    }

    public decimal Calcular(decimal precioBase, int cantidad)
    {
        Validar(precioBase);

        if (cantidad < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        }

        return Math.Round(precioBase * cantidad, 2);
    }

    public decimal Calcular(decimal precioBase, decimal descuentoPorcentaje, int cantidad)
    {
        var unitario = Calcular(precioBase, descuentoPorcentaje);

        if (cantidad < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        }

        return Math.Round(unitario * cantidad, 2);
    }

    private static void Validar(decimal precioBase)
    {
        if (precioBase <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(precioBase));
        }
    }
}
