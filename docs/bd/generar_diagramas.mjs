import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import {
  palette,
  tables,
  relationships,
  conceptual,
  catalogSeeds,
  createOrder,
  tableById
} from './modelo-bd.mjs';

const here = path.dirname(fileURLToPath(import.meta.url));

const esc = (value) => String(value)
  .replaceAll('&', '&amp;')
  .replaceAll('<', '&lt;')
  .replaceAll('>', '&gt;')
  .replaceAll('"', '&quot;');

const shortType = (type) => {
  if (type.startsWith('NVARCHAR')) return 'NVARCHAR';
  if (type.startsWith('DECIMAL')) return 'DECIMAL';
  if (type === 'UNIQUEIDENTIFIER') return 'UUID';
  if (type.startsWith('DATETIME2')) return 'DATETIME';
  if (type.startsWith('CHAR')) return 'CHAR';
  if (type.startsWith('SMALLINT')) return 'SMALLINT';
  if (type.startsWith('TINYINT')) return 'TINYINT';
  return type;
};

const keyLabel = (key) => {
  if (!key) return '';
  return `<b><font color="${key.includes('PK') ? '#B45309' : '#1D4ED8'}">${key}</font></b> `;
};

const fieldLine = (field, mode) => {
  const nullable = field.nullable ? '?' : '';
  const type = mode === 'physical' ? field.type : shortType(field.type);
  const suffix = mode === 'logical' || mode === 'physical' ? ` <font color="#64748B">: ${type}${nullable}</font>` : '';
  return `${keyLabel(field.key)}${field.name}${suffix}`;
};

const heightOf = (fields) => 50 + fields.length * 18;

const tableHtml = (table, mode) => {
  const lines = table.fields.map((field) => fieldLine(field, mode));
  return [
    `<b><font color="${palette[table.section].title}" style="font-size:12px">${table.title}</font></b>`,
    `<font color="#64748B" style="font-size:9px">${table.subtitle}</font>`,
    ...lines
  ].join('<br>');
};

const tableCell = (table, mode) => {
  const h = heightOf(table.fields);
  const p = palette[table.section];
  const fontSize = mode === 'physical' ? 10 : 11;
  return `<mxCell id="t_${table.id}" value="${esc(tableHtml(table, mode))}" style="rounded=1;whiteSpace=wrap;html=1;align=left;verticalAlign=top;spacing=10;fillColor=${p.fill};strokeColor=${p.stroke};strokeWidth=2;fontColor=#0F172A;fontSize=${fontSize};fontFamily=Segoe UI;" vertex="1" parent="1"><mxGeometry x="${table.x}" y="${table.y}" width="${table.w}" height="${h}" as="geometry" /></mxCell>`;
};

const conceptualCell = (entity) => {
  const p = palette[entity.section];
  const html = [
    `<b><font color="${p.title}" style="font-size:12px">${entity.title}</font></b>`,
    `<font color="#64748B" style="font-size:9px">${entity.subtitle}</font>`,
    ...entity.fields
  ].join('<br>');
  const h = heightOf(entity.fields);
  return `<mxCell id="t_${entity.id}" value="${esc(html)}" style="rounded=1;whiteSpace=wrap;html=1;align=left;verticalAlign=top;spacing=10;fillColor=${p.fill};strokeColor=${p.stroke};strokeWidth=2;fontColor=#0F172A;fontSize=11;fontFamily=Segoe UI;" vertex="1" parent="1"><mxGeometry x="${entity.x}" y="${entity.y}" width="${entity.w}" height="${h}" as="geometry" /></mxCell>`;
};

const sectionCell = (section, x, y, w, h, label) => `<mxCell id="sec_${section}" value="${esc(label)}" style="rounded=0;whiteSpace=wrap;html=1;fillColor=${palette[section].section};strokeColor=${palette[section].stroke};dashed=1;verticalAlign=top;align=left;spacing=12;fontSize=11;fontStyle=1;fontColor=${palette[section].title};fontFamily=Segoe UI;opacity=55;" vertex="1" parent="1"><mxGeometry x="${x}" y="${y}" width="${w}" height="${h}" as="geometry" /></mxCell>`;

const textCell = (id, value, x, y, w, h, extra = '') => `<mxCell id="${id}" value="${esc(value)}" style="text;html=1;align=left;verticalAlign=top;whiteSpace=wrap;fontSize=10;fontColor=#334155;fontFamily=Segoe UI;${extra}" vertex="1" parent="1"><mxGeometry x="${x}" y="${y}" width="${w}" height="${h}" as="geometry" /></mxCell>`;

const legend = (x, y, w = 420, h = 110) => textCell(
  'leyenda',
  [
    '<b>Leyenda</b>',
    'PK = clave primaria · FK = clave foránea · PK/FK = subtipo · UQ = valor único',
    '1 ── N = uno a muchos · 0..1 = opcional · △ = herencia (tabla hija → tabla base)',
    'El signo ? indica que la columna admite NULL.'
  ].join('<br>'),
  x, y, w, h
);

const edgeStyle = (kind) => {
  if (kind === 'inherit') {
    return 'edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;endArrow=block;endFill=0;startArrow=none;strokeColor=#64748B;strokeWidth=1.6;';
  }

  const arrows = {
    'one-to-many': 'startArrow=ERmandOne;endArrow=ERmany;',
    'zero-to-many': 'startArrow=ERzeroToOne;endArrow=ERzeroToMany;',
    'one-to-one': 'startArrow=ERmandOne;endArrow=ERmandOne;',
    'zero-to-one': 'startArrow=ERzeroToOne;endArrow=ERmandOne;',
    'many-to-many': 'startArrow=ERmany;endArrow=ERmany;'
  };

  return `edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;${arrows[kind] ?? arrows['one-to-many']}endFill=0;startFill=0;strokeColor=#64748B;strokeWidth=1.5;jumpStyle=arc;`;
};

const edgeCell = (relationship, index) => {
  const { from, to, kind, waypoints = [], style = '' } = relationship;
  const points = waypoints.length
    ? `<Array as="points">${waypoints.map(([x, y]) => `<mxPoint x="${x}" y="${y}" />`).join('')}</Array>`
    : '';
  return `<mxCell id="e_${from}_${to}_${index}" style="${edgeStyle(kind)}${style}" edge="1" parent="1" source="t_${from}" target="t_${to}"><mxGeometry relative="1" as="geometry">${points}</mxGeometry></mxCell>`;
};

const diagramFile = (name, pageWidth, pageHeight, cells) => `<?xml version="1.0" encoding="UTF-8"?>
<mxfile host="app.diagrams.net" modified="2026-10-29T00:00:00.000Z" agent="HuellaVet" version="24.7.17" type="device">
  <diagram id="${name.replaceAll(' ', '_')}" name="${name}">
    <mxGraphModel dx="${pageWidth}" dy="${pageHeight}" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="${pageWidth}" pageHeight="${pageHeight}" math="0" shadow="0" background="#FFFFFF">
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />
${cells.join('\n')}
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
`;

const buildLogical = () => {
  const cells = [];
  cells.push(sectionCell('personas', 40, 80, 660, 780, 'PERSONAS · CLIENTES · EMPLEADOS'));
  cells.push(sectionCell('mascotas', 40, 980, 660, 760, 'PACIENTES · MASCOTAS'));
  cells.push(sectionCell('servicios', 730, 80, 700, 1040, 'SERVICIOS · CATÁLOGO Y SUBTIPOS'));
  cells.push(sectionCell('operacion', 1500, 80, 860, 1280, 'OPERACIÓN · AGENDA, COBRO E INVENTARIO'));

  for (const table of tables) cells.push(tableCell(table, 'logical'));
  relationships.forEach((rel, index) => cells.push(edgeCell(rel, index)));
  cells.push(legend(40, 1770, 660, 100));
  cells.push(textCell('nota_logico', 'Modelo lógico · claves, relaciones y atributos atómicos. Los totales y la edad se calculan en vistas; no se almacenan.', 1580, 1390, 760, 60));

  return diagramFile('02 Modelo logico', 2400, 1900, cells);
};

const buildPhysical = () => {
  const cells = [];
  cells.push(sectionCell('personas', 40, 80, 660, 780, 'PERSONAS · SQL SERVER 2022'));
  cells.push(sectionCell('mascotas', 40, 980, 660, 760, 'PACIENTES · SQL SERVER 2022'));
  cells.push(sectionCell('servicios', 730, 80, 700, 1040, 'SERVICIOS · SQL SERVER 2022'));
  cells.push(sectionCell('operacion', 1500, 80, 860, 1280, 'OPERACIÓN · SQL SERVER 2022'));

  for (const table of tables) cells.push(tableCell(table, 'physical'));
  relationships.forEach((rel, index) => cells.push(edgeCell(rel, index)));
  cells.push(legend(40, 1770, 660, 100));
  cells.push(textCell('indices', [
    '<b>Índices y restricciones principales</b>',
    '• UX_Facturas_Cita: índice único filtrado (CitaId IS NOT NULL) → una factura por cita.',
    '• IX_Citas_Veterinario_Fecha e IX_Citas_Mascota_Fecha → agenda y detección de choques.',
    '• IX_Facturas_Cliente_Fecha, IX_DetallesFactura_Factura → consultas de cobro.',
    '• CK_Facturas_Pago_Coherente → una factura pagada exige método y fecha de pago.',
    '• UNIQUE en Documento, Numero, Codigo, NumeroColegiatura y correos de catálogo.'
  ].join('<br>'), 1580, 1390, 760, 180));
  cells.push(textCell('fisico_nota', 'Modelo físico · SQL Server 2022. GUID con NEWSEQUENTIALID(), NVARCHAR para texto, DECIMAL(12,2) para dinero y DATETIME2(0) para marcas de tiempo. Los totales se calculan en vistas.', 1580, 1650, 760, 70));

  return diagramFile('05 Modelo fisico', 2400, 1900, cells);
};

const buildConceptual = () => {
  const cells = [];
  cells.push(sectionCell('personas', 40, 60, 650, 520, 'PERSONAS'));
  cells.push(sectionCell('mascotas', 720, 60, 320, 280, 'PACIENTES'));
  cells.push(sectionCell('servicios', 1080, 60, 320, 280, 'SERVICIOS'));
  cells.push(sectionCell('operacion', 720, 350, 1000, 380, 'OPERACIÓN'));
  cells.push(sectionCell('catalogos', 1080, 740, 320, 180, 'INVENTARIO'));

  for (const entity of conceptual.entities) cells.push(conceptualCell(entity));
  conceptual.relationships.forEach((rel, index) => cells.push(edgeCell(rel, index)));

  cells.push(legend(60, 650, 620, 110));
  cells.push(textCell('nota_conceptual', [
    '<b>Reglas de negocio representadas</b>',
    '• Un cliente tiene una o muchas mascotas; una mascota pertenece a un cliente.',
    '• Una cita relaciona un cliente, una mascota, un veterinario y un servicio.',
    '• Una cita atendida puede generar como máximo una factura.',
    '• Una factura tiene una o muchas líneas; una línea puede referenciar un servicio o ser un concepto manual.',
    '• El inventario es independiente de la facturación en esta versión.'
  ].join('<br>'), 60, 780, 900, 100));
  cells.push(textCell('nota_subtipos', [
    '<b>Subtipos</b>',
    'Persona → Cliente / Empleado → Veterinario, Recepcionista, Auxiliar.',
    'Mascota → Perro, Gato, Ave, Conejo.',
    'Servicio → Consulta, Vacunación, Cirugía, Peluquería, Hospitalización.'
  ].join('<br>'), 1430, 110, 320, 200));

  return diagramFile('01 Modelo conceptual', 1800, 950, cells);
};

const buildDictionary = () => {
  const lines = [
    '# Diccionario de datos · HuellaVet',
    '',
    'Generado desde `modelo-bd.mjs` con `node docs/bd/generar_diagramas.mjs`.',
    '',
    '**DBMS:** SQL Server 2022 · **Base:** `HuellaVet` · **Esquema:** `dbo`',
    '',
    'Convenciones: `PK` = clave primaria, `FK` = clave foránea, `PK/FK` = clave compartida con la tabla base, `UQ` = restricción única. La columna **Nulo** indica si se permite `NULL`.',
    '',
    '---',
    ''
  ];

  for (const id of createOrder) {
    const table = tableById[id];
    lines.push(`## ${table.title}`, '', table.purpose, '', '| Columna | Tipo | Nulo | Clave | Referencia | Default / regla | Descripción |', '|---|---|---|---|---|---|---|');
    for (const field of table.fields) {
      const ref = field.fk ? `${tableById[field.fk].title}(${tableById[field.fk].fields.find(x => x.key.includes('PK')).name})` : '';
      const rule = [field.defaultValue ? `DEFAULT ${field.defaultValue}` : '', field.check].filter(Boolean).join(' · ');
      lines.push(`| **${field.name}** | \`${field.type}\` | ${field.nullable ? 'Sí' : 'No'} | ${field.key || ''} | ${ref} | ${rule.replaceAll('|', '\\|')} | ${field.desc} |`);
    }
    if (table.checks?.length) {
      lines.push('', '**Restricciones de tabla:**', '');
      for (const check of table.checks) lines.push(`- \`${check.name}\`: \`${check.expr}\``);
    }
    if (table.indexes?.length) {
      lines.push('', '**Índices:**', '');
      for (const index of table.indexes) {
        const unique = index.unique ? 'UNIQUE ' : '';
        const filter = index.filter ? ` WHERE ${index.filter}` : '';
        lines.push(`- \`${index.name}\`: ${unique}(${index.columns.join(', ')})${filter}`);
      }
    }
    lines.push('', '---', '');
  }

  lines.push('## Resumen', '', `- Tablas: **${tables.length}**`, `- Columnas: **${tables.reduce((n, t) => n + t.fields.length, 0)}**`, '- Diagramas: `02_modelo_conceptual.drawio`, `03_modelo_logico.drawio`, `05_modelo_fisico.drawio`', '- Script: `06_huellavet_sqlserver.sql`', '');
  return lines.join('\n');
};

const sqlType = (field) => field.type;
const sqlDefault = (field) => field.defaultValue ? ` DEFAULT ${field.defaultValue}` : '';

const buildSql = () => {
  const lines = [
    '/* ============================================================',
    '   HuellaVet · Script de implementación para SQL Server 2022',
    '   Generado desde docs/bd/modelo-bd.mjs',
    '   Ejecutar en una instancia de SQL Server con permisos de CREATE DATABASE.',
    '   ============================================================ */',
    '',
    'IF DB_ID(N\'HuellaVet\') IS NULL',
    'BEGIN',
    '    CREATE DATABASE HuellaVet;',
    'END',
    'GO',
    '',
    'USE HuellaVet;',
    'GO',
    '',
    'SET NOCOUNT ON;',
    'GO',
    '',
    'SET ANSI_NULLS ON;',
    'SET QUOTED_IDENTIFIER ON;',
    'GO',
    ''
  ];

  for (const id of createOrder) {
    const table = tableById[id];
    lines.push(`/* ── ${table.title} ─────────────────────────────────────────── */`, `CREATE TABLE dbo.${table.title} (`);

    const columnLines = table.fields.map((field) => {
      const nullability = field.nullable ? 'NULL' : 'NOT NULL';
      return `    [${field.name}] ${sqlType(field)} ${nullability}${sqlDefault(field)}`;
    });

    const pk = table.fields.find(field => field.key.includes('PK'));
    const constraints = [];
    if (pk) constraints.push(`    CONSTRAINT [PK_${table.title}] PRIMARY KEY CLUSTERED ([${pk.name}] ASC)`);

    for (const field of table.fields.filter(f => f.key === 'UQ')) {
      constraints.push(`    CONSTRAINT [UQ_${table.title}_${field.name}] UNIQUE ([${field.name}])`);
    }

    for (const field of table.fields.filter(f => f.check)) {
      constraints.push(`    CONSTRAINT [CK_${table.title}_${field.name}] CHECK (${field.check})`);
    }

    for (const check of table.checks ?? []) {
      constraints.push(`    CONSTRAINT [${check.name}] CHECK (${check.expr})`);
    }

    const fkLines = table.fields.filter(field => field.fk).map((field) => {
      const target = tableById[field.fk];
      const targetPk = target.fields.find(f => f.key.includes('PK'));
      return `    CONSTRAINT [FK_${table.title}_${target.title}] FOREIGN KEY ([${field.name}]) REFERENCES dbo.${target.title} ([${targetPk.name}])`;
    });

    lines.push([...columnLines, ...constraints, ...fkLines].join(',\n'));
    lines.push(');', 'GO', '');
  }

  lines.push('/* ── Índices ────────────────────────────────────────────────── */');
  for (const table of tables) {
    for (const index of table.indexes ?? []) {
      const unique = index.unique ? 'UNIQUE ' : '';
      const filter = index.filter ? ` WHERE ${index.filter}` : '';
      lines.push(`CREATE ${unique}INDEX [${index.name}] ON dbo.${table.title} (${index.columns.map(c => `[${c}]`).join(', ')})${filter};`);
    }
  }
  lines.push('GO', '');

  lines.push('/* ── Datos de catálogo ──────────────────────────────────────── */');
  for (const [id, rows] of Object.entries(catalogSeeds)) {
    const table = tableById[id];
    const idField = table.fields.find(f => f.key.includes('PK')).name;
    const nameField = table.fields.find(f => f.name === 'Nombre').name;
    lines.push(`INSERT INTO dbo.${table.title} ([${idField}], [${nameField}]) VALUES`);
    lines.push(rows.map(([key, value]) => `    (${key}, N'${value}')`).join(',\n') + ';');
    lines.push('GO', '');
  }

  lines.push(`
/* ── Vistas de consulta ─────────────────────────────────────── */
CREATE OR ALTER VIEW dbo.vw_Empleados
AS
SELECT
    p.PersonaId AS EmpleadoId,
    p.Nombre + N' ' + p.Apellido AS NombreCompleto,
    p.Documento,
    p.Telefono,
    p.Email,
    e.TipoEmpleado,
    CASE e.TipoEmpleado
        WHEN 'V' THEN N'Veterinario'
        WHEN 'R' THEN N'Recepcionista'
        ELSE N'Auxiliar veterinario'
    END AS Cargo,
    e.SalarioBase,
    e.FechaIngreso,
    e.Activo,
    v.Especialidad,
    v.NumeroColegiatura,
    v.AniosExperiencia,
    r.Turno,
    a.Certificacion
FROM dbo.Personas p
INNER JOIN dbo.Empleados e ON e.EmpleadoId = p.PersonaId
LEFT JOIN dbo.Veterinarios v ON v.VeterinarioId = e.EmpleadoId
LEFT JOIN dbo.Recepcionistas r ON r.RecepcionistaId = e.EmpleadoId
LEFT JOIN dbo.AuxiliaresVeterinarios a ON a.AuxiliarId = e.EmpleadoId;
GO

CREATE OR ALTER VIEW dbo.vw_Mascotas
AS
SELECT
    m.MascotaId,
    m.Nombre,
    esp.Nombre AS Especie,
    m.Raza,
    m.FechaNacimiento,
    DATEDIFF(YEAR, m.FechaNacimiento, CAST(SYSDATETIME() AS date))
      - CASE
          WHEN DATEADD(YEAR, DATEDIFF(YEAR, m.FechaNacimiento, CAST(SYSDATETIME() AS date)), m.FechaNacimiento) > CAST(SYSDATETIME() AS date)
          THEN 1 ELSE 0
        END AS EdadAnios,
    m.PesoKg,
    m.Sexo,
    m.Alergias,
    m.Esterilizado,
    m.UltimaVisita,
    p.Nombre + N' ' + p.Apellido AS Dueno
FROM dbo.Mascotas m
INNER JOIN dbo.Especies esp ON esp.EspecieId = m.EspecieId
INNER JOIN dbo.Clientes c ON c.ClienteId = m.ClienteId
INNER JOIN dbo.Personas p ON p.PersonaId = c.ClienteId;
GO

CREATE OR ALTER VIEW dbo.vw_Agenda
AS
SELECT
    c.CitaId,
    CAST(c.FechaHora AS date) AS Fecha,
    c.FechaHora,
    p.Nombre + N' ' + p.Apellido AS Cliente,
    m.Nombre AS Mascota,
    esp.Nombre AS Especie,
    vp.Nombre + N' ' + vp.Apellido AS Veterinario,
    s.Nombre AS Servicio,
    ec.Nombre AS Estado,
    c.Motivo
FROM dbo.Citas c
INNER JOIN dbo.Clientes cl ON cl.ClienteId = c.ClienteId
INNER JOIN dbo.Personas p ON p.PersonaId = cl.ClienteId
INNER JOIN dbo.Mascotas m ON m.MascotaId = c.MascotaId
INNER JOIN dbo.Especies esp ON esp.EspecieId = m.EspecieId
INNER JOIN dbo.Veterinarios v ON v.VeterinarioId = c.VeterinarioId
INNER JOIN dbo.Personas vp ON vp.PersonaId = v.VeterinarioId
INNER JOIN dbo.Servicios s ON s.ServicioId = c.ServicioId
INNER JOIN dbo.EstadosCita ec ON ec.EstadoCitaId = c.EstadoCitaId;
GO

CREATE OR ALTER VIEW dbo.vw_FacturasResumen
AS
WITH Base AS (
    SELECT
        f.FacturaId,
        f.Numero,
        f.ClienteId,
        f.CitaId,
        f.Fecha,
        f.Pagada,
        f.DescuentoPorcentaje,
        f.ImpuestoPorcentaje,
        CAST(COALESCE(SUM(d.Cantidad * d.PrecioUnitario), 0) AS DECIMAL(12,2)) AS Subtotal
    FROM dbo.Facturas f
    LEFT JOIN dbo.DetallesFactura d ON d.FacturaId = f.FacturaId
    GROUP BY f.FacturaId, f.Numero, f.ClienteId, f.CitaId, f.Fecha, f.Pagada,
             f.DescuentoPorcentaje, f.ImpuestoPorcentaje
),
Calculada AS (
    SELECT
        *,
        CAST(Subtotal * DescuentoPorcentaje / 100.0 AS DECIMAL(12,2)) AS Descuento,
        CAST((Subtotal - (Subtotal * DescuentoPorcentaje / 100.0)) * ImpuestoPorcentaje / 100.0 AS DECIMAL(12,2)) AS Impuesto
    FROM Base
)
SELECT
    FacturaId,
    Numero,
    ClienteId,
    CitaId,
    Fecha,
    Pagada,
    Subtotal,
    Descuento,
    Impuesto,
    CAST(Subtotal - Descuento + Impuesto AS DECIMAL(12,2)) AS Total
FROM Calculada;
GO

CREATE OR ALTER VIEW dbo.vw_StockBajo
AS
SELECT
    p.ProductoId,
    p.Nombre,
    cp.Nombre AS Categoria,
    p.Stock,
    p.StockMinimo,
    p.Precio,
    p.Vencimiento
FROM dbo.Productos p
INNER JOIN dbo.CategoriasProducto cp ON cp.CategoriaProductoId = p.CategoriaProductoId
WHERE p.Stock <= p.StockMinimo;
GO

CREATE OR ALTER VIEW dbo.vw_VacunasProximas
AS
WITH Proximas AS (
    SELECT
        m.Nombre AS Mascota,
        p.Nombre + N' ' + p.Apellido AS Dueno,
        vac.Vacuna,
        DATEADD(MONTH, vac.ProximaDosisMeses, COALESCE(c.FechaAtencion, c.FechaHora)) AS ProximaDosis
    FROM dbo.Citas c
    INNER JOIN dbo.EstadosCita ec ON ec.EstadoCitaId = c.EstadoCitaId
    INNER JOIN dbo.Vacunaciones vac ON vac.ServicioId = c.ServicioId
    INNER JOIN dbo.Mascotas m ON m.MascotaId = c.MascotaId
    INNER JOIN dbo.Clientes cl ON cl.ClienteId = c.ClienteId
    INNER JOIN dbo.Personas p ON p.PersonaId = cl.ClienteId
    WHERE ec.Nombre = N'Atendida'
)
SELECT
    Mascota,
    Dueno,
    Vacuna,
    ProximaDosis,
    DATEDIFF(DAY, CAST(SYSDATETIME() AS date), CAST(ProximaDosis AS date)) AS Dias
FROM Proximas
WHERE CAST(ProximaDosis AS date) BETWEEN CAST(SYSDATETIME() AS date) AND DATEADD(DAY, 45, CAST(SYSDATETIME() AS date));
GO

PRINT N'Base HuellaVet creada correctamente.';
GO
`);

  return lines.join('\n').replace(/\n{3,}/g, '\n\n').replaceAll('─', '-');
};

const write = (file, content, withBom = false) => {
  fs.writeFileSync(path.join(here, file), `${withBom ? '\uFEFF' : ''}${content}`, 'utf8');
  console.log(`Generado: ${file}`);
};

write('02_modelo_conceptual.drawio', buildConceptual());
write('03_modelo_logico.drawio', buildLogical());
write('05_modelo_fisico.drawio', buildPhysical());
write('07_diccionario_datos.md', buildDictionary());
write('06_huellavet_sqlserver.sql', buildSql(), true);
