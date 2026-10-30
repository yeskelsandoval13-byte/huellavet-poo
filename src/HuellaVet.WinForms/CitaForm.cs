using System;
using System.Linq;
using System.Windows.Forms;
using HuellaVet.Application.Services;
using HuellaVet.Domain.Personas;

namespace HuellaVet.WinForms;

public sealed class CitaForm : Form
{
    private readonly ClinicaService _clinica;
    private readonly ComboBox _cmbCliente = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbMascota = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbVeterinario = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbServicio = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpFecha = new() { Width = 260, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
    private readonly TextBox _txtMotivo = new() { Width = 260 };

    public Guid ClienteId => (Guid)_cmbCliente.SelectedValue!;
    public Guid MascotaId => (Guid)_cmbMascota.SelectedValue!;
    public Guid VeterinarioId => (Guid)_cmbVeterinario.SelectedValue!;
    public Guid ServicioId => (Guid)_cmbServicio.SelectedValue!;
    public DateTime FechaHora => _dtpFecha.Value;
    public string Motivo => _txtMotivo.Text;

    public CitaForm(ClinicaService clinica)
    {
        _clinica = clinica;
        Text = "Programar cita";
        Width = 380;
        Height = 360;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;

        _cmbCliente.DataSource = clinica.ObtenerClientes().ToList();
        _cmbCliente.DisplayMember = "NombreCompleto";
        _cmbCliente.ValueMember = "Id";

        _cmbMascota.DataSource = clinica.ObtenerMascotas().ToList();
        _cmbMascota.DisplayMember = "Nombre";
        _cmbMascota.ValueMember = "Id";

        _cmbVeterinario.DataSource = clinica.ObtenerVeterinarios().Where(v => v.Activo).ToList();
        _cmbVeterinario.DisplayMember = "NombreCompleto";
        _cmbVeterinario.ValueMember = "Id";

        _cmbServicio.DataSource = clinica.ObtenerServicios().ToList();
        _cmbServicio.DisplayMember = "Nombre";
        _cmbServicio.ValueMember = "Id";

        _dtpFecha.Value = DateTime.Now.AddDays(1).Date.AddHours(9);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
        Agregar(layout, "Cliente", _cmbCliente);
        Agregar(layout, "Mascota", _cmbMascota);
        Agregar(layout, "Veterinario", _cmbVeterinario);
        Agregar(layout, "Servicio", _cmbServicio);
        Agregar(layout, "Fecha y hora", _dtpFecha);
        Agregar(layout, "Motivo", _txtMotivo);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
        var btnAceptar = new Button { Text = "Aceptar", Width = 100 };
        var btnCancelar = new Button { Text = "Cancelar", Width = 100 };
        btnAceptar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
        btnCancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        botones.Controls.Add(btnAceptar);
        botones.Controls.Add(btnCancelar);

        Controls.Add(layout);
        Controls.Add(botones);
    }

    private static void Agregar(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left }, 0, fila);
        layout.Controls.Add(control, 1, fila);
    }
}
