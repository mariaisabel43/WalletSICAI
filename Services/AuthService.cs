using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;

namespace WalletSICAI.Services
{
    public class AuthService
    {
        private readonly WalletContext _context;

        public AuthService(WalletContext context)
        {
            _context = context;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);

            if (user == null) return false;

            return user.AdministrativoPassword == password; 
        }
    }
}
