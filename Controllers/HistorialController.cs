using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
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
        [HttpGet]
        public IActionResult BuscarEstudiante()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HistorialEstudiante(string cedula, string nombreCompleto)
        {
            var historial = await _authService.ObtenerHistorialEstudianteAsync(cedula, nombreCompleto);
            if (!historial.Any())
            {
                ViewBag.Error = "No se encontraron recargas para este estudiante.";
                return View(new List<VwHistorialRecarga>());
            }
            return View(historial);
        }
    }
}