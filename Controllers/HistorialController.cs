using Microsoft.AspNetCore.Mvc;
using WalletSICAI.Models;
using WalletSICAI.Services;

namespace WalletSICAI.Controllers
{
    public class HistorialController : Controller
    {
        private readonly AuthService _authService;

        public HistorialController(AuthService authService)
        {
            _authService = authService;
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
    }
}
