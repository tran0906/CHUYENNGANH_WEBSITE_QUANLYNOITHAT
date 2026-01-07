// FILE: DAL/VanChuyenDAL.cs - Truy cập dữ liệu bảng VAN_CHUYEN

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class VanChuyenDAL
    {
        // Lấy tất cả bản ghi vận chuyển
        public List<VanChuyen> GetAll()
        {
            string query = @"SELECT vc.*, u.HoTen as TenNguoiTao, dh.NGAYDAT as Ngaydat
                            FROM VAN_CHUYEN vc
                            LEFT JOIN [User] u ON vc.USERID = u.UserId
                            LEFT JOIN DON_HANG dh ON vc.MADONHANG = dh.MADONHANG
                            ORDER BY vc.MAVANDON";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public List<VanChuyen> GetByDonHang(string maDh)
        {
            string query = @"SELECT vc.*, u.HoTen as TenNguoiTao
                            FROM VAN_CHUYEN vc
                            LEFT JOIN [User] u ON vc.USERID = u.UserId
                            WHERE vc.MADONHANG = @MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        public VanChuyen? GetById(string ma)
        {
            string query = @"SELECT vc.*, u.HoTen as TenNguoiTao
                            FROM VAN_CHUYEN vc
                            LEFT JOIN [User] u ON vc.USERID = u.UserId
                            WHERE vc.MAVANDON = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(VanChuyen obj)
        {
            string query = @"INSERT INTO VAN_CHUYEN (MAVANDON, USERID, MADONHANG, DONVIVANCHUYEN, TRANGTHAIGIAO, MANVDIEUPHOI) 
                            VALUES (@Mavandon, @Userid, @Madonhang, @Donvivanchuyen, @Trangthaigiao, @Manvdieuphoi)";
            SqlParameter[] parameters = {
                new SqlParameter("@Mavandon", obj.Mavandon),
                new SqlParameter("@Userid", obj.Userid),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Donvivanchuyen", (object?)obj.Donvivanchuyen ?? DBNull.Value),
                new SqlParameter("@Trangthaigiao", (object?)obj.Trangthaigiao ?? DBNull.Value),
                new SqlParameter("@Manvdieuphoi", (object?)obj.Manvdieuphoi ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(VanChuyen obj)
        {
            string query = @"UPDATE VAN_CHUYEN SET USERID=@Userid, MADONHANG=@Madonhang, DONVIVANCHUYEN=@Donvivanchuyen, 
                            TRANGTHAIGIAO=@Trangthaigiao, MANVDIEUPHOI=@Manvdieuphoi WHERE MAVANDON=@Mavandon";
            SqlParameter[] parameters = {
                new SqlParameter("@Mavandon", obj.Mavandon),
                new SqlParameter("@Userid", obj.Userid),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Donvivanchuyen", (object?)obj.Donvivanchuyen ?? DBNull.Value),
                new SqlParameter("@Trangthaigiao", (object?)obj.Trangthaigiao ?? DBNull.Value),
                new SqlParameter("@Manvdieuphoi", (object?)obj.Manvdieuphoi ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM VAN_CHUYEN WHERE MAVANDON=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM VAN_CHUYEN WHERE MAVANDON=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public string GenerateNewId()
        {
            var result = SqlConnectionHelper.ExecuteScalar("SELECT MAX(MAVANDON) FROM VAN_CHUYEN WHERE MAVANDON LIKE 'VC%'");
            if (result == null || result == DBNull.Value) return "VC001";
            int num = int.Parse(result.ToString()!.Substring(2)) + 1;
            return $"VC{num:D3}";
        }

        private List<VanChuyen> MapDataTableToList(DataTable dt)
        {
            var list = new List<VanChuyen>();
            foreach (DataRow row in dt.Rows)
            {
                var vc = new VanChuyen
                {
                    Mavandon = row["MAVANDON"].ToString() ?? "",
                    Userid = row["USERID"].ToString() ?? "",
                    Madonhang = row["MADONHANG"].ToString() ?? "",
                    Donvivanchuyen = row["DONVIVANCHUYEN"] != DBNull.Value ? row["DONVIVANCHUYEN"].ToString() : null,
                    Trangthaigiao = row["TRANGTHAIGIAO"] != DBNull.Value ? row["TRANGTHAIGIAO"].ToString() : null,
                    Manvdieuphoi = row["MANVDIEUPHOI"] != DBNull.Value ? row["MANVDIEUPHOI"].ToString() : null
                };

                if (dt.Columns.Contains("TenNguoiTao") && row["TenNguoiTao"] != DBNull.Value)
                    vc.User = new User { UserId = vc.Userid, HoTen = row["TenNguoiTao"].ToString() };

                list.Add(vc);
            }
            return list;
        }
    }
}
