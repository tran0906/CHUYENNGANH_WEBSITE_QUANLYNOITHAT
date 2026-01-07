// FILE: DAL/NhaCungCapDAL.cs - Truy cập dữ liệu bảng NHA_CUNG_CAP

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class NhaCungCapDAL
    {
        // Lấy tất cả nhà cung cấp
        public List<NhaCungCap> GetAll()
        {
            string query = "SELECT * FROM NHA_CUNG_CAP ORDER BY MANCC";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public NhaCungCap? GetById(string ma)
        {
            string query = "SELECT * FROM NHA_CUNG_CAP WHERE MANCC = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(NhaCungCap obj)
        {
            string query = @"INSERT INTO NHA_CUNG_CAP (MANCC, TENNCC, DIACHINCC, SDTNCC, EMAILNCC, SANPHAM, GIANHAP, NGAYCAPNHATSP) 
                            VALUES (@Mancc, @Tenncc, @Diachincc, @Sdtncc, @Emailncc, @Sanpham, @Gianhap, @Ngaycapnhatsp)";
            SqlParameter[] parameters = {
                new SqlParameter("@Mancc", obj.Mancc),
                new SqlParameter("@Tenncc", (object?)obj.Tenncc ?? DBNull.Value),
                new SqlParameter("@Diachincc", (object?)obj.Diachincc ?? DBNull.Value),
                new SqlParameter("@Sdtncc", (object?)obj.Sdtncc ?? DBNull.Value),
                new SqlParameter("@Emailncc", (object?)obj.Emailncc ?? DBNull.Value),
                new SqlParameter("@Sanpham", (object?)obj.Sanpham ?? DBNull.Value),
                new SqlParameter("@Gianhap", (object?)obj.Gianhap ?? DBNull.Value),
                new SqlParameter("@Ngaycapnhatsp", (object?)obj.Ngaycapnhatsp ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(NhaCungCap obj)
        {
            string query = @"UPDATE NHA_CUNG_CAP SET TENNCC=@Tenncc, DIACHINCC=@Diachincc, SDTNCC=@Sdtncc, 
                            EMAILNCC=@Emailncc, SANPHAM=@Sanpham, GIANHAP=@Gianhap, NGAYCAPNHATSP=@Ngaycapnhatsp 
                            WHERE MANCC=@Mancc";
            SqlParameter[] parameters = {
                new SqlParameter("@Mancc", obj.Mancc),
                new SqlParameter("@Tenncc", (object?)obj.Tenncc ?? DBNull.Value),
                new SqlParameter("@Diachincc", (object?)obj.Diachincc ?? DBNull.Value),
                new SqlParameter("@Sdtncc", (object?)obj.Sdtncc ?? DBNull.Value),
                new SqlParameter("@Emailncc", (object?)obj.Emailncc ?? DBNull.Value),
                new SqlParameter("@Sanpham", (object?)obj.Sanpham ?? DBNull.Value),
                new SqlParameter("@Gianhap", (object?)obj.Gianhap ?? DBNull.Value),
                new SqlParameter("@Ngaycapnhatsp", (object?)obj.Ngaycapnhatsp ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM NHA_CUNG_CAP WHERE MANCC=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM NHA_CUNG_CAP WHERE MANCC=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        private List<NhaCungCap> MapDataTableToList(DataTable dt)
        {
            var list = new List<NhaCungCap>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new NhaCungCap
                {
                    Mancc = row["MANCC"].ToString() ?? "",
                    Tenncc = row["TENNCC"] != DBNull.Value ? row["TENNCC"].ToString() : null,
                    Diachincc = row["DIACHINCC"] != DBNull.Value ? row["DIACHINCC"].ToString() : null,
                    Sdtncc = row["SDTNCC"] != DBNull.Value ? row["SDTNCC"].ToString() : null,
                    Emailncc = row["EMAILNCC"] != DBNull.Value ? row["EMAILNCC"].ToString() : null,
                    Sanpham = row["SANPHAM"] != DBNull.Value ? row["SANPHAM"].ToString() : null,
                    Gianhap = row["GIANHAP"] != DBNull.Value ? Convert.ToDecimal(row["GIANHAP"]) : null,
                    Ngaycapnhatsp = row["NGAYCAPNHATSP"] != DBNull.Value ? Convert.ToDateTime(row["NGAYCAPNHATSP"]) : null
                });
            }
            return list;
        }
    }
}
