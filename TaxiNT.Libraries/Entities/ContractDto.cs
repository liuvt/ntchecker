
namespace TaxiNT.Libraries.Entities;
public class ContractDto
{
    public string userId { get; set; } = string.Empty;
    public string numberCar { get; set; } = string.Empty;
    public string ctKey { get; set; } = string.Empty;
    public decimal? ctDefaultAmount { get; set; } // Giá hợp đồng
    public string ctDefaultDistance { get; set; } = string.Empty; // Thông tin hợp đồng
    public string ctOverDistance { get; set; } = string.Empty; // Thông tin vượt so với hợp đồng
    public decimal ctSurcharge { get; set; } // Phụ phí
    public decimal? ctPromotion { get; set; }
    public decimal ctTotalPrice { get; set; } // Thành tiền
    public string createdAt { get; set; } = string.Empty; //Ngày nợp tiền về
}

// Dùng để Mapper từ SQL Server
public class ContractSQLDto
{
    public string ctId { get; set; } = string.Empty;
    public string numberCar { get; set; } = string.Empty;
    public string ctKey { get; set; } = string.Empty;
    public decimal? ctAmout { get; set; }
    public string ctDefaultDistance { get; set; } = string.Empty;
    public string ctOverDistance { get; set; } = string.Empty;
    public decimal? ctSurcharge { get; set; }
    public decimal? ctPromotion { get; set; }
    public decimal? totalPrice { get; set; }
    public DateTime? createdAt { get; set; }
}

