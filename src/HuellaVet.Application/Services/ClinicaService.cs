using HuellaVet.Application.Abstractions;
using HuellaVet.Application.Dtos;
using HuellaVet.Domain;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Common;
using HuellaVet.Domain.Inventario;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;
using HuellaVet.Domain.Servicios;

namespace HuellaVet.Application.Services;

/// <summary>
/// Casos de uso de la clínica: clientes, pacientes, agenda,
/// personal, servicios e inventario.
/// </summary>
public sealed class ClinicaService
{
    private readonly IAlmacenClinica _almacen;

    public ClinicaService(IAlmacenClinica almacen)
    {
        _almacen = almacen;
    }

    private ClinicaData Datos => _almacen.Datos;

    // ── Clientes ───────────────────────────────────────────────────

    public IReadOnlyList<Cliente> ObtenerClientes() =>
        Datos.Clientes.OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToList();

    public Cliente? ObtenerCliente(Guid id) =>
        Datos.Clientes.FirstOrDefault(c => c.Id == id);

    public IReadOnlyList<Cliente> BuscarClientes(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return ObtenerClientes();
        }

        var termino = texto.Trim();
        return Datos.Clientes
            .Where(c =>
                c.NombreCompleto.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                c.Documento.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                c.Telefono.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(termino, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Apellido)
            .ToList();
    }

    public Cliente RegistrarCliente(
        string nombre,
        string apellido,
        string documento,
        string telefono,
        string email,
        string direccion)
    {
        Validador.TextoRequerido(nombre, "nombre");
        Validador.TextoRequerido(apellido, "apellido");
        Validador.TextoRequerido(documento, "documento");
        Validador.TextoRequerido(telefono, "teléfono");

        var documentoNormalizado = documento.Trim();
        if (Datos.Clientes.Any(c =>
                c.Documento.Equals(documentoNormalizado, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ReglaNegocioException(
                $"Ya existe un cliente con el documento «{documentoNormalizado}».");
        }

        if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
        {
            throw new ReglaNegocioException("El correo electrónico no tiene un formato válido.");
        }

        var cliente = new Cliente
        {
            Nombre = nombre.Trim(),
            Apellido = apellido.Trim(),
            Documento = documentoNormalizado,
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Direccion = direccion.Trim(),
            FechaRegistro = DateTime.Now
        };

        Datos.Clientes.Add(cliente);
        _almacen.Guardar();
        return cliente;
    }

    public void ActualizarContactoCliente(Guid clienteId, string telefono, string email, string direccion)
    {
        var cliente = ObtenerCliente(clienteId)
            ?? throw new ReglaNegocioException("El cliente no existe.");

        Validador.TextoRequerido(telefono, "teléfono");
        if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
        {
            throw new ReglaNegocioException("El correo electrónico no tiene un formato válido.");
        }

        cliente.Telefono = telefono.Trim();
        cliente.Email = email.Trim();
        cliente.Direccion = direccion.Trim();
        _almacen.Guardar();
    }

    // ── Mascotas ───────────────────────────────────────────────────

    public IReadOnlyList<Mascota> ObtenerMascotas() =>
        Datos.Mascotas.OrderBy(m => m.Nombre).ToList();

    public IReadOnlyList<Mascota> MascotasDeCliente(Guid clienteId) =>
        Datos.Mascotas
            .Where(m => m.ClienteId == clienteId)
            .OrderBy(m => m.Nombre)
            .ToList();

    public Mascota? ObtenerMascota(Guid id) =>
        Datos.Mascotas.FirstOrDefault(m => m.Id == id);

    public Mascota RegistrarMascota(
        Guid clienteId,
        string nombre,
        string especie,
        string raza,
        DateTime fechaNacimiento,
        decimal pesoKg,
        SexoMascota sexo,
        string alergias,
        string notas)
    {
        var cliente = ObtenerCliente(clienteId)
            ?? throw new ReglaNegocioException("El cliente seleccionado no existe.");

        Validador.TextoRequerido(nombre, "nombre de la mascota");
        Validador.TextoRequerido(especie, "especie");
        Validador.FechaNoFutura(fechaNacimiento, "fecha de nacimiento");
        Validador.Rango(pesoKg, 0.01m, 200m, "peso");

        if (fechaNacimiento < DateTime.Today.AddYears(-50))
        {
            throw new ReglaNegocioException("La fecha de nacimiento no parece válida.");
        }

        Mascota mascota = especie.Trim().ToLowerInvariant() switch
        {
            "perro" => new Perro(),
            "gato" => new Gato(),
            "ave" => new Ave(),
            "conejo" => new Conejo(),
            _ => throw new ReglaNegocioException(
                "La especie debe ser Perro, Gato, Ave o Conejo.")
        };

        mascota.Nombre = nombre.Trim();
        mascota.Raza = string.IsNullOrWhiteSpace(raza) ? "Criollo" : raza.Trim();
        mascota.FechaNacimiento = fechaNacimiento.Date;
        mascota.PesoKg = pesoKg;
        mascota.Sexo = sexo;
        mascota.ClienteId = cliente.Id;
        mascota.Alergias = string.IsNullOrWhiteSpace(alergias) ? "Ninguna" : alergias.Trim();
        mascota.Notas = notas.Trim();

        Datos.Mascotas.Add(mascota);
        _almacen.Guardar();
        return mascota;
    }

    public void ActualizarFichaMascota(Guid mascotaId, decimal pesoKg, string alergias, string notas)
    {
        var mascota = ObtenerMascota(mascotaId)
            ?? throw new ReglaNegocioException("La mascota no existe.");

        Validador.Rango(pesoKg, 0.01m, 200m, "peso");
        mascota.PesoKg = pesoKg;
        mascota.Alergias = string.IsNullOrWhiteSpace(alergias) ? "Ninguna" : alergias.Trim();
        mascota.Notas = notas.Trim();
        _almacen.Guardar();
    }

    public ExpedienteMascota ObtenerExpediente(Guid mascotaId)
    {
        var mascota = ObtenerMascota(mascotaId)
            ?? throw new ReglaNegocioException("La mascota no existe.");

        var dueno = ObtenerCliente(mascota.ClienteId)
            ?? throw new ReglaNegocioException("El dueño de la mascota no existe.");

        var citas = Datos.Citas
            .Where(c => c.MascotaId == mascotaId)
            .OrderByDescending(c => c.FechaHora)
            .ToList();

        var idsCitas = citas.Select(c => c.Id).ToHashSet();
        var facturas = Datos.Facturas
            .Where(f => f.CitaId.HasValue && idsCitas.Contains(f.CitaId.Value))
            .OrderByDescending(f => f.Fecha)
            .ToList();

        return new ExpedienteMascota(mascota, dueno, citas, facturas);
    }

    // ── Personal y servicios ───────────────────────────────────────

    public IReadOnlyList<Empleado> ObtenerEmpleados() =>
        Datos.Empleados.OrderBy(e => e.Apellido).ToList();

    public IReadOnlyList<Veterinario> ObtenerVeterinarios() =>
        Datos.Empleados
            .OfType<Veterinario>()
            .Where(v => v.Activo)
            .OrderBy(v => v.Apellido)
            .ToList();

    public decimal CalcularNominaTotal() =>
        Datos.Empleados.Where(e => e.Activo).Sum(e => e.CalcularSalario());

    public IReadOnlyList<ServicioVeterinario> ObtenerServicios(bool soloActivos = true)
    {
        var consulta = Datos.Servicios.AsEnumerable();
        if (soloActivos)
        {
            consulta = consulta.Where(s => s.Activo);
        }

        return consulta.OrderBy(s => s.Categoria).ThenBy(s => s.Nombre).ToList();
    }

    public ServicioVeterinario? ObtenerServicio(Guid id) =>
        Datos.Servicios.FirstOrDefault(s => s.Id == id);

    public void ActualizarPrecioServicio(Guid servicioId, decimal nuevoPrecio)
    {
        var servicio = ObtenerServicio(servicioId)
            ?? throw new ReglaNegocioException("El servicio no existe.");

        Validador.Rango(nuevoPrecio, 1m, 1_000_000m, "precio");
        servicio.PrecioBase = nuevoPrecio;
        _almacen.Guardar();
    }

    // ── Agenda ─────────────────────────────────────────────────────

    public IReadOnlyList<Cita> ObtenerCitas(DateTime? fecha = null)
    {
        var consulta = Datos.Citas.AsEnumerable();
        if (fecha.HasValue)
        {
            consulta = consulta.Where(c => c.FechaHora.Date == fecha.Value.Date);
        }

        return consulta.OrderBy(c => c.FechaHora).ToList();
    }

    public IReadOnlyList<Cita> ObtenerCitasHoy() => ObtenerCitas(DateTime.Today);

    public IReadOnlyList<Cita> ObtenerCitasPendientes() =>
        Datos.Citas
            .Where(c => c.EstaActiva)
            .OrderBy(c => c.FechaHora)
            .ToList();

    public Cita? ObtenerCita(Guid citaId) =>
        Datos.Citas.FirstOrDefault(c => c.Id == citaId);

    public Cita ProgramarCita(
        Guid clienteId,
        Guid mascotaId,
        Guid veterinarioId,
        Guid servicioId,
        DateTime fechaHora,
        string motivo)
    {
        Validador.FechaFutura(fechaHora, "fecha y hora");
        Validador.TextoRequerido(motivo, "motivo");

        var cliente = ObtenerCliente(clienteId)
            ?? throw new ReglaNegocioException("El cliente no existe.");

        var mascota = ObtenerMascota(mascotaId)
            ?? throw new ReglaNegocioException("La mascota no existe.");

        if (mascota.ClienteId != cliente.Id)
        {
            throw new ReglaNegocioException("La mascota no pertenece al cliente seleccionado.");
        }

        var veterinario = Datos.Empleados
            .OfType<Veterinario>()
            .FirstOrDefault(v => v.Id == veterinarioId && v.Activo)
            ?? throw new ReglaNegocioException("El veterinario no existe o no está activo.");

        var servicio = ObtenerServicio(servicioId);
        if (servicio is null || !servicio.Activo)
        {
            throw new ReglaNegocioException("El servicio no existe o no está activo.");
        }

        var choqueVeterinario = Datos.Citas.Any(c =>
            c.VeterinarioId == veterinario.Id &&
            c.EstaActiva &&
            Math.Abs((c.FechaHora - fechaHora).TotalMinutes) < 30);

        if (choqueVeterinario)
        {
            throw new ReglaNegocioException(
                "El veterinario ya tiene una cita en ese horario (margen de 30 minutos).");
        }

        var choqueMascota = Datos.Citas.Any(c =>
            c.MascotaId == mascota.Id &&
            c.EstaActiva &&
            Math.Abs((c.FechaHora - fechaHora).TotalMinutes) < 30);

        if (choqueMascota)
        {
            throw new ReglaNegocioException(
                "La mascota ya tiene una cita en ese horario.");
        }

        var cita = new Cita
        {
            ClienteId = cliente.Id,
            MascotaId = mascota.Id,
            VeterinarioId = veterinario.Id,
            ServicioId = servicio.Id,
            FechaHora = fechaHora,
            Motivo = motivo.Trim()
        };

        Datos.Citas.Add(cita);
        _almacen.Guardar();
        return cita;
    }

    public void ConfirmarCita(Guid citaId)
    {
        var cita = ObtenerCita(citaId)
            ?? throw new ReglaNegocioException("La cita no existe.");

        cita.Confirmar();
        _almacen.Guardar();
    }

    public void CancelarCita(Guid citaId, string motivo)
    {
        var cita = ObtenerCita(citaId)
            ?? throw new ReglaNegocioException("La cita no existe.");

        cita.Cancelar(motivo);
        _almacen.Guardar();
    }

    public void IniciarAtencion(Guid citaId)
    {
        var cita = ObtenerCita(citaId)
            ?? throw new ReglaNegocioException("La cita no existe.");

        cita.IniciarAtencion();
        _almacen.Guardar();
    }

    public void AtenderCita(Guid citaId, string diagnostico, decimal pesoKg, string? observaciones)
    {
        var cita = ObtenerCita(citaId)
            ?? throw new ReglaNegocioException("La cita no existe.");

        cita.Atender(diagnostico, pesoKg, observaciones);

        var mascota = ObtenerMascota(cita.MascotaId);
        if (mascota is not null)
        {
            mascota.PesoKg = pesoKg;
            mascota.UltimaVisita = cita.FechaAtencion;
        }

        _almacen.Guardar();
    }

    public void MarcarNoAsistio(Guid citaId)
    {
        var cita = ObtenerCita(citaId)
            ?? throw new ReglaNegocioException("La cita no existe.");

        cita.MarcarNoAsistio();
        _almacen.Guardar();
    }

    // ── Inventario ─────────────────────────────────────────────────

    public IReadOnlyList<Producto> ObtenerProductos() =>
        Datos.Productos.OrderBy(p => p.Nombre).ToList();

    public IReadOnlyList<Producto> ObtenerProductosStockBajo() =>
        Datos.Productos.Where(p => p.StockBajo).OrderBy(p => p.Stock).ToList();

    public void ReponerProducto(Guid productoId, int cantidad)
    {
        var producto = Datos.Productos.FirstOrDefault(p => p.Id == productoId)
            ?? throw new ReglaNegocioException("El producto no existe.");

        producto.Reponer(cantidad);
        _almacen.Guardar();
    }

    public void DescontarProducto(Guid productoId, int cantidad)
    {
        var producto = Datos.Productos.FirstOrDefault(p => p.Id == productoId)
            ?? throw new ReglaNegocioException("El producto no existe.");

        producto.Descontar(cantidad);
        _almacen.Guardar();
    }
}
