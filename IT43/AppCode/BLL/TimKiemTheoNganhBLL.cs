/// <summary>Business Logic Layer cho trang Tìm kiếm theo ngành.</summary>
public static class TimKiemTheoNganhBLL
{
    public static PagedTable TimKiem(int? maChuyenNganh, int? namTuyenSinh, int pageIndex, int pageSize)
        => TimKiemTheoNganhDAL.TimKiem(maChuyenNganh, namTuyenSinh, pageIndex, pageSize);
}
