using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<Administrativo?> LoginAsync(string email, string password)
        {
            var user = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);

            if (user == null) return null;

            // ?? Usar Unicode porque SQL CONVERT usa UTF-16
            var passwordBytes = Encoding.Unicode.GetBytes(password);

            // Concatenar con la sal
            var passwordWithSalt = passwordBytes.Concat(user.AdministrativoSalt).ToArray();

            // Calcular hash
            using var sha256 = SHA256.Create();
            var inputHash = sha256.ComputeHash(passwordWithSalt);

            // Comparar con el hash almacenado
            if (inputHash.SequenceEqual(user.AdministrativoPassword))
                return user;

            return null;
        }


        public async Task<List<Estudiante>> BuscarEstudiantesPorInstitucionAsync(string buscar, int adminId)
        {
            // Buscar el administrador y su institución
            var admin = await _context.Administrativos
                .FirstOrDefaultAsync(a => a.AdministrativoId == adminId);

            if (admin == null)
                return new List<Estudiante>();

            // Filtrar estudiantes de la misma institución
            var query = _context.Estudiantes
                .Where(e => e.InstitucionId == admin.InstitucionId);

            // Aplicar búsqueda adicional (por cédula o nombre)
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(e => e.EstudianteCedula.Contains(buscar)
                                      || e.EstudianteNombreCompleto.Contains(buscar));
            }

            return await query.ToListAsync();
        }


        //Metodo de busqueda
        public async Task<List<Estudiante>> BuscarEstudiantesAsync(string buscar) 
        { 
            return await _context.Estudiantes
                .Where(u => u.EstudianteCedula.Contains(buscar) 
                || u.EstudianteNombreCompleto.Contains(buscar)).ToListAsync();
        }
        /*Modal*/
        public async Task<Estudiante?> ObtenerEstudiantePorCedula(string cedula)
        {
            return await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteCedula == cedula);
        }
        public async Task<Administrativo?> ObtenerAdministrativoPorIdAsync(int adminId)
        {
            return await _context.Administrativos
                .FirstOrDefaultAsync(a => a.AdministrativoId == adminId);
        }

        public async Task<bool> RecargaAsync(Recarga recarga, string estudianteCedula, string estudianteNombreCompleto)
        {
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => 
                (!string.IsNullOrEmpty(estudianteCedula) && e.EstudianteCedula == estudianteCedula) || 
                (!string.IsNullOrEmpty(estudianteNombreCompleto) && e.EstudianteNombreCompleto == estudianteNombreCompleto));
            if (estudiante == null) return false;
            // Asociar estudiante a la recarga
            recarga.EstudianteId = estudiante.EstudianteId;
            recarga.FechaRecarga = DateOnly.FromDateTime(DateTime.Now);

            // Guardar recarga
            _context.Recargas.Add(recarga);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<VwHistorialRecarga>> ObtenerHistorialEstudianteAsync(string cedula, string? nombreCompleto = null)
        {
            var query = _context.VwHistorialRecargas.AsQueryable();

            if (!string.IsNullOrEmpty(cedula))
                query = query.Where(v => v.EstudianteCedula == cedula);

            if (!string.IsNullOrEmpty(nombreCompleto))
                query = query.Where(v => v.EstudianteNombreCompleto.Contains(nombreCompleto));

            return await query
                .OrderByDescending(v => v.FechaRecarga)
                .ToListAsync();
        }

    }
}

