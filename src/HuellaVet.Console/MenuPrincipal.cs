using HuellaVet.Application.Abstractions;
using HuellaVet.Application.Services;
using HuellaVet.ConsoleUI.Pages;
using HuellaVet.Domain.Common;
using Spectre.Console;

namespace HuellaVet.ConsoleUI;

/// <summary>
/// Menú principal. Cada módulo es independiente y vuelve aquí al terminar.
/// </summary>
public sealed class MenuPrincipal
{
    private readonly ClinicaService _clinica;
    private readonly FacturacionService _facturacion;
    private readonly ReporteService _reportes;
    private readonly IAlmacenClinica _almacen;
    private readonly string _ruta;

    public MenuPrincipal(
        ClinicaService clinica,
        FacturacionService facturacion,
        ReporteService reportes,
        IAlmacenClinica almacen,
        string ruta)
    {
        _clinica = clinica;
        _facturacion = facturacion;
        _reportes = reportes;
        _almacen = almacen;
        _ruta = ruta;
    }

    public void Ejecutar()
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Menú principal", "Selecciona un módulo para trabajar.");

            var opcion = UI.SeleccionarTexto("Módulo", [
                "Dashboard",
                "Clientes y mascotas",
                "Agenda de citas",
                "Atención y facturación",
                "Servicios y precios",
                "Personal y nómina",
                "Reportes",
                "Laboratorio POO (Clase 03)",
                "Configuración",
                "Salir"
            ]);

            try
            {
                switch (opcion)
                {
                    case "Dashboard":
                        DashboardPage.Mostrar(_clinica, _reportes);
                        break;
                    case "Clientes y mascotas":
                        ClientesPage.Ejecutar(_clinica);
                        break;
                    case "Agenda de citas":
                        AgendaPage.Ejecutar(_clinica, _facturacion);
                        break;
                    case "Atención y facturación":
                        FacturacionPage.Ejecutar(_facturacion, _clinica);
                        break;
                    case "Servicios y precios":
                        ServiciosPage.Ejecutar(_clinica);
                        break;
                    case "Personal y nómina":
                        PersonalPage.Mostrar(_clinica);
                        break;
                    case "Reportes":
                        ReportesPage.Mostrar(_reportes, _clinica);
                        break;
                    case "Laboratorio POO (Clase 03)":
                        LaboratorioPage.Ejecutar();
                        break;
                    case "Configuración":
                        ConfiguracionPage.Ejecutar(_almacen, _ruta);
                        break;
                    case "Salir":
                        AnsiConsole.WriteLine();
                        AnsiConsole.MarkupLine(
                            "[bold teal]Gracias por usar HuellaVet. Cuidamos cada huella.[/]");
                        return;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (ReglaNegocioException ex)
            {
                UI.Error(ex.Message);
                UI.Pausa();
            }
            catch (Exception ex)
            {
                UI.Error($"Ocurrió un error inesperado: {ex.Message}");
                UI.Advertencia("Revisa la consola o el archivo de datos e intenta de nuevo.");
                UI.Pausa();
            }
        }
    }
}
