using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Services;

public class EstudiantesController : Controller
{
    private readonly AuthService _authService;
    public EstudiantesController(AuthService authService)
    {
        _authService = authService;
    }

    // Acción para buscar estudiantes
    [HttpGet] 
    public IActionResult Buscar() {
        return View(); 
    }
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

    // Acción para ver historial de un estudiante
    public async Task<IActionResult> Historial(int id)
    {
        var adminIdClaim = User.FindFirst("AdministrativoId");
        if (adminIdClaim == null)
            return Unauthorized();

        int adminId = int.Parse(adminIdClaim.Value);

        var estudiante = await _authService.ObtenerEstudianteGastosAsync(id, adminId);

        if (estudiante == null)
            return NotFound();

        return View(estudiante);
    }

}
