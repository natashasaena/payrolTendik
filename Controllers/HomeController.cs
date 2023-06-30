using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Diagnostics;
using System.Dynamic;
using payrolTendik.DAO;
using payrolTendik.Models;
using Ionic.Zip;

namespace payrolTendik.Controllers
{
    [Authorize(Roles = "KSDM")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        HomeDAO dao;
        GeneralDAO daoGeneral;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            dao = new HomeDAO();
            daoGeneral = new GeneralDAO();
        }

        public IActionResult Index(string npp)
        {
            dynamic objek = new ExpandoObject();

            var data2 = dao.getData1();
            objek.persentaseysr = dao.getPersentaseTunjanganYSR();
            objek.tarif_payroll = data2;
            objek.golongan = dao.getGolongan();
            objek.id_komponen = dao.getDataID(npp);

            objek.dataKomponen = dao.getKomponenTetap();
            objek.komponenTendik = dao.getKomponenGajiTendik(npp);
            objek.komponenPendidik = dao.getKomponenGajiPendidik(npp);
            objek.data = dao.getDatabyNpp(npp);
            var items = dao.cariData(npp);

            objek.table = items;
            return View(objek);
        }
        public JsonResult GetGajiPokok(int masa_kerja_golongan, string npp, string golongan)
        {
            List<KomponenGajiModel> gaji_pokok = dao.getGajiPokok(masa_kerja_golongan, npp, golongan);
            return Json(gaji_pokok);
        }
        public JsonResult GetTunjanganYsr(int masa_kerja_golongan, string npp, decimal persentase)
        {
            List<KomponenGajiModel> tunjangan_ysr = dao.getTunjanganYsr(masa_kerja_golongan, npp, persentase);
            return Json(tunjangan_ysr);
        }
        public IActionResult UbahKomponenGajiKaryawan(string npp, int id_komponen_gaji)
        {
            dynamic objek = new ExpandoObject();
            objek.npp = npp;
            objek.id_komponen_gaji = id_komponen_gaji;
            objek.data = dao.getKaryawanbyNpp(npp, id_komponen_gaji); ;
            objek.komponen_gaji = dao.getData1();

            return View(objek);
        }
        [HttpPost]
        public IActionResult TambahKomponenGaji(KomponenGajiModel mdl)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = dao.tambahKomponen(mdl);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " gagal";
            }
            return Json(data);

        }
        [HttpPost]
        public IActionResult UbahKomponenGaji(List<KomponenGajiModel> mdl)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = dao.ubahKomponen(mdl);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " gagal";
            }

            return Json(data);
        }
        [HttpPost]
        public IActionResult HapusKomponenGajiTrue(KomponenGajiModel mdl)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = dao.hapusKomponenTrue(mdl);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " gagal";
            }
            return Json(data);

        }
        public IActionResult HapusKomponenGajiFalse(KomponenGajiModel mdl)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = dao.hapusKomponenFalse(mdl);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " gagal";
            }
            return Json(data);

        }
        public JsonResult CariData(string npp)
        {
            var data = dao.cariData(npp);
            return Json(data);
        }       
        public IActionResult HitungGaji(int id_tahun, int id_bulan, int id_unit)
        {
            dynamic objek = new ExpandoObject();
            objek.tahun = id_tahun;
            objek.bulan = daoGeneral.getDataBulan();
            objek.unit = daoGeneral.getDataUnit();
            return View(objek);
        }
        public async Task<FileStreamResult> DownloadPDFAsync(int id_tahun, int id_bulan, int id_unit)
        {
            var unit = dao.getNppbyUnit(id_unit);
            //string namaUnit = dao.getDataUnitbyParamater(id_unit).nama_unit;
           
            MemoryStream workStream = new MemoryStream();
            using (var zip = new ZipFile())
            {
                foreach (var item in unit)
                {
                    MemoryStream pdfStream = new MemoryStream();
                    dynamic objek = new ExpandoObject();

                    string halaman = "CetakSlipGaji1";
                    string npp = item.npp;

                    objek.data = dao.getOneKaryawan(id_tahun, id_bulan, npp);
                    objek.data2 = dao.getDataKepalaKantor();
                    objek.dataGaji = dao.getGajiKomponenGaji(id_tahun, id_bulan, npp);
                    objek.dataGajiKP = dao.getGajiKomponenPotongan(id_tahun, id_bulan, npp);
                    objek.dataPajakPenghasilan = dao.getPerhitunganPajakPenghasilan(id_tahun, id_bulan, npp);

                    var test = new ViewAsPdf(halaman, objek) 
                    {
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        PageMargins = {
                                    Left = 20,
                                    Right = 20,
                                    Top = 5,
                                    Bottom = 5
                                }
                    };
                    byte[] pdfByteInfo = await test.BuildFile(ControllerContext);
                    zip.AddEntry(npp + ".pdf", pdfByteInfo);
                    pdfStream.Close();

                }

                zip.Save(workStream);
            }
            workStream.Position = 0;

            FileStreamResult fileResult = new FileStreamResult(workStream, System.Net.Mime.MediaTypeNames.Application.Zip);
            fileResult.FileDownloadName = "SlipGaji" + ".zip";

            return fileResult;
        }   
   
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}