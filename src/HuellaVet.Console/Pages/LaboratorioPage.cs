using HuellaVet.Laboratorio.Clase03;
using Spectre.Console;

namespace HuellaVet.ConsoleUI.Pages;

public static class LaboratorioPage
{
    public static void Ejecutar()
    {
        while (true)
        {
            UI.Limpiar();
            UI.Cabecera();
            UI.Titulo("Laboratorio POO · Clase 03", "Herencia, polimorfismo y sobrecarga, con las clases de la ilustración.");

            var opcion = UI.SeleccionarTexto("Selecciona una demostración", [
                "1. Herencia · Selección de fútbol",
                "2. Polimorfismo · Formas geométricas",
                "3. Sobrecarga · Calculadora de precios",
                "4. ¿Dónde se aplica en HuellaVet?",
                "Volver al menú principal"
            ]);

            switch (opcion)
            {
                case "1. Herencia · Selección de fútbol":
                    UI.Titulo("Herencia", "SeleccionFutbol → Futbolista, Entrenador y Masajista.");
                    UI.MostrarLineas(DemostracionClase03.Herencia());
                    UI.Pausa();
                    break;
                case "2. Polimorfismo · Formas geométricas":
                    UI.Titulo("Polimorfismo", "Forma → Círculo, Rectángulo y Cuadrado.");
                    UI.MostrarLineas(DemostracionClase03.Polimorfismo());
                    UI.Pausa();
                    break;
                case "3. Sobrecarga · Calculadora de precios":
                    UI.Titulo("Sobrecarga", "Mismo método, distintos parámetros.");
                    UI.MostrarLineas(DemostracionClase03.Sobrecarga());
                    UI.Pausa();
                    break;
                case "4. ¿Dónde se aplica en HuellaVet?":
                    MostrarMapa();
                    UI.Pausa();
                    break;
                default:
                    return;
            }
        }
    }

    private static void MostrarMapa()
    {
        UI.Titulo("Del laboratorio al proyecto", "Los mismos conceptos sostienen el dominio de HuellaVet.");

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Teal)
            .Expand();

        tabla.AddColumn("Concepto");
        tabla.AddColumn("En la Clase 03");
        tabla.AddColumn("En HuellaVet");

        tabla.AddRow(
            "Herencia",
            "SeleccionFutbol → Futbolista, Entrenador, Masajista",
            "Persona → Cliente / Empleado → Veterinario, Recepcionista, Auxiliar");
        tabla.AddRow(
            "Herencia",
            "Forma → Círculo, Rectángulo, Cuadrado",
            "Mascota → Perro, Gato, Ave, Conejo");
        tabla.AddRow(
            "Herencia",
            "—",
            "ServicioVeterinario → Consulta, Vacunación, Cirugía, Peluquería, Hospitalización");
        tabla.AddRow(
            "Polimorfismo",
            "forma.CalculaArea() sin conocer el tipo",
            "servicio.CalcularPrecio(mascota) y empleado.CalcularSalario()");
        tabla.AddRow(
            "Polimorfismo",
            "miembro.Concentrarse() en List<SeleccionFutbol>",
            "mascota.Sonido() y mascota.FactorDosis en List<Mascota>");
        tabla.AddRow(
            "Sobrecarga",
            "CalculadoraPrecios.Calcular(...) con 1, 2 o 3 argumentos",
            "ServicioVeterinario.CalcularPrecio(...) y Factura.AgregarDetalle(...)");
        tabla.AddRow(
            "Encapsulación",
            "—",
            "Cita.Estado solo cambia con Confirmar/Atender/Cancelar; Factura.Total es calculado");
        tabla.AddRow(
            "Abstracción",
            "Forma y SeleccionFutbol",
            "Persona, Mascota, ServicioVeterinario e IAlmacenClinica");

        AnsiConsole.Write(tabla);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[grey]Archivos del laboratorio:[/] src/HuellaVet.Laboratorio/Clase03/");
    }
}
