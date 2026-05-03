using System;
using System.Collections.Generic;
using System.Data;

/// <summary>Kết quả phân trang generic (dùng cho List&lt;T&gt;).</summary>
public class PagedResult<T>
{
    public List<T> Items    { get; set; } = new List<T>();
    public int     TongSo   { get; set; }
    public int     PageIndex { get; set; }
    public int     PageSize  { get; set; }

    public int TongTrang =>
        PageSize > 0 ? (int)Math.Ceiling((double)TongSo / PageSize) : 0;
}

/// <summary>Kết quả phân trang DataTable (dùng cho GridView bind thẳng).</summary>
public class PagedTable
{
    public DataTable Data      { get; set; }
    public int       TongSo    { get; set; }
    public int       PageIndex { get; set; }
    public int       PageSize  { get; set; }

    public int TongTrang =>
        PageSize > 0 ? (int)Math.Ceiling((double)TongSo / PageSize) : 0;
}
