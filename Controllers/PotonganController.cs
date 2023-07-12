using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using System.Dynamic;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{
    [Authorize(Roles = "KSDM")]
    public class PotonganController : Controller
    {
        PotonganDAO dao;
        GeneralDAO daoGeneral;
        public PotonganController()
        {
            dao = new PotonganDAO();
            daoGeneral = new GeneralDAO();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PotonganTetap(int id_tahun)
        {
            dynamic objek = new ExpandoObject();

            objek.tahun = id_tahun;
            objek.bulan = daoGeneral.getDataBulan();
            objek.unit = daoGeneral.getDataUnit();            
            objek.status_fungsional = daoGeneral.getDataStatusFungsional();
            objek.potongan = dao.getDataPotonganTetap();

            return View(objek);
        }
        public IActionResult PotonganVariabel(int id_tahun)
        {
            dynamic objek = new ExpandoObject();

            objek.tahun = id_tahun;
            objek.unit = daoGeneral.getDataUnit();
            objek.bulan = daoGeneral.getDataBulan();
            objek.status_fungsional = daoGeneral.getDataStatusFungsional();
            objek.potongan = dao.getDataPotonganVariabel();

            return View(objek);
        }
        public IActionResult PotonganLainLain(string npp)
        {
            dynamic objek = new ExpandoObject();
          
            objek.bulan = daoGeneral.getDataBulan();
            objek.komponen_potongan = dao.getDataPotonganLainLain();
            objek.data = dao.getDatabyNpp(npp);
            return View(objek);
        }
        public IActionResult ExportPotonganTetap(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji, int id_fungsional)
        {
            byte[] result;
            string namafile = "Template_Import_Potongan.xlsx";
            var data = dao.getKaryawanAll(id_tahun, id_bulan, id_unit, id_komponen_gaji, id_fungsional);
            if (id_tahun == 0 || id_bulan == 0 || id_unit == 00 || id_komponen_gaji == 0 || id_fungsional == 0)
            {
                TempData["error"] = "Data tahun, bulan, unit, nama potongan, dan status fungsional tidak boleh kosong";
                return Redirect("PotonganTetap");
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
        public IActionResult ExportPotonganVariabel(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji, int id_fungsional)
        {
            byte[] result;
            string namafile = "Template_Import_Potongan.xlsx";
            var data = dao.getKaryawanAll(id_tahun, id_bulan, id_unit, id_komponen_gaji, id_fungsional);
            if (id_tahun == 0 || id_bulan == 0 || id_unit == 00 || id_komponen_gaji == 0 || id_fungsional == 0)
            {
                TempData["error"] = "Data tahun, bulan, unit, nama potongan, dan status fungsional tidak boleh kosong";
                return Redirect("PotonganVariabel");
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
        public IActionResult BatchUserUpload(IFormFile batchUsers)
        {

            if (batchUsers == null || batchUsers.Length <= 0)
            {
                TempData["error"] = "Silahkan Upload File";
                return RedirectToAction("PotonganTetap");
            }

            if (!Path.GetExtension(batchUsers.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "File Harus berbentuk Xlsx";
                return RedirectToAction("PotonganTetap");
            }

            List<PotonganModel> lists = new List<PotonganModel>();

            using (var stream = new MemoryStream())
            {
                batchUsers.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {

                        var nama = (worksheet.Cells[row, 1].Value == null) ? "" : worksheet.Cells[row, 1].Value.ToString().Trim();
                        var npp = (worksheet.Cells[row, 2].Value == null) ? "" : worksheet.Cells[row, 2].Value.ToString().Trim();
                        var tahun = (worksheet.Cells[row, 3].Value == null) ? "" : worksheet.Cells[row, 3].Value.ToString().Trim();
                        var bulan = (worksheet.Cells[row, 4].Value == null) ? "" : worksheet.Cells[row, 4].Value.ToString().Trim();
                        var komponen_gaji = (worksheet.Cells[row, 5].Value == null) ? "" : worksheet.Cells[row, 5].Value.ToString().Trim();
                        var jumlah = (worksheet.Cells[row, 6].Value == null) ? "" : worksheet.Cells[row, 6].Value.ToString().Trim();

                        if (!String.IsNullOrEmpty(npp) && String.IsNullOrEmpty(komponen_gaji))
                        {
                            TempData["error"] = "Error! Kolom email wajib diisi!";
                            return RedirectToAction("Index");
                        }

                        if (!String.IsNullOrEmpty(npp))
                        {
                            lists.Add(new PotonganModel
                            {
                                id_tahun = int.Parse(tahun),
                                id_bulan = int.Parse(bulan),
                                npp = npp,
                                nama = nama,
                                id_komponen_gaji = int.Parse(komponen_gaji),
                                nominal = float.Parse(jumlah)
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
                return RedirectToAction("PotonganTetap");
            }
        }
        public JsonResult cariPotongan(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji, int id_fungsional)
        {
            var data = dao.getDataPotongan(id_tahun, id_bulan, id_unit, id_komponen_gaji, id_fungsional);
            return Json(data);
        }

        [HttpPost]
        public IActionResult UbahPotongan(List<PotonganModel> mdl)
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
      
    }
}
