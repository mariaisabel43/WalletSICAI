using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Services;


namespace WalletSICAI.Controllers
{
    [Route("Reportes")]
    public class ReportesController : Controller
    {
        // Declaración del campo privado
        private readonly ReportService _reportService; 
        // Constructor con inyección de dependencias
        public ReportesController(ReportService reportService) 
        { 
            _reportService = reportService; 
        }
        [HttpGet("FormularioReportes")]
        public IActionResult FormularioReportes()
        {
            return View();
        }


        [HttpGet("RecargasPorFecha")]
        public async Task<IActionResult> RecargasPorFecha(DateTime fecha) 
        { 
            var pdf = await _reportService.GenerarReporteRecargasPorFecha(fecha); 
            return File(pdf, "application/pdf", $"ReporteRecargas_{fecha:yyyyMMdd}.pdf"); }
    }

}


