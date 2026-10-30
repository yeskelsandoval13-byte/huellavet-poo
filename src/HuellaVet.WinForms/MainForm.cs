using System;
using System.Linq;
using System.Windows.Forms;
using HuellaVet.Application.Services;
using HuellaVet.Domain;
using HuellaVet.Domain.Citas;
using HuellaVet.Domain.Facturacion;
using HuellaVet.Domain.Mascotas;

namespace HuellaVet.WinForms;

public sealed class MainForm : Form
{
    private readonly ClinicaService _clinica;
    private readonly FacturacionService _facturacion;
    private readonly ReporteService _reportes;

    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _dgvClientes = new() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly DataGridView _dgvMascotas = new() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly DataGridView _dgvCitas = new() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly DataGridView _dgvServicios = new() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly DataGridView _dgvProductos = new() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly DataGridView _dgvFacturas = new() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly TextBox _txtReportes = new() { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical, Font = new System.Drawing.Font("Consolas", 10) };

    public MainForm(ClinicaService clinica, FacturacionService facturacion, ReporteService reportes)
    {
        _clinica = clinica;
        _facturacion = facturacion;
        _reportes = reportes;

        Text = "HuellaVet - Sistema de gestión clínica veterinaria";
        Width = 1180;
        Height = 720;
        StartPosition = FormStartPosition.CenterScreen;

        var menu = new MenuStrip();
        var archivo = new ToolStripMenuItem("Archivo");
        archivo.DropDownItems.Add("Salir", null, (_, _) => Close());
        menu.Items.Add(archivo);

        var ayuda = new ToolStripMenuItem("Ayuda");
        ayuda.DropDownItems.Add("Acerca de", null, (_, _) => MessageBox.Show("HuellaVet\nAvance con SQL Server, Windows Forms y POO.", "Acerca de"));
        menu.Items.Add(ayuda);
        MainMenuStrip = menu;
        Controls.Add(menu);

        CrearPestana("Clientes", _dgvClientes, new (string, EventHandler)[] { ("Nuevo cliente", BtnNuevoCliente_Click), ("Actualizar contacto", BtnActualizarCliente_Click) });
        CrearPestana("Mascotas", _dgvMascotas, new (string, EventHandler)[] { ("Nueva mascota", BtnNuevaMascota_Click) });
        CrearPestana("Agenda", _dgvCitas, new (string, EventHandler)[] { ("Programar cita", BtnProgramarCita_Click), ("Atender seleccionada", BtnAtenderCita_Click) });
        CrearPestana("Servicios", _dgvServicios, Array.Empty<(string, EventHandler)>());
        CrearPestana("Inventario", _dgvProductos, new (string, EventHandler)[] { ("Reponer 5 unidades", BtnReponer_Click) });
        CrearPestana("Facturación", _dgvFacturas, new (string, EventHandler)[] { ("Facturar cita atendida", BtnFacturar_Click) });

        var tabReportes = new TabPage("Reportes");
        var panelReportes = new Panel { Dock = DockStyle.Fill };
        var btnReportes = new Button { Text = "Actualizar reportes", Dock = DockStyle.Top, Height = 34 };
        btnReportes.Click += (_, _) => CargarReportes();
        panelReportes.Controls.Add(_txtReportes);
        panelReportes.Controls.Add(btnReportes);
        tabReportes.Controls.Add(panelReportes);
        _tabs.TabPages.Add(tabReportes);

        Controls.Add(_tabs);
        Controls.Add(menu);

        CargarTodo();
    }

    private void CrearPestana(string titulo, DataGridView dgv, (string Texto, EventHandler Evento)[] botones)
    {
        var tab = new TabPage(titulo);
        var panel = new Panel { Dock = DockStyle.Fill };
        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(6), FlowDirection = FlowDirection.LeftToRight };

        foreach (var (texto, evento) in botones)
        {
            var boton = new Button { Text = texto, Width = 180, Height = 28 };
            boton.Click += evento;
            barra.Controls.Add(boton);
        }

        panel.Controls.Add(dgv);
        panel.Controls.Add(barra);
        tab.Controls.Add(panel);
        _tabs.TabPages.Add(tab);
    }

    private void CargarTodo()
    {
        CargarClientes();
        CargarMascotas();
        CargarCitas();
        CargarServicios();
        CargarProductos();
        CargarFacturas();
        CargarReportes();
    }

    private void CargarClientes()
    {
        _dgvClientes.DataSource = _clinica.ObtenerClientes()
            .Select(c => new
            {
                c.Id,
                Nombre = c.Nombre + " " + c.Apellido,
                c.Documento,
                c.Telefono,
                c.Email,
                c.Direccion,
                c.PuntosFidelidad
            })
            .ToList();
    }

    private void CargarMascotas()
    {
        _dgvMascotas.DataSource = _clinica.ObtenerMascotas()
            .Select(m => new
            {
                m.Id,
                m.Nombre,
                Especie = m.Especie,
                m.Raza,
                m.PesoKg,
                Cliente = _clinica.ObtenerClientes().FirstOrDefault(c => c.Id == m.ClienteId)?.NombreCompleto ?? ""
            })
            .ToList();
    }

    private void CargarCitas()
    {
        _dgvCitas.DataSource = _clinica.ObtenerCitas()
            .Select(c => new
            {
                c.Id,
                Fecha = c.FechaHora,
                Cliente = _clinica.ObtenerClientes().FirstOrDefault(x => x.Id == c.ClienteId)?.NombreCompleto ?? "",
                Mascota = _clinica.ObtenerMascotas().FirstOrDefault(x => x.Id == c.MascotaId)?.Nombre ?? "",
                Estado = c.Estado.ToString(),
                c.Motivo
            })
            .ToList();
    }

    private void CargarServicios()
    {
        _dgvServicios.DataSource = _clinica.ObtenerServicios()
            .Select(s => new
            {
                s.Id,
                s.Codigo,
                s.Nombre,
                s.Categoria,
                s.PrecioBase,
                s.DuracionMinutos,
                s.Activo
            })
            .ToList();
    }

    private void CargarProductos()
    {
        _dgvProductos.DataSource = _clinica.ObtenerProductos()
            .Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Categoria,
                p.Precio,
                p.Stock,
                p.StockMinimo,
                Estado = p.StockBajo ? "Bajo" : "OK"
            })
            .ToList();
    }

    private void CargarFacturas()
    {
        _dgvFacturas.DataSource = _facturacion.ObtenerFacturas()
            .Select(f => new
            {
                f.Id,
                f.Numero,
                Cliente = _clinica.ObtenerClientes().FirstOrDefault(c => c.Id == f.ClienteId)?.NombreCompleto ?? "",
                f.Fecha,
                f.Subtotal,
                f.Descuento,
                f.Impuesto,
                f.Total,
                f.Pagada
            })
            .ToList();
    }

    private void CargarReportes()
    {
        var resumen = _reportes.ObtenerResumen();
        _txtReportes.Text =
            $"Pacientes: {resumen.Pacientes}\r\n" +
            $"Citas de hoy: {resumen.CitasHoy}\r\n" +
            $"Ingresos del mes: {resumen.IngresosMes:C}\r\n" +
            $"Stock bajo: {resumen.StockBajo}\r\n" +
            $"Próximas vacunas: {resumen.CitasPendientes}\r\n\r\n" +
            "Ingresos por servicio:\r\n" +
            string.Join("\r\n", _reportes.IngresosPorServicio().Select(x => $"- {x.Servicio}: {x.Total:C}"));
    }

    private void BtnNuevoCliente_Click(object? sender, EventArgs e)
    {
        using var form = new ClienteForm();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _clinica.RegistrarCliente(form.Nombre, form.Apellido, form.Documento, form.Telefono, form.Email, form.Direccion);
                CargarClientes();
                MessageBox.Show("Cliente registrado correctamente.", "HuellaVet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void BtnActualizarCliente_Click(object? sender, EventArgs e)
    {
        if (_dgvClientes.CurrentRow?.DataBoundItem is null)
        {
            MessageBox.Show("Seleccione un cliente.", "HuellaVet");
            return;
        }

        var id = (Guid)_dgvClientes.CurrentRow.Cells["Id"].Value;
        using var form = new ClienteForm(esActualizacion: true);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _clinica.ActualizarContactoCliente(id, form.Telefono, form.Email, form.Direccion);
                CargarClientes();
                MessageBox.Show("Contacto actualizado.", "HuellaVet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void BtnNuevaMascota_Click(object? sender, EventArgs e)
    {
        using var form = new MascotaForm(_clinica.ObtenerClientes());
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _clinica.RegistrarMascota(form.ClienteId, form.Nombre, form.Especie, form.Raza, form.FechaNacimiento, form.PesoKg, form.Sexo, form.Alergias, form.Notas);
                CargarMascotas();
                MessageBox.Show("Mascota registrada correctamente.", "HuellaVet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void BtnProgramarCita_Click(object? sender, EventArgs e)
    {
        using var form = new CitaForm(_clinica);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                var cita = _clinica.ProgramarCita(form.ClienteId, form.MascotaId, form.VeterinarioId, form.ServicioId, form.FechaHora, form.Motivo);
                _clinica.ConfirmarCita(cita.Id);
                CargarCitas();
                MessageBox.Show("Cita programada y confirmada.", "HuellaVet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    private void BtnAtenderCita_Click(object? sender, EventArgs e)
    {
        if (_dgvCitas.CurrentRow is null)
        {
            MessageBox.Show("Seleccione una cita.", "HuellaVet");
            return;
        }

        var id = (Guid)_dgvCitas.CurrentRow.Cells["Id"].Value;
        try
        {
            _clinica.IniciarAtencion(id);
            _clinica.AtenderCita(id, "Atención realizada desde Windows Forms.", 5.0m, "Demostración");
            CargarCitas();
            MessageBox.Show("Cita atendida.", "HuellaVet");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnReponer_Click(object? sender, EventArgs e)
    {
        if (_dgvProductos.CurrentRow is null)
        {
            MessageBox.Show("Seleccione un producto.", "HuellaVet");
            return;
        }

        var id = (Guid)_dgvProductos.CurrentRow.Cells["Id"].Value;

        try
        {
            _clinica.ReponerProducto(id, 5);
            CargarProductos();
            MessageBox.Show("Stock repuesto.", "HuellaVet");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnFacturar_Click(object? sender, EventArgs e)
    {
        var citaAtendida = _clinica.ObtenerCitas().FirstOrDefault(c => c.Estado == EstadoCita.Atendida && !_facturacion.ObtenerFacturas().Any(f => f.CitaId == c.Id));
        if (citaAtendida is null)
        {
            MessageBox.Show("No hay citas atendidas pendientes de facturar.", "HuellaVet");
            return;
        }

        try
        {
            var factura = _facturacion.GenerarFacturaParaCita(citaAtendida.Id, MetodoPago.Efectivo, 10m);
            CargarFacturas();
            CargarCitas();
            MessageBox.Show($"Factura {factura.Numero} generada. Total: {factura.Total:C}", "HuellaVet");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "HuellaVet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
