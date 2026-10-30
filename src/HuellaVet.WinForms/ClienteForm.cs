using System;
using System.Windows.Forms;
using HuellaVet.Domain.Mascotas;

namespace HuellaVet.WinForms;

public sealed class ClienteForm : Form
{
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;

    private readonly TextBox _txtNombre = new() { Width = 260 };
    private readonly TextBox _txtApellido = new() { Width = 260 };
    private readonly TextBox _txtDocumento = new() { Width = 260 };
    private readonly TextBox _txtTelefono = new() { Width = 260 };
    private readonly TextBox _txtEmail = new() { Width = 260 };
    private readonly TextBox _txtDireccion = new() { Width = 260 };

    public ClienteForm(bool esActualizacion = false)
    {
        Text = esActualizacion ? "Actualizar contacto" : "Nuevo cliente";
        Width = 360;
        Height = esActualizacion ? 260 : 340;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = esActualizacion ? 4 : 7, Padding = new Padding(10) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        if (!esActualizacion)
        {
            AgregarFila(layout, "Nombre", _txtNombre);
            AgregarFila(layout, "Apellido", _txtApellido);
            AgregarFila(layout, "Documento", _txtDocumento);
        }
        AgregarFila(layout, "Teléfono", _txtTelefono);
        AgregarFila(layout, "Correo", _txtEmail);
        AgregarFila(layout, "Dirección", _txtDireccion);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40 };
        var btnAceptar = new Button { Text = "Aceptar", Width = 100 };
        var btnCancelar = new Button { Text = "Cancelar", Width = 100 };
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

    private static void AgregarFila(TableLayoutPanel layout, string etiqueta, Control control)
    {
        var fila = layout.RowCount;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left }, 0, fila - 1);
        layout.Controls.Add(control, 1, fila - 1);
    }
}
