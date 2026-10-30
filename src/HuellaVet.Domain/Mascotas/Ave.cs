namespace HuellaVet.Domain.Mascotas;

public sealed class Ave : Mascota
{
    public bool PuedeVolar { get; set; } = true;
    public string ColorPlumaje { get; set; } = "Verde";

    public override string Especie => "Ave";
    public override decimal FactorDosis => 1.2m;

    public override string Sonido() => "¡Pío, pío!";

    public override string Ficha() =>
        $"{base.Ficha()} · plumaje {ColorPlumaje}" + (PuedeVolar ? " · vuelo libre" : " · no vuela");
}
