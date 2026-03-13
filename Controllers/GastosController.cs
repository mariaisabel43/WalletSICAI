using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using WalletSICAI.Models;
using WalletSICAI.Models_;
using WalletSICAI.Services;
using WalletSICAI.viewModels;


namespace WalletSICAI.Controllers
{
    //[Authorize(Roles = "Administrador")]
    public class GastosController : Controller
    {
        private readonly AuthService _authService;
        private readonly WalletContext _context;
        public GastosController(AuthService authService, WalletContext context)
        {
            _authService = authService;
            _context = context;
        }

        // Acción para buscar estudiantes
        [HttpGet]
        public IActionResult Buscar()
        {
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

        // GET: Crear Gasto
        [HttpGet]
        public IActionResult CrearGasto()
        {
            var model = RecargarCombos();
            return View(model);
        }
        // POST: Crear Gasto
        [HttpPost]
        public async Task<IActionResult> CrearGasto(GastoViewModel model)
        {
            var administrativoIdClaim = User.FindFirst("AdministrativoId")?.Value;
            if (string.IsNullOrEmpty(administrativoIdClaim))
            {
                TempData["Error"] = "No se pudo identificar al administrador.";
                model = RecargarCombos();
                return View("CrearGasto", model);
            }

            int adminId = int.Parse(administrativoIdClaim);
            var admin = await _authService.ObtenerAdministrativoPorIdAsync(adminId);
            if (admin == null)
            {
                TempData["Error"] = "Administrador no encontrado.";
                model = RecargarCombos();
                return View("CrearGasto", model);
            }

            var gasto = new GastosEstudiante
            {
                Descripcion = model.Descripcion,
                TipoGastoId = model.TipoGastoId
            };

            var resultado = await _authService.CrearGastoAsync(
                gasto,
                model.EstudianteCedula,
                model.EstudianteNombreCompleto
            );

            if (!resultado)
            {
                TempData["Error"] = "No se pudo registrar el gasto. Estudiante o tipo de gasto inválido.";
                model = RecargarCombos();
                return View("CrearGasto", model);
            }

            TempData["Exito"] = "Gasto registrado con éxito.";
            return RedirectToAction("CrearGasto");
        }


        // Método auxiliar para recargar combos
        public GastoViewModel RecargarCombos()
        {
            //var model = new GastoViewModel
            //{
            //    TiposGasto = _context.TiposGastos
            //        .Select(t => new GastoViewModel.TipoGastoItem
            //        {
            //            TipoGastoId = t.TipoGastoId,
            //            Categoria = t.Categoria,
            //            Precio = t.Precio
            //        }).ToList() ?? new List<GastoViewModel.TipoGastoItem>()
            //};
            //---------
            var tipos = _context.TiposGastos.ToList() ?? new List<TiposGasto>();

            var vm = new GastoViewModel
            {
                TiposGasto = tipos
                    .Select(t => new GastoViewModel.TipoGastoItem
                    {
                        TipoGastoId = t.TipoGastoId,
                        Categoria = t.Categoria,
                        Precio = t.Precio
                    }).ToList(),

                // 👈 Aquí inicializamos siempre TiposGastoVM
                TiposGastoVM = new TiposGastoViewModel
                {
                    NuevoTipo = new TiposGasto(),
                    Categorias = tipos
                }
            };
            //----------
            ViewBag.Estudiantes = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteNombreCompleto");

            //return model;
            return vm;
        }
        public IActionResult Index()
        {
            var model = RecargarCombos();
            return View(model);
        }



    }
}
