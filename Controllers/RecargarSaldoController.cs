using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Services;

namespace WalletSICAI.Controllers
{
    public class RecargarSaldoController : Controller
    {
        private readonly AuthService _authService;
        public RecargarSaldoController(AuthService authService)
        {
            _authService = authService;
        }
        [HttpGet] 
        public IActionResult Recargar() {
            return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> Recargar(Recarga recarga, string EstudianteCedula, string EstudianteNombreCompleto)
        {
            var administrativoIdClaim = User.FindFirst("AdministrativoId")?.Value;
            if (string.IsNullOrEmpty(administrativoIdClaim))
            {
                TempData["Error"] = "No se pudo identificar al administrador.";
                return View("Recarga", recarga);
            }

            recarga.AdministrativoId = int.Parse(administrativoIdClaim);

            var resultado = await _authService.RecargaAsync(recarga, EstudianteCedula, EstudianteNombreCompleto);

            if (!resultado)
            {
                TempData["Error"] = "No se pudo realizar la recarga. Estudiante no encontrado.";
                return View("Recarga", recarga);
            }

            TempData["Exito"] = "Recarga realizada con éxito.";
            return RedirectToAction("Recargar");
        }
    }
}
