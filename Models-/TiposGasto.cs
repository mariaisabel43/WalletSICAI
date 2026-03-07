using WalletSICAI.Models;

namespace WalletSICAI.Models_
{
    public class TiposGasto
    {
        public int TipoGastoId { get; set; }
        public int InstitucionId { get; set; }
        public int AdministrativoId { get; set; }
        public string Categoria { get; set; }
        public int Precio { get; set; }
        public virtual Institucione? Institucion { get; set; }
        public virtual Administrativo? Administrativo { get; set; }

        public virtual ICollection<GastosEstudiante> GastosEstudiantes { get; set; }
    }
}
