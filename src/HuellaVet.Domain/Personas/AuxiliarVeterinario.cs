namespace HuellaVet.Domain.Personas;

/// <summary>
/// Auxiliar veterinario: apoyo en consulta, laboratorio y hospitalización.
/// </summary>
public sealed class AuxiliarVeterinario : Empleado
{
    public string Certificacion { get; set; } = "Asistencia veterinaria";

    public override string Cargo => "Auxiliar veterinario";

    public override decimal CalcularSalario() => SalarioBase + 900m;
}
