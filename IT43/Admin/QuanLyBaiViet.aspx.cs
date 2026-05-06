using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý bài viết (Admin) — CRUD bài viết với modal form,
/// upload ảnh bìa, bộ lọc (trường, trạng thái), phân trang.
/// </summary>
public partial class Admin_QuanLyBaiViet : Page
{
    private const int PAGE_SIZE = 15;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Flag điều khiển hiển thị modal Bootstrap từ server-side.</summary>
    public string ShowModal = "false";

    /// <summary>Khởi tạo trang: load dropdown trường và bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Luôn thêm data-field attributes cho controls (cho JavaScript)
        AddDataFieldAttributes();
        gvBaiViet.PreRender += gvBaiViet_PreRender;

        if (!IsPostBack)
        {
            LoadTruongFilter();
            BindStats();
            LoadData();
        }
        else
        {
            // EnableViewState="false" → phải rebind để LinkButton trong GridView tồn tại
            // trước khi ASP.NET xử lý RowCommand (Xoa/Sua/Toggle)
            LoadData();
        }
    }

    /// <summary>Load danh sách trường vào dropdown filter và modal.</summary>
    private void LoadTruongFilter()
    {
        var dt = BaiVietBLL.GetDanhSachTruongFilter();
        foreach (System.Data.DataRow r in dt.Rows)
        {
            ddlTruong.Items.Add(new ListItem(r["TenTruong"].ToString(), r["MaTruong"].ToString()));
            ddlMTruong.Items.Add(new ListItem(r["TenTruong"].ToString(), r["MaTruong"].ToString()));
        }
    }

    private void BindStats()
    {
        var stats = ThongKeBLL.ThongKeBaiViet();
        litTongBV.Text = stats.tong.ToString("N0");
        litHienThi.Text = stats.hienThi.ToString("N0");
        litLuotXem.Text = stats.luotXem.ToString("N0");
    }

    /// <summary>Truy vấn bài viết theo bộ lọc và bind vào GridView + phân trang.</summary>
    private void LoadData()
    {
        int? maTruong = null;
        if (int.TryParse(ddlTruong.SelectedValue, out int mt) && mt > 0) maTruong = mt;

        bool? trangThai = null;
        if (ddlTrangThai.SelectedValue == "1")      trangThai = true;
        else if (ddlTrangThai.SelectedValue == "0") trangThai = false;

        var paged = BaiVietBLL.GetDanhSach(maTruong, CurrentPage, PAGE_SIZE,
                                            chiActive: false, trangThai: trangThai);
        gvBaiViet.DataSource = paged.Data;
        gvBaiViet.DataBind();
        litTong.Text = paged.TongSo.ToString("N0");
        BindPaging(paged.TongSo);
    }

    /// <summary>Tạo danh sách nút phân trang.</summary>
    private void BindPaging(int tongSo)
    {
        int totalPages = (int)Math.Ceiling((double)tongSo / PAGE_SIZE);
        var pages = new List<object>();
        for (int i = 0; i < totalPages; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; LoadData(); }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            LoadData();
        }
    }

    /// <summary>
    /// Xử lý command trên GridView: ToggleBV (ẩn/hiện), XoaBV (xóa + xóa ảnh),
    /// SuaBV (load dữ liệu vào modal form).
    /// TuVanVien chỉ được xem — không thể ToggleBV, XoaBV, SuaBV.
    /// </summary>
    protected void gvBaiViet_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!int.TryParse(e.CommandArgument?.ToString(), out int id)) return;

        // TuVanVien chỉ xem, chặn mọi write op
        if (!SecurityHelper.CanManageContent())
        {
            litThongBao.Text = "<div class='alert alert-danger py-2'><i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền thực hiện thao tác này.</div>";
            return;
        }

        if (e.CommandName == "ToggleBV")
        {
            BaiVietBLL.ToggleTrangThai(id);

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, "TOGGLE_BAI_VIET",
                $"Thay đổi trạng thái bài viết (MaBaiViet={id})",
                bangTacDong: "tbl_BaiViet");

            litThongBao.Text = "<div class='alert alert-success alert-dismissible fade show py-2'>Đã thay đổi trạng thái bài viết.</div>";
            LoadData();
            return;
        }

        if (e.CommandName == "XoaBV")
        {
            // Lấy tiêu đề bài viết trước khi xóa để ghi log
            var dtInfo = DBHelper.Query("SELECT TieuDe FROM tbl_BaiViet WHERE MaBaiViet=@id",
                new[] { new System.Data.SqlClient.SqlParameter("@id", id) });
            string tieuDe = dtInfo.Rows.Count > 0 ? dtInfo.Rows[0]["TieuDe"].ToString() : $"ID={id}";

            BaiVietBLL.Xoa(id);   // BLL tự xóa ảnh trước khi xóa DB

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, "XOA_BAI_VIET",
                $"Xóa bài viết: {tieuDe} (MaBaiViet={id})",
                bangTacDong: "tbl_BaiViet");

            litThongBao.Text = "<div class='alert alert-success alert-dismissible fade show'>Đã xóa bài viết.</div>";
            LoadData();
            return;
        }

        if (e.CommandName == "SuaBV")
        {
            var dt = DBHelper.Query("SELECT * FROM tbl_BaiViet WHERE MaBaiViet=@id",
                new[] { new System.Data.SqlClient.SqlParameter("@id", id) });
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];
            hfMaBaiViet.Value    = id.ToString();
            txtTieuDe.Text       = r["TieuDe"].ToString();
            txtSlug.Text         = r["Slug"]    == DBNull.Value ? "" : r["Slug"].ToString();
            txtTheLoai.Text      = r["TheLoai"] == DBNull.Value ? "" : r["TheLoai"].ToString();
            hfNoiDung.Value      = r["NoiDung"] == DBNull.Value ? "" : r["NoiDung"].ToString();
            txtTenNguoiDang.Text = r.Table.Columns.Contains("TenNguoiDang") && r["TenNguoiDang"] != DBNull.Value ? r["TenNguoiDang"].ToString() : "";
            chkTrangThai.Checked = DBHelper.Val<bool>(r["TrangThai"]);

            // Lưu ảnh cũ vào hidden + hiển thị preview
            string anhCu = r["AnhChinh"] == DBNull.Value ? "" : r["AnhChinh"].ToString();
            hfAnhChinh.Value = anhCu;
            if (!string.IsNullOrEmpty(anhCu))
            {
                imgPreviewBV.ImageUrl = anhCu;
                imgPreviewBV.Visible  = true;
            }

            if (r["MaTruong"] != DBNull.Value)
                ddlMTruong.SelectedValue = r["MaTruong"].ToString();

            ShowModal = "true";
            LoadData();
        }
    }

    /// <summary>
    /// Xử lý nút Lưu trong modal — validate tiêu đề, upload ảnh bìa,
    /// gọi BaiVietBLL.Them hoặc BaiVietBLL.CapNhat.
    /// TuVanVien không được phép lưu.
    /// </summary>
    protected void btnLuuBV_Click(object sender, EventArgs e)
    {
        if (!SecurityHelper.CanManageContent())
        {
            litThongBao.Text = "<div class='alert alert-danger py-2'><i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền thực hiện thao tác này.</div>";
            return;
        }
        if (string.IsNullOrWhiteSpace(txtTieuDe.Text))
        {
            litThongBao.Text = "<div class='alert alert-warning'>Vui lòng nhập tiêu đề.</div>";
            ShowModal = "true";
            LoadData();
            return;
        }

        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        int.TryParse(hfMaBaiViet.Value, out int maBV);

        int? maTruong = null;
        if (int.TryParse(ddlMTruong.SelectedValue, out int mt) && mt > 0)
            maTruong = mt;

        string slug = string.IsNullOrWhiteSpace(txtSlug.Text)
            ? SlugHelper.ToSlug(txtTieuDe.Text)
            : txtSlug.Text.Trim();

        // Xử lý ảnh: giữ ảnh cũ, upload mới nếu có
        string anhChinh = hfAnhChinh.Value;
        if (fuAnhChinh.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuAnhChinh.PostedFile, "baiviet", slug + "_thumb", ImageUploadHelper.MAX_COVER_SIZE);
            if (ok)
            {
                ImageUploadHelper.DeleteOld(anhChinh);
                anhChinh = result;
            }
            else
            {
                litThongBao.Text = $"<div class='alert alert-danger'>Ảnh bìa: {result}</div>";
                ShowModal = "true";
                LoadData();
                return;
            }
        }

        var model = new BaiVietModel
        {
            MaBaiViet = maBV,
            TieuDe    = txtTieuDe.Text.Trim(),
            TheLoai   = string.IsNullOrWhiteSpace(txtTheLoai.Text) ? null : txtTheLoai.Text.Trim(),
            Slug      = slug,
            AnhChinh  = string.IsNullOrEmpty(anhChinh) ? null : anhChinh,
            NoiDung   = (hfNoiDung.Value ?? "").Trim(),
            MaTacGia  = maTK,
            TrangThai = chkTrangThai.Checked,
            MaTruong  = maTruong,
            TenNguoiDang = string.IsNullOrWhiteSpace(txtTenNguoiDang.Text) ? null : txtTenNguoiDang.Text.Trim()
        };

        if (maBV == 0)
        {
            var (ok, err) = BaiVietBLL.Them(model);
            if (ok)
            {
                LogHelper.Ghi(maTK, "THEM_BAI_VIET",
                    $"Thêm bài viết: {model.TieuDe}",
                    bangTacDong: "tbl_BaiViet");
            }
            litThongBao.Text = ok
                ? "<div class='alert alert-success'>Đã thêm bài viết mới.</div>"
                : $"<div class='alert alert-danger'>{err}</div>";
        }
        else
        {
            var (ok, err) = BaiVietBLL.CapNhat(model);
            if (ok)
            {
                LogHelper.Ghi(maTK, "CAP_NHAT_BAI_VIET",
                    $"Cập nhật bài viết: {model.TieuDe} (MaBaiViet={maBV})",
                    bangTacDong: "tbl_BaiViet");
            }
            litThongBao.Text = ok
                ? "<div class='alert alert-success'>Đã cập nhật bài viết.</div>"
                : $"<div class='alert alert-danger'>{err}</div>";
        }

        // Reset form
        hfMaBaiViet.Value    = "0";
        hfAnhChinh.Value     = "";
        hfNoiDung.Value      = "";
        txtTieuDe.Text       = txtSlug.Text = "";
        txtTenNguoiDang.Text = "";
        chkTrangThai.Checked = true;
        ddlMTruong.SelectedIndex = 0;

        LoadData();
    }

    /// <summary>Thêm data-field attributes cho các controls trong modal (cho JavaScript).</summary>
    private void AddDataFieldAttributes()
    {
        hfMaBaiViet.Attributes["data-field"] = "id";
        hfNoiDung.Attributes["data-field"] = "noiDung";
        txtTieuDe.Attributes["data-field"] = "tieude";
        txtSlug.Attributes["data-field"] = "slug";
        txtTheLoai.Attributes["data-field"] = "theloai";
        txtTenNguoiDang.Attributes["data-field"] = "tennguoidang";
        ddlMTruong.Attributes["data-field"] = "truong";
        hfAnhChinh.Attributes["data-field"] = "anhchinh";
        chkTrangThai.Attributes["data-field"] = "trangthai";
    }

    protected void gvBaiViet_PreRender(object sender, EventArgs e)
    {
        if (gvBaiViet.HeaderRow != null)
            gvBaiViet.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    protected bool SafeGetBool(object value)
    {
        return value != DBNull.Value && value != null && (bool)value;
    }

    protected string GetTrangThaiBadge(object value)
    {
        if (value == DBNull.Value || value == null) return "";
        bool trangThai = (bool)value;
        return trangThai
            ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hiện</span>"
            : "<span class='badge badge-soft-secondary'><i class='bi bi-eye-slash me-1'></i>Ẩn</span>";
    }
}
