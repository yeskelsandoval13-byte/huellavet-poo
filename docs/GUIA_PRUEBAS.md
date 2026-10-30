# Guía de pruebas y demostración · HuellaVet

## 1. Qué llevamos ahora

### Aplicación y POO
- Dominio rico en objetos: `Persona`, `Empleado`, `Mascota`, `ServicioVeterinario`, `Cita`, `Factura`, `Producto`.
- Herencia, polimorfismo y sobrecarga implementados.
- Laboratorio exacto de la Clase 03: `SeleccionFutbol`, `Forma`, `CalculadoraPrecios`.
- Casos de uso: clientes, mascotas, agenda, servicios, inventario, facturación y reportes.

### Interfaz
- **Consola** (`HuellaVet.Console`): menús, dashboard, demo no interactiva y modo `--demo`.
- **Windows Forms** (`HuellaVet.WinForms`): dashboard, clientes, mascotas, agenda, servicios, inventario, facturación y reportes.

### Persistencia
- **JSON** local con guardado atómico y respaldo de archivo dañado.
- **SQL Server** con `SqlAlmacenClinica` (ADO.NET), carga y guardado del agregado completo.

### Base de datos
- 26 tablas, 28 relaciones, 6 vistas, índices y restricciones.
- Script `docs/bd/06_huellavet_sqlserver.sql` ejecutado correctamente en SQL Server local.
- Normalización documentada en 3NF.

### Calidad
- 32 pruebas automatizadas aprobadas.
- Compilación Release sin errores.
- CI con GitHub Actions.

---

## 2. Cómo ejecutar

### Compilar todo
```powershell
dotnet build HuellaVet.sln --configuration Release
```

### Pruebas
```powershell
dotnet test HuellaVet.sln --configuration Release
```

### Consola
```powershell
dotnet run --project src/HuellaVet.Console
dotnet run --project src/HuellaVet.Console -- --demo
dotnet run --project src/HuellaVet.Console -- --reset
```

### Windows Forms
```powershell
dotnet run --project src/HuellaVet.WinForms
```

La aplicación Windows Forms se conecta a SQL Server usando `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "HuellaVet": "Server=localhost;Database=HuellaVet;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

### Base de datos
La base `HuellaVet` ya debe existir. Si se desea recrearla:

```powershell
sqlcmd -S localhost -E -Q "ALTER DATABASE HuellaVet SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE HuellaVet;" -C
sqlcmd -S localhost -E -i "docs\bd\06_huellavet_sqlserver.sql" -b -C
```

---

## 3. Flujo de demostración sugerido

1. Abrir **Windows Forms**.
2. Ver el dashboard con pacientes, citas, ingresos, stock bajo y vacunas.
3. Registrar un cliente.
4. Registrar una mascota asociada al cliente.
5. Programar una cita.
6. Atender la cita.
7. Generar la factura.
8. Reponer inventario.
9. Mostrar reportes.
10. Abrir la **consola** y ejecutar `--demo` para la parte de POO y Clase 03.
11. Mostrar el script SQL en SSMS y las tablas/vistas.

---

## 4. Archivos clave

| Archivo | Uso en la demo |
|---|---|
| `src/HuellaVet.WinForms/MainForm.cs` | Interfaz principal. |
| `src/HuellaVet.Infrastructure/Persistencia/SqlAlmacenClinica.cs` | Conexión real a SQL Server. |
| `docs/bd/06_huellavet_sqlserver.sql` | Script de base de datos. |
| `docs/bd/02_modelo_conceptual.drawio` | Diagrama ERD. |
| `docs/bd/03_modelo_logico.drawio` | Modelo lógico. |
| `docs/bd/05_modelo_fisico.drawio` | Modelo físico. |
| `docs/CLASE_03.md` | Explicación de la actividad. |
| `docs/GUION_DEFENSA.md` | Guion de exposición. |

---

## 5. Estado actual

| Componente | Estado |
|---|---|
| Dominio y POO | ✅ |
| Consola | ✅ |
| Windows Forms | ✅ Avance funcional |
| Persistencia JSON | ✅ |
| Persistencia SQL Server | ✅ |
| Diseño de base de datos | ✅ |
| Pruebas automatizadas | ✅ 32/32 |
| CRUD completo | 🔜 En progreso |
| Autenticación y roles | 🔜 Futuro |
