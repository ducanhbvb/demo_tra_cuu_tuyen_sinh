using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Gợi ý trường phù hợp — cho phép người dùng (kể cả khách) nhập tiêu chí
/// để tìm trường đại học phù hợp dựa trên điểm dự kiến, ngành, khu vực, v.v.
/// </summary>
public partial class Client_GoiYTruong : Page
{
    /// <summary>Điểm dự kiến để pre-fill slider từ Profile (nếu đã đăng nhập).</summary>
    public decimal PreFillDiem { get; private set; } = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropdowns();
            TryPreFillFromProfile();
        }
        else
        {
            // On postback, restore khu vực selection if hidden field has value
            if (!string.IsNullOrEmpty(hfKhuVuc.Value))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "restoreKhuVucPostback",
                    $"document.addEventListener('DOMContentLoaded', function() {{ " +
                    $"var radio = document.querySelector('input[name=\"khuVuc\"][value=\"{hfKhuVuc.Value}\"]'); " +
                    $"if (radio) radio.checked = true; updateAdjustedScoreDisplay(); }});", true);
            }
        }
        // Kiểm soát hiển thị nút Load Profile dựa trên trạng thái đăng nhập
        btnLoadProfile.Visible = User.Identity.IsAuthenticated;
    }

    // ── Load dropdowns ───────────────────────────────────────────────────────

    private void LoadDropdowns()
    {
        // Ngành học
        foreach (var n in DanhMucBLL.GetChuyenNganh())
            ddlNganh.Items.Add(new System.Web.UI.WebControls.ListItem(n.Ten, n.Id.ToString()));

        // Tỉnh / thành phố
        foreach (var t in DanhMucDAL.GetTinhThanh())
            ddlTinh.Items.Add(new System.Web.UI.WebControls.ListItem(t, t));
    }

    // ── Pre-fill từ Profile học sinh (nếu đã đăng nhập) ────────────────────

    private void TryPreFillFromProfile()
    {
        if (!User.Identity.IsAuthenticated) return;
        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK <= 0) return;

        var p = ProfileBLL.GetProfile(maTK);
        if (p == null) return;

        LoadProfileData(p);
    }

    private void LoadProfileData(ProfileHocSinhModel p)
    {
        if (p.DiemDuKien.HasValue)
        {
            PreFillDiem  = p.DiemDuKien.Value;
            txtDiem.Text = p.DiemDuKien.Value.ToString("F2");
        }
        if (p.MaChuyenNganh.HasValue)
        {
            var item = ddlNganh.Items.FindByValue(p.MaChuyenNganh.Value.ToString());
            if (item != null) item.Selected = true;
        }
        if (p.KhuVuc.HasValue)
        {
            hfKhuVuc.Value = p.KhuVuc.Value.ToString();
            // Register startup script to restore khu vực selection client-side
            ScriptManager.RegisterStartupScript(this, GetType(), "restoreKhuVuc",
                $"document.addEventListener('DOMContentLoaded', function() {{ " +
                $"var radio = document.querySelector('input[name=\"khuVuc\"][value=\"{p.KhuVuc.Value}\"]'); " +
                $"if (radio) radio.checked = true; updateAdjustedScoreDisplay(); }});", true);
        }
    }

    // ── Nút Load từ Profile ────────────────────────────────────────────────

    protected void btnLoadProfile_Click(object sender, EventArgs e)
    {
        // Nút này chỉ hiển thị khi đã đăng nhập (được kiểm soát ở ASPX)
        // nhưng vẫn kiểm tra lại để đảm bảo an toàn
        if (!User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/Client/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
            return;
        }

        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK <= 0)
        {
            return;
        }

        var p = ProfileBLL.GetProfile(maTK);
        if (p == null)
        {
            // Chưa có profile, chuyển đến trang cập nhật profile
            Response.Redirect("~/Client/MyProfile.aspx");
            return;
        }

        LoadProfileData(p);
    }

    // ── Nút tìm kiếm ───────────────────────────────────────────────────────

    protected void btnGoiY_Click(object sender, EventArgs e)
    {
        pnlDefault.Visible  = false;
        pnlEmpty.Visible    = false;
        pnlKetQua.Visible   = false;

        // Parse điểm gốc (không có bonus khu vực) - dùng InvariantCulture vì JS lưu dạng "26.75"
        if (!decimal.TryParse(hfDiemGoc.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal diemGoc) || diemGoc <= 0)
        {
            pnlDefault.Visible = true;
            return;
        }

        // Khu vực (từ hidden field)
        byte? khuVuc = byte.TryParse(hfKhuVuc.Value, out byte kv) && kv > 0 ? kv : (byte?)null;

        // Ngành
        int? nganh = int.TryParse(ddlNganh.SelectedValue, out int n) && n > 0 ? n : (int?)null;

        // Loại trường
        byte? loai = byte.TryParse(ddlLoaiTruong.SelectedValue, out byte l) && l > 0 ? l : (byte?)null;

        // Tỉnh/thành
        string tinh = string.IsNullOrEmpty(ddlTinh.SelectedValue) ? null : ddlTinh.SelectedValue;

        // Kiểm định
        bool? kiemDinh = chkKiemDinh.Checked ? (bool?)true : null;

        // Cấp bậc đào tạo
        byte? capBac = byte.TryParse(ddlCapBacDaoTao.SelectedValue, out byte cb) && cb > 0 ? cb : (byte?)null;

        // Tổ hợp xét tuyển
        string toHop = string.IsNullOrEmpty(txtToHopXetTuyen.Text) ? null : txtToHopXetTuyen.Text.Trim().ToUpper();

        // Số kết quả
        int top = int.TryParse(ddlTop.SelectedValue, out int t) ? t : 12;

        // Gọi BLL với đầy đủ 9 tham số
        DataTable dt = GoiYTruongBLL.GoiYNangCao(
            diemGoc,
            nganh,
            khuVuc,
            loai,
            tinh,
            kiemDinh,
            capBac,
            toHop,
            top);

        if (dt.Rows.Count == 0)
        {
            pnlEmpty.Visible = true;
        }
        else
        {
            pnlKetQua.Visible    = true;
            rptKetQua.DataSource = dt;
            rptKetQua.DataBind();
            litSoKetQua.Text    = dt.Rows.Count.ToString();

            // Hiển thị tiêu đề kết quả
            litKetQuaTitle.Text = "Kết quả gợi ý trường phù hợp";
        }
    }

    protected string GetCapBacBadge(object value)
    {
        string v = Convert.ToString(value);
        string text = v == "1" ? "Đại học" : v == "2" ? "Cao Đẳng" : v == "3" ? "Trường nghề" : "";
        if (string.IsNullOrEmpty(text)) return "";
        string css = v == "1" ? "school-level-university" : v == "2" ? "school-level-college" : "school-level-vocational";
        return $"<span class='school-level-badge {css}'>{text}</span>";
    }

    /// <summary>Trả về TiLePhuHop an toàn (không bao giờ NULL, giới hạn 0-100).</summary>
    protected int GetTiLePhuHop(object value)
    {
        if (value == null || value == DBNull.Value) return 0;
        if (!int.TryParse(Convert.ToString(value), out int pct)) return 0;
        return Math.Min(100, Math.Max(0, pct));
    }
}
