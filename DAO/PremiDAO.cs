using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class PremiDAO
    {
        public List<PremiModel> getDataBulan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 12 [ID_BULAN_GAJI],[ID_TAHUN],[BULAN],[IS_ACTIVE],[ID_BULAN]
                                    FROM [PAYROLL].[payroll].[TBL_BULAN_GAJI] 
                                    ORDER BY ID_BULAN_GAJI DESC";

                    var data = conn.Query<PremiModel>(query).AsList();

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
  
        public List<PremiModel> getDataNamaKomponen()
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
                                      ,[JENIS_FUNGSIONAL]
                                      ,[IS_DELETED]
                                  FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] where  ID_KOMPONEN_GAJI IN (28,29);";

                    var data = conn.Query<PremiModel>(query).AsList();

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
        public List<PremiModel> getDataBpjs(int id_unit, int id_fungsional, int id_komponen_gaji)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"IF(@id_unit=0)
                                    SELECT [ID_BULAN_GAJI]
                                    ,a.[NPP], NAMA
                                    ,[ID_MST_TARIF_PAYROLL] 
                                    ,[TOTAL]
                                    ,[JML_POTONGAN]
                                    FROM [PAYROLL].[payroll].[TBL_IURAN_BPJS] a JOIN [simka].[MST_KARYAWAN] b 
                                    ON a.NPP = b.NPP WHERE ID_MST_TARIF_PAYROLL = @id_komponen_gaji AND ID_REF_FUNGSIONAL = @id_fungsional
                                    ELSE
                                    SELECT [ID_BULAN_GAJI]
                                    ,a.[NPP], NAMA
                                    ,[ID_MST_TARIF_PAYROLL] 
                                    ,[TOTAL]
                                    ,[JML_POTONGAN]
                                    FROM [PAYROLL].[payroll].[TBL_IURAN_BPJS] a JOIN [simka].[MST_KARYAWAN] b 
                                    ON a.NPP = b.NPP WHERE ID_UNIT = @id_unit 
                                    AND ID_MST_TARIF_PAYROLL = @id_komponen_gaji AND ID_REF_FUNGSIONAL = @id_fungsional";

                    var param = new {id_unit = id_unit, id_fungsional = id_fungsional, id_komponen_gaji = id_komponen_gaji };
                    var data = conn.Query<PremiModel>(query, param).ToList();

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
        public bool simpanData(List<PremiModel> list)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bln_gaji int, @tahun int, @bulan int
                                    SELECT @tahun = YEAR(GETDATE()),@bulan = MONTH(GETDATE())
                                    SELECT @id_bln_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI]
                                    WHERE ID_TAHUN = @tahun AND ID_BULAN = @bulan

                                    IF(@total !=0) and NOT EXISTS(SELECT NPP FROM [payroll].[TBL_IURAN_BPJS] where NPP =@npp AND ID_MST_TARIF_PAYROLL = @id_komponen_gaji) 
                                    INSERT INTO [payroll].[TBL_IURAN_BPJS] (ID_BULAN_GAJI,NPP, ID_MST_TARIF_PAYROLL,TOTAL,JML_POTONGAN) 
                                    VALUES (@id_bln_gaji,@npp,@id_komponen_gaji,@total,@jml_potongan)
                                    ELSE IF (@total !=0) and EXISTS(SELECT NPP FROM [payroll].[TBL_IURAN_BPJS] where NPP =@npp AND ID_MST_TARIF_PAYROLL = @id_komponen_gaji)
                                    UPDATE [payroll].[TBL_IURAN_BPJS] SET TOTAL = @total, JML_POTONGAN = @jml_potongan WHERE NPP =@npp AND  ID_MST_TARIF_PAYROLL= @id_komponen_gaji";
                              
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
        public int ubahData(List<PremiModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_IURAN_BPJS] SET TOTAL = @total, JML_POTONGAN = @jml_potongan 
                                    WHERE NPP = @npp AND ID_MST_TARIF_PAYROLL = @id_komponen_gaji";

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
        public List<BpjsModel> getKaryawanAll(int id_unit, int id_fungsional, int id_komponen_gaji)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"IF(@id_unit = 0)
                                    SELECT @id_komponen_gaji AS ID_KOMPONEN_GAJI,
                                    NPP,NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] WHERE 
                                    ID_REF_FUNGSIONAL = @id_fungsional AND CURRENT_STATUS ='Aktif'
                                    ELSE
                                    SELECT @id_komponen_gaji AS ID_KOMPONEN_GAJI,
                                    NPP,NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] WHERE 
                                    ID_UNIT = @id_unit AND ID_REF_FUNGSIONAL = @id_fungsional AND CURRENT_STATUS ='Aktif' 
                                    ";

                    var param = new {id_unit = id_unit, id_fungsional = id_fungsional, id_komponen_gaji = id_komponen_gaji };
                    var data = conn.Query<BpjsModel>(query, param).AsList();

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
        // Iuran Avrist
        public List<AvristModel> getAllDataAvrist()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT ROW_NUMBER() OVER(ORDER BY a.NPP ASC) AS ID,a.[NPP],NAMA_LENGKAP_GELAR AS NAMA,FORMAT(AVRIST, 'c2', 'id-ID') AS JUMLAH 
                                    FROM [PAYROLL].[payroll].[TBL_AVRIST] a LEFT JOIN [simka].[MST_KARYAWAN] b ON a.NPP = b.NPP";

                    var data = conn.Query<AvristModel>(query).AsList();

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
        public int simpanIuranAvrist(AvristModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"IF NOT EXISTS (SELECT NPP FROM [payroll].[TBL_AVRIST] WHERE NPP =@npp)
                                    INSERT INTO [payroll].[TBL_AVRIST]([NPP],[AVRIST]) VALUES (@npp,@jumlah)";

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
        public int ubahIuranAvrist(AvristModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_AVRIST] SET AVRIST = @jumlah WHERE NPP = @npp";

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
        public int hapusIuranAvrist(AvristModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [payroll].[TBL_AVRIST] WHERE NPP = @npp";

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
    }
}
