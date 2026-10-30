using HuellaVet.Domain.Mascotas;

namespace HuellaVet.Domain.Servicios;

/// <summary>
/// Vacunación. El precio se ajusta si el paciente es un gato.
/// Guarda la vacuna y el intervalo para la próxima dosis.
/// </summary>
public sealed class Vacunacion : ServicioVeterinario
{
    public string Vacuna { get; set; } = "Rabia";
    public int ProximaDosisMeses { get; set; } = 12;

    public override string Categoria => "Vacunación";

    public override decimal CalcularPrecio(Mascota mascota)
    {
        var ajuste = mascota.Especie.Equals("Gato", StringComparison.OrdinalIgnoreCase) ? 50m : 0m;
        return Math.Round(PrecioBase + ajuste, 2);
    }
}
