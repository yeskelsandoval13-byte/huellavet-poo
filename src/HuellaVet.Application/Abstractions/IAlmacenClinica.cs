using HuellaVet.Domain;

namespace HuellaVet.Application.Abstractions;

/// <summary>
/// Abstracción de persistencia. La aplicación no sabe si los datos
/// viven en JSON, memoria o SQL Server: solo pide guardar y consultar.
/// </summary>
public interface IAlmacenClinica
{
    ClinicaData Datos { get; }
    void Guardar();
    void Restablecer(ClinicaData datos);
}
