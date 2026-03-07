using System;
using System.Collections.Generic;
using WalletSICAI.Models_;

namespace WalletSICAI.Models;

public partial class Institucione
{
    public int InstitucionId { get; set; }

    public string InstitucionNombre { get; set; } = null!;

    public string CedulaJuridica { get; set; } = null!;

    public virtual ICollection<Administrativo> Administrativos { get; set; } = new List<Administrativo>();

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();

    public ICollection<TiposGasto> TiposGastos { get; set; }

}
