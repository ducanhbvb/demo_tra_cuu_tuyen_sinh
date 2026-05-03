using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý tài khoản (Admin) — danh sách tài khoản với bộ lọc quyền/email,
/// phân trang và toggle trạng thái kích hoạt/khóa.
/// </summary>
public partial class Admin_QuanLyTaiKhoan : Page
{
    private const int PAGE_SIZE = 20;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: bind dữ liệu lần đầu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) BindData();
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>Xử lý command ToggleTT — kích hoạt/khóa tài khoản.</summary>
    protected void gvTK_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ToggleTT")
        {
            var parts = e.CommandArgument.ToString().Split(',');
            int id    = int.Parse(parts[0]);
            bool cur  = parts[1] == "True";
            TaiKhoanDAL.CapNhatTrangThai(id, !cur);
            BindData();
        }
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    /// <summary>
    /// Gọi TaiKhoanDAL.GetDanhSach với bộ lọc (quyền, email),
    /// bind vào GridView và tạo nút phân trang.
    /// </summary>
    private void BindData()
    {
        int? quyen = int.TryParse(ddlQuyen.SelectedValue, out int q) ? q : (int?)null;
        var result = TaiKhoanDAL.GetDanhSach(CurrentPage, PAGE_SIZE, quyen, txtEmail.Text.Trim());

        litTong.Text = result.TongSo.ToString("N0");
        gvTK.DataSource = result.Data;
        gvTK.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
