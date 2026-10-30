using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;

namespace HuellaVet.Application.Dtos;

public sealed record ExpedienteMascota(
    Mascota Mascota,
    Cliente Dueno,
    IReadOnlyList<Cita> Citas,
    IReadOnlyList<Factura> Facturas);

public sealed record ResumenDashboard(
    int Pacientes,
    int CitasHoy,
    decimal IngresosMes,
    int StockBajo,
    int CitasPendientes,
    int EmpleadosActivos);

public sealed record IngresoPorServicio(
    string Servicio,
    int Cantidad,
    decimal Total);

public sealed record ConteoEspecie(
    string Especie,
    int Cantidad);

public sealed record ServicioPopular(
    string Servicio,
    int Citas,
    decimal Ingresos);

public sealed record VacunaProxima(
    string Mascota,
    string Dueno,
    string Vacuna,
    DateTime Fecha,
    int Dias);
