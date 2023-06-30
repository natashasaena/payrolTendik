using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class GeneralDAO
    {
        public List<YearMonthModel> getDataBulan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_BULAN],[BULAN] FROM [PAYROLL].[payroll].[REF_BULAN]C";

                    var data = conn.Query<YearMonthModel>(query).AsList();

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
        public List<UnitModel> getDataUnit()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT siatmax.mst_unit.MST_ID_UNIT AS ID_UNIT, MST_UNIT_1.NAMA_UNIT
                                    FROM siatmax.mst_unit INNER JOIN
                                    siatmax.mst_unit AS MST_UNIT_1 ON siatmax.mst_unit.MST_ID_UNIT = MST_UNIT_1.ID_UNIT
                                    WHERE (MST_UNIT_1.[LEVEL] <> 0)
                                    ORDER BY MST_UNIT_1.NAMA_UNIT";

                    var data = conn.Query<UnitModel>(query).AsList();

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
        public List<FungsionalModel> getDataStatusFungsional()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_REF_FUNGSIONAL] AS Id_Fungsional,[DESKRIPSI] AS STATUS_FUNGSIONAL FROM [simka].[REF_FUNGSIONAL]";

                    var data = conn.Query<FungsionalModel>(query).AsList();

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
        //public UserModel getDatabyNpp(string npp)
        //{
        //    using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //    {
        //        try
        //        {
        //            string query = @"DECLARE @SearchWord VARCHAR(30) ='780'
        //                            SELECT NAMA_LENGKAP_GELAR AS NAMA, NPP FROM [simka].[MST_KARYAWAN]
        //                            WHERE NPP LIKE '%'+@SearchWord+'%'";

        //            var param = new { SearchWord = npp };

        //            var data = conn.QueryFirstOrDefault<UserModel>(query, param);

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
        public GeneralModel getDatabyNpp(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    if (npp == null)
                    {
                        npp = "$";
                    }
                    string query = @"SELECT NAMA_LENGKAP_GELAR AS NAMA, NPP, STATUS_KEPEGAWAIAN, ID_REF_GOLONGAN AS GOLONGAN FROM [simka].[MST_KARYAWAN]
                                   WHERE NPP LIKE '%" + npp+"%'";

                    var param = new { npp = npp };

                    var data = conn.QueryFirstOrDefault<GeneralModel>(query, param);

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
