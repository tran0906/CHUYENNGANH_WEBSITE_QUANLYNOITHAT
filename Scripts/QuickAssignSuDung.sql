-- ============================================
-- SCRIPT NHANH: GÁN TẤT CẢ SẢN PHẨM VÀO CÁC MỤC ĐÍCH SỬ DỤNG
-- ============================================

-- Xem dữ liệu hiện tại
SELECT 'MUC_DICH_SU_DUNG' as Bang, COUNT(*) as SoLuong FROM MUC_DICH_SU_DUNG
UNION ALL
SELECT 'SAN_PHAM', COUNT(*) FROM SAN_PHAM
UNION ALL
SELECT 'SUDUNG', COUNT(*) FROM SUDUNG;

-- Xem danh sách mục đích sử dụng
SELECT * FROM MUC_DICH_SU_DUNG;

-- Xem danh sách sản phẩm
SELECT MASP, TENSP FROM SAN_PHAM;

-- ============================================
-- XÓA DỮ LIỆU CŨ VÀ GÁN LẠI
-- ============================================
DELETE FROM SUDUNG;

-- Gán TẤT CẢ sản phẩm vào TẤT CẢ mục đích sử dụng
-- (Mỗi sản phẩm sẽ xuất hiện trong tất cả danh mục)
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT md.MAMDSD, sp.MASP
FROM MUC_DICH_SU_DUNG md
CROSS JOIN SAN_PHAM sp;

-- Kiểm tra kết quả
SELECT md.TENMDSD as [Mục đích sử dụng], COUNT(sd.MASP) as [Số sản phẩm]
FROM MUC_DICH_SU_DUNG md
LEFT JOIN SUDUNG sd ON md.MAMDSD = sd.MAMDSD
GROUP BY md.MAMDSD, md.TENMDSD
ORDER BY md.MAMDSD;

PRINT N'Đã gán xong! Mỗi mục đích sử dụng sẽ hiển thị tất cả sản phẩm.';
