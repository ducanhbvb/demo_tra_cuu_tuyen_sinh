using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Tìm kiếm trường — lọc theo tên, tỉnh/thành, loại trường, ngành đào tạo;
/// hiển thị danh sách trường dạng card với phân trang.
/// </summary>
public partial class TimKiemTruong : Page
{
    private const int PAGE_SIZE = 12;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage
    {
        get => ViewState["Page"] is int p ? p : 0;
        set => ViewState["Page"] = value;
    }

    /// <summary>Khởi tạo trang: load dropdown, đọc QueryString bộ lọc và bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropdowns();
            // Lấy param từ URL
            txtTen.Text = Request.QueryString["q"] ?? "";
            if (byte.TryParse(Request.QueryString["loai"], out byte loai))
                ddlLoai.SelectedValue = loai.ToString();
            CurrentPage = 0;
            BindData();
        }
    }

    /// <summary>Load danh sách tỉnh/thành và ngành đào tạo vào DropDownList.</summary>
    private void LoadDropdowns()
    {
        foreach (var t in DanhMucDAL.GetTinhThanh())
            ddlTinh.Items.Add(new ListItem(t, t));

        foreach (var n in DanhMucDAL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý nút Reset — xóa tất cả bộ lọc và tải lại toàn bộ dữ liệu.</summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtTen.Text = "";
        ddlTinh.SelectedIndex = 0;
        ddlLoai.SelectedIndex = 0;
        ddlNganh.SelectedIndex = 0;
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page")
        {
            CurrentPage = int.Parse(e.CommandArgument.ToString());
            BindData();
        }
    }

    /// <summary>
    /// Gọi TruongBLL.TimKiem với các bộ lọc, bind kết quả vào Repeater
    /// và tạo danh sách nút phân trang.
    /// </summary>
    private void BindData()
    {
        byte? loai = byte.TryParse(ddlLoai.SelectedValue, out byte l) ? l : (byte?)null;
        int?  nganh = int.TryParse(ddlNganh.SelectedValue, out int n) ? n : (int?)null;

        // Phía client: chỉ hiển thị trường đang active (TrangThai=1)
        var result = TruongBLL.TimKiem(
            txtTen.Text.Trim(),
            ddlTinh.SelectedValue,
            null, loai, nganh, CurrentPage, PAGE_SIZE, trangThai: true);

        litTong.Text = result.TongSo.ToString("N0");
        rptKetQua.DataSource = result.Data;
        rptKetQua.DataBind();

        // Phân trang
        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
