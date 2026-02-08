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

            var estudiantes = await _authService.BuscarEstudiantesAsync(buscar);

            if (!estudiantes.Any())
            {
                ViewBag.Error = "No se encontraron estudiantes.";
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
