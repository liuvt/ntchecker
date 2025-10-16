namespace TaxiNT.Libraries.Entities;
public class RevenueDto
{
    public string userId { get; set; } = string.Empty; // ID
    public string qrUrl { get; set; } = string.Empty; //QR chuyển khoản
    public string numberCar { get; set; } = string.Empty; //Danh sách các xe chạy
    public decimal revenueByMonth { get; set; } //Doanh thu tháng n-1
    public decimal revenueByDate { get; set; } //Doanh thu ngày
    public decimal discountOther { get; set; } //Khoản trừ khác
    public decimal arrearsOther { get; set; } //Truy thu khác
    public decimal totalPrice { get; set; } //Tiền nợp
    public decimal walletGSM { get; set; } //Ví
    public decimal discountGSM { get; set; } //Giảm giá GSM
    public decimal discountNT { get; set; } //Giảm giá NT
    public string createdAt { get; set; } = string.Empty;//Ngày nợp tiền
    public string typeCar { get; set; } = string.Empty; //Loại hình: Khoán - không khoán
    public string area { get; set; } = string.Empty; 
    public string rank { get; set; } = string.Empty;
}


