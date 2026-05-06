using System;
using System.Collections.Generic;
using System.Data.SqlClient;

/// <summary>
/// DAL cho bảng tbl_Config — đọc/ghi cấu hình hệ thống.
/// Dùng parameterized queries để tránh SQL Injection.
/// </summary>
public static class ConfigDAL
{
    // ── Đọc toàn bộ config ────────────────────────────────────────────────
    /// <summary>Lấy tất cả config dưới dạng Dictionary để cache 1 lần.</summary>
    public static Dictionary<string, string> GetAll()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var dt = DBHelper.Query("SELECT ConfigKey, ConfigValue FROM tbl_Config");
            if (dt == null) return result;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                var key = DBHelper.Val<string>(row["ConfigKey"], "");
                var val = row["ConfigValue"] == DBNull.Value ? null : row["ConfigValue"]?.ToString();
                if (!string.IsNullOrEmpty(key))
                    result[key] = val ?? "";
            }
        }
        catch (Exception ex)
        {
            // Bảng chưa tồn tại hoặc lỗi kết nối → trả về rỗng, không crash
            System.Diagnostics.Debug.WriteLine("[ConfigDAL.GetAll] Error: " + ex.Message);
        }
        return result;
    }

    // ── Đọc 1 config theo key ─────────────────────────────────────────────
    /// <summary>Lấy giá trị string của 1 key. Trả defaultValue nếu không tìm thấy.</summary>
    public static string GetValue(string key, string defaultValue = "")
    {
        try
        {
            var val = DBHelper.Scalar(
                "SELECT ConfigValue FROM tbl_Config WHERE ConfigKey = @key",
                new[] { new SqlParameter("@key", key) });
            if (val == null || val == DBNull.Value) return defaultValue;
            return val.ToString();
        }
        catch
        {
            return defaultValue;
        }
    }

    // ── Ghi/Cập nhật 1 config ─────────────────────────────────────────────
    /// <summary>
    /// Upsert: tạo mới nếu chưa có, cập nhật nếu đã có.
    /// </summary>
    public static void SetValue(string key, string value)
    {
        DBHelper.Execute(@"
            MERGE tbl_Config AS target
            USING (VALUES (@key, @val)) AS source (ConfigKey, ConfigValue)
            ON target.ConfigKey = source.ConfigKey
            WHEN MATCHED THEN
                UPDATE SET ConfigValue = source.ConfigValue, NgayCapNhat = GETDATE()
            WHEN NOT MATCHED THEN
                INSERT (ConfigKey, ConfigValue, NgayCapNhat)
                VALUES (source.ConfigKey, source.ConfigValue, GETDATE());",
            new[]
            {
                new SqlParameter("@key", key),
                new SqlParameter("@val", (object)value ?? DBNull.Value)
            });
    }
}
