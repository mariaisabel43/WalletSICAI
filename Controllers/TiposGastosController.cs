using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Models_;
using WalletSICAI.Services;
using WalletSICAI.viewModels;

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
        public async Task<IActionResult> CrearTipoGasto(TiposGastoViewModel vm)
        {
            var adminIdClaim = User.FindFirst("AdministrativoId");
            if (adminIdClaim == null)
            {
                TempData["Error"] = "No se pudo identificar al administrador.";
                return RedirectToAction("Index", "Gastos", new { tab = "tipo" });
            }

            int adminId = int.Parse(adminIdClaim.Value);

            var admin = await _authService.ObtenerAdministrativoPorIdAsync(adminId);
            if (admin == null)
            {
                TempData["Error"] = "Administrador no encontrado.";
                return RedirectToAction("Index", "Gastos", new { tab = "tipo" });
            }

            // Extraemos el objeto real a guardar
            var tipo = vm.NuevoTipo;
            tipo.AdministrativoId = admin.AdministrativoId;
            tipo.InstitucionId = admin.InstitucionId.Value;

            var resultado = await _authService.CrearTipoGastoAsync(tipo);

            if (resultado)
            {
                TempData["Exito"] = "Categoría de gasto creada con éxito.";
                return RedirectToAction("Index", "Gastos", new { tab = "tipo" });
            }

            TempData["Error"] = "No se pudo crear la categoría de gasto.";
            return RedirectToAction("Index", "Gastos", new { tab = "tipo" });
        }

        // GET: Editar Tipo de Gasto
        [HttpGet]
        public async Task<IActionResult> EditarTipoGasto(int id)
        {
            var tipo = await _context.TiposGastos.FindAsync(id);
            if (tipo == null)
            {
                return NotFound();
            }

            var vm = new TiposGastoViewModel
            {
                NuevoTipo = tipo
            };

            return PartialView("_EditarTipoGastoModal", vm); 
        }

        // POST: Editar Tipo de Gasto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarTipoGasto(TiposGastoViewModel vm)
        {
            var tipo = vm.NuevoTipo;
            var resultado = await _authService.EditarTipoGastoAsync(tipo);

            if (resultado)
            {
                TempData["Exito"] = "Categoría actualizada con éxito.";
                return RedirectToAction("Index", "Gastos", new { tab = "tipo" });
            }

            TempData["Error"] = "No se pudo actualizar la categoría.";
            return RedirectToAction("Index", "Gastos", new { tab = "tipo" });
        }


        // POST: Eliminar Tipo de Gasto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTipoGasto(int id)
        {
            var resultado = await _authService.EliminarTipoGastoAsync(id);
            if (resultado)
            {
                TempData["Exito"] = "Categoría eliminada con éxito.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la categoría.";
            }
            return RedirectToAction("Index", "Gastos");
        }


    }
}
