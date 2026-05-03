using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý Logs (Admin) — hiển thị lịch sử hoạt động hệ thống
/// với bộ lọc (hành động, trạng thái, thiết bị, email), thống kê tổng/thành công/thất bại.
/// </summary>
public partial class Admin_QuanLyLogs : Page
{
    private const int PAGE_SIZE = 30;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: load dữ liệu logs lần đầu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadData();
    }

    /// <summary>
    /// Build WHERE clause từ bộ lọc, tính thống kê (tổng/thành công/thất bại),
    /// truy vấn logs với phân trang OFFSET-FETCH và bind vào GridView.
    /// </summary>
    private void LoadData()
    {
        // Build WHERE
        string where = "WHERE 1=1";
        var countPrms = new List<SqlParameter>();
        var dataPrms  = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(ddlHanhDong.SelectedValue))
        {
            where += " AND l.HanhDong=@hd";
            countPrms.Add(new SqlParameter("@hd", ddlHanhDong.SelectedValue));
            dataPrms.Add(new SqlParameter("@hd",  ddlHanhDong.SelectedValue));
        }
        if (!string.IsNullOrEmpty(ddlIsSuccess.SelectedValue))
        {
            where += " AND l.IsSuccess=@ok";
            bool ok = ddlIsSuccess.SelectedValue == "1";
            countPrms.Add(new SqlParameter("@ok", ok));
            dataPrms.Add(new SqlParameter("@ok",  ok));
        }
        if (!string.IsNullOrEmpty(ddlThietBi.SelectedValue))
        {
            where += " AND l.LoaiThietBi=@ltd";
            countPrms.Add(new SqlParameter("@ltd", ddlThietBi.SelectedValue));
            dataPrms.Add(new SqlParameter("@ltd",  ddlThietBi.SelectedValue));
        }
        if (!string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            where += " AND tk.Email LIKE @em";
            string kw = $"%{txtEmail.Text.Trim()}%";
            countPrms.Add(new SqlParameter("@em", kw));
            dataPrms.Add(new SqlParameter("@em",  kw));
        }

        // Count tổng / thành công / thất bại
        int tong = DBHelper.Val<int>(DBHelper.Scalar(
            $"SELECT COUNT(1) FROM tbl_Logs l LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan=l.MaTaiKhoan {where}",
            countPrms.ToArray()));

        var statPrms = new List<SqlParameter>(countPrms.Count);
        foreach (var p in countPrms)
            statPrms.Add(new SqlParameter(p.ParameterName, p.Value));

        var statPrms2 = new List<SqlParameter>(countPrms.Count);
        foreach (var p in countPrms)
            statPrms2.Add(new SqlParameter(p.ParameterName, p.Value));

        int thanhCong = DBHelper.Val<int>(DBHelper.Scalar(
            $"SELECT COUNT(1) FROM tbl_Logs l LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan=l.MaTaiKhoan {where} AND l.IsSuccess=1",
            statPrms.ToArray()));
        int thatBai = tong - thanhCong;

        litTong.Text      = tong.ToString("N0");
        litThanhCong.Text = thanhCong.ToString("N0");
        litThatBai.Text   = thatBai.ToString("N0");

        // Data với phân trang
        dataPrms.Add(new SqlParameter("@skip", CurrentPage * PAGE_SIZE));
        dataPrms.Add(new SqlParameter("@take", PAGE_SIZE));

        var dt = DBHelper.Query($@"
            SELECT l.LogID, l.HanhDong, l.ThoiGian, l.IPAddress, l.LoaiThietBi,
                   l.MoTa, l.IsSuccess, l.MaLoi, l.TrangUrl, l.SessionID,
                   tk.Email
            FROM tbl_Logs l
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = l.MaTaiKhoan
            {where}
            ORDER BY l.ThoiGian DESC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
            dataPrms.ToArray());

        gvLogs.DataSource = dt;
        gvLogs.DataBind();
        BindPaging(tong);
    }

    /// <summary>Tạo danh sách nút phân trang.</summary>
    private void BindPaging(int tongSo)
    {
        int total = (int)Math.Ceiling((double)tongSo / PAGE_SIZE);
        var pages = new List<object>();
        for (int i = 0; i < total; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        LoadData();
    }

    /// <summary>Xử lý nút Reset — xóa tất cả bộ lọc về mặc định.</summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlHanhDong.SelectedIndex = 0;
        ddlIsSuccess.SelectedIndex = 0;
        ddlThietBi.SelectedIndex = 0;
        txtEmail.Text = "";
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
