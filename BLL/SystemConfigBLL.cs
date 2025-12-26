using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class SystemConfigBLL
    {
        private readonly SystemConfigDAL _dal = new SystemConfigDAL();

        public List<SystemConfig> GetAll() => _dal.GetAll();
        public SystemConfig? GetByKey(string key) => _dal.GetByKey(key);
        public string GetValue(string key, string defaultValue = "") => _dal.GetValue(key, defaultValue);
        public int GetIntValue(string key, int defaultValue = 0) => _dal.GetIntValue(key, defaultValue);

        public (bool Success, string Message) UpdateConfig(string key, string value, string? updatedBy = null)
        {
            if (string.IsNullOrEmpty(key))
                return (false, "Key không được để trống");
            if (string.IsNullOrEmpty(value))
                return (false, "Value không được để trống");

            try
            {
                var result = _dal.Update(key, value, updatedBy);
                return result > 0 ? (true, "Cập nhật cấu hình thành công") : (false, "Cấu hình không tồn tại");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // Các hằng số cấu hình
        public const string SESSION_TIMEOUT = "SessionTimeout";
        public const string COOKIE_EXPIRE_DAYS = "CookieExpireDays";
        public const string MAX_SESSION_PER_USER = "MaxSessionPerUser";
    }
}
