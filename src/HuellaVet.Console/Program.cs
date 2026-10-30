using System.Globalization;
using System.Text;
using HuellaVet.Application.Services;
using HuellaVet.ConsoleUI;
using HuellaVet.Infrastructure.Persistencia;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

var cultura = CultureInfo.GetCultureInfo("es-NI");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

try
{
    AnsiConsole.Profile.Width = 120;
}
catch
{
    // Si la consola no permite fijar el ancho, Spectre usa el disponible.
}

var opciones = args
    .Select(a => a.Trim().ToLowerInvariant())
    .ToHashSet();

if (opciones.Contains("--help") || opciones.Contains("-h"))
{
    MostrarAyuda();
    return;
}

if (opciones.Contains("--version"))
{
    Console.WriteLine("HuellaVet 1.0 · Avance del proyecto final de POO");
    return;
}

var rutaDatos = ObtenerRutaDatos(args);
var almacen = new JsonAlmacenClinica(rutaDatos);

if (opciones.Contains("--reset"))
{
    almacen.Restablecer(DatosDemo.Crear());
    AnsiConsole.MarkupLine("[green]✔ Datos de demostración restablecidos.[/]");
}

if (almacen.AdvertenciaCarga is not null)
{
    UI.Advertencia(almacen.AdvertenciaCarga);
    UI.Pausa();
}

if (almacen.Datos.Clientes.Count == 0 && almacen.Datos.Mascotas.Count == 0)
{
    almacen.Restablecer(DatosDemo.Crear());
    AnsiConsole.MarkupLine("[grey]Se cargaron datos de demostración en el primer inicio.[/]");
}

var clinica = new ClinicaService(almacen);
var facturacion = new FacturacionService(almacen);
var reportes = new ReporteService(almacen);

if (opciones.Contains("--demo"))
{
    DemoRunner.Ejecutar(clinica, facturacion, reportes);
    return;
}

var menu = new MenuPrincipal(clinica, facturacion, reportes, almacen, almacen.Ruta);
menu.Ejecutar();

static string ObtenerRutaDatos(string[] args)
{
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (args[i].Equals("--data", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFullPath(args[i + 1]);
        }
    }

    return JsonAlmacenClinica.RutaPorDefecto;
}

static void MostrarAyuda()
{
    AnsiConsole.Write(new FigletText("HuellaVet").LeftJustified().Color(Color.Teal));
    AnsiConsole.MarkupLine("[bold orange1]Cuidamos cada huella.[/]");
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold]Uso:[/]");
    AnsiConsole.MarkupLine("  dotnet run --project src/HuellaVet.Console");
    AnsiConsole.MarkupLine("  dotnet run --project src/HuellaVet.Console -- --demo");
    AnsiConsole.MarkupLine("  dotnet run --project src/HuellaVet.Console -- --reset");
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold]Opciones:[/]");
    AnsiConsole.MarkupLine("  --demo           Ejecuta una demostración automática y termina.");
    AnsiConsole.MarkupLine("  --reset          Restablece los datos de demostración.");
    AnsiConsole.MarkupLine("  --data <ruta>    Usa un archivo JSON distinto para los datos.");
    AnsiConsole.MarkupLine("  --help, -h       Muestra esta ayuda.");
    AnsiConsole.MarkupLine("  --version        Muestra la versión.");
}
