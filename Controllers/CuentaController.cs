using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletSICAI.Services;
using WalletSICAI.viewModels;

namespace WalletSICAI.Controllers
{
   
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
                new Claim("AdministrativoId", usuario.AdministrativoId.ToString()) 
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
        
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
