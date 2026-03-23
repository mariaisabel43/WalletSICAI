using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WalletSICAI.Models;
using WalletSICAI.Models_;

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


        public async Task<bool> ResetPasswordAsync(string email, string nuevaPassword)
        {
            var user = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);
            if (user == null) return false;

            var newSalt = RandomNumberGenerator.GetBytes(32);
            var newPasswordBytes = Encoding.Unicode.GetBytes(nuevaPassword);
            var newPasswordWithSalt = newPasswordBytes.Concat(newSalt).ToArray();

            using var sha256 = SHA256.Create();
            var newHash = sha256.ComputeHash(newPasswordWithSalt);

            user.AdministrativoSalt = newSalt;
            user.AdministrativoPassword = newHash;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Estudiante>> BuscarEstudiantesPorInstitucionAsync(string buscar, int adminId)
        {
            var admin = await _context.Administrativos
                .FirstOrDefaultAsync(a => a.AdministrativoId == adminId);

            if (admin == null)
                return new List<Estudiante>();
            var query = _context.Estudiantes
                .Where(e => e.InstitucionId == admin.InstitucionId);
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(e => e.EstudianteCedula.Contains(buscar)
                                      || e.EstudianteNombreCompleto.Contains(buscar));
            }

            return await query.ToListAsync();
        }

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
            recarga.FechaRecarga = DateTime.Now.Date;


            // Guardar recarga
            _context.Recargas.Add(recarga);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<VwHistorialRecarga>> ObtenerHistorialEstudianteAsync(string cedula,string? nombreCompleto = null)
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

        public async Task<(bool Exito, string Mensaje, string Cedula)> DevolverRecargaAsync(int recargaId, int adminId)
        {
            var recarga = await _context.Recargas
                .Include(r => r.Estudiante)
                .FirstOrDefaultAsync(r => r.RecargaId == recargaId);

            if (recarga == null)
                return (false, "Recarga no encontrada.", null);

            var admin = await _context.Administrativos
                .FirstOrDefaultAsync(a => a.AdministrativoId == adminId);

            if (admin == null || admin.InstitucionId != recarga.Estudiante.InstitucionId)
                return (false, "Administrador inválido.", recarga.Estudiante.EstudianteCedula);

            if (recarga.EsDevuelto)
                return (false, "La recarga ya fue devuelta.", recarga.Estudiante.EstudianteCedula);

            var limite = DateTime.Now.AddDays(-30).Date;
            if (recarga.FechaRecarga == null || recarga.FechaRecarga.Value.Date < limite)
                return (false, "No se puede devolver una recarga con más de 30 días de antigüedad.", recarga.Estudiante.EstudianteCedula);

            recarga.EsDevuelto = true;
            await _context.SaveChangesAsync();
            await _context.Entry(recarga.Estudiante).ReloadAsync();

            return (true, "Recarga devuelta correctamente.", recarga.Estudiante.EstudianteCedula);
        }
        public async Task<Estudiante?> ObtenerEstudianteGastosAsync(int estudianteId, int adminId)
        {
            return await _context.Estudiantes
                .Include(e => e.GastosEstudiantes)
                    .ThenInclude(g => g.TipoGastos)
                .FirstOrDefaultAsync(e => e.EstudianteId == estudianteId
                                          && e.InstitucionId == adminId);
        }
        //Devolucion al eliminar gasto
        public async Task<int?> DevolverGastoAsync(int gastoId, int adminId)
        {
            var gasto = await _context.GastosEstudiantes
                .Include(g => g.Estudiante)
                .FirstOrDefaultAsync(g => g.GastoId == gastoId);

            if (gasto == null) return null;

            var admin = await _context.Administrativos
                .FirstOrDefaultAsync(a => a.AdministrativoId == adminId);

            if (admin == null || admin.InstitucionId != gasto.Estudiante.InstitucionId)
                return null;

            if (gasto.EsDevuelto)
                return null; 

            gasto.EsDevuelto = true;
            //gasto.Descripcion += " DEVUELTO";

            // Ajustar saldo
            gasto.Estudiante.MontoActual = (gasto.Estudiante.MontoActual ?? 0) + gasto.MontoGasto;

            await _context.SaveChangesAsync();

            return gasto.EstudianteId;
        }

        // Crear gasto a estudiante
        public async Task<bool> CrearGastoAsync(GastosEstudiante gasto, string estudianteCedula, string estudianteNombreCompleto)
        {
            // Buscar estudiante por cédula o nombre
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e =>
                    (!string.IsNullOrEmpty(estudianteCedula) && e.EstudianteCedula == estudianteCedula) ||
                    (!string.IsNullOrEmpty(estudianteNombreCompleto) && e.EstudianteNombreCompleto == estudianteNombreCompleto));

            if (estudiante == null) return false;

            // Validar que el tipo de gasto exista
            var tipoGasto = await _context.TiposGastos
                .FirstOrDefaultAsync(t => t.TipoGastoId == gasto.TipoGastoId);

            if (tipoGasto == null) return false;

            
            if (estudiante.MontoActual < tipoGasto.Precio) return false;


            // Asignar datos obligatorios
            gasto.EstudianteId = estudiante.EstudianteId;
            gasto.FechaGasto = DateOnly.FromDateTime(DateTime.Now);
            gasto.MontoGasto = tipoGasto.Precio;

            // Guardar gasto
            _context.GastosEstudiantes.Add(gasto);
            await _context.SaveChangesAsync();

            return true;
        }


        

        // Crear nuevo tipo de gasto
        public async Task<bool> CrearTipoGastoAsync(TiposGasto tipo)
        {
            if (string.IsNullOrEmpty(tipo.Categoria) || tipo.Precio <= 0)
                return false;
            _context.TiposGastos.Add(tipo);
            await _context.SaveChangesAsync();
            return true;
        }

        // Editar tipo de gasto existente
        public async Task<bool> EditarTipoGastoAsync(TiposGasto tipo)
        {
            var existente = await _context.TiposGastos
                .FirstOrDefaultAsync(t => t.TipoGastoId == tipo.TipoGastoId);

            if (existente == null) return false;

            // Actualizar propiedades
            existente.Categoria = tipo.Categoria;
            existente.Precio = tipo.Precio;

            _context.TiposGastos.Update(existente);
            await _context.SaveChangesAsync();
            return true;
        }

        // Eliminar tipo de gasto
        public async Task<bool> EliminarTipoGastoAsync(int id)
        {
            var existente = await _context.TiposGastos
                .FirstOrDefaultAsync(t => t.TipoGastoId == id);

            if (existente == null) return false;

            _context.TiposGastos.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}