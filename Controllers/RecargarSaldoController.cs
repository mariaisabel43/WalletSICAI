using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Services;
using WalletSICAI.viewModels;

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
        public async Task<IActionResult> Recargar(string? cedula)
        {
            var vm = new RecargaViewModel();

            if (!string.IsNullOrEmpty(cedula))
            {
                // Datos si vienen del modal
                var estudiante = await _authService.ObtenerEstudiantePorCedula(cedula);
                if (estudiante != null)
                {
                    vm.EstudianteCedula = estudiante.EstudianteCedula;
                    vm.EstudianteNombreCompleto = estudiante.EstudianteNombreCompleto;
                }
                else
                {
                    TempData["Error"] = "Estudiante no encontrado.";
                }
            }
            else
            {
                // Recarga desde cero 
                vm.EstudianteCedula = string.Empty;
                vm.EstudianteNombreCompleto = string.Empty;
            }

            ViewBag.Exito = TempData["Exito"];
            ViewBag.Error = TempData["Error"];

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Recargar(RecargaViewModel model)
        {
            // Obtener el administrativo que realiza la recarga
            var administrativoIdClaim = User.FindFirst("AdministrativoId")?.Value;
            if (string.IsNullOrEmpty(administrativoIdClaim))
            {
                TempData["Error"] = "No se pudo identificar al administrador.";
                return View("Recargar", model);
            }
            int adminId = int.Parse(administrativoIdClaim); 
            // Buscar administrador y estudiante
            var admin = await _authService.ObtenerAdministrativoPorIdAsync(adminId); 
            var estudiante = await _authService.ObtenerEstudiantePorCedula(model.EstudianteCedula); 
            if (admin == null || estudiante == null) 
            { 
                TempData["Error"] = "No se pudo realizar la recarga. Administrador o estudiante no encontrado.";
                return View("Recargar", model);
            } 
            // Validar que ambos pertenezcan a la misma institución
            if (admin.InstitucionId != estudiante.InstitucionId) 
            { 
                TempData["Error"] = "No puede realizar recargas a estudiantes de otra institución."; 
                return View("Recargar", model); 
            }
            // Construir la entidad Recarga con los datos del ViewModel
            var recarga = new Recarga 
            { 
                EstudianteId = model.EstudianteId, 
                MontoRecarga = model.MontoRecarga, 
                ModoPagoRecarga = model.ModoPagoRecarga, 
                FechaRecarga = DateOnly.FromDateTime(DateTime.Now), 
                AdministrativoId = int.Parse(administrativoIdClaim), 
                SolicitanteRecargaCedula = model.SolicitanteRecargaCedula, 
                SolicitanteRecargaNombre = model.SolicitanteRecargaNombre, 
                SolicitanteRecargaApellido = model.SolicitanteRecargaApellido, 
                SolicitanteRecargaEmail = model.SolicitanteRecargaEmail };

            // Cedula y Nombre se usan para validar que el estudiante existe
            var resultado = await _authService.RecargaAsync(recarga, model.EstudianteCedula, model.EstudianteNombreCompleto);

            if (!resultado)
            {
                TempData["Error"] = "No se pudo realizar la recarga. Estudiante no encontrado.";
                return View("Recargar", model);
            }

            //TempData["Exito"] = "Recarga realizada con éxito.";
            //return RedirectToAction("Recargar");

            TempData["Exito"] = "Recarga realizada con éxito.";
            return RedirectToAction("Recargar", new { cedula = model.EstudianteCedula });

        }


    }
}
