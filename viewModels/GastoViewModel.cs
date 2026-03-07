namespace WalletSICAI.viewModels
{
    public class GastoViewModel
    {
        public int EstudianteId { get; set; }
        public int TipoGastoId { get; set; }
        public int Precio { get; set; }
        public string Categoria { get; set; }

        public string EstudianteCedula { get; set; }
        public string EstudianteNombreCompleto { get; set; }
        public int MontoGasto { get; set; }
        public string Descripcion { get; set; }
    
        public List<TipoGastoItem> TiposGasto { get; set; } 
        public class TipoGastoItem 
        { 
            public int TipoGastoId { get; set; } 
            public string Categoria { get; set; } 
            public int Precio { get; set; } 
        }
    }

}
