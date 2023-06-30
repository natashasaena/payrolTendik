using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class RekeningDAO
    {
        //Rekening
        public List<RekeningModel> getAllRekening()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT ROW_NUMBER() OVER(ORDER BY a.NPP ASC) AS ID, [NO_REKENING], a.[NPP] AS NPP,[NAMA_BANK],[STATUS_REKENING]
                                  FROM [PAYROLL].[simka].[MST_REKENING] a JOIN [simka].[MST_KARYAWAN] b
                                  ON a.NPP = b.NPP
                                  WHERE ID_REF_FUNGSIONAL != 1";                              

                    var data = conn.Query<RekeningModel>(query).AsList();

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
        public int simpanRekening(RekeningModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                   
                    string query = @"IF NOT EXISTS (SELECT NPP FROM [simka].[MST_REKENING] WHERE NPP =@npp)
                                    INSERT INTO [simka].[MST_REKENING]([NO_REKENING],[NPP],[NAMA_BANK],[STATUS_REKENING]) VALUES 
                                    (@no_rekening,@npp,@nama_bank,'Aktif')";

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
        public int ubahRekening(RekeningModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [simka].[MST_REKENING] SET NO_REKENING = @no_rekening, NAMA_BANK = @nama_bank WHERE NPP =@npp";

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
        public int hapusRekening(RekeningModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [simka].[MST_REKENING] WHERE NO_REKENING = @no_rekening";

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
        public List<KaryawanModel> getAllKaryawan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT npp, nama, file_ttd as ttd FROM [simka].[MST_KARYAWAN]
WHERE FILE_TTD IS NOT NULL";

                    var data = conn.Query<KaryawanModel>(query).AsList();

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
    }
}
