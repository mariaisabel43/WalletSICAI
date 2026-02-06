using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;

namespace WalletSICAI.Services
{
    public class AuthService
    {
        /*
         Constructor
        Recibe el WalletContext por inyección de dependencias.

        Lo guarda en _context para usarlo en los métodos.

        Esto permite que el servicio trabaje directamente con las tablas Administrativos y Estudiantes.
        */
        private readonly WalletContext _context;

        public AuthService(WalletContext context)
        {
            _context = context;
        }
        /*
         Método LoginAsync
        Busca en la tabla Administrativos un registro cuyo email coincida.

        Si no encuentra nada ? devuelve false.

        Si encuentra un usuario ? compara la contraseña almacenada con la ingresada.

        Devuelve true si coincide, false si no.
         */
        public async Task<Administrativo?> LoginAsync(string email, string password)
        {
            var user = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);

            if (user == null) return null;

            return user.AdministrativoPassword == password ? user : null;
        }
        //Metodo de busqueda
        /*
         Método BuscarEstudiantesAsync
        Busca en la tabla Estudiantes.

        Filtra registros donde la cédula o el nombre completo contengan el texto ingresado (buscar).

        Devuelve una lista (List<Estudiante>) con los resultados.
        */
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


        //Historial por estudiante
        /*public async Task<List<VwHistorialRecarga>> ObtenerHistorialEstudianteAsync(string cedula)
        {
            return await _context.VwHistorialRecargas
                .Where(v => v.EstudianteCedula == cedula)
                .OrderByDescending(v => v.FechaRecarga)
                .ToListAsync();
        }*/

    }
}


/*
 Contexto general
AuthService es un servicio de lógica de negocio que encapsula dos responsabilidades principales:

Login de administrativos (validar credenciales).

Búsqueda de estudiantes (filtrar por cédula o nombre completo).

Se apoya en WalletContext, que es tu DbContext de Entity Framework Core, encargado de conectarse a la base de datos.

 Resumen
 AuthService centraliza la lógica de login y búsqueda de estudiantes.

Se conecta a la base de datos mediante WalletContext.

LoginAsync valida credenciales de administrativos.

BuscarEstudiantesAsync devuelve estudiantes filtrados por cédula o nombre.
*/