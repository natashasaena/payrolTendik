using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{
    public class RoleController : Controller
    {
        RoleDAO dao;
        GeneralDAO daoGeneral;
        public RoleController()
        {
            dao = new RoleDAO();
            daoGeneral = new GeneralDAO();
        }
        public IActionResult Index(string npp)
        {
            dynamic objek = new ExpandoObject();
            objek.deskripsi_role = dao.getAllRole();
            objek.data = daoGeneral.getDatabyNpp(npp);
            objek.table = dao.getAllRoleTendik();
            return View(objek);
        }      

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult TambahRole(RoleModel mdl)
        {
            if (dao.simpanRole(mdl) == 1)
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
        public IActionResult UbahRole(RoleModel mdl)
        {

            if (dao.ubahRole(mdl) == 1)
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
        public IActionResult HapusRole(RoleModel mdl)
        {

            if (dao.hapusRole(mdl) == 1)
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
