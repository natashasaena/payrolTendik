using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class AccountDAO
    {
        public UserModel getKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 a.NPP, a.NAMA_LENGKAP_GELAR AS NAMA, 
                                    a.PASSWORD, c.DESKRIPSI
                                    FROM simka.MST_KARYAWAN a
                                    JOIN siatmax.TBL_USER_ROLE b ON a.NPP = b.NPP
                                    JOIN siatmax.REF_ROLE c ON b.ID_ROLE = c.ID_ROLE
                                    WHERE a.NPP = @username";
                    var param = new { username = npp };
                    var data = conn.QueryFirstOrDefault<UserModel>(query, param);

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

        public List<MDLMENU> GetMenuKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT b.*
                                    FROM siatmax.TBL_SISTEM_INFORMASI a
                                    JOIN siatmax.TBL_SI_MENU b ON a.ID_SISTEM_INFORMASI = b.ID_SISTEM_INFORMASI
                                    JOIN siatmax.TBL_SI_SUBMENU c ON b.ID_SI_MENU = c.ID_SI_MENU
                                    JOIN siatmax.TBL_ROLE_SUBMENU d ON d.ID_SI_SUBMENU = c.ID_SI_SUBMENU
                                    JOIN siatmax.REF_ROLE e ON e.ID_ROLE = d.ID_ROLE
                                    JOIN siatmax.TBL_USER_ROLE f ON f.ID_ROLE = e.ID_ROLE
                                    JOIN simka.MST_KARYAWAN g ON g.NPP = f.NPP
                                    WHERE e.DESKRIPSI =@role AND b.ID_SI_MENU !=8";
                    var param = new { role = npp };
                    var data = conn.Query<MDLMENU>(query, param).AsList();

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

        public List<MDLSUBMENU> GetSubMenuKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT SM.* FROM siatmax.TBL_ROLE_SUBMENU RSM
                                JOIN siatmax.REF_ROLE RR ON RR.ID_ROLE = RSM.ID_ROLE
	                            LEFT JOIN siatmax.TBL_SI_SUBMENU SM ON RSM.ID_SI_SUBMENU = SM.ID_SI_SUBMENU
	                            LEFT JOIN siatmax.TBL_SI_MENU M ON M.ID_SI_MENU = SM.ID_SI_MENU
	                            WHERE RR.DESKRIPSI = @role AND SM.ISACTIVE = 1 AND M.ISACTIVE = 1 AND RSM.ID_SI_SUBMENU NOT IN (13,14,15,16,17,18,19,20,22)";
                    var param = new { role = npp };
                    var data = conn.Query<MDLSUBMENU>(query, param).AsList();

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
