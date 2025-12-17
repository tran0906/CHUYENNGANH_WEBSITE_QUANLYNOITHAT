-- Script gán sản phẩm vào các mục đích sử dụng
-- Chạy script này trong SQL Server Management Studio

USE WEB_NOITHAT;
GO

-- Xem danh sách mục đích sử dụng hiện có
SELECT * FROM MUC_DICH_SU_DUNG;

-- Xem danh sách sản phẩm hiện có
SELECT MASP, TENSP, MANHOMSP FROM SAN_PHAM;

-- Xem dữ liệu hiện có trong bảng SUDUNG
SELECT * FROM SUDUNG;

-- =====================================================
-- GÁN SẢN PHẨM VÀO MỤC ĐÍCH SỬ DỤNG
-- =====================================================

-- Xóa dữ liệu cũ (nếu muốn làm lại từ đầu)
-- DELETE FROM SUDUNG;

-- Gán tất cả sản phẩm vào tất cả mục đích sử dụng (để test)
-- Bạn có thể tùy chỉnh theo nhu cầu

INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT md.MAMDSD, sp.MASP
FROM MUC_DICH_SU_DUNG md
CROSS JOIN SAN_PHAM sp
WHERE NOT EXISTS (
    SELECT 1 FROM SUDUNG s 
    WHERE s.MAMDSD = md.MAMDSD AND s.MASP = sp.MASP
);

-- Kiểm tra kết quả
SELECT 
    md.TENMDSD as 'Mục đích sử dụng',
    COUNT(s.MASP) as 'Số sản phẩm'
FROM MUC_DICH_SU_DUNG md
LEFT JOIN SUDUNG s ON md.MAMDSD = s.MAMDSD
GROUP BY md.MAMDSD, md.TENMDSD
ORDER BY md.MAMDSD;

-- Xem chi tiết sản phẩm theo mục đích sử dụng
SELECT 
    md.TENMDSD as 'Mục đích',
    sp.MASP,
    sp.TENSP as 'Tên sản phẩm'
FROM SUDUNG s
INNER JOIN MUC_DICH_SU_DUNG md ON s.MAMDSD = md.MAMDSD
INNER JOIN SAN_PHAM sp ON s.MASP = sp.MASP
ORDER BY md.MAMDSD, sp.MASP;
