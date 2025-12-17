-- ============================================
-- SCRIPT TỰ ĐỘNG GÁN SẢN PHẨM VÀO MỤC ĐÍCH SỬ DỤNG
-- ============================================

-- Bước 1: Xem danh sách mục đích sử dụng hiện có
SELECT * FROM MUC_DICH_SU_DUNG ORDER BY MAMDSD;

-- Bước 2: Xem danh sách sản phẩm hiện có
SELECT MASP, TENSP FROM SAN_PHAM ORDER BY MASP;

-- Bước 3: Xóa dữ liệu SUDUNG cũ (nếu muốn làm lại từ đầu)
-- DELETE FROM SUDUNG;

-- ============================================
-- GÁN TẤT CẢ SẢN PHẨM VÀO CÁC MỤC ĐÍCH
-- Mỗi sản phẩm sẽ được gán vào nhiều mục đích phù hợp
-- ============================================

-- Lấy danh sách MAMDSD từ database
DECLARE @MDSD_DoTrangTri NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%trang trí nội thất%');
DECLARE @MDSD_Decor NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%decor%');
DECLARE @MDSD_TanGia NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%tân gia%');
DECLARE @MDSD_KhaiTruong NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%khai trương%');
DECLARE @MDSD_SinhNhat NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%sinh nhật%');
DECLARE @MDSD_Sep NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%sếp%');
DECLARE @MDSD_DoiTac NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%đối tác%');
DECLARE @MDSD_Cuoi NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%cưới%');
DECLARE @MDSD_Tet NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%tết%');
DECLARE @MDSD_DoanhNghiep NVARCHAR(20) = (SELECT TOP 1 MAMDSD FROM MUC_DICH_SU_DUNG WHERE TENMDSD LIKE N'%doanh nghiệp%');

-- Hiển thị các mã đã tìm được
SELECT 'DoTrangTri' as MucDich, @MDSD_DoTrangTri as Ma
UNION SELECT 'Decor', @MDSD_Decor
UNION SELECT 'TanGia', @MDSD_TanGia
UNION SELECT 'KhaiTruong', @MDSD_KhaiTruong
UNION SELECT 'SinhNhat', @MDSD_SinhNhat
UNION SELECT 'Sep', @MDSD_Sep
UNION SELECT 'DoiTac', @MDSD_DoiTac
UNION SELECT 'Cuoi', @MDSD_Cuoi
UNION SELECT 'Tet', @MDSD_Tet
UNION SELECT 'DoanhNghiep', @MDSD_DoanhNghiep;

-- ============================================
-- GÁN SẢN PHẨM THEO LOẠI
-- ============================================

-- 1. Đồ trang trí nội thất: Bàn, Ghế, Tủ, Kệ, Giường
IF @MDSD_DoTrangTri IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_DoTrangTri, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Bàn%' OR TENSP LIKE N'%Ghế%' OR TENSP LIKE N'%Tủ%' OR TENSP LIKE N'%Kệ%' OR TENSP LIKE N'%Giường%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_DoTrangTri);
END

-- 2. Đồ decor trang trí phòng: Đèn, Thảm, Gương, Decor
IF @MDSD_Decor IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_Decor, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Đèn%' OR TENSP LIKE N'%Thảm%' OR TENSP LIKE N'%Gương%' OR TENSP LIKE N'%Decor%' OR TENSP LIKE N'%trang trí%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_Decor);
END

-- 3. Quà tặng tân gia: Tủ, Bàn, Ghế, Sofa
IF @MDSD_TanGia IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_TanGia, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Tủ%' OR TENSP LIKE N'%Bàn%' OR TENSP LIKE N'%Ghế%' OR TENSP LIKE N'%Sofa%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_TanGia);
END

-- 4. Quà tặng khai trương: Tất cả sản phẩm (phù hợp làm quà khai trương)
IF @MDSD_KhaiTruong IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_KhaiTruong, MASP FROM SAN_PHAM 
    WHERE MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_KhaiTruong);
END

-- 5. Quà tặng sinh nhật: Đèn, Gương, Decor, Thảm
IF @MDSD_SinhNhat IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_SinhNhat, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Đèn%' OR TENSP LIKE N'%Gương%' OR TENSP LIKE N'%Decor%' OR TENSP LIKE N'%Thảm%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_SinhNhat);
END

-- 6. Quà tặng sếp cao cấp: Bàn, Ghế, Kệ sách
IF @MDSD_Sep IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_Sep, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Bàn%' OR TENSP LIKE N'%Ghế%' OR TENSP LIKE N'%Kệ%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_Sep);
END

-- 7. Quà tặng đối tác D.Nghiệp: Đèn, Gương, Decor
IF @MDSD_DoiTac IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_DoiTac, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Đèn%' OR TENSP LIKE N'%Gương%' OR TENSP LIKE N'%Decor%' OR TENSP LIKE N'%Bàn%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_DoiTac);
END

-- 8. Quà cưới - Kỷ niệm ngày cưới: Đèn, Gương, Decor, Thảm
IF @MDSD_Cuoi IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_Cuoi, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Đèn%' OR TENSP LIKE N'%Gương%' OR TENSP LIKE N'%Decor%' OR TENSP LIKE N'%Thảm%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_Cuoi);
END

-- 9. Quà tặng lễ Tết: Tất cả sản phẩm
IF @MDSD_Tet IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_Tet, MASP FROM SAN_PHAM 
    WHERE MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_Tet);
END

-- 10. Quà tặng doanh nghiệp: Bàn, Ghế, Kệ, Đèn
IF @MDSD_DoanhNghiep IS NOT NULL
BEGIN
    INSERT INTO SUDUNG (MAMDSD, MASP)
    SELECT @MDSD_DoanhNghiep, MASP FROM SAN_PHAM 
    WHERE (TENSP LIKE N'%Bàn%' OR TENSP LIKE N'%Ghế%' OR TENSP LIKE N'%Kệ%' OR TENSP LIKE N'%Đèn%')
    AND MASP NOT IN (SELECT MASP FROM SUDUNG WHERE MAMDSD = @MDSD_DoanhNghiep);
END

-- ============================================
-- KIỂM TRA KẾT QUẢ
-- ============================================
PRINT N'=== KẾT QUẢ GÁN SẢN PHẨM ===';

SELECT md.TENMDSD as [Mục đích sử dụng], COUNT(sd.MASP) as [Số sản phẩm]
FROM MUC_DICH_SU_DUNG md
LEFT JOIN SUDUNG sd ON md.MAMDSD = sd.MAMDSD
GROUP BY md.MAMDSD, md.TENMDSD
ORDER BY md.MAMDSD;

-- Xem chi tiết
SELECT md.TENMDSD as [Mục đích], sp.MASP as [Mã SP], sp.TENSP as [Tên sản phẩm]
FROM SUDUNG sd
JOIN MUC_DICH_SU_DUNG md ON sd.MAMDSD = md.MAMDSD
JOIN SAN_PHAM sp ON sd.MASP = sp.MASP
ORDER BY md.MAMDSD, sp.MASP;
