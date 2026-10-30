namespace HuellaVet.Laboratorio.Clase03;

/// <summary>
/// Clase base de la ilustración de polimorfismo de la Clase 03.
/// Define el atributo Area y el método CalculaArea().
/// </summary>
public abstract class Forma
{
    public double Area { get; protected set; }

    public abstract double CalculaArea();
}

/// <summary>Área = π × radio².</summary>
public sealed class Circulo : Forma
{
    public double Radio { get; set; }

    public override double CalculaArea()
    {
        Area = Math.Round(Math.PI * Radio * Radio, 2);
        return Area;
    }
}

/// <summary>Área = base × altura.</summary>
public sealed class Rectangulo : Forma
{
    public double Base { get; set; }
    public double Altura { get; set; }

    public override double CalculaArea()
    {
        Area = Math.Round(Base * Altura, 2);
        return Area;
    }
}

/// <summary>Área = lado².</summary>
public sealed class Cuadrado : Forma
{
    public double Lado { get; set; }

    public override double CalculaArea()
    {
        Area = Math.Round(Lado * Lado, 2);
        return Area;
    }
}
