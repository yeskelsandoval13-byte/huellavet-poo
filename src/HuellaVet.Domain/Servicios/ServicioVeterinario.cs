using System.Text.Json.Serialization;
using HuellaVet.Domain.Common;
using HuellaVet.Domain.Mascotas;

namespace HuellaVet.Domain.Servicios;

/// <summary>
/// Servicio veterinario abstracto. Cada servicio decide cómo calcular
/// su precio final; la clínica solo invoca CalcularPrecio(mascota).
/// Es el ejemplo central de polimorfismo del proyecto.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(ConsultaGeneral), "consulta")]
[JsonDerivedType(typeof(Vacunacion), "vacunacion")]
[JsonDerivedType(typeof(Cirugia), "cirugia")]
[JsonDerivedType(typeof(Peluqueria), "peluqueria")]
[JsonDerivedType(typeof(Hospitalizacion), "hospitalizacion")]
public abstract class ServicioVeterinario : EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public int DuracionMinutos { get; set; } = 30;
    public bool RequiereAyuno { get; set; }
    public bool Activo { get; set; } = true;

    public abstract string Categoria { get; }

    /// <summary>
    /// Cada servicio concreto implementa su propia tarifa.
    /// </summary>
    public abstract decimal CalcularPrecio(Mascota mascota);

    public virtual string Descripcion() =>
        $"{Nombre} · {DuracionMinutos} min · desde C$ {PrecioBase:N2}";

    // ── Sobrecarga de métodos ──────────────────────────────────────
    // Mismo nombre, distintos parámetros. El compilador decide cuál
    // ejecutar según los argumentos enviados.

    public decimal CalcularPrecio(Mascota mascota, decimal descuentoPorcentaje)
    {
        Validador.Rango(descuentoPorcentaje, 0m, 100m, "descuento");
        var precio = CalcularPrecio(mascota);
        return Math.Round(precio * (1m - (descuentoPorcentaje / 100m)), 2);
    }

    public decimal CalcularPrecio(Mascota mascota, int cantidad)
    {
        if (cantidad < 1)
        {
            throw new ReglaNegocioException("La cantidad debe ser al menos 1.");
        }

        return Math.Round(CalcularPrecio(mascota) * cantidad, 2);
    }

    public decimal CalcularPrecio(Mascota mascota, decimal descuentoPorcentaje, int cantidad)
    {
        var unitario = CalcularPrecio(mascota, descuentoPorcentaje);
        return Math.Round(unitario * Math.Max(1, cantidad), 2);
    }
}
