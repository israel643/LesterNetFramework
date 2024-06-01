using Lester.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
/// Biblioteca para descarga 
using Excel = Microsoft.Office.Interop.Excel;

namespace Lester.Controllers
{
    public class HomeController : Controller
    {
            DataAccessDAL dataAccess = new DataAccessDAL();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
        {
            var data = dataAccess.GeneradorDeEmabrquesPorRango(FechaInicio, FechaDeFinalizacion);
            ViewBag.FormularioEnviado = true;
            


            // Pasar los datos a la vista utilizando ViewBag, pero asegurando el tipo correcto
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
                Viaje  = elements.Viaje
            }).ToList();



            ViewBag.ReportAgroup = reportAgroup;
            ViewBag.ReportUnitario = reportUnitario;

            /// Guardamos los datos para poder acceder a ellos
            ViewBag.FechaInicio = FechaInicio;
            ViewBag.FechaDeFinalizacion = FechaDeFinalizacion;

            return View();
        }

        [HttpGet]
        public ActionResult ExporToExcel(DateTime? FechaInicio, DateTime? FechaDeFinalizacion)
        {
            var reportUnitarioForExcel = dataAccess.GeneradorDeEmabrquesPorRango(FechaInicio, FechaDeFinalizacion);
            var reportAgroupForExcel = dataAccess.GeneratorAddFilterByCount(FechaInicio, FechaDeFinalizacion);

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

            // Generar un nombre de archivo temporal único
            string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");
            LibroTrab.SaveAs(tempFilePath, Excel.XlFileFormat.xlWorkbookDefault);
            LibroTrab.Close(false);
            excelAPP.Quit();

            // Liberar recursos
            Marshal.ReleaseComObject(hoja1);
            Marshal.ReleaseComObject(hoja2);
            Marshal.ReleaseComObject(LibroTrab);
            Marshal.ReleaseComObject(excelAPP);

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            System.IO.File.Delete(tempFilePath);

            // Devolver el archivo Excel como resultado de la acción
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte.xlsx");
        }
    }

    
}