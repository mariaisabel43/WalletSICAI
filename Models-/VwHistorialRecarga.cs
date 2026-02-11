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
    public string SolicitanteRecargaNombreCompleto { get; set; } = null!;

    public string AdministrativoNombreCompleto { get; set; } = null!;
}
