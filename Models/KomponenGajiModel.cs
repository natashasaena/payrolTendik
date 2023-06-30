namespace payrolTendik.Models
{
    public class KomponenGajiModel
    {
        public int row { get; set; }
        public string npp { get; set; }
        public string nama { get; set; }
        public string status_restitusi { get; set; }
        public string gaji_pokok_nominal { get; set; }
        public float gaji_pokok { get; set; }
        public string tunj_suamiistri { get; set; }
        public float tunjangan_suami_istri { get; set; }
        public string tunj_anak { get; set; }
        public float tunjangan_anak { get; set; }
        public int jumlah_anak { get; set; }
        public string tunj_beras { get; set; }
        public float tunjangan_beras { get; set; }
        public string tunj_yayasan { get; set; }
        public float tunjangan_yayasan { get; set; }
        public string tunj_fungsional { get; set; }
        public float tunjangan_fungsional { get; set; }
        public string tunj_struktural { get; set; }
        public float tunjangan_struktural { get; set; }
        public int tunj_transport { get; set; }
        public string tunj_pengobatan { get; set; }
        public float tunjangan_pengobatan { get; set; }
        public string tunj_kependidikan { get; set; }
        public float tunjangan_kependidikan { get; set; }
        public int jumlah_tanggungan { get; set; }
        public decimal persentase { get; set; }
        public int hari { get; set; }
        public int uang_makan { get; set; }
        public int lembur { get; set; }
        public int lembur_libur { get; set; }
        public int lembur_ekstra { get; set; }
        public int lembur_libur_ekstra { get; set; }
        public int id { get; set; }
        public int id_komponen_gaji { get; set; }
        public int id_komponen { get; set; }
        public string komponen_gaji { get; set; }
        public int id_jns_komponen { get; set; }
        public string jns_komponen { get; set; }
        public string golongan { get; set; }
        public string tmt_golongan { get; set; }
        public string purnakarya { get; set; }
        public int masa_kerja_golongan { get; set; }
        public string tgl_sk { get; set; }
        public string status_sipil { get; set; }
        public string status_kepegawaian { get; set; }
        public string nominal { get; set; }
        public int jml_tanggungan { get; set; }
        public float nominal_persentase { get; set; }
        public List<string> temp { get; set; }
        public List<string> tempIdKomponen { get; set; }
        public int deleted { get; set; }
        public string keterangan { get; set; }
        public int is_tunj { get; set; }
    }   
}
