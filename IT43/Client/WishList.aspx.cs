using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Danh sách yêu thích — hiển thị các trường/tin mà người dùng đã lưu,
/// cho phép xóa từng mục khỏi wishlist.
/// </summary>
public partial class Profile_WishList : Page
{
    /// <summary>Mã tài khoản đang đăng nhập.</summary>
    private int MaTK => SecurityHelper.GetCurrentMaTaiKhoan();

    /// <summary>Kiểm tra đăng nhập, nếu chưa → redirect Login; ngược lại bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated) { Response.Redirect("~/Client/Login.aspx"); return; }
        if (!IsPostBack) BindData();
    }

    /// <summary>Lấy danh sách wishlist theo tài khoản và bind vào Repeater.</summary>
    private void BindData()
    {
        var data = WishListDAL.GetByTaiKhoan(MaTK);
        rptWishList.DataSource = data;
        rptWishList.DataBind();
        pnlEmpty.Visible = (data == null || !data.Any());
    }

    /// <summary>Xử lý lệnh "Xóa" — xóa mục wishlist theo ID và refresh danh sách.</summary>
    protected void rptWishList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "XoaWish" && int.TryParse(e.CommandArgument.ToString(), out int id))
        {
            WishListDAL.Xoa(id, MaTK);
            litThongBao.Text = "<div class='alert alert-success alert-dismissible fade show small'>Đã xóa khỏi danh sách yêu thích.</div>";
            BindData();
        }
    }
}
