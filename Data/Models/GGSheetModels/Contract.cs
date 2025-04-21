using ntchecker.Extensions;

namespace ntchecker.Data.Models.GGSheetModels;
public class Contract
{
    public string userId { get; set; } = string.Empty;
    public List<ContractDetail>? contracts { get; set; }

    public string TotalPrice => contracts?.ltvSumFieldValues<ContractDetail>(e => e.totalPrice);
    public int count => contracts?.Count ?? 0;

}

public class ContractDetail
{
    public string ctId { get; set; } = string.Empty;
    public string numberCar { get; set; } = string.Empty;
    public string ctKey { get; set; } = string.Empty;
    public string ctAmount { get; set; } = string.Empty;
    public string ctDefaultDistance { get; set; } = string.Empty; // Khoản các mặc định
    public string ctOverDistance { get; set; } = string.Empty;  // Khoản cách vượt
    public string ctSurcharge { get; set; } = string.Empty; // Phụ phí
    public string ctPromotion { get; set; } = string.Empty; // Khuyến mãi
    public string totalPrice { get; set; } = string.Empty; // Thành tiền
    public string userId { get; set; } = string.Empty; // Key
    public string createdAt { get; set; } = string.Empty;
}
