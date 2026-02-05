using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletSICAI.Services;
using WalletSICAI.viewModels;

namespace WalletSICAI.Controllers
{
    //Recibe por inyección de dependencias un servicio AuthService, que se encarga de la lógica de autenticación y búsqueda de usuarios/estudiantes.

    //Guarda ese servicio en _authService para usarlo en las acciones.
    public class CuentaController : Controller
    {
        private readonly AuthService _authService;
        public CuentaController(AuthService authService) 
        { 
            _authService = authService; 
        }

        //Muestra la vista de login
        public IActionResult Login()
        {
            return View();
        }
        /*
         Recibe el formulario de login.

        Valida el modelo.

        Llama a _authService.LoginAsync para verificar credenciales.

        Si son válidas:

        Crea una lista de claims (información sobre el usuario, aquí solo el email).

        Construye un ClaimsIdentity con el esquema "MiCookieAuth"(Claim se usa  para autorizar acceso, yo lo use tambièn para guardar con cookies).

        Crea un ClaimsPrincipal y lo guarda en la cookie con SignInAsync.

        Redirige al usuario al Index del HomeController.

        Si no son válidas:

        Agrega un error al ModelState.

        Devuelve la vista con el mensaje “Credenciales inválidas”.

        Esto implementa el login con cookies
         */
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _authService.LoginAsync(model.AdministrativoEmail, model.AdministrativoPassword);
                if (usuario != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.AdministrativoEmail),
                new Claim("AdministrativoId", usuario.AdministrativoId.ToString()) // 👈 Guardamos el ID
            };

                    var identity = new ClaimsIdentity(claims, "MiCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MiCookieAuth", principal);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Credenciales inválidas");
                    return View(model);
                }
            }
            return View(model);
        }

        /*Login Funcional*/
        /*public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuarioValido = await _authService.LoginAsync(model.AdministrativoEmail, model.AdministrativoPassword);
                if (usuarioValido) 
                {
                    var claims = new List<Claim> 
                    {
                        new Claim(ClaimTypes.Name, model.AdministrativoEmail) 
                    }; 
                    var identity = new ClaimsIdentity(claims, "MiCookieAuth");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("MiCookieAuth", principal);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Credenciales inválidas");
                    return View(model);
                }
            }
            return View(model);
        }*/

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
