using System;
using System.Linq;
using System.Windows.Forms;
using HuellaVet.Application.Services;

namespace HuellaVet.WinForms;

public sealed class CitaForm : Form
{
    private readonly ClinicaService _clinica;
    private readonly ComboBox _cmbCliente = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbMascota = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbVeterinario = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbServicio = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpFecha = new() { Width = 250, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
    private readonly TextBox _txtMotivo = new() { Width = 250 };

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
        Width = 430;
        Height = 330;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoScroll = true;

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

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 0,
            Padding = new Padding(12),
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));

        Agregar(layout, "Cliente", _cmbCliente);
        Agregar(layout, "Mascota", _cmbMascota);
        Agregar(layout, "Veterinario", _cmbVeterinario);
        Agregar(layout, "Servicio", _cmbServicio);
        Agregar(layout, "Fecha y hora", _dtpFecha);
        Agregar(layout, "Motivo", _txtMotivo);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var btnAceptar = new Button { Text = "Aceptar", Width = 110, Height = 30 };
        var btnCancelar = new Button { Text = "Cancelar", Width = 110, Height = 30 };
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
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3) }, 0, fila);
        layout.Controls.Add(control, 1, fila);
        layout.RowCount++;
    }
}
