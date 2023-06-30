using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class RoleDAO
    {
        //Role
        public List<RoleModel> getAllRole()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_ROLE],[DESKRIPSI] AS ROLE,[ID_SISTEM_INFORMASI] FROM [PAYROLL].[siatmax].[REF_ROLE] WHERE ID_ROLE IN (2,3,4,5,6,7)";

                    var data = conn.Query<RoleModel>(query).AsList();

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
        public List<RoleModel> getAllRoleTendik()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT  ROW_NUMBER() OVER (
	                                ORDER BY a.NPP
                                    )id,a.[ID_SISTEM_INFORMASI]
                                    ,a.[ID_ROLE],DESKRIPSI AS ROLE
                                    ,b.[NPP],NAMA_LENGKAP_GELAR AS NAMA
                                    ,[ID_FAKULTAS]
                                     FROM [PAYROLL].[siatmax].[TBL_USER_ROLE] a JOIN [simka].[MST_KARYAWAN] b
                                    ON a.NPP = b.NPP
                                    JOIN [siatmax].[REF_ROLE] c
                                    ON c.ID_ROLE = a.ID_ROLE";

                    var data = conn.Query<RoleModel>(query).AsList();

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
        public int simpanRole(RoleModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {

                    string query = @"IF NOT EXISTS (SELECT NPP FROM [siatmax].[TBL_USER_ROLE] WHERE NPP =@npp and ID_ROLE = @id_role)
                                    INSERT INTO [siatmax].[TBL_USER_ROLE] (ID_SISTEM_INFORMASI,ID_ROLE,NPP)
                                    VALUES (1,@id_role, @npp)";

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
        public int ubahRole(RoleModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    UPDATE [siatmax].[TBL_USER_ROLE] SET ID_ROLE = @role_update  WHERE NPP =@npp AND ID_ROLE =@id_role ";

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
        public int hapusRole(RoleModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DELETE FROM [siatmax].[TBL_USER_ROLE] WHERE NPP =@npp AND ID_ROLE = @id_role";

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
