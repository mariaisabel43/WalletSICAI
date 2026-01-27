using System;
using System.Collections.Generic;

namespace WalletSICAI.Models;

public partial class Recarga
{
    public int RecargaId { get; set; }

    public int? EstudianteId { get; set; }

    public int? AdministrativoId { get; set; }

    public string? SolicitanteRecargaNombre { get; set; }

    public string? SolicitanteRecargaApellido { get; set; }

    public string? SolicitanteRecargaCedula { get; set; }

    public string? SolicitanteRecargaEmail { get; set; }

    public string? ModoPagoRecarga { get; set; }

    public int? MontoRecarga { get; set; }

    public DateOnly? FechaRecarga { get; set; }

    public string? SolicitanteRecargaNombreCompleto { get; set; }

    public virtual Administrativo? Administrativo { get; set; }

    public virtual Estudiante? Estudiante { get; set; }

    public virtual ICollection<HistorialRecargaEstudiante> HistorialRecargaEstudiantes { get; set; } = new List<HistorialRecargaEstudiante>();
}
