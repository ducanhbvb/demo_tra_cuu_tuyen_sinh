using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>Tiện ích kết nối SQL Server, dùng chung cho toàn bộ DAL.</summary>
public static class DBHelper
{
    private static readonly string ConnStr = ResolveConnectionString();

    private static string ResolveConnectionString()
    {
        // Prefer the new key, then fall back to the legacy key.
        var cs =
            ConfigurationManager.ConnectionStrings["TraCuuTuyenSinh_V1"] ??
            ConfigurationManager.ConnectionStrings["TraCuuTuyenSinh"];

        if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
        {
            throw new ConfigurationErrorsException(
                "Missing connection string. Add 'TraCuuTuyenSinh_V1' or 'TraCuuTuyenSinh' in Web.config <connectionStrings>.");
        }

        return cs.ConnectionString;
    }

    public static SqlConnection GetConnection() => new SqlConnection(ConnStr);

    // ── Trả DataTable (thường dùng nhất) ──────────────────────────────────
    public static DataTable Query(string sql, SqlParameter[] p = null, bool isSP = false)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn)
            { CommandType = isSP ? CommandType.StoredProcedure : CommandType.Text };
        if (p != null) cmd.Parameters.AddRange(p);
        var da = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    // ── Trả DataSet (dùng khi SP trả nhiều result-set) ────────────────────
    public static DataSet QuerySet(string sql, SqlParameter[] p = null, bool isSP = false)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn)
            { CommandType = isSP ? CommandType.StoredProcedure : CommandType.Text };
        if (p != null) cmd.Parameters.AddRange(p);
        var da = new SqlDataAdapter(cmd);
        var ds = new DataSet();
        da.Fill(ds);
        return ds;
    }

    // ── Execute (INSERT/UPDATE/DELETE) ────────────────────────────────────
    public static int Execute(string sql, SqlParameter[] p = null, bool isSP = false)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn)
            { CommandType = isSP ? CommandType.StoredProcedure : CommandType.Text };
        if (p != null) cmd.Parameters.AddRange(p);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    // ── Scalar ────────────────────────────────────────────────────────────
    public static object Scalar(string sql, SqlParameter[] p = null, bool isSP = false)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn)
            { CommandType = isSP ? CommandType.StoredProcedure : CommandType.Text };
        if (p != null) cmd.Parameters.AddRange(p);
        conn.Open();
        return cmd.ExecuteScalar();
    }

    // ── Gọi SP có OUTPUT params, trả SqlCommand để đọc OUTPUT ─────────────
    public static void ExecSP(string spName, SqlParameter[] p)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(spName, conn)
            { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddRange(p);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    // ── Helper chuyển DBNull sang kiểu C# ─────────────────────────────────
    public static T Val<T>(object val, T def = default)
    {
        if (val == null || val == DBNull.Value) return def;
        try { return (T)Convert.ChangeType(val, typeof(T)); }
        catch { return def; }
    }

    public static T? ValN<T>(object val) where T : struct
    {
        if (val == null || val == DBNull.Value) return null;
        try { return (T)Convert.ChangeType(val, typeof(T)); }
        catch { return null; }
    }
}
