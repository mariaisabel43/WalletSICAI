using System;
using System.Collections.Generic;

namespace WalletSICAI.Models;

public partial class HistorialRecargaEstudiante
{
    public int HistorialId { get; set; }

    public int? EstudianteId { get; set; }

    public int? RecargaId { get; set; }

    public virtual Estudiante? Estudiante { get; set; }

    public virtual Recarga? Recarga { get; set; }
}
