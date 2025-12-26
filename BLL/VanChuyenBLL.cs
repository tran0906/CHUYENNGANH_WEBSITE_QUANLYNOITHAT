using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class VanChuyenBLL
    {
        private readonly VanChuyenDAL _dal = new VanChuyenDAL();
        private readonly DonHangDAL _donHangDAL = new DonHangDAL();

        public List<VanChuyen> GetAll() => _dal.GetAll();
        public List<VanChuyen> GetListByDonHang(string maDh) => _dal.GetByDonHang(maDh);
        
        // Lấy vận chuyển theo mã đơn hàng (trả về 1 record)
        public VanChuyen? GetByDonHang(string maDh)
        {
            if (string.IsNullOrEmpty(maDh)) return null;
            return _dal.GetByDonHang(maDh).FirstOrDefault();
        }

        public VanChuyen? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(VanChuyen obj)
        {
            if (string.IsNullOrEmpty(obj.Mavandon))
                obj.Mavandon = _dal.GenerateNewId();
            if (string.IsNullOrEmpty(obj.Madonhang))
                return (false, "Vui lòng chọn đơn hàng");

            // Kiểm tra đơn hàng tồn tại
            var donHang = _donHangDAL.GetById(obj.Madonhang);
            if (donHang == null)
                return (false, "Đơn hàng không tồn tại");

            // Kiểm tra đơn hàng đã có vận chuyển chưa
            var existingVC = GetByDonHang(obj.Madonhang);
            if (existingVC != null)
                return (false, $"Đơn hàng {obj.Madonhang} đã có phiếu vận chuyển {existingVC.Mavandon}");

            // Kiểm tra trạng thái đơn hàng phù hợp để tạo vận chuyển
            var validStatuses = new[] { "Đã xác nhận", "Đang xử lý" };
            if (!validStatuses.Contains(donHang.Trangthai))
                return (false, $"Chỉ tạo vận chuyển cho đơn hàng ở trạng thái 'Đã xác nhận' hoặc 'Đang xử lý'. Trạng thái hiện tại: {donHang.Trangthai}");

            var result = _dal.Insert(obj);
            if (result > 0)
            {
                // Tự động cập nhật trạng thái đơn hàng sang "Đang giao"
                if (obj.Trangthaigiao == "Đang giao")
                {
                    donHang.Trangthai = "Đang giao";
                    _donHangDAL.Update(donHang);
                }
                return (true, "Thêm vận chuyển thành công");
            }
            return (false, "Có lỗi xảy ra khi thêm vận chuyển");
        }

        public (bool Success, string Message) Update(VanChuyen obj)
        {
            if (string.IsNullOrEmpty(obj.Mavandon))
                return (false, "Mã vận đơn không được để trống");
            if (!_dal.Exists(obj.Mavandon))
                return (false, "Vận chuyển không tồn tại");

            var result = _dal.Update(obj);
            if (result > 0)
            {
                // Tự động cập nhật trạng thái đơn hàng theo trạng thái giao
                if (!string.IsNullOrEmpty(obj.Madonhang))
                {
                    var donHang = _donHangDAL.GetById(obj.Madonhang);
                    if (donHang != null)
                    {
                        var newStatus = obj.Trangthaigiao switch
                        {
                            "Đang giao" => "Đang giao",
                            "Đã giao" => "Đã giao",
                            "Thất bại" => "Đã xác nhận", // Giao thất bại -> quay lại trạng thái chờ giao lại
                            _ => donHang.Trangthai
                        };
                        
                        if (donHang.Trangthai != newStatus)
                        {
                            donHang.Trangthai = newStatus;
                            _donHangDAL.Update(donHang);
                        }
                    }
                }
                return (true, "Cập nhật vận chuyển thành công");
            }
            return (false, "Có lỗi xảy ra khi cập nhật");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");
            
            var vanChuyen = _dal.GetById(ma);
            if (vanChuyen == null)
                return (false, "Vận chuyển không tồn tại");

            // Không cho xóa nếu đã giao thành công
            if (vanChuyen.Trangthaigiao == "Đã giao")
                return (false, "Không thể xóa vận chuyển đã giao thành công");

            try
            {
                var result = _dal.Delete(ma);
                if (result > 0)
                {
                    // Cập nhật lại trạng thái đơn hàng nếu cần
                    if (!string.IsNullOrEmpty(vanChuyen.Madonhang))
                    {
                        var donHang = _donHangDAL.GetById(vanChuyen.Madonhang);
                        if (donHang != null && donHang.Trangthai == "Đang giao")
                        {
                            donHang.Trangthai = "Đã xác nhận";
                            _donHangDAL.Update(donHang);
                        }
                    }
                    return (true, "Xóa vận chuyển thành công");
                }
                return (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa vận chuyển này vì đang có dữ liệu liên quan");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
        public string GenerateNewId() => _dal.GenerateNewId();
        
        // Lấy danh sách đơn hàng có thể tạo vận chuyển
        public List<DonHang> GetDonHangCanVanChuyen()
        {
            var allDonHang = _donHangDAL.GetAll();
            var validStatuses = new[] { "Đã xác nhận", "Đang xử lý" };
            
            return allDonHang.Where(dh => 
                validStatuses.Contains(dh.Trangthai) && 
                GetByDonHang(dh.Madonhang) == null // Chưa có vận chuyển
            ).ToList();
        }
    }
}
