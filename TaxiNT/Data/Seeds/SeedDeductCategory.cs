using Microsoft.EntityFrameworkCore;
using TaxiNT.Data;
using TaxiNT.Libraries.Models;

namespace TVTMedia.Data.Seeds;

public static class SeedDeductCategory
{
    public static async Task SeedDeductCategoryAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<taxiNTDBContext>();

        // ===== 1) Upsert DeductCategories =====
        var deductCategory = new[]
        {
            new DeductCategory { Id = 1, SortOrder = 1, Code = "phiThuongHieu", Name = "Phí thương hiệu", IsActive = true },
            new DeductCategory { Id = 2, SortOrder = 2, Code = "kyQuyLaiXe", Name = "Ký quỹ lái xe", IsActive = true },
            new DeductCategory { Id = 3, SortOrder = 3, Code = "kyQuyThietBiBoDam", Name = "Ký quỹ thiết bị bộ đàm", IsActive = true },
            new DeductCategory { Id = 4, SortOrder = 4, Code = "kyQuyKhoanXe", Name = "Ký quỹ khoán xe", IsActive = true },
            new DeductCategory { Id = 5, SortOrder = 5, Code = "noAmLaiXe", Name = "Nợ âm lái xe", IsActive = true },
            new DeductCategory { Id = 6, SortOrder = 6, Code = "noAmThuongQuyen", Name = "Nợ âm thương quyền", IsActive = true },
            new DeductCategory { Id = 7, SortOrder = 7, Code = "truGocVay", Name = "Trừ gốc vay", IsActive = true },
            new DeductCategory { Id = 8, SortOrder = 8, Code = "truLaiVay", Name = "Trừ lãi vay", IsActive = true },
            new DeductCategory { Id = 9, SortOrder = 9, Code = "noDoanhThuTamUng", Name = "Nợ doanh thu tạm ứng", IsActive = true },
            new DeductCategory { Id = 10, SortOrder = 10, Code = "noViPham", Name = "Nợ vi phạm", IsActive = true },
            new DeductCategory { Id = 11, SortOrder = 11, Code = "noSuaChua", Name = "Nợ sửa chữa", IsActive = true },
            new DeductCategory { Id = 12,  SortOrder = 12, Code = "hoTroChecker", Name = "Hỗ trợ checker", IsActive = true },
            new DeductCategory { Id = 13, SortOrder = 13, Code = "thueHkd", Name = "Thuế HKD", IsActive = true },
            new DeductCategory { Id = 14, SortOrder = 14, Code = "thueTncn", Name = "Thuế TNCN", IsActive = true },
            new DeductCategory { Id = 15, SortOrder = 15, Code = "phatKhongRuaXe", Name = "Phạt không rửa xe", IsActive = true },
            new DeductCategory { Id = 16, SortOrder = 16, Code = "bhvcBhtnds", Name = "bhvc-bhtnds", IsActive = true },
            new DeductCategory { Id = 17, SortOrder = 17, Code = "tienSacPin", Name = "Tiền sạc pin", IsActive = true },
            new DeductCategory { Id = 18, SortOrder = 18, Code = "phiPhatSac", Name = "Phí phạt sạc", IsActive = true },
            new DeductCategory { Id = 19, SortOrder = 19, Code = "luongUng", Name = "Lương ứng", IsActive = true },
            new DeductCategory { Id = 20, SortOrder = 20, Code = "kiemDinhDongHo", Name = "Kiểm định đồng hồ", IsActive = true },
            new DeductCategory { Id = 21, SortOrder = 21, Code = "truAmLuongLaiXe", Name = "Trừ âm lương lái xe", IsActive = true },
            new DeductCategory { Id = 22, SortOrder = 22, Code = "tienThueXe", Name = "Tiền thuê xe", IsActive = true },
            new DeductCategory { Id = 23, SortOrder = 23, Code = "ngungCaKinhDoanh", Name = "Ngưng ca kinh doanh", IsActive = true },
            new DeductCategory { Id = 24, SortOrder = 24, Code = "xangNgoai", Name = "Xăng ngoài", IsActive = true },
            new DeductCategory { Id = 25, SortOrder = 25, Code = "truyThu", Name = "Truy thu", IsActive = true },
            new DeductCategory { Id = 26, SortOrder = 26, Code = "truAppXanhSm", Name = "Trừ APP Xanh SM", IsActive = true },
            new DeductCategory { Id = 27, SortOrder = 27, Code = "phiQuaTram", Name = "Phí qua trạm", IsActive = true },
            new DeductCategory { Id = 28, SortOrder = 28, Code = "haoMonVoXe", Name = "Hao mòn vỏ xe", IsActive = true },
            new DeductCategory { Id = 29, SortOrder = 29, Code = "bhxh", Name = "BHXH", IsActive = true },
            new DeductCategory { Id = 30, SortOrder = 30, Code = "truAmLuongThuongQuyen", Name = "Trừ âm lương thương quyền", IsActive = true },
            new DeductCategory { Id = 31, SortOrder = 31, Code = "thuePin", Name = "Thuê pin", IsActive = true },
            new DeductCategory { Id = 32, SortOrder = 32, Code = "phiDangKiem", Name = "Phí đăng kiểm", IsActive = true },
            new DeductCategory { Id = 33, SortOrder = 33, Code = "truAmLuongXeKhoan", Name = "Trừ âm lương xe khoán", IsActive = true },
            new DeductCategory { Id = 34, SortOrder = 34, Code = "thuHoLaiXe", Name = "Thu hộ lái xe", IsActive = true },
            new DeductCategory { Id = 35, SortOrder = 35, Code = "truKhac", Name = "Trừ khác", IsActive = true }
        };

        var catIds = deductCategory.Select(x => x.Id).ToList();

        var catDb = await db.DeductCategories
            .Where(x => catIds.Contains(x.Id))
            .ToListAsync();

        foreach (var seed in deductCategory)
        {
            var exist = catDb.FirstOrDefault(x => x.Id == seed.Id);

            if (exist is null)
            {
                db.DeductCategories.Add(seed);
            }
            else
            {
                exist.SortOrder = seed.SortOrder;
                exist.Code = seed.Code;
                exist.Name = seed.Name;
                exist.IsActive = seed.IsActive;
            }
        }

        await db.SaveChangesAsync();
    }
}