namespace HuellaVet.Laboratorio.Clase03;

/// <summary>
/// Clase padre de la ilustración de herencia de la Clase 03.
/// Contiene los datos comunes y los métodos que heredan
/// Futbolista, Entrenador y Masajista.
/// </summary>
public class SeleccionFutbol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int Edad { get; set; }

    public virtual string Concentrarse() =>
        $"{Nombre} {Apellidos} se concentra con la selección.";

    public virtual string Viajar() =>
        $"{Nombre} {Apellidos} viaja con la selección.";
}

/// <summary>
/// Clase hija: futbolista. Hereda los datos personales y agrega
/// dorsal y demarcación, además de sus propias acciones.
/// </summary>
public class Futbolista : SeleccionFutbol
{
    public int Dorsal { get; set; }
    public string Demarcacion { get; set; } = string.Empty;

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} se concentra para el partido como {Demarcacion}.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja en el autobús del equipo.";

    public string JugarPartido() =>
        $"{Nombre} {Apellidos} juega el partido con el dorsal {Dorsal}.";

    public string Entrenar() =>
        $"{Nombre} {Apellidos} entrena la definición y la resistencia.";
}

/// <summary>
/// Clase hija: entrenador. Agrega su licencia federativa y las
/// acciones propias de dirección técnica.
/// </summary>
public class Entrenador : SeleccionFutbol
{
    public string IdFederacion { get; set; } = string.Empty;

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} revisa la estrategia con el cuerpo técnico.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja con el cuerpo técnico.";

    public string DirigirPartido() =>
        $"{Nombre} {Apellidos} dirige el partido desde la banca.";

    public string DirigirEntrenamiento() =>
        $"{Nombre} {Apellidos} dirige el entrenamiento táctico.";
}

/// <summary>
/// Clase hija: masajista. Agrega titulación y experiencia, y las
/// acciones de recuperación física del equipo.
/// </summary>
public class Masajista : SeleccionFutbol
{
    public string Titulacion { get; set; } = string.Empty;
    public int AniosExperiencia { get; set; }

    public override string Concentrarse() =>
        $"{Nombre} {Apellidos} prepara la camilla y los vendajes.";

    public override string Viajar() =>
        $"{Nombre} {Apellidos} viaja con el equipo médico.";

    public string DarMasaje() =>
        $"{Nombre} {Apellidos} da un masaje de recuperación ({Titulacion}, {AniosExperiencia} años de experiencia).";
}
