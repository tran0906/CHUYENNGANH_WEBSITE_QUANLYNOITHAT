namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models
{
    public class UserSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string UserType { get; set; } = string.Empty; // "Admin" or "Customer"
        public string? UserName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime ExpireTime { get; set; }
        public bool IsActive { get; set; }
        
        // Computed properties
        public bool IsExpired => DateTime.Now > ExpireTime;
        public string Status => !IsActive ? "Đã đăng xuất" : (IsExpired ? "Hết hạn" : "Đang hoạt động");
        public string StatusBadgeClass => !IsActive ? "bg-secondary" : (IsExpired ? "bg-warning" : "bg-success");
    }

    public class SystemConfig
    {
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
