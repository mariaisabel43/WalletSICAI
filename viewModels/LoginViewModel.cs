using System.ComponentModel.DataAnnotations;

namespace WalletSICAI.viewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email es requerido")]        
        public string AdministrativoEmail { get; set; }
        [Required(ErrorMessage = "Contraseña es requerida")]
        [DataType(DataType.Password)]
        public string AdministrativoPassword { get; set; }
        [Display(Name ="Remember me?")]
        public bool RememberMe { get; set; }

    }
}
