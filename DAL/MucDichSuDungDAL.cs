using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho MucDichSuDung - Mô hình 3 lớp
    /// </summary>
    public class MucDichSuDungDAL
    {
        public List<MucDichSuDung> GetAll()
        {
            string query = "SELECT * FROM MUC_DICH_SU_DUNG ORDER BY MAMDSD";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public MucDichSuDung? GetById(string ma)
        {
            string query = "SELECT * FROM MUC_DICH_SU_DUNG WHERE MAMDSD = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

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

        public int Delete(string ma)
        {
            string query = "DELETE FROM MUC_DICH_SU_DUNG WHERE MAMDSD=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM MUC_DICH_SU_DUNG WHERE MAMDSD=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

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
