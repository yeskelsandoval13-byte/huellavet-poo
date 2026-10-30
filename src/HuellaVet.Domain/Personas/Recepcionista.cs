namespace HuellaVet.Domain.Personas;

/// <summary>
/// Recepcionista: empleado de atención al público.
/// El turno nocturno recibe un bono adicional.
/// </summary>
public sealed class Recepcionista : Empleado
{
    public string Turno { get; set; } = "Matutino";

    public override string Cargo => "Recepcionista";

    public override decimal CalcularSalario()
    {
        var bonoTurno = Turno.Equals("Nocturno", StringComparison.OrdinalIgnoreCase)
            ? 1500m
            : 800m;

        return SalarioBase + bonoTurno;
    }
}
