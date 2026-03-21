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
        // Acción para devolver gasto

        [HttpPost]
        public async Task<IActionResult> DevolverGasto(int id)
        {
            var adminIdClaim = User.FindFirst("AdministrativoId");
            if (adminIdClaim == null)
                return Unauthorized();

            int adminId = int.Parse(adminIdClaim.Value);

            var estudianteId = await _authService.DevolverGastoAsync(id, adminId);

            if (estudianteId == null)
            {
                TempData["Error"] = "No se pudo registrar la devolución.";
                return RedirectToAction("Index", "Gastos");
            }

            TempData["Exito"] = "La devolución se registró correctamente.";

            var estudiante = await _authService.ObtenerEstudianteGastosAsync(estudianteId.Value, adminId);
            if (estudiante == null)
                return NotFound();

            return View("Historial", estudiante);
        }

        // GET: Crear Gasto
        [HttpGet]

        [HttpGet]
        public async Task<IActionResult> CrearGasto(string? cedula, string? nombre)
        {
            var vm = new GastoViewModel();

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
                // Crear gasto desde cero
                vm.EstudianteCedula = string.Empty;
                vm.EstudianteNombreCompleto = string.Empty;
            }

            // 👇 Aquí llamamos a RecargarCombos SIN argumentos
            vm = RecargarCombos(vm);

            ViewBag.Exito = TempData["Exito"];
            ViewBag.Error = TempData["Error"];

            return View(vm);
        }

        // POST: Crear Gasto
        [HttpPost]
        public async Task<IActionResult> CrearGasto(GastoViewModel model)
        {
            var administrativoIdClaim = User.FindFirst("AdministrativoId")?.Value;
            if (string.IsNullOrEmpty(administrativoIdClaim))
            {
                TempData["Error"] = "No se pudo identificar al administrador.";
                //return View("CrearGasto", model);
                return RedirectToAction("Index", "Gastos", new { tab = "gasto" });
            }

            int adminId = int.Parse(administrativoIdClaim);
            var admin = await _authService.ObtenerAdministrativoPorIdAsync(adminId);
            var estudiante = await _authService.ObtenerEstudiantePorCedula(model.EstudianteCedula);

            if (admin == null || estudiante == null)
            {
                TempData["Error"] = "No se pudo registrar el gasto. Administrador o estudiante no encontrado.";
                //return View("CrearGasto", model);
                return RedirectToAction("Index", "Gastos", new { tab = "gasto" });

            }

            if (admin.InstitucionId != estudiante.InstitucionId)
            {
                TempData["Error"] = "No puede registrar gastos a estudiantes de otra institución.";
                //return View("CrearGasto", model);
                return RedirectToAction("Index", "Gastos", new { tab = "gasto" });

            }

            var gasto = new GastosEstudiante
            {
                EstudianteId = estudiante.EstudianteId,
                TipoGastoId = model.TipoGastoId,
                Descripcion = model.Descripcion,
                FechaGasto = DateOnly.FromDateTime(DateTime.Now),
                MontoGasto = model.MontoGasto
            };

            var resultado = await _authService.CrearGastoAsync(
                gasto,
                model.EstudianteCedula,
                model.EstudianteNombreCompleto
            );

            if (!resultado)
            {
                TempData["Error"] = "No se pudo registrar el gasto.";
                //return View("CrearGasto", model);
                return RedirectToAction("Index", "Gastos", new { tab = "gasto" });

            }

            TempData["Exito"] = "Gasto registrado con éxito.";
            //return RedirectToAction("CrearGasto", new { cedula = model.EstudianteCedula });
            return RedirectToAction("Index", "Gastos", new { tab = "gasto" });

        }


        // Versión sin parámetros: crea un modelo nuevo
        private GastoViewModel RecargarCombos()
        {
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

                TiposGastoVM = new TiposGastoViewModel
                {
                    NuevoTipo = new TiposGasto(),
                    Categorias = tipos
                }
            };

            ViewBag.Estudiantes = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteNombreCompleto");

            return vm;
        }

        // Versión con parámetros: completa un modelo existente
        private GastoViewModel RecargarCombos(GastoViewModel vm)
        {
            var tipos = _context.TiposGastos.ToList() ?? new List<TiposGasto>();

            vm.TiposGasto = tipos
                .Select(t => new GastoViewModel.TipoGastoItem
                {
                    TipoGastoId = t.TipoGastoId,
                    Categoria = t.Categoria,
                    Precio = t.Precio
                }).ToList();

            vm.TiposGastoVM = new TiposGastoViewModel
            {
                NuevoTipo = new TiposGasto(),
                Categorias = tipos
            };

            ViewBag.Estudiantes = new SelectList(_context.Estudiantes, "EstudianteId", "EstudianteNombreCompleto");

            return vm;
        }


        public async Task<IActionResult> Index(string? cedula)
        {
            var vm = new GastoViewModel();

            if (!string.IsNullOrEmpty(cedula))
            {
                var estudiante = await _authService.ObtenerEstudiantePorCedula(cedula);
                if (estudiante != null)
                {
                    vm.EstudianteCedula = estudiante.EstudianteCedula;
                    vm.EstudianteNombreCompleto = estudiante.EstudianteNombreCompleto;
                }
                else
                {
                    vm.EstudianteCedula = cedula;
                    vm.EstudianteNombreCompleto = string.Empty;
                    TempData["Error"] = "Estudiante no encontrado.";
                }
            }

            vm = RecargarCombos(vm);
            return View(vm);
        }
    }
}