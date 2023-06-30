using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Dynamic;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{
    [Authorize(Roles = "KSDM")]
    public class PremiController : Controller
    {
        PremiDAO dao;
        GeneralDAO daoGeneral;
        public PremiController()
        {
            dao = new PremiDAO();
            daoGeneral = new GeneralDAO();
        }

        public IActionResult IuranBpjs()
        {
            dynamic objek = new ExpandoObject();
            objek.bulan = dao.getDataBulan();
            objek.unit = daoGeneral.getDataUnit();
            objek.status_fungsional = daoGeneral.getDataStatusFungsional();
            objek.namaBpjs = dao.getDataNamaKomponen();
            return View(objek);

        }
        public JsonResult cariIuranBpjs(int id_unit, int id_fungsional, int id_komponen_gaji)
        {
            var data = dao.getDataBpjs(id_unit, id_fungsional, id_komponen_gaji);
            return Json(data);
        }
        public IActionResult ExportIuranBpjs(int id_unit, int id_fungsional, int id_komponen_gaji)
        {
            byte[] result;
            string namafile = "Template_Import_Iuran_BPJS.xlsx";
            var data = dao.getKaryawanAll(id_unit, id_fungsional, id_komponen_gaji);
            if (id_unit == 00 || id_komponen_gaji == 0 || id_fungsional == 0)
            {
                TempData["error"] = "Data unit, komponen gaji, dan status fungsional tidak boleh kosong";
                return Redirect("IuranBpjs");
            }
            using (var package = new ExcelPackage())
            {
                try
                {
                    var worksheet = package.Workbook.Worksheets.Add("Nama Sheet"); //Worksheet name
                    worksheet.Cells.LoadFromCollection(data, true);
                    result = package.GetAsByteArray();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return File(result, "application/ms-excel", namafile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportIuranBpjs(IFormFile batchUsers)
        {

            if (batchUsers == null || batchUsers.Length <= 0)
            {
                TempData["error"] = "Silahkan Upload File";
                return RedirectToAction("IuranBpjs");
            }

            if (!Path.GetExtension(batchUsers.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "File Harus berbentuk Xlsx";
                return RedirectToAction("IuranBpjs");
            }

            List<PremiModel> lists = new List<PremiModel>();

            using (var stream = new MemoryStream())
            {
                batchUsers.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var komponen_gaji = (worksheet.Cells[row, 1].Value == null) ? "" : worksheet.Cells[row, 1].Value.ToString().Trim();
                        var npp = (worksheet.Cells[row, 2].Value == null) ? "" : worksheet.Cells[row, 2].Value.ToString().Trim();
                        var nama = (worksheet.Cells[row, 3].Value == null) ? "" : worksheet.Cells[row, 3].Value.ToString().Trim();
                        var total = (worksheet.Cells[row, 4].Value == null) ? "" : worksheet.Cells[row, 4].Value.ToString().Trim();
                        var jml_potongan = (worksheet.Cells[row, 5].Value == null) ? "" : worksheet.Cells[row, 5].Value.ToString().Trim();

                        if (!String.IsNullOrEmpty(npp) && String.IsNullOrEmpty(komponen_gaji))
                        {
                            TempData["error"] = "Error! Kolom email wajib diisi!";
                            return RedirectToAction("Index");
                        }

                        if (!String.IsNullOrEmpty(npp))
                        {
                            lists.Add(new PremiModel
                            {                             
                                npp = npp,
                                id_komponen_gaji = int.Parse(komponen_gaji),
                                total = float.Parse(total),
                                jml_potongan = float.Parse(jml_potongan),
                            });
                        }
                    }
                    if (dao.simpanData(lists))
                    {
                        TempData["success"] = "Berhasil Upload Data!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal Mengupload Data!";
                    }

                }
                return RedirectToAction("IuranBpjs");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UbahBpjs(List<PremiModel> mdl)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = dao.ubahData(mdl);
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
       
        public IActionResult IuranAvrist(string npp)
        {
            dynamic objek = new ExpandoObject();           
            objek.data = daoGeneral.getDatabyNpp(npp);
            objek.table = dao.getAllDataAvrist();
            return View(objek);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult TambahIuranAvrist(AvristModel mdl)
        {
            if (dao.simpanIuranAvrist(mdl) == 1)
            {
                TempData["success"] = "Berhasil menambahkan data!";

            }
            else 
            {
                TempData["error"] = "Data sudah ada!";
            }

            return RedirectToAction("IuranAvrist");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UbahIuranAvrist(AvristModel mdl)
        {

            if (dao.ubahIuranAvrist(mdl) == 1)
            {
                TempData["success"] = "Berhasil mengubah data!";
            }
            else
            {
                TempData["error"] = "Gagal Mengubah Data";
            }

            return RedirectToAction("IuranAvrist");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult HapusIuranAvrist(AvristModel mdl)
        {

            if (dao.hapusIuranAvrist(mdl) == 1)
            {
                TempData["success"] = "Berhasil menghapus data!";
            }
            else
            {
                TempData["error"] = "Gagal Mengubah Data";
            }

            return RedirectToAction("IuranAvrist");
        }
    }
}
