using System;
using System.IO;
using System.Text.Json;
using HuellaVet.Application.Services;
using HuellaVet.Infrastructure.Persistencia;
using Microsoft.Extensions.Configuration;

namespace HuellaVet.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        try
        {
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var cadena = configuration.GetConnectionString("HuellaVet")
                ?? "Server=localhost;Database=HuellaVet;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

            var almacen = new SqlAlmacenClinica(cadena);
            var clinica = new ClinicaService(almacen);
            var facturacion = new FacturacionService(almacen);
            var reportes = new ReporteService(almacen);

            System.Windows.Forms.Application.Run(new MainForm(clinica, facturacion, reportes));
        }
        catch (Exception ex)
        {
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "crash.log"), ex.ToString());
            MessageBox.Show(ex.Message, "HuellaVet - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
