using System;
using System.Collections.Generic;

namespace WalletSICAI.Models;

public partial class Administrativo
{
    public int AdministrativoId { get; set; }

    public string AdministrativoNombre { get; set; } = null!;

    public string AdministrativoApellido { get; set; } = null!;

    public string? AdministrativoCedula { get; set; }

    public string AministrativoPuesto { get; set; } = null!;

    public byte[]? AdministrativoPassword { get; set; } = null!;

    public byte[]? AdministrativoSalt { get; set; }

    public int? InstitucionId { get; set; }

    public string AdministrativoNombreCompleto { get; set; } = null!;

    public string? AdministrativoEmail { get; set; }

    public virtual Institucione? Institucion { get; set; }

    public virtual ICollection<Recarga> Recargas { get; set; } = new List<Recarga>();
}
