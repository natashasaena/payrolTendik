namespace payrolTendik.Models
{
    public class PotonganModel
    {
        public int id { get; set; }
        public List<int> tempId { get; set; }
        public List<int> tempNominal { get; set; }
        public List<int> tempJumlah { get; set; }
        public int id_tahun { get; set; }
        public int id_bulan { get; set; }
        public string bulan { get; set; }
        public int id_unit { get; set; }
        public string nama_unit { get; set; }
        public int id_fungsional { get; set; }
        public string status_fungsional { get; set; }
        public int id_komponen_gaji { get; set; }
        public string komponen_gaji { get; set; }
        public string nama { get; set; }
        public string npp { get; set; }
        public int jumlah_satuan { get; set; }
        public float nominal { get; set; }
        //public SelectListItem Potong { get; set; }
    }
    public class PotonganVariabel
    {
        public string NAMA { get; set; }
        public string NPP { get; set; }
        public int TAHUN { get; set; }
        public int BULAN { get; set; }
        public int ID_POTONGAN { get; set; }
        public int NOMINAL { get; set; }
    }
}
