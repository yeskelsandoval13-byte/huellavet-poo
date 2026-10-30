namespace HuellaVet.Domain.Common;

/// <summary>
/// Clase base para todas las entidades del dominio.
/// Centraliza la identidad y la fecha de creación.
/// </summary>
public abstract class EntidadBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreadoEn { get; set; } = DateTime.Now;
}
