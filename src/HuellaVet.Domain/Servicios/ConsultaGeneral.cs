using HuellaVet.Domain.Mascotas;

namespace HuellaVet.Domain.Servicios;

/// <summary>
/// Consulta general. Aplica recargos por edad avanzada y por peso.
/// </summary>
public sealed class ConsultaGeneral : ServicioVeterinario
{
    public bool IncluyeReceta { get; set; } = true;

    public override string Categoria => "Consulta";

    public override decimal CalcularPrecio(Mascota mascota)
    {
        var recargo = 0m;

        if (mascota.EdadAnios >= 8)
        {
            recargo += 150m;
        }

        if (mascota.PesoKg > 30m)
        {
            recargo += 100m;
        }

        return Math.Round(PrecioBase + recargo, 2);
    }
}
