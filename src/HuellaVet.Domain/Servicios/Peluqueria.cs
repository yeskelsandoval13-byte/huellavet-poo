using HuellaVet.Domain.Mascotas;

namespace HuellaVet.Domain.Servicios;

/// <summary>
/// Peluquería. El precio se ajusta por peso y por el baño.
/// </summary>
public sealed class Peluqueria : ServicioVeterinario
{
    public TipoCorte Tipo { get; set; } = TipoCorte.Completo;
    public bool IncluyeBano { get; set; } = true;

    public override string Categoria => "Estética";

    public override decimal CalcularPrecio(Mascota mascota)
    {
        var factorPeso = mascota.PesoKg switch
        {
            <= 10m => 1.0m,
            <= 25m => 1.2m,
            _ => 1.5m
        };

        var extraBano = IncluyeBano ? 200m : 0m;
        return Math.Round((PrecioBase * factorPeso) + extraBano, 2);
    }
}
