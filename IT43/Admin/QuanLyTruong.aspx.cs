using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý trường (Admin) — danh sách trường với bộ lọc, phân trang,
/// toggle trạng thái hiển thị và xóa trường.
/// </summary>
public partial class Admin_QuanLyTruong : Page
{
    private const int PAGE_SIZE = 15;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: load dropdown tỉnh/thành và bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            foreach (var t in DanhMucDAL.GetTinhThanh())
                ddlTinh.Items.Add(new ListItem(t, t));
            BindData();
        }
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>
    /// Xử lý command trên GridView: ToggleTruong (ẩn/hiện) và XoaTruong (xóa vĩnh viễn).
    /// </summary>
    protected void gvTruong_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!int.TryParse(e.CommandArgument?.ToString(), out int id)) return;

        if (e.CommandName == "ToggleTruong")
        {
            TruongDAL.ToggleTrangThai(id);
            litThongBao.Text = "<div class='alert alert-success alert-dismissible fade show py-2'>Đã thay đổi trạng thái hiển thị trường.</div>";
            BindData();
            return;
        }

        if (e.CommandName == "XoaTruong")
        {
            TruongBLL.Xoa(id);
            litThongBao.Text = "<div class='alert alert-success alert-dismissible'>Đã xóa trường thành công.</div>";
            BindData();
        }
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    /// <summary>
    /// Gọi TruongBLL.TimKiem với bộ lọc (tên, tỉnh, trạng thái),
    /// bind vào GridView và tạo nút phân trang.
    /// </summary>
    private void BindData()
    {
        bool? trangThai = null;
        if (ddlTrangThai.SelectedValue == "1")      trangThai = true;
        else if (ddlTrangThai.SelectedValue == "0") trangThai = false;

        var result = TruongBLL.TimKiem(txtFilter.Text.Trim(), ddlTinh.SelectedValue,
            null, null, null, CurrentPage, PAGE_SIZE, trangThai);

        litTong.Text = result.TongSo.ToString("N0");
        gvTruong.DataSource = result.Data;
        gvTruong.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
