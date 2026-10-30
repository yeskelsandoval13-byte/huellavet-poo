using System.Text.Json.Serialization;
using HuellaVet.Domain.Common;

namespace HuellaVet.Domain.Mascotas;

/// <summary>
/// Mascota es la clase abstracta de los pacientes.
/// Cada especie implementa su sonido, su ficha y su factor de dosis.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(Perro), "perro")]
[JsonDerivedType(typeof(Gato), "gato")]
[JsonDerivedType(typeof(Ave), "ave")]
[JsonDerivedType(typeof(Conejo), "conejo")]
public abstract class Mascota : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Raza { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public decimal PesoKg { get; set; }
    public SexoMascota Sexo { get; set; } = SexoMascota.Desconocido;
    public Guid ClienteId { get; set; }
    public string Alergias { get; set; } = "Ninguna";
    public string Notas { get; set; } = string.Empty;
    public bool Esterilizado { get; set; }
    public DateTime? UltimaVisita { get; set; }

    public abstract string Especie { get; }

    /// <summary>Sonido característico: cada especie lo implementa.</summary>
    public abstract string Sonido();

    /// <summary>
    /// Factor clínico usado para ajustar dosis. También es polimórfico:
    /// un ave no metaboliza igual que un perro.
    /// </summary>
    public abstract decimal FactorDosis { get; }

    public int EdadAnios
    {
        get
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Year;
            if (FechaNacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }

            return Math.Max(0, edad);
        }
    }

    public string EdadTexto
    {
        get
        {
            var hoy = DateTime.Today;
            var meses = ((hoy.Year - FechaNacimiento.Year) * 12) + hoy.Month - FechaNacimiento.Month;
            if (hoy.Day < FechaNacimiento.Day)
            {
                meses--;
            }

            meses = Math.Max(0, meses);
            return meses < 12 ? $"{meses} mes(es)" : $"{meses / 12} año(s)";
        }
    }

    public virtual string Ficha() =>
        $"{Nombre} · {Especie} · {Raza} · {EdadTexto} · {PesoKg:N2} kg";

    /// <summary>
    /// Cálculo de dosis usando el factor de la especie.
    /// El servicio clínico no necesita saber si es perro, gato o ave.
    /// </summary>
    public decimal CalcularDosis(decimal mgPorKg) =>
        Math.Round(PesoKg * mgPorKg * FactorDosis, 2);

    public string EstadoClinico() =>
        string.IsNullOrWhiteSpace(Alergias) || Alergias.Equals("Ninguna", StringComparison.OrdinalIgnoreCase)
            ? "Sin alergias registradas"
            : $"Alergias: {Alergias}";
}
