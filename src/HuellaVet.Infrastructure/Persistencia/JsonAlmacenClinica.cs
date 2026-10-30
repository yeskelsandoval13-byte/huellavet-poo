using System.Text.Json;
using System.Text.Json.Serialization;
using HuellaVet.Application.Abstractions;
using HuellaVet.Domain;

namespace HuellaVet.Infrastructure.Persistencia;

/// <summary>
/// Persistencia JSON del agregado ClinicaData. Guarda de forma atómica
/// (archivo temporal + reemplazo) para evitar datos corruptos.
/// </summary>
public sealed class JsonAlmacenClinica : IAlmacenClinica
{
    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _ruta;

    public JsonAlmacenClinica(string? ruta = null)
    {
        _ruta = ruta ?? RutaPorDefecto;
        Datos = Cargar();
    }

    public ClinicaData Datos { get; private set; }

    public string Ruta => _ruta;

    public string? AdvertenciaCarga { get; private set; }

    public static string RutaPorDefecto
    {
        get
        {
            var carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HuellaVet");

            return Path.Combine(carpeta, "huellavet.json");
        }
    }

    public void Guardar()
    {
        var carpeta = Path.GetDirectoryName(_ruta);
        if (!string.IsNullOrWhiteSpace(carpeta))
        {
            Directory.CreateDirectory(carpeta);
        }

        var temporal = _ruta + ".tmp";
        var json = JsonSerializer.Serialize(Datos, Opciones);
        File.WriteAllText(temporal, json);
        File.Move(temporal, _ruta, overwrite: true);
    }

    public void Restablecer(ClinicaData datos)
    {
        Datos = datos;
        Guardar();
    }

    private ClinicaData Cargar()
    {
        if (!File.Exists(_ruta))
        {
            return new ClinicaData();
        }

        try
        {
            var json = File.ReadAllText(_ruta);
            return JsonSerializer.Deserialize<ClinicaData>(json, Opciones) ?? new ClinicaData();
        }
        catch (JsonException)
        {
            var respaldo = $"{_ruta}.corrupto-{DateTime.Now:yyyyMMddHHmmss}.json";
            File.Copy(_ruta, respaldo, overwrite: true);
            AdvertenciaCarga =
                $"El archivo de datos estaba dañado. Se creó una copia en «{respaldo}» y se inició con datos nuevos.";
            return new ClinicaData();
        }
    }
}
