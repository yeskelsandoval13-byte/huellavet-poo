namespace HuellaVet.Domain.Personas;

/// <summary>
/// Veterinario: empleado especializado. Su salario incluye un
/// bono por especialidad y otro por experiencia.
/// </summary>
public sealed class Veterinario : Empleado
{
    public string Especialidad { get; set; } = "Medicina general";
    public string NumeroColegiatura { get; set; } = string.Empty;
    public int AniosExperiencia { get; set; }

    public override string Cargo => "Veterinario";

    public override decimal CalcularSalario()
    {
        var bonoEspecialidad = Especialidad.Contains("Cirug", StringComparison.OrdinalIgnoreCase)
            ? 3500m
            : 2000m;

        return SalarioBase + bonoEspecialidad + (AniosExperiencia * 250m);
    }

    public string AtenderPaciente(string nombreMascota) =>
        $"La Dra./Dr. {NombreCompleto} inicia la atención de {nombreMascota}.";
}
