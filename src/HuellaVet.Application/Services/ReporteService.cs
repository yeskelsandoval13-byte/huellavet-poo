using HuellaVet.Application.Abstractions;
using HuellaVet.Application.Dtos;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Servicios;

namespace HuellaVet.Application.Services;

/// <summary>
/// Reportes de solo lectura para el dashboard y la toma de decisiones.
/// </summary>
public sealed class ReporteService
{
    private readonly IAlmacenClinica _almacen;

    public ReporteService(IAlmacenClinica almacen)
    {
        _almacen = almacen;
    }

    private Domain.ClinicaData Datos => _almacen.Datos;

    public ResumenDashboard ObtenerResumen()
    {
        var hoy = DateTime.Today;

        return new ResumenDashboard(
            Pacientes: Datos.Mascotas.Count,
            CitasHoy: Datos.Citas.Count(c =>
                c.FechaHora.Date == hoy &&
                c.Estado != EstadoCita.Cancelada),
            IngresosMes: Datos.Facturas
                .Where(f => f.Pagada && f.Fecha.Month == hoy.Month && f.Fecha.Year == hoy.Year)
                .Sum(f => f.Total),
            StockBajo: Datos.Productos.Count(p => p.StockBajo),
            CitasPendientes: Datos.Citas.Count(c => c.EstaActiva),
            EmpleadosActivos: Datos.Empleados.Count(e => e.Activo));
    }

    public IReadOnlyList<IngresoPorServicio> IngresosPorServicio()
    {
        var servicios = Datos.Servicios.ToDictionary(s => s.Id, s => s.Nombre);

        return Datos.Facturas
            .Where(f => f.Pagada)
            .SelectMany(f => f.Detalles)
            .GroupBy(d => d.ServicioId)
            .Select(g => new IngresoPorServicio(
                servicios.TryGetValue(g.Key, out var nombre) ? nombre : "Otros conceptos",
                g.Sum(d => d.Cantidad),
                g.Sum(d => d.Subtotal)))
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    public IReadOnlyList<ConteoEspecie> PacientesPorEspecie() =>
        Datos.Mascotas
            .GroupBy(m => m.Especie)
            .Select(g => new ConteoEspecie(g.Key, g.Count()))
            .OrderByDescending(x => x.Cantidad)
            .ToList();

    public IReadOnlyList<ServicioPopular> ServiciosMasSolicitados()
    {
        var servicios = Datos.Servicios.ToDictionary(s => s.Id, s => s.Nombre);
        var ingresosPorCita = Datos.Facturas
            .Where(f => f.Pagada && f.CitaId.HasValue)
            .ToDictionary(f => f.CitaId!.Value, f => f.Total);

        return Datos.Citas
            .GroupBy(c => c.ServicioId)
            .Select(g => new ServicioPopular(
                servicios.TryGetValue(g.Key, out var nombre) ? nombre : "Otros",
                g.Count(),
                g.Sum(c => ingresosPorCita.TryGetValue(c.Id, out var total) ? total : 0m)))
            .OrderByDescending(x => x.Citas)
            .ThenByDescending(x => x.Ingresos)
            .Take(6)
            .ToList();
    }

    public IReadOnlyList<VacunaProxima> ProximasVacunas(int dias = 45)
    {
        var hoy = DateTime.Today;
        var limite = hoy.AddDays(dias);
        var servicios = Datos.Servicios.OfType<Vacunacion>().ToDictionary(s => s.Id);
        var mascotas = Datos.Mascotas.ToDictionary(m => m.Id);
        var clientes = Datos.Clientes.ToDictionary(c => c.Id);

        var resultado = new List<VacunaProxima>();

        foreach (var cita in Datos.Citas.Where(c => c.Estado == EstadoCita.Atendida))
        {
            if (!servicios.TryGetValue(cita.ServicioId, out var vacuna))
            {
                continue;
            }

            if (!mascotas.TryGetValue(cita.MascotaId, out var mascota) ||
                !clientes.TryGetValue(cita.ClienteId, out var dueno))
            {
                continue;
            }

            var fechaBase = cita.FechaAtencion ?? cita.FechaHora;
            var proxima = fechaBase.AddMonths(Math.Max(1, vacuna.ProximaDosisMeses));

            if (proxima.Date < hoy || proxima.Date > limite)
            {
                continue;
            }

            resultado.Add(new VacunaProxima(
                mascota.Nombre,
                dueno.NombreCompleto,
                vacuna.Vacuna,
                proxima,
                (proxima.Date - hoy).Days));
        }

        return resultado.OrderBy(v => v.Fecha).ToList();
    }

    public IReadOnlyList<(DateTime Mes, decimal Total)> IngresosUltimosMeses(int meses = 6)
    {
        var hoy = DateTime.Today;
        var inicio = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-(meses - 1));

        return Enumerable.Range(0, meses)
            .Select(i =>
            {
                var mes = inicio.AddMonths(i);
                var total = Datos.Facturas
                    .Where(f => f.Pagada && f.Fecha.Year == mes.Year && f.Fecha.Month == mes.Month)
                    .Sum(f => f.Total);
                return (mes, total);
            })
            .ToList();
    }
}
