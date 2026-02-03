using Microsoft.AspNetCore.Mvc;
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
        //Metodo de busqueda

        public async Task<List<Estudiante>> BuscarEstudiantesAsync(string buscar) 
        { 
            return await _context.Estudiantes
                .Where(u => u.EstudianteCedula.Contains(buscar) 
                || u.EstudianteNombreCompleto.Contains(buscar)).ToListAsync();
        }
        public async Task<Estudiante?> ObtenerEstudiantePorCedula(string cedula)
        {
            return await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteCedula == cedula);
        }
        
    }
}

