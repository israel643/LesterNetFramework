🚀 Proyecto de Generación y Exportación de Reportes en ASP.NET MVC
Este proyecto permite generar reportes basados en fechas y exportarlos a archivos de Excel. Utiliza ASP.NET MVC para la lógica del backend y Bootstrap 5 para un diseño responsivo y moderno.

📋 Tabla de Contenidos
Descripción
Características
Tecnologías Utilizadas
Instalación
Uso
Procedimientos Almacenados
Contribuciones
Licencia
📄 Descripción
Este proyecto permite a los usuarios generar reportes detallados y agrupados basados en un rango de fechas. Los reportes pueden visualizarse en la web y exportarse a un archivo de Excel para su análisis y almacenamiento.

✨ Características
Generación de reportes unitarios y agrupados.
Filtrado de reportes por fecha de inicio y/o fecha de finalización.
Exportación de reportes a archivos de Excel.
Diseño responsivo y moderno con Bootstrap 5.
🛠️ Tecnologías Utilizadas
ASP.NET MVC
C#
Bootstrap 5
Microsoft Excel Interop
SQL Server
🚀 Instalación
Clona el repositorio:
bash
Copiar código
git clone https://github.com/tu-usuario/tu-repositorio.git
Abre el proyecto en Visual Studio.
Configura la cadena de conexión a tu base de datos en el archivo web.config.
Restaura los paquetes NuGet:
bash
Copiar código
Update-Package -reinstall
📈 Uso
Navega a la página principal.
Selecciona una fecha de inicio y/o una fecha de finalización.
Haz clic en "Generar" para ver los reportes.
Para exportar los reportes a Excel, haz clic en el botón "Exportar a Excel".
🌟 Interfaz de Usuario
Formulario de Selección de Fechas
html
Copiar código
<main>
    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <h2 class="text-center mb-4">Genera Reporte</h2>
                <div class="mt-2">
                    <div class="alert alert-primary d-flex alert-dismissible fade show" role="alert">
                        <strong>Selecciona una o un rango de fechas </strong> Para poder realizar un reporte.
                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                    </div>
                </div>
                @using (Html.BeginForm("Index", "Home", FormMethod.Post))
                {
                    <div class="form-group row">
                        <div class="col-md-8">
                            <label for="FechaInicio" class="col-form-label">Fecha Inicio:</label>
                            <input type="date" class="form-control" id="FechaInicio" name="FechaInicio" required />
                        </div>
                        <div class="col-md-4">
                            <label for="FechaDeFinalizacion" class="col-form-label">Fecha Fin:</label>
                            <input type="date" class="form-control" id="FechaDeFinalizacion" name="FechaDeFinalizacion" />
                        </div>
                    </div>
                    <div class="form-group row mt-4">
                        <div class="col text-center">
                            <input type="submit" class="btn btn-primary" value="Generar" />
                        </div>
                    </div>
                }
            </div>
        </div>
    </div>
</main>
Botón de Exportación
html
Copiar código
@if (reportUnitario != null && reportUnitario.Any())
{
    <h3>Reportes</h3>
    <div>
        <div class="d-flex justify-content-end">
            <a href="@Url.Action("ExporToExcel", "Home", new { FechaInicio = ViewBag.FechaInicio, FechaDeFinalizacion = ViewBag.FechaDeFinalizacion })" class="btn" style="background-color: #217346; color: white; border-color: #1e6b41;">
                <i class="fas fa-file-excel" style="margin-right: 5px;"></i> Exportar a Excel
            </a>
        </div>
    </div>
}
📊 Controlador
csharp
Copiar código
public ActionResult Index(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
{
    var data = dataAccess.GeneradorDeEmabrquesPorRango(FechaInicio, FechaDeFinalizacion);

    List<Embarques> reportUnitario = data.Select(items => new Embarques
    {
        codebar = items.codebar,
        acronimo = items.acronimo,
        fechaLectura = items.fechaLectura,
        Viaje = items.Viaje
    }).ToList();

    var filterData = dataAccess.GeneratorAddFilterByCount(FechaInicio, FechaDeFinalizacion);

    List<Agrupamiento> reportAgroup = filterData.Select(elements => new Agrupamiento
    {
        acronimo = elements.acronimo,
        cantidad = elements.cantidad,
        Viaje = elements.Viaje
    }).ToList();

    ViewBag.ReportAgroup = reportAgroup;
    ViewBag.ReportUnitario = reportUnitario;
    ViewBag.FechaInicio = FechaInicio;
    ViewBag.FechaDeFinalizacion = FechaDeFinalizacion;

    return View();
}
📥 Procedimientos Almacenados
sql
Copiar código
CREATE PROCEDURE FilterByTravel 
    @DateFrom DATETIME = NULL, 
    @DateTo DATETIME = NULL
AS
BEGIN
    SELECT acronimo, COUNT(acronimo) AS Cantidad, Viaje 
    FROM tblRFID_CodiCaptEmbarques
    WHERE (@DateFrom IS NULL OR fechaLectura >= @DateFrom)
      AND (@DateTo IS NULL OR fechaLectura <= @DateTo)
    GROUP BY acronimo, Viaje;
END
📊 Exportación a Excel
csharp
Copiar código
public ActionResult ExporToExcel(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
{
    var reportUnitarioForExcel = TempData["ReportUnitario"] as List<Embarques>;
    var reportAgroupForExcel = TempData["ReportAgroup"] as List<Agrupamiento>;

    if (reportUnitarioForExcel == null || reportAgroupForExcel == null)
    {
        return RedirectToAction("Index");
    }

    var excelAPP = new Excel.Application();
    var LibroTrab = excelAPP.Workbooks.Add();
    var hoja1 = (Excel.Worksheet)LibroTrab.Worksheets[1];
    hoja1.Name = "Reporte Unitario";

    hoja1.Cells[1, 1] = "Codebar";
    hoja1.Cells[1, 2] = "Acrónimo";
    hoja1.Cells[1, 3] = "Hora de Lectura";
    hoja1.Cells[1, 4] = "Viaje";

    for (int i = 0; i < reportUnitarioForExcel.Count; i++)
    {
        hoja1.Cells[i + 2, 1] = reportUnitarioForExcel[i].codebar;
        hoja1.Cells[i + 2, 2] = reportUnitarioForExcel[i].acronimo;
        hoja1.Cells[i + 2, 3] = reportUnitarioForExcel[i].fechaLectura.ToShortTimeString();
        hoja1.Cells[i + 2, 4] = reportUnitarioForExcel[i].Viaje;
    }

    var hoja2 = (Excel.Worksheet)LibroTrab.Worksheets.Add();
    hoja2.Name = "Reporte Agrupado";

    hoja2.Cells[1, 1] = "Acrónimo";
    hoja2.Cells[1, 2] = "Cantidad";
    hoja2.Cells[1, 3] = "Viaje";

    for (int i = 0; i < reportAgroupForExcel.Count; i++)
    {
        hoja2.Cells[i + 2, 1] = reportAgroupForExcel[i].acronimo;
        hoja2.Cells[i + 2, 2] = reportAgroupForExcel[i].cantidad;
        hoja2.Cells[i + 2, 3] = reportAgroupForExcel[i].Viaje;
    }

    string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");
    LibroTrab.SaveAs(tempFilePath, Excel.XlFileFormat.xlWorkbookDefault);
    LibroTrab.Close(false);
    excelAPP.Quit();

    Marshal.ReleaseComObject(hoja1);
    Marshal.ReleaseComObject(hoja2);
    Marshal.ReleaseComObject(LibroTrab);
    Marshal.ReleaseComObject(excelAPP);

    byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
    System.IO.File.Delete(tempFilePath);

    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte.xlsx");
}

📜 Licencia
Este proyecto está bajo la Licencia MIT.

