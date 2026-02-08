using System;
using System.Collections.Generic;

namespace WalletSICAI.Models;

public partial class VwHistorialRecarga
{
    public int HistorialId { get; set; }

    public string EstudianteNombreCompleto { get; set; } = null!;

    public string EstudianteCedula { get; set; } = null!;

    public int? MontoRecarga { get; set; }

    public DateOnly? FechaRecarga { get; set; }

    //public string? SolicitanteRecargaNombre { get; set; }

    //public string? SolicitanteRecargaApellido { get; set; }

    //public string? SolicitanteRecargaCedula { get; set; }

    public string AdministrativoNombreCompleto { get; set; } = null!;
}
