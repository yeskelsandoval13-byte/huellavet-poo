using HuellaVet.Domain;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Inventario;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;
using HuellaVet.Domain.Servicios;

namespace HuellaVet.Infrastructure.Persistencia;

/// <summary>
/// Datos de demostración con relaciones coherentes: dueños, mascotas,
/// personal, servicios, citas pasadas y futuras, facturas e inventario.
/// </summary>
public static class DatosDemo
{
    public static ClinicaData Crear()
    {
        var data = new ClinicaData();
        var hoy = DateTime.Today;

        // ── Clientes ───────────────────────────────────────────────
        var maria = new Cliente
        {
            Nombre = "María José",
            Apellido = "López",
            Documento = "001-120890-0001A",
            Telefono = "8888-1111",
            Email = "maria.lopez@email.com",
            Direccion = "Barrio Los Robles, Managua",
            FechaRegistro = hoy.AddYears(-2).AddMonths(-4)
        };

        var carlos = new Cliente
        {
            Nombre = "Carlos",
            Apellido = "Méndez",
            Documento = "001-050785-0002B",
            Telefono = "8777-2222",
            Email = "carlos.mendez@email.com",
            Direccion = "Residencial Las Colinas, Managua",
            FechaRegistro = hoy.AddYears(-1).AddMonths(-8)
        };

        var ana = new Cliente
        {
            Nombre = "Ana Sofía",
            Apellido = "Torres",
            Documento = "001-201195-0003C",
            Telefono = "8666-3333",
            Email = "ana.torres@email.com",
            Direccion = "Villa Fontana, Managua",
            FechaRegistro = hoy.AddYears(-3)
        };

        var jose = new Cliente
        {
            Nombre = "José",
            Apellido = "Ramírez",
            Documento = "001-110970-0004D",
            Telefono = "8555-4444",
            Email = "jose.ramirez@email.com",
            Direccion = "Barrio Monseñor Lezcano, Managua",
            FechaRegistro = hoy.AddMonths(-10)
        };

        var lucia = new Cliente
        {
            Nombre = "Lucía",
            Apellido = "Vega",
            Documento = "001-030802-0005E",
            Telefono = "8444-5555",
            Email = "lucia.vega@email.com",
            Direccion = "Ciudad Jardín, Managua",
            FechaRegistro = hoy.AddMonths(-6)
        };

        data.Clientes.AddRange([maria, carlos, ana, jose, lucia]);

        // ── Personal ───────────────────────────────────────────────
        var valeria = new Veterinario
        {
            Nombre = "Valeria",
            Apellido = "Ríos",
            Documento = "001-150388-0006F",
            Telefono = "8999-0001",
            Email = "valeria.rios@huellavet.com",
            Especialidad = "Cirugía",
            NumeroColegiatura = "VET-1842",
            AniosExperiencia = 8,
            SalarioBase = 22000m,
            FechaIngreso = hoy.AddYears(-4)
        };

        var marco = new Veterinario
        {
            Nombre = "Marco",
            Apellido = "Salas",
            Documento = "001-220792-0007G",
            Telefono = "8999-0002",
            Email = "marco.salas@huellavet.com",
            Especialidad = "Medicina general",
            NumeroColegiatura = "VET-2301",
            AniosExperiencia = 3,
            SalarioBase = 18000m,
            FechaIngreso = hoy.AddYears(-2)
        };

        var karla = new Recepcionista
        {
            Nombre = "Karla",
            Apellido = "Méndez",
            Documento = "001-300995-0008H",
            Telefono = "8999-0003",
            Email = "karla.mendez@huellavet.com",
            Turno = "Matutino",
            SalarioBase = 12000m,
            FechaIngreso = hoy.AddMonths(-18)
        };

        var luis = new AuxiliarVeterinario
        {
            Nombre = "Luis",
            Apellido = "Paredes",
            Documento = "001-180900-0009I",
            Telefono = "8999-0004",
            Email = "luis.paredes@huellavet.com",
            Certificacion = "Asistencia clínica y laboratorio",
            SalarioBase = 10000m,
            FechaIngreso = hoy.AddMonths(-9)
        };

        data.Empleados.AddRange([valeria, marco, karla, luis]);

        // ── Servicios ──────────────────────────────────────────────
        var consulta = new ConsultaGeneral
        {
            Codigo = "CON-01",
            Nombre = "Consulta general",
            PrecioBase = 500m,
            DuracionMinutos = 30,
            IncluyeReceta = true
        };

        var vacunaRabia = new Vacunacion
        {
            Codigo = "VAC-01",
            Nombre = "Vacunación antirrábica",
            PrecioBase = 350m,
            DuracionMinutos = 20,
            Vacuna = "Rabia",
            ProximaDosisMeses = 12
        };

        var vacunaPolivalente = new Vacunacion
        {
            Codigo = "VAC-02",
            Nombre = "Vacuna polivalente",
            PrecioBase = 450m,
            DuracionMinutos = 20,
            Vacuna = "Polivalente",
            ProximaDosisMeses = 12
        };

        var cirugia = new Cirugia
        {
            Codigo = "CIR-01",
            Nombre = "Cirugía de tejidos blandos",
            PrecioBase = 4500m,
            DuracionMinutos = 90,
            Riesgo = NivelRiesgo.Medio,
            RequiereHospitalizacion = true,
            RequiereAyuno = true
        };

        var peluqueria = new Peluqueria
        {
            Codigo = "PEL-01",
            Nombre = "Peluquería completa",
            PrecioBase = 400m,
            DuracionMinutos = 60,
            Tipo = TipoCorte.Completo,
            IncluyeBano = true
        };

        var hospitalizacion = new Hospitalizacion
        {
            Codigo = "HOS-01",
            Nombre = "Hospitalización por día",
            PrecioBase = 850m,
            DuracionMinutos = 1440,
            Dias = 1,
            CostoPorDia = 850m
        };

        data.Servicios.AddRange([
            consulta,
            vacunaRabia,
            vacunaPolivalente,
            cirugia,
            peluqueria,
            hospitalizacion
        ]);

        // ── Mascotas ───────────────────────────────────────────────
        var rocco = new Perro
        {
            Nombre = "Rocco",
            Raza = "Labrador",
            FechaNacimiento = hoy.AddYears(-4).AddMonths(-3),
            PesoKg = 28m,
            Sexo = SexoMascota.Macho,
            ClienteId = maria.Id,
            Talla = TallaPerro.Grande,
            Entrenado = true,
            Alergias = "Ninguna",
            Notas = "Muy sociable. Le teme a los gatos.",
            UltimaVisita = hoy.AddDays(-8)
        };

        var luna = new Gato
        {
            Nombre = "Luna",
            Raza = "Siamesa",
            FechaNacimiento = hoy.AddYears(-2).AddMonths(-5),
            PesoKg = 4.2m,
            Sexo = SexoMascota.Hembra,
            ClienteId = maria.Id,
            EsIndoor = true,
            Alergias = "Ninguna",
            Notas = "Gata de interior. Muy vocal.",
            UltimaVisita = hoy.AddMonths(-10)
        };

        var milo = new Perro
        {
            Nombre = "Milo",
            Raza = "Beagle",
            FechaNacimiento = hoy.AddYears(-6),
            PesoKg = 12.5m,
            Sexo = SexoMascota.Macho,
            ClienteId = carlos.Id,
            Talla = TallaPerro.Mediana,
            Entrenado = true,
            Alergias = "Ninguna",
            Notas = "Tendencia a escaparse cuando escucha cohetes.",
            UltimaVisita = hoy.AddMonths(-11)
        };

        var nala = new Gato
        {
            Nombre = "Nala",
            Raza = "Persa",
            FechaNacimiento = hoy.AddYears(-3).AddMonths(-2),
            PesoKg = 3.8m,
            Sexo = SexoMascota.Hembra,
            ClienteId = ana.Id,
            EsIndoor = true,
            Alergias = "Ninguna",
            Notas = "Requiere cepillado diario.",
            UltimaVisita = hoy.AddDays(-20)
        };

        var coco = new Ave
        {
            Nombre = "Coco",
            Raza = "Periquito",
            FechaNacimiento = hoy.AddYears(-1).AddMonths(-4),
            PesoKg = 0.05m,
            Sexo = SexoMascota.Macho,
            ClienteId = jose.Id,
            PuedeVolar = true,
            ColorPlumaje = "Verde",
            Alergias = "Ninguna",
            Notas = "Come semillas y fruta fresca."
        };

        var copito = new Conejo
        {
            Nombre = "Copito",
            Raza = "Enano",
            FechaNacimiento = hoy.AddYears(-2),
            PesoKg = 1.8m,
            Sexo = SexoMascota.Macho,
            ClienteId = lucia.Id,
            LargoOrejasCm = 7.5m,
            Alergias = "Ninguna",
            Notas = "Muy tranquilo; le gusta el heno fresco."
        };

        var toby = new Perro
        {
            Nombre = "Toby",
            Raza = "Criollo",
            FechaNacimiento = hoy.AddYears(-9),
            PesoKg = 18m,
            Sexo = SexoMascota.Macho,
            ClienteId = ana.Id,
            Talla = TallaPerro.Mediana,
            Entrenado = false,
            Alergias = "Polen",
            Notas = "Paciente geriátrico. Control de peso y articulaciones.",
            UltimaVisita = hoy.AddDays(-2)
        };

        data.Mascotas.AddRange([rocco, luna, milo, nala, coco, copito, toby]);

        // ── Citas activas ──────────────────────────────────────────
        var citaRocco = CrearCita(data, maria.Id, rocco.Id, valeria.Id, consulta.Id,
            hoy.AddHours(9), "Revisión general y control de peso");
        citaRocco.Confirmar();

        CrearCita(data, maria.Id, luna.Id, marco.Id, vacunaRabia.Id,
            hoy.AddHours(10).AddMinutes(30), "Refuerzo anual de rabia");

        var citaMilo = CrearCita(data, carlos.Id, milo.Id, valeria.Id, peluqueria.Id,
            hoy.AddHours(13), "Corte de verano");
        citaMilo.Confirmar();

        CrearCita(data, ana.Id, nala.Id, marco.Id, consulta.Id,
            hoy.AddHours(15).AddMinutes(30), "Pérdida de apetito");

        CrearCita(data, ana.Id, toby.Id, valeria.Id, consulta.Id,
            hoy.AddDays(1).AddHours(9).AddMinutes(30), "Control geriátrico");

        CrearCita(data, jose.Id, coco.Id, marco.Id, consulta.Id,
            hoy.AddDays(1).AddHours(11), "Revisión de plumaje");

        var citaCancelada = CrearCita(data, lucia.Id, copito.Id, marco.Id, consulta.Id,
            hoy.AddDays(-3).AddHours(16), "Consulta de seguimiento");
        citaCancelada.Cancelar("El dueño reprogramó la cita por viaje.");

        // ── Citas históricas y facturas ────────────────────────────
        var citaToby = CrearCita(data, ana.Id, toby.Id, valeria.Id, consulta.Id,
            hoy.AddDays(-2).AddHours(10), "Picazón y enrojecimiento en la piel");
        citaToby.IniciarAtencion();
        citaToby.Atender(
            "Dermatitis alérgica leve",
            18m,
            "Shampoo medicado por 7 días. Evitar exposición al polen.",
            hoy.AddDays(-2).AddHours(10).AddMinutes(35));

        var citaRoccoCirugia = CrearCita(data, maria.Id, rocco.Id, valeria.Id, cirugia.Id,
            hoy.AddDays(-8).AddHours(11), "Masa cutánea en el flanco derecho");
        citaRoccoCirugia.IniciarAtencion();
        citaRoccoCirugia.Atender(
            "Extracción de masa benigna",
            28m,
            "Reposo absoluto por 10 días. Control de sutura en 7 días.",
            hoy.AddDays(-8).AddHours(12).AddMinutes(30));

        var citaNala = CrearCita(data, ana.Id, nala.Id, marco.Id, consulta.Id,
            hoy.AddDays(-20).AddHours(9), "Vómitos ocasionales");
        citaNala.IniciarAtencion();
        citaNala.Atender(
            "Gastritis leve",
            3.8m,
            "Dieta blanda por 5 días. Hidratación frecuente.",
            hoy.AddDays(-20).AddHours(9).AddMinutes(30));

        var citaMiloVacuna = CrearCita(data, carlos.Id, milo.Id, marco.Id, vacunaPolivalente.Id,
            hoy.AddMonths(-11).AddHours(11), "Refuerzo anual");
        citaMiloVacuna.IniciarAtencion();
        citaMiloVacuna.Atender(
            "Vacuna polivalente aplicada",
            12.5m,
            "Próxima dosis en 12 meses.",
            hoy.AddMonths(-11).AddHours(11).AddMinutes(20));

        var citaLunaVacuna = CrearCita(data, maria.Id, luna.Id, marco.Id, vacunaRabia.Id,
            hoy.AddMonths(-10).AddHours(12), "Refuerzo anual");
        citaLunaVacuna.IniciarAtencion();
        citaLunaVacuna.Atender(
            "Vacuna antirrábica aplicada",
            4.2m,
            "Próxima dosis en 12 meses.",
            hoy.AddMonths(-10).AddHours(12).AddMinutes(20));

        CrearFactura(data, $"HV-{hoy.Year}-0001", maria, citaToby, consulta, toby,
            citaToby.FechaAtencion ?? hoy.AddDays(-2), MetodoPago.Tarjeta, 0m);

        CrearFactura(data, $"HV-{hoy.Year}-0002", maria, citaRoccoCirugia, cirugia, rocco,
            citaRoccoCirugia.FechaAtencion ?? hoy.AddDays(-8), MetodoPago.Transferencia, 5m);

        CrearFactura(data, $"HV-{hoy.Year}-0003", ana, citaNala, consulta, nala,
            citaNala.FechaAtencion ?? hoy.AddDays(-20), MetodoPago.Efectivo, 0m);

        CrearFactura(data, $"HV-{hoy.Year}-0004", carlos, citaMiloVacuna, vacunaPolivalente, milo,
            citaMiloVacuna.FechaAtencion ?? hoy.AddMonths(-11), MetodoPago.Efectivo, 0m);

        CrearFactura(data, $"HV-{hoy.Year}-0005", maria, citaLunaVacuna, vacunaRabia, luna,
            citaLunaVacuna.FechaAtencion ?? hoy.AddMonths(-10), MetodoPago.Tarjeta, 0m);

        // ── Inventario ─────────────────────────────────────────────
        data.Productos.AddRange([
            new Producto
            {
                Nombre = "Alimento premium para perro 3 kg",
                Categoria = CategoriaProducto.Alimento,
                Stock = 12,
                StockMinimo = 5,
                Precio = 850m,
                Vencimiento = hoy.AddMonths(8)
            },
            new Producto
            {
                Nombre = "Antibiótico oral 250 mg",
                Categoria = CategoriaProducto.Medicamento,
                Stock = 4,
                StockMinimo = 6,
                Precio = 320m,
                Vencimiento = hoy.AddMonths(14)
            },
            new Producto
            {
                Nombre = "Vacuna antirrábica (dosis)",
                Categoria = CategoriaProducto.Medicamento,
                Stock = 8,
                StockMinimo = 5,
                Precio = 280m,
                Vencimiento = hoy.AddMonths(6)
            },
            new Producto
            {
                Nombre = "Shampoo medicado",
                Categoria = CategoriaProducto.Higiene,
                Stock = 3,
                StockMinimo = 4,
                Precio = 250m,
                Vencimiento = hoy.AddMonths(18)
            },
            new Producto
            {
                Nombre = "Gasas estériles",
                Categoria = CategoriaProducto.Insumo,
                Stock = 25,
                StockMinimo = 10,
                Precio = 80m
            },
            new Producto
            {
                Nombre = "Alimento para gato 1.5 kg",
                Categoria = CategoriaProducto.Alimento,
                Stock = 9,
                StockMinimo = 4,
                Precio = 620m,
                Vencimiento = hoy.AddMonths(10)
            }
        ]);

        return data;
    }

    private static Cita CrearCita(
        ClinicaData data,
        Guid clienteId,
        Guid mascotaId,
        Guid veterinarioId,
        Guid servicioId,
        DateTime fechaHora,
        string motivo)
    {
        var cita = new Cita
        {
            ClienteId = clienteId,
            MascotaId = mascotaId,
            VeterinarioId = veterinarioId,
            ServicioId = servicioId,
            FechaHora = fechaHora,
            Motivo = motivo
        };

        data.Citas.Add(cita);
        return cita;
    }

    private static Factura CrearFactura(
        ClinicaData data,
        string numero,
        Cliente cliente,
        Cita cita,
        ServicioVeterinario servicio,
        Mascota mascota,
        DateTime fecha,
        MetodoPago metodoPago,
        decimal descuento)
    {
        var factura = new Factura
        {
            Numero = numero,
            ClienteId = cliente.Id,
            CitaId = cita.Id,
            Fecha = fecha
        };

        factura.AgregarDetalle(servicio, mascota);

        if (descuento > 0)
        {
            factura.AplicarDescuento(descuento);
        }

        factura.RegistrarPago(metodoPago);
        data.Facturas.Add(factura);
        cliente.AcumularPuntos(factura.Total);

        return factura;
    }
}
