-- Script cập nhật dữ liệu bảng MUC_DICH_SU_DUNG
-- Xóa dữ liệu cũ (Phòng khách, Phòng ngủ...) và thêm dữ liệu mới

-- Bước 1: Xóa dữ liệu liên quan trong bảng SUDUNG trước (nếu có)
DELETE FROM SUDUNG;

-- Bước 2: Xóa dữ liệu cũ trong MUC_DICH_SU_DUNG
DELETE FROM MUC_DICH_SU_DUNG;

-- Bước 3: Thêm dữ liệu mới theo yêu cầu
INSERT INTO MUC_DICH_SU_DUNG (MAMDSD, TENMDSD, MOTAMDSD) VALUES 
('MDSD01', N'Đồ trang trí nội thất', N'Các sản phẩm trang trí nội thất cao cấp'),
('MDSD02', N'Đồ decor trang trí phòng', N'Đồ decor trang trí phòng đẹp'),
('MDSD03', N'Quà tặng tân gia cao cấp', N'Quà tặng dịp tân gia, nhà mới'),
('MDSD04', N'Quà tặng khai trương', N'Quà tặng dịp khai trương cửa hàng, công ty'),
('MDSD05', N'Quà tặng sinh nhật', N'Quà tặng sinh nhật ý nghĩa'),
('MDSD06', N'Quà tặng sếp cao cấp', N'Quà tặng sếp, cấp trên sang trọng'),
('MDSD07', N'Quà tặng đối tác D.Nghiệp', N'Quà tặng đối tác doanh nghiệp'),
('MDSD08', N'Quà cưới - Kỷ niệm ngày cưới', N'Quà cưới và kỷ niệm ngày cưới');

-- Kiểm tra kết quả
SELECT * FROM MUC_DICH_SU_DUNG ORDER BY MAMDSD;
