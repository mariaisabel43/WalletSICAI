using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletSICAI.Models;
using WalletSICAI.Models_;

using WalletSICAI.Services;

namespace WalletSICAI.Controllers
{
    //[Authorize(Roles = "Administrador")]
    public class TiposGastosController : Controller
    {
        private readonly AuthService _authService;
        private readonly WalletContext _context;

        public TiposGastosController(AuthService authService, WalletContext context)
        {
            _authService = authService;
            _context = context;
        }

        // GET: Crear Tipo de Gasto
        [HttpGet]
        public IActionResult CrearTipoGasto()
        {
            return View(new TiposGasto());
        }

        // POST: Crear Tipo de Gasto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTipoGasto(TiposGasto model)
        {
            // Recuperar el Id del administrador desde el claim
            var adminIdClaim = User.FindFirst("AdministrativoId");
            if (adminIdClaim == null)
            {
                TempData["Error"] = "No se pudo identificar al administrador.";
                return View(model);
            }

            int adminId = int.Parse(adminIdClaim.Value);

            // Buscar el administrativo en la BD
            var admin = await _authService.ObtenerAdministrativoPorIdAsync(adminId);
            if (admin == null)
            {
                TempData["Error"] = "Administrador no encontrado.";
                return View(model);
            }

            // Completar el modelo con los IDs del administrador y su institución
            model.AdministrativoId = admin.AdministrativoId;
            model.InstitucionId = admin.InstitucionId.Value;

            var resultado = await _authService.CrearTipoGastoAsync(model);

            if (resultado)
            {
                TempData["Exito"] = "Categoría de gasto creada con éxito.";
                return RedirectToAction("model");
            }

            TempData["Error"] = "No se pudo crear la categoría de gasto.";
            return View(model);
        }

       
    }
}
