namespace HuellaVet.Domain.Common;

/// <summary>
/// Excepción para errores de negocio que deben mostrarse al usuario
/// sin romper la aplicación.
/// </summary>
public sealed class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
