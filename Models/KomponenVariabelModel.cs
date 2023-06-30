namespace payrolTendik.Models
{  
    public class KomponenVariabelModel
    {
        public int id_tahun { get; set; }
        public int id_bulan { get; set; } 
        public int id_unit { get; set; }
        public int id { get; set; } 
        public int id_komponen_gaji { get; set; }
        public string komponen_gaji { get; set; }
        public string nama { get; set; }
        public string npp { get; set; }
        public int jumlah_satuan { get; set; }
        public float jumlah { get; set; }
        public bool is_permanent { get; set; }
      
    }

    public class KehadiranModel
    {
        public int id_tahun { get; set; }
        public int id_bulan { get; set; }
        public int hari { get; set; }
        public int terlambat { get; set; }
        public string lembur { get; set; }
        public string lembur_ekstra { get; set; }
        public string lembur_libur { get; set; }
        public string lembur_libur_ekstra { get; set; }
        public string nama { get; set; }
        public string npp { get; set; }
        public int id_komponen_gaji { get; set; }
        public string komponen_gaji { get; set; }
        public float jumlah_satuan { get; set; }
        public int nominal { get; set; }
        public string nama_unit { get; set; }
        public bool is_permanent { get; set; }

    }
    public class KehadiranLemburModel
    {
        public string NPP { get; set; }
        public int TAHUN { get; set; }
        public int BULAN { get; set; }
        public int SATUAN { get; set; }
    }

    public class CutiPanjangModel
    {
        public int is_lock { get; set; }
        public int id { get; set; }
        public int tahun { get; set; }
        public int id_bulan { get; set; }
        public string bulan { get; set; }
        public string npp { get; set; }
        public string nama { get; set; }
        public int jml_bulan_diganti { get; set; }
        public string tgl_pengajuan { get; set; }
        public string tgl_awal_cuti { get; set; }
        public string tgl_akhir_cuti { get; set; }
    }

    public class SuplisiInsentif
    {
        public int UNIT { get; set; }
        public int TAHUN { get; set; }
        public int BULAN { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public int REFERENSI { get; set; }
        public int JUMLAH { get; set; }
    }
    public class PenerimaanLainLain
    {
        public int id_tahun { get; set; }
        public int id_bulan { get; set; }
        public string bulan { get; set; }
        public int id { get; set; }
        public string npp { get; set; }
        public string nama { get; set; }
        public int jumlah { get; set; }
        public string deskripsi { get; set; }
    }
}
