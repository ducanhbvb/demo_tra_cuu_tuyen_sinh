<%@ Page Language="C#" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // Redirect về /index.aspx (route sẽ ẩn /Client/)
        Response.RedirectPermanent("~/index.aspx", true);
    }
</script>
