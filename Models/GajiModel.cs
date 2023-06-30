namespace payrolTendik.Models
{
    public class GajiModel
    {
        public int id { get; set; }
        public int id_tahun { get; set; }
        public int id_bulan { get; set; }
        public string bulan { get; set; }
        public int id_unit { get; set; }
        public string nama_unit { get; set; }
        public string npp { get; set; }
        public string nama { get; set; }
        public string masa_kerja_gol { get; set; }
        public string gaji_pokok { get; set; }
        public string tunjangan_suami_istri { get; set; }
        public string tunjangan_anak { get; set; }
        public string tunjangan_beras { get; set; }
        public string tunjangan_yayasan { get; set; }
        public string tunjangan_struktural { get; set; }
        public string tunjangan_fungsional { get; set; }
        public string tunjangan_pengobatan { get; set; }
        public int is_permanent { get; set; }
        public string tgl_cetak { get; set; }
        public string status_kepegawaian { get; set; }
        public string masa_kerja_riil { get; set; }
        public string pangkat { get; set; }
        public string npwp { get; set; }
        public string no_tabungan { get; set; }
        public string jbt_struktural { get; set; }
        public string jbt_fungsional { get; set; }
        public string golongan { get; set; }
        public string nominal { get; set; }
        public string komponen_gaji { get; set; }
        public string penerimaan_kotor { get; set; }
        public string total_potongan { get; set; }
        public string penerimaan_bersih { get; set; }
        public string honor_dop { get; set; }
        public string premi_astek { get; set; }
        public string premi_bpjs_kesehatan { get; set; }
        public string avrist { get; set; }
        public string biaya_jabatan { get; set; }
        public string biaya_yadapen { get; set; }
        public string bpjs_kesehatan { get; set; }
        public string bpjs_ketenagakerjaan { get; set; }
        public string pdp_tidak_kena_pajak { get; set; }
        public string pdp_kena_pajak { get; set; }
        public string pajak_seharusnya { get; set; }
        public string pajak_potong { get; set; }
        public string penyesuaian_pajak { get; set; }
        public string total_penghasilan { get; set; }
        public string total_pengurangan { get; set; }
        public string keterangan { get; set; }
        public string jumlah { get; set; }
        public int num { get; set; }
        public string pajak { get; set; }
        public string jumlah_satuan { get; set; }
        public byte[] ttd { get; set; }
    }
}
