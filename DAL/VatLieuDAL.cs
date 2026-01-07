// FILE: DAL/VatLieuDAL.cs - Truy cập dữ liệu bảng VAT_LIEU

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class VatLieuDAL
    {
        // Lấy tất cả vật liệu
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
            string query = "INSERT INTO VAT_LIEU (MAVL, TENVL, MOTAVL) VALUES (@Ma, @Ten, @Mota)";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Mavl),
                new SqlParameter("@Ten", (object?)obj.Tenvl ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Motavl ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(VatLieu obj)
        {
            string query = "UPDATE VAT_LIEU SET TENVL = @Ten, MOTAVL = @Mota WHERE MAVL = @Ma";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Mavl),
                new SqlParameter("@Ten", (object?)obj.Tenvl ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Motavl ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM VAT_LIEU WHERE MAVL = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public List<VatLieu> Search(string? search)
        {
            string query = "SELECT * FROM VAT_LIEU WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (MAVL LIKE @Search OR TENVL LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", $"%{search}%"));
            }
            query += " ORDER BY MAVL";

            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters.ToArray()));
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
                    Motavl = row["MOTAVL"] != DBNull.Value ? row["MOTAVL"].ToString() : null
                });
            }
            return list;
        }
    }
}
