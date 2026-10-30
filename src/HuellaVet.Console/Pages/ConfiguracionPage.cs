using HuellaVet.Application.Abstractions;
using HuellaVet.Infrastructure.Persistencia;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class ConfiguracionPage
{
    public static void Ejecutar(IAlmacenClinica almacen, string ruta)
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Configuración", "Datos, respaldo y estado del prototipo.");

            var opcion = UI.SeleccionarTexto("¿Qué deseas hacer?", [
                "Ver información del almacenamiento",
                "Exportar resumen a TXT",
                "Restablecer datos de demostración",
                "Volver al menú principal"
            ]);

            switch (opcion)
            {
                case "Ver información del almacenamiento":
                    VerInformacion(almacen, ruta);
                    UI.Pausa();
                    break;
                case "Exportar resumen a TXT":
                    Exportar(almacen);
                    UI.Pausa();
                    break;
                case "Restablecer datos de demostración":
                    if (UI.Confirmar("Esto reemplaza los datos actuales. ¿Continuar?", false))
                    {
                        almacen.Restablecer(DatosDemo.Crear());
                        UI.Exito("Datos de demostración restablecidos.");
                    }
                    UI.Pausa();
                    break;
                default:
                    return;
            }
        }
    }

    private static void VerInformacion(IAlmacenClinica almacen, string ruta)
    {
        var datos = almacen.Datos;
        var panel = new Panel(
                new Markup(
                    $"[grey]Archivo de datos:[/] {ruta.EscapeMarkup()}\n" +
                    $"[grey]Clientes:[/] {datos.Clientes.Count}   " +
                    $"[grey]Mascotas:[/] {datos.Mascotas.Count}   " +
                    $"[grey]Empleados:[/] {datos.Empleados.Count}\n" +
                    $"[grey]Servicios:[/] {datos.Servicios.Count}   " +
                    $"[grey]Citas:[/] {datos.Citas.Count}   " +
                    $"[grey]Facturas:[/] {datos.Facturas.Count}   " +
                    $"[grey]Productos:[/] {datos.Productos.Count}\n\n" +
                    "[grey]Persistencia actual: JSON local. La interfaz IAlmacenClinica permite cambiar a SQL Server sin tocar el dominio.[/]"))
            .Header("[bold]Almacenamiento[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        AnsiConsole.Write(panel);
    }

    private static void Exportar(IAlmacenClinica almacen)
    {
        try
        {
            var datos = almacen.Datos;
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "reporte-huellavet.txt");

            var lineas = new List<string>
            {
                "HUELLAVET · RESUMEN DEL AVANCE",
                $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}",
                string.Empty,
                $"Clientes:  {datos.Clientes.Count}",
                $"Mascotas:  {datos.Mascotas.Count}",
                $"Empleados: {datos.Empleados.Count}",
                $"Servicios: {datos.Servicios.Count}",
                $"Citas:     {datos.Citas.Count}",
                $"Facturas:  {datos.Facturas.Count}",
                $"Productos: {datos.Productos.Count}",
                string.Empty,
                $"Ingresos facturados: C$ {datos.Facturas.Where(f => f.Pagada).Sum(f => f.Total):N2}",
                string.Empty,
                "Pacientes:"
            };

            lineas.AddRange(datos.Mascotas.Select(m =>
                $"  - {m.Nombre} ({m.Especie}, {m.Raza}) · {m.EdadTexto} · {m.PesoKg:N2} kg"));

            lineas.Add(string.Empty);
            lineas.Add("Personal:");
            lineas.AddRange(datos.Empleados.Select(e =>
                $"  - {e.NombreCompleto} · {e.Cargo} · C$ {e.CalcularSalario():N2}"));

            File.WriteAllLines(ruta, lineas);
            UI.Exito($"Resumen exportado en {ruta}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            UI.Error($"No se pudo exportar: {ex.Message}");
        }
    }
}
