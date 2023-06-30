using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class PotonganDAO
    {      
        public List<PotonganModel> getDataPotonganTetap()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_KOMPONEN_GAJI]
                                      ,[ID_JNS_KOMPONEN]
                                      ,[ID_JNS_POTONGAN]
                                      ,[KOMPONEN_GAJI_FOXPRO]
                                      ,[KOMPONEN_GAJI]
                                      ,[NO_URUT]
                                      ,[IS_SATUAN]
                                      ,[IS_DELETED]
                                    FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] WHERE ID_JNS_POTONGAN =1;";

                    var data = conn.Query<PotonganModel>(query).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public List<PotonganModel> getDataPotonganVariabel()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_KOMPONEN_GAJI]
                                      ,[ID_JNS_KOMPONEN]
                                      ,[ID_JNS_POTONGAN]
                                      ,[KOMPONEN_GAJI_FOXPRO]
                                      ,[KOMPONEN_GAJI]
                                      ,[NO_URUT]
                                      ,[IS_SATUAN]
                                      ,[IS_DELETED]
                                    FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] WHERE ID_JNS_POTONGAN =2;";

                    var data = conn.Query<PotonganModel>(query).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public List<PotonganVariabel> getKaryawanAll(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji, int id_fungsional)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"IF(@id_unit = 0)
                                    SELECT @id_tahun AS TAHUN,@id_bulan AS BULAN,@id_komponen_gaji AS ID_POTONGAN,
                                    NPP,NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] 
                                    WHERE ID_UNIT iS NOT NULL AND ID_REF_FUNGSIONAL = @id_fungsional AND CURRENT_STATUS ='Aktif'
                                    ELSE
                                    SELECT @id_tahun AS TAHUN,@id_bulan AS BULAN,@id_komponen_gaji AS ID_POTONGAN,
                                    NPP,NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND ID_REF_FUNGSIONAL = @id_fungsional AND CURRENT_STATUS ='Aktif'
                                    ";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit, id_komponen_gaji = id_komponen_gaji, id_fungsional = id_fungsional };
                    var data = conn.Query<PotonganVariabel>(query, param).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public bool simpanData(List<PotonganModel> list)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    DECLARE @id_bulan_gaji int, @id_penggajian int, @nominal_satuan int, @nominal_total int
                                    SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND ID_TAHUN = @id_tahun
                                    IF(@nominal != 0)
                                    INSERT INTO [payroll].[TBL_VAKASI](NPP,ID_BULAN_GAJI,ID_KOMPONEN_GAJI,JUMLAH,DATE_INSERTED) 
                                    VALUES (@npp,@id_bulan_gaji,@id_komponen_gaji,@nominal,GETDATE())
                                    ";

                    var data = conn.Execute(query, list);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public int ubahData(List<PotonganModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @nominal WHERE ID_VAKASI = @id";

                    var data = conn.Execute(query, mdl);

                    return data;
                }
                catch (Exception ex)
                {
                    return 0;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public List<PotonganModel> getDataPotongan(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji, int id_fungsional)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                  IF (@id_unit = 0)
                                  SELECT [ID_VAKASI] AS ID
                                  ,a.[ID_KOMPONEN_GAJI],KOMPONEN_GAJI
                                  ,a.[NPP],NAMA_LENGKAP_GELAR AS NAMA
                                  ,[JUMLAH] AS NOMINAL
                                  ,[DATE_INSERTED]
                                  ,[DESKRIPSI]
                                  ,[IS_PERMANENT]
                                  FROM [PAYROLL].[payroll].[TBL_VAKASI] a JOIN [payroll].[TBL_BULAN_GAJI] b 
                                  ON a.ID_BULAN_GAJI = b.ID_BULAN_GAJI
                                  JOIN [simka].[MST_KARYAWAN] c 
                                  ON a.NPP = c.NPP 
                                  JOIN [payroll].[MST_KOMPONEN_GAJI] d
                                  ON a.ID_KOMPONEN_GAJI = d.ID_KOMPONEN_GAJI
                                  WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan AND a.ID_KOMPONEN_GAJI = @id_komponen_gaji AND ID_REF_FUNGSIONAL =@id_fungsional;   
                                  ELSE IF(@id_unit !=0)
                                  SELECT [ID_VAKASI] AS ID
                                  ,a.[ID_KOMPONEN_GAJI],KOMPONEN_GAJI
                                  ,a.[NPP],NAMA_LENGKAP_GELAR AS NAMA
                                  ,[JUMLAH] AS NOMINAL
                                  ,[DATE_INSERTED]
                                  ,[DESKRIPSI]
                                  ,[IS_PERMANENT]
                                  FROM [PAYROLL].[payroll].[TBL_VAKASI] a JOIN [payroll].[TBL_BULAN_GAJI] b 
                                  ON a.ID_BULAN_GAJI = b.ID_BULAN_GAJI
                                  JOIN [simka].[MST_KARYAWAN] c 
                                  ON a.NPP = c.NPP 
                                  JOIN [payroll].[MST_KOMPONEN_GAJI] d
                                  ON a.ID_KOMPONEN_GAJI = d.ID_KOMPONEN_GAJI
                                  WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan AND ID_UNIT = @id_unit AND a.ID_KOMPONEN_GAJI = @id_komponen_gaji AND ID_REF_FUNGSIONAL =@id_fungsional";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit, id_komponen_gaji = id_komponen_gaji, id_fungsional = id_fungsional };
                    var data = conn.Query<PotonganModel>(query, param).ToList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public PotonganModel getDatabyNpp(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    if (npp == null)
                    {
                        npp = "$";
                    }
                    string query = @"SELECT DISTINCT siatmax.mst_unit.MST_ID_UNIT AS ID_UNIT, MST_UNIT_1.NAMA_UNIT,NAMA_LENGKAP_GELAR AS NAMA,c.NPP
                                    FROM siatmax.mst_unit INNER JOIN
                                    siatmax.mst_unit AS MST_UNIT_1 ON siatmax.mst_unit.MST_ID_UNIT = MST_UNIT_1.ID_UNIT
                                    JOIN [simka].[MST_KARYAWAN] C 
                                    ON siatmax.mst_unit.MST_ID_UNIT = c.ID_UNIT
                                    WHERE (MST_UNIT_1.[LEVEL] <> 0) AND c.NPP  LIKE '%" + npp + "%'"
                                    +"ORDER BY MST_UNIT_1.NAMA_UNIT";

                    var param = new { npp = npp };

                    var data = conn.QueryFirstOrDefault<PotonganModel>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        public List<PotonganModel> getDataPotonganLainLain()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_KOMPONEN_GAJI]
                                      ,[ID_JNS_KOMPONEN]
                                      ,[ID_JNS_POTONGAN]
                                      ,[KOMPONEN_GAJI_FOXPRO]
                                      ,[KOMPONEN_GAJI]
                                      ,[NO_URUT]
                                      ,[IS_SATUAN]
                                      ,[IS_DELETED]
                                    FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] WHERE ID_JNS_POTONGAN =3;";

                    var data = conn.Query<PotonganModel>(query).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
        //public PotonganModel getDatabyNpp(string npp)
        //{
        //    using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //    {
        //        try
        //        {
        //            string query = @"SELECT TOP 1 a.NPP, a.NAMA, b.deskripsi as status_fungsional, nama_unit 
        //                            FROM [simka].[MST_KARYAWAN] a 
        //                            JOIN [simka].[REF_FUNGSIONAL] b 
        //                            ON a.[ID_REF_FUNGSIONAL] = b.[ID_REF_FUNGSIONAL] 
        //                            JOIN [siatmax].[MST_UNIT] c
        //                            ON a.[ID_UNIT] = c.[ID_UNIT]
        //                            where a.npp = @npp";

        //            var param = new { npp = npp };

        //            var data = conn.QueryFirstOrDefault<PotonganModel>(query, param);

        //            return data;
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //        finally
        //        {
        //            conn.Dispose();
        //        }
        //    }
        //}
    }
}
