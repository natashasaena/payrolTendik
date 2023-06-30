using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using System.Dynamic;
using payrolTendik.DAO;
using payrolTendik.Models;

namespace payrolTendik.Controllers
{
    [Authorize(Roles = "KSDM,Unit")]
    public class KomponenVariabelController : Controller
    {
        KomponenVariabelDAO dao;
        GeneralDAO daoGeneral;
        public KomponenVariabelController()
        {
            dao = new KomponenVariabelDAO();
            daoGeneral = new GeneralDAO();
        }
        public IActionResult Index()
        {
            return View();
        }
        //View Kehadiran dan Lembur
        public IActionResult KehadiranLembur(int id_tahun, int id_bulan, int id_unit)
        {
            dynamic objek = new ExpandoObject();
            objek.tahun = id_tahun;
            objek.unit = daoGeneral.getDataUnit();
            objek.bulan = daoGeneral.getDataBulan();
          
            //objek.unit = dao.getDataUnit();
            //objek.bulan = dao.getDataBulan();
            //objek.tahun = id_tahun;

            var data = dao.getKehadiranLembur(id_tahun, id_bulan, id_unit);
            objek.table = data;

            return View(objek);
        }
        //Fungsi untuk mencari data kehadiran dan lembur berdasarkan inputan tahun, bulan dan unit
        public JsonResult cariKehadiranLembur(int id_tahun, int id_bulan, int id_unit)
        {
            var data = dao.getKehadiranLembur(id_tahun, id_bulan, id_unit);         
            return Json(data);
        }
        [HttpPost]
        public IActionResult UbahKehadiranLembur(List<KehadiranModel> mdl)
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
        public JsonResult tambahKehadiranLembur(int id_tahun, int id_bulan, int id_unit)
        {
            var data = dao.tambahData(id_tahun, id_bulan, id_unit);
            return Json(data);
        }
       
        //Export data ke excel di halaman kehadiran dan lembur
        public IActionResult ExportKehadiranLembur(int id_tahun, int id_bulan, int id_unit)
        {
            byte[] result;
            string namafile = "Template_Import_KomponenVariabel.xlsx";
            var data = dao.getKaryawanAll(id_tahun, id_bulan, id_unit);
            if (id_tahun == 0 || id_bulan == 0 || id_unit == 00)
            {
                TempData["error"] = "Data tahun,bulan, dan unit tidak boleh kosong";
                return Redirect("KehadiranLembur");
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
        //Import Data Kehadiran dan Lembur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportDataKehadiranLembur(IFormFile batchUsers, int id_komponen_gaji)
        {

            if (batchUsers == null || batchUsers.Length <= 0)
            {
                TempData["error"] = "Silahkan Upload File";
                return RedirectToAction("KehadiranLembur");
            }

            if (!Path.GetExtension(batchUsers.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "File Harus berbentuk Xlsx";
                return RedirectToAction("KehadiranLembur");
            }
            if(id_komponen_gaji == 0)
            {
                TempData["error"] = "Jenis Gaji Tidak Boleh Kosong";
                return Redirect("KehadiranLembur");
            }
            List<KehadiranModel> lists = new List<KehadiranModel>();

            using (var stream = new MemoryStream())
            {
                batchUsers.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var npp = (worksheet.Cells[row, 1].Value == null) ? "" : worksheet.Cells[row, 1].Value.ToString().Trim();
                        var tahun = (worksheet.Cells[row, 2].Value == null) ? "" : worksheet.Cells[row, 2].Value.ToString().Trim();
                        var bulan = (worksheet.Cells[row, 3].Value == null) ? "" : worksheet.Cells[row, 3].Value.ToString().Trim();
                        var satuan = (worksheet.Cells[row, 4].Value == null) ? "" : worksheet.Cells[row, 4].Value.ToString().Trim();

                        if (!String.IsNullOrEmpty(npp) && String.IsNullOrEmpty(satuan))
                        {
                            TempData["error"] = "Error! satuan email wajib diisi!";
                            return RedirectToAction("KehadiranLembur");
                        }

                        if (!String.IsNullOrEmpty(npp))
                        {
                            lists.Add(new KehadiranModel
                            {
                                npp = npp,
                                id_tahun = int.Parse(tahun),
                                id_bulan = int.Parse(bulan),
                                jumlah_satuan = float.Parse(satuan),
                                id_komponen_gaji = id_komponen_gaji
                            });
                        }
                    }
                    if (dao.simpanKehadiranLembur(lists))
                    {
                        TempData["success"] = "Berhasil Upload Data!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal Mengupload Data!";
                    }
                }
                return RedirectToAction("KehadiranLembur");
            }

            //return View();
        }
        //View Cuti Panjang
        public IActionResult CutiPanjang(int tahun, string npp, string SearchText = "")
        {
            dynamic objek = new ExpandoObject();
            objek.tahun = tahun;         
            objek.bulan = daoGeneral.getDataBulan();          
            objek.data1 = dao.getDatabyNpp(npp);
            var data = dao.getDataCutiPanjang();

            objek.table = data;
            if (SearchText != "" && SearchText != null)
            {
                objek.table = data.Where(p => p.npp.Contains(SearchText)
                                || p.tgl_awal_cuti.Contains(SearchText)
                                || p.tgl_akhir_cuti.Contains(SearchText));
            }
            return View(objek);
        }

        //Tambah Data Cuti Panjang
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult TambahCutiPanjang(CutiPanjangModel mdl)
        {
            if (dao.simpanCutiPanjang(mdl) == 1)
            {
                TempData["success"] = "Berhasil menambahkan data!";

            }
            else if (dao.simpanCutiPanjang(mdl) == -1)
            {
                TempData["error"] = "Pengajuan Cuti Minimal 6 Tahun Sekali";
            }
            else
            {
                TempData["error"] = "Gagal menambahkan data!";
            }

            return RedirectToAction("CutiPanjang");
        }
        //Ubah Data CutiP Panjang
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UbahCutiPanjang(CutiPanjangModel mdl)
        {

            if (mdl.is_lock == 0)
            {
                dao.ubahCutiPanjang(mdl);
                TempData["success"] = "Berhasil mengubah data!";
            }
            else if (mdl.is_lock == 1)
            {
                TempData["error"] = "Data dilock";
            }


            return RedirectToAction("CutiPanjang");
        }
        //Lock Data CUti Panjang
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LockCutiPanjang(CutiPanjangModel mdl)
        {

            if (dao.lockCutiPanjang(mdl))
            {
                TempData["success"] = "Data Berhasil dilock!";
            }
            else
            {
                TempData["error"] = "Gagal menambahkan data!";
            }


            return RedirectToAction("CutiPanjang");
        }

        public IActionResult SuplesiInsentif(int id_tahun)
        {
            dynamic objek = new ExpandoObject();
            objek.tahun = id_tahun;
            objek.unit = daoGeneral.getDataUnit();
            objek.bulan = daoGeneral.getDataBulan();        
            objek.referensi = dao.getDataReferensi();
          
            return View(objek);

        }
        public JsonResult cariSuplisiInsentif(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji)
        {
            var data = dao.getDataSuplisiInsentif(id_tahun, id_bulan, id_unit, id_komponen_gaji);
            return Json(data);
        }
        //Export data ke Excel
        public IActionResult ExportSuplisiInsentif(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji)
        {
            byte[] result;
            string namafile = "Template_Import_Suplisi_Insentif.xlsx";
            var data = dao.getKaryawanAll(id_tahun, id_bulan, id_unit, id_komponen_gaji);
            if (id_tahun == 0 || id_bulan == 0 || id_unit == 00 || id_komponen_gaji == 0 )
            {
                TempData["error"] = "Data tahun, bulan, unit, dan komponen gaji tidak boleh kosong";
                return Redirect("SuplesiInsentif");
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
        //Import data dari Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BatchUserUpload(IFormFile batchUsers)
        {

            if (batchUsers == null || batchUsers.Length <= 0)
            {
                TempData["error"] = "Silahkan Upload File";
                return RedirectToAction("SuplisiInsentif");
            }

            if (!Path.GetExtension(batchUsers.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "File Harus berbentuk Xlsx";
                return RedirectToAction("SuplisiInsentif");
            }

            List<KomponenVariabelModel> lists = new List<KomponenVariabelModel>();

            using (var stream = new MemoryStream())
            {
                batchUsers.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var unit = (worksheet.Cells[row, 1].Value == null) ? "" : worksheet.Cells[row, 1].Value.ToString().Trim();
                        var tahun = (worksheet.Cells[row, 2].Value == null) ? "" : worksheet.Cells[row, 2].Value.ToString().Trim();
                        var bulan = (worksheet.Cells[row, 3].Value == null) ? "" : worksheet.Cells[row, 3].Value.ToString().Trim();
                        var npp = (worksheet.Cells[row, 4].Value == null) ? "" : worksheet.Cells[row, 4].Value.ToString().Trim();
                        var nama = (worksheet.Cells[row, 5].Value == null) ? "" : worksheet.Cells[row, 5].Value.ToString().Trim();
                        var komponen_gaji = (worksheet.Cells[row, 6].Value == null) ? "" : worksheet.Cells[row, 6].Value.ToString().Trim();
                        var jumlah = (worksheet.Cells[row, 7].Value == null) ? "" : worksheet.Cells[row, 7].Value.ToString().Trim();

                        if (!String.IsNullOrEmpty(npp) && String.IsNullOrEmpty(komponen_gaji))
                        {
                            TempData["error"] = "Error! kolom npp wajib diisi!";
                            return RedirectToAction("Index");
                        }

                        if (!String.IsNullOrEmpty(npp))
                        {
                            lists.Add(new KomponenVariabelModel
                            {
                                id_unit = int.Parse(unit),
                                id_tahun = int.Parse(tahun),
                                id_bulan = int.Parse(bulan),
                                npp = npp,
                                nama = nama,
                                id_komponen_gaji = int.Parse(komponen_gaji),
                                jumlah = float.Parse(jumlah)
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
                return RedirectToAction("SuplesiInsentif");
                //return View("SuplisiInsentif",lists);
            }

            //return View();
        }
        [HttpPost]
        public IActionResult UbahSuplisiInsentif(List<KomponenVariabelModel> mdl)
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
        public IActionResult simpanPermanenSuplisiInsentif(List<KomponenVariabelModel> mdl)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = dao.simpanPermanen(mdl);
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

        // -- Penerimaan Lain - Lain --
        public IActionResult PenerimaanLainLain(string npp, int id_tahun)
        {
            dynamic objek = new ExpandoObject();

            objek.tahun = id_tahun;           
            objek.bulan = daoGeneral.getDataBulan();
            objek.data = daoGeneral.getDatabyNpp(npp);
            var data1 = dao.getDataPenerimaanLainLain();
            objek.table = data1;

            return View(objek);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult TambahPenerimaanLainLain(PenerimaanLainLain mdl)
        {
            if (dao.simpanPenerimaanLainLain(mdl))
            {
                TempData["success"] = "Berhasil menambahkan data!";
            }
            else
            {
                TempData["error"] = "Gagal menambahkan data!";
            }

            return RedirectToAction("PenerimaanLainLain");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UbahPenerimaanLainLain(PenerimaanLainLain mdl)
        {

            if (dao.ubahPenerimaanLainLain(mdl))
            {
                TempData["success"] = "Berhasil mengubah data!";
            }
            else
            {
                TempData["error"] = "Gagal Mengubah Data";
            }

            return RedirectToAction("PenerimaanLainLain");
        }
    }
}

