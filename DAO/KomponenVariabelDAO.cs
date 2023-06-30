using Dapper;
using System.Data.SqlClient;
using payrolTendik.Models;

namespace payrolTendik.DAO
{
    public class KomponenVariabelDAO
    {             
        public List<KomponenVariabelModel> getDataReferensi()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT ID_KOMPONEN_GAJI, ID_JNS_KOMPONEN, ID_JNS_POTONGAN, 
                                    KOMPONEN_GAJI_FOXPRO, KOMPONEN_GAJI, NO_URUT, IS_SATUAN, JENIS_FUNGSIONAL, IS_DELETED
                                    FROM payroll.MST_KOMPONEN_GAJI
                                    WHERE (IS_SATUAN = 0) AND (JENIS_FUNGSIONAL IS NULL) AND (ID_JNS_KOMPONEN) = 1";

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
        public KomponenVariabelModel getDatabyNpp(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 NPP, NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] WHERE NPP = @npp;";

                    var param = new { npp = npp };

                    var data = conn.QueryFirstOrDefault<KomponenVariabelModel>(query, param);

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
        public List<KehadiranModel> getKehadiranLembur(int id_tahun, int id_bulan, int id_unit)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"BEGIN
                                    DECLARE @tempTable table (num int, npp varchar(10), nama varchar(100),hari int, kerja varchar(10),lembur varchar(10),lembur_ekstra varchar(10),lembur_libur varchar(10),lembur_libur_ekstra varchar(10),terlambat int, nppp varchar(10))
                                    DECLARE @id_bulan_gaji int
                                    SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan 
                                    IF NOT EXISTS(SELECT * FROM [payroll].[TBL_VAKASI] a JOIN [simka].[MST_KARYAWAN] b 
                                    ON a.NPP = b.NPP 
                                    WHERE ID_BULAN_GAJI = @id_bulan_gaji AND ID_UNIT !=0 AND ID_KOMPONEN_GAJI IN (6,7,8,22,23,11))
                                    BEGIN 
                                    INSERT INTO @tempTable 
                                    EXEC [dbo].[sp_GetPresensiUnit]
	                                    @id_unit = @id_unit,
	                                    @tahun = @id_tahun,
	                                    @bulan = @id_bulan

                                    SELECT a.NPP,b.NAMA_LENGKAP_GELAR AS NAMA,NAMA_UNIT,HARI, LEMBUR, LEMBUR_LIBUR,LEMBUR_EKSTRA,LEMBUR_LIBUR_EKSTRA
                                    FROM @tempTable a 
                                    JOIN [simka].[MST_KARYAWAN] b ON a.NPP = b.NPP 
                                    JOIN [siatmax].[MST_UNIT] c ON b.ID_UNIT = c.ID_UNIT
                                    END

                                    IF (@id_unit =0) AND EXISTS (SELECT NPP FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI IN (6,7,22,23,11) AND ID_BULAN_GAJI = @id_bulan_gaji)
                                    BEGIN
                                    select DISTINCT a.NPP,NAMA_LENGKAP_GELAR AS NAMA,NAMA_UNIT,COALESCE(l.JUMLAH, 0 ) AS LEMBUR,COALESCE(lb.JUMLAH, 0 ) AS LEMBUR_LIBUR,
                                    COALESCE(lbe.JUMLAH, 0 ) AS LEMBUR_EKSTRA,COALESCE(lle.JUMLAH, 0 ) AS LEMBUR_LIBUR_EKSTRA,
                                    COALESCE(um.JUMLAH, 0 ) AS UANG_MAKAN,COALESCE(p.JUMLAH, 0 ) AS HARI, IS_PERMANENT FROM [payroll].[TBL_VAKASI] a
                                    LEFT join(
                                        select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 6 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) l
                                    on a.NPP = l.NPP 
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 7 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) lb
                                    ON a.NPP = lb.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 22 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) lbe
                                    ON a.NPP = lbe.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 23 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) lle
                                    ON a.NPP = lle.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 8 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) um
                                    ON a.NPP = um.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 11 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) p
                                    ON a.NPP = p.NPP
                                    JOIN [simka].[MST_KARYAWAN] b
                                    ON a.NPP = b.NPP
                                    JOIN [siatmax].[MST_UNIT] c
                                    ON b.ID_UNIT = c.ID_UNIT
                                    WHERE ID_BULAN_GAJI = @id_bulan_gaji AND CURRENT_STATUS ='Aktif' AND ID_REF_FUNGSIONAL !=1 AND ID_KOMPONEN_GAJI IN(6,7,11,22,23)
                                    END
                                    ELSE IF EXISTS (SELECT NPP FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI IN (6,7,22,23,11) AND ID_BULAN_GAJI = @id_bulan_gaji)
                                    BEGIN
                                    select DISTINCT a.NPP,NAMA_LENGKAP_GELAR AS NAMA,NAMA_UNIT,COALESCE(l.JUMLAH, 0 ) AS LEMBUR,COALESCE(lb.JUMLAH, 0 ) AS LEMBUR_LIBUR,
                                    COALESCE(lbe.JUMLAH, 0 ) AS LEMBUR_EKSTRA,COALESCE(lle.JUMLAH, 0 ) AS LEMBUR_LIBUR_EKSTRA,
                                    COALESCE(um.JUMLAH, 0 ) AS UANG_MAKAN,COALESCE(p.JUMLAH, 0 ) AS HARI, IS_PERMANENT FROM [payroll].[TBL_VAKASI] a
                                    LEFT join(
                                        select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 6 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) l
                                    on a.NPP = l.NPP 
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 7 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) lb
                                    ON a.NPP = lb.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 22 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) lbe
                                    ON a.NPP = lbe.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 23 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) lle
                                    ON a.NPP = lle.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 8 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) um
                                    ON a.NPP = um.NPP
                                    LEFT JOIN(
	                                    select NPP, JUMLAH FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI = 11 AND ID_BULAN_GAJI = @id_bulan_gaji
                                    ) p
                                    ON a.NPP = p.NPP
                                    JOIN [simka].[MST_KARYAWAN] b
                                    ON a.NPP = b.NPP
                                    JOIN [siatmax].[MST_UNIT] c
                                    ON b.ID_UNIT = c.ID_UNIT
                                    WHERE ID_BULAN_GAJI = @id_bulan_gaji AND b.ID_UNIT = @id_unit AND CURRENT_STATUS ='Aktif' AND ID_REF_FUNGSIONAL !=1 AND ID_KOMPONEN_GAJI IN(6,7,11,22,23)
                                    END
                                
                                    END";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit };
                    var data = conn.Query<KehadiranModel>(query, param).ToList();

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
        public List<KehadiranLemburModel> getKaryawanAll(int id_tahun, int id_bulan, int id_unit)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @" 
                                    IF (@id_unit = 0)
                                    SELECT @id_tahun AS TAHUN,@id_bulan AS BULAN,NPP FROM [simka].[MST_KARYAWAN] 
                                    WHERE ID_REF_FUNGSIONAL != 1 AND CURRENT_STATUS ='Aktif'
                                    ELSE
                                    SELECT @id_tahun AS TAHUN,@id_bulan AS BULAN,NPP FROM [simka].[MST_KARYAWAN] 
                                    WHERE ID_UNIT = @id_unit AND ID_REF_FUNGSIONAL != 1 AND CURRENT_STATUS ='Aktif'
                                    ";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit };
                    var data = conn.Query<KehadiranLemburModel>(query, param).AsList();

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
        public bool simpanKehadiranLembur(List<KehadiranModel> list)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int                                    
                                    SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
                                   
                                    IF EXISTS (SELECT * FROM [payroll].[TBL_VAKASI] WHERE NPP =@npp AND id_komponen_gaji = @id_komponen_gaji AND ID_BULAN_GAJI = @id_bulan_gaji)
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @jumlah_satuan, DATE_INSERTED = GETDATE() 
                                    WHERE NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_KOMPONEN_GAJI = @id_komponen_gaji
                                    ELSE 
                                    IF (@jumlah_satuan != 0)
                                    INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,DATE_INSERTED)
                                    VALUES (@id_komponen_gaji,@id_bulan_gaji,@npp,@jumlah_satuan,GETDATE())";

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
        public int ubahData(List<KehadiranModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    DECLARE @id_bulan_gaji int
                                    SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
                                    
                                    IF(@id_komponen_gaji = 11)
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @hari WHERE NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_KOMPONEN_GAJI = @id_komponen_gaji
                                    IF(@id_komponen_gaji = 6)
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @hari WHERE NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_KOMPONEN_GAJI = @id_komponen_gaji                                  
                                    IF(@id_komponen_gaji = 7)
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @hari WHERE NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_KOMPONEN_GAJI = @id_komponen_gaji 
                                    IF(@id_komponen_gaji = 22)
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @hari WHERE NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_KOMPONEN_GAJI = @id_komponen_gaji
                                    IF(@id_komponen_gaji = 23 )
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @hari WHERE NPP = @npp AND ID_BULAN_GAJI = @id_bulan_gaji AND ID_KOMPONEN_GAJI = @id_komponen_gaji ";


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
        public List<KehadiranLemburModel> tambahData(int id_tahun, int id_bulan, int id_unit)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int
SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND @id_tahun = @id_tahun
IF NOT EXISTS (SELECT NPP FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI IN (6,7,22,23,11) AND ID_BULAN_GAJI = @id_bulan_gaji)
BEGIN 

IF(@id_unit = 0) AND NOT EXISTS(SELECT NPP FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI IN (6,7,22,23,11) AND ID_BULAN_GAJI = @id_bulan_gaji)
BEGIN
    INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 6,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE CURRENT_STATUS = 'Aktif' AND ID_REF_FUNGSIONAL != 1
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 7,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE CURRENT_STATUS = 'Aktif' AND ID_REF_FUNGSIONAL != 1
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 22,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE CURRENT_STATUS = 'Aktif' AND ID_REF_FUNGSIONAL != 1
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 23,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE CURRENT_STATUS = 'Aktif' AND ID_REF_FUNGSIONAL != 1
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 11,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE CURRENT_STATUS = 'Aktif' AND ID_REF_FUNGSIONAL != 1
END
ELSE IF NOT EXISTS(SELECT NPP FROM [payroll].[TBL_VAKASI] WHERE ID_KOMPONEN_GAJI IN (6,7,22,23,11) AND ID_BULAN_GAJI = @id_bulan_gaji)
BEGIN
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 6,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND CURRENT_STATUS = 'Aktif'
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 7,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND CURRENT_STATUS = 'Aktif'
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 22,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND CURRENT_STATUS = 'Aktif'
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 23,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND CURRENT_STATUS = 'Aktif'
	INSERT INTO [payroll].[TBL_VAKASI] (ID_KOMPONEN_GAJI,ID_BULAN_GAJI,NPP,JUMLAH,IS_PERMANENT)
	SELECT 11,@id_bulan_gaji,NPP,0,0 AS IS_PERMANENT FROM [simka].[MST_KARYAWAN] WHERE ID_UNIT = @id_unit AND CURRENT_STATUS = 'Aktif'
END
                                    END";


                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit};
                    var data = conn.Query<KehadiranLemburModel>(query, param).ToList();

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
        // -- Suplisi dan Insentif --
        public List<SuplisiInsentif> getKaryawanAll(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"IF(@id_unit = 0)                                   
                                    SELECT @id_tahun AS TAHUN,@id_bulan AS BULAN,@id_komponen_gaji AS REFERENSI,
                                    @id_unit AS UNIT,NPP,NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] 
                                    WHERE CURRENT_STATUS ='Aktif' AND ID_REF_FUNGSIONAL <> 1
                                    ELSE
                                    SELECT @id_tahun AS TAHUN,@id_bulan AS BULAN,@id_komponen_gaji AS REFERENSI,
                                    ID_UNIT AS UNIT,NPP,NAMA_LENGKAP_GELAR AS NAMA FROM [simka].[MST_KARYAWAN] 
                                    WHERE ID_UNIT = @id_unit AND CURRENT_STATUS ='Aktif' AND ID_REF_FUNGSIONAL <> 1
                                    ";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit, id_komponen_gaji = id_komponen_gaji };
                    var data = conn.Query<SuplisiInsentif>(query, param).AsList();

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
        public List<KomponenVariabelModel> getDataSuplisiInsentif(int id_tahun, int id_bulan, int id_unit, int id_komponen_gaji)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"

                                  IF (@id_unit = 0)
                                  SELECT [ID_VAKASI] as id
                                  ,a.[ID_KOMPONEN_GAJI],KOMPONEN_GAJI
                                  ,a.[NPP],NAMA_LENGKAP_GELAR AS NAMA
                                  ,[JUMLAH]
                                  ,[DATE_INSERTED]
                                  ,[DESKRIPSI]
                                  ,[IS_PERMANENT]
                                  FROM [PAYROLL].[payroll].[TBL_VAKASI] a JOIN [payroll].[TBL_BULAN_GAJI] b 
                                  ON a.ID_BULAN_GAJI = b.ID_BULAN_GAJI
                                  JOIN [simka].[MST_KARYAWAN] c 
                                  ON a.NPP = c.NPP 
                                  JOIN [payroll].[MST_KOMPONEN_GAJI] d
                                  ON a.ID_KOMPONEN_GAJI = d.ID_KOMPONEN_GAJI
                                  WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan AND a.ID_KOMPONEN_GAJI = @id_komponen_gaji;   
                                  ELSE IF(@id_unit !=0)
                                  SELECT [ID_VAKASI] as id
                                  ,a.[ID_KOMPONEN_GAJI],KOMPONEN_GAJI
                                  ,a.[NPP],NAMA_LENGKAP_GELAR AS NAMA
                                  ,[JUMLAH]
                                  ,[DATE_INSERTED]
                                  ,[DESKRIPSI]
                                  ,[IS_PERMANENT]
                                  FROM [PAYROLL].[payroll].[TBL_VAKASI] a JOIN [payroll].[TBL_BULAN_GAJI] b 
                                  ON a.ID_BULAN_GAJI = b.ID_BULAN_GAJI
                                  JOIN [simka].[MST_KARYAWAN] c 
                                  ON a.NPP = c.NPP 
                                  JOIN [payroll].[MST_KOMPONEN_GAJI] d
                                  ON a.ID_KOMPONEN_GAJI = d.ID_KOMPONEN_GAJI
                                  WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan AND ID_UNIT = @id_unit AND a.ID_KOMPONEN_GAJI = @id_komponen_gaji";

                    var param = new { id_tahun = id_tahun, id_bulan = id_bulan, id_unit = id_unit, id_komponen_gaji = id_komponen_gaji };
                    var data = conn.Query<KomponenVariabelModel>(query, param).ToList();

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
        public bool simpanData(List<KomponenVariabelModel> list)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    DECLARE @id_bulan_gaji int, @id_penggajian int, @nominal_satuan int, @nominal_total int
                                    SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN = @id_bulan AND ID_TAHUN = @id_tahun
                                    --IF NOT EXISTS (SELECT NPP FROM [payroll].[TBL_VAKASI] WHERE ID_BULAN_GAJI = @id_bulan_gaji AND npp=@npp)
                                    IF @jumlah != 0
                                    INSERT INTO [payroll].[TBL_VAKASI](NPP,ID_BULAN_GAJI,ID_KOMPONEN_GAJI,JUMLAH,DATE_INSERTED,IS_PERMANENT) 
                                    VALUES (@npp,@id_bulan_gaji,@id_komponen_gaji,@jumlah,GETDATE(),0)
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
        public int ubahData(List<KomponenVariabelModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"
                                    DECLARE @id_komponen int,@nominal_satuan int, @nominal_total int
                                    SELECT @id_komponen = ID_KOMPONEN_GAJI FROM [payroll].[MST_KOMPONEN_GAJI] WHERE KOMPONEN_GAJI = @komponen_gaji
                                    UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @jumlah
                                    WHERE NPP = @npp AND ID_KOMPONEN_GAJI = @id_komponen";

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
        public int simpanPermanen(List<KomponenVariabelModel> mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_VAKASI] SET IS_PERMANENT = 1 WHERE ID_VAKASI =@id";

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
        // -- Start Cuti Panjang --
        public List<CutiPanjangModel> getDataCutiPanjang()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT ID_CUTI_PANJANG AS ID,a.NPP,TGL_PENGAJUAN, JML_BULAN_DIGANTI, ID_TAHUN_ANGGARAN AS TAHUN, a.BULAN AS ID_BULAN, c.BULAN, IS_LOCK
                                    ,NAMA_LENGKAP_GELAR AS NAMA,CONVERT(char(10), TGL_AWAL_CUTI,126) AS TGL_AWAL_CUTI,
                                    CONVERT(char(10), TGL_AKHIR_CUTI,126) AS TGL_AKHIR_CUTI, IS_LOCK FROM 
                                    [PAYROLL].[payroll].[TBL_CUTI_PANJANG] a 
                                    JOIN [simka].[MST_KARYAWAN] b ON a.NPP = b.NPP
									JOIN [payroll].[REF_BULAN] c ON a.BULAN = c.ID_BULAN";

                    var data = conn.Query<CutiPanjangModel>(query).AsList();

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
        public int simpanCutiPanjang(CutiPanjangModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @durasi int
                                    SELECT @durasi =(SELECT TOP 1 DATEDIFF(year, TGL_PENGAJUAN,GETDATE()) AS DateDiff FROM [payroll].[TBL_CUTI_PANJANG] 
                                    WHERE NPP = @npp
                                    ORDER BY Datediff ASC)
                                    IF NOT EXISTS (SELECT * FROM [payroll].[TBL_CUTI_PANJANG] WHERE NPP = @npp)
                                    INSERT INTO [payroll].[TBL_CUTI_PANJANG]
                                    ([NPP] ,[TGL_PENGAJUAN],[TGL_AWAL_CUTI], [TGL_AKHIR_CUTI], [JML_BULAN_DIGANTI], [ID_TAHUN_ANGGARAN], [BULAN], [IS_LOCK])
                                    VALUES
                                    (@npp,@tgl_pengajuan,@tgl_awal_cuti,@tgl_akhir_cuti,@jml_bulan_diganti,@tahun,@id_bulan,0)
                                    IF EXISTS (SELECT * FROM [payroll].[TBL_CUTI_PANJANG] WHERE NPP = @npp)
                                    IF @durasi >= 6
                                    INSERT INTO [payroll].[TBL_CUTI_PANJANG]
                                    ([NPP] ,[TGL_PENGAJUAN],[TGL_AWAL_CUTI], [TGL_AKHIR_CUTI], [JML_BULAN_DIGANTI], [ID_TAHUN_ANGGARAN], [BULAN], [IS_LOCK])
                                    VALUES
                                    (@npp,@tgl_pengajuan,@tgl_awal_cuti,@tgl_akhir_cuti,@jml_bulan_diganti,@tahun,@id_bulan,0)";

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
        public bool ubahCutiPanjang(CutiPanjangModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_CUTI_PANJANG] SET JML_BULAN_DIGANTI = @jml_bulan_diganti
                                    ,TGL_AWAL_CUTI = @tgl_awal_cuti, TGL_AKHIR_CUTI = @tgl_akhir_cuti
                                    ,ID_TAHUN_ANGGARAN = @tahun, BULAN = @ID_BULAN 
                                    WHERE ID_CUTI_PANJANG = @id AND IS_LOCK = 0";

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
        public bool lockCutiPanjang(CutiPanjangModel mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_CUTI_PANJANG] SET IS_LOCK = 1                                  
                                    WHERE ID_CUTI_PANJANG = @id";

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
        // --End Cuti Panjang--

        //-- Start Penerimaan Lain-Lain --
        public List<PenerimaanLainLain> getDataPenerimaanLainLain()
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"SELECT [ID_VAKASI] AS ID,NAMA_LENGKAP_GELAR AS NAMA,
                                    [ID_KOMPONEN_GAJI],a.[ID_BULAN_GAJI],BULAN, ID_TAHUN,a.[NPP],[JUMLAH],[DATE_INSERTED],[DESKRIPSI]
                                    FROM [PAYROLL].[payroll].[TBL_VAKASI] a JOIN [simka].[MST_KARYAWAN] b 
                                    ON a.NPP = b.NPP
                                    JOIN [payroll].[TBL_BULAN_GAJI] c
                                    ON a.ID_BULAN_GAJI = c.ID_BULAN_GAJI
                                    WHERE ID_KOMPONEN_GAJI = 70
                                    ORDER BY a.ID_BULAN_GAJI ASC";

                    var data = conn.Query<PenerimaanLainLain>(query).AsList();

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
        public bool simpanPenerimaanLainLain(PenerimaanLainLain mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"DECLARE @id_bulan_gaji int
                                   SELECT @id_bulan_gaji = ID_BULAN_GAJI FROM [payroll].[TBL_BULAN_GAJI] WHERE ID_TAHUN = @id_tahun AND ID_BULAN = @id_bulan
                                   INSERT INTO [payroll].[TBL_VAKASI]
                                   ([ID_KOMPONEN_GAJI],ID_BULAN_GAJI,[NPP],[DATE_INSERTED],[JUMLAH], [DESKRIPSI])
                                   VALUES
                                   (70,@id_bulan_gaji,@npp,GETDATE(),@jumlah,@deskripsi)";

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
        public bool ubahPenerimaanLainLain(PenerimaanLainLain mdl)
        {
            using (SqlConnection conn = new SqlConnection(DBKoneksi.koneksi))
            {
                try
                {
                    string query = @"UPDATE [payroll].[TBL_VAKASI] SET JUMLAH = @jumlah, DESKRIPSI =@deskripsi WHERE ID_VAKASI = @id";

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

        // -- End Penerimaan Lain-Lain --
    }
}
