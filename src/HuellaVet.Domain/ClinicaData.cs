using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Inventario;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;
using HuellaVet.Domain.Servicios;

namespace HuellaVet.Domain;

/// <summary>
/// Agregado raíz que representa toda la información persistida
/// de la clínica. La infraestructura lo serializa como una unidad.
/// </summary>
public sealed class ClinicaData
{
    public List<Cliente> Clientes { get; set; } = [];
    public List<Empleado> Empleados { get; set; } = [];
    public List<Mascota> Mascotas { get; set; } = [];
    public List<ServicioVeterinario> Servicios { get; set; } = [];
    public List<Cita> Citas { get; set; } = [];
    public List<Factura> Facturas { get; set; } = [];
    public List<Producto> Productos { get; set; } = [];
}
