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
        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _context.Administrativos
                .FirstOrDefaultAsync(u => u.AdministrativoEmail == email);

            if (user == null) return false;

            return user.AdministrativoPassword == password; 
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
        public async Task<Estudiante?> ObtenerEstudiantePorCedula(string cedula)
        {
            return await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteCedula == cedula);
        }
        
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