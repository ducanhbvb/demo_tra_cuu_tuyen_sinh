-- ============================================================
-- FILE: sample_data.sql
-- MỤC ĐÍCH: Dữ liệu mẫu để test trang web TraCuuTuyenSinh
-- CÁCH DÙNG: Mở SSMS → chạy file này SAU KHI đã chạy db.sql
-- CẬP NHẬT  : 2026-03-25
-- ============================================================

USE [TraCuuTuyenSinh]
GO
SET ARITHABORT ON
SET QUOTED_IDENTIFIER ON
SET NOCOUNT ON
GO

-- ============================================================
-- 1. QUYỀN HẠN (nếu chưa có)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_Quyen WHERE MaQuyen = 1)
BEGIN
    SET IDENTITY_INSERT tbl_Quyen ON
    INSERT INTO tbl_Quyen (MaQuyen, TenQuyen) VALUES (1,N'Admin'),(2,N'TruongHoc'),(3,N'HocSinh')
    SET IDENTITY_INSERT tbl_Quyen OFF
END
GO

-- ============================================================
-- 2. TÀI KHOẢN MẪU
-- ============================================================
-- Admin (Admin@123)
IF NOT EXISTS (SELECT 1 FROM tbl_TaiKhoan WHERE Email='admin@tracuutuyensinh.vn')
    INSERT INTO tbl_TaiKhoan (Email,MatKhau,MaQuyen,TrangThai,EmailDaXacNhan)
    VALUES (N'admin@tracuutuyensinh.vn',N'I86KnRU0/t5JTfZRvtQhO38QgbnPCRUYWvXHY0c79qc=',1,1,1)

-- Tài khoản Trường học (truong01 / Test@123)
IF NOT EXISTS (SELECT 1 FROM tbl_TaiKhoan WHERE Email='truong01@test.com')
    INSERT INTO tbl_TaiKhoan (Email,MatKhau,MaQuyen,TrangThai,EmailDaXacNhan)
    VALUES (N'truong01@test.com',N'I86KnRU0/t5JTfZRvtQhO38QgbnPCRUYWvXHY0c79qc=',2,1,1)

-- Tài khoản Học sinh (hocsinh01 / Test@123)
IF NOT EXISTS (SELECT 1 FROM tbl_TaiKhoan WHERE Email='hocsinh01@test.com')
    INSERT INTO tbl_TaiKhoan (Email,MatKhau,MaQuyen,TrangThai,EmailDaXacNhan)
    VALUES (N'hocsinh01@test.com',N'I86KnRU0/t5JTfZRvtQhO38QgbnPCRUYWvXHY0c79qc=',3,1,1)
GO

-- ============================================================
-- 3. CẤP BẬC
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_CapBac)
BEGIN
    SET IDENTITY_INSERT tbl_CapBac ON
    INSERT INTO tbl_CapBac (MaCapBac,TenCapBac) VALUES
        (1,N'Đại học'), (2,N'Cao đẳng'), (3,N'Liên thông'), (4,N'Thạc sĩ')
    SET IDENTITY_INSERT tbl_CapBac OFF
END
GO

-- ============================================================
-- 4. DANH MỤC NGÀNH
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_DanhMucNganh)
BEGIN
    SET IDENTITY_INSERT tbl_DanhMucNganh ON
    INSERT INTO tbl_DanhMucNganh (MaDanhMuc,TenDanhMuc,ThuTu) VALUES
        (1,N'Kỹ thuật - Công nghệ',1),
        (2,N'Kinh tế - Quản trị',2),
        (3,N'Y - Dược - Sức khỏe',3),
        (4,N'Khoa học Xã hội & Nhân văn',4),
        (5,N'Nghệ thuật - Thiết kế',5),
        (6,N'Nông - Lâm - Ngư nghiệp',6)
    SET IDENTITY_INSERT tbl_DanhMucNganh OFF
END
GO

-- ============================================================
-- 5. CHUYÊN NGÀNH
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_ChuyenNganh)
BEGIN
    SET IDENTITY_INSERT tbl_ChuyenNganh ON
    INSERT INTO tbl_ChuyenNganh (MaChuyenNganh,TenChuyenNganh,MaDanhMuc) VALUES
        -- Kỹ thuật - Công nghệ
        (1, N'Công nghệ Thông tin', 1),
        (2, N'Kỹ thuật Phần mềm',  1),
        (3, N'Trí tuệ Nhân tạo',   1),
        (4, N'An toàn Thông tin',   1),
        (5, N'Kỹ thuật Điện tử - Viễn thông', 1),
        (6, N'Kỹ thuật Cơ điện tử', 1),
        (7, N'Xây dựng Dân dụng & Công nghiệp', 1),
        (8, N'Kiến trúc', 1),
        -- Kinh tế - Quản trị
        (9, N'Quản trị Kinh doanh', 2),
        (10,N'Kế toán', 2),
        (11,N'Tài chính - Ngân hàng', 2),
        (12,N'Marketing', 2),
        (13,N'Logistics & Quản lý Chuỗi cung ứng', 2),
        (14,N'Thương mại Điện tử', 2),
        -- Y - Dược
        (15,N'Y đa khoa', 3),
        (16,N'Dược học', 3),
        (17,N'Điều dưỡng', 3),
        (18,N'Răng Hàm Mặt', 3),
        -- KHXH & NV
        (19,N'Luật', 4),
        (20,N'Ngôn ngữ Anh', 4),
        (21,N'Báo chí', 4),
        (22,N'Tâm lý học', 4),
        -- Nghệ thuật
        (23,N'Thiết kế Đồ họa', 5),
        (24,N'Thiết kế thời trang', 5),
        -- Nông - Lâm
        (25,N'Nông nghiệp công nghệ cao', 6)
    SET IDENTITY_INSERT tbl_ChuyenNganh OFF
END
GO

-- ============================================================
-- 6. PHƯƠNG THỨC XÉT TUYỂN
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_PhuongThucXetTuyen)
BEGIN
    SET IDENTITY_INSERT tbl_PhuongThucXetTuyen ON
    INSERT INTO tbl_PhuongThucXetTuyen (MaPhuongThuc,TenPhuongThuc,MoTa,ThuTu) VALUES
        (1,N'Điểm thi THPT Quốc gia', N'Xét tuyển dựa trên kết quả kỳ thi THPT Quốc gia', 1),
        (2,N'Xét học bạ THPT',        N'Xét tuyển dựa trên điểm trung bình học bạ THPT',  2),
        (3,N'Đánh giá năng lực (ĐGNL)',N'Xét tuyển dựa trên bài thi ĐGNL của ĐHQG',        3),
        (4,N'Xét tuyển thẳng / Ưu tiên',N'Học sinh giỏi quốc gia, HSG tỉnh, Huy chương quốc tế', 4)
    SET IDENTITY_INSERT tbl_PhuongThucXetTuyen OFF
END
GO

-- ============================================================
-- 7. TRƯỜNG ĐẠI HỌC (dùng MaTaiKhoan=1 = Admin)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_Truong)
BEGIN
    INSERT INTO tbl_Truong (TenTruong,DiaChi,TinhThanh,MaVung,LoaiTruong,SoDienThoai,Website,MoTa,QuyMo,KiemDinhChatLuong,Slug,MaTaiKhoan) VALUES
    -- MIỀN BẮC
    (N'Đại học Bách khoa Hà Nội',
     N'Số 1 Đại Cồ Việt, Hai Bà Trưng, Hà Nội',
     N'Hà Nội',1,1,N'024 3869 2008',N'https://hust.edu.vn',
     N'Trường Đại học Bách khoa Hà Nội là trường đại học kỹ thuật hàng đầu Việt Nam, được thành lập năm 1956. Trường đào tạo các ngành kỹ thuật, công nghệ và khoa học ứng dụng.',
     N'Hơn 32.000 sinh viên',1,N'bach-khoa-ha-noi',1),

    (N'Đại học Quốc gia Hà Nội',
     N'144 Xuân Thủy, Cầu Giấy, Hà Nội',
     N'Hà Nội',1,1,N'024 3754 7670',N'https://vnu.edu.vn',
     N'Đại học Quốc gia Hà Nội là hệ thống đại học trọng điểm quốc gia, tập hợp nhiều trường đại học thành viên uy tín với chất lượng đào tạo cao.',
     N'Hơn 45.000 sinh viên',1,N'dai-hoc-quoc-gia-ha-noi',1),

    (N'Đại học Kinh tế Quốc dân',
     N'207 Giải Phóng, Hai Bà Trưng, Hà Nội',
     N'Hà Nội',1,1,N'024 3628 0280',N'https://neu.edu.vn',
     N'Trường Đại học Kinh tế Quốc dân (NEU) là đại học kinh tế và quản trị kinh doanh hàng đầu tại Việt Nam, đào tạo nguồn nhân lực kinh tế chất lượng cao.',
     N'Hơn 48.000 sinh viên',1,N'kinh-te-quoc-dan',1),

    (N'Đại học Y Hà Nội',
     N'1 Tôn Thất Tùng, Đống Đa, Hà Nội',
     N'Hà Nội',1,1,N'024 3852 6722',N'https://hmu.edu.vn',
     N'Trường Đại học Y Hà Nội là cơ sở đào tạo y khoa lớn và lâu đời nhất Việt Nam, đào tạo bác sĩ, dược sĩ, điều dưỡng chất lượng cao.',
     N'Hơn 10.000 sinh viên',1,N'y-ha-noi',1),

    (N'Học viện Công nghệ Bưu chính Viễn thông',
     N'Km10, Đường Nguyễn Trãi, Hà Đông, Hà Nội',
     N'Hà Nội',1,1,N'024 3755 2016',N'https://ptit.edu.vn',
     N'Học viện Công nghệ Bưu chính Viễn thông đào tạo các ngành về CNTT, điện tử, viễn thông và kinh tế.',
     N'Hơn 25.000 sinh viên',1,N'hoc-vien-buu-chinh-vien-thong',1),

    -- MIỀN TRUNG
    (N'Đại học Đà Nẵng',
     N'41 Lê Duẩn, Hải Châu, Đà Nẵng',
     N'Đà Nẵng',2,1,N'0236 382 2041',N'https://ud.edu.vn',
     N'Đại học Đà Nẵng là đại học vùng trọng điểm quốc gia tại miền Trung, đào tạo đa ngành với chất lượng cao và gắn kết doanh nghiệp.',
     N'Hơn 55.000 sinh viên',1,N'dai-hoc-da-nang',1),

    (N'Đại học Huế',
     N'3 Lê Lợi, Vĩnh Ninh, Thuận Hóa, Huế',
     N'Thừa Thiên Huế',2,1,N'0234 382 2735',N'https://hueuni.edu.vn',
     N'Đại học Huế là đại học vùng lớn tại miền Trung, có lịch sử hơn 60 năm đào tạo và nghiên cứu khoa học.',
     N'Hơn 40.000 sinh viên',1,N'dai-hoc-hue',1),

    -- MIỀN NAM
    (N'Đại học Bách khoa TP.HCM',
     N'268 Lý Thường Kiệt, Quận 10, TP.HCM',
     N'TP. Hồ Chí Minh',3,1,N'028 3865 4086',N'https://hcmut.edu.vn',
     N'Trường Đại học Bách khoa TP.HCM (HCMUT) là đại học kỹ thuật hàng đầu phía Nam, thành viên của Đại học Quốc gia TP.HCM.',
     N'Hơn 40.000 sinh viên',1,N'bach-khoa-tphcm',1),

    (N'Đại học Kinh tế TP.HCM',
     N'59C Nguyễn Đình Chiểu, Quận 3, TP.HCM',
     N'TP. Hồ Chí Minh',3,1,N'028 3829 8946',N'https://ueh.edu.vn',
     N'Trường Đại học Kinh tế TP.HCM (UEH) là đại học kinh tế hàng đầu phía Nam, đào tạo nguồn nhân lực kinh tế, quản trị kinh doanh.',
     N'Hơn 50.000 sinh viên',1,N'kinh-te-tphcm',1),

    (N'Đại học Y Dược TP.HCM',
     N'217 Hồng Bàng, Quận 5, TP.HCM',
     N'TP. Hồ Chí Minh',3,1,N'028 3855 4269',N'https://ump.edu.vn',
     N'Trường Đại học Y Dược TP.HCM là cơ sở đào tạo y tế lớn nhất miền Nam, đào tạo bác sĩ, dược sĩ và cán bộ y tế chất lượng cao.',
     N'Hơn 13.000 sinh viên',1,N'y-duoc-tphcm',1),

    (N'Đại học FPT',
     N'Khu Công nghệ cao Hòa Lạc, Thạch Thất, Hà Nội',
     N'Hà Nội',1,2,N'1900 6600',N'https://fpt.edu.vn',
     N'Đại học FPT là đại học tư thục theo mô hình tiên tiến, liên kết chặt chẽ với doanh nghiệp, đào tạo CNTT và kinh tế với 100% việc làm sau tốt nghiệp.',
     N'Hơn 20.000 sinh viên',1,N'dai-hoc-fpt',1),

    (N'Đại học RMIT Việt Nam',
     N'702 Nguyễn Văn Linh, Quận 7, TP.HCM',
     N'TP. Hồ Chí Minh',3,3,N'028 3776 1300',N'https://rmit.edu.vn',
     N'RMIT Việt Nam là cơ sở quốc tế của Đại học RMIT Australia, cung cấp chương trình đào tạo bằng tiếng Anh theo chuẩn quốc tế.',
     N'Hơn 7.000 sinh viên',1,N'rmit-viet-nam',1)
END
GO

-- ============================================================
-- 8. TRƯỜNG - CHUYÊN NGÀNH (liên kết)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_TruongChuyenNganh)
BEGIN
    INSERT INTO tbl_TruongChuyenNganh (MaTruong,MaChuyenNganh,MaCapBac,ChiTieu) VALUES
    -- Bách khoa HN (1)
    (1,1,1,350),(1,2,1,300),(1,3,1,150),(1,4,1,100),(1,5,1,200),(1,6,1,180),(1,7,1,250),(1,8,1,100),
    -- ĐHQG HN (2)
    (2,1,1,200),(2,2,1,150),(2,19,1,180),(2,20,1,200),(2,22,1,120),
    -- KTQD (3)
    (3,9,1,500),(3,10,1,450),(3,11,1,400),(3,12,1,350),(3,13,1,200),(3,14,1,150),
    -- Y HN (4)
    (4,15,1,200),(4,16,1,150),(4,17,1,250),(4,18,1,100),
    -- PTIT (5)
    (5,1,1,400),(5,2,1,300),(5,5,1,250),(5,13,1,150),(5,14,1,100),
    -- ĐH Đà Nẵng (6)
    (6,1,1,300),(6,2,1,200),(6,7,1,200),(6,9,1,300),(6,20,1,150),
    -- ĐH Huế (7)
    (7,15,1,150),(7,16,1,100),(7,9,1,200),(7,20,1,180),(7,25,1,120),
    -- Bách khoa TPHCM (8)
    (8,1,1,400),(8,2,1,350),(8,3,1,200),(8,5,1,250),(8,7,1,300),(8,8,1,150),
    -- KTQD TPHCM (9)
    (9,9,1,600),(9,10,1,500),(9,11,1,450),(9,12,1,400),(9,13,1,250),(9,14,1,200),
    -- Y Dược TPHCM (10)
    (10,15,1,200),(10,16,1,200),(10,17,1,300),(10,18,1,120),
    -- FPT (11)
    (11,1,1,2000),(11,2,1,1500),(11,3,1,500),(11,4,1,300),(11,9,1,600),(11,12,1,400),(11,14,1,300),
    -- RMIT (12)
    (12,9,1,500),(12,12,1,400),(12,23,1,200),(12,14,1,300),(12,1,1,300)
END
GO

-- ============================================================
-- 9. TIN TUYỂN SINH (điểm chuẩn các năm)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_TinTuyenSinh)
BEGIN
    -- Bách khoa HN - CNTT - Thi THPT
    INSERT INTO tbl_TinTuyenSinh (MaTruong,MaChuyenNganh,MaPhuongThuc,NamTuyenSinh,ChiTieu,HocPhi,ToHopMonHoc,DiemChuanNamTruoc,DiemChuanNamNay,TrangThai)
    VALUES
    (1,1,1,2024,350,23500000,N'A00,A01',28.15,28.45,1),
    (1,2,1,2024,300,23500000,N'A00,A01',27.80,28.20,1),
    (1,3,1,2024,150,25000000,N'A00,A01',29.50,29.70,1),
    (1,4,1,2024,100,24000000,N'A00,A01',28.00,28.30,1),
    (1,5,1,2024,200,22000000,N'A00,A01',25.50,25.85,1),
    (1,7,1,2024,250,21000000,N'A00,A01',23.50,24.00,1),
    (1,8,1,2024,100,22000000,N'A00,V00',24.00,24.50,1),

    -- ĐHQG HN - CNTT
    (2,1,1,2024,200,24000000,N'A00,A01,D01',28.50,28.80,1),
    (2,19,1,2024,180,21000000,N'C00,D01',27.00,27.25,1),
    (2,20,1,2024,200,21000000,N'D01,D14',28.20,28.50,1),

    -- KTQD
    (3,9,1,2024,500,22000000,N'A00,A01,D01',27.50,27.80,1),
    (3,10,1,2024,450,20000000,N'A00,A01,D01',26.80,27.10,1),
    (3,11,1,2024,400,21000000,N'A00,A01,D01',27.20,27.40,1),
    (3,12,1,2024,350,22000000,N'A00,A01,D01',27.00,27.25,1),
    (3,13,1,2024,200,21000000,N'A00,A01,D01',25.50,25.80,1),

    -- Y Hà Nội (điểm cao nhất)
    (4,15,1,2024,200,6200000,N'B00',29.40,29.65,1),
    (4,16,1,2024,150,12000000,N'B00,A00',28.50,28.75,1),
    (4,17,1,2024,250,9000000,N'B00',23.50,24.00,1),
    (4,18,1,2024,100,8000000,N'B00',29.00,29.20,1),

    -- PTIT
    (5,1,1,2024,400,15500000,N'A00,A01,D01',25.50,25.80,1),
    (5,2,1,2024,300,15500000,N'A00,A01',25.00,25.30,1),
    (5,5,1,2024,250,15000000,N'A00,A01',23.50,24.00,1),

    -- Bách khoa TPHCM
    (8,1,1,2024,400,24000000,N'A00,A01',27.50,27.80,1),
    (8,2,1,2024,350,24000000,N'A00,A01',27.00,27.35,1),
    (8,3,1,2024,200,26000000,N'A00,A01',29.00,29.30,1),
    (8,7,1,2024,300,22000000,N'A00',23.50,24.20,1),

    -- KTQD TPHCM
    (9,9,1,2024,600,24000000,N'A00,A01,D01',26.50,26.80,1),
    (9,10,1,2024,500,22000000,N'A00,A01,D01',26.00,26.35,1),
    (9,11,1,2024,450,23000000,N'A00,A01,D01',26.30,26.60,1),
    (9,12,1,2024,400,24000000,N'A00,D01',26.00,26.25,1),

    -- Y Dược TPHCM
    (10,15,1,2024,200,7000000,N'B00',29.10,29.40,1),
    (10,16,1,2024,200,14000000,N'B00,A00',28.20,28.50,1),

    -- FPT (học bạ)
    (11,1,2,2024,2000,25500000,N'A00,A01,D01',NULL,NULL,1),
    (11,2,2,2024,1500,25500000,N'A00,A01',NULL,NULL,1),
    (11,3,2,2024,500,27000000,N'A00,A01',NULL,NULL,1),
    (11,9,2,2024,600,26000000,N'A00,A01,D01',NULL,NULL,1),

    -- RMIT (học bạ / ĐGNL)
    (12,9,2,2024,500,97000000,N'A00,D01',NULL,NULL,1),
    (12,12,2,2024,400,97000000,N'A00,D01',NULL,NULL,1),

    -- ===== NĂM 2023 (lịch sử so sánh) =====
    (1,1,1,2023,350,22000000,N'A00,A01',27.80,28.15,1),
    (1,2,1,2023,300,22000000,N'A00,A01',27.50,27.80,1),
    (1,3,1,2023,150,24000000,N'A00,A01',29.20,29.50,1),
    (8,1,1,2023,400,23000000,N'A00,A01',27.20,27.50,1),
    (9,9,1,2023,600,23000000,N'A00,A01,D01',26.20,26.50,1),
    (4,15,1,2023,200,5800000,N'B00',29.10,29.40,1),
    (3,9,1,2023,500,21000000,N'A00,A01,D01',27.20,27.50,1)
END
GO

-- ============================================================
-- 10. LỊCH SỬ ĐIỂM CHUẨN (cho biểu đồ xu hướng)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_DiemChuanLichSu)
BEGIN
    INSERT INTO tbl_DiemChuanLichSu (MaTruong,MaChuyenNganh,MaPhuongThuc,NamTuyenSinh,DiemChuan,ChiTieu) VALUES
    -- Bách khoa HN - CNTT (2020-2024)
    (1,1,1,2020,27.35,300),(1,1,1,2021,27.60,320),(1,1,1,2022,27.90,335),(1,1,1,2023,28.15,345),(1,1,1,2024,28.45,350),
    -- Bách khoa HN - AI
    (1,3,1,2022,29.00,120),(1,3,1,2023,29.50,140),(1,3,1,2024,29.70,150),
    -- Y HN - Y đa khoa (2020-2024)
    (4,15,1,2020,29.00,180),(4,15,1,2021,29.15,190),(4,15,1,2022,29.25,195),(4,15,1,2023,29.40,200),(4,15,1,2024,29.65,200),
    -- KTQD - QTKD (2020-2024)
    (3,9,1,2020,26.80,450),(3,9,1,2021,27.00,470),(3,9,1,2022,27.20,490),(3,9,1,2023,27.50,500),(3,9,1,2024,27.80,500),
    -- Bách khoa TPHCM - CNTT
    (8,1,1,2020,26.80,350),(8,1,1,2021,27.00,370),(8,1,1,2022,27.20,385),(8,1,1,2023,27.50,395),(8,1,1,2024,27.80,400)
END
GO

-- ============================================================
-- 11. TƯ VẤN MẪU
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_TuVan)
BEGIN
    INSERT INTO tbl_TuVan (MaTruong,HoTen,Email,SoDienThoai,NoiDung,TrangThai) VALUES
    (1,N'Nguyễn Văn An', N'van.an@gmail.com',  N'0912345678',N'Em muốn hỏi về điều kiện xét tuyển vào ngành CNTT năm 2024, điểm chuẩn dự kiến là bao nhiêu?', 0),
    (8,N'Trần Thị Bình',  N'thi.binh@gmail.com',N'0978345612',N'Em có điểm thi THPT 27 điểm tổ hợp A01, em có đăng ký được ngành Kỹ thuật phần mềm không?', 0),
    (4,N'Lê Minh Hoàng',  N'minh.hoang@yahoo.com',N'0938456789',N'Cho em hỏi ngành Y đa khoa học mấy năm và học phí như thế nào ạ?', 1),
    (11,N'Phạm Thị Lan',  N'thi.lan@gmail.com', N'0965456123',N'Em quan tâm đến ngành AI của FPT, xin cho biết học phí và cơ hội việc làm sau khi tốt nghiệp?', 0),
    (3,N'Đặng Văn Tú',    N'van.tu@gmail.com',  N'0901234567',N'Em muốn biết thêm về chương trình học của ngành Tài chính - Ngân hàng tại trường.',0)
END
GO

-- ============================================================
-- 12. ĐÁNH GIÁ TRƯỜNG
-- ============================================================
-- (Cần tài khoản học sinh - MaTaiKhoan = 3)
DECLARE @hsID INT = (SELECT MaTaiKhoan FROM tbl_TaiKhoan WHERE Email='hocsinh01@test.com')
IF @hsID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM tbl_DanhGiaTruong)
BEGIN
    INSERT INTO tbl_DanhGiaTruong (MaTruong,MaTaiKhoan,DiemDanhGia,NoiDung,TrangThai) VALUES
    (1,@hsID,5,N'Trường Bách khoa Hà Nội rất tốt, cơ sở vật chất hiện đại, giảng viên giỏi và nhiều cơ hội thực hành.',1),
    (8,@hsID,5,N'Bách khoa TPHCM đào tạo chuyên sâu, gắn kết tốt với doanh nghiệp, môi trường học tập năng động.',1)
END
GO

-- ============================================================
-- 13. BÀI VIẾT / TIN TỨC
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_BaiViet)
BEGIN
    INSERT INTO tbl_BaiViet (TieuDe,Slug,NoiDung,MaTacGia,LuotXem,TrangThai,MaTruong) VALUES
    (N'Điểm chuẩn Đại học Bách khoa Hà Nội 2024 tăng mạnh',
     N'diem-chuan-bach-khoa-ha-noi-2024',
     N'Năm 2024, điểm chuẩn vào các ngành hot của Đại học Bách khoa Hà Nội tiếp tục tăng so với năm ngoái. Ngành Trí tuệ Nhân tạo dẫn đầu với 29.70 điểm, tiếp theo là CNTT với 28.45 điểm...',
     1, 1250, 1, 1),

    (N'Đại học Y Hà Nội: Bí quyết chinh phục ngành Y đa khoa',
     N'bi-quyet-chinh-phuc-nganh-y-da-khoa-ha-noi',
     N'Ngành Y đa khoa tại Đại học Y Hà Nội luôn là ngành có điểm chuẩn cao nhất cả nước. Năm 2024, điểm chuẩn là 29.65. Để đạt được điểm số này, thí sinh cần có chiến lược ôn thi hiệu quả...',
     1, 980, 1, 4),

    (N'FPT University: Học phí cao nhưng tỷ lệ việc làm 100%',
     N'fpt-university-hoc-phi-co-hoi-viec-lam',
     N'Đại học FPT nổi tiếng với chương trình đào tạo gắn liền thực tế doanh nghiệp. Dù học phí khá cao (25-27 triệu/kỳ), sinh viên ra trường đều có việc làm ngay với mức lương hấp dẫn...',
     1, 750, 1, 11),

    (N'Top 5 ngành Công nghệ Thông tin có điểm chuẩn cao nhất 2024',
     N'top-5-nganh-cntt-diem-chuan-cao-2024',
     N'Trong xu thế chuyển đổi số mạnh mẽ, các ngành Công nghệ Thông tin, AI và An toàn Thông tin ngày càng được nhiều thí sinh lựa chọn, dẫn đến điểm chuẩn liên tục tăng qua các năm...',
     1, 2100, 1, NULL),

    (N'Xu hướng tuyển sinh 2024: Ngành nào hot nhất?',
     N'xu-huong-tuyen-sinh-2024-nganh-hot',
     N'Năm 2024, các ngành liên quan đến Công nghệ, Y tế và Kinh tế số tiếp tục dẫn đầu về sức hút với thí sinh. Trong đó, Trí tuệ Nhân tạo, Khoa học Dữ liệu và Logistics là những ngành mới nổi được nhiều trường mở thêm chỉ tiêu...',
     1, 3500, 1, NULL),

    (N'Bách khoa TP.HCM mở ngành mới: Khoa học Dữ liệu và Trí tuệ Nhân tạo',
     N'bach-khoa-tphcm-nganh-moi-khoa-hoc-du-lieu-ai',
     N'Năm học 2024-2025, Trường ĐH Bách khoa TP.HCM chính thức tuyển sinh ngành Khoa học Dữ liệu và Trí tuệ Nhân tạo với 200 chỉ tiêu, điểm chuẩn dự kiến trên 29 điểm tổ hợp A00/A01...',
     1, 1800, 1, 8)
END
GO

-- ============================================================
-- 14. GÓP Ý / PHẢN HỒI
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tbl_GopY)
BEGIN
    INSERT INTO tbl_GopY (MaTruong,LoaiGopY,NoiDung,TrangThai) VALUES
    (1, 1, N'Thông tin học phí ngành CNTT của Bách khoa HN cần cập nhật, hiện tại đã tăng lên 26 triệu/năm rồi ạ.', 0),
    (8, 1, N'Điểm chuẩn ngành Kiến trúc năm 2024 bị ghi sai, thực tế là 25.50 chứ không phải 24.50.', 1),
    (NULL,2, N'Trang tìm kiếm trường load hơi chậm khi filter theo tỉnh thành, mong được cải thiện.', 0),
    (NULL,3, N'Website rất hữu ích! Mong bổ sung thêm thông tin về ký túc xá và học bổng của từng trường.', 2),
    (4, 1, N'Số điện thoại của Đại học Y Hà Nội bị sai mã vùng, cần kiểm tra lại.', 0)
END
GO

-- ============================================================
-- 15. HỒ SƠ HỌC SINH
-- ============================================================
DECLARE @hsID2 INT = (SELECT MaTaiKhoan FROM tbl_TaiKhoan WHERE Email='hocsinh01@test.com')
IF @hsID2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM tbl_ProfileHocSinh WHERE MaTaiKhoan=@hsID2)
BEGIN
    INSERT INTO tbl_ProfileHocSinh
        (MaTaiKhoan,HoTen,NgaySinh,TinhThanh,DiemDuKien,DiemMonHoc,MaChuyenNganh,MucTieuNghe,KhuVuc)
    VALUES
        (@hsID2, N'Nguyễn Học Sinh', '2006-05-15', N'Hà Nội',
         27.50,
         N'{"Toan":9.5,"LyHoa":9.0,"AnhVan":8.5}',
         1, -- CNTT
         N'Kỹ sư phần mềm tại các công ty công nghệ lớn',
         2) -- KV2
END
GO

-- ============================================================
-- 16. DANH SÁCH YÊU THÍCH (WISHLIST)
-- ============================================================
DECLARE @hsID3 INT = (SELECT MaTaiKhoan FROM tbl_TaiKhoan WHERE Email='hocsinh01@test.com')
IF @hsID3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM tbl_WishList WHERE MaTaiKhoan=@hsID3)
BEGIN
    INSERT INTO tbl_WishList (MaTaiKhoan,MaTruong,MaChuyenNganh,GhiChu) VALUES
    (@hsID3, 1, 1, N'Trường mơ ước! Cần đạt 28+ điểm'),
    (@hsID3, 8, 1, N'Lựa chọn thứ 2 nếu không đủ điểm BK HN'),
    (@hsID3,11, 1, N'Backup - FPT đảm bảo việc làm 100%')
END
GO

-- ============================================================
-- KIỂM TRA KẾT QUẢ
-- ============================================================
SELECT 'tbl_TaiKhoan'           AS Bang, COUNT(*) AS SoLuong FROM tbl_TaiKhoan           UNION ALL
SELECT 'tbl_Quyen',                       COUNT(*)           FROM tbl_Quyen                    UNION ALL
SELECT 'tbl_CapBac',                      COUNT(*)           FROM tbl_CapBac                   UNION ALL
SELECT 'tbl_DanhMucNganh',               COUNT(*)           FROM tbl_DanhMucNganh              UNION ALL
SELECT 'tbl_ChuyenNganh',                COUNT(*)           FROM tbl_ChuyenNganh               UNION ALL
SELECT 'tbl_PhuongThucXetTuyen',         COUNT(*)           FROM tbl_PhuongThucXetTuyen        UNION ALL
SELECT 'tbl_Truong',                      COUNT(*)           FROM tbl_Truong                    UNION ALL
SELECT 'tbl_TruongChuyenNganh',          COUNT(*)           FROM tbl_TruongChuyenNganh         UNION ALL
SELECT 'tbl_TinTuyenSinh',               COUNT(*)           FROM tbl_TinTuyenSinh              UNION ALL
SELECT 'tbl_DiemChuanLichSu',            COUNT(*)           FROM tbl_DiemChuanLichSu           UNION ALL
SELECT 'tbl_TuVan',                       COUNT(*)           FROM tbl_TuVan                     UNION ALL
SELECT 'tbl_DanhGiaTruong',              COUNT(*)           FROM tbl_DanhGiaTruong             UNION ALL
SELECT 'tbl_BaiViet',                    COUNT(*)           FROM tbl_BaiViet                   UNION ALL
SELECT 'tbl_GopY',                        COUNT(*)           FROM tbl_GopY                      UNION ALL
SELECT 'tbl_ProfileHocSinh',             COUNT(*)           FROM tbl_ProfileHocSinh            UNION ALL
SELECT 'tbl_WishList',                   COUNT(*)           FROM tbl_WishList
ORDER BY SoLuong DESC
GO

PRINT '=============================================='
PRINT 'SAMPLE DATA LOADED SUCCESSFULLY!'
PRINT 'Tai khoan test:'
PRINT '  Admin    : admin@tracuutuyensinh.vn / Admin@123'
PRINT '  TruongHoc: truong01@test.com        / Admin@123'
PRINT '  HocSinh  : hocsinh01@test.com       / Admin@123'
PRINT '  (mat khau cung = Admin@123 de de nho)'
PRINT '=============================================='
GO
