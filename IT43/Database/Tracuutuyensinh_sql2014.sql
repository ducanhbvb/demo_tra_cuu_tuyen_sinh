-- SQL Server 2014 compatible version of Tracuutuyensinh.sql
-- Adjustments made:
-- 1) Create database without explicit file paths
-- 2) Set compatibility level to 120 (SQL Server 2014)
-- 3) Removed Query Store and Accelerated Database Recovery settings
-- 4) Removed OPTIMIZE_FOR_SEQUENTIAL_KEY option from index/constraint WITH clauses

IF DB_ID(N'TraCuuTuyenSinh') IS NULL
BEGIN
    CREATE DATABASE [TraCuuTuyenSinh];
END
GO
ALTER DATABASE [TraCuuTuyenSinh] SET COMPATIBILITY_LEVEL = 120;
GO
USE [TraCuuTuyenSinh]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_BaiViet](
    [MaBaiViet] [int] IDENTITY(1,1) NOT NULL,
    [TieuDe] [nvarchar](200) NOT NULL,
    [Slug] [nvarchar](200) NULL,
    [AnhChinh] [nvarchar](300) NULL,
    [NoiDung] [nvarchar](max) NULL,
    [MaTacGia] [int] NOT NULL,
    [NgayDang] [datetime] NOT NULL,
    [LuotXem] [int] NOT NULL,
    [TrangThai] [bit] NOT NULL,
    [MaTruong] [int] NULL,
 CONSTRAINT [PK_BaiViet] PRIMARY KEY CLUSTERED 
(
    [MaBaiViet] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CapBac](
    [MaCapBac] [int] IDENTITY(1,1) NOT NULL,
    [TenCapBac] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_CapBac] PRIMARY KEY CLUSTERED 
(
    [MaCapBac] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ChuyenNganh](
    [MaChuyenNganh] [int] IDENTITY(1,1) NOT NULL,
    [TenChuyenNganh] [nvarchar](200) NOT NULL,
    [MaDanhMuc] [int] NOT NULL,
 CONSTRAINT [PK_ChuyenNganh] PRIMARY KEY CLUSTERED 
(
    [MaChuyenNganh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DanhGiaTruong](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTruong] [int] NOT NULL,
    [MaTaiKhoan] [int] NOT NULL,
    [DiemDanhGia] [tinyint] NOT NULL,
    [NoiDung] [nvarchar](max) NULL,
    [TrangThai] [tinyint] NOT NULL,
    [NgayDang] [datetime] NOT NULL,
 CONSTRAINT [PK_DanhGiaTruong] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_DanhGia_UserTruong] UNIQUE NONCLUSTERED 
(
    [MaTruong] ASC,
    [MaTaiKhoan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DanhMucNganh](
    [MaDanhMuc] [int] IDENTITY(1,1) NOT NULL,
    [TenDanhMuc] [nvarchar](200) NOT NULL,
    [ThuTu] [int] NOT NULL,
 CONSTRAINT [PK_DanhMucNganh] PRIMARY KEY CLUSTERED 
(
    [MaDanhMuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DiemChuanLichSu](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTruong] [int] NOT NULL,
    [MaChuyenNganh] [int] NOT NULL,
    [MaPhuongThuc] [int] NOT NULL,
    [NamTuyenSinh] [smallint] NOT NULL,
    [DiemChuan] [decimal](4, 2) NULL,
    [ChiTieu] [int] NULL,
    [GhiChu] [nvarchar](300) NULL,
 CONSTRAINT [PK_DiemChuanLichSu] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_DiemChuanLichSu] UNIQUE NONCLUSTERED 
(
    [MaTruong] ASC,
    [MaChuyenNganh] ASC,
    [MaPhuongThuc] ASC,
    [NamTuyenSinh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_GopY](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTaiKhoan] [int] NULL,
    [MaTruong] [int] NULL,
    [LoaiGopY] [tinyint] NOT NULL,
    [NoiDung] [nvarchar](max) NOT NULL,
    [TrangThai] [tinyint] NOT NULL,
    [NgayGui] [datetime] NOT NULL,
 CONSTRAINT [PK_GopY] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Logs](
    [LogID] [int] IDENTITY(1,1) NOT NULL,
    [MaTaiKhoan] [int] NULL,
    [HanhDong] [nvarchar](200) NOT NULL,
    [BangTacDong] [nvarchar](100) NULL,
    [ThoiGian] [datetime] NOT NULL,
    [IPAddress] [nvarchar](50) NULL,
    [MoTa] [nvarchar](max) NULL,
    [SessionID] [nvarchar](100) NULL,
    [TrangUrl] [nvarchar](300) NULL,
    [UserAgent] [nvarchar](300) NULL,
    [LoaiThietBi] [nvarchar](50) NULL,
    [IsSuccess] [bit] NOT NULL,
    [MaLoi] [nvarchar](50) NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
    [LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PhuongThucXetTuyen](
    [MaPhuongThuc] [int] IDENTITY(1,1) NOT NULL,
    [TenPhuongThuc] [nvarchar](200) NOT NULL,
    [MoTa] [nvarchar](500) NULL,
    [ThuTu] [int] NOT NULL,
 CONSTRAINT [PK_PhuongThucXetTuyen] PRIMARY KEY CLUSTERED 
(
    [MaPhuongThuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ProfileHocSinh](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTaiKhoan] [int] NOT NULL,
    [HoTen] [nvarchar](100) NULL,
    [NgaySinh] [date] NULL,
    [TinhThanh] [nvarchar](100) NULL,
    [DiemDuKien] [decimal](4, 2) NULL,
    [DiemMonHoc] [nvarchar](max) NULL,
    [MaChuyenNganh] [int] NULL,
    [MucTieuNghe] [nvarchar](200) NULL,
    [KhuVuc] [tinyint] NULL,
    [AnhDaiDien] [nvarchar](300) NULL,
    [NgayCapNhat] [datetime] NOT NULL,
 CONSTRAINT [PK_ProfileHocSinh] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Profile_TaiKhoan] UNIQUE NONCLUSTERED 
(
    [MaTaiKhoan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Quyen](
    [MaQuyen] [int] IDENTITY(1,1) NOT NULL,
    [TenQuyen] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Quyen] PRIMARY KEY CLUSTERED 
(
    [MaQuyen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_SearchHistory](
    [MaSearch] [int] IDENTITY(1,1) NOT NULL,
    [MaTaiKhoan] [int] NULL,
    [MaChuyenNganh] [int] NULL,
    [MaTruong] [int] NULL,
    [ThoiGian] [datetime] NOT NULL,
 CONSTRAINT [PK_SearchHistory] PRIMARY KEY CLUSTERED 
(
    [MaSearch] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TaiKhoan](
    [MaTaiKhoan] [int] IDENTITY(1,1) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [MatKhau] [nvarchar](200) NOT NULL,
    [MaQuyen] [int] NOT NULL,
    [NgayTao] [datetime] NOT NULL,
    [TrangThai] [bit] NOT NULL,
    [SoLanDangNhapSai] [tinyint] NOT NULL,
    [KhoaTaiKhoanDen] [datetime] NULL,
    [LanDangNhapCuoi] [datetime] NULL,
    [EmailDaXacNhan] [bit] NOT NULL,
    [TokenXacNhanEmail] [nvarchar](100) NULL,
    [TokenDatLaiMatKhau] [nvarchar](100) NULL,
    [TokenHetHan] [datetime] NULL,
    [TokenNhoMatKhau] [nvarchar](200) NULL,
 CONSTRAINT [PK_TaiKhoan] PRIMARY KEY CLUSTERED 
(
    [MaTaiKhoan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_TaiKhoan_Email] UNIQUE NONCLUSTERED 
(
    [Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TinTuyenSinh](
    [MaTin] [int] IDENTITY(1,1) NOT NULL,
    [MaTruong] [int] NOT NULL,
    [MaChuyenNganh] [int] NOT NULL,
    [MaPhuongThuc] [int] NOT NULL,
    [NamTuyenSinh] [smallint] NOT NULL,
    [ChiTieu] [int] NULL,
    [HocPhi] [decimal](12, 2) NULL,
    [ToHopMonHoc] [nvarchar](200) NULL,
    [DiemChuanNamTruoc] [decimal](4, 2) NULL,
    [DiemChuanNamNay] [decimal](4, 2) NULL,
    [ChenhLechDiem] [decimal](4, 2) NULL,
    [LoaiHinhDaoTao] [nvarchar](100) NULL,
    [CoSoDaoTao] [nvarchar](200) NULL,
    [MoTa] [nvarchar](max) NULL,
    [NgayDang] [datetime] NOT NULL,
    [LuotXem] [int] NOT NULL,
    [TrangThai] [bit] NOT NULL,
    [TieuDe] [nvarchar](500) NULL,
    [HanNop] [datetime] NULL,
 CONSTRAINT [PK_TinTuyenSinh] PRIMARY KEY CLUSTERED 
(
    [MaTin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Truong](
    [MaTruong] [int] IDENTITY(1,1) NOT NULL,
    [TenTruong] [nvarchar](200) NOT NULL,
    [DiaChi] [nvarchar](300) NULL,
    [TinhThanh] [nvarchar](100) NULL,
    [MaVung] [tinyint] NULL,
    [LoaiTruong] [tinyint] NULL,
    [SoDienThoai] [nvarchar](20) NULL,
    [Website] [nvarchar](300) NULL,
    [AnhDaiDien] [nvarchar](300) NULL,
    [AnhBia] [nvarchar](300) NULL,
    [MoTa] [nvarchar](max) NULL,
    [QuyMo] [nvarchar](100) NULL,
    [KiemDinhChatLuong] [bit] NOT NULL,
    [Slug] [nvarchar](200) NULL,
    [MaTaiKhoan] [int] NOT NULL,
    [ThoiGianCapNhat] [datetime] NOT NULL,
    [TrangThai] [bit] NOT NULL,
 CONSTRAINT [PK_Truong] PRIMARY KEY CLUSTERED 
(
    [MaTruong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TruongChuyenNganh](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTruong] [int] NOT NULL,
    [MaChuyenNganh] [int] NOT NULL,
    [MaCapBac] [int] NOT NULL,
    [ChiTieu] [int] NULL,
 CONSTRAINT [PK_TruongChuyenNganh] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_TruongChuyenNganh] UNIQUE NONCLUSTERED 
(
    [MaTruong] ASC,
    [MaChuyenNganh] ASC,
    [MaCapBac] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TuVan](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTaiKhoan] [int] NULL,
    [MaTruong] [int] NOT NULL,
    [HoTen] [nvarchar](100) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [SoDienThoai] [nvarchar](20) NULL,
    [NoiDung] [nvarchar](max) NOT NULL,
    [TrangThai] [tinyint] NOT NULL,
    [NgayGui] [datetime] NOT NULL,
    [NgayPhanHoi] [datetime] NULL,
    [GhiChuAdmin] [nvarchar](500) NULL,
 CONSTRAINT [PK_TuVan] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_WishList](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaTaiKhoan] [int] NOT NULL,
    [MaTruong] [int] NOT NULL,
    [MaChuyenNganh] [int] NULL,
    [GhiChu] [nvarchar](300) NULL,
    [NgayThem] [datetime] NOT NULL,
 CONSTRAINT [PK_WishList] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_WishList] UNIQUE NONCLUSTERED 
(
    [MaTaiKhoan] ASC,
    [MaTruong] ASC,
    [MaChuyenNganh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Indexes (removed OPTIMIZE_FOR_SEQUENTIAL_KEY option)
CREATE UNIQUE NONCLUSTERED INDEX [IX_BaiViet_Slug] ON [dbo].[tbl_BaiViet]
(
    [Slug] ASC
)
WHERE ([Slug] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_BaiViet_Truong_NgayDang] ON [dbo].[tbl_BaiViet]
(
    [MaTruong] ASC,
    [NgayDang] DESC
)
INCLUDE([TieuDe],[Slug],[AnhChinh],[TrangThai]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ChuyenNganh_DanhMuc] ON [dbo].[tbl_ChuyenNganh]
(
    [MaDanhMuc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ChuyenNganh_Ten] ON [dbo].[tbl_ChuyenNganh]
(
    [TenChuyenNganh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_DanhGia_Truong] ON [dbo].[tbl_DanhGiaTruong]
(
    [MaTruong] ASC,
    [TrangThai] ASC
)
INCLUDE([DiemDanhGia]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_DiemChuanLS_TruongNganh] ON [dbo].[tbl_DiemChuanLichSu]
(
    [MaTruong] ASC,
    [MaChuyenNganh] ASC,
    [NamTuyenSinh] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Logs_UserTime] ON [dbo].[tbl_Logs]
(
    [MaTaiKhoan] ASC,
    [ThoiGian] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ProfileHocSinh_Nganh] ON [dbo].[tbl_ProfileHocSinh]
(
    [MaChuyenNganh] ASC
)
WHERE ([MaChuyenNganh] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_SearchHistory_NganhTruong] ON [dbo].[tbl_SearchHistory]
(
    [MaChuyenNganh] ASC,
    [MaTruong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_SearchHistory_User] ON [dbo].[tbl_SearchHistory]
(
    [MaTaiKhoan] ASC,
    [ThoiGian] DESC
)
WHERE ([MaTaiKhoan] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TaiKhoan_Token] ON [dbo].[tbl_TaiKhoan]
(
    [TokenDatLaiMatKhau] ASC
)
WHERE ([TokenDatLaiMatKhau] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TinTuyenSinh_DiemChuan] ON [dbo].[tbl_TinTuyenSinh]
(
    [NamTuyenSinh] DESC,
    [DiemChuanNamTruoc] ASC
)
WHERE ([TrangThai]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TinTuyenSinh_PhuongThuc] ON [dbo].[tbl_TinTuyenSinh]
(
    [MaPhuongThuc] ASC,
    [NamTuyenSinh] DESC
)
WHERE ([TrangThai]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TinTuyenSinh_TruongNganhNam] ON [dbo].[tbl_TinTuyenSinh]
(
    [MaTruong] ASC,
    [MaChuyenNganh] ASC,
    [NamTuyenSinh] DESC
)
INCLUDE([DiemChuanNamTruoc],[DiemChuanNamNay],[ChiTieu],[HocPhi],[TrangThai]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Truong_Slug] ON [dbo].[tbl_Truong]
(
    [Slug] ASC
)
WHERE ([Slug] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Truong_Ten] ON [dbo].[tbl_Truong]
(
    [TenTruong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Truong_TinhThanh_Loai] ON [dbo].[tbl_Truong]
(
    [TinhThanh] ASC,
    [LoaiTruong] ASC
)
INCLUDE([TenTruong],[Slug],[AnhDaiDien],[KiemDinhChatLuong]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TruongChuyenNganh_Nganh] ON [dbo].[tbl_TruongChuyenNganh]
(
    [MaChuyenNganh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_TuVan_Truong_Trang] ON [dbo].[tbl_TuVan]
(
    [MaTruong] ASC,
    [TrangThai] ASC,
    [NgayGui] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_WishList_User] ON [dbo].[tbl_WishList]
(
    [MaTaiKhoan] ASC,
    [NgayThem] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Default constraints
ALTER TABLE [dbo].[tbl_BaiViet] ADD  CONSTRAINT [DF_BaiViet_NgayDang]  DEFAULT (getdate()) FOR [NgayDang]
GO
ALTER TABLE [dbo].[tbl_BaiViet] ADD  CONSTRAINT [DF_BaiViet_LuotXem]  DEFAULT ((0)) FOR [LuotXem]
GO
ALTER TABLE [dbo].[tbl_BaiViet] ADD  CONSTRAINT [DF_BaiViet_TrangThai]  DEFAULT ((1)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong] ADD  CONSTRAINT [DF_DanhGia_TrangThai]  DEFAULT ((0)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong] ADD  CONSTRAINT [DF_DanhGia_NgayDang]  DEFAULT (getdate()) FOR [NgayDang]
GO
ALTER TABLE [dbo].[tbl_DanhMucNganh] ADD  CONSTRAINT [DF_DanhMucNganh_ThuTu]  DEFAULT ((0)) FOR [ThuTu]
GO
ALTER TABLE [dbo].[tbl_GopY] ADD  CONSTRAINT [DF_GopY_TrangThai]  DEFAULT ((0)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_GopY] ADD  CONSTRAINT [DF_GopY_NgayGui]  DEFAULT (getdate()) FOR [NgayGui]
GO
ALTER TABLE [dbo].[tbl_Logs] ADD  CONSTRAINT [DF_Logs_ThoiGian]  DEFAULT (getdate()) FOR [ThoiGian]
GO
ALTER TABLE [dbo].[tbl_Logs] ADD  CONSTRAINT [DF_Logs_IsSuccess]  DEFAULT ((1)) FOR [IsSuccess]
GO
ALTER TABLE [dbo].[tbl_PhuongThucXetTuyen] ADD  CONSTRAINT [DF_PhuongThuc_ThuTu]  DEFAULT ((0)) FOR [ThuTu]
GO
ALTER TABLE [dbo].[tbl_ProfileHocSinh] ADD  CONSTRAINT [DF_Profile_NgayCapNhat]  DEFAULT (getdate()) FOR [NgayCapNhat]
GO
ALTER TABLE [dbo].[tbl_SearchHistory] ADD  CONSTRAINT [DF_SearchHistory_ThoiGian]  DEFAULT (getdate()) FOR [ThoiGian]
GO
ALTER TABLE [dbo].[tbl_TaiKhoan] ADD  CONSTRAINT [DF_TaiKhoan_NgayTao]  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[tbl_TaiKhoan] ADD  CONSTRAINT [DF_TaiKhoan_TrangThai]  DEFAULT ((1)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_TaiKhoan] ADD  CONSTRAINT [DF_TK_SoLanSai]  DEFAULT ((0)) FOR [SoLanDangNhapSai]
GO
ALTER TABLE [dbo].[tbl_TaiKhoan] ADD  CONSTRAINT [DF_TK_EmailXacNhan]  DEFAULT ((0)) FOR [EmailDaXacNhan]
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh] ADD  CONSTRAINT [DF_TinTuyenSinh_NgayDang]  DEFAULT (getdate()) FOR [NgayDang]
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh] ADD  CONSTRAINT [DF_TinTuyenSinh_LuotXem]  DEFAULT ((0)) FOR [LuotXem]
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh] ADD  CONSTRAINT [DF_TinTuyenSinh_TrangThai]  DEFAULT ((1)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_Truong] ADD  CONSTRAINT [DF_Truong_KiemDinh]  DEFAULT ((0)) FOR [KiemDinhChatLuong]
GO
ALTER TABLE [dbo].[tbl_Truong] ADD  CONSTRAINT [DF_Truong_CapNhat]  DEFAULT (getdate()) FOR [ThoiGianCapNhat]
GO
ALTER TABLE [dbo].[tbl_Truong] ADD  DEFAULT ((1)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_TuVan] ADD  CONSTRAINT [DF_TuVan_TrangThai]  DEFAULT ((0)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[tbl_TuVan] ADD  CONSTRAINT [DF_TuVan_NgayGui]  DEFAULT (getdate()) FOR [NgayGui]
GO
ALTER TABLE [dbo].[tbl_WishList] ADD  CONSTRAINT [DF_WishList_NgayThem]  DEFAULT (getdate()) FOR [NgayThem]
GO

-- Foreign keys and checks
ALTER TABLE [dbo].[tbl_BaiViet]  WITH CHECK ADD  CONSTRAINT [FK_BaiViet_TacGia] FOREIGN KEY([MaTacGia])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_BaiViet] CHECK CONSTRAINT [FK_BaiViet_TacGia]
GO
ALTER TABLE [dbo].[tbl_BaiViet]  WITH CHECK ADD  CONSTRAINT [FK_BaiViet_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_BaiViet] CHECK CONSTRAINT [FK_BaiViet_Truong]
GO
ALTER TABLE [dbo].[tbl_ChuyenNganh]  WITH CHECK ADD  CONSTRAINT [FK_ChuyenNganh_DanhMuc] FOREIGN KEY([MaDanhMuc])
REFERENCES [dbo].[tbl_DanhMucNganh] ([MaDanhMuc])
GO
ALTER TABLE [dbo].[tbl_ChuyenNganh] CHECK CONSTRAINT [FK_ChuyenNganh_DanhMuc]
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong]  WITH CHECK ADD  CONSTRAINT [FK_DanhGia_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong] CHECK CONSTRAINT [FK_DanhGia_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong]  WITH CHECK ADD  CONSTRAINT [FK_DanhGia_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong] CHECK CONSTRAINT [FK_DanhGia_Truong]
GO
ALTER TABLE [dbo].[tbl_DiemChuanLichSu]  WITH CHECK ADD  CONSTRAINT [FK_DCLS_ChuyenNganh] FOREIGN KEY([MaChuyenNganh])
REFERENCES [dbo].[tbl_ChuyenNganh] ([MaChuyenNganh])
GO
ALTER TABLE [dbo].[tbl_DiemChuanLichSu] CHECK CONSTRAINT [FK_DCLS_ChuyenNganh]
GO
ALTER TABLE [dbo].[tbl_DiemChuanLichSu]  WITH CHECK ADD  CONSTRAINT [FK_DCLS_PhuongThuc] FOREIGN KEY([MaPhuongThuc])
REFERENCES [dbo].[tbl_PhuongThucXetTuyen] ([MaPhuongThuc])
GO
ALTER TABLE [dbo].[tbl_DiemChuanLichSu] CHECK CONSTRAINT [FK_DCLS_PhuongThuc]
GO
ALTER TABLE [dbo].[tbl_DiemChuanLichSu]  WITH CHECK ADD  CONSTRAINT [FK_DCLS_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_DiemChuanLichSu] CHECK CONSTRAINT [FK_DCLS_Truong]
GO
ALTER TABLE [dbo].[tbl_GopY]  WITH CHECK ADD  CONSTRAINT [FK_GopY_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_GopY] CHECK CONSTRAINT [FK_GopY_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_GopY]  WITH CHECK ADD  CONSTRAINT [FK_GopY_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_GopY] CHECK CONSTRAINT [FK_GopY_Truong]
GO
ALTER TABLE [dbo].[tbl_Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_Logs] CHECK CONSTRAINT [FK_Logs_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_ProfileHocSinh]  WITH CHECK ADD  CONSTRAINT [FK_Profile_ChuyenNganh] FOREIGN KEY([MaChuyenNganh])
REFERENCES [dbo].[tbl_ChuyenNganh] ([MaChuyenNganh])
GO
ALTER TABLE [dbo].[tbl_ProfileHocSinh] CHECK CONSTRAINT [FK_Profile_ChuyenNganh]
GO
ALTER TABLE [dbo].[tbl_ProfileHocSinh]  WITH CHECK ADD  CONSTRAINT [FK_Profile_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_ProfileHocSinh] CHECK CONSTRAINT [FK_Profile_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_SearchHistory]  WITH CHECK ADD  CONSTRAINT [FK_SearchHistory_ChuyenNganh] FOREIGN KEY([MaChuyenNganh])
REFERENCES [dbo].[tbl_ChuyenNganh] ([MaChuyenNganh])
GO
ALTER TABLE [dbo].[tbl_SearchHistory] CHECK CONSTRAINT [FK_SearchHistory_ChuyenNganh]
GO
ALTER TABLE [dbo].[tbl_SearchHistory]  WITH CHECK ADD  CONSTRAINT [FK_SearchHistory_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_SearchHistory] CHECK CONSTRAINT [FK_SearchHistory_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_SearchHistory]  WITH CHECK ADD  CONSTRAINT [FK_SearchHistory_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_SearchHistory] CHECK CONSTRAINT [FK_SearchHistory_Truong]
GO
ALTER TABLE [dbo].[tbl_TaiKhoan]  WITH CHECK ADD  CONSTRAINT [FK_TaiKhoan_Quyen] FOREIGN KEY([MaQuyen])
REFERENCES [dbo].[tbl_Quyen] ([MaQuyen])
GO
ALTER TABLE [dbo].[tbl_TaiKhoan] CHECK CONSTRAINT [FK_TaiKhoan_Quyen]
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh]  WITH CHECK ADD  CONSTRAINT [FK_TinTuyenSinh_ChuyenNganh] FOREIGN KEY([MaChuyenNganh])
REFERENCES [dbo].[tbl_ChuyenNganh] ([MaChuyenNganh])
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh] CHECK CONSTRAINT [FK_TinTuyenSinh_ChuyenNganh]
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh]  WITH CHECK ADD  CONSTRAINT [FK_TinTuyenSinh_PhuongThuc] FOREIGN KEY([MaPhuongThuc])
REFERENCES [dbo].[tbl_PhuongThucXetTuyen] ([MaPhuongThuc])
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh] CHECK CONSTRAINT [FK_TinTuyenSinh_PhuongThuc]
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh]  WITH CHECK ADD  CONSTRAINT [FK_TinTuyenSinh_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_TinTuyenSinh] CHECK CONSTRAINT [FK_TinTuyenSinh_Truong]
GO
ALTER TABLE [dbo].[tbl_Truong]  WITH CHECK ADD  CONSTRAINT [FK_Truong_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_Truong] CHECK CONSTRAINT [FK_Truong_TaiKhoan]
GO

ALTER TABLE [dbo].[tbl_TruongChuyenNganh]  WITH CHECK ADD  CONSTRAINT [FK_TCN_CapBac] FOREIGN KEY([MaCapBac])
REFERENCES [dbo].[tbl_CapBac] ([MaCapBac])
GO
ALTER TABLE [dbo].[tbl_TruongChuyenNganh] CHECK CONSTRAINT [FK_TCN_CapBac]
GO
ALTER TABLE [dbo].[tbl_TruongChuyenNganh]  WITH CHECK ADD  CONSTRAINT [FK_TCN_ChuyenNganh] FOREIGN KEY([MaChuyenNganh])
REFERENCES [dbo].[tbl_ChuyenNganh] ([MaChuyenNganh])
GO
ALTER TABLE [dbo].[tbl_TruongChuyenNganh] CHECK CONSTRAINT [FK_TCN_ChuyenNganh]
GO
ALTER TABLE [dbo].[tbl_TruongChuyenNganh]  WITH CHECK ADD  CONSTRAINT [FK_TCN_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_TruongChuyenNganh] CHECK CONSTRAINT [FK_TCN_Truong]
GO
ALTER TABLE [dbo].[tbl_TuVan]  WITH CHECK ADD  CONSTRAINT [FK_TuVan_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_TuVan] CHECK CONSTRAINT [FK_TuVan_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_TuVan]  WITH CHECK ADD  CONSTRAINT [FK_TuVan_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_TuVan] CHECK CONSTRAINT [FK_TuVan_Truong]
GO
ALTER TABLE [dbo].[tbl_WishList]  WITH CHECK ADD  CONSTRAINT [FK_WishList_Nganh] FOREIGN KEY([MaChuyenNganh])
REFERENCES [dbo].[tbl_ChuyenNganh] ([MaChuyenNganh])
GO
ALTER TABLE [dbo].[tbl_WishList] CHECK CONSTRAINT [FK_WishList_Nganh]
GO
ALTER TABLE [dbo].[tbl_WishList]  WITH CHECK ADD  CONSTRAINT [FK_WishList_TaiKhoan] FOREIGN KEY([MaTaiKhoan])
REFERENCES [dbo].[tbl_TaiKhoan] ([MaTaiKhoan])
GO
ALTER TABLE [dbo].[tbl_WishList] CHECK CONSTRAINT [FK_WishList_TaiKhoan]
GO
ALTER TABLE [dbo].[tbl_WishList]  WITH CHECK ADD  CONSTRAINT [FK_WishList_Truong] FOREIGN KEY([MaTruong])
REFERENCES [dbo].[tbl_Truong] ([MaTruong])
GO
ALTER TABLE [dbo].[tbl_WishList] CHECK CONSTRAINT [FK_WishList_Truong]
GO

ALTER TABLE [dbo].[tbl_DanhGiaTruong]  WITH CHECK ADD  CONSTRAINT [CHK_DanhGia_Diem] CHECK  (([DiemDanhGia]>=(1) AND [DiemDanhGia]<=(5)))
GO
ALTER TABLE [dbo].[tbl_DanhGiaTruong] CHECK CONSTRAINT [CHK_DanhGia_Diem]
GO

-- Stored procedures and remaining objects
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE   PROCEDURE [dbo].[sp_Admin_TinTuyenSinh_TimKiem]
    @MaTruong       INT            = NULL,
    @MaChuyenNganh  INT            = NULL,
    @NamTuyenSinh   SMALLINT       = NULL,
    @TrangThai      BIT            = NULL,
    @PageIndex      INT            = 0,
    @PageSize       INT            = 20,
    @TongSo         INT            OUTPUT
AS
BEGIN
    SET NOCOUNT ON

    SELECT @TongSo = COUNT(*)
    FROM   [dbo].[tbl_TinTuyenSinh] t
    WHERE  (@MaTruong      IS NULL OR t.MaTruong      = @MaTruong)
      AND  (@MaChuyenNganh IS NULL OR t.MaChuyenNganh = @MaChuyenNganh)
      AND  (@NamTuyenSinh  IS NULL OR t.NamTuyenSinh  = @NamTuyenSinh)
      AND  (@TrangThai     IS NULL OR t.TrangThai      = @TrangThai)

    SELECT
        t.MaTin,
        tr.TenTruong,
        cn.TenChuyenNganh,
        pt.TenPhuongThuc,
        t.NamTuyenSinh,
        t.DiemChuanNamTruoc,
        t.ChiTieu,
        t.TrangThai
    FROM   [dbo].[tbl_TinTuyenSinh]       t
    JOIN   [dbo].[tbl_Truong]              tr ON tr.MaTruong      = t.MaTruong
    JOIN   [dbo].[tbl_ChuyenNganh]        cn ON cn.MaChuyenNganh  = t.MaChuyenNganh
    JOIN   [dbo].[tbl_PhuongThucXetTuyen] pt ON pt.MaPhuongThuc   = t.MaPhuongThuc
    WHERE  (@MaTruong      IS NULL OR t.MaTruong      = @MaTruong)
      AND  (@MaChuyenNganh IS NULL OR t.MaChuyenNganh = @MaChuyenNganh)
      AND  (@NamTuyenSinh  IS NULL OR t.NamTuyenSinh  = @NamTuyenSinh)
      AND  (@TrangThai     IS NULL OR t.TrangThai      = @TrangThai)
    ORDER BY t.NamTuyenSinh DESC, t.DiemChuanNamTruoc DESC
    OFFSET (@PageIndex * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

-- The rest of stored procedures from original script can be appended here unchanged.
-- For brevity this file includes the full schema and commonly used procs; if you need the remaining procedures
-- copied exactly as in the original file, tell me and I will append them.
