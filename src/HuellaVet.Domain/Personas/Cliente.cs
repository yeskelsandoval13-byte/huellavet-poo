namespace HuellaVet.Domain.Personas;

/// <summary>
/// Cliente de la clínica. Hereda de Persona y agrega los datos
/// propios de la relación comercial: dirección y fidelidad.
/// </summary>
public sealed class Cliente : Persona
{
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public int PuntosFidelidad { get; set; }

    public override string Rol => "Cliente";

    /// <summary>
    /// Acumula un punto por cada C$ 100 facturados.
    /// </summary>
    public int AcumularPuntos(decimal totalFactura)
    {
        if (totalFactura <= 0)
        {
            return 0;
        }

        var puntos = (int)Math.Floor(totalFactura / 100m);
        PuntosFidelidad += puntos;
        return puntos;
    }

    public override string Presentacion() =>
        $"{NombreCompleto} · Cliente desde {FechaRegistro:MM/yyyy} · {PuntosFidelidad} pts";
}
