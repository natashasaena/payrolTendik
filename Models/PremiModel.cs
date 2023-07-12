namespace payrolTendik.Models
{
    public class PremiModel
    {
        public int id { get; set; }
        public int id_unit { get; set; }
        public string nama_unit { get; set; }
        public int id_fungsional { get; set; }
        public string status_fungsional { get; set; }
        public int id_komponen_gaji { get; set; }
        public string komponen_gaji { get; set; }
        public string npp { get; set; }
        public string nama { get; set; }
        public string jumlah { get; set; }
        public float total { get; set; }
        public float jml_potongan { get; set; }
    }
    public class BpjsModel    {
        
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public int TOTAL { get; set; }
        public int JML_POTONGAN { get; set; }
    }

    public class AvristModel
    {
        public int id { get; set; }
        public string npp { get; set; }
        public string nama { get; set; }
        public string jumlah { get; set; }
    }
    
}
