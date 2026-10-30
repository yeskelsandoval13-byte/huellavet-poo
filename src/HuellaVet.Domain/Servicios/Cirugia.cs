using HuellaVet.Domain.Mascotas;

namespace HuellaVet.Domain.Servicios;

/// <summary>
/// Cirugía. El precio depende del riesgo, de la hospitalización
/// y del peso del paciente.
/// </summary>
public sealed class Cirugia : ServicioVeterinario
{
    public NivelRiesgo Riesgo { get; set; } = NivelRiesgo.Medio;
    public bool RequiereHospitalizacion { get; set; }

    public override string Categoria => "Cirugía";

    public override decimal CalcularPrecio(Mascota mascota)
    {
        var factorRiesgo = Riesgo switch
        {
            NivelRiesgo.Bajo => 1.0m,
            NivelRiesgo.Medio => 1.25m,
            NivelRiesgo.Alto => 1.50m,
            _ => 1.0m
        };

        var total = PrecioBase * factorRiesgo;

        if (RequiereHospitalizacion)
        {
            total += 1200m;
        }

        if (mascota.PesoKg > 25m)
        {
            total += 500m;
        }

        return Math.Round(total, 2);
    }
}
