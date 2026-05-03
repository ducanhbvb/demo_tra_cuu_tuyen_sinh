using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang hiển thị lỗi chung — nhận mã lỗi từ QueryString (403, 404, 500)
/// và hiển thị thông báo phù hợp.
/// </summary>
public partial class ErrorPage : Page
{
    /// <summary>Đọc mã lỗi từ QueryString và hiển thị tiêu đề + mô tả tương ứng.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string code = Request.QueryString["code"] ?? "500";
        litCode.Text = code switch { "403" => "403 - Không có quyền truy cập", "404" => "404 - Trang không tồn tại", _ => "Đã xảy ra lỗi" };
        litMsg.Text  = code switch { "403" => "Bạn không có quyền xem trang này.", "404" => "Trang bạn tìm kiếm không tồn tại hoặc đã bị xóa.", _ => "Vui lòng thử lại sau." };
    }
}
