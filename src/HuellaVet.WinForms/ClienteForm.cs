using System;
using System.Windows.Forms;

namespace HuellaVet.WinForms;

public sealed class ClienteForm : Form
{
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;

    private readonly TextBox _txtNombre = new() { Width = 250 };
    private readonly TextBox _txtApellido = new() { Width = 250 };
    private readonly TextBox _txtDocumento = new() { Width = 250 };
    private readonly TextBox _txtTelefono = new() { Width = 250 };
    private readonly TextBox _txtEmail = new() { Width = 250 };
    private readonly TextBox _txtDireccion = new() { Width = 250 };

    public ClienteForm(bool esActualizacion = false)
    {
        Text = esActualizacion ? "Actualizar contacto" : "Nuevo cliente";
        Width = 420;
        Height = esActualizacion ? 230 : 330;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        AutoScroll = true;

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

        if (!esActualizacion)
        {
            Agregar(layout, "Nombre", _txtNombre);
            Agregar(layout, "Apellido", _txtApellido);
            Agregar(layout, "Documento", _txtDocumento);
        }
        Agregar(layout, "Teléfono", _txtTelefono);
        Agregar(layout, "Correo", _txtEmail);
        Agregar(layout, "Dirección", _txtDireccion);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, Padding = new Padding(8) };
        var btnAceptar = new Button { Text = "Aceptar", Width = 110, Height = 30 };
        var btnCancelar = new Button { Text = "Cancelar", Width = 110, Height = 30 };
        btnAceptar.Click += (_, _) =>
        {
            Nombre = _txtNombre.Text;
            Apellido = _txtApellido.Text;
            Documento = _txtDocumento.Text;
            Telefono = _txtTelefono.Text;
            Email = _txtEmail.Text;
            Direccion = _txtDireccion.Text;
            DialogResult = DialogResult.OK;
            Close();
        };
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
