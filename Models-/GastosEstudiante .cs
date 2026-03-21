using WalletSICAI.Models_;

namespace WalletSICAI.Models;

public class GastosEstudiante
{
    public int GastoId { get; set; }
    public int EstudianteId { get; set; }
    public int TipoGastoId { get; set; }
    public string? Descripcion { get; set; }
    public DateOnly? FechaGasto { get; set; }
    public int MontoGasto { get; set; }
    public virtual Estudiante? Estudiante { get; set; }
    public virtual TiposGasto? TipoGastos { get; set; }
    public bool EsDevuelto { get; set; } = false;

}
