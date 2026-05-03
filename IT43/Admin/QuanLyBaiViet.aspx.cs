using System;
using System.Collections.Generic;
using System.Data;
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
        if (!IsPostBack)
        {
            LoadTruongFilter();
            LoadData();
        }
    }

    /// <summary>Load danh sách trường vào dropdown filter và modal.</summary>
    private void LoadTruongFilter()
    {
        var dt = DBHelper.Query("SELECT MaTruong, TenTruong FROM tbl_Truong ORDER BY TenTruong");
        foreach (DataRow r in dt.Rows)
        {
            ddlTruong.Items.Add(new ListItem(r["TenTruong"].ToString(), r["MaTruong"].ToString()));
            ddlMTruong.Items.Add(new ListItem(r["TenTruong"].ToString(), r["MaTruong"].ToString()));
        }
    }

    /// <summary>Truy vấn bài viết theo bộ lọc và bind vào GridView + phân trang.</summary>
    private void LoadData()
    {
        int? maTruong = null;
        if (int.TryParse(ddlTruong.SelectedValue, out int mt) && mt > 0) maTruong = mt;

        bool? trangThai = null;
        if (ddlTrangThai.SelectedValue == "1")      trangThai = true;
        else if (ddlTrangThai.SelectedValue == "0") trangThai = false;

        var paged = BaiVietDAL.GetDanhSach(maTruong, CurrentPage, PAGE_SIZE,
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
    /// </summary>
    protected void gvBaiViet_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!int.TryParse(e.CommandArgument?.ToString(), out int id)) return;

        if (e.CommandName == "ToggleBV")
        {
            BaiVietDAL.ToggleTrangThai(id);
            litThongBao.Text = "<div class='alert alert-success alert-dismissible fade show py-2'>Đã thay đổi trạng thái bài viết.</div>";
            LoadData();
            return;
        }

        if (e.CommandName == "XoaBV")
        {
            // Xóa file ảnh khi xóa bài viết
            var bv = BaiVietDAL.GetChiTiet(
                DBHelper.Val<string>(DBHelper.Scalar(
                    "SELECT Slug FROM tbl_BaiViet WHERE MaBaiViet=@id",
                    new[] { new System.Data.SqlClient.SqlParameter("@id", id) })));
            if (bv != null) ImageUploadHelper.DeleteOld(bv.AnhChinh);

            BaiVietDAL.Xoa(id);
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
            txtSlug.Text         = r["Slug"]     == DBNull.Value ? "" : r["Slug"].ToString();
            txtNoiDung.Text      = r["NoiDung"]  == DBNull.Value ? "" : r["NoiDung"].ToString();
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
    /// gọi BaiVietDAL.Them hoặc BaiVietDAL.CapNhat.
    /// </summary>
    protected void btnLuuBV_Click(object sender, EventArgs e)
    {
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
            Slug      = slug,
            AnhChinh  = string.IsNullOrEmpty(anhChinh) ? null : anhChinh,
            NoiDung   = txtNoiDung.Text.Trim(),
            MaTacGia  = maTK,
            TrangThai = chkTrangThai.Checked,
            MaTruong  = maTruong
        };

        if (maBV == 0)
        {
            BaiVietDAL.Them(model);
            litThongBao.Text = "<div class='alert alert-success'>Đã thêm bài viết mới.</div>";
        }
        else
        {
            BaiVietDAL.CapNhat(model);
            litThongBao.Text = "<div class='alert alert-success'>Đã cập nhật bài viết.</div>";
        }

        // Reset form
        hfMaBaiViet.Value   = "0";
        hfAnhChinh.Value    = "";
        txtTieuDe.Text      = txtSlug.Text = txtNoiDung.Text = "";
        chkTrangThai.Checked = true;
        ddlMTruong.SelectedIndex = 0;

        LoadData();
    }
}
