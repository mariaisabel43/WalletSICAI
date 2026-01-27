using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity;
using System.Security.Claims;
using WalletSICAI.Models;
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
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
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
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
