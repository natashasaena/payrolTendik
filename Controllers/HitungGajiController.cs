using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{
    [Authorize(Roles ="KSDM")]
    public class HitungGajiController : Controller
    {
        HomeDAO dao;
        HitungGajiDAO dao1;
        GeneralDAO daoGeneral;
        public HitungGajiController()
        {
            dao = new HomeDAO();           
            daoGeneral = new GeneralDAO();
        }
        public IActionResult Index()
        {
            return View();
        }

        //Fungsi Hitung Gaji
        public JsonResult insertGaji(int id_tahun, int id_bulan, int id_unit, int id_tunj)
        {
            var data = dao.insertGajiUnit(id_tahun, id_bulan, id_unit, id_tunj);
            return Json(data);
        }    

        //Fungsi Hapus Gaji
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult HapusGaji(List<GajiModel> mdl)
        {          
            var success = 0;

            success = dao.hapusGajiUnit(mdl);
            if (success != 0)
            {
                return Json(new { pesan = "Berhasil Hapus Gaji", status = true });
            }
            else
            {
                return Json(new { pesan = "Data sudah permanen!", status = false });    
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SimpanPermanenGaji(List<GajiModel> mdl)
        {           
            var success = 0;

            success = dao.simpanPermanenGaji(mdl);
            if (success != 0)
            {
                return Json(new { pesan = "Berhasil Simpan Permanen", status = true });
            }
            else
            {
                return Json(new { pesan = "Gagal Simpan Permanen", status = false });
            }         
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SimpanPermanenGajiPerUnit(List<GajiModel> mdl)
        {
            var success = 0;
            
            success = dao.simpanPermanenGajiUnit(mdl);
            if (success != 0)
            {
                return Json(new {pesan = "Berhasil Simpan Permanen",status=true});              
            }
            else
            {
                return Json(new { pesan = "Gagal Simpan Permanen", status = false });               
            }        
            
        }
    }
}
