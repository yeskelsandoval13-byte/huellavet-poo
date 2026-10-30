namespace HuellaVet.Domain.Mascotas;

public sealed class Conejo : Mascota
{
    public decimal LargoOrejasCm { get; set; } = 8m;

    public override string Especie => "Conejo";
    public override decimal FactorDosis => 1.1m;

    public override string Sonido() => "Mmm...";

    public override string Ficha() =>
        $"{base.Ficha()} · orejas de {LargoOrejasCm:N1} cm";
}
