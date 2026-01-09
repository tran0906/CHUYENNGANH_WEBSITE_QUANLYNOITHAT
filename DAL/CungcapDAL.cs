// FILE: DAL/CungcapDAL.cs
// TẦNG DAL - Truy cập dữ liệu bảng CUNGCAP (NCC - SP)
// LUỒNG: BLL → DAL → SqlConnectionHelper → Database

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class CungcapDAL
    {
        // Lấy tất cả thông tin cung cấp
        public List<Cungcap> GetAll()
        {
            string query = @"SELECT cc.*, ncc.TENNCC as Tenncc, sp.TENSP as Tensp
                            FROM CUNGCAP cc
                            LEFT JOIN NHA_CUNG_CAP ncc ON cc.MANCC = ncc.MANCC
                            LEFT JOIN SAN_PHAM sp ON cc.MASP = sp.MASP
                            ORDER BY cc.MANCC, cc.MASP";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        // Lấy thông tin cung cấp theo mã NCC và mã SP
        public Cungcap? GetById(string maNcc, string maSp)
        {
            string query = @"SELECT cc.*, ncc.TENNCC as Tenncc, sp.TENSP as Tensp
                            FROM CUNGCAP cc
                            LEFT JOIN NHA_CUNG_CAP ncc ON cc.MANCC = ncc.MANCC
                            LEFT JOIN SAN_PHAM sp ON cc.MASP = sp.MASP
                            WHERE cc.MANCC = @MaNcc AND cc.MASP = @MaSp";
            SqlParameter[] parameters = { 
                new SqlParameter("@MaNcc", maNcc), 
                new SqlParameter("@MaSp", maSp) 
            };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        // Thêm liên kết cung cấp mới
        public int Insert(Cungcap obj)
        {
            string query = "INSERT INTO CUNGCAP (MANCC, MASP, SOLUONGSP) VALUES (@Mancc, @Masp, @Soluongsp)";
            SqlParameter[] parameters = {
                new SqlParameter("@Mancc", obj.Mancc),
                new SqlParameter("@Masp", obj.Masp),
                new SqlParameter("@Soluongsp", (object?)obj.Soluongsp ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Cập nhật số lượng cung cấp
        public int Update(Cungcap obj)
        {
            string query = "UPDATE CUNGCAP SET SOLUONGSP=@Soluongsp WHERE MANCC=@Mancc AND MASP=@Masp";
            SqlParameter[] parameters = {
                new SqlParameter("@Mancc", obj.Mancc),
                new SqlParameter("@Masp", obj.Masp),
                new SqlParameter("@Soluongsp", (object?)obj.Soluongsp ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa liên kết cung cấp
        public int Delete(string maNcc, string maSp)
        {
            string query = "DELETE FROM CUNGCAP WHERE MANCC=@MaNcc AND MASP=@MaSp";
            SqlParameter[] parameters = { new SqlParameter("@MaNcc", maNcc), new SqlParameter("@MaSp", maSp) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Kiểm tra liên kết cung cấp tồn tại
        public bool Exists(string maNcc, string maSp)
        {
            string query = "SELECT COUNT(*) FROM CUNGCAP WHERE MANCC=@MaNcc AND MASP=@MaSp";
            SqlParameter[] parameters = { new SqlParameter("@MaNcc", maNcc), new SqlParameter("@MaSp", maSp) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Chuyển DataTable thành List<Cungcap>
        private List<Cungcap> MapDataTableToList(DataTable dt)
        {
            var list = new List<Cungcap>();
            foreach (DataRow row in dt.Rows)
            {
                var cc = new Cungcap
                {
                    Mancc = row["MANCC"].ToString() ?? "",
                    Masp = row["MASP"].ToString() ?? "",
                    Soluongsp = row["SOLUONGSP"] != DBNull.Value ? Convert.ToInt32(row["SOLUONGSP"]) : null
                };

                if (dt.Columns.Contains("Tenncc") && row["Tenncc"] != DBNull.Value)
                    cc.ManccNavigation = new NhaCungCap { Mancc = cc.Mancc, Tenncc = row["Tenncc"].ToString() };
                if (dt.Columns.Contains("Tensp") && row["Tensp"] != DBNull.Value)
                    cc.MaspNavigation = new SanPham { Masp = cc.Masp, Tensp = row["Tensp"].ToString() };

                list.Add(cc);
            }
            return list;
        }
    }
}
