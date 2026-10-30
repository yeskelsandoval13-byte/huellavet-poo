using HuellaVet.Domain.Mascotas;

namespace HuellaVet.Domain.Servicios;

/// <summary>
/// Hospitalización. Cobra por día y aplica un recargo a pacientes
/// que superan los 20 kg.
/// </summary>
public sealed class Hospitalizacion : ServicioVeterinario
{
    public int Dias { get; set; } = 1;
    public decimal CostoPorDia { get; set; } = 850m;

    public override string Categoria => "Hospitalización";

    public override decimal CalcularPrecio(Mascota mascota)
    {
        var total = CostoPorDia * Math.Max(1, Dias);
        if (mascota.PesoKg > 20m)
        {
            total += 150m;
        }

        return Math.Round(total, 2);
    }
}
