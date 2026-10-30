namespace HuellaVet.Domain.Mascotas;

public enum TallaPerro
{
    Pequena,
    Mediana,
    Grande,
    Gigante
}

public sealed class Perro : Mascota
{
    public TallaPerro Talla { get; set; } = TallaPerro.Mediana;
    public bool Entrenado { get; set; }

    public override string Especie => "Perro";
    public override decimal FactorDosis => 1.0m;

    public override string Sonido() => "¡Guau, guau!";

    public override string Ficha() =>
        $"{base.Ficha()} · Talla {Talla}" + (Entrenado ? " · entrenado" : string.Empty);
}
