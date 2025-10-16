using TaxiNT.Libraries.Extensions;

namespace TaxiNT.Libraries.Models.GGSheets;

public class RevenueDetail
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); //Tự động
    public string numberCar { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;
    public string revenueByMonth { get; set; } = string.Empty;
    public string revenueByDate { get; set; } = string.Empty;
    public string qrContext { get; set; } = string.Empty;
    public string qrUrl { get; set; } = string.Empty;
    public string discountOther { get; set; } = string.Empty;
    public string arrearsOther { get; set; } = string.Empty;
    public string totalPrice { get; set; } = string.Empty;
    public string walletGSM { get; set; } = string.Empty;
    public string discountGSM { get; set; } = string.Empty;
    public string discountNT { get; set; } = string.Empty;
    public string bank_Id { get; set; } = string.Empty;
    public string createdAt { get; set; } = string.Empty;
    public string typeCar { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
}

public class Revenue
{
    // Khi 1 tài 2 xe cần tổng lại một số cột quan trọng
    public string userId { get; set; } = string.Empty;
    public string bank_Id { get; set; } = string.Empty;
    public List<RevenueDetail>? revenues { get; set; }
    public Bank? bank { get; set; }
    public string qrUrl { get; set; } = string.Empty;

    public List<string>? numberCar => revenues?.Select(r => r.numberCar).Distinct().ToList() ?? new(); // Lấy danh sách các xe có trong danh sách chi tiết
    public string revenueByMonth => new List<string> { revenues?.FirstOrDefault().revenueByMonth
                                                , revenues.ltvSumFieldValues<RevenueDetail>(e => e.revenueByDate) }.ltvSumFieldValues(e =>e);
    public string revenueByDate => revenues.ltvSumFieldValues<RevenueDetail>(e => e.revenueByDate); // Tổng doanh thu ngày
    public string discountOther => revenues.ltvSumFieldValues<RevenueDetail>(e => e.discountOther); // Khoản trừ
    public string arrearsOther => revenues.ltvSumFieldValues<RevenueDetail>(e => e.arrearsOther); // Truy thu
    public string totalPrice => revenues.ltvSumFieldValues<RevenueDetail>(e => e.totalPrice); // Tiền nợp về
    public string walletGSM => revenues.ltvSumFieldValues<RevenueDetail>(e => e.walletGSM); // Tiền ví GSM
    public string discountGSM => revenues.ltvSumFieldValues<RevenueDetail>(e => e.discountGSM); // Tiền giảm giá GSM
    public string discountNT => revenues.ltvSumFieldValues<RevenueDetail>(e => e.discountNT); // Tiền giảm giá Nam Thắng
    public string createdAt => revenues?.FirstOrDefault().createdAt ?? string.Empty;
    public string typeCar => revenues?.FirstOrDefault().typeCar ?? string.Empty; // XE ĐIỆN HOẶC XE KHOÁN
    public string area => revenues?.FirstOrDefault().Area ?? string.Empty; // XE ĐIỆN HOẶC XE KHOÁN
    public string rank => revenues?.FirstOrDefault().Rank ?? string.Empty; // XE ĐIỆN HOẶC XE KHOÁN
}
