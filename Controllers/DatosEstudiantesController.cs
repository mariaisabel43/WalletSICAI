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

        /*
         Recibe un parámetro buscar.*/
        [HttpGet] 
        public async Task<IActionResult> Buscar(string buscar) 
        { 
            var resultados = await _authService.BuscarEstudiantesAsync(buscar); 
            return View(resultados); 
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
