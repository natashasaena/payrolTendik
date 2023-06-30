namespace payrolTendik.Models
{
    public class GeneralModel
    {      
        public string npp { get; set; }
        public string nama { get; set; }
   
    }
    
    public class UnitModel
    {
        public int id_unit { get; set; }
        public string nama_unit { get; set; }
    }

    public class YearMonthModel
    {
        public int id_tahun { get; set; }
        public int id_bulan { get; set; }
        public int id_bulan_gaji { get; set; }
        public string bulan { get; set; }
    }
    public class FungsionalModel
    {
        public int id_fungsional { get; set; }
        public string status_fungsional { get; set; }
    }
}
