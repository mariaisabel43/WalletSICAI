using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            var solicitantes = _reportService.ObtenerSolicitantes();
            return View(solicitantes); 
        }



        [HttpGet("RecargasPorRango")] 
        public async Task<IActionResult> RecargasPorRango(DateTime fechaInicio, DateTime? fechaFin, string? solicitante) 
        { 
            var pdf = await _reportService.GenerarReporteRecargas(fechaInicio, fechaFin, solicitante); 
            var nombreArchivo = fechaFin.HasValue 
                ? $"ReporteRecargas_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf" 
                : $"ReporteRecargas_{fechaInicio:yyyyMMdd}.pdf"; 
            return File(pdf, "application/pdf", nombreArchivo); }
    }

}


