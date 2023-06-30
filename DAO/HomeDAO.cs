using Dapper;
using System.Data.SqlClient;
using System.Security.Cryptography;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class HomeDAO
    {
        public List<KomponenVariabelModel> getDataBulan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_BULAN],[BULAN] FROM [PAYROLL].[payroll].[REF_BULAN]";

                    var data = conn.Query<KomponenVariabelModel>(query).AsList();

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
        public List<GajiModel> getDataUnit()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT DISTINCT siatmax.mst_unit.MST_ID_UNIT as ID_UNIT, MST_UNIT_1.NAMA_UNIT as NAMA_UNIT
                                    FROM siatmax.mst_unit INNER JOIN
                                    siatmax.mst_unit AS MST_UNIT_1 ON siatmax.mst_unit.MST_ID_UNIT = MST_UNIT_1.ID_UNIT
                                    WHERE (MST_UNIT_1.[LEVEL] <> 0)
                                    ORDER BY MST_UNIT_1.NAMA_UNIT";

                    var data = conn.Query<GajiModel>(query).AsList();

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
        public GajiModel getDataUnitbyParamater(int id_unit)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT ID_UNIT, NAMA_UNIT FROM [siatmax].[MST_UNIT] WHERE ID_UNIT = @id_unit";

                    var param = new { id_unit = id_unit };
                    var data = conn.QueryFirstOrDefault<GajiModel>(query, param);

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
        public List<KomponenGajiModel> getDataID(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @tabel TABLE (id int,deleted bit)
                                    INSERT INTO @tabel
                                    SELECT a.ID_KOMPONEN_GAJI AS ID, a.IS_DELETED AS DELETED FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] a JOIN [payroll].[MST_KOMPONEN_GAJI] b 
                                    ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
                                    WHERE NPP=@npp AND JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan') AND a.IS_DELETED = 0
                                    INSERT INTO @tabel
                                    SELECT a.ID_KOMPONEN_GAJI AS ID, a.IS_DELETED AS DELETED FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] a JOIN [payroll].[MST_KOMPONEN_GAJI] b 
                                    ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
                                    WHERE NPP=@npp AND a.ID_KOMPONEN_GAJI = 41 AND a.IS_DELETED = 0
                                    Insert INTO @tabel
									SELECT a.ID_KOMPONEN_GAJI,a.IS_DELETED FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] a JOIN [payroll].[MST_KOMPONEN_GAJI] b 
                                    ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
                                    WHERE SUBSTRING(NPP, 7, 10) = @npp AND ID_JNS_KOMPONEN =1 AND a.IS_DELETED = 0
                                    SELECT * from  @tabel";

                    var param = new { npp = npp };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        public List<GajiModel> getNppbyUnit(int id_unit)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT NPP FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND CURRENT_STATUS ='Aktif'";

                    var param = new { id_unit = id_unit };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        public KomponenGajiModel getKaryawanbyNpp(string npp, int id_komponen_gaji)
        {

            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 a.*,b.[KOMPONEN_GAJI], c.[JNS_KOMPONEN] FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] a 
                                    JOIN [payroll].[MST_KOMPONEN_GAJI] b 
                                    ON a.[ID_KOMPONEN_GAJI] = b.[ID_KOMPONEN_GAJI] 
                                    JOIN [payroll].[REF_JNS_KOMPONEN_GAJI] c 
                                    ON b.[ID_JNS_KOMPONEN] = c.[ID_JNS_KOMPONEN] WHERE a.npp = @npp AND a.id_komponen_gaji= @id_komponen_gaji;";

                    var param = new { npp = npp, id_komponen_gaji = id_komponen_gaji };

                    var data = conn.QueryFirstOrDefault<KomponenGajiModel>(query, param);

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
        public List<KomponenGajiModel> getPersentaseTunjanganYSR()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [NOMINAL] AS NOMINAL_PERSENTASE FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] WHERE NAMA_TARIF_PAYROLL ='Tunjangan YSR' AND ID_REF_FUNGSIONAL =1";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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
        //public KomponenGajiModel getKaryawanbyNppIdPayroll(string npp, int idPayroll)
        //{
        //    using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //    {
        //        try
        //        {
        //            string query = @"SELECT TOP 1 a.*,a.ID_MST_TARIF_PAYROLL AS idPayroll
        //                                FROM [payroll].[MST_KOMPONEN_GAJI] a 
        //                                WHERE a.NPP = @npp AND a.ID_MST_TARIF_PAYROLL= @idPayroll";

        //            var data = conn.QueryFirstOrDefault<KomponenGajiModel>(query, new { npp = npp, idPayroll = idPayroll });

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
        public bool simpanKaryawan(KomponenGajiModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"INSERT INTO [payroll].[TBL_KOMPONEN_GAJI_KARY]
                                    ([NPP],[ID_KOMPONEN_GAJI],[NOMINAL],[IS_DELETED])
                                    VALUES
                                    (@npp, @id_komponen_gaji, @nominal,0)";

                    var data = conn.Execute(query, mdl);

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
        public int tambahKomponen(KomponenGajiModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    IF EXISTS (SELECT ID_KOMPONEN_GAJI FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI = @id_komponen_gaji)
	                                    UPDATE [payroll].[TBL_KOMPONEN_GAJI_KARY] SET IS_DELETED = 0 WHERE NPP = @npp and ID_KOMPONEN_GAJI = @id_komponen_gaji
                                    ELSE
	                                    INSERT INTO [payroll].[TBL_KOMPONEN_GAJI_KARY] VALUES(@npp,@id_komponen_gaji,@nominal,NULL,0,NULL)
                                    --BEGIN
                                    --DECLARE @uangmakan int
                                    --SELECT @uangmakan = (SELECT NOMINAL FROM [simka].[MST_TARIF_PAYROLL] WHERE ID_KOMPONEN_GAJI = 8)
                                    --IF(@id_komponen_gaji = 8)
                                    --INSERT INTO [payroll].[TBL_KOMPONEN_GAJI_KARY] VALUES(@npp,@id_komponen_gaji,@uangmakan,NULL,0)
                                    --ELSE
                                    --INSERT INTO [payroll].[TBL_KOMPONEN_GAJI_KARY] VALUES(@npp,@id_komponen_gaji,@nominal,NULL,0)
                                    --END

                                    ";

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
        public int ubahKomponen(List<KomponenGajiModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"BEGIN
                                    DECLARE @gaji int, @idKomponenGaji int
                                    SELECT @idKomponenGaji = ID_KOMPONEN_GAJI FROM [payroll].[MST_KOMPONEN_GAJI] WHERE KOMPONEN_GAJI = @komponen_gaji
                                    SELECT @gaji =  NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =@idKomponenGaji
                                    UPDATE [simka].[MST_KARYAWAN] SET MASA_KERJA_GOLONGAN = @masa_kerja_golongan, 
		                                    ID_REF_GOLONGAN_LOKAL = @golongan, TMT_GOLONGAN = @tgl_sk
		                                    WHERE NPP =@npp
                                    UPDATE [payroll].[TBL_KOMPONEN_GAJI_KARY] SET NOMINAL = @nominal WHERE NPP =@npp AND ID_KOMPONEN_GAJI = @idKomponenGaji;
                                    if(@gaji != @nominal)
                                    INSERT INTO [payroll].[HST_KOMPONEN_GAJI_KARY] (NPP,ID_KOMPONEN_GAJI,NOMINAL,JML_TANGGUNGAN,IS_DELETED,NPP_UPDATE,TGL_UPDATE,AKSI) 
	                                    VALUES (@npp,@idKomponenGaji,@gaji,0,1,@npp,GETDATE(),'UPDATE')

                                    END";

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
        public int hapusKomponenTrue(KomponenGajiModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_KOMPONEN_GAJI_KARY] SET IS_DELETED = 1 WHERE NPP = @npp AND ID_KOMPONEN_GAJI = @id_komponen_gaji";

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
        public int hapusKomponenFalse(KomponenGajiModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_KOMPONEN_GAJI_KARY] SET IS_DELETED = 0 WHERE NPP = @npp AND ID_KOMPONEN_GAJI = @id_komponen_gaji";

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

        public List<KomponenGajiModel> getKomponenGajiKaryawan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT top 10 [NPP],a.[ID_KOMPONEN_GAJI],[NOMINAL],[JML_TANGGUNGAN],a.[IS_DELETED],[KOMPONEN_GAJI],[JNS_KOMPONEN]
                                   FROM [PAYROLL].[payroll].[TBL_KOMPONEN_GAJI_KARY] a 
                                   JOIN [payroll].[MST_KOMPONEN_GAJI] b
                                   ON a.[ID_KOMPONEN_GAJI] = b.[ID_KOMPONEN_GAJI] 
                                   JOIN [payroll].[REF_JNS_KOMPONEN_GAJI] c 
                                   ON c.[ID_JNS_KOMPONEN] =b.[ID_JNS_KOMPONEN] 
                                   WHERE a.IS_DELETED = 0;";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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

        // -- Start Komponen Gaji Tenaga Kependidikan --

        public List<KomponenGajiModel> getKomponenGajiTendik(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @tabel table (npp varchar(15),komponen_gaji varchar (50), id_komponen int, nominal varchar (50))
declare  @golongan varchar(10)
SELECT @golongan = ID_REF_GOLONGAN_LOKAL FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp
INSERT INTO @tabel
SELECT @npp,[KOMPONEN_GAJI],a.ID_KOMPONEN_GAJI,COALESCE(FORMAT (b.NOMINAL, 'c2', 'id-ID'),'-') AS NOMINAL
FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] a
JOIN [simka].[MST_TARIF_PAYROLL] b
ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
WHERE JENIS_FUNGSIONAL = 'Kependidikan' AND a.IS_DELETED = 0 AND a.ID_KOMPONEN_GAJI NOT IN ('11') AND ISACTIVE =1 AND ID_REF_GOLONGAN = @golongan
INSERT INTO @tabel
SELECT DISTINCT a.NPP,b.NAMA_TARIF_PAYROLL,b.ID_KOMPONEN_GAJI,COALESCE(FORMAT (13200, 'c2', 'id-ID'),'-') AS NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] a CROSS JOIN [simka].[MST_TARIF_PAYROLL] b
WHERE b.ID_KOMPONEN_GAJI = 11 AND NPP =@npp

SELECT ROW_NUMBER() OVER(ORDER BY ID_KOMPONEN ASC) AS Row, * FROM @tabel

";

                    var param = new { npp = npp };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        // -- End Komponen Gaji Tenaga Kependidikan --   

        // -- Start Komponen Gaji Tenaga Pendidik --

        public List<KomponenGajiModel> getKomponenGajiPendidik(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_jbt_akademik int, @jenjang int
DECLARE @tabel table (npp varchar(20),id_komponen int, komponen_gaji varchar(50),nominal varchar (50))
SELECT @id_jbt_akademik = ID_REF_JBTN_AKADEMIK_LOKAL,@jenjang = ID_REF_JENJANG FROM [simka].[MST_KARYAWAN] a JOIN [simka].[REF_JENJANG] b 
ON DESKRIPSI = PENDIDIKAN_TERAKHIR WHERE NPP = @npp
--SELECT @j = ID_REF_JBTN_AKADEMIK_LOKAL FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp
INSERT INTO @tabel
SELECT  @npp,a.[ID_KOMPONEN_GAJI],[KOMPONEN_GAJI],COALESCE(FORMAT (b.NOMINAL, 'c2', 'id-ID'),'-') AS NOMINAL
FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] a
JOIN [simka].[MST_TARIF_PAYROLL] b
ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
WHERE a.IS_DELETED = 0 AND JENIS_FUNGSIONAL='Pendidik' AND a.ID_KOMPONEN_GAJI NOT IN ('41','65','66','71') AND ISACTIVE = 1
INSERT INTO @tabel
SELECT @npp,b.ID_KOMPONEN_GAJI,KOMPONEN_GAJI,COALESCE(FORMAT (NOMINAL, 'c2', 'id-ID'),'-') FROM [simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
WHERE ID_REF_JBTN_AKADEMIK = @id_jbt_akademik AND ID_REF_JENJANG = @jenjang
INSERT INTO @tabel
SELECT @npp,b.ID_KOMPONEN_GAJI,KOMPONEN_GAJI,COALESCE(FORMAT (NOMINAL, 'c2', 'id-ID'),'-')  FROM [simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
WHERE b.ID_KOMPONEN_GAJI = 66 AND ID_REF_JBTN_AKADEMIK = @id_jbt_akademik 

SELECT ROW_NUMBER() OVER(ORDER BY ID_KOMPONEN ASC) AS Row, * FROM @tabel
";

                    var param = new { npp = npp };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        // -- End Komponen Gaji Tenaga Pendidik --   
        public List<KomponenGajiModel> getData()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"select * from [payroll].[REF_JNS_KOMPONEN_GAJI]";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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

        public List<KomponenGajiModel> getData1()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT * FROM [payroll].[MST_KOMPONEN_GAJI];";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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
        public List<KomponenGajiModel> getIdKomponenbyNpp(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT NPP, ID_KOMPONEN_GAJI FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] where NPP ='01.09.780'";

                    var param = new { npp = npp };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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

        public List<KomponenGajiModel> getKomponenTetap()
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
                                FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] WHERE JENIS_FUNGSIONAL 
                                IN ('Pendidik/Kependidikan','Kependidikan') ORDER BY  JENIS_FUNGSIONAL DESC";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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
        public KomponenGajiModel getDatabyNpp(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 NPP, NAMA_LENGKAP_GELAR AS NAMA,ID_REF_GOLONGAN_LOKAL AS GOLONGAN, MASA_KERJA_GOLONGAN,STATUS_KEPEGAWAIAN,STATUS_SIPIL  FROM [simka].[MST_KARYAWAN] WHERE SUBSTRING(NPP, 7, 10) = @npp";

                    var param = new { npp = npp };

                    var data = conn.QueryFirstOrDefault<KomponenGajiModel>(query, param);

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

        internal List<KomponenGajiModel> getGajiPokok(int masa_kerja_golongan, string npp, string golongan)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    BEGIN
                                    DECLARE @jumlah_keluarga int,@tunjangansuamiistri decimal, @nominal decimal,@suamiistri int,@jumlah_anak int,@tunjangananak decimal,@nominaltunjanak float,
                                    @nominaltunjsuamiistri float,@nominaltunjberas decimal,@tunjangan_yayasan decimal,@kepegawaian_status varchar(20)
                                    DECLARE @umur TABLE (UMUR int)

                                    SELECT @kepegawaian_status= STATUS_KEPEGAWAIAN FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp
                                     INSERT INTO @umur
                                        SELECT (CONVERT(int,CONVERT(char(8),GETDATE(),112))-CONVERT(char(8),TGL_LAHIR,112))/10000 AS UMUR
                                            FROM [simka].[MST_KELUARGA] WHERE ID_REF_KELUARGA IN (3,9,10,11,12) AND NPP =@npp AND IS_TANGGUNG = 1

                                        SELECT @jumlah_anak = (SELECT COUNT(umur) FROM @umur WHERE umur <= 25)

	                                    SELECT @nominaltunjsuamiistri = (SELECT NOMINAL FROM [simka].[MST_TARIF_PAYROLL] WHERE ID_KOMPONEN_GAJI = 2)
	                                    SELECT @nominaltunjanak = (SELECT NOMINAL FROM [simka].[MST_TARIF_PAYROLL] WHERE ID_KOMPONEN_GAJI = 3)
	                                    SELECT @nominaltunjberas = (SELECT NOMINAL FROM [simka].[MST_TARIF_PAYROLL] WHERE ID_KOMPONEN_GAJI = 4)

	                                    SELECT @jumlah_keluarga = (SELECT COUNT(ID_REF_KELUARGA) + 1 AS JUMLAH_KELUARGA
	                                    FROM [PAYROLL].[simka].[MST_KELUARGA] where NPP=@npp AND IS_TANGGUNG =1)

	                                    if(@jumlah_anak > 3)
		                                    SELECT @jumlah_anak = 3

                                    SELECT @nominal = (SELECT TOP 1 [NOMINAL]     
                                      FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a CROSS JOIN [simka].[MST_KARYAWAN] b
                                      WHERE a.ID_REF_GOLONGAN = @golongan AND ID_KOMPONEN_GAJI =1 AND NPP = @npp
                                     ORDER BY ABS(a.MASAKERJA - @masa_kerja_golongan))
                                    IF(@kepegawaian_status = 'Tetap')
	                                BEGIN	
	                                    SELECT @nominal = (SELECT TOP 1 [NOMINAL]     
                                                      FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a CROSS JOIN [simka].[MST_KARYAWAN] b
                                                      WHERE a.ID_REF_GOLONGAN = @golongan AND ID_KOMPONEN_GAJI =1 AND NPP = @npp
                                                      ORDER BY ABS(a.MASAKERJA - @masa_kerja_golongan))

	                                END
	                                ELSE IF(@kepegawaian_status = 'Calon')
	                                BEGIN
	                                    SELECT @nominal = (SELECT TOP 1 [NOMINAL]     
                                                      FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a CROSS JOIN [simka].[MST_KARYAWAN] b
                                                      WHERE a.ID_REF_GOLONGAN = @golongan AND ID_KOMPONEN_GAJI =1 AND NPP = @npp
                                                      ORDER BY ABS(a.MASAKERJA - @masa_kerja_golongan)) * 0.8
	                                END

  
                                    SELECT @suamiistri = (SELECT COUNT(ID_REF_KELUARGA) AS SUAMI_ISTRI
		                                    FROM [PAYROLL].[simka].[MST_KELUARGA] WHERE ID_REF_KELUARGA IN (1,2) AND npp =@npp AND STATUS_SIPIL ='MENIKAH' AND IS_TANGGUNG =1)

                                    SELECT @tunjangan_yayasan = (SELECT TOP 1 [NOMINAL]
				                                      FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
				                                      CROSS JOIN [simka].[MST_KARYAWAN] c
				                                      WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan')
				                                      AND NPP = @npp AND a.ID_KOMPONEN_GAJI = 5 AND MASAKERJA <= @masa_kerja_golongan
				                                      ORDER BY NOMINAL desc)
                                    SELECT NOMINAL = @nominal, TUNJANGAN_SUAMI_ISTRI = @suamiistri * @nominal * @nominaltunjsuamiistri,TUNJANGAN_ANAK = @nominaltunjanak * @nominal * @jumlah_anak,
                                    TUNJANGAN_BERAS = @nominaltunjberas * @jumlah_keluarga,TUNJANGAN_YAYASAN = @tunjangan_yayasan * 0.01 * @nominal,PERSENTASE =@tunjangan_yayasan 

                                    END
                                    ";

                    var param = new { masa_kerja_golongan = masa_kerja_golongan, npp = npp, golongan = golongan };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        internal List<KomponenGajiModel> getTunjanganYsr(int masa_kerja_golongan, string npp, decimal persentase)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                 DECLARE @nominal decimal, @persentase_nominal decimal,@kepegawaian_status varchar(20)
								 SELECT @kepegawaian_status= STATUS_KEPEGAWAIAN FROM [simka].[MST_KARYAWAN] WHERE NPP =  @npp
								 SELECT @nominal =(SELECT [NOMINAL]     
												  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] WHERE NAMA_TARIF_PAYROLL ='Tunjangan YSR' AND ID_REF_FUNGSIONAL =1 AND NOMINAL = @persentase)
								 SELECT @persentase_nominal =(SELECT TOP 1 [NOMINAL]     
												  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a CROSS JOIN [simka].[MST_KARYAWAN] b
												  WHERE a.ID_REF_GOLONGAN = b.ID_REF_GOLONGAN_LOKAL AND ID_KOMPONEN_GAJI =1 AND NPP = @npp
												  ORDER BY ABS(a.MASAKERJA - @masa_kerja_golongan))
								  IF(@kepegawaian_status ='Tetap')							  						
									SELECT TUNJANGAN_YAYASAN = ROUND((@nominal * @persentase_nominal * 0.01),0)								
								 ELSE IF(@kepegawaian_status = 'Calon')							
									SELECT TUNJANGAN_YAYASAN = ROUND((@nominal * @persentase_nominal * 0.01 *0.8),0)
                                ";

                    var param = new { masa_kerja_golongan = masa_kerja_golongan, npp = npp, persentase = persentase };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        //        public List<KomponenGajiModel> cariData(string npp)
        //        {
        //            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //            {
        //                try
        //                {

        //                    string query = @"BEGIN
        //	                                DECLARE @jumlah_anak int,@jumlah_keluarga int, @gaji_pokok numeric (18,3), @tunjangan_fungsional float,@persentase_tunjangan_yayasan float,@tunjangan_struktural numeric (18,3),
        //	                                @suamiistri int, @tunjangan_pengobatan numeric (18,3), @jenjang varchar(15), @tunjangan_kependidikan numeric (18,3),@mk_riil numeric(5,2)
        //                                    DECLARE @umur TABLE (UMUR int)
        //                                    DECLARE @gajipokok_nominal float,@tunjsuamiistri decimal,@tunjanak decimal,@tunjberas decimal, @tunjyayasan decimal,
        //	                                @tunjstruktural decimal,@tunjfungsional decimal, @tunjtransport decimal, @tunjpengobatan decimal,@tunjkependidikan numeric (18,3), @hari int, 
        //                                    @uang_makan decimal,@lembur decimal, @lembur_ekstra decimal, @lembur_libur decimal, @lembur_libur_ekstra decimal

        //                                    INSERT INTO @umur
        //                                    SELECT (CONVERT(int,CONVERT(char(8),GETDATE(),112))-CONVERT(char(8),TGL_LAHIR,112))/10000 AS UMUR
        //                                        FROM [simka].[MST_KELUARGA] WHERE ID_REF_KELUARGA IN (3,9,10,11,12) AND NPP =@npp AND IS_TANGGUNG = 1

        //                                    SELECT @jumlah_anak = (SELECT COUNT(umur) FROM @umur WHERE umur <= 25)
        //	                                --GET JUMLAH ANAK
        //	                                --SELECT @jumlah_anak = (SELECT COUNT(ID_REF_KELUARGA) AS JUMLAH_ANAK
        //	                                --	FROM [PAYROLL].[simka].[MST_KELUARGA] where NPP=@npp AND ID_REF_KELUARGA IN (3,9,10,11,12))
        //	                                --GET DATA SUAMI_ISTRI
        //	                                SELECT @suamiistri = (SELECT COUNT(ID_REF_KELUARGA) AS SUAMI_ISTRI
        //		                                FROM [PAYROLL].[simka].[MST_KELUARGA] WHERE ID_REF_KELUARGA IN (1,2) AND npp =@npp AND STATUS_SIPIL ='MENIKAH' AND IS_TANGGUNG =1)
        //	                                --GET JUMLAH TANGGUNGAN
        //	                                SELECT @jumlah_keluarga = (SELECT COUNT(ID_REF_KELUARGA) + 1 AS JUMLAH_KELUARGA
        //	                                FROM [PAYROLL].[simka].[MST_KELUARGA] where NPP=@npp AND IS_TANGGUNG =1)

        //									--GET JENJANG
        //									SELECT @jenjang = ID_REF_JENJANG FROM [simka].[REF_JENJANG] a JOIN [simka].[MST_KARYAWAN] b ON a.DESKRIPSI = b.PENDIDIKAN_DIAKUI WHERE NPP =@npp
        //	                                --GET NOMINAL GAJI POKOK
        //	                                SELECT @gaji_pokok = (SELECT TOP 1 [NOMINAL]
        //				                                  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
        //				                                  CROSS JOIN [simka].[MST_KARYAWAN] c
        //				                                  WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp AND a.ID_REF_GOLONGAN = c.ID_REF_GOLONGAN_LOKAL
        //				                                  ORDER BY ABS(a.MASAKERJA - c.MASA_KERJA_GOLONGAN))

        //	                                --GET NOMINAL TUNJANGAN FUNGSIONAL
        //	                                SELECT @tunjangan_fungsional = (SELECT [NOMINAL]
        //				                                  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
        //				                                  CROSS JOIN [simka].[MST_KARYAWAN] c
        //				                                  WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp AND a.ID_REF_JBTN_AKADEMIK = c.ID_REF_JBTN_AKADEMIK_LOKAL)

        //	                                --GET NOMIMAL PERSENTASE TUNJANGAN YAYASAN
        //									SELECT @mk_riil= (SELECT CAST((SELECT FORMAT(GETDATE() - TGL_MASUK , 'yy.MM') FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp) AS NUMERIC(5,2)))
        //									IF (@mk_riil = 0)
        //									SET @persentase_tunjangan_yayasan =(SELECT DISTINCT TOP 1 nominal FROM [simka].[MST_TARIF_PAYROLL] where ID_KOMPONEN_GAJI =5 and
        //									MASAKERJA = 0
        //									ORDER BY NOMINAL desc)
        //									ELSE
        //									SET @persentase_tunjangan_yayasan = (SELECT DISTINCT TOP 1 nominal FROM [simka].[MST_TARIF_PAYROLL] where ID_KOMPONEN_GAJI =5 and
        //									MASAKERJA < @mk_riil
        //									ORDER BY NOMINAL desc)

        //	                                --GET NOMINAL TUNJANGAN STRUKTURAL
        //	                                SELECT @tunjangan_struktural = (SELECT TOP 1 NOMINAL
        //				                                  FROM [PAYROLL].[siatmax].[MST_UNIT] a JOIN [simka].[MST_TARIF_PAYROLL] b
        //				                                  ON a.ID_REF_STRUKTURAL = b.ID_REF_STRUKTURAL
        //				                                  WHERE NPP =@npp)

        //                                --GET NOMINAL TUNJANGAN PENGOBATAN
        //                                SELECT @tunjangan_pengobatan = (SELECT [NOMINAL]
        //				                                  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
        //				                                  CROSS JOIN [simka].[MST_KARYAWAN] c
        //				                                  WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp AND a.STATUS_RESTITUSI = c.STATUS_RESTITUSI)

        //								--GET NOMINAL TUNJANGAN KEPENDIDIKAN
        //								SELECT @tunjangan_kependidikan = (SELECT NOMINAL FROM [simka].[MST_TARIF_PAYROLL] a JOIN [simka].[MST_KARYAWAN] b
        //								ON a.ID_REF_JBTN_AKADEMIK = b.ID_REF_JBTN_AKADEMIK
        //								WHERE ID_KOMPONEN_GAJI = 41 and npp =@npp
        //								AND ID_REF_JENJANG = @jenjang)

        //                                --GET HARI
        //                                SELECT @hari = (SELECT COUNT(TANGGAL)
        //                                  FROM [PAYROLL].[dbo].[TBL_PRESENSI_REKAP] 
        //                                  WHERE datepart(MONTH,TANGGAL) =MONTH(GETDATE()) AND datepart(YEAR,TANGGAL) =YEAR(GETDATE()) AND NPP = SUBSTRING(@npp, 7, 15))

        //                                SELECT @gajipokok_nominal = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =1)
        //	                                SELECT @tunjsuamiistri =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =2)
        //	                                SELECT @tunjanak =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =3)
        //	                                SELECT @tunjberas =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =4)
        //	                                SELECT @tunjyayasan =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =5)
        //	                                SELECT @tunjstruktural =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =9)
        //	                                SELECT @tunjfungsional =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =10)
        //	                                SELECT @tunjtransport =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =11)
        //	                                SELECT @tunjpengobatan =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =12)
        //									SELECT @tunjkependidikan =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =41)
        //                                    SELECT @lembur = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =6)
        //	                                SELECT @lembur_libur = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =7)
        //	                                SELECT @uang_makan = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =8)
        //	                                SELECT @lembur_ekstra = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =22)
        //	                                SELECT @lembur_libur_ekstra = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp AND ID_KOMPONEN_GAJI =23)


        //	                                SELECT TOP 1 NPP,c.ID_REF_JBTN_AKADEMIK, c.ID_REF_JBTN_AKADEMIK_LOKAL
        //                                      ,a.[ID_REF_GOLONGAN],c.ID_REF_GOLONGAN_LOKAL AS GOLONGAN, MASA_KERJA_GOLONGAN, MASAKERJA,PERSENTASE = (@persentase_tunjangan_yayasan * 0.01),
        //	                                  JUMLAH_ANAK = @jumlah_anak,JUMLAH_TANGGUNGAN = @jumlah_keluarga, c.STATUS_RESTITUSI,
        //	                                  GAJI_POKOK = @gaji_pokok,
        //	                                  TUNJANGAN_SUAMI_ISTRI = (0.10 * @gaji_pokok * @suamiistri),
        //	                                  TUNJANGAN_ANAK =(0.02 * NOMINAL * @jumlah_anak),
        //	                                  TUNJANGAN_BERAS =(130000 * @jumlah_keluarga),
        //	                                  TUNJANGAN_FUNGSIONAL = @tunjangan_fungsional,
        //	                                  TUNJANGAN_YAYASAN = (@persentase_tunjangan_yayasan * 0.01 * @gaji_pokok),
        //	                                  TUNJANGAN_STRUKTURAL = @tunjangan_struktural,
        //                                    TUNJANGAN_PENGOBATAN = @tunjangan_pengobatan,
        //									TUNJANGAN_KEPENDIDIKAN = @tunjangan_kependidikan
        //                                      ,a.[ID_REF_FUNGSIONAL]
        //                                      ,[ID_REF_JENJANG]
        //                                      ,[MASAKERJA]
        //                                      ,[NAMA_TARIF_PAYROLL] AS KOMPONEN_GAJI
        //                                      ,[NOMINAL]
        //                                      ,[ISACTIVE]
        //                                      ,[JENIS]
        //                                      ,[JENJANG_KELAS]
        //                                      ,[ket1]
        //                                      ,a.[ID_REF_TUNJANGAN_KHUSUS]
        //                                      ,[ID_REF_TUNJANGAN_TA_KP]
        //                                      ,[ID_REF_TUNJANGAN_PASCA]
        //                                      ,a.[ID_KOMPONEN_GAJI],
        //COALESCE(FORMAT (@gajipokok_nominal, 'c2', 'id-ID'),'-') AS GAJI_POKOK_NOMINAL,
        //COALESCE(FORMAT (@tunjsuamiistri, 'c2', 'id-ID'),'-') AS TUNJ_SUAMIISTRI,
        //COALESCE(FORMAT (@tunjanak, 'c2', 'id-ID'),'-') AS TUNJ_ANAK,
        //COALESCE(FORMAT (@tunjberas, 'c2', 'id-ID'),'-') AS TUNJ_BERAS,
        //COALESCE(FORMAT (@tunjyayasan, 'c2', 'id-ID'),'-') AS TUNJ_YAYASAN,		                                
        //COALESCE(FORMAT (@tunjstruktural, 'c2', 'id-ID'),'-') AS TUNJ_STRUKTURAL, 
        //COALESCE(FORMAT (@tunjfungsional, 'c2', 'id-ID'),'-') AS TUNJ_FUNGSIONAL, TUNJ_TRANSPORT = @tunjtransport, 
        //COALESCE(FORMAT (@tunjpengobatan, 'c2', 'id-ID'),'-') AS TUNJ_PENGOBATAN , 
        //COALESCE(FORMAT (@tunjkependidikan, 'c2', 'id-ID'),'-') AS TUNJ_KEPENDIDIKAN,HARI =@hari, 
        //                                LEMBUR = @lembur, LEMBUR_LIBUR = @lembur_libur, LEMBUR_EKSTRA = @lembur_ekstra, LEMBUR_LIBUR_EKSTRA = @lembur_libur_ekstra, UANG_MAKAN = @uang_makan
        //                                  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
        //                                  CROSS JOIN [simka].[MST_KARYAWAN] c
        //                                  WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp AND a.ID_REF_GOLONGAN = c.ID_REF_GOLONGAN_LOKAL
        //                                  ORDER BY ABS(a.MASAKERJA - c.MASA_KERJA_GOLONGAN)
        //                                END
        //                                ";

        //                    var param = new { npp = npp };
        //                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

        //                    return data;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return null;
        //                }
        //                finally
        //                {
        //                    conn.Dispose();
        //                }
        //            }
        //        }
        public List<KomponenGajiModel> cariData(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {

                    string query = @"BEGIN
	DECLARE @jumlah_anak int,@jumlah_keluarga int, @gaji_pokok numeric (18,3), @tunjangan_fungsional float,@persentase_tunjangan_yayasan float,@tunjangan_struktural numeric (18,3),
	@suamiistri int, @tunjangan_pengobatan numeric (18,3), @jenjang varchar(15), @tunjangan_kependidikan numeric (18,3),@mk_riil numeric(5,2), @tunjangan_yayasan numeric (18,3)
    DECLARE @umur TABLE (UMUR int)
    DECLARE @gajipokok_nominal float,@tunjsuamiistri decimal,@tunjanak decimal,@tunjberas decimal, @tunjyayasan decimal,
	@tunjstruktural decimal,@tunjfungsional decimal, @tunjtransport decimal, @tunjpengobatan decimal,@tunjkependidikan numeric (18,3), @hari int, 
    @uang_makan decimal,@lembur decimal, @lembur_ekstra decimal, @lembur_libur decimal, @lembur_libur_ekstra decimal, @npp1 varchar(20),@kepegawaian_status varchar(20)
	SELECT @npp1 = NPP FROM [simka].[MST_KARYAWAN] WHERE SUBSTRING(NPP, 7, 10) = @npp
	SELECT @kepegawaian_status= STATUS_KEPEGAWAIAN FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp1
    INSERT INTO @umur
    SELECT (CONVERT(int,CONVERT(char(8),GETDATE(),112))-CONVERT(char(8),TGL_LAHIR,112))/10000 AS UMUR
        FROM [simka].[MST_KELUARGA] WHERE ID_REF_KELUARGA IN (3,9,10,11,12) AND NPP =@npp1 AND IS_TANGGUNG = 1

    SELECT @jumlah_anak = (SELECT COUNT(umur) FROM @umur WHERE umur <= 25)
	--GET JUMLAH ANAK
	--SELECT @jumlah_anak = (SELECT COUNT(ID_REF_KELUARGA) AS JUMLAH_ANAK
	--	FROM [PAYROLL].[simka].[MST_KELUARGA] where NPP=@npp AND ID_REF_KELUARGA IN (3,9,10,11,12))
	--GET DATA SUAMI_ISTRI
	SELECT @suamiistri = (SELECT COUNT(ID_REF_KELUARGA) AS SUAMI_ISTRI
		FROM [PAYROLL].[simka].[MST_KELUARGA] WHERE ID_REF_KELUARGA IN (1,2) AND npp =@npp1 AND STATUS_SIPIL ='MENIKAH' AND IS_TANGGUNG =1)
	--GET JUMLAH TANGGUNGAN
	SELECT @jumlah_keluarga = (SELECT COUNT(ID_REF_KELUARGA) + 1 AS JUMLAH_KELUARGA
	FROM [PAYROLL].[simka].[MST_KELUARGA] where NPP=@npp1 AND IS_TANGGUNG =1)

	--GET JENJANG
	SELECT @jenjang = ID_REF_JENJANG FROM [simka].[REF_JENJANG] a JOIN [simka].[MST_KARYAWAN] b ON a.DESKRIPSI = b.PENDIDIKAN_DIAKUI WHERE NPP =@npp1

	--GET NOMIMAL PERSENTASE TUNJANGAN YAYASAN
	SELECT @mk_riil= (SELECT CAST((SELECT FORMAT(GETDATE() - TGL_MASUK , 'yy.MM') FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp1) AS NUMERIC(5,2)))
	IF (@mk_riil = 0)
	SET @persentase_tunjangan_yayasan =(SELECT DISTINCT TOP 1 nominal FROM [simka].[MST_TARIF_PAYROLL] where ID_KOMPONEN_GAJI =5 and
	MASAKERJA = 0
	ORDER BY NOMINAL desc)
	ELSE
	SET @persentase_tunjangan_yayasan = (SELECT DISTINCT TOP 1 nominal FROM [simka].[MST_TARIF_PAYROLL] where ID_KOMPONEN_GAJI =5 and
	MASAKERJA < @mk_riil
	ORDER BY NOMINAL desc)

	IF(@kepegawaian_status = 'Tetap')
	BEGIN
	--GET NOMINAL GAJI POKOK
	SELECT @gaji_pokok = (SELECT TOP 1 [NOMINAL]
				    FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
				    CROSS JOIN [simka].[MST_KARYAWAN] c
				    WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp1 AND a.ID_REF_GOLONGAN = c.ID_REF_GOLONGAN_LOKAL
				    ORDER BY ABS(a.MASAKERJA - c.MASA_KERJA_GOLONGAN))

	--GET NOMINAL TUNJANGAN FUNGSIONAL	
	SELECT @tunjangan_fungsional = (SELECT [NOMINAL]
				    FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
				    CROSS JOIN [simka].[MST_KARYAWAN] c
				    WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp1 AND a.ID_REF_JBTN_AKADEMIK = c.ID_REF_JBTN_AKADEMIK_LOKAL)
	SELECT @tunjangan_yayasan = ROUND((@persentase_tunjangan_yayasan* 0.01 * @gaji_pokok),0)
	END
	ELSE IF(@kepegawaian_status = 'Calon')
	BEGIN
	SELECT @gaji_pokok = (SELECT TOP 1 [NOMINAL]
				    FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
				    CROSS JOIN [simka].[MST_KARYAWAN] c
				    WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp1 AND a.ID_REF_GOLONGAN = c.ID_REF_GOLONGAN_LOKAL
				    ORDER BY ABS(a.MASAKERJA - c.MASA_KERJA_GOLONGAN)) * (0.8)
	SET @tunjangan_fungsional = 0
	SELECT @tunjangan_yayasan = ROUND((@persentase_tunjangan_yayasan* 0.01 * @gaji_pokok),0)
	END

	
	--GET NOMINAL TUNJANGAN STRUKTURAL
	SELECT @tunjangan_struktural = (SELECT TOP 1 NOMINAL
				    FROM [PAYROLL].[siatmax].[MST_UNIT] a JOIN [simka].[MST_TARIF_PAYROLL] b
				    ON a.ID_REF_STRUKTURAL = b.ID_REF_STRUKTURAL
				    WHERE NPP =@npp1)

--GET NOMINAL TUNJANGAN PENGOBATAN
SELECT @tunjangan_pengobatan = (SELECT [NOMINAL]
				    FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
				    CROSS JOIN [simka].[MST_KARYAWAN] c
				    WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp1 AND a.STATUS_RESTITUSI = c.STATUS_RESTITUSI)

--GET NOMINAL TUNJANGAN KEPENDIDIKAN
SELECT @tunjangan_kependidikan = (SELECT NOMINAL FROM [simka].[MST_TARIF_PAYROLL] a JOIN [simka].[MST_KARYAWAN] b
ON a.ID_REF_JBTN_AKADEMIK = b.ID_REF_JBTN_AKADEMIK
WHERE ID_KOMPONEN_GAJI = 41 and npp =@npp1
AND ID_REF_JENJANG = @jenjang)

--GET HARI
SELECT @hari = (SELECT COUNT(TANGGAL)
    FROM [PAYROLL].[dbo].[TBL_PRESENSI_REKAP] 
    WHERE datepart(MONTH,TANGGAL) =MONTH(GETDATE()) AND datepart(YEAR,TANGGAL) =YEAR(GETDATE()) AND NPP = SUBSTRING(@npp1, 7, 15))

SELECT @gajipokok_nominal = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =1)
	SELECT @tunjsuamiistri =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =2)
	SELECT @tunjanak =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =3)
	SELECT @tunjberas =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =4)
	SELECT @tunjyayasan =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =5)
	SELECT @tunjstruktural =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =9)
	SELECT @tunjfungsional =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =10)
	SELECT @tunjtransport =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =11)
	SELECT @tunjpengobatan =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =12)
	SELECT @tunjkependidikan =(SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =41)
    SELECT @lembur = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =6)
	SELECT @lembur_libur = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =7)
	SELECT @uang_makan = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =8)
	SELECT @lembur_ekstra = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =22)
	SELECT @lembur_libur_ekstra = (SELECT NOMINAL FROM [payroll].[TBL_KOMPONEN_GAJI_KARY] WHERE NPP = @npp1 AND ID_KOMPONEN_GAJI =23)
	

	SELECT TOP 1 NPP,c.ID_REF_JBTN_AKADEMIK, c.ID_REF_JBTN_AKADEMIK_LOKAL
        ,a.[ID_REF_GOLONGAN],c.ID_REF_GOLONGAN_LOKAL AS GOLONGAN, MASA_KERJA_GOLONGAN, MASAKERJA,PERSENTASE = (@persentase_tunjangan_yayasan * 0.01),
	    JUMLAH_ANAK = @jumlah_anak,JUMLAH_TANGGUNGAN = @jumlah_keluarga, c.STATUS_RESTITUSI,
	    GAJI_POKOK = @gaji_pokok,
	    TUNJANGAN_SUAMI_ISTRI = (0.10 * @gaji_pokok * @suamiistri),
	    TUNJANGAN_ANAK =(0.02 * NOMINAL * @jumlah_anak),
	    TUNJANGAN_BERAS =(130000 * @jumlah_keluarga),
	    TUNJANGAN_FUNGSIONAL = @tunjangan_fungsional,
	    TUNJANGAN_YAYASAN = @tunjangan_yayasan,
	    TUNJANGAN_STRUKTURAL = @tunjangan_struktural,
    TUNJANGAN_PENGOBATAN = @tunjangan_pengobatan,
	TUNJANGAN_KEPENDIDIKAN = @tunjangan_kependidikan
        ,a.[ID_REF_FUNGSIONAL]
        ,[ID_REF_JENJANG]
        ,[MASAKERJA]
        ,[NAMA_TARIF_PAYROLL] AS KOMPONEN_GAJI
        ,[NOMINAL]
        ,[ISACTIVE]
        ,[JENIS]
        ,[JENJANG_KELAS]
        ,[ket1]
        ,a.[ID_REF_TUNJANGAN_KHUSUS]
        ,[ID_REF_TUNJANGAN_TA_KP]
        ,[ID_REF_TUNJANGAN_PASCA]
        ,a.[ID_KOMPONEN_GAJI],
COALESCE(FORMAT (@gajipokok_nominal, 'c2', 'id-ID'),'-') AS GAJI_POKOK_NOMINAL,
COALESCE(FORMAT (@tunjsuamiistri, 'c2', 'id-ID'),'-') AS TUNJ_SUAMIISTRI,
COALESCE(FORMAT (@tunjanak, 'c2', 'id-ID'),'-') AS TUNJ_ANAK,
COALESCE(FORMAT (@tunjberas, 'c2', 'id-ID'),'-') AS TUNJ_BERAS,
COALESCE(FORMAT (@tunjyayasan, 'c2', 'id-ID'),'-') AS TUNJ_YAYASAN,		                                
COALESCE(FORMAT (@tunjstruktural, 'c2', 'id-ID'),'-') AS TUNJ_STRUKTURAL, 
COALESCE(FORMAT (@tunjfungsional, 'c2', 'id-ID'),'-') AS TUNJ_FUNGSIONAL, TUNJ_TRANSPORT = @tunjtransport, 
COALESCE(FORMAT (@tunjpengobatan, 'c2', 'id-ID'),'-') AS TUNJ_PENGOBATAN , 
COALESCE(FORMAT (@tunjkependidikan, 'c2', 'id-ID'),'-') AS TUNJ_KEPENDIDIKAN,HARI =@hari, 
LEMBUR = @lembur, LEMBUR_LIBUR = @lembur_libur, LEMBUR_EKSTRA = @lembur_ekstra, LEMBUR_LIBUR_EKSTRA = @lembur_libur_ekstra, UANG_MAKAN = @uang_makan
    FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI
    CROSS JOIN [simka].[MST_KARYAWAN] c
    WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp1 AND a.ID_REF_GOLONGAN = c.ID_REF_GOLONGAN_LOKAL
    ORDER BY ABS(a.MASAKERJA - c.MASA_KERJA_GOLONGAN)
END
                                ";

                    var param = new { npp = npp };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        public List<KomponenGajiModel> getGolongan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_REF_GOLONGAN] AS GOLONGAN
      ,[DESKRIPSI]
  FROM [PAYROLL].[simka].[REF_GOLONGAN]";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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
        public List<KomponenGajiModel> getDataGajiPokok(string npp, int masa_kerja_golongan, string golongan)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP (1000) [ID_MST_TARIF_PAYROLL],NPP,NAMA_LENGKAP_GELAR AS NAMA
                                      ,a.[ID_REF_JBTN_AKADEMIK], c.ID_REF_JBTN_AKADEMIK
                                      ,[ID_REF_STRUKTURAL]
                                      ,a.[ID_REF_GOLONGAN],c.ID_REF_GOLONGAN AS GOLONGAN, MASA_KERJA_GOLONGAN,STATUS_KEPEGAWAIAN,STATUS_SIPIL
                                      ,a.[ID_REF_FUNGSIONAL]
                                      ,[ID_REF_JENJANG]
                                      ,[MASAKERJA]
                                      ,[NAMA_TARIF_PAYROLL]
                                      ,[NOMINAL]
                                      ,[ISACTIVE]
                                      ,[JENIS]
                                      ,[JENJANG_KELAS]
                                      ,[ket1]
                                      ,a.[ID_REF_TUNJANGAN_KHUSUS]
                                      ,[ID_REF_TUNJANGAN_TA_KP]
                                      ,[ID_REF_TUNJANGAN_PASCA]
                                      ,a.[ID_KOMPONEN_GAJI]
                                  FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] a JOIN [payroll].[MST_KOMPONEN_GAJI] b ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
                                  CROSS JOIN [simka].[MST_KARYAWAN] c  
                                  WHERE JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan','Kependidikan') AND NPP = @npp AND a.ID_REF_GOLONGAN = @golongan AND a.MASAKERJA = @masa_kerja_golongan
                                 ";

                    var param = new { npp = npp, masa_kerja_golongan = masa_kerja_golongan, golongan = golongan };
                    var data = conn.Query<KomponenGajiModel>(query, param).ToList();

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
        public List<GajiModel> getDataGajiUnit(int id_tahun, int id_bulan, int id_unit)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"--DECLARE @id_tahun int = 2022,@id_bulan int = 11, @id_unit int = 14
DECLARE @id_bulan_gaji int

--get id_bulan_gaji
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan

SELECT DISTINCT b.ID_PENGGAJIAN AS ID,b.NPP AS NPP,b.NAMA AS NAMA, MASA_KERJA_GOL,COALESCE(FORMAT (gp.NOMINAL, 'c', 'id-ID'),'-') AS GAJI_POKOK,COALESCE(FORMAT (tsi.NOMINAL, 'c', 'id-ID'),'-') AS TUNJANGAN_SUAMI_ISTRI,COALESCE(FORMAT (ta.NOMINAL, 'c', 'id-ID'),'-') AS TUNJANGAN_ANAK
,COALESCE(FORMAT (tb.NOMINAL, 'c', 'id-ID'),'-') AS tunjangan_beras, COALESCE(FORMAT (ty.NOMINAL, 'c', 'id-ID'),'-') AS tunjangan_yayasan,
COALESCE(FORMAT (ts.NOMINAL, 'c', 'id-ID'),'-') AS tunjangan_struktural, COALESCE(FORMAT (tf.NOMINAL, 'c', 'id-ID'),'-') AS tunjangan_fungsional,COALESCE(FORMAT (tp.NOMINAL, 'c', 'id-ID'),'-') AS tunjangan_pengobatan,
IS_PERMANENT
FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN
LEFT JOIN [simka].[MST_KARYAWAN] c
ON b.NPP = c.NPP
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 1
)gp
ON b.ID_PENGGAJIAN = gp.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 2
)tsi
ON b.ID_PENGGAJIAN = tsi.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 3
)ta
ON b.ID_PENGGAJIAN = ta.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 4
)tb
ON b.ID_PENGGAJIAN = tb.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 5
)ty
ON b.ID_PENGGAJIAN = ty.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 9
)ts
ON b.ID_PENGGAJIAN = ts.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 10
)tf
ON b.ID_PENGGAJIAN = tf.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 12
)tp
ON b.ID_PENGGAJIAN = tp.ID_PENGGAJIAN
WHERE (c.ID_UNIT = @id_unit OR c.MST_ID_UNIT = @id_unit) AND CURRENT_STATUS ='Aktif' AND ID_BULAN_GAJI = @id_bulan_gaji ";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        public List<GajiModel> insertGajiUnit(int id_tahun, int id_bulan, int id_unit, int id_tunj)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @nppp varchar(15)
                                    DECLARE karyawan_cursor CURSOR
                                    FOR SELECT 
	                                    NPP FROM [simka].[MST_KARYAWAN]
	                                    WHERE CURRENT_STATUS ='Aktif' and (ID_UNIT = @id_unit)

                                    OPEN karyawan_cursor;
                                    FETCH NEXT FROM karyawan_cursor INTO 
	                                     @nppp;
                                    WHILE @@FETCH_STATUS = 0
                                        BEGIN
		                                    exec [dbo].[InsertPenggajianUnit]
			                                    @npp =@nppp,
			                                    @bulan = @id_bulan,
			                                    @tahun = @id_tahun
           
		                                    exec [dbo].[HitungGaji_1]
			                                    @npp =@nppp,
			                                    @bulan = @id_bulan,
			                                    @tahun = @id_tahun,
                                                @id = @id_tunj
		                                    FETCH NEXT FROM karyawan_cursor INTO 
                                                @nppp;
                                        END;

                                    CLOSE karyawan_cursor;
                                    DEALLOCATE karyawan_cursor;";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit, id_tunj = id_tunj };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        public int hapusGajiUnit(List<GajiModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DELETE [payroll].[DTL_PENGGAJIAN] FROM [payroll].[DTL_PENGGAJIAN] a 
                                    JOIN [payroll].[TBL_PENGGAJIAN] b
                                    ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN 
                                    WHERE a.ID_PENGGAJIAN = @id AND IS_PERMANENT <> 1 

                                    DELETE [payroll].[TBL_PENGGAJIAN] 
                                    WHERE ID_PENGGAJIAN = @id AND IS_PERMANENT <> 1 ";


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
        public int simpanPermanenGaji(List<GajiModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"declare @status int
SELECT @status = IS_PERMANENT FROM [payroll].[TBL_PENGGAJIAN] WHERE ID_PENGGAJIAN = @id
IF(@status = 0)
UPDATE [payroll].[TBL_PENGGAJIAN] SET IS_PERMANENT = 1 WHERE ID_PENGGAJIAN = @id
ELSE IF(@status =1)
UPDATE [payroll].[TBL_PENGGAJIAN] SET IS_PERMANENT = 0 WHERE ID_PENGGAJIAN = @id
ELSE
UPDATE [payroll].[TBL_PENGGAJIAN] SET IS_PERMANENT = 1 WHERE ID_PENGGAJIAN = @id";


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
        public int simpanPermanenGajiUnit(List<GajiModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
UPDATE [payroll].[TBL_PENGGAJIAN] SET IS_PERMANENT = 1 WHERE ID_PENGGAJIAN = @id";


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
        public GajiModel getDataGajiPerKaryawan(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"--DECLARE @id_tahun int = 2022,@id_bulan int = 11, @id_unit int = 14
DECLARE @id_bulan_gaji int,@bulan varchar(20)

--get id_bulan_gaji
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
SELECT @bulan = BULAN FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan
SELECT DISTINCT BULAN = @bulan,b.ID_PENGGAJIAN AS ID,b.NPP AS NPP,b.NAMA AS NAMA,NAMA_UNIT,GETDATE() AS TGL_CETAK,
b.STATUS_KEPEGAWAIAN,MASA_KERJA_RIIL,JBT_STRUKTURAL,JBT_AKADEMIK AS JBT_FUNGSIONAL,b.NPWP,NO_TABUNGAN,GOLONGAN
,PANGKAT,MASA_KERJA_GOL,COALESCE(FORMAT (gp.NOMINAL, 'c2', 'id-ID'),'-') AS GAJI_POKOK,COALESCE(FORMAT (tsi.NOMINAL, 'c2', 'id-ID'),'-') AS TUNJANGAN_SUAMI_ISTRI,COALESCE(FORMAT (ta.NOMINAL, 'c2', 'id-ID'),'-') AS TUNJANGAN_ANAK
,COALESCE(FORMAT (tb.NOMINAL, 'c2', 'id-ID'),'-') AS tunjangan_beras, COALESCE(FORMAT (ty.NOMINAL, 'c2', 'id-ID'),'-') AS tunjangan_yayasan,
COALESCE(FORMAT (ts.NOMINAL, 'c2', 'id-ID'),'-') AS tunjangan_struktural, COALESCE(FORMAT (tf.NOMINAL, 'c2', 'id-ID'),'-') AS tunjangan_fungsional,COALESCE(FORMAT (tp.NOMINAL, 'c2', 'id-ID'),'-') AS tunjangan_pengobatan,
IS_PERMANENT
FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN
LEFT JOIN [simka].[MST_KARYAWAN] c
ON b.NPP = c.NPP
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 1
)gp
ON b.ID_PENGGAJIAN = gp.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 2
)tsi
ON b.ID_PENGGAJIAN = tsi.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 3
)ta
ON b.ID_PENGGAJIAN = ta.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 4
)tb
ON b.ID_PENGGAJIAN = tb.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 5
)ty
ON b.ID_PENGGAJIAN = ty.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 9
)ts
ON b.ID_PENGGAJIAN = ts.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 10
)tf
ON b.ID_PENGGAJIAN = tf.ID_PENGGAJIAN
LEFT JOIN (
SELECT ID_PENGGAJIAN,NOMINAL FROM [payroll].[DTL_PENGGAJIAN] WHERE ID_KOMPONEN_GAJI = 12
)tp
ON b.ID_PENGGAJIAN = tp.ID_PENGGAJIAN
LEFT JOIN [siatmax].[TBL_USER_ROLE] d
ON b.NPP = c.NPP
LEFT JOIN [siatmax].[MST_UNIT] e
ON c.ID_UNIT = e.ID_UNIT
WHERE  CURRENT_STATUS ='Aktif' AND ID_BULAN_GAJI = @id_bulan_gaji AND b.NPP = @npp";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.QueryFirstOrDefault<GajiModel>(query, param);

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
        public GajiModel getOneKaryawan(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int,@penerimaan_kotor numeric(18,3),@bulan varchar(15)
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
SELECT @bulan = BULAN FROM [payroll].[TBL_BULAN_GAJI] where ID_BULAN =@id_bulan
SELECT @penerimaan_kotor = (SELECT SUM(NOMINAL) FROM [payroll].[DTL_PENGGAJIAN] a 
	  JOIN [payroll].[TBL_PENGGAJIAN] b ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN 
	  JOIN [payroll].[MST_KOMPONEN_GAJI] c ON a.ID_KOMPONEN_GAJI = c.ID_KOMPONEN_GAJI WHERE NPP=@npp AND JENIS_FUNGSIONAL IN ('Pendidik/Kependidikan'))
SELECT TOP 1 [ID_PENGGAJIAN],BULAN = @bulan
      ,a.[NPP],GETDATE() as tgl_cetak
      ,[ID_BULAN_GAJI]
      ,a.[NAMA]
      ,a.[STATUS_KEPEGAWAIAN]
      ,[MASA_KERJA_RIIL]
      ,[MASA_KERJA_GOL]
      ,[JBT_STRUKTURAL]
      ,[JBT_AKADEMIK] 
      ,[JBT_FUNGSIONAL]
      ,[PANGKAT]
      ,[GOLONGAN]
      ,[JENJANG]
      ,[NO_TABUNGAN]
      ,a.[NPWP]
      ,[IS_PERMANENT],NAMA_UNIT,COALESCE(FORMAT (@penerimaan_kotor, 'c2', 'id-ID'),'-') AS PENERIMAAN_KOTOR
  FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] a JOIN [simka].[MST_KARYAWAN] b
  ON a.NPP = b.NPP
  JOIN [siatmax].[MST_UNIT] c
  ON b.ID_UNIT =  c.ID_UNIT
  WHERE a.NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.QueryFirstOrDefault<GajiModel>(query, param);

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
        public GajiModel getDataKepalaKantor()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT a.NPP, NAMA_LENGKAP_GELAR AS NAMA, file_ttd as ttd, NAMA_UNIT FROM [simka].[MST_KARYAWAN] a JOIN [siatmax].[MST_UNIT] b
                                    ON a.NPP = b.NPP
                                    WHERE NAMA_UNIT ='Rektor'";

                    var param = new { };
                    var data = conn.QueryFirstOrDefault<GajiModel>(query);

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
        //        public List<GajiModel> getGajiKomponenGaji(int id_tahun, int id_bulan,string npp)
        //        {
        //            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //            {
        //                try
        //                {
        //                    string query = @"DECLARE @id_bulan_gaji int,@bulan varchar(15)
        //SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
        //SELECT a.[ID_PENGGAJIAN]
        //      ,a.[ID_KOMPONEN_GAJI],KOMPONEN_GAJI
        //      ,[JUMLAH_SATUAN]
        //      ,COALESCE(FORMAT (NOMINAL, 'c2', 'id-ID'),'-') AS NOMINAL
        //  FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
        //  ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN
        //  JOIN [payroll].[MST_KOMPONEN_GAJI] c
        //  ON a.ID_KOMPONEN_GAJI = c.ID_KOMPONEN_GAJI
        //  WHERE b.NPP =@npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_JNS_KOMPONEN = 1";

        //                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp=npp };
        //                    var data = conn.Query<GajiModel>(query, param).ToList();

        //                    return data;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return null;
        //                }
        //                finally
        //                {
        //                    conn.Dispose();
        //                }
        //            }
        //        }
        public List<GajiModel> getGajiKomponenGaji(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int 
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND ID_TAHUN = @id_tahun
SELECT b.[ID_KOMPONEN_GAJI]   
      ,[KOMPONEN_GAJI],COALESCE(FORMAT (NOMINAL, 'c2', 'id-ID'),'-') AS NOMINAL, JUMLAH_SATUAN
  FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] a JOIN [payroll].[DTL_PENGGAJIAN] b 
  ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
  JOIN [payroll].[TBL_PENGGAJIAN] c 
  ON b.ID_PENGGAJIAN = c.ID_PENGGAJIAN
  WHERE NPP = @npp AND a.IS_DELETED = 0 AND ID_JNS_KOMPONEN = 1 AND ID_BULAN_GAJI = @id_bulan_gaji";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        //        public List<GajiModel> getGajiKomponenPotongan(int id_tahun, int id_bulan, string npp)
        //        {
        //            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //            {
        //                try
        //                {
        //                    string query = @"DECLARE @id_bulan_gaji int,@bulan varchar(15)
        //SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = 2022 AND ID_BULAN = 11
        //SELECT a.[ID_PENGGAJIAN]
        //      ,a.[ID_KOMPONEN_GAJI],KOMPONEN_GAJI
        //      ,[JUMLAH_SATUAN]
        //      ,COALESCE(FORMAT (NOMINAL, 'c2', 'id-ID'),'-') AS NOMINAL
        //  FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
        //  ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN
        //  JOIN [payroll].[MST_KOMPONEN_GAJI] c
        //  ON a.ID_KOMPONEN_GAJI = c.ID_KOMPONEN_GAJI
        //  WHERE b.NPP =@npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_JNS_KOMPONEN = 2";

        //                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
        //                    var data = conn.Query<GajiModel>(query, param).ToList();

        //                    return data;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return null;
        //                }
        //                finally
        //                {
        //                    conn.Dispose();
        //                }
        //            }
        //        }

        public List<GajiModel> getGajiKomponenPotongan(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int 
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND ID_TAHUN = @id_tahun
SELECT b.[ID_KOMPONEN_GAJI]   
      ,[KOMPONEN_GAJI], COALESCE(FORMAT (NOMINAL, 'c2', 'id-ID'),'-') AS NOMINAL
  FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] a JOIN [payroll].[DTL_PENGGAJIAN] b 
  ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
  JOIN [payroll].[TBL_PENGGAJIAN] c 
  ON b.ID_PENGGAJIAN = c.ID_PENGGAJIAN
  WHERE NPP = @npp AND a.IS_DELETED = 0 AND ID_JNS_KOMPONEN = 2 AND ID_BULAN_GAJI = @id_bulan_gaji";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        //        public List<GajiModel> getPerhitunganPajakPenghasilan(int id_tahun, int id_bulan, string npp)
        //        {
        //            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
        //            {
        //                try
        //                {
        //                    string query = @"DECLARE @id_bulan_gaji int 
        //SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND ID_TAHUN = @id_tahun
        //SELECT b.[ID_KOMPONEN_GAJI]   
        //      ,[KOMPONEN_GAJI], FORMAT (NOMINAL, 'c2', 'id-ID') AS NOMINAL
        //  FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] a left JOIN [payroll].[DTL_PENGGAJIAN] b 
        //  ON a.ID_KOMPONEN_GAJI = b.ID_KOMPONEN_GAJI 
        //  JOIN [payroll].[TBL_PENGGAJIAN] c 
        //  ON b.ID_PENGGAJIAN = c.ID_PENGGAJIAN
        //  WHERE NPP = @npp AND a.IS_DELETED = 0 AND ID_JNS_KOMPONEN = 3 AND ID_BULAN_GAJI = @id_bulan_gaji ";

        //                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
        //                    var data = conn.Query<GajiModel>(query, param).ToList();

        //                    return data;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return null;
        //                }
        //                finally
        //                {
        //                    conn.Dispose();
        //                }
        //            }
        //        }
        public GajiModel getPerhitunganPajakPenghasilan(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int, @total_potongan numeric(18,3), @penerimaan_kotor numeric(18,3), @penerimaan_bersih numeric(18,3)
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND ID_TAHUN = @id_tahun
SELECT @penerimaan_kotor = NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 25 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji
SELECT @total_potongan = SUM(NOMINAL) FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN JOIN [payroll].[MST_KOMPONEN_GAJI] c ON a.ID_KOMPONEN_GAJI = c.ID_KOMPONEN_GAJI
WHERE NPP =@npp AND ID_JNS_KOMPONEN = 2 AND ID_BULAN_GAJI = @id_bulan_gaji
SELECT @penerimaan_bersih = @penerimaan_kotor - @total_potongan
SELECT FORMAT(@penerimaan_kotor, 'c2', 'id-ID') AS PENERIMAAN_KOTOR,
FORMAT(@total_potongan, 'c2', 'id-ID') AS TOTAL_POTONGAN, FORMAT(@penerimaan_bersih, 'c2', 'id-ID') AS PENERIMAAN_BERSIH,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 26 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) HONOR_DOP,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 28 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PREMI_ASTEK,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 29 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PREMI_BPJS_KESEHATAN,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 30 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) AVRIST,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 32 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) BIAYA_JABATAN,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 33 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) BIAYA_YADAPEN,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 15 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) BPJS_KETENAGAKERJAAN,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 19 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) BPJS_KESEHATAN,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 36 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PDP_TIDAK_KENA_PAJAK,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 37 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PDP_KENA_PAJAK,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 38 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PAJAK_SEHARUSNYA,
(SELECT FORMAT(CAST(ROUND(NOMINAL, 0) AS NUMERIC(18,2)) , 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 39 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PAJAK_POTONG,
(SELECT FORMAT(NOMINAL, 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a JOIN [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN WHERE ID_KOMPONEN_GAJI = 40 and npp =@npp and ID_BULAN_GAJI = @id_bulan_gaji) PENYESUAIAN_PAJAK,
(SELECT FORMAT(SUM(NOMINAL), 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a join [payroll].[TBL_PENGGAJIAN] b
ON a.ID_PENGGAJIAN = b.ID_PENGGAJIAN join [payroll].[MST_KOMPONEN_GAJI] c on a.ID_KOMPONEN_GAJI = c.ID_KOMPONEN_GAJI where npp=@npp and ID_BULAN_GAJI = @id_bulan_gaji 
and a.ID_KOMPONEN_GAJI IN (25,26,28,29,30)) TOTAL_PENGHASILAN,
(SELECT FORMAT(SUM(NOMINAL), 'c2', 'id-ID') AS NOMINAL FROM [payroll].[DTL_PENGGAJIAN] a join [payroll].[TBL_PENGGAJIAN] b
On a.ID_PENGGAJIAN = b.ID_PENGGAJIAN join [payroll].[MST_KOMPONEN_GAJI] c on a.ID_KOMPONEN_GAJI = c.ID_KOMPONEN_GAJI where npp=@npp and ID_BULAN_GAJI = @id_bulan_gaji 
and a.ID_KOMPONEN_GAJI IN (32,33,15,19,36)) TOTAL_PENGURANGAN



";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.QueryFirstOrDefault<GajiModel>(query, param);

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
        public GajiModel getKaryawanHonor(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int,@penerimaan_kotor numeric(18,3),@bulan varchar(15), @pb numeric(18,3)
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
SELECT @bulan = BULAN FROM [payroll].[TBL_BULAN_GAJI] where ID_BULAN =@id_bulan
SELECT @pb = ((SELECT SUM(JUMLAH) FROM [payroll].[TR_HONORARIUM] WHERE NPP =@npp and ID_BULAN_GAJI = @id_bulan_gaji and ID_REF_DESKRIPSI_HON =1)-(SELECT SUM(PAJAK) FROM [payroll].[TR_HONORARIUM] WHERE NPP =@npp and ID_BULAN_GAJI = @id_bulan_gaji and ID_REF_DESKRIPSI_HON =1
)) 
SELECT TOP 1 BULAN = @bulan, a.NPP, NAMA_LENGKAP_GELAR AS NAMA,NAMA_UNIT,NO_REKENING AS NO_TABUNGAN,
(SELECT  FORMAT(SUM(PAJAK), 'c2', 'id-ID') FROM [payroll].[TR_HONORARIUM] WHERE NPP =@npp and ID_BULAN_GAJI = @id_bulan_gaji and ID_REF_DESKRIPSI_HON =1
) PAJAK,(SELECT FORMAT(SUM(JUMLAH), 'c2', 'id-ID') FROM [payroll].[TR_HONORARIUM] WHERE NPP =@npp and ID_BULAN_GAJI = @id_bulan_gaji and ID_REF_DESKRIPSI_HON =1
) PENERIMAAN_KOTOR,FORMAT(@pb, 'c2', 'id-ID') AS PENERIMAAN_BERSIH FROM [payroll].[TR_HONORARIUM] a JOIN [simka].[MST_KARYAWAN] b
ON a.NPP = b.NPP
JOIN [siatmax].[MST_UNIT] c
ON b.ID_UNIT = c.ID_UNIT
JOIN [simka].[MST_REKENING] d
ON c.NPP = d.NPP
WHERE ID_BULAN_GAJI = @id_bulan_gaji AND a.NPP =@npp";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.QueryFirstOrDefault<GajiModel>(query, param);

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
        public List<GajiModel> getHonorarium(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
SELECT ROW_NUMBER() OVER(ORDER BY jumlah desc) AS num,KETERANGAN,COALESCE(FORMAT (JUMLAH, 'c2', 'id-ID'),'-') AS JUMLAH 
FROM [payroll].[TR_HONORARIUM] WHERE NPP =@npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_REF_DESKRIPSI_HON = 1
";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        // -- Start Potongan Koperasi --
        public List<GajiModel> getPotonganKoperasi(int id_tahun, int id_bulan, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @keterangan varchar(50) --@npp varchar(15) ='01.09.780',@id_tahun int = 2022, @id_bulan int = 11
DECLARE @tabel table (id int,ket varchar(50))

INSERT INTO @tabel
SELECT ID_POT_KOPERASI,CONCAT(ANGSUR_KE,'/',JML_ANGSUR,' ',KETERANGAN) FROM [payroll].[TBL_POTONGAN_KOPERASI] WHERE JML_ANGSUR !=0 AND ANGSUR_KE !=0 
AND TAHUN_ANGGARAN = @id_tahun AND BULAN = @id_bulan AND NPP =@npp
INSERT INTO @tabel
SELECT ID_POT_KOPERASI,KETERANGAN  FROM [payroll].[TBL_POTONGAN_KOPERASI] WHERE JML_ANGSUR =0 AND ANGSUR_KE =0 
AND TAHUN_ANGGARAN = @id_tahun AND BULAN = @id_bulan AND NPP =@npp

SELECT [ID_POT_KOPERASI]
      ,a.[NPP],NAMA_LENGKAP_GELAR AS NAMA
      ,[TAHUN_ANGGARAN]
      ,[BULAN]
      ,[JML_ANGSUR]
      ,[ANGSUR_KE]
      ,COALESCE(FORMAT (JUMLAH, 'c2', 'id-ID'),'0') AS JUMLAH 
      ,ket AS KETERANGAN
      ,[IS_APPROVED]
      ,[IS_TRANSFER]
  FROM [PAYROLL].[payroll].[TBL_POTONGAN_KOPERASI] a
  JOIN @tabel b ON a.ID_POT_KOPERASI = b.id
JOIN [simka].[MST_KARYAWAN] c
  ON a.NPP = c.NPP
  WHERE TAHUN_ANGGARAN = @id_tahun AND BULAN = @id_bulan AND a.NPP =@npp

";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, npp = npp };
                    var data = conn.Query<GajiModel>(query, param).ToList();

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
        // -- End Potongan Koperasi --

        // -- Start Dashboard --
        public List<KomponenGajiModel> getDataKaryawanPensiun()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @nppp varchar(15)
DECLARE @tabel table (npp varchar(15),nama varchar(100),purnakarya varchar(50),golongan varchar(20))
DECLARE karyawan_cursor CURSOR
FOR SELECT 
	NPP FROM [simka].[MST_KARYAWAN]
	WHERE CURRENT_STATUS ='Aktif' AND STATUS_KEPEGAWAIAN ='Tetap' 
OPEN karyawan_cursor;
FETCH NEXT FROM karyawan_cursor INTO 
	 @nppp;
WHILE @@FETCH_STATUS = 0
    BEGIN
		declare @yearNow int, @monthNow int,@yearPensiun int,@monthPensiun int,@gol_before varchar(4)
		select @yearNow = YEAR(getdate());
		select @monthNow = MONTH(getdate());
		select @yearPensiun = YEAR(TMT_PURNAKARYA) FROM [simka].[MST_KARYAWAN] WHERE npp=@nppp;
		select @monthPensiun = MONTH(TMT_PURNAKARYA) FROM [simka].[MST_KARYAWAN] WHERE npp=@nppp;
		select @gol_before = ID_REF_GOLONGAN FROM [simka].[MST_KARYAWAN] WHERE NPP = @nppp

		IF (SELECT ID_REF_GOLONGAN_LOKAL FROM [simka].[MST_KARYAWAN] where npp = @nppp ) = @gol_before
	
		IF @yearPensiun = @yearNow 
		insert into @tabel
		select npp= @nppp,NAMA_LENGKAP_GELAR,TMT_PURNAKARYA,ID_REF_GOLONGAN_LOKAL from [simka].[MST_KARYAWAN] where npp = @nppp
		

		ELSE
		IF @yearPensiun = @yearNow + 1 and @monthPensiun <= @monthNow +2
		insert into @tabel
		select npp= @nppp,NAMA,TMT_PURNAKARYA,ID_REF_GOLONGAN_LOKAL from [simka].[MST_KARYAWAN] where npp = @nppp	
		FETCH NEXT FROM karyawan_cursor INTO 
            @nppp;
    END;

CLOSE karyawan_cursor;
DEALLOCATE karyawan_cursor;
select * from @tabel

";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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
        public List<KomponenGajiModel> getReminderGolongan()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @nppp varchar(15)
DECLARE @tabel table (npp varchar(15),nama varchar(100),tmt_golongan varchar(12),golongan varchar(20),keterangan varchar(15))
DECLARE karyawan_cursor CURSOR
FOR SELECT 
	NPP FROM [simka].[MST_KARYAWAN]
	WHERE CURRENT_STATUS ='Aktif' AND STATUS_KEPEGAWAIAN ='Tetap' 
OPEN karyawan_cursor;
FETCH NEXT FROM karyawan_cursor INTO 
	 @nppp;
WHILE @@FETCH_STATUS = 0
    BEGIN
		declare @reminder_sk date, @reminder_kgb date,@ket varchar(15),@mk int
		select @reminder_sk = DATEADD(mm, 45, TMT_GOLONGAN) from [simka].[MST_KARYAWAN] where npp = @nppp
		select @reminder_kgb =  DATEADD(mm, 21, TMT_GOLONGAN) from [simka].[MST_KARYAWAN] where npp = @nppp
		select @mk = MASA_KERJA_GOLONGAN + 2 from [simka].[MST_KARYAWAN] where npp = @nppp
		if(getdate() > @reminder_sk)
		BEGIN
		set @ket = 'Golongan'
		insert into @tabel
		select NPP,NAMA,TMT_GOLONGAN, ID_REF_GOLONGAN_LOKAL, @ket
			from [simka].[MST_KARYAWAN] where getdate() > @reminder_sk and TMT_GOLONGAN IS NOT NULL and npp=@nppp
		END
		else if(GETDATE() > @reminder_kgb AND (SELECT MASA_KERJA_GOLONGAN from [simka].[MST_KARYAWAN] where npp = @nppp) != @mk)
		BEGIN
		set @ket = 'KGB'
		insert into @tabel
		select NPP,NAMA_LENGKAP_GELAR,TMT_GOLONGAN, ID_REF_GOLONGAN_LOKAL, @ket
			from [simka].[MST_KARYAWAN] where getdate() > @reminder_kgb and TMT_GOLONGAN IS NOT NULL and npp=@nppp
		END
		
		FETCH NEXT FROM karyawan_cursor INTO 
            @nppp;
    END;

CLOSE karyawan_cursor;
DEALLOCATE karyawan_cursor;
SELECT * from @tabel
";

                    var data = conn.Query<KomponenGajiModel>(query).AsList();

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
        // -- End Dashboard --
    }
}
