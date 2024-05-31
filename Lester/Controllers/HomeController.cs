using Lester.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
/// Biblioteca para descarga de excel 
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
        public ActionResult Index(DateTime FechaInicio, DateTime FechaDeFinalizacion, int pages = 1, int total= 5)
        {
            var data = dataAccess.GeneradorDeEmabrquesPorRango(FechaInicio, FechaDeFinalizacion);

         
            // Pasar los datos a la vista utilizando ViewBag, pero asegurando el tipo correcto
            List<Embarques> reportUnitario = data.Select(d => new Embarques
            {
                codebar = d.codebar,
                acronimo = d.acronimo,
                fechaLectura = d.fechaLectura,
                Viaje = d.Viaje
            }).ToList();

            ViewBag.ReportUnitario = reportUnitario;

            return View();
        }

    }
}