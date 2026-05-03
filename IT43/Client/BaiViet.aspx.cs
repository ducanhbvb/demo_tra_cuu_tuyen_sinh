using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Danh sách bài viết — lọc theo trường, phân trang,
/// hiển thị bài viết dạng card (ảnh, tiêu đề, tóm tắt).
/// </summary>
public partial class BaiViet_Page : Page
{
    private const int PAGE_SIZE = 9;

    /// <summary>Trang hiện tại (lưu ViewState để persist qua PostBack).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: load bộ lọc trường và dữ liệu bài viết.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadTruongFilter();
            LoadData();
        }
    }

    /// <summary>Load danh sách trường vào DropDownList bộ lọc.</summary>
    private void LoadTruongFilter()
    {
        var dt = DBHelper.Query(
            "SELECT MaTruong, TenTruong FROM tbl_Truong ORDER BY TenTruong");
        foreach (DataRow r in dt.Rows)
            ddlTruong.Items.Add(new ListItem(
                r["TenTruong"].ToString(),
                r["MaTruong"].ToString()));
    }

    /// <summary>Truy vấn bài viết theo bộ lọc và bind vào Repeater + phân trang.</summary>
    private void LoadData()
    {
        int? maTruong = null;
        if (int.TryParse(ddlTruong.SelectedValue, out int mt) && mt > 0)
            maTruong = mt;

        var paged = BaiVietDAL.GetDanhSach(maTruong, CurrentPage, PAGE_SIZE, chiActive: true);

        if (paged.TongSo == 0)
        {
            rptBaiViet.DataSource = null;
            rptBaiViet.DataBind();
            pnlEmpty.Visible = true;
        }
        else
        {
            pnlEmpty.Visible = false;
            rptBaiViet.DataSource = paged.Data;
            rptBaiViet.DataBind();
        }

        litTong.Text = paged.TongSo.ToString("N0");
        BindPaging(paged.TongSo);
    }

    /// <summary>Tạo danh sách nút phân trang dựa trên tổng số bài viết.</summary>
    private void BindPaging(int tongSo)
    {
        int totalPages = (int)Math.Ceiling((double)tongSo / PAGE_SIZE);
        var pages = new List<object>();
        for (int i = 0; i < totalPages; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });

        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    /// <summary>Khi thay đổi trường trong dropdown → reset trang và tải lại.</summary>
    protected void ddlTruong_Changed(object sender, EventArgs e)
    {
        CurrentPage = 0;
        LoadData();
    }

    /// <summary>Nút "Xem tất cả" — xóa bộ lọc trường và hiển thị toàn bộ bài viết.</summary>
    protected void btnXemTat_Click(object sender, EventArgs e)
    {
        ddlTruong.SelectedIndex = 0;
        CurrentPage = 0;
        LoadData();
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            LoadData();
        }
    }
}
