using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho VatLieu - Mô hình 3 lớp
    /// </summary>
    public class VatLieuDAL
    {
        public List<VatLieu> GetAll()
        {
            string query = "SELECT * FROM VAT_LIEU ORDER BY MAVL";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public VatLieu? GetById(string ma)
        {
            string query = "SELECT * FROM VAT_LIEU WHERE MAVL = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(VatLieu obj)
        {
            string query = "INSERT INTO VAT_LIEU (MAVL, TENVL, MAUVL, MOTAVL, SOLUONG) VALUES (@Ma, @Ten, @Mau, @Mota, @SoLuong)";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Mavl),
                new SqlParameter("@Ten", (object?)obj.Tenvl ?? DBNull.Value),
                new SqlParameter("@Mau", (object?)obj.Mauvl ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Motavl ?? DBNull.Value),
                new SqlParameter("@SoLuong", (object?)obj.Soluong ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(VatLieu obj)
        {
            string query = "UPDATE VAT_LIEU SET TENVL = @Ten, MAUVL = @Mau, MOTAVL = @Mota, SOLUONG = @SoLuong WHERE MAVL = @Ma";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Mavl),
                new SqlParameter("@Ten", (object?)obj.Tenvl ?? DBNull.Value),
                new SqlParameter("@Mau", (object?)obj.Mauvl ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Motavl ?? DBNull.Value),
                new SqlParameter("@SoLuong", (object?)obj.Soluong ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM VAT_LIEU WHERE MAVL = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM VAT_LIEU WHERE MAVL = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        private List<VatLieu> MapDataTableToList(DataTable dt)
        {
            var list = new List<VatLieu>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new VatLieu
                {
                    Mavl = row["MAVL"].ToString() ?? "",
                    Tenvl = row["TENVL"] != DBNull.Value ? row["TENVL"].ToString() : null,
                    Mauvl = row["MAUVL"] != DBNull.Value ? row["MAUVL"].ToString() : null,
                    Motavl = row["MOTAVL"] != DBNull.Value ? row["MOTAVL"].ToString() : null,
                    Soluong = row["SOLUONG"] != DBNull.Value ? Convert.ToInt32(row["SOLUONG"]) : null
                });
            }
            return list;
        }
    }
}
