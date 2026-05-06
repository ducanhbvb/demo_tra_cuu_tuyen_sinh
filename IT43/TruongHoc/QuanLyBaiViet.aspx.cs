using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý bài viết cho tài khoản TruongHoc.
/// Chỉ hiển thị và thao tác bài viết của trường mình.
/// Hỗ trợ upload ảnh bìa, xem trước bài viết, lọc theo trạng thái.
/// </summary>
public partial class TruongHoc_QuanLyBaiViet : Page
{
    private const int PAGE_SIZE = 15;
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }
    private int MaTruong => SecurityHelper.GetCurrentMaTruong();
    private int MaTK     => SecurityHelper.GetCurrentMaTaiKhoan();

    /// <summary>Flag điều khiển hiển thị modal Bootstrap từ server-side.</summary>
    public string ShowModal = "false";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindStats();
            BindData();
        }
        else
        {
            // Khi EnableViewState="false", GridView không có rows trong postback.
            // Phải BindData() sớm để LinkButton bên trong tồn tại trong control tree
            // → RowCommand mới fire được (Xoa/Toggle/Sua).
            BindData();
        }
    }

    protected void btnLoc_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    protected void gvBaiViet_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int maBaiViet = int.Parse(e.CommandArgument.ToString());

        switch (e.CommandName)
        {
            case "Sua":
            {
                var bv = BaiVietDAL.GetById(maBaiViet);
                if (bv == null || bv.MaTruong != MaTruong) break; // bảo vệ IDOR

                hfMaBaiViet.Value    = maBaiViet.ToString();
                txtTieuDe.Text       = bv.TieuDe;
                txtTheLoai.Text      = bv.TheLoai ?? "";
                hfNoiDung.Value      = bv.NoiDung ?? "";
                txtSlug.Text         = bv.Slug;
                chkTrangThai.Checked = bv.TrangThai;

                // Load ảnh cũ
                hfAnhChinh.Value = bv.AnhChinh ?? "";
                if (!string.IsNullOrEmpty(bv.AnhChinh))
                {
                    imgPreviewBV.ImageUrl = bv.AnhChinh;
                    imgPreviewBV.Visible  = true;
                }

                ShowModal = "true";
                BindStats();
                BindData();
                break;
            }
            case "Toggle":
            {
                var bv = BaiVietDAL.GetById(maBaiViet);
                if (bv == null || bv.MaTruong != MaTruong) break;
                BaiVietBLL.ToggleTrangThai(maBaiViet);

                LogHelper.Ghi(MaTK, "TOGGLE_BAI_VIET",
                    $"Thay đổi trạng thái bài viết (MaBaiViet={maBaiViet})",
                    bangTacDong: "tbl_BaiViet");

                ShowMsg("success", "Đã cập nhật trạng thái bài viết.");
                BindStats();
                BindData();
                break;
            }
            case "Xoa":
            {
                var bv = BaiVietDAL.GetById(maBaiViet);
                if (bv == null || bv.MaTruong != MaTruong) break;
                string tieuDe = bv.TieuDe ?? $"ID={maBaiViet}";
                BaiVietBLL.Xoa(maBaiViet); // BLL tự xóa ảnh trước khi xóa DB

                LogHelper.Ghi(MaTK, "XOA_BAI_VIET",
                    $"Xóa bài viết: {tieuDe} (MaBaiViet={maBaiViet})",
                    bangTacDong: "tbl_BaiViet");

                ShowMsg("success", "Đã xóa bài viết.");
                BindStats();
                BindData();
                break;
            }
        }
    }

    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    protected void btnLuu_Click(object sender, EventArgs e)
    {
        int maBV = int.Parse(hfMaBaiViet.Value);

        // Xử lý slug
        string tieuDeVal = txtTieuDe.Text.Trim();
        string slugVal   = string.IsNullOrWhiteSpace(txtSlug.Text)
            ? SlugHelper.ToSlug(tieuDeVal)
            : txtSlug.Text.Trim();

        // Validate cơ bản
        if (string.IsNullOrWhiteSpace(tieuDeVal))
        {
            litModalLoi.Text = Alert("danger", "Tiêu đề không được để trống.");
            ShowModal = "true";
            BindStats();
            BindData();
            return;
        }

        // Xử lý ảnh bìa: giữ ảnh cũ, upload mới nếu có
        string anhChinh = hfAnhChinh.Value;
        if (fuAnhChinh.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuAnhChinh.PostedFile, "baiviet", slugVal + "_thumb", ImageUploadHelper.MAX_COVER_SIZE);
            if (ok)
            {
                ImageUploadHelper.DeleteOld(anhChinh);
                anhChinh = result;
            }
            else
            {
                litModalLoi.Text = Alert("danger", "Ảnh bìa: " + Server.HtmlEncode(result));
                ShowModal = "true";
                BindStats();
                BindData();
                return;
            }
        }

        var m = new BaiVietModel
        {
            MaBaiViet  = maBV,
            TieuDe     = tieuDeVal,
            TheLoai    = string.IsNullOrWhiteSpace(txtTheLoai.Text) ? null : txtTheLoai.Text.Trim(),
            NoiDung    = (hfNoiDung.Value ?? "").Trim(),
            Slug       = slugVal,
            AnhChinh   = string.IsNullOrEmpty(anhChinh) ? null : anhChinh,
            TrangThai  = chkTrangThai.Checked,
            MaTruong   = MaTruong,
            MaTacGia   = MaTK,
        };

        bool saveOk; string saveErr;
        if (maBV == 0)
        {
            m.NgayDang = DateTime.Now;
            var r = BaiVietBLL.Them(m);
            saveOk = r.Item1; saveErr = r.Item2;
        }
        else
        {
            // Kiểm tra IDOR — chỉ cho sửa bài của trường mình
            var existing = BaiVietDAL.GetById(maBV);
            if (existing == null || existing.MaTruong != MaTruong)
            {
                litModalLoi.Text = Alert("danger", "Không có quyền chỉnh sửa bài viết này.");
                ShowModal = "true";
                BindStats();
                BindData();
                return;
            }
            var r = BaiVietBLL.CapNhat(m);
            saveOk = r.Item1; saveErr = r.Item2;
        }

        if (saveOk)
        {
            LogHelper.Ghi(MaTK, maBV == 0 ? "THEM_BAI_VIET" : "CAP_NHAT_BAI_VIET",
                $"{(maBV == 0 ? "Thêm" : "Cập nhật")} bài viết: {m.TieuDe} (MaBaiViet={maBV})",
                bangTacDong: "tbl_BaiViet");

            hfMaBaiViet.Value = "0";
            hfAnhChinh.Value  = "";
            ShowMsg("success", maBV == 0 ? "Đã thêm bài viết thành công." : "Đã cập nhật bài viết.");
            BindStats();
            BindData();
        }
        else
        {
            litModalLoi.Text = Alert("danger", Server.HtmlEncode(saveErr));
            ShowModal = "true";
            BindStats();
            BindData();
        }
    }

    // ── Helper: Stat cards ──────────────────────────────────────────────────
    private void BindStats()
    {
        // Tổng bài viết của trường
        var all    = BaiVietBLL.GetDanhSach(MaTruong, 0, 1, chiActive: false);
        var active = BaiVietBLL.GetDanhSach(MaTruong, 0, 1, chiActive: false, trangThai: true);

        litTongBV.Text  = all.TongSo.ToString("N0");
        litHienThi.Text = active.TongSo.ToString("N0");

        // Tổng lượt xem từ DB trực tiếp (per-truong, không cần load toàn bộ rows)
        var v = DBHelper.Scalar(
            "SELECT ISNULL(SUM(LuotXem),0) FROM tbl_BaiViet WHERE MaTruong=@mt",
            new[] { new System.Data.SqlClient.SqlParameter("@mt", MaTruong) });
        litLuotXem.Text = (v == null ? 0L : System.Convert.ToInt64(v)).ToString("N0");
    }

    // ── Helper: BindData với filter ─────────────────────────────────────────
    private void BindData()
    {
        bool? trangThaiFilter = null;
        int val;
        if (int.TryParse(ddlTrangThai.SelectedValue, out val) && !string.IsNullOrEmpty(ddlTrangThai.SelectedValue))
            trangThaiFilter = (val == 1);

        var result = BaiVietBLL.GetDanhSach(
            MaTruong, CurrentPage, PAGE_SIZE,
            chiActive: false,
            trangThai: trangThaiFilter);

        litTong.Text         = result.TongSo.ToString("N0");
        gvBaiViet.DataSource = result.Data;
        gvBaiViet.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    private void ShowMsg(string type, string msg) => litThongBao.Text = Alert(type, msg);
    private static string Alert(string type, string msg)
        => $"<div class='alert alert-{type} alert-dismissible fade show'>{msg}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
}
