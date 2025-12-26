using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho QuanBaSp - Mô hình 3 lớp
    /// </summary>
    public class QuanBaSpDAL
    {
        public List<QuanBaSp> GetAll()
        {
            string query = @"SELECT qb.*, u.HoTen as TenNguoiTao
                            FROM QUAN_BA_SP qb
                            LEFT JOIN [User] u ON qb.USERID = u.UserId
                            ORDER BY qb.NGAYBATDAU DESC";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public QuanBaSp? GetById(string ma)
        {
            string query = @"SELECT qb.*, u.HoTen as TenNguoiTao
                            FROM QUAN_BA_SP qb
                            LEFT JOIN [User] u ON qb.USERID = u.UserId
                            WHERE qb.MADOTGIAMGIA = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(QuanBaSp obj)
        {
            string query = @"INSERT INTO QUAN_BA_SP (MADOTGIAMGIA, USERID, NGAYBATDAU, NGAYKETTHUC, MANVCHON, PHANTRAMGIAM) 
                            VALUES (@Ma, @Userid, @Ngaybatdau, @Ngayketthuc, @Manvchon, @Phantramgiam)";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Madotgiamgia),
                new SqlParameter("@Userid", (object?)obj.Userid ?? DBNull.Value),
                new SqlParameter("@Ngaybatdau", (object?)obj.Ngaybatdau ?? DBNull.Value),
                new SqlParameter("@Ngayketthuc", (object?)obj.Ngayketthuc ?? DBNull.Value),
                new SqlParameter("@Manvchon", (object?)obj.Manvchon ?? DBNull.Value),
                new SqlParameter("@Phantramgiam", (object?)obj.Phantramgiam ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(QuanBaSp obj)
        {
            string query = @"UPDATE QUAN_BA_SP SET USERID=@Userid, NGAYBATDAU=@Ngaybatdau, 
                            NGAYKETTHUC=@Ngayketthuc, MANVCHON=@Manvchon, PHANTRAMGIAM=@Phantramgiam WHERE MADOTGIAMGIA=@Ma";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", obj.Madotgiamgia),
                new SqlParameter("@Userid", (object?)obj.Userid ?? DBNull.Value),
                new SqlParameter("@Ngaybatdau", (object?)obj.Ngaybatdau ?? DBNull.Value),
                new SqlParameter("@Ngayketthuc", (object?)obj.Ngayketthuc ?? DBNull.Value),
                new SqlParameter("@Manvchon", (object?)obj.Manvchon ?? DBNull.Value),
                new SqlParameter("@Phantramgiam", (object?)obj.Phantramgiam ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM QUAN_BA_SP WHERE MADOTGIAMGIA=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM QUAN_BA_SP WHERE MADOTGIAMGIA=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        private List<QuanBaSp> MapDataTableToList(DataTable dt)
        {
            var list = new List<QuanBaSp>();
            foreach (DataRow row in dt.Rows)
            {
                var qb = new QuanBaSp
                {
                    Madotgiamgia = row["MADOTGIAMGIA"].ToString() ?? "",
                    Userid = row["USERID"] != DBNull.Value ? row["USERID"].ToString() : null,
                    Ngaybatdau = row["NGAYBATDAU"] != DBNull.Value ? Convert.ToDateTime(row["NGAYBATDAU"]) : null,
                    Ngayketthuc = row["NGAYKETTHUC"] != DBNull.Value ? Convert.ToDateTime(row["NGAYKETTHUC"]) : null,
                    Manvchon = row["MANVCHON"] != DBNull.Value ? row["MANVCHON"].ToString() : null,
                    Phantramgiam = row.Table.Columns.Contains("PHANTRAMGIAM") && row["PHANTRAMGIAM"] != DBNull.Value 
                        ? Convert.ToInt32(row["PHANTRAMGIAM"]) : 0
                };

                if (dt.Columns.Contains("TenNguoiTao") && row["TenNguoiTao"] != DBNull.Value)
                    qb.User = new User { UserId = qb.Userid ?? "", HoTen = row["TenNguoiTao"].ToString() };

                list.Add(qb);
            }
            return list;
        }
    }
}
