using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Web;

/// <summary>
/// Generic Handler — trả JSON cho Chart.js vẽ biểu đồ xu hướng điểm chuẩn.
/// URL: /Handlers/DiemChuanChart.ashx?maTruong=1
/// </summary>
public class Handlers_DiemChuanChart : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.AddHeader("Access-Control-Allow-Origin", "*");

        int.TryParse(context.Request.QueryString["maTruong"], out int maTruong);
        if (maTruong == 0)
        {
            context.Response.Write("{\"error\":\"maTruong required\"}");
            return;
        }

        // Cache 10 phút — dữ liệu điểm chuẩn ít thay đổi
        context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(10));
        context.Response.Cache.SetMaxAge(TimeSpan.FromMinutes(10));
        context.Response.Cache.VaryByParams["maTruong"] = true;

        // Lấy dữ liệu điểm chuẩn theo năm, nhóm theo ngành
        var dt = DBHelper.Query(@"
            SELECT ls.NamTuyenSinh, cn.TenChuyenNganh, ISNULL(ls.DiemChuan, 0) AS DiemChuan
            FROM tbl_DiemChuanLichSu ls
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = ls.MaChuyenNganh
            WHERE ls.MaTruong = @tr
            ORDER BY cn.TenChuyenNganh, ls.NamTuyenSinh",
            new[] { new SqlParameter("@tr", maTruong) });

        // Thu thập labels (năm) và datasets (theo ngành)
        var years    = new SortedSet<int>();
        var datasets = new Dictionary<string, Dictionary<int, decimal>>();

        foreach (System.Data.DataRow r in dt.Rows)
        {
            int    nam   = DBHelper.Val<int>(r["NamTuyenSinh"]);
            string nganh = r["TenChuyenNganh"].ToString();
            decimal diem = DBHelper.Val<decimal>(r["DiemChuan"]);

            years.Add(nam);
            if (!datasets.ContainsKey(nganh))
                datasets[nganh] = new Dictionary<int, decimal>();
            datasets[nganh][nam] = diem;
        }

        // Build JSON
        var sb = new StringBuilder();
        sb.Append("{\"labels\":[");
        sb.Append(string.Join(",", years));
        sb.Append("],\"datasets\":[");

        // Bảng màu cố định cho tối đa 10 ngành
        string[] colors = {
            "#4e79a7","#f28e2b","#e15759","#76b7b2",
            "#59a14f","#edc948","#b07aa1","#ff9da7",
            "#9c755f","#bab0ac"
        };
        int ci = 0;
        bool firstDs = true;
        foreach (var kv in datasets)
        {
            if (!firstDs) sb.Append(",");
            firstDs = false;
            string color = colors[ci % colors.Length];
            ci++;

            sb.Append("{");
            sb.Append($"\"label\":\"{EscapeJson(kv.Key)}\",");
            sb.Append($"\"borderColor\":\"{color}\",");
            sb.Append($"\"backgroundColor\":\"{color}22\",");
            sb.Append("\"tension\":0.3,\"fill\":false,\"data\":[");

            bool firstPt = true;
            foreach (int yr in years)
            {
                if (!firstPt) sb.Append(",");
                firstPt = false;
                sb.Append(kv.Value.ContainsKey(yr) ? kv.Value[yr].ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "null");
            }
            sb.Append("]}");
        }
        sb.Append("]}");

        context.Response.Write(sb.ToString());
    }

    private static string EscapeJson(string s)
        => s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";

    public bool IsReusable => false;
}
