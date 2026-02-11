using System.ComponentModel.DataAnnotations;

namespace WalletSICAI.viewModels
{
    public class RecargaViewModel { 
        public int EstudianteId { get; set; } 
        public string EstudianteCedula { get; set; } 
        public string EstudianteNombreCompleto { get; set; } 
        public int MontoRecarga { get; set; }
        public string ModoPagoRecarga { get; set; }
        public string SolicitanteRecargaCedula { get; set; }
        public string SolicitanteRecargaNombre { get; set; }
        public string SolicitanteRecargaApellido { get; set; }
        public string SolicitanteRecargaEmail { get; set; }
    }
}
