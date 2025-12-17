-- ============================================
-- GÁN TẤT CẢ SẢN PHẨM VÀO TẤT CẢ MỤC ĐÍCH SỬ DỤNG
-- ============================================

-- Xóa dữ liệu cũ
DELETE FROM SUDUNG;

-- Gán tất cả sản phẩm vào tất cả mục đích
INSERT INTO SUDUNG (MAMDSD, MASP)
SELECT md.MAMDSD, sp.MASP
FROM MUC_DICH_SU_DUNG md
CROSS JOIN SAN_PHAM sp;

-- Kiểm tra
SELECT md.TENMDSD, COUNT(sd.MASP) as SoSanPham
FROM MUC_DICH_SU_DUNG md
LEFT JOIN SUDUNG sd ON md.MAMDSD = sd.MAMDSD
GROUP BY md.MAMDSD, md.TENMDSD
ORDER BY md.MAMDSD;
