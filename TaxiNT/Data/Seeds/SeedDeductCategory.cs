using Microsoft.EntityFrameworkCore;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Data.Seeds;

public static class SeedDeductCategory
{
    public static async Task SeedDeductCategoryAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<taxiNTDBContext>();

        // ===== 1) Upsert DeductCategories =====
        var deductCategory = new[]
        {
            new DeductCategory { SortOrder = 1, Code = "phiThuongHieu", Name = "Phí thương hiệu", IsActive = true },
            new DeductCategory { SortOrder = 2, Code = "kyQuyLaiXe", Name = "Ký quỹ lái xe", IsActive = true },
            new DeductCategory { SortOrder = 3, Code = "kyQuyThietBiBoDam", Name = "Ký quỹ thiết bị bộ đàm", IsActive = true },
            new DeductCategory { SortOrder = 4, Code = "kyQuyKhoanXe", Name = "Ký quỹ khoán xe", IsActive = true },
            new DeductCategory { SortOrder = 5, Code = "noAmLaiXe", Name = "Nợ âm lái xe", IsActive = true },
            new DeductCategory { SortOrder = 6, Code = "noAmThuongQuyen", Name = "Nợ âm thương quyền", IsActive = true },
            new DeductCategory { SortOrder = 7, Code = "truGocVay", Name = "Trừ gốc vay", IsActive = true },
            new DeductCategory { SortOrder = 8, Code = "truLaiVay", Name = "Trừ lãi vay", IsActive = true },
            new DeductCategory { SortOrder = 9, Code = "noDoanhThuTamUng", Name = "Nợ doanh thu tạm ứng", IsActive = true },
            new DeductCategory { SortOrder = 10, Code = "noViPham", Name = "Nợ vi phạm", IsActive = true },
            new DeductCategory { SortOrder = 11, Code = "noSuaChua", Name = "Nợ sửa chữa", IsActive = true },
            new DeductCategory { SortOrder = 12, Code = "hoTroChecker", Name = "Hỗ trợ checker", IsActive = true },
            new DeductCategory { SortOrder = 13, Code = "thueHkd", Name = "Thuế HKD", IsActive = true },
            new DeductCategory { SortOrder = 14, Code = "thueTncn", Name = "Thuế TNCN", IsActive = true },
            new DeductCategory { SortOrder = 15, Code = "phatKhongRuaXe", Name = "Phạt không rửa xe", IsActive = true },
            new DeductCategory { SortOrder = 16, Code = "bhvcBhtnds", Name = "bhvc-bhtnds", IsActive = true },
            new DeductCategory { SortOrder = 17, Code = "tienSacPin", Name = "Tiền sạc pin", IsActive = true },
            new DeductCategory { SortOrder = 18, Code = "phiPhatSac", Name = "Phí phạt sạc", IsActive = true },
            new DeductCategory { SortOrder = 19, Code = "luongUng", Name = "Lương ứng", IsActive = true },
            new DeductCategory { SortOrder = 20, Code = "kiemDinhDongHo", Name = "Kiểm định đồng hồ", IsActive = true },
            new DeductCategory { SortOrder = 21, Code = "truAmLuongLaiXe", Name = "Trừ âm lương lái xe", IsActive = true },
            new DeductCategory { SortOrder = 22, Code = "tienThueXe", Name = "Tiền thuê xe", IsActive = true },
            new DeductCategory { SortOrder = 23, Code = "ngungCaKinhDoanh", Name = "Ngưng ca kinh doanh", IsActive = true },
            new DeductCategory { SortOrder = 24, Code = "xangNgoai", Name = "Xăng ngoài", IsActive = true },
            new DeductCategory { SortOrder = 25, Code = "truyThu", Name = "Truy thu", IsActive = true },
            new DeductCategory { SortOrder = 26, Code = "truAppXanhSm", Name = "Trừ APP Xanh SM", IsActive = true },
            new DeductCategory { SortOrder = 27, Code = "phiQuaTram", Name = "Phí qua trạm", IsActive = true },
            new DeductCategory { SortOrder = 28, Code = "haoMonVoXe", Name = "Hao mòn vỏ xe", IsActive = true },
            new DeductCategory { SortOrder = 29, Code = "bhxh", Name = "BHXH", IsActive = true },
            new DeductCategory { SortOrder = 30, Code = "truAmLuongThuongQuyen", Name = "Trừ âm lương thương quyền", IsActive = true },
            new DeductCategory { SortOrder = 31, Code = "thuePin", Name = "Thuê pin", IsActive = true },
            new DeductCategory { SortOrder = 32, Code = "phiDangKiem", Name = "Phí đăng kiểm", IsActive = true },
            new DeductCategory { SortOrder = 33, Code = "truAmLuongXeKhoan", Name = "Trừ âm lương xe khoán", IsActive = true },
            new DeductCategory { SortOrder = 34, Code = "thuHoLaiXe", Name = "Thu hộ lái xe", IsActive = true },
            new DeductCategory { SortOrder = 35, Code = "truKhac", Name = "Trừ khác", IsActive = true }
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