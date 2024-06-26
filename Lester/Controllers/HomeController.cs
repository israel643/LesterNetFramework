﻿using Lester.Models;
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
        public ActionResult GenerateReport(DateTime? FechaInicio, DateTime? FechaDeFinalizacion, string ReportType)
        {
            bool noRecords = false;

            if (ReportType == "Unitario")
            {
                var data = dataAccess.GeneradorDeEmabrquesPorRango(FechaInicio, FechaDeFinalizacion);
                List<Embarques> reportUnitario = data.Select(items => new Embarques
                {
                    codebar = items.codebar,
                    acronimo = items.acronimo,
                    fechaLectura = items.fechaLectura,
                    Viaje = items.Viaje
                }).ToList();

                ViewBag.ReportUnitario = reportUnitario;
                noRecords = !reportUnitario.Any();
            }
            else if (ReportType == "Agrupamiento")
            {
                var dataFork = dataAccess.GeneratorAddFilterByCount(FechaInicio, FechaDeFinalizacion);
                List<Agrupamiento> reportAgroup = dataFork.Select(elements => new Agrupamiento
                {
                    acronimo = elements.acronimo,
                    cantidad = elements.cantidad,
                    Viaje = elements.Viaje
                }).ToList();

                ViewBag.ReportAgroup = reportAgroup;
                noRecords = !reportAgroup.Any();
            }
            else if (ReportType == "TotalA")
            {
                var dataTotal = dataAccess.GetTotalItemsCargados(FechaInicio, FechaDeFinalizacion);
                List<TotalItemsCargados> reportTotal = dataTotal.Select(elements => new TotalItemsCargados
                {
                    Viaje = elements.Viaje,
                    TotalItems = elements.TotalItems
                }).ToList();
                ViewBag.ReportTotal = reportTotal;
            }

            ViewBag.FechaInicio = FechaInicio;
            ViewBag.FechaDeFinalizacion = FechaDeFinalizacion;
            ViewBag.ReportType = ReportType;
            ViewBag.NoRecords = noRecords;

            return View("Index");
        }


        [HttpGet]
        public ActionResult ExporToExcel(DateTime? FechaInicio, DateTime? FechaDeFinalizacion, string ReportType)
        {
            var excelAPP = new Excel.Application();
            var LibroTrab = excelAPP.Workbooks.Add();
            Excel.Worksheet hoja;

            if (ReportType == "Unitario")
            {
                var reportUnitarioForExcel = dataAccess.GeneradorDeEmabrquesPorRango(FechaInicio, FechaDeFinalizacion);

                if (reportUnitarioForExcel == null)
                {
                    return RedirectToAction("Index");
                }

                hoja = (Excel.Worksheet)LibroTrab.Worksheets[1];
                hoja.Name = "Reporte Unitario";

                hoja.Cells[1, 1] = "Codebar";
                hoja.Cells[1, 2] = "Acrónimo";
                hoja.Cells[1, 3] = "Hora de Lectura";
                hoja.Cells[1, 4] = "Viaje";

                for (int i = 0; i < reportUnitarioForExcel.Count; i++)
                {
                    hoja.Cells[i + 2, 1] = reportUnitarioForExcel[i].codebar;
                    hoja.Cells[i + 2, 2] = reportUnitarioForExcel[i].acronimo;
                    hoja.Cells[i + 2, 3] = reportUnitarioForExcel[i].fechaLectura.ToShortTimeString();
                    hoja.Cells[i + 2, 4] = reportUnitarioForExcel[i].Viaje;
                }
            }
            else if (ReportType == "Agrupamiento")
            {
                var reportAgroupForExcel = dataAccess.GeneratorAddFilterByCount(FechaInicio, FechaDeFinalizacion);

                if (reportAgroupForExcel == null)
                {
                    return RedirectToAction("Index");
                }

                hoja = (Excel.Worksheet)LibroTrab.Worksheets[1];
                hoja.Name = "Reporte total por Viaje";
                hoja.Cells[1, 1] = "Acrónimo";
                hoja.Cells[1, 2] = "Cantidad";
                hoja.Cells[1, 3] = "Viaje";

                for (int i = 0; i < reportAgroupForExcel.Count; i++)
                {
                    hoja.Cells[i + 2, 1] = reportAgroupForExcel[i].acronimo;
                    hoja.Cells[i + 2, 2] = reportAgroupForExcel[i].cantidad;
                    hoja.Cells[i + 2, 3] = reportAgroupForExcel[i].Viaje;
                }
            }
            else if (ReportType == "TotalA")
            {
                var reportAgroupForExcel = dataAccess.GetTotalItemsCargados(FechaInicio, FechaDeFinalizacion);

                if (reportAgroupForExcel == null)
                {
                    return RedirectToAction("Index");
                }

                hoja = (Excel.Worksheet)LibroTrab.Worksheets[1];
                hoja.Name = "Reporte Agrupado";

          
                hoja.Cells[1, 2] = "Total";
                hoja.Cells[1, 3] = "Viaje";

                for (int i = 0; i < reportAgroupForExcel.Count; i++)
                {
                    hoja.Cells[i + 2, 1] = reportAgroupForExcel[i].TotalItems;
                    hoja.Cells[i + 2, 2] = reportAgroupForExcel[i].Viaje;
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

            // Generar un nombre de archivo temporal único
            string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xlsx");
            LibroTrab.SaveAs(tempFilePath, Excel.XlFileFormat.xlWorkbookDefault);
            LibroTrab.Close(false);
            excelAPP.Quit();

            // Liberar recursos
            Marshal.ReleaseComObject(hoja);
            Marshal.ReleaseComObject(LibroTrab);
            Marshal.ReleaseComObject(excelAPP);

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            System.IO.File.Delete(tempFilePath);

            // Devolver el archivo Excel como resultado de la acción
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte.xlsx");
        }
    }


}