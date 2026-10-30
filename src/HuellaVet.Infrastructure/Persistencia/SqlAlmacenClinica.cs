using System.Data;
using HuellaVet.Application.Abstractions;
using HuellaVet.Domain;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Inventario;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;
using HuellaVet.Domain.Servicios;
using Microsoft.Data.SqlClient;

namespace HuellaVet.Infrastructure.Persistencia;

/// <summary>
/// Almacén que carga y guarda el agregado ClinicaData en SQL Server.
/// Implementa el mismo contrato que JsonAlmacenClinica para que la
/// aplicación pueda cambiar de persistencia sin tocar los servicios.
/// </summary>
public sealed class SqlAlmacenClinica : IAlmacenClinica
{
    private readonly string _cadenaConexion;

    public ClinicaData Datos { get; private set; }

    public SqlAlmacenClinica(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
        Datos = new ClinicaData();
        Cargar();
    }

    public void Guardar() => Guardar(Datos);

    public void Restablecer(ClinicaData datos)
    {
        Datos = datos;
        Guardar(datos);
    }

    private void Guardar(ClinicaData datos)
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            GuardarPersonas(conexion, transaccion, datos);
            GuardarMascotas(conexion, transaccion, datos);
            GuardarServicios(conexion, transaccion, datos);
            GuardarCitas(conexion, transaccion, datos);
            GuardarFacturas(conexion, transaccion, datos);
            GuardarProductos(conexion, transaccion, datos);

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private void Cargar()
    {
        using var conexion = new SqlConnection(_cadenaConexion);
        conexion.Open();

        Datos = new ClinicaData
        {
            Clientes = CargarClientes(conexion),
            Empleados = CargarEmpleados(conexion),
            Mascotas = CargarMascotas(conexion),
            Servicios = CargarServicios(conexion),
            Citas = CargarCitas(conexion),
            Facturas = CargarFacturas(conexion),
            Productos = CargarProductos(conexion)
        };
    }

    private static List<Cliente> CargarClientes(SqlConnection conexion)
    {
        const string sql = @"
            SELECT p.PersonaId, p.Nombre, p.Apellido, p.Documento, p.Telefono, p.Email,
                   c.Direccion, c.FechaRegistro, c.PuntosFidelidad
            FROM Personas p
            INNER JOIN Clientes c ON c.ClienteId = p.PersonaId
            WHERE p.TipoPersona = 'C';";

        var lista = new List<Cliente>();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(new Cliente
            {
                Id = lector.GetGuid(0),
                Nombre = lector.GetString(1),
                Apellido = lector.GetString(2),
                Documento = lector.GetString(3),
                Telefono = lector.GetString(4),
                Email = lector.IsDBNull(5) ? string.Empty : lector.GetString(5),
                Direccion = lector.IsDBNull(6) ? string.Empty : lector.GetString(6),
                FechaRegistro = lector.GetDateTime(7).Date,
                PuntosFidelidad = lector.GetInt32(8)
            });
        }

        return lista;
    }

    private static List<Empleado> CargarEmpleados(SqlConnection conexion)
    {
        var estados = new Dictionary<Guid, (string Tipo, string Especialidad, string Numero, int Anios, string Turno, string Certificacion)>();

        const string sqlBase = @"
            SELECT e.EmpleadoId, e.SalarioBase, e.FechaIngreso, e.Activo, e.TipoEmpleado
            FROM Empleados e;";
        using (var comando = new SqlCommand(sqlBase, conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                estados[lector.GetGuid(0)] = (lector.GetString(4), string.Empty, string.Empty, 0, string.Empty, string.Empty);
            }
        }

        var veterinarios = new Dictionary<Guid, (string Especialidad, string Numero, int Anios)>();
        using (var comando = new SqlCommand("SELECT VeterinarioId, Especialidad, NumeroColegiatura, AniosExperiencia FROM Veterinarios;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                veterinarios[lector.GetGuid(0)] = (lector.GetString(1), lector.GetString(2), lector.GetByte(3));
            }
        }

        var recepcionistas = new Dictionary<Guid, string>();
        using (var comando = new SqlCommand("SELECT RecepcionistaId, Turno FROM Recepcionistas;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                recepcionistas[lector.GetGuid(0)] = lector.GetString(1);
            }
        }

        var auxiliares = new Dictionary<Guid, string>();
        using (var comando = new SqlCommand("SELECT AuxiliarId, Certificacion FROM AuxiliaresVeterinarios;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                auxiliares[lector.GetGuid(0)] = lector.GetString(1);
            }
        }

        const string sql = @"
            SELECT p.PersonaId, p.Nombre, p.Apellido, p.Documento, p.Telefono, p.Email,
                   e.SalarioBase, e.FechaIngreso, e.Activo, e.TipoEmpleado
            FROM Personas p
            INNER JOIN Empleados e ON e.EmpleadoId = p.PersonaId
            WHERE p.TipoPersona = 'E';";

        var lista = new List<Empleado>();
        using var comandoFinal = new SqlCommand(sql, conexion);
        using var lectorFinal = comandoFinal.ExecuteReader();
        while (lectorFinal.Read())
        {
            var id = lectorFinal.GetGuid(0);
            var tipo = lectorFinal.GetString(9);
            Empleado empleado = tipo switch
            {
                "V" => new Veterinario
                {
                    Especialidad = veterinarios.TryGetValue(id, out var v) ? v.Especialidad : "Medicina general",
                    NumeroColegiatura = veterinarios.TryGetValue(id, out var v2) ? v2.Numero : string.Empty,
                    AniosExperiencia = veterinarios.TryGetValue(id, out var v3) ? v3.Anios : 0
                },
                "R" => new Recepcionista
                {
                    Turno = recepcionistas.TryGetValue(id, out var turno) ? turno : "Matutino"
                },
                _ => new AuxiliarVeterinario
                {
                    Certificacion = auxiliares.TryGetValue(id, out var cert) ? cert : "Asistencia veterinaria"
                }
            };

            empleado.Id = id;
            empleado.Nombre = lectorFinal.GetString(1);
            empleado.Apellido = lectorFinal.GetString(2);
            empleado.Documento = lectorFinal.GetString(3);
            empleado.Telefono = lectorFinal.GetString(4);
            empleado.Email = lectorFinal.IsDBNull(5) ? string.Empty : lectorFinal.GetString(5);
            empleado.SalarioBase = lectorFinal.GetDecimal(6);
            empleado.FechaIngreso = lectorFinal.GetDateTime(7).Date;
            empleado.Activo = lectorFinal.GetBoolean(8);

            lista.Add(empleado);
        }

        return lista;
    }

    private static List<Mascota> CargarMascotas(SqlConnection conexion)
    {
        var perros = new HashSet<Guid>();
        using (var comando = new SqlCommand("SELECT MascotaId, Talla, Entrenado FROM Perros;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                perros.Add(lector.GetGuid(0));
            }
        }

        var gatos = new HashSet<Guid>();
        using (var comando = new SqlCommand("SELECT MascotaId FROM Gatos;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) gatos.Add(lector.GetGuid(0));
        }

        var aves = new HashSet<Guid>();
        using (var comando = new SqlCommand("SELECT MascotaId FROM Aves;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) aves.Add(lector.GetGuid(0));
        }

        var detallesPerro = new Dictionary<Guid, (string Talla, bool Entrenado)>();
        using (var comando = new SqlCommand("SELECT MascotaId, Talla, Entrenado FROM Perros;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                detallesPerro[lector.GetGuid(0)] = (lector.GetString(1), lector.GetBoolean(2));
            }
        }

        var detallesGato = new Dictionary<Guid, bool>();
        using (var comando = new SqlCommand("SELECT MascotaId, EsIndoor FROM Gatos;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) detallesGato[lector.GetGuid(0)] = lector.GetBoolean(1);
        }

        var detallesAve = new Dictionary<Guid, (bool PuedeVolar, string Color)>();
        using (var comando = new SqlCommand("SELECT MascotaId, PuedeVolar, ColorPlumaje FROM Aves;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                detallesAve[lector.GetGuid(0)] = (lector.GetBoolean(1), lector.GetString(2));
            }
        }

        var detallesConejo = new Dictionary<Guid, decimal>();
        using (var comando = new SqlCommand("SELECT MascotaId, LargoOrejasCm FROM Conejos;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) detallesConejo[lector.GetGuid(0)] = lector.GetDecimal(1);
        }

        const string sql = @"
            SELECT MascotaId, ClienteId, EspecieId, Nombre, Raza, FechaNacimiento,
                   PesoKg, Sexo, Alergias, Notas, Esterilizado, UltimaVisita
            FROM Mascotas;";

        var lista = new List<Mascota>();
        using var comandoFinal = new SqlCommand(sql, conexion);
        using var lectorFinal = comandoFinal.ExecuteReader();
        while (lectorFinal.Read())
        {
            var id = lectorFinal.GetGuid(0);
            var especieId = lectorFinal.GetByte(2);
            Mascota mascota = especieId switch
            {
                1 => new Perro
                {
                    Talla = detallesPerro.TryGetValue(id, out var p) ? Enum.Parse<TallaPerro>(p.Talla, true) : TallaPerro.Mediana,
                    Entrenado = detallesPerro.TryGetValue(id, out var p2) && p2.Entrenado
                },
                2 => new Gato
                {
                    EsIndoor = detallesGato.TryGetValue(id, out var indoor) && indoor
                },
                3 => new Ave
                {
                    PuedeVolar = detallesAve.TryGetValue(id, out var a) && a.PuedeVolar,
                    ColorPlumaje = detallesAve.TryGetValue(id, out var a2) ? a2.Color : "Verde"
                },
                _ => new Conejo
                {
                    LargoOrejasCm = detallesConejo.TryGetValue(id, out var l) ? l : 8m
                }
            };

            mascota.Id = id;
            mascota.ClienteId = lectorFinal.GetGuid(1);
            mascota.Nombre = lectorFinal.GetString(3);
            mascota.Raza = lectorFinal.GetString(4);
            mascota.FechaNacimiento = lectorFinal.GetDateTime(5).Date;
            mascota.PesoKg = lectorFinal.GetDecimal(6);
            mascota.Sexo = lectorFinal.GetString(7) switch
            {
                "M" => SexoMascota.Macho,
                "H" => SexoMascota.Hembra,
                _ => SexoMascota.Desconocido
            };
            mascota.Alergias = lectorFinal.IsDBNull(8) ? "Ninguna" : lectorFinal.GetString(8);
            mascota.Notas = lectorFinal.IsDBNull(9) ? string.Empty : lectorFinal.GetString(9);
            mascota.Esterilizado = lectorFinal.GetBoolean(10);
            mascota.UltimaVisita = lectorFinal.IsDBNull(11) ? null : lectorFinal.GetDateTime(11);

            lista.Add(mascota);
        }

        return lista;
    }

    private static List<ServicioVeterinario> CargarServicios(SqlConnection conexion)
    {
        var consultas = new Dictionary<Guid, bool>();
        using (var comando = new SqlCommand("SELECT ServicioId, IncluyeReceta FROM ConsultasGenerales;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) consultas[lector.GetGuid(0)] = lector.GetBoolean(1);
        }

        var vacunas = new Dictionary<Guid, (string Vacuna, int Proxima)>();
        using (var comando = new SqlCommand("SELECT ServicioId, Vacuna, ProximaDosisMeses FROM Vacunaciones;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) vacunas[lector.GetGuid(0)] = (lector.GetString(1), lector.GetByte(2));
        }

        var cirugias = new Dictionary<Guid, (string Nivel, bool Hospitalizacion)>();
        using (var comando = new SqlCommand("SELECT ServicioId, NivelRiesgo, RequiereHospitalizacion FROM Cirugias;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) cirugias[lector.GetGuid(0)] = (lector.GetString(1), lector.GetBoolean(2));
        }

        var peluquerias = new Dictionary<Guid, (string Tipo, bool Bano)>();
        using (var comando = new SqlCommand("SELECT ServicioId, TipoCorte, IncluyeBano FROM Peluquerias;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) peluquerias[lector.GetGuid(0)] = (lector.GetString(1), lector.GetBoolean(2));
        }

        var hospitalizaciones = new Dictionary<Guid, (int Dias, decimal Costo)>();
        using (var comando = new SqlCommand("SELECT ServicioId, Dias, CostoPorDia FROM Hospitalizaciones;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read()) hospitalizaciones[lector.GetGuid(0)] = (lector.GetInt16(1), lector.GetDecimal(2));
        }

        const string sql = @"
            SELECT ServicioId, Codigo, Nombre, PrecioBase, DuracionMinutos,
                   RequiereAyuno, Activo, TipoServicio
            FROM Servicios;";

        var lista = new List<ServicioVeterinario>();
        using var comandoFinal = new SqlCommand(sql, conexion);
        using var lectorFinal = comandoFinal.ExecuteReader();
        while (lectorFinal.Read())
        {
            var id = lectorFinal.GetGuid(0);
            var tipo = lectorFinal.GetString(7);
            ServicioVeterinario servicio = tipo switch
            {
                "C" => new ConsultaGeneral
                {
                    IncluyeReceta = consultas.TryGetValue(id, out var c) && c
                },
                "V" => new Vacunacion
                {
                    Vacuna = vacunas.TryGetValue(id, out var v) ? v.Vacuna : "Rabia",
                    ProximaDosisMeses = vacunas.TryGetValue(id, out var v2) ? v2.Proxima : 12
                },
                "G" => new Cirugia
                {
                    Riesgo = cirugias.TryGetValue(id, out var ci) ? ci.Nivel switch
                    {
                        "B" => NivelRiesgo.Bajo,
                        "A" => NivelRiesgo.Alto,
                        _ => NivelRiesgo.Medio
                    } : NivelRiesgo.Medio,
                    RequiereHospitalizacion = cirugias.TryGetValue(id, out var ci2) && ci2.Hospitalizacion
                },
                "P" => new Peluqueria
                {
                    Tipo = peluquerias.TryGetValue(id, out var pe) ? Enum.Parse<TipoCorte>(pe.Tipo, true) : TipoCorte.Completo,
                    IncluyeBano = peluquerias.TryGetValue(id, out var pe2) && pe2.Bano
                },
                _ => new Hospitalizacion
                {
                    Dias = hospitalizaciones.TryGetValue(id, out var h) ? h.Dias : 1,
                    CostoPorDia = hospitalizaciones.TryGetValue(id, out var h2) ? h2.Costo : 850m
                }
            };

            servicio.Id = id;
            servicio.Codigo = lectorFinal.GetString(1);
            servicio.Nombre = lectorFinal.GetString(2);
            servicio.PrecioBase = lectorFinal.GetDecimal(3);
            servicio.DuracionMinutos = lectorFinal.GetInt16(4);
            servicio.RequiereAyuno = lectorFinal.GetBoolean(5);
            servicio.Activo = lectorFinal.GetBoolean(6);

            lista.Add(servicio);
        }

        return lista;
    }

    private static List<Cita> CargarCitas(SqlConnection conexion)
    {
        var estados = new Dictionary<int, EstadoCita>();
        using (var comando = new SqlCommand("SELECT EstadoCitaId, Nombre FROM EstadosCita;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                if (Enum.TryParse<EstadoCita>(lector.GetString(1), true, out var estado))
                {
                    estados[lector.GetByte(0)] = estado;
                }
            }
        }

        const string sql = @"
            SELECT CitaId, ClienteId, MascotaId, VeterinarioId, ServicioId, EstadoCitaId,
                   FechaHora, Motivo, Diagnostico, PesoRegistrado, FechaAtencion,
                   Observaciones, MotivoCancelacion
            FROM Citas;";

        var lista = new List<Cita>();
        using var comandoFinal = new SqlCommand(sql, conexion);
        using var lectorFinal = comandoFinal.ExecuteReader();
        while (lectorFinal.Read())
        {
            var cita = new Cita
            {
                Id = lectorFinal.GetGuid(0),
                ClienteId = lectorFinal.GetGuid(1),
                MascotaId = lectorFinal.GetGuid(2),
                VeterinarioId = lectorFinal.GetGuid(3),
                ServicioId = lectorFinal.GetGuid(4),
                FechaHora = lectorFinal.GetDateTime(6),
                Motivo = lectorFinal.GetString(7)
            };

            EstablecerCitaPrivada(cita,
                lectorFinal.IsDBNull(8) ? null : lectorFinal.GetString(8),
                lectorFinal.IsDBNull(9) ? null : lectorFinal.GetDecimal(9),
                lectorFinal.IsDBNull(10) ? null : lectorFinal.GetDateTime(10),
                lectorFinal.IsDBNull(11) ? null : lectorFinal.GetString(11),
                lectorFinal.IsDBNull(12) ? null : lectorFinal.GetString(12));

            var estadoId = lectorFinal.GetByte(5);
            cita.GetType().GetProperty("Estado")!.SetValue(cita, estados.TryGetValue(estadoId, out var estado) ? estado : EstadoCita.Programada);

            lista.Add(cita);
        }

        return lista;
    }

    private static void EstablecerCitaPrivada(Cita cita, string? diagnostico, decimal? peso, DateTime? fechaAtencion, string? observaciones, string? motivoCancelacion)
    {
        var tipo = typeof(Cita);
        tipo.GetProperty("Diagnostico", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(cita, diagnostico);
        tipo.GetProperty("PesoRegistrado", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(cita, peso);
        tipo.GetProperty("FechaAtencion", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(cita, fechaAtencion);
        tipo.GetProperty("Observaciones", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(cita, observaciones);
        tipo.GetProperty("MotivoCancelacion", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(cita, motivoCancelacion);
    }

    private static List<Factura> CargarFacturas(SqlConnection conexion)
    {
        var metodos = new Dictionary<int, MetodoPago>();
        using (var comando = new SqlCommand("SELECT MetodoPagoId, Nombre FROM MetodosPago;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                if (Enum.TryParse<MetodoPago>(lector.GetString(1), true, out var metodo))
                {
                    metodos[lector.GetByte(0)] = metodo;
                }
            }
        }

        var facturas = new List<Factura>();
        const string sqlFacturas = @"
            SELECT FacturaId, Numero, ClienteId, CitaId, Fecha,
                   DescuentoPorcentaje, ImpuestoPorcentaje, Pagada,
                   MetodoPagoId, FechaPago
            FROM Facturas;";

        using (var comando = new SqlCommand(sqlFacturas, conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                var factura = new Factura
                {
                    Id = lector.GetGuid(0),
                    Numero = lector.GetString(1),
                    ClienteId = lector.GetGuid(2),
                    CitaId = lector.IsDBNull(3) ? null : lector.GetGuid(3),
                    Fecha = lector.GetDateTime(4),
                    ImpuestoPorcentaje = lector.GetDecimal(6)
                };

                var descuento = lector.GetDecimal(5);
                if (descuento > 0)
                {
                    factura.AplicarDescuento(descuento);
                }

                var pagada = lector.GetBoolean(7);
                if (pagada)
                {
                    var metodoId = lector.IsDBNull(8) ? 0 : lector.GetByte(8);
                    var metodo = metodos.TryGetValue(metodoId, out var m) ? m : MetodoPago.Efectivo;
                    factura.RegistrarPago(metodo);
                }

                facturas.Add(factura);
            }
        }

        const string sqlDetalles = @"
            SELECT DetalleFacturaId, FacturaId, ServicioId, Descripcion, Cantidad, PrecioUnitario
            FROM DetallesFactura;";

        var detallesPorFactura = new Dictionary<Guid, List<DetalleFactura>>();
        using (var comando = new SqlCommand(sqlDetalles, conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                var detalle = new DetalleFactura
                {
                    ServicioId = lector.IsDBNull(2) ? Guid.Empty : lector.GetGuid(2),
                    Descripcion = lector.GetString(3),
                    Cantidad = lector.GetInt32(4),
                    PrecioUnitario = lector.GetDecimal(5)
                };

                var facturaId = lector.GetGuid(1);
                if (!detallesPorFactura.TryGetValue(facturaId, out var lista))
                {
                    lista = new List<DetalleFactura>();
                    detallesPorFactura[facturaId] = lista;
                }

                lista.Add(detalle);
            }
        }

        foreach (var factura in facturas)
        {
            if (detallesPorFactura.TryGetValue(factura.Id, out var detalles))
            {
                factura.Detalles.Clear();
                factura.Detalles.AddRange(detalles);
            }
        }

        return facturas;
    }

    private static List<Producto> CargarProductos(SqlConnection conexion)
    {
        var categorias = new Dictionary<int, CategoriaProducto>();
        using (var comando = new SqlCommand("SELECT CategoriaProductoId, Nombre FROM CategoriasProducto;", conexion))
        using (var lector = comando.ExecuteReader())
        {
            while (lector.Read())
            {
                if (Enum.TryParse<CategoriaProducto>(lector.GetString(1), true, out var categoria))
                {
                    categorias[lector.GetByte(0)] = categoria;
                }
            }
        }

        const string sql = @"
            SELECT ProductoId, CategoriaProductoId, Nombre, Precio, Stock, StockMinimo, Vencimiento
            FROM Productos;";

        var lista = new List<Producto>();
        using var comandoFinal = new SqlCommand(sql, conexion);
        using var lectorFinal = comandoFinal.ExecuteReader();
        while (lectorFinal.Read())
        {
            lista.Add(new Producto
            {
                Id = lectorFinal.GetGuid(0),
                Categoria = categorias.TryGetValue(lectorFinal.GetByte(1), out var categoria) ? categoria : CategoriaProducto.Insumo,
                Nombre = lectorFinal.GetString(2),
                Precio = lectorFinal.GetDecimal(3),
                Stock = lectorFinal.GetInt32(4),
                StockMinimo = lectorFinal.GetInt32(5),
                Vencimiento = lectorFinal.IsDBNull(6) ? null : lectorFinal.GetDateTime(6).Date
            });
        }

        return lista;
    }

    private static void GuardarPersonas(SqlConnection conexion, SqlTransaction transaccion, ClinicaData datos)
    {
        foreach (var cliente in datos.Clientes)
        {
            EjecutarUpsertPersona(conexion, transaccion, cliente, "C");
            EjecutarUpsertCliente(conexion, transaccion, cliente);
        }

        foreach (var empleado in datos.Empleados)
        {
            EjecutarUpsertPersona(conexion, transaccion, empleado, "E");
            EjecutarUpsertEmpleado(conexion, transaccion, empleado);
            EjecutarUpsertSubtipoEmpleado(conexion, transaccion, empleado);
        }
    }

    private static void EjecutarUpsertPersona(SqlConnection conexion, SqlTransaction transaccion, Persona persona, string tipo)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Personas WHERE PersonaId = @Id)
                UPDATE Personas
                SET Nombre = @Nombre, Apellido = @Apellido, Documento = @Documento,
                    Telefono = @Telefono, Email = @Email, TipoPersona = @Tipo,
                    ActualizadoEn = SYSDATETIME()
                WHERE PersonaId = @Id;
            ELSE
                INSERT INTO Personas (PersonaId, Nombre, Apellido, Documento, Telefono, Email, TipoPersona, CreadoEn)
                VALUES (@Id, @Nombre, @Apellido, @Documento, @Telefono, @Email, @Tipo, SYSDATETIME());";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", persona.Id);
        comando.Parameters.AddWithValue("@Nombre", persona.Nombre);
        comando.Parameters.AddWithValue("@Apellido", persona.Apellido);
        comando.Parameters.AddWithValue("@Documento", persona.Documento);
        comando.Parameters.AddWithValue("@Telefono", persona.Telefono);
        comando.Parameters.AddWithValue("@Email", (object?)persona.Email ?? DBNull.Value);
        comando.Parameters.AddWithValue("@Tipo", tipo);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertCliente(SqlConnection conexion, SqlTransaction transaccion, Cliente cliente)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Clientes WHERE ClienteId = @Id)
                UPDATE Clientes
                SET Direccion = @Direccion, FechaRegistro = @FechaRegistro, PuntosFidelidad = @Puntos
                WHERE ClienteId = @Id;
            ELSE
                INSERT INTO Clientes (ClienteId, Direccion, FechaRegistro, PuntosFidelidad)
                VALUES (@Id, @Direccion, @FechaRegistro, @Puntos);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", cliente.Id);
        comando.Parameters.AddWithValue("@Direccion", (object?)cliente.Direccion ?? DBNull.Value);
        comando.Parameters.AddWithValue("@FechaRegistro", cliente.FechaRegistro.Date);
        comando.Parameters.AddWithValue("@Puntos", cliente.PuntosFidelidad);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertEmpleado(SqlConnection conexion, SqlTransaction transaccion, Empleado empleado)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Empleados WHERE EmpleadoId = @Id)
                UPDATE Empleados
                SET SalarioBase = @SalarioBase, FechaIngreso = @FechaIngreso, Activo = @Activo, TipoEmpleado = @Tipo
                WHERE EmpleadoId = @Id;
            ELSE
                INSERT INTO Empleados (EmpleadoId, SalarioBase, FechaIngreso, Activo, TipoEmpleado)
                VALUES (@Id, @SalarioBase, @FechaIngreso, @Activo, @Tipo);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", empleado.Id);
        comando.Parameters.AddWithValue("@SalarioBase", empleado.SalarioBase);
        comando.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso.Date);
        comando.Parameters.AddWithValue("@Activo", empleado.Activo);
        comando.Parameters.AddWithValue("@Tipo", empleado switch
        {
            Veterinario => "V",
            Recepcionista => "R",
            _ => "A"
        });
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertSubtipoEmpleado(SqlConnection conexion, SqlTransaction transaccion, Empleado empleado)
    {
        if (empleado is Veterinario veterinario)
        {
            EjecutarUpsertVeterinario(conexion, transaccion, veterinario);
        }
        else if (empleado is Recepcionista recepcionista)
        {
            EjecutarUpsertRecepcionista(conexion, transaccion, recepcionista);
        }
        else if (empleado is AuxiliarVeterinario auxiliar)
        {
            EjecutarUpsertAuxiliar(conexion, transaccion, auxiliar);
        }
    }

    private static void EjecutarUpsertVeterinario(SqlConnection conexion, SqlTransaction transaccion, Veterinario veterinario)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Veterinarios WHERE VeterinarioId = @Id)
                UPDATE Veterinarios
                SET Especialidad = @Especialidad, NumeroColegiatura = @Numero, AniosExperiencia = @Anios
                WHERE VeterinarioId = @Id;
            ELSE
                INSERT INTO Veterinarios (VeterinarioId, Especialidad, NumeroColegiatura, AniosExperiencia)
                VALUES (@Id, @Especialidad, @Numero, @Anios);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", veterinario.Id);
        comando.Parameters.AddWithValue("@Especialidad", veterinario.Especialidad);
        comando.Parameters.AddWithValue("@Numero", veterinario.NumeroColegiatura);
        comando.Parameters.AddWithValue("@Anios", veterinario.AniosExperiencia);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertRecepcionista(SqlConnection conexion, SqlTransaction transaccion, Recepcionista recepcionista)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Recepcionistas WHERE RecepcionistaId = @Id)
                UPDATE Recepcionistas SET Turno = @Turno WHERE RecepcionistaId = @Id;
            ELSE
                INSERT INTO Recepcionistas (RecepcionistaId, Turno) VALUES (@Id, @Turno);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", recepcionista.Id);
        comando.Parameters.AddWithValue("@Turno", recepcionista.Turno);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertAuxiliar(SqlConnection conexion, SqlTransaction transaccion, AuxiliarVeterinario auxiliar)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM AuxiliaresVeterinarios WHERE AuxiliarId = @Id)
                UPDATE AuxiliaresVeterinarios SET Certificacion = @Certificacion WHERE AuxiliarId = @Id;
            ELSE
                INSERT INTO AuxiliaresVeterinarios (AuxiliarId, Certificacion) VALUES (@Id, @Certificacion);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", auxiliar.Id);
        comando.Parameters.AddWithValue("@Certificacion", auxiliar.Certificacion);
        comando.ExecuteNonQuery();
    }

    private static void GuardarMascotas(SqlConnection conexion, SqlTransaction transaccion, ClinicaData datos)
    {
        foreach (var mascota in datos.Mascotas)
        {
            EjecutarUpsertMascota(conexion, transaccion, mascota);
            EjecutarUpsertSubtipoMascota(conexion, transaccion, mascota);
        }
    }

    private static void EjecutarUpsertMascota(SqlConnection conexion, SqlTransaction transaccion, Mascota mascota)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Mascotas WHERE MascotaId = @Id)
                UPDATE Mascotas
                SET ClienteId = @ClienteId, EspecieId = @EspecieId, Nombre = @Nombre, Raza = @Raza,
                    FechaNacimiento = @FechaNacimiento, PesoKg = @PesoKg, Sexo = @Sexo,
                    Alergias = @Alergias, Notas = @Notas, Esterilizado = @Esterilizado,
                    UltimaVisita = @UltimaVisita, ActualizadoEn = SYSDATETIME()
                WHERE MascotaId = @Id;
            ELSE
                INSERT INTO Mascotas (MascotaId, ClienteId, EspecieId, Nombre, Raza, FechaNacimiento, PesoKg, Sexo, Alergias, Notas, Esterilizado, UltimaVisita, CreadoEn)
                VALUES (@Id, @ClienteId, @EspecieId, @Nombre, @Raza, @FechaNacimiento, @PesoKg, @Sexo, @Alergias, @Notas, @Esterilizado, @UltimaVisita, SYSDATETIME());";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", mascota.Id);
        comando.Parameters.AddWithValue("@ClienteId", mascota.ClienteId);
        comando.Parameters.AddWithValue("@EspecieId", mascota switch
        {
            Perro => 1,
            Gato => 2,
            Ave => 3,
            _ => 4
        });
        comando.Parameters.AddWithValue("@Nombre", mascota.Nombre);
        comando.Parameters.AddWithValue("@Raza", mascota.Raza);
        comando.Parameters.AddWithValue("@FechaNacimiento", mascota.FechaNacimiento.Date);
        comando.Parameters.AddWithValue("@PesoKg", mascota.PesoKg);
        comando.Parameters.AddWithValue("@Sexo", mascota.Sexo switch
        {
            SexoMascota.Macho => "M",
            SexoMascota.Hembra => "H",
            _ => "D"
        });
        comando.Parameters.AddWithValue("@Alergias", (object?)mascota.Alergias ?? DBNull.Value);
        comando.Parameters.AddWithValue("@Notas", (object?)mascota.Notas ?? DBNull.Value);
        comando.Parameters.AddWithValue("@Esterilizado", mascota.Esterilizado);
        comando.Parameters.AddWithValue("@UltimaVisita", (object?)mascota.UltimaVisita ?? DBNull.Value);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertSubtipoMascota(SqlConnection conexion, SqlTransaction transaccion, Mascota mascota)
    {
        if (mascota is Perro perro)
        {
            EjecutarUpsertPerro(conexion, transaccion, mascota.Id, perro);
        }
        else if (mascota is Gato gato)
        {
            EjecutarUpsertGato(conexion, transaccion, mascota.Id, gato);
        }
        else if (mascota is Ave ave)
        {
            EjecutarUpsertAve(conexion, transaccion, mascota.Id, ave);
        }
        else if (mascota is Conejo conejo)
        {
            EjecutarUpsertConejo(conexion, transaccion, mascota.Id, conejo);
        }
    }

    private static void EjecutarUpsertPerro(SqlConnection conexion, SqlTransaction transaccion, Guid id, Perro perro)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Perros WHERE MascotaId = @Id)
                UPDATE Perros SET Talla = @Talla, Entrenado = @Entrenado WHERE MascotaId = @Id;
            ELSE
                INSERT INTO Perros (MascotaId, Talla, Entrenado) VALUES (@Id, @Talla, @Entrenado);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Talla", perro.Talla.ToString());
        comando.Parameters.AddWithValue("@Entrenado", perro.Entrenado);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertGato(SqlConnection conexion, SqlTransaction transaccion, Guid id, Gato gato)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Gatos WHERE MascotaId = @Id)
                UPDATE Gatos SET EsIndoor = @EsIndoor WHERE MascotaId = @Id;
            ELSE
                INSERT INTO Gatos (MascotaId, EsIndoor) VALUES (@Id, @EsIndoor);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@EsIndoor", gato.EsIndoor);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertAve(SqlConnection conexion, SqlTransaction transaccion, Guid id, Ave ave)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Aves WHERE MascotaId = @Id)
                UPDATE Aves SET PuedeVolar = @PuedeVolar, ColorPlumaje = @Color WHERE MascotaId = @Id;
            ELSE
                INSERT INTO Aves (MascotaId, PuedeVolar, ColorPlumaje) VALUES (@Id, @PuedeVolar, @Color);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@PuedeVolar", ave.PuedeVolar);
        comando.Parameters.AddWithValue("@Color", ave.ColorPlumaje);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertConejo(SqlConnection conexion, SqlTransaction transaccion, Guid id, Conejo conejo)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Conejos WHERE MascotaId = @Id)
                UPDATE Conejos SET LargoOrejasCm = @Largo WHERE MascotaId = @Id;
            ELSE
                INSERT INTO Conejos (MascotaId, LargoOrejasCm) VALUES (@Id, @Largo);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Largo", conejo.LargoOrejasCm);
        comando.ExecuteNonQuery();
    }

    private static void GuardarServicios(SqlConnection conexion, SqlTransaction transaccion, ClinicaData datos)
    {
        foreach (var servicio in datos.Servicios)
        {
            EjecutarUpsertServicio(conexion, transaccion, servicio);
            EjecutarUpsertSubtipoServicio(conexion, transaccion, servicio);
        }
    }

    private static void EjecutarUpsertServicio(SqlConnection conexion, SqlTransaction transaccion, ServicioVeterinario servicio)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Servicios WHERE ServicioId = @Id)
                UPDATE Servicios
                SET Codigo = @Codigo, Nombre = @Nombre, PrecioBase = @PrecioBase, DuracionMinutos = @Duracion,
                    RequiereAyuno = @Ayuno, Activo = @Activo, CategoriaServicioId = @CategoriaId, TipoServicio = @Tipo,
                    ActualizadoEn = SYSDATETIME()
                WHERE ServicioId = @Id;
            ELSE
                INSERT INTO Servicios (ServicioId, Codigo, Nombre, PrecioBase, DuracionMinutos, RequiereAyuno, Activo, CategoriaServicioId, TipoServicio, CreadoEn)
                VALUES (@Id, @Codigo, @Nombre, @PrecioBase, @Duracion, @Ayuno, @Activo, @CategoriaId, @Tipo, SYSDATETIME());";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", servicio.Id);
        comando.Parameters.AddWithValue("@Codigo", servicio.Codigo);
        comando.Parameters.AddWithValue("@Nombre", servicio.Nombre);
        comando.Parameters.AddWithValue("@PrecioBase", servicio.PrecioBase);
        comando.Parameters.AddWithValue("@Duracion", servicio.DuracionMinutos);
        comando.Parameters.AddWithValue("@Ayuno", servicio.RequiereAyuno);
        comando.Parameters.AddWithValue("@Activo", servicio.Activo);
        comando.Parameters.AddWithValue("@CategoriaId", servicio.Categoria switch
        {
            "Consulta" => 1,
            "Vacunación" => 2,
            "Cirugía" => 3,
            "Estética" => 4,
            _ => 5
        });
        comando.Parameters.AddWithValue("@Tipo", servicio switch
        {
            ConsultaGeneral => "C",
            Vacunacion => "V",
            Cirugia => "G",
            Peluqueria => "P",
            _ => "H"
        });
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertSubtipoServicio(SqlConnection conexion, SqlTransaction transaccion, ServicioVeterinario servicio)
    {
        if (servicio is ConsultaGeneral consulta)
        {
            EjecutarUpsertConsulta(conexion, transaccion, servicio.Id, consulta);
        }
        else if (servicio is Vacunacion vacunacion)
        {
            EjecutarUpsertVacunacion(conexion, transaccion, servicio.Id, vacunacion);
        }
        else if (servicio is Cirugia cirugia)
        {
            EjecutarUpsertCirugia(conexion, transaccion, servicio.Id, cirugia);
        }
        else if (servicio is Peluqueria peluqueria)
        {
            EjecutarUpsertPeluqueria(conexion, transaccion, servicio.Id, peluqueria);
        }
        else if (servicio is Hospitalizacion hospitalizacion)
        {
            EjecutarUpsertHospitalizacion(conexion, transaccion, servicio.Id, hospitalizacion);
        }
    }

    private static void EjecutarUpsertConsulta(SqlConnection conexion, SqlTransaction transaccion, Guid id, ConsultaGeneral consulta)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM ConsultasGenerales WHERE ServicioId = @Id)
                UPDATE ConsultasGenerales SET IncluyeReceta = @Incluye WHERE ServicioId = @Id;
            ELSE
                INSERT INTO ConsultasGenerales (ServicioId, IncluyeReceta) VALUES (@Id, @Incluye);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Incluye", consulta.IncluyeReceta);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertVacunacion(SqlConnection conexion, SqlTransaction transaccion, Guid id, Vacunacion vacunacion)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Vacunaciones WHERE ServicioId = @Id)
                UPDATE Vacunaciones SET Vacuna = @Vacuna, ProximaDosisMeses = @Proxima WHERE ServicioId = @Id;
            ELSE
                INSERT INTO Vacunaciones (ServicioId, Vacuna, ProximaDosisMeses) VALUES (@Id, @Vacuna, @Proxima);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Vacuna", vacunacion.Vacuna);
        comando.Parameters.AddWithValue("@Proxima", vacunacion.ProximaDosisMeses);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertCirugia(SqlConnection conexion, SqlTransaction transaccion, Guid id, Cirugia cirugia)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Cirugias WHERE ServicioId = @Id)
                UPDATE Cirugias SET NivelRiesgo = @Nivel, RequiereHospitalizacion = @Hospitalizacion WHERE ServicioId = @Id;
            ELSE
                INSERT INTO Cirugias (ServicioId, NivelRiesgo, RequiereHospitalizacion) VALUES (@Id, @Nivel, @Hospitalizacion);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Nivel", cirugia.Riesgo switch
        {
            NivelRiesgo.Bajo => "B",
            NivelRiesgo.Alto => "A",
            _ => "M"
        });
        comando.Parameters.AddWithValue("@Hospitalizacion", cirugia.RequiereHospitalizacion);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertPeluqueria(SqlConnection conexion, SqlTransaction transaccion, Guid id, Peluqueria peluqueria)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Peluquerias WHERE ServicioId = @Id)
                UPDATE Peluquerias SET TipoCorte = @Tipo, IncluyeBano = @Bano WHERE ServicioId = @Id;
            ELSE
                INSERT INTO Peluquerias (ServicioId, TipoCorte, IncluyeBano) VALUES (@Id, @Tipo, @Bano);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Tipo", peluqueria.Tipo.ToString());
        comando.Parameters.AddWithValue("@Bano", peluqueria.IncluyeBano);
        comando.ExecuteNonQuery();
    }

    private static void EjecutarUpsertHospitalizacion(SqlConnection conexion, SqlTransaction transaccion, Guid id, Hospitalizacion hospitalizacion)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Hospitalizaciones WHERE ServicioId = @Id)
                UPDATE Hospitalizaciones SET Dias = @Dias, CostoPorDia = @Costo WHERE ServicioId = @Id;
            ELSE
                INSERT INTO Hospitalizaciones (ServicioId, Dias, CostoPorDia) VALUES (@Id, @Dias, @Costo);";

        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Id", id);
        comando.Parameters.AddWithValue("@Dias", hospitalizacion.Dias);
        comando.Parameters.AddWithValue("@Costo", hospitalizacion.CostoPorDia);
        comando.ExecuteNonQuery();
    }

    private static void GuardarCitas(SqlConnection conexion, SqlTransaction transaccion, ClinicaData datos)
    {
        foreach (var cita in datos.Citas)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Citas WHERE CitaId = @Id)
                    UPDATE Citas
                    SET ClienteId = @ClienteId, MascotaId = @MascotaId, VeterinarioId = @VeterinarioId,
                        ServicioId = @ServicioId, EstadoCitaId = @EstadoId, FechaHora = @FechaHora,
                        Motivo = @Motivo, Diagnostico = @Diagnostico, PesoRegistrado = @Peso,
                        FechaAtencion = @FechaAtencion, Observaciones = @Observaciones,
                        MotivoCancelacion = @MotivoCancelacion, ActualizadoEn = SYSDATETIME()
                    WHERE CitaId = @Id;
                ELSE
                    INSERT INTO Citas (CitaId, ClienteId, MascotaId, VeterinarioId, ServicioId, EstadoCitaId, FechaHora, Motivo, Diagnostico, PesoRegistrado, FechaAtencion, Observaciones, MotivoCancelacion, CreadoEn)
                    VALUES (@Id, @ClienteId, @MascotaId, @VeterinarioId, @ServicioId, @EstadoId, @FechaHora, @Motivo, @Diagnostico, @Peso, @FechaAtencion, @Observaciones, @MotivoCancelacion, SYSDATETIME());";

            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.AddWithValue("@Id", cita.Id);
            comando.Parameters.AddWithValue("@ClienteId", cita.ClienteId);
            comando.Parameters.AddWithValue("@MascotaId", cita.MascotaId);
            comando.Parameters.AddWithValue("@VeterinarioId", cita.VeterinarioId);
            comando.Parameters.AddWithValue("@ServicioId", cita.ServicioId);
            comando.Parameters.AddWithValue("@EstadoId", cita.Estado switch
            {
                EstadoCita.Programada => 1,
                EstadoCita.Confirmada => 2,
                EstadoCita.EnAtencion => 3,
                EstadoCita.Atendida => 4,
                EstadoCita.Cancelada => 5,
                _ => 6
            });
            comando.Parameters.AddWithValue("@FechaHora", cita.FechaHora);
            comando.Parameters.AddWithValue("@Motivo", cita.Motivo);
            comando.Parameters.AddWithValue("@Diagnostico", (object?)cita.Diagnostico ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Peso", (object?)cita.PesoRegistrado ?? DBNull.Value);
            comando.Parameters.AddWithValue("@FechaAtencion", (object?)cita.FechaAtencion ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Observaciones", (object?)cita.Observaciones ?? DBNull.Value);
            comando.Parameters.AddWithValue("@MotivoCancelacion", (object?)cita.MotivoCancelacion ?? DBNull.Value);
            comando.ExecuteNonQuery();
        }
    }

    private static void GuardarFacturas(SqlConnection conexion, SqlTransaction transaccion, ClinicaData datos)
    {
        foreach (var factura in datos.Facturas)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Facturas WHERE FacturaId = @Id)
                    UPDATE Facturas
                    SET Numero = @Numero, ClienteId = @ClienteId, CitaId = @CitaId, Fecha = @Fecha,
                        DescuentoPorcentaje = @Descuento, ImpuestoPorcentaje = @Impuesto,
                        Pagada = @Pagada, MetodoPagoId = @MetodoPagoId, FechaPago = @FechaPago,
                        ActualizadoEn = SYSDATETIME()
                    WHERE FacturaId = @Id;
                ELSE
                    INSERT INTO Facturas (FacturaId, Numero, ClienteId, CitaId, Fecha, DescuentoPorcentaje, ImpuestoPorcentaje, Pagada, MetodoPagoId, FechaPago, CreadoEn)
                    VALUES (@Id, @Numero, @ClienteId, @CitaId, @Fecha, @Descuento, @Impuesto, @Pagada, @MetodoPagoId, @FechaPago, SYSDATETIME());";

            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.AddWithValue("@Id", factura.Id);
            comando.Parameters.AddWithValue("@Numero", factura.Numero);
            comando.Parameters.AddWithValue("@ClienteId", factura.ClienteId);
            comando.Parameters.AddWithValue("@CitaId", (object?)factura.CitaId ?? DBNull.Value);
            comando.Parameters.AddWithValue("@Fecha", factura.Fecha);
            comando.Parameters.AddWithValue("@Descuento", factura.DescuentoPorcentaje);
            comando.Parameters.AddWithValue("@Impuesto", factura.ImpuestoPorcentaje);
            comando.Parameters.AddWithValue("@Pagada", factura.Pagada);
            comando.Parameters.AddWithValue("@MetodoPagoId", factura.MetodoPago.HasValue ? (object)(int)factura.MetodoPago.Value : DBNull.Value);
            comando.Parameters.AddWithValue("@FechaPago", (object?)factura.FechaPago ?? DBNull.Value);
            comando.ExecuteNonQuery();

            using var comandoDetalle = new SqlCommand("DELETE FROM DetallesFactura WHERE FacturaId = @Id;", conexion, transaccion);
            comandoDetalle.Parameters.AddWithValue("@Id", factura.Id);
            comandoDetalle.ExecuteNonQuery();

            foreach (var detalle in factura.Detalles)
            {
                const string sqlDetalle = @"
                    INSERT INTO DetallesFactura (DetalleFacturaId, FacturaId, ServicioId, Descripcion, Cantidad, PrecioUnitario)
                    VALUES (NEWID(), @FacturaId, @ServicioId, @Descripcion, @Cantidad, @PrecioUnitario);";

                using var comandoInsert = new SqlCommand(sqlDetalle, conexion, transaccion);
                comandoInsert.Parameters.AddWithValue("@FacturaId", factura.Id);
                comandoInsert.Parameters.AddWithValue("@ServicioId", detalle.ServicioId == Guid.Empty ? DBNull.Value : detalle.ServicioId);
                comandoInsert.Parameters.AddWithValue("@Descripcion", detalle.Descripcion);
                comandoInsert.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                comandoInsert.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                comandoInsert.ExecuteNonQuery();
            }
        }
    }

    private static void GuardarProductos(SqlConnection conexion, SqlTransaction transaccion, ClinicaData datos)
    {
        foreach (var producto in datos.Productos)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Productos WHERE ProductoId = @Id)
                    UPDATE Productos
                    SET CategoriaProductoId = @CategoriaId, Nombre = @Nombre, Precio = @Precio,
                        Stock = @Stock, StockMinimo = @StockMinimo, Vencimiento = @Vencimiento,
                        ActualizadoEn = SYSDATETIME()
                    WHERE ProductoId = @Id;
                ELSE
                    INSERT INTO Productos (ProductoId, CategoriaProductoId, Nombre, Precio, Stock, StockMinimo, Vencimiento, CreadoEn)
                    VALUES (@Id, @CategoriaId, @Nombre, @Precio, @Stock, @StockMinimo, @Vencimiento, SYSDATETIME());";

            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.AddWithValue("@Id", producto.Id);
            comando.Parameters.AddWithValue("@CategoriaId", producto.Categoria switch
            {
                CategoriaProducto.Medicamento => 1,
                CategoriaProducto.Alimento => 2,
                CategoriaProducto.Insumo => 3,
                _ => 4
            });
            comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
            comando.Parameters.AddWithValue("@Precio", producto.Precio);
            comando.Parameters.AddWithValue("@Stock", producto.Stock);
            comando.Parameters.AddWithValue("@StockMinimo", producto.StockMinimo);
            comando.Parameters.AddWithValue("@Vencimiento", (object?)producto.Vencimiento ?? DBNull.Value);
            comando.ExecuteNonQuery();
        }
    }
}
