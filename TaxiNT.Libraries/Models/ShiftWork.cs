using System.ComponentModel.DataAnnotations;

namespace TaxiNT.Libraries.Models;

public class ShiftWork
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string numberCar { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;

    public decimal? revenueByMonth { get; set; }
    public decimal? revenueByDate { get; set; }

    public string qrContext { get; set; } = string.Empty;
    public string qrUrl { get; set; } = string.Empty;

    public decimal? discountOther { get; set; }
    public decimal? arrearsOther { get; set; }

    public decimal? totalPrice { get; set; }

    public decimal? walletGSM { get; set; }
    public decimal? discountGSM { get; set; }
    public decimal? discountNT { get; set; }

    public string bank_Id { get; set; } = string.Empty;
    public DateTime? createdAt { get; set; }
    public string typeCar { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;
    public int Rank { get; set; }
    public decimal SauMucAnChia { get; set; } //Không dùng

    // Liên kết
    public List<TripDetail>? Trips { get; set; } // 1-n
    public List<ContractDetail>? Contracts { get; set; } // 1-n
}
