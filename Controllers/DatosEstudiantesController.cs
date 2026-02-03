using Microsoft.AspNetCore.Mvc;
using WalletSICAI.Services;


namespace WalletSICAI.Controllers
{
    //Recibe por inyección de dependencias un servicio AuthService, que se encarga de la lógica de autenticación y búsqueda de usuarios/estudiantes.

    //Guarda ese servicio en _authService para usarlo en las acciones.
    public class DatosEstudiantesController : Controller
    {
        private readonly AuthService _authService;
        public DatosEstudiantesController(AuthService authService) 
        { 
            _authService = authService; 
        }

        /*
         Recibe un parámetro buscar.

        Llama a _authService.BuscarEstudiantesAsync(buscar) para obtener estudiantes que coincidan.

        Devuelve la vista Buscar.cshtml con la lista de resultados como modelo.
         */
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
