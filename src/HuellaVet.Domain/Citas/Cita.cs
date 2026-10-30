using System.Text.Json.Serialization;
using HuellaVet.Domain.Common;

namespace HuellaVet.Domain.Citas;

/// <summary>
/// Cita: relaciona cliente, mascota, veterinario y servicio.
/// Su estado solo cambia mediante métodos, no asignando la propiedad.
/// Esa es una muestra de encapsulación.
/// </summary>
public sealed class Cita : EntidadBase
{
    public Guid ClienteId { get; set; }
    public Guid MascotaId { get; set; }
    public Guid VeterinarioId { get; set; }
    public Guid ServicioId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = string.Empty;

    [JsonInclude]
    public EstadoCita Estado { get; private set; } = EstadoCita.Programada;

    [JsonInclude]
    public string? Diagnostico { get; private set; }

    [JsonInclude]
    public string? Observaciones { get; private set; }

    [JsonInclude]
    public decimal? PesoRegistrado { get; private set; }

    [JsonInclude]
    public DateTime? FechaAtencion { get; private set; }

    [JsonInclude]
    public string? MotivoCancelacion { get; private set; }

    public bool EstaActiva =>
        Estado is EstadoCita.Programada or EstadoCita.Confirmada or EstadoCita.EnAtencion;

    public bool PuedeFacturarse => Estado == EstadoCita.Atendida;

    public void Confirmar()
    {
        if (Estado != EstadoCita.Programada)
        {
            throw new ReglaNegocioException("Solo una cita programada puede confirmarse.");
        }

        Estado = EstadoCita.Confirmada;
    }

    public void IniciarAtencion()
    {
        if (Estado is not (EstadoCita.Programada or EstadoCita.Confirmada))
        {
            throw new ReglaNegocioException("La cita no está en un estado válido para iniciar la atención.");
        }

        Estado = EstadoCita.EnAtencion;
    }

    public void Atender(
        string diagnostico,
        decimal pesoRegistrado,
        string? observaciones = null,
        DateTime? fechaAtencion = null)
    {
        if (Estado != EstadoCita.EnAtencion)
        {
            throw new ReglaNegocioException("Primero debe iniciarse la atención de la cita.");
        }

        Validador.TextoRequerido(diagnostico, "diagnóstico");
        Validador.Rango(pesoRegistrado, 0.01m, 200m, "peso");
        Validador.FechaNoFutura(fechaAtencion ?? DateTime.Now, "fecha de atención");

        Diagnostico = diagnostico.Trim();
        PesoRegistrado = pesoRegistrado;
        Observaciones = observaciones?.Trim();
        FechaAtencion = fechaAtencion ?? DateTime.Now;
        Estado = EstadoCita.Atendida;
    }

    public void Cancelar(string motivo)
    {
        if (Estado is EstadoCita.Atendida or EstadoCita.Cancelada)
        {
            throw new ReglaNegocioException("Una cita atendida o cancelada ya no puede cancelarse.");
        }

        Validador.TextoRequerido(motivo, "motivo de cancelación");
        MotivoCancelacion = motivo.Trim();
        Estado = EstadoCita.Cancelada;
    }

    public void MarcarNoAsistio()
    {
        if (Estado is not (EstadoCita.Programada or EstadoCita.Confirmada))
        {
            throw new ReglaNegocioException("Solo una cita pendiente puede marcarse como no asistida.");
        }

        Estado = EstadoCita.NoAsistio;
    }

    public void Reprogramar(DateTime nuevaFechaHora)
    {
        if (!EstaActiva)
        {
            throw new ReglaNegocioException("Solo una cita activa puede reprogramarse.");
        }

        Validador.FechaFutura(nuevaFechaHora, "nueva fecha");
        FechaHora = nuevaFechaHora;
        Estado = EstadoCita.Programada;
        MotivoCancelacion = null;
    }

    public string EstadoTexto => Estado switch
    {
        EstadoCita.Programada => "Programada",
        EstadoCita.Confirmada => "Confirmada",
        EstadoCita.EnAtencion => "En atención",
        EstadoCita.Atendida => "Atendida",
        EstadoCita.Cancelada => "Cancelada",
        EstadoCita.NoAsistio => "No asistió",
        _ => Estado.ToString()
    };
}
