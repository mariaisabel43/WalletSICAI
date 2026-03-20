using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSICAI.Models;
using WalletSICAI.Models_;
using WalletSICAI.Services;
using WalletSICAI.viewModels;

namespace WalletSICAI.Controllers
{
   
    public class CuentaController : Controller
    {
        private readonly AuthService _authService;
        private readonly WalletContext _context;
        private readonly EmailService _emailService;
        public CuentaController (AuthService authService, WalletContext context, EmailService emailService)
        {
            _authService = authService;
            _context = context;
            _emailService = emailService;

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
        //----------------------FUNCIONALIDAD DE RECUPERACIÓN DE CONTRASEÑA----------------------
        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword(string email, string nuevaPassword)
        //{
        //    var result = await _authService.ResetPasswordAsync(email, nuevaPassword);
        //    if (!result)
        //        return BadRequest("No se pudo actualizar la contraseña");
        //    return Ok("Contraseña actualizada exitosamente");
        //}

        //---------------------------------------------------------------------------------------
        [HttpGet("reset-password-confirm")]
        public IActionResult ResetPasswordConfirm(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Token o email inválido.");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                AdministrativoEmail = email
            };

            return View(model); 
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var usuario = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);

            if (usuario == null)
                return BadRequest("Usuario no encontrado");

            var token = Guid.NewGuid().ToString();

            var resetToken = new PasswordResetToken
            {
                AdministrativoId = usuario.AdministrativoId,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(30)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            //var resetLink = Url.Action("ResetPasswordConfirm", "Cuenta",
            //    new { token, email }, Request.Scheme);

            //await _emailService.SendEmailAsync(email, "Reset de contraseña",
            //    $"Haz clic aquí para resetear tu contraseña: {resetLink}");

            var resetLink = Url.Action("ResetPasswordConfirm", "Cuenta",
            new { token, email }, Request.Scheme);

            string body = $@"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f8f9fa;
                        padding: 20px;
                    }}
                    .card {{
                        max-width: 600px;
                        margin: auto;
                        background: #ffffff;
                        border-radius: 8px;
                        border: 1px solid #dee2e6;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                        padding: 20px;
                    }}
                    h2 {{
                        color: #0d6efd;
                        margin-bottom: 20px;
                    }}
                    p {{
                        color: #212529;
                        font-size: 14px;
                        line-height: 1.5;
                    }}
                    .btn {{
                        display: inline-block;
                        padding: 12px 20px;
                        margin-top: 20px;
                        background-color: #0d6efd;
                        color: #ffffff !important;
                        text-decoration: none;
                        border-radius: 5px;
                        font-weight: bold;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 12px;
                        color: #6c757d;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='card'>
                    <h2>Restablecer contraseña</h2>
                    <p>Hola,</p>
                    <p>Recibimos una solicitud para restablecer tu contraseña en <strong>Wallet SICAI</strong>.</p>
                    <p>Haz clic en el siguiente botón para continuar con el proceso:</p>
                    <a href='{resetLink}' class='btn'>Restablecer contraseña</a>
                    <div class='footer'>
                        <p>Si no solicitaste este cambio, puedes ignorar este correo.</p>
                        <p>&copy; {DateTime.Now.Year} Wallet SICAI</p>
                     </div>
                </div>
            </body>
            </html>
            ";

            await _emailService.SendEmailAsync(email, "Restablecimiento de contraseña", body);

            return Ok("Correo de reset enviado");
        }

        [HttpPost("reset-password-confirm")]
        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordViewModel model)
        {
            var usuario = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == model.AdministrativoEmail);

            if (usuario == null)
            //return BadRequest("Usuario no encontrado");
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Login", "Cuenta");
            }


            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == model.Token && t.AdministrativoId == usuario.AdministrativoId);

            if (resetToken == null || resetToken.Expiration < DateTime.UtcNow)
            //return BadRequest("Token inválido o expirado");
            {
                TempData["Error"] = "Token inválido o expirado.";
                return RedirectToAction("Login", "Cuenta");
            }


            var result = await _authService.ResetPasswordAsync(model.AdministrativoEmail, model.nuevaPassword);
            if (!result)
            //return BadRequest("No se pudo actualizar la contraseña");
            {
                TempData["Error"] = "No se pudo actualizar la contraseña.";
                return RedirectToAction("Login", "Cuenta");
            }


            _context.PasswordResetTokens.Remove(resetToken);
            await _context.SaveChangesAsync();

            //return Ok("Contraseña actualizada exitosamente");

            TempData["Exito"] = "Tu contraseña se actualizó exitosamente. Ya puedes iniciar sesión.";

            //return RedirectToAction("Login", "Cuenta", new { mensaje = "success" });
            return RedirectToAction("Login", "Cuenta");
        }



        public async Task<IActionResult> Logout()
        {
            // Cierra la sesión y elimina la cookie de autenticación
            await HttpContext.SignOutAsync("MiCookieAuth");

            return RedirectToAction("Login", "Cuenta");
        }
    }
}
