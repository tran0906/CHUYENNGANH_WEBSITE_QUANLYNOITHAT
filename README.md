Tổng quan dự án: Website Quản Lý Nội Thất Việt
Loại dự án: Website thương mại điện tử bán nội thất
Công nghệ: ASP.NET Core MVC (.NET 8)
Kiến trúc: 3 lớp (DAL - BLL - Presentation) với Raw SQL (ADO.NET)
Database: SQL Server - WEB_NOITHAT

Chức năng chính
Trang khách hàng (Frontend):

Trang chủ, giới thiệu, tin tức, dịch vụ, liên hệ
Xem sản phẩm theo nhóm, mục đích sử dụng, vật liệu
Giỏ hàng, đặt hàng, thanh toán COD
Đăng ký, đăng nhập, quản lý tài khoản, xem lịch sử đơn hàng
Trang quản trị (Admin Area):

Dashboard thống kê tổng quan
Quản lý: Sản phẩm, Nhóm SP, Vật liệu, Nhà cung cấp, Mục đích sử dụng
Quản lý: Khách hàng, Nhân viên, Người dùng
Quản lý đơn hàng với quy trình xử lý đầy đủ:
Chờ xác nhận → Đã xác nhận → Xuất kho → Đang giao → Đã giao → Thanh toán COD → Hoàn thành
Quản lý: Phiếu xuất kho, Vận chuyển, Thanh toán
Thống kê: Doanh thu, Sản phẩm bán chạy, Khách hàng VIP, Đơn hàng theo trạng thái
Phân quyền: Admin (toàn quyền) vs Nhân viên (hạn chế sửa/xóa/hủy)

Cấu trúc thư mục
├── Models/          # Các entity (SanPham, DonHang, KhachHang...)
├── DAL/             # Data Access Layer - Raw SQL với SqlDataReader
├── BLL/             # Business Logic Layer - Xử lý nghiệp vụ
├── Controllers/     # Controllers frontend
├── Views/           # Views frontend
├── Areas/Admin/     # Khu vực quản trị
│   ├── Controllers/
│   ├── Views/
│   └── Filters/     # AdminAuthFilter
├── wwwroot/         # CSS, JS, Images
└── Scripts/         # SQL scripts hỗ trợ

Stored Procedures chính
sp_NV_XuatKho - Xuất kho, tạo phiếu xuất
sp_NV_DieuPhoiGiaoHang - Điều phối vận chuyển
sp_NV_XacNhanThanhToan - Xác nhận thanh toán COD
sp_HuyDonHang - Hủy đơn và hoàn tồn kho
