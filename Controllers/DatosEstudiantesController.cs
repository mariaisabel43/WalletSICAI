using Microsoft.AspNetCore.Mvc;
using WalletSICAI.Services;


namespace WalletSICAI.Controllers
{
   
    public class DatosEstudiantesController : Controller
    {
        private readonly AuthService _authService;
        public DatosEstudiantesController(AuthService authService) 
        { 
            _authService = authService; 
        }
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Buscar(string buscar)
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


        public async Task<IActionResult> Informacion(string cedula)
        {
            var estudiante = await _authService.ObtenerEstudiantePorCedula(cedula);
            if (estudiante == null)
            {
                return NotFound();
            }
            return PartialView("_EstudianteInformacion", estudiante);
        }
    }
}
