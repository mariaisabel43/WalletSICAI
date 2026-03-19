using WalletSICAI.Models;

namespace WalletSICAI.Models_
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int AdministrativoId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }

        public virtual Administrativo Administrativo { get; set; }
    }
}
