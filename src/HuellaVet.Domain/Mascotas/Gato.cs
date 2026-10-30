namespace HuellaVet.Domain.Mascotas;

public sealed class Gato : Mascota
{
    public bool EsIndoor { get; set; }

    public override string Especie => "Gato";
    public override decimal FactorDosis => 0.9m;

    public override string Sonido() => "Miau...";

    public override string Ficha() =>
        $"{base.Ficha()}" + (EsIndoor ? " · gato de interior" : " · acceso al exterior");
}
