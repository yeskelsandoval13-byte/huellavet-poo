namespace HuellaVet.Domain.Personas;

/// <summary>
/// Persona es la raíz de la jerarquía de personas de la clínica.
/// Es abstracta porque no se registra una persona "genérica":
/// siempre es un cliente o un empleado.
/// </summary>
public abstract class Persona : Common.EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    /// <summary>Rol que cada tipo concreto decide mostrar (polimorfismo).</summary>
    public abstract string Rol { get; }

    public virtual string Presentacion() => $"{NombreCompleto} · {Rol}";
}
