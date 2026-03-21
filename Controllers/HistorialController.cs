using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Services;

namespace WalletSICAI.Controllers
{
    public class HistorialController : Controller
    {
        private readonly AuthService _authService;
        private readonly WalletContext _context;

        public HistorialController(AuthService authService, WalletContext context)
        {
            _authService = authService;
            _context = context;
        }

        // =========================
        // PANTALLA 1: BUSCAR ESTUDIANTE
        // =========================
        [HttpGet]
        public IActionResult BuscarEstudiante()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BuscarEstudiante(string buscar)
        {
            if (string.IsNullOrWhiteSpace(buscar))
            {
                ViewBag.Error = "Debe ingresar una cédula o un nombre.";
                return View();
            }

            // Recuperar el Id del administrador desde el claim
            var adminIdClaim = User.FindFirst("AdministrativoId");
            if (adminIdClaim == null)
                return Unauthorized();

            int adminId = int.Parse(adminIdClaim.Value);

            // Usar AuthService para buscar estudiantes SOLO de la institución del administrador
            var estudiantes = await _authService.BuscarEstudiantesPorInstitucionAsync(buscar, adminId);

            if (!estudiantes.Any())
            {
                ViewBag.Error = "No se encontraron estudiantes en su institución.";
            }

            return View(estudiantes);
        }


        // =========================
        // PANTALLA 2: HISTORIAL
        // =========================
        [HttpGet]
        public async Task<IActionResult> HistorialEstudiante(string cedula, string nombreCompleto)
        {
            var historial = await _authService.ObtenerHistorialEstudianteAsync(cedula, nombreCompleto);

            if (!historial.Any())
            {
                ViewBag.Error = "No se encontraron recargas para este estudiante.";
            }

            return View(historial);
        }

        [HttpPost]
        public async Task<IActionResult> DevolverRecarga(int id)
        {
            var adminIdClaim = User.FindFirst("AdministrativoId");
            if (adminIdClaim == null) return Unauthorized();

            int adminId = int.Parse(adminIdClaim.Value);

            var resultado = await _authService.DevolverRecargaAsync(id, adminId);

            if (!resultado.Exito)
            {
                TempData["Error"] = resultado.Mensaje;

                // Filtrar por cédula, porque VwHistorialRecarga no tiene EstudianteId
                var recargas = await _context.VwHistorialRecargas
                    .Where(r => r.EstudianteCedula == resultado.Cedula)
                    .ToListAsync();

                return View("HistorialEstudiante", recargas);
            }

            TempData["Exito"] = resultado.Mensaje;
            return RedirectToAction("HistorialEstudiante", new { cedula = resultado.Cedula });
        }
    }
}