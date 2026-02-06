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
        public async Task<Administrativo?> LoginAsync(string email, string password)
        {
            var user = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);

            if (user == null) return null;

            return user.AdministrativoPassword == password ? user : null;
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



