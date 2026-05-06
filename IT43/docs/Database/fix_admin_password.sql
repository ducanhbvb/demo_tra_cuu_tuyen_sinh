-- ============================================================
-- FIX: Reset tài khoản Admin về trạng thái sẵn sàng đăng nhập
-- Mật khẩu: Admin@123
-- Hash: SHA256('Admin@123' + 'TCTS@2026#Salt!') -> Base64
-- ============================================================

USE [TraCuuTuyenSinh]
GO

SET ARITHABORT ON
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- 1. Kiểm tra tài khoản Admin hiện tại
SELECT Email, LEN(MatKhau) AS ChieuDaiHash, MatKhau,
       SoLanDangNhapSai, KhoaTaiKhoanDen, EmailDaXacNhan, TrangThai
FROM tbl_TaiKhoan
WHERE Email = 'admin@tracuutuyensinh.vn'
GO

-- 2. Cập nhật Hash đúng + gỡ khoá + đặt lại số lần sai
UPDATE tbl_TaiKhoan
SET 
    MatKhau           = N'I86KnRU0/t5JTfZRvtQhO38QgbnPCRUYWvXHY0c79qc=',
    SoLanDangNhapSai  = 0,
    KhoaTaiKhoanDen   = NULL,
    EmailDaXacNhan    = 1,
    TrangThai         = 1
WHERE Email = 'admin@tracuutuyensinh.vn'
GO

-- 3. Xác nhận lại kết quả sau update
SELECT Email, LEN(MatKhau) AS ChieuDaiHash, MatKhau,
       SoLanDangNhapSai, KhoaTaiKhoanDen, EmailDaXacNhan, TrangThai
FROM tbl_TaiKhoan
WHERE Email = 'admin@tracuutuyensinh.vn'
GO

PRINT '=== XONG! Đăng nhập bằng: admin@tracuutuyensinh.vn / Admin@123 ===
'
