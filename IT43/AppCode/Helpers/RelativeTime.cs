using System;

/// <summary>
/// Helper chuyển DateTime thành dạng tương đối: "X phút trước", "X giờ trước", v.v.
/// Dùng cho timeline thread tư vấn và danh sách Hộp thư.
/// </summary>
public static class RelativeTime
{
    /// <summary>
    /// Trả về chuỗi tương đối so với thời điểm hiện tại.
    /// Ví dụ: "Vừa xong", "5 phút trước", "2 giờ trước", "3 ngày trước", "dd/MM/yyyy".
    /// </summary>
    public static string From(DateTime dt)
    {
        var diff = DateTime.Now - dt;

        if (diff.TotalSeconds < 30)  return "Vừa xong";
        if (diff.TotalMinutes < 1)   return "Dưới 1 phút trước";
        if (diff.TotalMinutes < 60)  return $"{(int)diff.TotalMinutes} phút trước";
        if (diff.TotalHours   < 24)  return $"{(int)diff.TotalHours} giờ trước";
        if (diff.TotalDays    < 7)   return $"{(int)diff.TotalDays} ngày trước";
        if (diff.TotalDays    < 30)  return $"{(int)(diff.TotalDays / 7)} tuần trước";
        if (diff.TotalDays    < 365) return $"{(int)(diff.TotalDays / 30)} tháng trước";

        // Ngày cụ thể nếu quá 1 năm
        return dt.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Trả chuỗi relative time kèm tooltip title với ngày giờ đầy đủ.
    /// Dùng trong template: &lt;span title="..."&gt;30 phút trước&lt;/span&gt;
    /// </summary>
    public static string FromWithTitle(DateTime dt)
    {
        string rel  = From(dt);
        string full = dt.ToString("HH:mm dd/MM/yyyy");
        return $"<span title=\"{full}\">{rel}</span>";
    }
}
