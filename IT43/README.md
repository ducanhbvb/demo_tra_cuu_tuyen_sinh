# 🎓 Tra Cứu Tuyển Sinh

> Hệ thống tra cứu thông tin tuyển sinh đại học, cao đẳng Việt Nam

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blueviolet?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet-framework/net48)
[![ASP.NET WebForms](https://img.shields.io/badge/ASP.NET-Web%20Forms-blue?logo=dotnet)](https://learn.microsoft.com/aspnet/web-forms/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2016+-red?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3.3-purple?logo=bootstrap)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](#license)

---

## 📸 Demo

| Trang chủ | Tra cứu điểm chuẩn | Admin Dashboard |
|:-:|:-:|:-:|
| ![Home](docs/screenshots/home.png) | ![DiemChuan](docs/screenshots/diemchuan.png) | ![Admin](docs/screenshots/admin.png) |

> ⚠️ *Screenshots sẽ được cập nhật sau khi hoàn thiện giao diện.*

---

## ✨ Tính năng

### 🔍 Dành cho Học sinh / Phụ huynh
- **Tìm kiếm trường** — theo tên, tỉnh thành, loại trường, ngành học
- **Tra cứu điểm chuẩn** — lọc theo năm, ngành, phương thức xét tuyển
- **Biểu đồ xu hướng** — điểm chuẩn qua các năm (Chart.js)
- **So sánh trường** — đặt song song thông tin 2-3 trường
- **Tìm theo ngành** — khám phá trường theo lĩnh vực quan tâm
- **Lưu yêu thích** — tạo danh sách trường quan tâm
- **Đăng ký tư vấn** — liên hệ trực tiếp với trường
- **Đánh giá trường** — viết nhận xét và chấm điểm 1-5 sao
- **Bài viết / Tin tức** — cập nhật thông tin tuyển sinh mới nhất

### 🛡️ Hệ thống tài khoản
- Đăng ký / Đăng nhập với xác nhận email
- Quên mật khẩu — gửi link reset qua email (hiệu lực 2 giờ)
- Bảo vệ brute-force — khóa tài khoản sau 5 lần sai (30 phút)
- Mã hóa mật khẩu SHA256 + Salt

### ⚙️ Trang quản trị (Admin)
- Dashboard thống kê tổng quan
- Quản lý trường, tin tuyển sinh, bài viết
- Quản lý góp ý / tư vấn
- Quản lý tài khoản (khóa/mở, phân quyền)
- Nhật ký hệ thống (audit log)

### 🌙 Giao diện
- **Dark / Light mode** — tự chuyển theo hệ thống hoặc chọn thủ công
- **Responsive** — hiển thị tốt trên desktop, tablet, mobile
- **Font tiếng Việt** — Be Vietnam Pro (Google Fonts)

---

## 🏗️ Kiến trúc

```
ASP.NET Web Forms — Kiến trúc 3 tầng (3-Layer Architecture)

┌────────────────────────────────────────────────┐
│  PRESENTATION     Client/*.aspx + Admin/*.aspx │
│                   Site.Master + Admin.Master   │
├────────────────────────────────────────────────┤
│  BUSINESS LOGIC   AppCode/BLL/*.cs             │
│                   (Validate, business rules)   │
├────────────────────────────────────────────────┤
│  DATA ACCESS      AppCode/DAL/*.cs             │
│                   ADO.NET + Stored Procedures  │
├────────────────────────────────────────────────┤
│  DATABASE         SQL Server — 18 tables, 6 SPs│
└────────────────────────────────────────────────┘
```

---

## 🛠️ Công nghệ

| Backend | Frontend | Database | Tools |
|---------|----------|----------|-------|
| ASP.NET Web Forms | Bootstrap 5.3.3 | SQL Server 2016+ | Visual Studio 2022 |
| .NET Framework 4.8 | Bootstrap Icons 1.11.3 | ADO.NET | SSMS 19+ |
| C# | Chart.js 4.4.3 | Stored Procedures | Git + GitHub |
| Forms Authentication | Google Fonts | | IIS Express |

---

## 🚀 Bắt đầu nhanh (Quick Start)

### Yêu cầu

- **Windows 10/11** (64-bit)
- **Visual Studio 2022** — workload "ASP.NET and web development"
- **SQL Server 2016+** (Express miễn phí)
- **SSMS** (SQL Server Management Studio)

### Bước 1: Clone repository

```bash
git clone https://github.com/bluestar998/TraCuuTuyenSinh.git
cd TraCuuTuyenSinh
```

### Bước 2: Tạo Database

1. Mở **SSMS** → kết nối SQL Server
2. **File → Open** → chọn `Database/Tracuutuyensinh.sql`
3. Nhấn **F5** (Execute) → chờ `Command(s) completed successfully`
4. Chạy thêm `Database/sample_data.sql` để có dữ liệu mẫu

### Bước 3: Cấu hình Connection String

Mở `Web.config`, sửa `Data Source` phù hợp:

```xml
<connectionStrings>
  <add name="TraCuuTuyenSinh"
       connectionString="Data Source=localhost;Initial Catalog=TraCuuTuyenSinh;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> Nếu dùng SQL Server Express: đổi thành `Data Source=localhost\SQLEXPRESS`

### Bước 4: Build & Run

1. Mở `TraCuuTuyenSinh.csproj` trong Visual Studio 2022
2. `Ctrl + Shift + B` — Build Solution
3. `F5` — Chạy (Debug mode)
4. Trình duyệt tự mở tại `http://localhost:63601/`

### Tài khoản mặc định

| Vai trò | Email | Mật khẩu |
|---------|-------|-----------|
| Admin | `admin@tracuutuyensinh.vn` | `Admin@123` |

> ⚠️ Mật khẩu chỉ dùng cho development. Đổi ngay khi triển khai production.

---

## 📁 Cấu trúc thư mục

```
TraCuuTuyenSinh/
├── AppCode/                  # Backend code
│   ├── BLL/                  #   Business Logic Layer
│   ├── DAL/                  #   Data Access Layer
│   ├── Helpers/              #   SecurityHelper, EmailHelper, SlugHelper...
│   └── Models/               #   Data models (POCO classes)
├── Client/                   # Trang người dùng (.aspx)
│   ├── Site.Master           #   Layout chung (navbar, footer)
│   └── *.aspx                #   index, TimKiem, DiemChuan, Login...
├── Admin/                    # Trang quản trị (.aspx)
│   ├── Admin.Master          #   Layout Admin (sidebar, topbar)
│   └── *.aspx                #   Dashboard, QuanLy*...
├── Content/                  # CSS (Client.css, Admin.css)
├── Scripts/                  # JavaScript (site.js)
├── Handlers/                 # Generic Handlers (API-like)
├── Database/                 # SQL scripts
│   ├── Tracuutuyensinh.sql   #   Tạo DB + tables + SPs
│   └── sample_data.sql       #   Dữ liệu mẫu
├── Resources/                # Icons, Images
├── docs/                     # Tài liệu dự án
├── Global.asax               # URL Routing, Auth
├── Web.config                # Cấu hình chính
└── favicon.ico               # Icon tab trình duyệt
```

---

## 📖 Tài liệu

| Tài liệu | Mô tả |
|-----------|--------|
| [📋 Hướng dẫn cài đặt chi tiết](docs/Huongdanbuild.md) | Hướng dẫn từng bước cho người mới |
| [🖥️ Môi trường phát triển](docs/moitruongphattrien.md) | Công nghệ, kiến trúc, yêu cầu hệ thống |
| [📂 Cấu trúc Project](docs/CautrucProject.md) | Mô tả chi tiết từng file/folder |
| [📝 Báo cáo đồ án](docs/BaoCaoDoAn.md) | Báo cáo đầy đủ |

---

## 🗄️ Database

**18 bảng** tổ chức theo nhóm:

| Nhóm | Bảng | Mô tả |
|------|------|-------|
| **Danh mục** | `tbl_Quyen`, `tbl_CapBac`, `tbl_DanhMucNganh`, `tbl_ChuyenNganh`, `tbl_PhuongThucXetTuyen` | Dữ liệu tham chiếu |
| **Tài khoản** | `tbl_TaiKhoan` | Đăng nhập, phân quyền, xác thực email |
| **Trường học** | `tbl_Truong`, `tbl_TruongChuyenNganh` | Thông tin trường + ngành đào tạo |
| **Tuyển sinh** | `tbl_TinTuyenSinh`, `tbl_DiemChuanLichSu` | Tin tuyển sinh + điểm chuẩn qua các năm |
| **Học sinh** | `tbl_ProfileHocSinh` | Hồ sơ cá nhân |
| **Tương tác** | `tbl_WishList`, `tbl_TuVan`, `tbl_DanhGiaTruong`, `tbl_GopY` | Yêu thích, tư vấn, đánh giá |
| **Nội dung** | `tbl_BaiViet` | Bài viết / tin tức |
| **Hệ thống** | `tbl_SearchHistory`, `tbl_Logs` | Lịch sử tìm kiếm, nhật ký |

**6 Stored Procedures:** `sp_Truong_TimKiem`, `sp_Truong_LayChiTiet`, `sp_TinTuyenSinh_TimKiem`, `sp_TaiKhoan_DangNhap`, `sp_TaiKhoan_DatLaiMatKhau`, `sp_GopY_TuVan_Them`

---

## 🔒 Bảo mật

- ✅ **SQL Injection** — 100% dùng `SqlParameter`, không nối chuỗi SQL
- ✅ **XSS** — `Server.HtmlEncode()` cho dữ liệu người dùng
- ✅ **CSRF** — ViewState MAC (`enableViewStateMac="true"`)
- ✅ **Brute-force** — Khóa tài khoản sau 5 lần sai (30 phút)
- ✅ **Password** — SHA256 + Salt, không lưu plaintext
- ✅ **HTTP Headers** — `X-Frame-Options`, `X-Content-Type-Options`, `X-XSS-Protection`
- ✅ **Cookie** — `httpOnly`, `sameSite="Lax"`

---

## 👥 Phân quyền

| Chức năng | Khách | Học sinh | Admin |
|-----------|:-----:|:--------:|:-----:|
| Tìm kiếm / Tra cứu | ✅ | ✅ | ✅ |
| Đăng ký tư vấn | ✅ | ✅ | ✅ |
| Lưu yêu thích | ❌ | ✅ | ✅ |
| Đánh giá trường | ❌ | ✅ | ✅ |
| Quản lý nội dung | ❌ | ❌ | ✅ |
| Quản lý tài khoản | ❌ | ❌ | ✅ |

---

## 📄 License

Dự án này được phát triển cho mục đích **đồ án môn học**.

---

<p align="center">
  <sub>Made with ❤️ by <strong>TraCuuTuyenSinh Team</strong> — 2026</sub>
</p>
