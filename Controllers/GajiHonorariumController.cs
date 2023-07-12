using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Dynamic;
using payrolTendik.DAO;

namespace payrolTendik.Controllers
{
    [Authorize(Roles ="Dosen,Pranata Komputer,Pustakawan,Administrasi,Pranata Laboratorium Pendidikan")]
    public class GajiHonorariumController : Controller
    {
        HomeDAO dao;
        GeneralDAO daoGeneral;
        public GajiHonorariumController()
        {
            dao = new HomeDAO();
            daoGeneral = new GeneralDAO();
        }
        public IActionResult SlipGaji(int tahun, int bulan, string npp)
        {
            dynamic objek = new ExpandoObject();

            objek.tahun = tahun;
            objek.bulan = daoGeneral.getDataBulan();

            return View(objek);
        }
        // Fungsi untuk Cetak Slip Gaji
        public IActionResult CetakSlipGajiKaryawan(int id_tahun, int id_bulan, string npp)
        {
            dynamic objek = new ExpandoObject();
            string halaman = "CetakSlipGaji";
            
            npp = User.Claims
            .Where(c => c.Type == "username")
            .Select(c => c.Value).SingleOrDefault();

            objek.data = dao.getOneKaryawan(id_tahun, id_bulan, npp);
            objek.data2 = dao.getDataKepalaKantor();
            objek.dataGaji = dao.getGajiKomponenGaji(id_tahun, id_bulan, npp);
            objek.dataGajiKP = dao.getGajiKomponenPotongan(id_tahun, id_bulan, npp);
            objek.dataPajakPenghasilan = dao.getPerhitunganPajakPenghasilan(id_tahun, id_bulan, npp);

            if (dao.getGajiKomponenGaji(id_tahun, id_bulan, npp).Count() == 0)
            {

                TempData["error"] = "Data Tidak Ditemukan!";

            }
            else
            {
                return new ViewAsPdf(halaman, objek)
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
            }

            return RedirectToAction("SlipGaji");
        }
        public IActionResult SlipHonorarium(int id_tahun, int id_bulan, string npp)
        {
            dynamic objek = new ExpandoObject();
            objek.tahun = id_tahun;
            objek.bulan = daoGeneral.getDataBulan();

            return View(objek);
        }
        // Fungsi untuk Mencetak Slip Honorarium 
        public IActionResult CetakHonorariumKaryawan(int id_tahun, int id_bulan, string npp)
        {

            dynamic objek = new ExpandoObject();
            string halaman = "CetakHonorarium";
           
            npp = User.Claims
            .Where(c => c.Type == "username")
            .Select(c => c.Value).SingleOrDefault();

            objek.data = dao.getKaryawanHonor(id_tahun, id_bulan, npp);
            objek.honor = dao.getHonorarium(id_tahun, id_bulan, npp);

            if (dao.getHonorarium(id_tahun, id_bulan, npp).Count() == 0)
            {
                TempData["error"] = "Data Tidak Ditemukan!";
            }
            else
            {
                return new ViewAsPdf(halaman, objek)
                {
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageMargins = {
                                Left = 10,
                                Right = 10,
                                Top = 10,
                                Bottom = 10
                            }
                };
            }
            return RedirectToAction("SlipHonorarium");
        }
        public IActionResult PotonganKoperasi(int id_tahun, int id_bulan, string npp)
        {
            dynamic objek = new ExpandoObject();
            npp = User.Claims
           .Where(c => c.Type == "username")
           .Select(c => c.Value).SingleOrDefault();
            objek.tahun = id_tahun;
            objek.bulan = daoGeneral.getDataBulan();
            objek.table = dao.getPotonganKoperasi(id_tahun, id_bulan, npp);

            return View(objek);
        }
    }
}
