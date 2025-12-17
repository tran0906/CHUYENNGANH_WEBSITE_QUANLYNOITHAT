-- Script gán sản phẩm vào các mục đích sử dụng
-- Bảng SUDUNG liên kết SAN_PHAM với MUC_DICH_SU_DUNG

-- Xem danh sách mục đích sử dụng hiện có
SELECT * FROM MUC_DICH_SU_DUNG;

-- Xem danh sách sản phẩm hiện có
SELECT MASP, TENSP FROM SAN_PHAM;

-- Xem dữ liệu SUDUNG hiện tại
SELECT * FROM SUDUNG;

-- ============================================
-- GÁN SẢN PHẨM VÀO CÁC MỤC ĐÍCH SỬ DỤNG
-- ============================================

-- Lưu ý: Thay đổi MASP và MAMDSD theo dữ liệu thực tế trong database của bạn
-- Ví dụ: Nếu MAMDSD của "Quà tặng khai trương" là 'MDSD04' và bạn có sản phẩm SP001, SP002...

-- Cách 1: Gán từng sản phẩm một
-- INSERT INTO SUDUNG (MAMDSD, MASP) VALUES ('MDSD01', 'SP001');
-- INSERT INTO SUDUNG (MAMDSD, MASP) VALUES ('MDSD01', 'SP002');

-- Cách 2: Gán nhiều sản phẩm cùng lúc cho 1 mục đích
-- Ví dụ: Gán tất cả sản phẩm có chữ "Bàn" vào "Đồ trang trí nội thất" (MDSD01)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD01', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Bàn%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD01' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Ghế" vào "Đồ trang trí nội thất"
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD01', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Ghế%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD01' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Đèn" vào "Đồ decor trang trí phòng" (MDSD02)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD02', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Đèn%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD02' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Tủ" vào "Quà tặng tân gia cao cấp" (MDSD03)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD03', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Tủ%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD03' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Sofa" vào "Quà tặng khai trương" (MDSD04)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD04', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Sofa%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD04' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Kệ" vào "Quà tặng sinh nhật" (MDSD05)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD05', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Kệ%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD05' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Giường" vào "Quà tặng sếp cao cấp" (MDSD06)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD06', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Giường%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD06' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Thảm" hoặc "Gương" vào "Quà tặng đối tác D.Nghiệp" (MDSD07)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD07', MASP FROM SAN_PHAM WHERE (TENSP LIKE N'%Thảm%' OR TENSP LIKE N'%Gương%')
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD07' AND MASP = SAN_PHAM.MASP);

-- Gán sản phẩm có chữ "Decor" vào "Quà cưới - Kỷ niệm ngày cưới" (MDSD08)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT 'MDSD08', MASP FROM SAN_PHAM WHERE TENSP LIKE N'%Decor%'
AND NOT EXISTS (SELECT 1 FROM SUDUNG WHERE MAMDSD = 'MDSD08' AND MASP = SAN_PHAM.MASP);

-- ============================================
-- KIỂM TRA KẾT QUẢ
-- ============================================
SELECT md.TENMDSD, COUNT(sd.MASP) as SoSanPham
FROM MUC_DICH_SU_DUNG md
LEFT JOIN SUDUNG sd ON md.MAMDSD = sd.MAMDSD
GROUP BY md.MAMDSD, md.TENMDSD
ORDER BY md.MAMDSD;

-- Xem chi tiết sản phẩm theo từng mục đích
SELECT md.TENMDSD, sp.TENSP
FROM SUDUNG sd
JOIN MUC_DICH_SU_DUNG md ON sd.MAMDSD = md.MAMDSD
JOIN SAN_PHAM sp ON sd.MASP = sp.MASP
ORDER BY md.MAMDSD, sp.TENSP;
