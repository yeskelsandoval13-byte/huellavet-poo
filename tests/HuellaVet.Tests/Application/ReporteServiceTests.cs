using HuellaVet.Application.Services;
using HuellaVet.Infrastructure.Persistencia;
using Xunit;

namespace HuellaVet.Tests.Application;

public sealed class ReporteServiceTests
{
    private static (ReporteService Reportes, HuellaVet.Domain.ClinicaData Data) Crear()
    {
        var data = DatosDemo.Crear();
        return (new ReporteService(new AlmacenMemoria(data)), data);
    }

    [Fact]
    public void ObtenerResumen_ReflejaLosDatos()
    {
        var (reportes, data) = Crear();

        var resumen = reportes.ObtenerResumen();

        Assert.Equal(data.Mascotas.Count, resumen.Pacientes);
        Assert.Equal(data.Citas.Count(c => c.FechaHora.Date == DateTime.Today), resumen.CitasHoy);
        Assert.Equal(data.Productos.Count(p => p.StockBajo), resumen.StockBajo);
        Assert.True(resumen.IngresosMes > 0);
    }

    [Fact]
    public void PacientesPorEspecie_AgrupaTodosLosPacientes()
    {
        var (reportes, data) = Crear();

        var especies = reportes.PacientesPorEspecie();

        Assert.Equal(data.Mascotas.Count, especies.Sum(e => e.Cantidad));
        Assert.Contains(especies, e => e.Especie == "Perro");
        Assert.Contains(especies, e => e.Especie == "Gato");
    }

    [Fact]
    public void IngresosPorServicio_SoloConsideraFacturasPagadas()
    {
        var (reportes, _) = Crear();

        var ingresos = reportes.IngresosPorServicio();

        Assert.NotEmpty(ingresos);
        Assert.All(ingresos, i => Assert.True(i.Total > 0));
    }

    [Fact]
    public void ProximasVacunas_IncluyeLaVacunaDeMilo()
    {
        var (reportes, _) = Crear();

        var vacunas = reportes.ProximasVacunas(45);

        Assert.Contains(vacunas, v => v.Mascota == "Milo");
    }

    [Fact]
    public void IngresosUltimosMeses_DevuelveSeisMeses()
    {
        var (reportes, data) = Crear();

        var meses = reportes.IngresosUltimosMeses();

        Assert.Equal(6, meses.Count);
        Assert.Equal(
            data.Facturas
                .Where(f => f.Pagada && f.Fecha.Year == DateTime.Today.Year && f.Fecha.Month == DateTime.Today.Month)
                .Sum(f => f.Total),
            meses[^1].Total);
    }
}
