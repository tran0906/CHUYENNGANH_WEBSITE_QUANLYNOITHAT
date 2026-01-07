// FILE: DAL/NhomSanPhamDAL.cs - Truy cập dữ liệu bảng NHOM_SAN_PHAM

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class NhomSanPhamDAL
    {
        // Lấy tất cả nhóm sản phẩm
        public List<NhomSanPham> GetAll()
        {
            string query = "SELECT * FROM NHOM_SAN_PHAM ORDER BY MANHOMSP";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public NhomSanPham? GetById(string ma)
        {
            string query = "SELECT * FROM NHOM_SAN_PHAM WHERE MANHOMSP = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(NhomSanPham obj)
        {
            string query = "INSERT INTO NHOM_SAN_PHAM (MANHOMSP, TENNHOMSP, MOTA) VALUES (@Ma, @Ten, @Mota)";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Manhomsp),
                new SqlParameter("@Ten", (object?)obj.Tennhomsp ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Mota ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(NhomSanPham obj)
        {
            string query = "UPDATE NHOM_SAN_PHAM SET TENNHOMSP = @Ten, MOTA = @Mota WHERE MANHOMSP = @Ma";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Manhomsp),
                new SqlParameter("@Ten", (object?)obj.Tennhomsp ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Mota ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM NHOM_SAN_PHAM WHERE MANHOMSP = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM NHOM_SAN_PHAM WHERE MANHOMSP = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        private List<NhomSanPham> MapDataTableToList(DataTable dt)
        {
            var list = new List<NhomSanPham>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new NhomSanPham
                {
                    Manhomsp = row["MANHOMSP"].ToString() ?? "",
                    Tennhomsp = row["TENNHOMSP"] != DBNull.Value ? row["TENNHOMSP"].ToString() : null,
                    Mota = row["MOTA"] != DBNull.Value ? row["MOTA"].ToString() : null
                });
            }
            return list;
        }
    }
}
