using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    // Data Access Layer cho MucDichSuDung - Mô hình 3 lớp
    public class MucDichSuDungDAL
    {
        // Lấy tất cả mục đích sử dụng
        public List<MucDichSuDung> GetAll()
        {
            string query = "SELECT * FROM MUC_DICH_SU_DUNG ORDER BY MAMDSD";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        // Lấy mục đích sử dụng theo mã
        public MucDichSuDung? GetById(string ma)
        {
            string query = "SELECT * FROM MUC_DICH_SU_DUNG WHERE MAMDSD = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        // Thêm mục đích sử dụng mới
        public int Insert(MucDichSuDung obj)
        {
            string query = "INSERT INTO MUC_DICH_SU_DUNG (MAMDSD, TENMDSD, MOTAMDSD) VALUES (@Ma, @Ten, @Mota)";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Mamdsd),
                new SqlParameter("@Ten", (object?)obj.Tenmdsd ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Motamdsd ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Cập nhật mục đích sử dụng
        public int Update(MucDichSuDung obj)
        {
            string query = "UPDATE MUC_DICH_SU_DUNG SET TENMDSD=@Ten, MOTAMDSD=@Mota WHERE MAMDSD=@Ma";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Mamdsd),
                new SqlParameter("@Ten", (object?)obj.Tenmdsd ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)obj.Motamdsd ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa mục đích sử dụng
        public int Delete(string ma)
        {
            string query = "DELETE FROM MUC_DICH_SU_DUNG WHERE MAMDSD=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Kiểm tra mục đích sử dụng tồn tại
        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM MUC_DICH_SU_DUNG WHERE MAMDSD=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Tìm kiếm mục đích sử dụng theo từ khóa
        public List<MucDichSuDung> Search(string? keyword)
        {
            string query = "SELECT * FROM MUC_DICH_SU_DUNG WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(keyword))
            {
                query += " AND (MAMDSD LIKE @Keyword OR TENMDSD LIKE @Keyword OR MOTAMDSD LIKE @Keyword)";
                parameters.Add(new SqlParameter("@Keyword", $"%{keyword}%"));
            }

            query += " ORDER BY MAMDSD";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters.ToArray()));
        }

        // Chuyển DataTable thành List<MucDichSuDung>
        private List<MucDichSuDung> MapDataTableToList(DataTable dt)
        {
            var list = new List<MucDichSuDung>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MucDichSuDung
                {
                    Mamdsd = row["MAMDSD"].ToString() ?? "",
                    Tenmdsd = row["TENMDSD"] != DBNull.Value ? row["TENMDSD"].ToString() : null,
                    Motamdsd = row["MOTAMDSD"] != DBNull.Value ? row["MOTAMDSD"].ToString() : null
                });
            }
            return list;
        }
    }
}
