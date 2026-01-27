using System;
using System.Collections.Generic;

namespace WalletSICAI.Models;

public partial class Estudiante
{
    public int EstudianteId { get; set; }

    public string EstudianteNombre { get; set; } = null!;

    public string EstudianteApellido { get; set; } = null!;

    public string EstudianteCedula { get; set; } = null!;

    public string? EstudianteSeccion { get; set; }

    public DateOnly EstudianteFechaNacimiento { get; set; }

    public string? EstudianteNumeroTelefonico { get; set; }

    public string? EstudianteEmailPersonal { get; set; }

    public string? EstudianteEmailInstitucion { get; set; }

    public int? MontoActual { get; set; }

    public int? InstitucionId { get; set; }

    public string EstudianteNombreCompleto { get; set; } = null!;

    public virtual ICollection<HistorialRecargaEstudiante> HistorialRecargaEstudiantes { get; set; } = new List<HistorialRecargaEstudiante>();

    public virtual Institucione? Institucion { get; set; }

    public virtual ICollection<Recarga> Recargas { get; set; } = new List<Recarga>();
}
