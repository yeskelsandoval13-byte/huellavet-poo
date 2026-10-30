using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;
using HuellaVet.Domain.Servicios;
using HuellaVet.Infrastructure.Persistencia;
using Xunit;

namespace HuellaVet.Tests.Persistencia;

public sealed class JsonAlmacenClinicaTests
{
    [Fact]
    public void GuardarYCargar_ConservaTiposDerivadosYRelaciones()
    {
        var ruta = Path.Combine(Path.GetTempPath(), $"huellavet-test-{Guid.NewGuid():N}.json");

        try
        {
            var original = new JsonAlmacenClinica(ruta);
            original.Restablecer(DatosDemo.Crear());

            var cargado = new JsonAlmacenClinica(ruta);

            Assert.Equal(original.Datos.Clientes.Count, cargado.Datos.Clientes.Count);
            Assert.Equal(original.Datos.Mascotas.Count, cargado.Datos.Mascotas.Count);
            Assert.Equal(original.Datos.Citas.Count, cargado.Datos.Citas.Count);
            Assert.Equal(original.Datos.Facturas.Count, cargado.Datos.Facturas.Count);

            Assert.Contains(cargado.Datos.Mascotas, m => m is Perro);
            Assert.Contains(cargado.Datos.Mascotas, m => m is Gato);
            Assert.Contains(cargado.Datos.Mascotas, m => m is Ave);
            Assert.Contains(cargado.Datos.Mascotas, m => m is Conejo);
            Assert.Contains(cargado.Datos.Empleados, e => e is Veterinario);
            Assert.Contains(cargado.Datos.Servicios, s => s is Cirugia);

            var factura = cargado.Datos.Facturas.First();
            Assert.True(factura.Pagada);
            Assert.NotEmpty(factura.Detalles);
        }
        finally
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
    }
}
