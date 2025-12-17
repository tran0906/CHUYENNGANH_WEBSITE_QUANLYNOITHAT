using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho Cungcap - Mô hình 3 lớp
    /// </summary>
    public class CungcapDAL
    {
        public List<Cungcap> GetAll()
        {
            string query = @"SELECT cc.*, ncc.TENNCC as Tenncc, sp.TENSP as Tensp
                            FROM CUNGCAP cc
                            LEFT JOIN NHA_CUNG_CAP ncc ON cc.MANCC = ncc.MANCC
                            LEFT JOIN SAN_PHAM sp ON cc.MASP = sp.MASP
                            ORDER BY cc.MANCC, cc.MASP";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

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

        public int Delete(string maNcc, string maSp)
        {
            string query = "DELETE FROM CUNGCAP WHERE MANCC=@MaNcc AND MASP=@MaSp";
            SqlParameter[] parameters = { new SqlParameter("@MaNcc", maNcc), new SqlParameter("@MaSp", maSp) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string maNcc, string maSp)
        {
            string query = "SELECT COUNT(*) FROM CUNGCAP WHERE MANCC=@MaNcc AND MASP=@MaSp";
            SqlParameter[] parameters = { new SqlParameter("@MaNcc", maNcc), new SqlParameter("@MaSp", maSp) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

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
