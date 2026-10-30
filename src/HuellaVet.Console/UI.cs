using System.Globalization;
using HuellaVet.Domain.Citas;
using Spectre.Console;

namespace HuellaVet.ConsoleUI;

/// <summary>
/// Utilidades de presentación: marca, colores, validaciones de entrada
/// y formatos comunes. Mantener esto centralizado hace que las páginas
/// se concentren en el flujo de negocio.
/// </summary>
public static class UI
{
    public static readonly Color Teal = Color.Teal;
    public static readonly Color Coral = Color.Orange1;
    public static readonly Color Ink = Color.Grey;

    public static void Cabecera()
    {
        AnsiConsole.Write(
            new FigletText("HuellaVet")
                .LeftJustified()
                .Color(Teal));

        AnsiConsole.MarkupLine(
            "[bold orange1]Cuidamos cada huella.[/] [grey]· Sistema de gestión clínica veterinaria[/]");
        AnsiConsole.Write(new Rule().RuleStyle("grey").LeftJustified());
    }

    public static void Titulo(string titulo, string? subtitulo = null)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new Rule($"[bold teal]{titulo.EscapeMarkup()}[/]")
                .LeftJustified()
                .RuleStyle("teal"));

        if (!string.IsNullOrWhiteSpace(subtitulo))
        {
            AnsiConsole.MarkupLine($"[grey]{subtitulo.EscapeMarkup()}[/]");
        }

        AnsiConsole.WriteLine();
    }

    public static void Exito(string mensaje) =>
        AnsiConsole.MarkupLine($"[green]✔ {mensaje.EscapeMarkup()}[/]");

    public static void Error(string mensaje) =>
        AnsiConsole.MarkupLine($"[red]✖ {mensaje.EscapeMarkup()}[/]");

    public static void Info(string mensaje) =>
        AnsiConsole.MarkupLine($"[blue]ℹ {mensaje.EscapeMarkup()}[/]");

    public static void Advertencia(string mensaje) =>
        AnsiConsole.MarkupLine($"[yellow]⚠ {mensaje.EscapeMarkup()}[/]");

    public static void Limpiar()
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        try
        {
            AnsiConsole.Clear();
        }
        catch (IOException)
        {
            // Algunos terminales no permiten limpiar; continuamos sin borrar.
        }
    }

    public static void Pausa()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Presiona ENTER para continuar...[/]");
        Console.ReadLine();
    }

    public static bool Confirmar(string pregunta, bool porDefecto = false) =>
        AnsiConsole.Confirm($"[yellow]{pregunta.EscapeMarkup()}[/]", porDefecto);

    public static string PedirTexto(string etiqueta, string? valorPorDefecto = null, bool permitirVacio = false)
    {
        var prompt = new TextPrompt<string>($"[grey]{etiqueta.EscapeMarkup()}:[/]")
            .PromptStyle("teal");

        if (!string.IsNullOrWhiteSpace(valorPorDefecto))
        {
            prompt.DefaultValue(valorPorDefecto);
        }

        if (!permitirVacio)
        {
            prompt.Validate(valor => string.IsNullOrWhiteSpace(valor)
                ? ValidationResult.Error("[red]Este campo es obligatorio.[/]")
                : ValidationResult.Success());
        }

        return AnsiConsole.Prompt(prompt).Trim();
    }

    public static decimal PedirDecimal(string etiqueta, decimal minimo, decimal maximo, decimal? porDefecto = null)
    {
        while (true)
        {
            var texto = AnsiConsole.Prompt(
                new TextPrompt<string>($"[grey]{etiqueta.EscapeMarkup()}:[/]")
                    .PromptStyle("teal")
                    .DefaultValue(porDefecto?.ToString("N2", CultureInfo.CurrentCulture) ?? string.Empty)
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(texto) && porDefecto.HasValue)
            {
                return porDefecto.Value;
            }

            if (TryParseDecimal(texto, out var valor) && valor >= minimo && valor <= maximo)
            {
                return valor;
            }

            Error($"Ingresa un número entre {minimo:N2} y {maximo:N2}.");
        }
    }

    public static int PedirEntero(string etiqueta, int minimo, int maximo, int? porDefecto = null)
    {
        while (true)
        {
            var texto = AnsiConsole.Prompt(
                new TextPrompt<string>($"[grey]{etiqueta.EscapeMarkup()}:[/]")
                    .PromptStyle("teal")
                    .DefaultValue(porDefecto?.ToString(CultureInfo.CurrentCulture) ?? string.Empty)
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(texto) && porDefecto.HasValue)
            {
                return porDefecto.Value;
            }

            if (int.TryParse(texto, NumberStyles.Integer, CultureInfo.CurrentCulture, out var valor) &&
                valor >= minimo && valor <= maximo)
            {
                return valor;
            }

            Error($"Ingresa un número entero entre {minimo} y {maximo}.");
        }
    }

    public static DateTime PedirFecha(string etiqueta, DateTime? porDefecto = null, bool debeSerFutura = false)
    {
        var formato = "dd/MM/yyyy";
        var inicial = (porDefecto ?? DateTime.Today).ToString(formato, CultureInfo.InvariantCulture);

        while (true)
        {
            var texto = AnsiConsole.Prompt(
                new TextPrompt<string>($"[grey]{etiqueta.EscapeMarkup()} ({formato}):[/]")
                    .PromptStyle("teal")
                    .DefaultValue(inicial));

            if (DateTime.TryParseExact(
                    texto,
                    ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var fecha))
            {
                if (debeSerFutura && fecha.Date < DateTime.Today)
                {
                    Error("La fecha no puede ser anterior a hoy.");
                    continue;
                }

                return fecha.Date;
            }

            Error("Formato inválido. Usa dd/MM/yyyy.");
        }
    }

    public static DateTime PedirFechaHora(string etiqueta, DateTime? porDefecto = null, bool debeSerFutura = true)
    {
        const string formato = "dd/MM/yyyy HH:mm";
        var inicial = (porDefecto ?? DateTime.Now.AddDays(1).Date.AddHours(9))
            .ToString(formato, CultureInfo.InvariantCulture);

        while (true)
        {
            var texto = AnsiConsole.Prompt(
                new TextPrompt<string>($"[grey]{etiqueta.EscapeMarkup()} ({formato}):[/]")
                    .PromptStyle("teal")
                    .DefaultValue(inicial));

            if (DateTime.TryParseExact(
                    texto,
                    ["dd/MM/yyyy HH:mm", "d/M/yyyy H:mm", "dd-MM-yyyy HH:mm", "d-M-yyyy H:mm"],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var fecha))
            {
                if (debeSerFutura && fecha <= DateTime.Now)
                {
                    Error("La fecha y hora debe ser futura.");
                    continue;
                }

                return fecha;
            }

            Error("Formato inválido. Usa dd/MM/yyyy HH:mm.");
        }
    }

    public static T Seleccionar<T>(string titulo, IEnumerable<T> opciones, Func<T, string> convertidor)
        where T : notnull
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title($"[grey]{titulo.EscapeMarkup()}[/]")
                .PageSize(12)
                .UseConverter(convertidor)
                .AddChoices(opciones));
    }

    public static string SeleccionarTexto(string titulo, IEnumerable<string> opciones) =>
        Seleccionar(titulo, opciones, x => x);

    public static void MostrarLineas(IEnumerable<string> lineas)
    {
        foreach (var linea in lineas)
        {
            AnsiConsole.WriteLine(linea);
        }
    }

    public static string Moneda(decimal monto) =>
        $"C$ {monto.ToString("N2", CultureInfo.CurrentCulture)}";

    public static string IdCorto(Guid id) => id.ToString()[..8];

    public static string Estado(EstadoCita estado) => estado switch
    {
        EstadoCita.Programada => "[yellow]Programada[/]",
        EstadoCita.Confirmada => "[blue]Confirmada[/]",
        EstadoCita.EnAtencion => "[orange1]En atención[/]",
        EstadoCita.Atendida => "[green]Atendida[/]",
        EstadoCita.Cancelada => "[red]Cancelada[/]",
        EstadoCita.NoAsistio => "[grey]No asistió[/]",
        _ => estado.ToString()
    };

    public static string Truncar(string texto, int maximo) =>
        texto.Length <= maximo ? texto : texto[..Math.Max(0, maximo - 1)] + "…";

    private static bool TryParseDecimal(string texto, out decimal valor)
    {
        return decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor) ||
               decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
    }
}
