# TRƯỜNG ĐẠI HỌC MỞ HÀ NỘI
## TRUNG TÂM ĐÀO TẠO E-LEARNING

---

# BÁO CÁO MÔN HỌC

## Đề tài: XÂY DỰNG WEBSITE HỖ TRỢ TRA CỨU THÔNG TIN CÁC TRƯỜNG ĐẠI HỌC VÀ CAO ĐẲNG VIỆT NAM

---

| | |
|---|---|
| **Giảng viên hướng dẫn:** | Ths. Nguyễn Thị Tâm |
| **Sinh viên thực hiện:** | Lê Thị Hồng Hạnh |
| **Lớp:** | CHTM118 |
| **Ngành đào tạo:** | Công Nghệ Thông Tin |
| **Địa điểm học:** | Đại Học Mở Hà Nội |

**Hà Nội - 2025**

---

# MỤC LỤC

- [CHƯƠNG 1. TỔNG QUAN ĐỀ TÀI](#chương-1-tổng-quan-đề-tài)
  - [1.1. Giới thiệu nơi thực tập](#11-giới-thiệu-nơi-thực-tập)
  - [1.2. Giới thiệu đề tài](#12-giới-thiệu-đề-tài)
  - [1.3. Yêu cầu hệ thống](#13-yêu-cầu-hệ-thống)
    - [1.3.1 Yêu cầu chức năng](#131-yêu-cầu-chức-năng)
    - [1.3.2 Yêu cầu phi chức năng](#132-yêu-cầu-phi-chức-năng)
- [CHƯƠNG 2. CƠ SỞ LÝ THUYẾT](#chương-2-cơ-sở-lý-thuyết)
  - [2.1. Giới thiệu ASP.NET Web Forms](#21-giới-thiệu-aspnet-web-forms)
  - [2.2. Ngôn ngữ lập trình C#](#22-ngôn-ngữ-lập-trình-c)
  - [2.3. HTML – CSS – JavaScript – Bootstrap](#23-html--css--javascript--bootstrap)
  - [2.4. SQL Server](#24-sql-server)
  - [2.5. Kiến trúc phần mềm 3-Layer Architecture](#25-kiến-trúc-phần-mềm-3-layer-architecture)
- [CHƯƠNG 3. PHÂN TÍCH THIẾT KẾ HỆ THỐNG](#chương-3-phân-tích-thiết-kế-hệ-thống)
  - [3.1. Sơ đồ phân rã chức năng](#31-sơ-đồ-phân-rã-chức-năng)
  - [3.2. Sơ đồ ngữ cảnh](#32-sơ-đồ-ngữ-cảnh)
  - [3.3. Sơ đồ mức đỉnh](#33-sơ-đồ-mức-đỉnh)
  - [3.4. Sơ đồ mức dưới đỉnh](#34-sơ-đồ-mức-dưới-đỉnh)
  - [3.5. Thiết kế cơ sở dữ liệu](#35-thiết-kế-cơ-sở-dữ-liệu)
  - [3.6. Database Diagram](#36-database-diagram)
- [CHƯƠNG 4. THIẾT KẾ CHƯƠNG TRÌNH](#chương-4-thiết-kế-chương-trình)
  - [4.1. Admin](#41-admin)
  - [4.2. Học sinh / Thí sinh](#42-học-sinh--thí-sinh)
  - [4.3. Các luồng xử lý chính](#43-các-luồng-xử-lý-chính)
- [CHƯƠNG 5. BẢO MẬT HỆ THỐNG](#chương-5-bảo-mật-hệ-thống)
- [CHƯƠNG 6. KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN](#chương-6-kết-luận-và-hướng-phát-triển)

---

# CHƯƠNG 1. TỔNG QUAN ĐỀ TÀI

## 1.1. Giới thiệu nơi thực tập

Công ty TNHH Lush Era Việt Nam, thành lập vào tháng 9 năm 2014, có trụ sở tại tầng 23B, tòa nhà HH4 Sông Đà, đường Phạm Hùng, phường Mỹ Đình 1, quận Nam Từ Liêm, Hà Nội. Người sáng lập công ty cũng là người đứng đầu Gemtek Technology, tập đoàn hàng đầu Đài Loan trong lĩnh vực công nghệ không dây băng thông rộng.

Lush Era Việt Nam tập trung vào việc nghiên cứu và phát triển phần mềm ứng dụng di động, chăm sóc khách hàng và marketing. Công ty luôn chú trọng xây dựng đội ngũ nhân sự chuyên nghiệp và năng động, tạo môi trường làm việc thân thiện và chuyên nghiệp. Với phương châm "Con người là yếu tố then chốt để tạo nên sự thành công của doanh nghiệp", Lush Era không ngừng tìm kiếm những nhân tài có kinh nghiệm và đam mê công việc để cùng phát triển bền vững.

Thực tập tại Lush Era Việt Nam, em đã có cơ hội làm việc trong môi trường năng động, chuyên nghiệp, tham gia vào các dự án thực tế về phát triển web và ứng dụng, từ đó phát triển kỹ năng lập trình C#, ASP.NET và quản trị cơ sở dữ liệu SQL Server.

## 1.2. Giới thiệu đề tài

Hiện nay Việt Nam có rất nhiều trường đại học và cao đẳng với nhiều ngành nghề đào tạo khác nhau. Nhằm phục vụ nhu cầu tra cứu cho thí sinh trong các mùa tuyển sinh, hệ thống **"Website hỗ trợ tra cứu thông tin các trường đại học và cao đẳng Việt Nam"** được xây dựng nhằm giải quyết các vấn đề:

- **Tra cứu thông tin trường:** Người dùng có thể tìm kiếm trường đại học, cao đẳng theo nhiều tiêu chí như tên trường, tỉnh/thành phố, khu vực, loại trường (công lập/tư thục/quốc tế) và ngành đào tạo.
- **Tra cứu điểm chuẩn:** Hệ thống cung cấp thông tin điểm chuẩn qua các năm, hỗ trợ biểu đồ xu hướng trực quan giúp thí sinh đánh giá cơ hội trúng tuyển.
- **Quản lý tin tuyển sinh:** Admin cập nhật thông tin tuyển sinh bao gồm chỉ tiêu, điểm chuẩn, phương thức xét tuyển, học phí, tổ hợp môn thi cho từng ngành của từng trường.
- **Hỗ trợ tư vấn:** Thí sinh có thể gửi yêu cầu tư vấn trực tiếp đến trường quan tâm.
- **Đánh giá trường:** Học sinh đã đăng nhập có thể đánh giá và nhận xét về trường.
- **Danh sách yêu thích:** Học sinh có thể lưu các trường quan tâm vào danh sách riêng để theo dõi.
- **Bài viết tin tức:** Hệ thống quản lý bài viết tin tức, thông báo tuyển sinh liên quan đến các trường.
- **Gợi ý trường phù hợp:** Dựa vào điểm dự kiến và ngành quan tâm trong hồ sơ cá nhân, hệ thống gợi ý các trường phù hợp cho thí sinh.

## 1.3. Yêu cầu hệ thống

### 1.3.1 Yêu cầu chức năng

**Phía Admin:**
- Đăng nhập với phân quyền (Admin / Học sinh)
- Quản lý tài khoản người dùng (xem danh sách, kích hoạt/khóa tài khoản)
- Quản lý thông tin trường học (thêm, sửa, xóa trường, upload ảnh đại diện/ảnh bìa)
- Quản lý tin tuyển sinh (thêm, sửa, xóa thông tin điểm chuẩn, chỉ tiêu)
- Quản lý bài viết (thêm, sửa, xóa, ẩn/hiện bài viết)
- Quản lý yêu cầu tư vấn và đánh giá (xem, xử lý, duyệt)
- Xem log hệ thống (lịch sử hành động của người dùng)
- Dashboard thống kê tổng quan

**Phía thí sinh / học sinh:**
- Đăng ký tài khoản mới (yêu cầu xác nhận email)
- Đăng nhập / Đăng xuất
- Quên mật khẩu (gửi email đặt lại)
- Tìm kiếm trường theo đa tiêu chí
- Xem chi tiết trường (giới thiệu, ngành đào tạo, điểm chuẩn, tin tuyển sinh, bài viết, đánh giá)
- Tra cứu điểm chuẩn tổng hợp
- Tìm kiếm theo ngành
- Xem biểu đồ xu hướng điểm chuẩn
- Gửi yêu cầu tư vấn
- Đánh giá trường (1-5 sao + nhận xét)
- Quản lý danh sách trường yêu thích (WishList)
- Quản lý hồ sơ cá nhân + đổi mật khẩu
- So sánh trường
- Xem bài viết tin tức

### 1.3.2 Yêu cầu phi chức năng

- **Hiệu suất:** Hệ thống phải phản hồi nhanh, thời gian load trang không quá 2 giây. Sử dụng Stored Procedure và Covering Index để tối ưu truy vấn SQL Server.
- **Tính sẵn sàng:** Hệ thống hoạt động 24/7, database sử dụng Read Committed Snapshot Isolation để tránh deadlock khi nhiều người dùng truy cập đồng thời.
- **Bảo mật:** Mật khẩu được hash SHA-256 với salt, chống brute-force (khóa sau 5 lần sai), chống SQL Injection (dùng SqlParameter), chống XSS (HtmlEncode), cookie HttpOnly.
- **Tính mở rộng:** Kiến trúc 3 lớp (Presentation – BLL – DAL) cho phép dễ dàng mở rộng chức năng mà không ảnh hưởng đến các lớp khác.
- **SEO thân thiện:** URL slug cho trang chi tiết trường và bài viết.
- **Responsive:** Giao diện hỗ trợ đa thiết bị nhờ Bootstrap 5.

---

# CHƯƠNG 2. CƠ SỞ LÝ THUYẾT

## 2.1. Giới thiệu ASP.NET Web Forms

ASP.NET là một mã nguồn mở dành cho web được tạo bởi Microsoft, chạy trên nền tảng .NET Framework. ASP.NET cho phép các nhà phát triển tạo các ứng dụng web, dịch vụ web và các trang web động.

Hệ thống sử dụng **ASP.NET Web Forms** — mô hình lập trình event-driven quen thuộc, sử dụng file `.aspx` cho giao diện và file `.aspx.cs` (code-behind) cho logic xử lý. Web Forms cung cấp:

- **Server Controls:** TextBox, GridView, Repeater, DropDownList — được render thành HTML tương ứng
- **Master Pages:** Template dùng chung cho toàn bộ trang (header, footer, navigation)
- **ViewState:** Tự động duy trì trạng thái controls giữa các lần PostBack
- **Validation Controls:** RequiredFieldValidator, RegexValidator — validate phía server và client

ASP.NET Web Forms được biên dịch dưới dạng Common Language Runtime (CLR), hỗ trợ viết code bằng C#, VB.NET. Mã nguồn được biên dịch thành DLL, chạy tốc độ nhanh hơn so với mã thông dịch.

## 2.2. Ngôn ngữ lập trình C#

C# (C-Sharp) là ngôn ngữ lập trình hướng đối tượng do Microsoft phát triển, chạy trên nền .NET Framework. C# có cú pháp rõ ràng, mạnh về kiểu dữ liệu (strongly typed), hỗ trợ đầy đủ tính kế thừa, đa hình, đóng gói.

Trong hệ thống này, C# được sử dụng để:
- Viết code-behind xử lý sự kiện trên trang `.aspx.cs`
- Xây dựng lớp Business Logic (BLL) và Data Access (DAL)
- Tạo các Model (POCO class) ánh xạ dữ liệu
- Xử lý bảo mật (hash mật khẩu, sinh token, quản lý phiên)

## 2.3. HTML – CSS – JavaScript – Bootstrap

**HTML** (HyperText Markup Language) là ngôn ngữ đánh dấu siêu văn bản, được sử dụng để xây dựng cấu trúc trang web. Mỗi trang web trong hệ thống đều được render ra HTML thông qua ASP.NET Web Forms controls.

**CSS** (Cascading Style Sheets) là ngôn ngữ tạo phong cách cho website, quy định cách các thành phần HTML hiển thị. Hệ thống sử dụng:
- `Content/Client.css` — CSS cho phía Client (thí sinh)
- `Admin/Admin.css` — CSS cho phía Admin
- Bootstrap 5 CDN — Framework CSS responsive

**JavaScript** là ngôn ngữ lập trình phía client, được sử dụng trong hệ thống để:
- Xử lý tương tác giao diện (preview ảnh trước khi upload, auto-generate slug)
- Vẽ biểu đồ xu hướng điểm chuẩn bằng Chart.js
- Tăng trải nghiệm người dùng (tooltip, modal, tab)

**Bootstrap 5** là framework CSS phổ biến, cung cấp hệ thống grid responsive, components sẵn có (card, modal, table, badge, pagination), giúp xây dựng giao diện đẹp và thống nhất trên mọi thiết bị.

## 2.4. SQL Server

SQL Server (Microsoft SQL Server) là hệ quản trị cơ sở dữ liệu quan hệ (RDBMS) được sử dụng để lưu trữ và quản lý toàn bộ dữ liệu của hệ thống.

**Ưu điểm:**
- **Hiệu suất cao:** Hỗ trợ Covering Index, Filtered Index, Query Store để tối ưu hiệu năng truy vấn
- **Quản lý dữ liệu dễ dàng:** Hỗ trợ Stored Procedure, Transaction, ACID, sao lưu và phục hồi
- **Bảo mật mạnh mẽ:** Phân quyền người dùng, mã hóa dữ liệu, kiểm soát truy cập
- **Tích hợp tốt:** Tương thích hoàn hảo với .NET Framework, Visual Studio, ADO.NET

**Cấu hình database sử dụng:**
- `READ_COMMITTED_SNAPSHOT ON` — Tránh deadlock khi nhiều người dùng đồng thời
- `RECOVERY SIMPLE` — Giữ log nhỏ gọn cho môi trường development
- `QUERY_STORE = ON` — Theo dõi và tối ưu hiệu năng truy vấn
- Compatibility Level 160 (SQL Server 2022)

## 2.5. Kiến trúc phần mềm 3-Layer Architecture

Hệ thống được tổ chức theo mô hình **3 lớp (3-Layer Architecture)** nhằm tách biệt rõ ràng giữa giao diện, nghiệp vụ và truy cập dữ liệu:

| Lớp | Vị trí trong project | Vai trò |
|---|---|---|
| **Presentation Layer** (UI) | File `.aspx` + code-behind `.aspx.cs` | Nhận input từ người dùng, hiển thị kết quả |
| **Business Logic Layer** (BLL) | `AppCode/BLL/*.cs` | Xử lý nghiệp vụ, validate dữ liệu, điều phối luồng |
| **Data Access Layer** (DAL) | `AppCode/DAL/*.cs` | Thực thi truy vấn SQL Server, ánh xạ dữ liệu vào Model |

**Sơ đồ luồng dữ liệu:**

```
  Người dùng (Browser)
        │
        │  HTTP Request
        ▼
  ┌─────────────────────┐
  │  Presentation Layer │  ← .aspx + .aspx.cs (Web Forms)
  │  (Default.aspx,     │
  │   Login.aspx, ...)  │
  └─────────┬───────────┘
            │  Gọi phương thức BLL
            ▼
  ┌─────────────────────┐
  │  Business Logic     │  ← AppCode/BLL/*.cs
  │  Layer (BLL)        │     (TaiKhoanBLL, TruongBLL, ...)
  │  - Validate input   │
  │  - Xử lý nghiệp vụ │
  └─────────┬───────────┘
            │  Gọi phương thức DAL
            ▼
  ┌─────────────────────┐
  │  Data Access Layer  │  ← AppCode/DAL/*.cs
  │  (DAL)              │     (TaiKhoanDAL, TruongDAL, ...)
  │  - Tạo SqlParameter │
  │  - Gọi DBHelper     │
  └─────────┬───────────┘
            │  SQL / Stored Procedure
            ▼
  ┌─────────────────────┐
  │    SQL Server        │
  │  (Database TCTS)    │
  └─────────────────────┘
```

**Ưu điểm kiến trúc 3 lớp:**
- **Tách biệt trách nhiệm:** Mỗi lớp có vai trò riêng biệt, dễ bảo trì
- **Tái sử dụng:** BLL và DAL có thể dùng cho nhiều trang khác nhau
- **Kiểm thử dễ dàng:** Có thể test từng lớp độc lập
- **Mở rộng linh hoạt:** Thay đổi database hoặc giao diện không ảnh hưởng lớp khác

**Các lớp tiện ích dùng chung:**
- `DBHelper.cs` — Lớp tĩnh cung cấp phương thức giao tiếp SQL Server (Query, Execute, Scalar, ExecSP)
- `SecurityHelper.cs` — Hash mật khẩu SHA-256, sinh token, FormsAuthentication
- `EmailHelper.cs` — Gửi email xác nhận và đặt lại mật khẩu
- `SlugHelper.cs` — Tạo URL thân thiện SEO từ tiếng Việt

---

# CHƯƠNG 3. PHÂN TÍCH THIẾT KẾ HỆ THỐNG

## 3.1. Sơ đồ phân rã chức năng

Hệ thống được chia thành 2 nhóm chức năng chính: **Quản trị (Admin)** và **Thí sinh (Client)**.

```
Hệ thống Tra Cứu Tuyển Sinh
├── QUẢN TRỊ (Admin)
│   ├── Đăng nhập Admin
│   ├── Dashboard thống kê
│   ├── Quản lý trường học
│   │   ├── Danh sách trường
│   │   ├── Thêm trường mới
│   │   ├── Sửa thông tin trường
│   │   └── Xóa trường
│   ├── Quản lý tin tuyển sinh
│   │   ├── Danh sách tin
│   │   ├── Thêm/Sửa/Xóa tin
│   │   └── Ẩn/Hiện tin
│   ├── Quản lý bài viết
│   │   ├── Danh sách bài viết
│   │   ├── Thêm/Sửa/Xóa bài viết
│   │   └── Ẩn/Hiện bài viết
│   ├── Quản lý tài khoản
│   │   ├── Danh sách tài khoản
│   │   └── Kích hoạt/Khóa tài khoản
│   ├── Quản lý tư vấn & đánh giá
│   │   ├── Xem yêu cầu tư vấn
│   │   ├── Duyệt đánh giá
│   │   └── Đánh dấu đã xử lý
│   └── Xem log hệ thống
│
└── THÍ SINH (Client)
    ├── Tài khoản
    │   ├── Đăng ký (xác nhận email)
    │   ├── Đăng nhập / Đăng xuất
    │   ├── Quên mật khẩu
    │   └── Đặt lại mật khẩu
    ├── Tra cứu thông tin
    │   ├── Tìm kiếm trường
    │   ├── Tìm kiếm theo ngành
    │   ├── Tra cứu điểm chuẩn
    │   ├── Xem chi tiết trường
    │   ├── So sánh trường
    │   └── Xem bài viết
    ├── Tương tác
    │   ├── Đánh giá trường (1-5 sao)
    │   ├── Gửi yêu cầu tư vấn
    │   └── Lưu trường yêu thích
    └── Hồ sơ cá nhân
        ├── Cập nhật thông tin
        ├── Đổi mật khẩu
        └── Gợi ý trường phù hợp
```

## 3.2. Sơ đồ ngữ cảnh

Hệ thống có 2 tác nhân chính tương tác:

```
                    ┌─────────────┐
  Thí sinh ────────▶│  Hệ thống   │◀──────── Admin
  (Client)          │  Tra Cứu    │          (Quản trị)
                    │  Tuyển Sinh  │
                    └──────┬──────┘
                           │
                    ┌──────▼──────┐
                    │  SQL Server  │
                    │  Database    │
                    └─────────────┘
```

- **Thí sinh:** Tra cứu thông tin, tìm kiếm trường, xem điểm chuẩn, đăng ký tư vấn, đánh giá trường
- **Admin:** Quản lý toàn bộ dữ liệu hệ thống (trường, tin tuyển sinh, bài viết, tài khoản, tư vấn)

## 3.3. Sơ đồ mức đỉnh

```
                         ┌──────────────────┐
  Thí sinh ──────────────▶ 1. Quản lý       │
                         │    Tài khoản      │
  Admin ─────────────────▶    (Đăng nhập,    │
                         │    Đăng ký, ...)  │
                         └────────┬─────────┘
                                  │
                    ┌─────────────┼──────────────┐
                    ▼             ▼               ▼
             ┌──────────┐  ┌──────────┐    ┌──────────┐
             │ 2. Tra   │  │ 3. Quản  │    │ 4. Quản  │
             │ cứu      │  │ lý       │    │ lý       │
             │ thông tin │  │ trường   │    │ nội dung │
             └──────────┘  └──────────┘    └──────────┘
```

## 3.4. Sơ đồ mức dưới đỉnh

### 3.4.1 Chức năng Đăng nhập

```
Người dùng ──▶ [1.1 Nhập thông tin đăng nhập]
                        │
                        ▼
              [1.2 Hash mật khẩu SHA-256]
                        │
                        ▼
              [1.3 Gọi SP sp_TaiKhoan_DangNhap]
                        │
              ┌─────────┼──────────┐
              ▼         ▼          ▼
         Thành công  Sai MK    Bị khóa
         (Tạo        (Tăng    (Hiển thị
          cookie      đếm      cảnh báo)
          Auth)       lỗi)
```

### 3.4.2 Chức năng Quản lý Trường

```
Admin ──▶ [3.1 Danh sách trường (GridView + phân trang)]
                │
        ┌───────┼───────┬──────────┐
        ▼       ▼       ▼          ▼
   [3.2 Thêm] [3.3 Sửa] [3.4 Xóa] [3.5 Tìm kiếm]
        │       │
        ▼       ▼
   [3.6 Upload ảnh đại diện + ảnh bìa]
        │
        ▼
   [3.7 Tự sinh Slug SEO]
        │
        ▼
   [3.8 Lưu vào tbl_Truong]
```

### 3.4.3 Chức năng Tra cứu Điểm chuẩn

```
Thí sinh ──▶ [2.1 Chọn bộ lọc (trường, ngành, năm, phương thức)]
                        │
                        ▼
              [2.2 Gọi SP sp_TinTuyenSinh_TimKiem]
                        │
                        ▼
              [2.3 Hiển thị kết quả GridView + phân trang]
                        │
                        ▼
              [2.4 Click chi tiết trường → biểu đồ Chart.js]
```

## 3.5. Thiết kế cơ sở dữ liệu

### 3.5.1 tbl_Quyen

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaQuyen | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TenQuyen | NVARCHAR(100) | NOT NULL |

### 3.5.2 tbl_TaiKhoan

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaTaiKhoan | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | Email | NVARCHAR(100) | NOT NULL, UNIQUE |
| 3 | MatKhau | NVARCHAR(200) | NOT NULL (SHA-256 hash) |
| 4 | MaQuyen | INT | NOT NULL (FK → tbl_Quyen) |
| 5 | NgayTao | DATETIME | DEFAULT GETDATE() |
| 6 | TrangThai | BIT | DEFAULT 1 (1=Active) |
| 7 | SoLanDangNhapSai | TINYINT | DEFAULT 0 (Brute-force protect) |
| 8 | KhoaTaiKhoanDen | DATETIME | NULL (Thời gian hết khóa tạm) |
| 9 | LanDangNhapCuoi | DATETIME | NULL |
| 10 | EmailDaXacNhan | BIT | DEFAULT 0 |
| 11 | TokenXacNhanEmail | NVARCHAR(100) | NULL |
| 12 | TokenDatLaiMatKhau | NVARCHAR(100) | NULL |
| 13 | TokenHetHan | DATETIME | NULL (Hết hạn 2 giờ) |
| 14 | TokenNhoMatKhau | NVARCHAR(200) | NULL |

### 3.5.3 tbl_CapBac

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaCapBac | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TenCapBac | NVARCHAR(100) | NOT NULL |

### 3.5.4 tbl_DanhMucNganh

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaDanhMuc | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TenDanhMuc | NVARCHAR(200) | NOT NULL |
| 3 | ThuTu | INT | DEFAULT 0 |

### 3.5.5 tbl_ChuyenNganh

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaChuyenNganh | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TenChuyenNganh | NVARCHAR(200) | NOT NULL |
| 3 | MaDanhMuc | INT | NOT NULL (FK → tbl_DanhMucNganh) |

### 3.5.6 tbl_PhuongThucXetTuyen

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaPhuongThuc | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TenPhuongThuc | NVARCHAR(200) | NOT NULL |
| 3 | MoTa | NVARCHAR(500) | NULL |
| 4 | ThuTu | INT | DEFAULT 0 |

### 3.5.7 tbl_Truong

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaTruong | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TenTruong | NVARCHAR(200) | NOT NULL |
| 3 | DiaChi | NVARCHAR(300) | NULL |
| 4 | TinhThanh | NVARCHAR(100) | NULL |
| 5 | MaVung | TINYINT | NULL (1=Bắc, 2=Trung, 3=Nam) |
| 6 | LoaiTruong | TINYINT | NULL (1=Công lập, 2=Tư thục, 3=Quốc tế) |
| 7 | SoDienThoai | NVARCHAR(20) | NULL |
| 8 | Website | NVARCHAR(300) | NULL |
| 9 | AnhDaiDien | NVARCHAR(300) | NULL |
| 10 | AnhBia | NVARCHAR(300) | NULL |
| 11 | MoTa | NVARCHAR(MAX) | NULL |
| 12 | QuyMo | NVARCHAR(100) | NULL |
| 13 | KiemDinhChatLuong | BIT | DEFAULT 0 |
| 14 | Slug | NVARCHAR(200) | NULL, UNIQUE (SEO URL) |
| 15 | MaTaiKhoan | INT | NOT NULL (FK → tbl_TaiKhoan) |
| 16 | ThoiGianCapNhat | DATETIME | DEFAULT GETDATE() |

### 3.5.8 tbl_TruongChuyenNganh

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | ID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTruong | INT | NOT NULL (FK → tbl_Truong) |
| 3 | MaChuyenNganh | INT | NOT NULL (FK → tbl_ChuyenNganh) |
| 4 | MaCapBac | INT | NOT NULL (FK → tbl_CapBac) |
| 5 | ChiTieu | INT | NULL |

### 3.5.9 tbl_TinTuyenSinh

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaTin | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTruong | INT | NOT NULL (FK → tbl_Truong) |
| 3 | MaChuyenNganh | INT | NOT NULL (FK → tbl_ChuyenNganh) |
| 4 | MaPhuongThuc | INT | NOT NULL (FK → tbl_PhuongThucXetTuyen) |
| 5 | NamTuyenSinh | SMALLINT | NOT NULL |
| 6 | ChiTieu | INT | NULL |
| 7 | HocPhi | DECIMAL(12,2) | NULL |
| 8 | ToHopMonHoc | NVARCHAR(200) | NULL |
| 9 | DiemChuanNamTruoc | DECIMAL(4,2) | NULL |
| 10 | DiemChuanNamNay | DECIMAL(4,2) | NULL |
| 11 | ChenhLechDiem | DECIMAL(4,2) | NULL |
| 12 | LoaiHinhDaoTao | NVARCHAR(100) | NULL |
| 13 | CoSoDaoTao | NVARCHAR(200) | NULL |
| 14 | MoTa | NVARCHAR(MAX) | NULL |
| 15 | NgayDang | DATETIME | DEFAULT GETDATE() |
| 16 | LuotXem | INT | DEFAULT 0 |
| 17 | TrangThai | BIT | DEFAULT 1 |

### 3.5.10 tbl_DiemChuanLichSu

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | ID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTruong | INT | NOT NULL (FK → tbl_Truong) |
| 3 | MaChuyenNganh | INT | NOT NULL (FK → tbl_ChuyenNganh) |
| 4 | MaPhuongThuc | INT | NOT NULL (FK → tbl_PhuongThucXetTuyen) |
| 5 | NamTuyenSinh | SMALLINT | NOT NULL |
| 6 | DiemChuan | DECIMAL(4,2) | NULL |
| 7 | ChiTieu | INT | NULL |
| 8 | GhiChu | NVARCHAR(300) | NULL |

### 3.5.11 tbl_ProfileHocSinh

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | ID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTaiKhoan | INT | NOT NULL, UNIQUE (FK → tbl_TaiKhoan) |
| 3 | HoTen | NVARCHAR(100) | NULL |
| 4 | NgaySinh | DATE | NULL |
| 5 | TinhThanh | NVARCHAR(100) | NULL |
| 6 | DiemDuKien | DECIMAL(4,2) | NULL |
| 7 | DiemMonHoc | NVARCHAR(MAX) | NULL (JSON) |
| 8 | MaChuyenNganh | INT | NULL (FK → tbl_ChuyenNganh) |
| 9 | MucTieuNghe | NVARCHAR(200) | NULL |
| 10 | KhuVuc | TINYINT | NULL (1=KV1, 2=KV2, 3=KV3) |
| 11 | AnhDaiDien | NVARCHAR(300) | NULL |
| 12 | NgayCapNhat | DATETIME | DEFAULT GETDATE() |

### 3.5.12 tbl_WishList

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | ID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTaiKhoan | INT | NOT NULL (FK → tbl_TaiKhoan) |
| 3 | MaTruong | INT | NOT NULL (FK → tbl_Truong) |
| 4 | MaChuyenNganh | INT | NULL (FK → tbl_ChuyenNganh) |
| 5 | GhiChu | NVARCHAR(300) | NULL |
| 6 | NgayThem | DATETIME | DEFAULT GETDATE() |

### 3.5.13 tbl_TuVan

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | ID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTaiKhoan | INT | NULL (cho phép khách chưa đăng nhập) |
| 3 | MaTruong | INT | NOT NULL (FK → tbl_Truong) |
| 4 | HoTen | NVARCHAR(100) | NOT NULL |
| 5 | Email | NVARCHAR(100) | NOT NULL |
| 6 | SoDienThoai | NVARCHAR(20) | NULL |
| 7 | NoiDung | NVARCHAR(MAX) | NOT NULL |
| 8 | TrangThai | TINYINT | DEFAULT 0 (0=Chờ, 1=Đã phản hồi) |
| 9 | NgayGui | DATETIME | DEFAULT GETDATE() |
| 10 | NgayPhanHoi | DATETIME | NULL |
| 11 | GhiChuAdmin | NVARCHAR(500) | NULL |

### 3.5.14 tbl_DanhGiaTruong

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | ID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTruong | INT | NOT NULL (FK → tbl_Truong) |
| 3 | MaTaiKhoan | INT | NOT NULL (FK → tbl_TaiKhoan) |
| 4 | DiemDanhGia | TINYINT | NOT NULL, CHECK (1-5) |
| 5 | NoiDung | NVARCHAR(MAX) | NULL |
| 6 | TrangThai | TINYINT | DEFAULT 0 (0=Chờ duyệt, 1=Hiện) |
| 7 | NgayDang | DATETIME | DEFAULT GETDATE() |

### 3.5.15 tbl_BaiViet

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaBaiViet | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | TieuDe | NVARCHAR(200) | NOT NULL |
| 3 | Slug | NVARCHAR(200) | NULL, UNIQUE |
| 4 | AnhChinh | NVARCHAR(300) | NULL |
| 5 | NoiDung | NVARCHAR(MAX) | NULL |
| 6 | MaTacGia | INT | NOT NULL (FK → tbl_TaiKhoan) |
| 7 | NgayDang | DATETIME | DEFAULT GETDATE() |
| 8 | LuotXem | INT | DEFAULT 0 |
| 9 | TrangThai | BIT | DEFAULT 1 |
| 10 | MaTruong | INT | NULL (FK → tbl_Truong) |

### 3.5.16 tbl_SearchHistory

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | MaSearch | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTaiKhoan | INT | NULL (FK → tbl_TaiKhoan) |
| 3 | MaChuyenNganh | INT | NULL (FK → tbl_ChuyenNganh) |
| 4 | MaTruong | INT | NULL (FK → tbl_Truong) |
| 5 | ThoiGian | DATETIME | DEFAULT GETDATE() |

### 3.5.17 tbl_Logs

| STT | Tên Thuộc Tính | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| 1 | LogID | INT | IDENTITY(1,1) NOT NULL (Primary Key) |
| 2 | MaTaiKhoan | INT | NULL (FK → tbl_TaiKhoan) |
| 3 | HanhDong | NVARCHAR(200) | NOT NULL |
| 4 | BangTacDong | NVARCHAR(100) | NULL |
| 5 | ThoiGian | DATETIME | DEFAULT GETDATE() |
| 6 | IPAddress | NVARCHAR(50) | NULL |
| 7 | MoTa | NVARCHAR(MAX) | NULL |
| 8 | SessionID | NVARCHAR(100) | NULL |
| 9 | TrangUrl | NVARCHAR(300) | NULL |
| 10 | UserAgent | NVARCHAR(300) | NULL |
| 11 | LoaiThietBi | NVARCHAR(50) | NULL |
| 12 | IsSuccess | BIT | DEFAULT 1 |
| 13 | MaLoi | NVARCHAR(50) | NULL |

## 3.6. Database Diagram

Hệ thống gồm **17 bảng** với các mối quan hệ chính:

```
tbl_Quyen ──1:N──▶ tbl_TaiKhoan ──1:N──▶ tbl_ProfileHocSinh
                        │                         │
                        │──1:N──▶ tbl_WishList     │
                        │──1:N──▶ tbl_Logs         │
                        │──1:N──▶ tbl_BaiViet      │
                        │                         │
                        ▼                         │
               tbl_Truong ──1:N──▶ tbl_TruongChuyenNganh
                  │                       │
                  │──1:N──▶ tbl_TinTuyenSinh ◀── tbl_PhuongThucXetTuyen
                  │──1:N──▶ tbl_DiemChuanLichSu
                  │──1:N──▶ tbl_TuVan
                  │──1:N──▶ tbl_DanhGiaTruong
                  │──1:N──▶ tbl_GopY
                  │
tbl_DanhMucNganh ──1:N──▶ tbl_ChuyenNganh ◀──── tbl_TruongChuyenNganh
                                    │
tbl_CapBac ─────────────────────────┘
```

---

# CHƯƠNG 4. THIẾT KẾ CHƯƠNG TRÌNH

## 4.1. Admin

### 4.1.1 Đăng nhập Admin

Admin truy cập URL `/Admin/` → nếu chưa đăng nhập sẽ được redirect về `/Client/Login.aspx`. Sau khi đăng nhập thành công với role "Admin", cookie FormsAuthentication được tạo và admin được phép truy cập toàn bộ thư mục `/Admin/`.

**Bảo vệ thư mục Admin:** File `Admin/Web.config` sử dụng `<authorization>` cho phép chỉ role "Admin", từ chối tất cả user khác.

### 4.1.2 Dashboard (Trang chủ Admin)

Trang `Admin/Default.aspx` hiển thị:
- **4 thẻ thống kê:** Tổng số trường, tổng tin tuyển sinh, số yêu cầu tư vấn chờ xử lý, tổng tài khoản
- **Danh sách 5 trường mới cập nhật** (liên kết nhanh đến trang chỉnh sửa)
- **Danh sách 5 yêu cầu tư vấn mới nhất** chưa xử lý

### 4.1.3 Quản lý Trường học

**Trang danh sách:** `Admin/QuanLyTruong.aspx`
- GridView hiển thị danh sách trường với phân trang
- Tìm kiếm theo tên trường
- Nút thêm mới, sửa, xóa (có xác nhận)

**Trang thêm/sửa:** `Admin/ChinhSuaTruong.aspx`
- Form nhập liệu đầy đủ: tên, slug SEO, địa chỉ, tỉnh thành, khu vực, loại trường, điện thoại, website, quy mô
- Upload ảnh đại diện (100×100px, tối đa 5MB)
- Upload ảnh bìa (1200×400px, tối đa 10MB)
- Preview ảnh trước khi upload
- Click ảnh → mở tab mới xem ảnh gốc đầy đủ
- Checkbox kiểm định chất lượng
- Textarea mô tả (hỗ trợ HTML)
- Slug tự động sinh từ tên trường nếu để trống

### 4.1.4 Quản lý Tin tuyển sinh

Trang `Admin/QuanLyTinTuyenSinh.aspx` cho phép:
- Thêm/sửa/xóa thông tin tuyển sinh theo từng ngành, từng năm
- Filter theo trường, ngành, năm, phương thức
- Modal thêm/sửa với đầy đủ thông tin: trường, ngành, phương thức xét tuyển, năm, chỉ tiêu, điểm chuẩn, học phí, tổ hợp môn, loại hình đào tạo

### 4.1.5 Quản lý Bài viết

Trang `Admin/QuanLyBaiViet.aspx` cho phép:
- Thêm/sửa/xóa bài viết tin tức
- Filter theo trường liên quan, trạng thái (hiện/ẩn)
- Modal thêm/sửa: tiêu đề, slug (tự sinh), trường liên quan, ảnh bìa, nội dung HTML
- Ẩn/hiện bài viết (toggle trạng thái)
- Preview ảnh bìa + click xem full

### 4.1.6 Quản lý Tài khoản

Trang `Admin/QuanLyTaiKhoan.aspx`:
- Danh sách tài khoản với filter theo quyền (Admin/Học sinh) và từ khóa email
- Kích hoạt / Khóa tài khoản
- Phân trang

### 4.1.7 Quản lý Tư vấn & Đánh giá

Trang `Admin/QuanLyGopYTuVan.aspx`:
- Xem danh sách yêu cầu tư vấn với filter theo trạng thái và trường
- Duyệt / Đánh dấu đã xử lý
- Xem đánh giá trường, duyệt hiển thị

### 4.1.8 Xem Log hệ thống

Trang `Admin/QuanLyLogs.aspx`:
- Xem lịch sử hành động người dùng
- Filter theo hành động, thời gian, tài khoản
- Hiển thị IP, thiết bị, URL, trạng thái (thành công/lỗi)

## 4.2. Học sinh / Thí sinh

### 4.2.1 Trang chủ

Trang `Client/index.aspx` hiển thị:
- **Hero search box:** Ô tìm kiếm nổi bật
- **Thống kê hệ thống:** Số trường, số ngành, số tin tuyển sinh, năm mới nhất
- **8 trường nổi bật:** Các trường đã kiểm định chất lượng
- **10 tin tuyển sinh mới nhất**

### 4.2.2 Đăng ký

Trang `Client/DangKy.aspx`:
- Nhập email + mật khẩu (tối thiểu 6 ký tự)
- Mặc định role "Học sinh"
- Gửi email xác nhận sau khi đăng ký
- Tài khoản chỉ được đăng nhập sau khi xác nhận email

### 4.2.3 Đăng nhập

Trang `Client/Login.aspx`:
- Nhập email + mật khẩu
- Checkbox "Nhớ mật khẩu" (30 ngày)
- Redirect về trang trước đó sau khi đăng nhập
- Hiển thị lỗi cụ thể: sai mật khẩu, bị khóa tạm, chưa xác nhận email

### 4.2.4 Quên mật khẩu

Trang `Client/QuenMatKhau.aspx` → nhập email → gửi link reset.
Trang `Client/DatLaiMatKhau.aspx` → nhập mật khẩu mới (token hết hạn 2 giờ).

### 4.2.5 Tìm kiếm Trường

Trang `Client/TimKiemTruong.aspx`:
- Filter đa tiêu chí: từ khóa, tỉnh/thành, khu vực, loại trường, ngành
- Kết quả hiển thị dạng card với ảnh đại diện, tên trường, tỉnh thành, badge kiểm định
- Phân trang (12 trường/trang)

### 4.2.6 Tìm kiếm theo Ngành

Trang `Client/TimKiemTheoNganh.aspx`:
- Chọn danh mục ngành → chọn ngành cụ thể
- Hiển thị danh sách trường có đào tạo ngành đó

### 4.2.7 Xem Chi tiết Trường

Trang `Client/ChiTietTruong.aspx?slug=xxx`:
- **Ảnh bìa** + **logo trường** (click xem full trong tab mới)
- **6 tab thông tin:**
  - *Giới thiệu:* Mô tả chi tiết trường
  - *Ngành đào tạo:* GridView danh sách ngành (lĩnh vực, tên ngành, bậc, chỉ tiêu)
  - *Điểm chuẩn:* Bảng điểm + biểu đồ xu hướng Chart.js (vẽ lazy khi click tab)
  - *Tin tuyển sinh:* Danh sách tin tuyển sinh mới nhất
  - *Bài viết:* Bài viết liên quan đến trường
  - *Đánh giá:* Danh sách review + form gửi đánh giá (cần đăng nhập)
- **Sidebar:** Thông tin nhanh, nút yêu thích, form đăng ký tư vấn

### 4.2.8 Tra cứu Điểm chuẩn

Trang `Client/TraCuuDiemChuan.aspx`:
- Filter: trường, ngành, năm, phương thức, khoảng điểm
- GridView hiển thị kết quả với phân trang
- Cột: tên trường, tên ngành, phương thức, năm, tổ hợp, điểm chuẩn, chỉ tiêu, học phí

### 4.2.9 So sánh Trường

Trang `Client/SoSanhTruong.aspx`:
- Chọn 2 trường để so sánh song song
- So sánh: loại trường, khu vực, điểm đánh giá, kiểm định, quy mô, ngành đào tạo

### 4.2.10 Xem Bài viết

Trang `Client/BaiViet.aspx` — danh sách bài viết.
Trang `Client/ChiTietBaiViet.aspx?slug=xxx`:
- Ảnh bìa bài viết (click xem full)
- Nội dung HTML
- Bài viết liên quan
- Sidebar: bài viết mới nhất

### 4.2.11 Hồ sơ cá nhân

Trang `Client/MyProfile.aspx`:
- Cập nhật: họ tên, ngày sinh, tỉnh thành, khu vực ưu tiên, điểm dự kiến, ngành quan tâm, mục tiêu nghề nghiệp
- Upload avatar (click xem full)
- Đổi mật khẩu (nhập mật khẩu cũ + mới)
- **Gợi ý trường phù hợp** dựa trên điểm dự kiến và ngành quan tâm

### 4.2.12 Danh sách Yêu thích

Trang `Client/WishList.aspx`:
- Danh sách trường đã yêu thích
- Nút xóa từng mục
- Liên kết đến trang chi tiết trường

## 4.3. Các luồng xử lý chính

### 4.3.1 Luồng Đăng nhập

```
Bước 1: Người dùng nhập email + mật khẩu → Nhấn "Đăng nhập"
Bước 2: Code-behind gọi TaiKhoanBLL.DangNhap(email, matKhau)
Bước 3: BLL hash mật khẩu SHA-256 → gọi TaiKhoanDAL.DangNhap()
Bước 4: DAL gọi SP sp_TaiKhoan_DangNhap:
         - Kiểm tra tài khoản tồn tại
         - Kiểm tra khóa tạm thời (brute-force 30 phút)
         - So sánh hash mật khẩu
         - Kiểm tra email đã xác nhận
         - Trả về @KetQua, @MaTaiKhoan, @MaQuyen
Bước 5: Nếu thành công → SecurityHelper.SignIn() tạo cookie → redirect
         Nếu thất bại → hiển thị lỗi cụ thể
```

### 4.3.2 Luồng Tìm kiếm Trường

```
Bước 1: Người dùng nhập từ khóa + chọn filter → PostBack
Bước 2: Code-behind gọi TruongBLL.TimKiem(...)
Bước 3: BLL → DAL gọi SP sp_Truong_TimKiem:
         - WHERE động theo tham số không NULL
         - Tính @TongSo OUTPUT cho phân trang
         - OFFSET/FETCH NEXT phân trang
Bước 4: DAL trả về PagedTable → bind vào Repeater + render phân trang
```

### 4.3.3 Luồng Phân quyền Admin

```
Bước 1: Truy cập URL /Admin/...
Bước 2: Global.asax đọc cookie → giải mã FormsAuthenticationTicket
         → tạo GenericPrincipal với role từ ticket.UserData
Bước 3: ASP.NET kiểm tra Web.config /Admin/ → <allow roles="Admin" />
Bước 4: Nếu không phải Admin → redirect về Login hoặc trang lỗi 403
```

---

# CHƯƠNG 5. BẢO MẬT HỆ THỐNG

| Cơ chế | Chi tiết |
|---|---|
| **Hash mật khẩu** | SHA-256 kết hợp static salt `"TCTS@2026#Salt!"`, không lưu plain text |
| **Chống brute-force** | SP theo dõi `SoLanDangNhapSai`, khóa tạm 30 phút sau 5 lần sai |
| **Xác thực email** | Token 64 byte `RandomNumberGenerator` (cryptographically secure) gửi qua email |
| **Quên mật khẩu** | Token reset hết hạn sau 2 giờ, SP kiểm tra trước khi cho đổi |
| **FormsAuthentication** | Ticket mã hóa, cookie `HttpOnly=true`, timeout 1h (hoặc 30 ngày nếu Remember Me) |
| **Phân quyền** | `/Admin/Web.config` chỉ cho role "Admin", Global.asax gán role vào Principal |
| **Chống XSS** | `Server.HtmlEncode()` cho mọi dữ liệu hiển thị từ database |
| **Chống SQL Injection** | 100% sử dụng `SqlParameter`, không nối chuỗi SQL |
| **Cookie bảo mật** | `HttpOnly=true` ngăn JavaScript truy cập, giảm nguy cơ đánh cắp phiên |

---

# CHƯƠNG 6. KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

## 6.1. Kết luận

Qua quá trình thực hiện đồ án, hệ thống **"Website hỗ trợ tra cứu thông tin các trường đại học và cao đẳng Việt Nam"** đã được xây dựng hoàn chỉnh với các tính năng chính:

**Đã hoàn thành:**
- ✅ Hệ thống quản trị đầy đủ (quản lý trường, tin tuyển sinh, bài viết, tài khoản, tư vấn, đánh giá, logs)
- ✅ Trang tra cứu thông tin trường với filter đa tiêu chí
- ✅ Tra cứu điểm chuẩn tổng hợp
- ✅ Biểu đồ xu hướng điểm chuẩn Chart.js
- ✅ Hệ thống tài khoản hoàn chỉnh (đăng ký, xác nhận email, đăng nhập, quên mật khẩu)
- ✅ Đánh giá trường + yêu cầu tư vấn
- ✅ Danh sách yêu thích (WishList)
- ✅ Hồ sơ cá nhân + gợi ý trường phù hợp
- ✅ Bài viết tin tức
- ✅ So sánh trường
- ✅ Bảo mật đa lớp (hash, brute-force, XSS, SQL Injection)
- ✅ Kiến trúc 3 lớp rõ ràng, dễ bảo trì

**Công nghệ sử dụng:**
- ASP.NET Web Forms (.NET Framework), C#
- SQL Server 2022 với Stored Procedure, Covering Index, Filtered Index
- Bootstrap 5, Chart.js
- FormsAuthentication, SHA-256

## 6.2. Hướng phát triển

- Tích hợp AI gợi ý ngành học dựa trên sở thích và năng lực
- Thêm chức năng chat trực tuyến với tư vấn viên
- Xây dựng API RESTful để phát triển ứng dụng mobile
- Tích hợp thanh toán trực tuyến cho tính năng premium
- Nâng cấp lên ASP.NET Core để hỗ trợ đa nền tảng
- Thêm chức năng thống kê và báo cáo nâng cao (trường được tìm kiếm nhiều nhất, ngành hot)
- Tối ưu SEO với Open Graph, JSON-LD structured data
- Triển khai lên Azure Cloud với CI/CD pipeline

---

*Báo cáo được hoàn thành tháng 03/2025*
*Sinh viên: Lê Thị Hồng Hạnh — Lớp CHTM118*
