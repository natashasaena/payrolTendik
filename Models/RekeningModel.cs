namespace payrolTendik.Models
{
    public class RekeningModel
    {
        public int id { get; set; }
        public string npp { get; set; }
        public string no_rekening { get; set; }
        public string nama_bank { get; set; }
        public string status_rekening { get; set; }

    }
    public class KaryawanModel
    {
       
        public string npp { get; set; }
        public string nama { get; set; }
        public byte[] ttd { get; set; }
       

    }
}
