using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang So sánh trường — cho phép chọn tối đa 3 trường để so sánh song song
/// các tiêu chí: loại trường, vùng miền, kiểm định, điểm chuẩn, số ngành…
/// </summary>
public partial class SoSanhTruong_Page : Page
{
    private const int MAX_TRUONG = 3;

    /// <summary>Danh sách MaTruong đang so sánh, lưu trong Session["SoSanh"].</summary>
    private List<int> DanhSachSoSanh
    {
        get
        {
            if (Session["SoSanh"] is List<int> list) return list;
            var newList = new List<int>();
            Session["SoSanh"] = newList;
            return newList;
        }
    }

    /// <summary>Khởi tạo trang: load dropdown trường và hiển thị bảng so sánh.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropDown();
            LoadBang();
        }
    }

    /// <summary>Load danh sách trường vào DropDownList để người dùng chọn thêm.</summary>
    private void LoadDropDown()
    {
        ddlChonTruong.Items.Clear();
        ddlChonTruong.Items.Add(new ListItem("-- Chọn trường để thêm --", "0"));

        var dt = DBHelper.Query(
            "SELECT MaTruong, TenTruong FROM tbl_Truong ORDER BY TenTruong");
        foreach (DataRow r in dt.Rows)
            ddlChonTruong.Items.Add(new ListItem(
                r["TenTruong"].ToString(),
                r["MaTruong"].ToString()));
    }

    /// <summary>
    /// Lấy dữ liệu chi tiết từ SoSanhDAL và bind vào các Repeater trong bảng so sánh.
    /// Nếu danh sách trống → hiển thị panel rỗng.
    /// </summary>
    private void LoadBang()
    {
        var ds = DanhSachSoSanh;
        litSoTruong.Text = ds.Count.ToString();
        litThongBao.Text = "";

        if (ds.Count == 0)
        {
            pnlBang.Visible  = false;
            pnlEmpty.Visible = true;
            return;
        }

        pnlBang.Visible  = true;
        pnlEmpty.Visible = false;

        var data = SoSanhDAL.GetDanhSachSoSanh(ds);

        rptHeader.DataSource   = data;
        rptHeader.DataBind();
        rptLoai.DataSource     = data; rptLoai.DataBind();
        rptVung.DataSource     = data; rptVung.DataBind();
        rptKiemDinh.DataSource = data; rptKiemDinh.DataBind();
        rptDanhGia.DataSource  = data; rptDanhGia.DataBind();
        rptWebsite.DataSource  = data; rptWebsite.DataBind();
        rptQuyMo.DataSource    = data; rptQuyMo.DataBind();
        rptDiemChuan.DataSource= data; rptDiemChuan.DataBind();
        rptSoNganh.DataSource  = data; rptSoNganh.DataBind();
        rptChiTiet.DataSource  = data; rptChiTiet.DataBind();
    }

    /// <summary>
    /// Xử lý nút "Thêm trường" — validate chọn trường, kiểm tra trùng và giới hạn tối đa.
    /// </summary>
    protected void btnThem_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(ddlChonTruong.SelectedValue, out int maTruong) || maTruong == 0)
        {
            litThongBao.Text = "<div class='alert alert-warning mt-2 py-1 small'>Vui lòng chọn trường.</div>";
            return;
        }

        var ds = DanhSachSoSanh;
        if (ds.Contains(maTruong))
        {
            litThongBao.Text = "<div class='alert alert-info mt-2 py-1 small'>Trường này đã có trong danh sách so sánh.</div>";
        }
        else if (ds.Count >= MAX_TRUONG)
        {
            litThongBao.Text = $"<div class='alert alert-warning mt-2 py-1 small'>Chỉ có thể so sánh tối đa {MAX_TRUONG} trường.</div>";
        }
        else
        {
            ds.Add(maTruong);
            Session["SoSanh"] = ds;
        }

        LoadBang();
    }

    /// <summary>Xử lý lệnh "Xóa" trường khỏi bảng so sánh (từ Repeater header).</summary>
    protected void rptHeader_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "Xoa" && int.TryParse(e.CommandArgument?.ToString(), out int maTruong))
        {
            var ds = DanhSachSoSanh;
            ds.Remove(maTruong);
            Session["SoSanh"] = ds;
            LoadBang();
        }
    }

    /// <summary>Xóa toàn bộ danh sách so sánh khỏi Session.</summary>
    protected void btnXoaTatCa_Click(object sender, EventArgs e)
    {
        Session.Remove("SoSanh");
        LoadBang();
    }
}
