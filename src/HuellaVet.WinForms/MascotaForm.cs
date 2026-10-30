using System;
using System.Linq;
using System.Windows.Forms;
using HuellaVet.Application.Services;
using HuellaVet.Domain.Mascotas;
using HuellaVet.Domain.Personas;

namespace HuellaVet.WinForms;

public sealed class MascotaForm : Form
{
    private readonly ComboBox _cmbCliente = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtNombre = new() { Width = 250 };
    private readonly ComboBox _cmbEspecie = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtRaza = new() { Width = 250 };
    private readonly DateTimePicker _dtpNacimiento = new() { Width = 250, Format = DateTimePickerFormat.Short };
    private readonly NumericUpDown _numPeso = new() { Width = 250, Minimum = 0.01m, Maximum = 200m, DecimalPlaces = 2, Value = 5m };
    private readonly ComboBox _cmbSexo = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _txtAlergias = new() { Width = 250 };
    private readonly TextBox _txtNotas = new() { Width = 250 };

    public Guid ClienteId => (Guid)_cmbCliente.SelectedValue!;
    public string Nombre => _txtNombre.Text;
    public string Especie => _cmbEspecie.Text;
    public string Raza => _txtRaza.Text;
    public DateTime FechaNacimiento => _dtpNacimiento.Value;
    public decimal PesoKg => _numPeso.Value;
    public SexoMascota Sexo => _cmbSexo.Text switch { "Macho" => SexoMascota.Macho, "Hembra" => SexoMascota.Hembra, _ => SexoMascota.Desconocido };
    public string Alergias => _txtAlergias.Text;
    public string Notas => _txtNotas.Text;

    public MascotaForm(IEnumerable<Cliente> clientes)
    {
        Text = "Nueva mascota";
        Width = 430;
        Height = 470;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoScroll = true;

        _cmbCliente.DataSource = clientes.ToList();
        _cmbCliente.DisplayMember = "NombreCompleto";
        _cmbCliente.ValueMember = "Id";

        _cmbEspecie.Items.AddRange(new object[] { "Perro", "Gato", "Ave", "Conejo" });
        _cmbEspecie.SelectedIndex = 0;
        _cmbSexo.Items.AddRange(new object[] { "Desconocido", "Macho", "Hembra" });
        _cmbSexo.SelectedIndex = 0;

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
        Agregar(layout, "Nombre", _txtNombre);
        Agregar(layout, "Especie", _cmbEspecie);
        Agregar(layout, "Raza", _txtRaza);
        Agregar(layout, "Nacimiento", _dtpNacimiento);
        Agregar(layout, "Peso (kg)", _numPeso);
        Agregar(layout, "Sexo", _cmbSexo);
        Agregar(layout, "Alergias", _txtAlergias);
        Agregar(layout, "Notas", _txtNotas);

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
