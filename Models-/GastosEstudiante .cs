namespace WalletSICAI.Models;

public class GastosEstudiante
{
    public int GastoID { get; set; }
    public int EstudianteID { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaGasto { get; set; }
    public virtual Estudiante? Estudiante { get; set; }
}
