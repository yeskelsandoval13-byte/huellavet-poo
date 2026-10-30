using HuellaVet.Laboratorio.Clase03;
using Xunit;

namespace HuellaVet.Tests.Laboratorio;

public sealed class Clase03Tests
{
    [Fact]
    public void Futbolista_HeredaDeSeleccionFutbol()
    {
        var futbolista = new Futbolista
        {
            Id = 1,
            Nombre = "Diego",
            Apellidos = "Martínez",
            Edad = 24,
            Dorsal = 10,
            Demarcacion = "Mediocampista"
        };

        Assert.IsAssignableFrom<SeleccionFutbol>(futbolista);
        Assert.Contains("Mediocampista", futbolista.Concentrarse());
        Assert.Contains("dorsal 10", futbolista.JugarPartido());
    }

    [Fact]
    public void ListaDeSeleccionFutbol_EjecutaComportamientoPolimorfico()
    {
        var equipo = new List<SeleccionFutbol>
        {
            new Futbolista { Nombre = "Diego", Apellidos = "Martínez", Demarcacion = "Delantero" },
            new Entrenador { Nombre = "Gustavo", Apellidos = "Acevedo", IdFederacion = "FED-01" },
            new Masajista { Nombre = "Karla", Apellidos = "Ríos", Titulacion = "Fisioterapia" }
        };

        var mensajes = equipo.Select(m => m.Concentrarse()).ToList();

        Assert.Equal(3, mensajes.Distinct().Count());
        Assert.Contains("Delantero", mensajes[0]);
        Assert.Contains("cuerpo técnico", mensajes[1]);
        Assert.Contains("camilla", mensajes[2]);
    }

    [Theory]
    [InlineData(5, 78.54)]
    [InlineData(10, 314.16)]
    public void Circulo_CalculaArea(double radio, double esperada)
    {
        var circulo = new Circulo { Radio = radio };

        var area = circulo.CalculaArea();

        Assert.Equal(esperada, area, 2);
        Assert.Equal(esperada, circulo.Area, 2);
    }

    [Fact]
    public void Rectangulo_CalculaArea()
    {
        var rectangulo = new Rectangulo { Base = 6, Altura = 4 };

        Assert.Equal(24, rectangulo.CalculaArea(), 2);
    }

    [Fact]
    public void Cuadrado_CalculaArea()
    {
        var cuadrado = new Cuadrado { Lado = 4 };

        Assert.Equal(16, cuadrado.CalculaArea(), 2);
    }

    [Fact]
    public void ListaDeFormas_CalculaAreaSinConocerElTipoConcreto()
    {
        var formas = new List<Forma>
        {
            new Circulo { Radio = 5 },
            new Rectangulo { Base = 6, Altura = 4 },
            new Cuadrado { Lado = 4 }
        };

        var areas = formas.Select(f => f.CalculaArea()).ToList();

        Assert.Equal([78.54, 24.00, 16.00], areas);
        Assert.All(formas, f => Assert.True(f.Area > 0));
    }

    [Fact]
    public void Sobrecarga_ResuelveSegunLosArgumentos()
    {
        var calculadora = new CalculadoraPrecios();

        Assert.Equal(500m, calculadora.Calcular(500m));
        Assert.Equal(450m, calculadora.Calcular(500m, 10m));
        Assert.Equal(1500m, calculadora.Calcular(500m, 3));
        Assert.Equal(5000m, calculadora.Calcular(500m, 10));
        Assert.Equal(1350m, calculadora.Calcular(500m, 10m, 3));
    }
}
