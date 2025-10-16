namespace TaxiNT.Libraries.Models.GGSheets;
public class Bank
{
    public string bank_Id { get; set; } = string.Empty;
    public string bank_NumberId { get; set; } = string.Empty;
    public string bank_Name { get; set; } = string.Empty;
    public string bank_Number { get; set; } = string.Empty;
    public string bank_Type { get; set; } = string.Empty;
    public string bank_AccountName { get; set; } = string.Empty;
    public string bank_Url { get; set; } = string.Empty;
    public string bank_Status { get; set; } = string.Empty; // Kiểm tra trạng thái tài khoản ngân hàng
    public string createdAt { get; set; } = string.Empty; // Kiểm tra trạng thái tài khoản ngân hàng
}

