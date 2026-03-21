using System;
using System.Collections.Generic;

namespace WalletSICAI.Models;

public partial class VwHistorialRecarga
{
    public int HistorialId { get; set; }

    public string? EstudianteNombreCompleto { get; set; }

    public string? EstudianteCedula { get; set; } 

    public int? MontoRecarga { get; set; }

    public DateTime? FechaRecarga { get; set; }
    public string? SolicitanteRecargaNombreCompleto { get; set; }

    public string? AdministrativoNombreCompleto { get; set; }
    public bool EsDevuelto { get; set; }
    public int RecargaId { get; set; }

}

