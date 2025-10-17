using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiNT.Libraries.Models;

public class ShiftWork
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string numberCar { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")] //Format tiền tệ với 2 chữ số thập phân
    public decimal? revenueByMonth { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? revenueByDate { get; set; }

    public string qrContext { get; set; } = string.Empty;
    public string qrUrl { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? discountOther { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? arrearsOther { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? totalPrice { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? walletGSM { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? discountGSM { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? discountNT { get; set; }

    public string bank_Id { get; set; } = string.Empty;
    public DateTime? createdAt { get; set; }
    public string typeCar { get; set; } = string.Empty;

    public string area { get; set; } = string.Empty;
    public int ranking { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal basicSalary { get; set; } //Không dùng

    // Liên kết
    public List<Trip>? Trips { get; set; } // 1-n
    public List<Contract>? Contracts { get; set; } // 1-n
}
