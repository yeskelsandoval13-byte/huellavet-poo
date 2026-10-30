using HuellaVet.Application.Abstractions;
using HuellaVet.Domain;

namespace HuellaVet.Infrastructure.Persistencia;

/// <summary>
/// Implementación en memoria para pruebas y demostraciones.
/// No escribe archivos; cumple el mismo contrato que JSON.
/// </summary>
public sealed class AlmacenMemoria : IAlmacenClinica
{
    public AlmacenMemoria(ClinicaData? datos = null)
    {
        Datos = datos ?? new ClinicaData();
    }

    public ClinicaData Datos { get; private set; }

    public void Guardar()
    {
        // Intencionalmente vacío: los datos viven en memoria.
    }

    public void Restablecer(ClinicaData datos)
    {
        Datos = datos;
    }
}
