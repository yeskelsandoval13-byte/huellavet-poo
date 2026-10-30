using System.Text.Json.Serialization;

namespace HuellaVet.Domain.Personas;

/// <summary>
/// Empleado es la clase intermedia de la jerarquía de personal.
/// Define el salario base y obliga a cada cargo a calcular su salario.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(Veterinario), "veterinario")]
[JsonDerivedType(typeof(Recepcionista), "recepcionista")]
[JsonDerivedType(typeof(AuxiliarVeterinario), "auxiliar")]
public abstract class Empleado : Persona
{
    public decimal SalarioBase { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.Today;
    public bool Activo { get; set; } = true;

    public abstract string Cargo { get; }

    public override string Rol => Cargo;

    /// <summary>
    /// Cada cargo calcula su salario con reglas distintas.
    /// Este método es el corazón del polimorfismo de personal.
    /// </summary>
    public virtual decimal CalcularSalario() => SalarioBase;

    public virtual string FichaTecnica() =>
        $"{NombreCompleto} · {Cargo} · C$ {CalcularSalario():N2}";
}
