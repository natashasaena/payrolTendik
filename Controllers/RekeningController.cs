using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{  
     [Authorize(Roles ="KSDM")]
    public class RekeningController : Controller
    {
      
        RekeningDAO dao;
        GeneralDAO daoGeneral;
        public RekeningController()
        {
            dao = new RekeningDAO();
            daoGeneral = new GeneralDAO();
        }
        public IActionResult Index(string npp)
        {
            dynamic objek = new ExpandoObject();

            objek.data = daoGeneral.getDatabyNpp(npp);
            objek.table = dao.getAllRekening();
            return View(objek);
        }    
     
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult TambahRekening(RekeningModel mdl)
        {
            if (dao.simpanRekening(mdl) == 1)
            {
                TempData["success"] = "Berhasil menambahkan data!";

            }
            else
            {
                TempData["error"] = "Data sudah ada!";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UbahRekening(RekeningModel mdl)
        {

            if (dao.ubahRekening(mdl) == 1)
            {
                TempData["success"] = "Berhasil mengubah data!";
            }
            else
            {
                TempData["error"] = "Gagal Mengubah Data";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult HapusRekening(RekeningModel mdl)
        {

            if (dao.hapusRekening(mdl) == 1)
            {
                TempData["success"] = "Berhasil menghapus data!";
            }
            else
            {
                TempData["error"] = "Gagal Mengubah Data";
            }

            return RedirectToAction("Index");
        }
    }
}
