
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiNT.Libraries.Models;

public class ContractGroup
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [NotMapped]// Không lưu vào SQL

    public List<Contract> ContractDetails { get; set; } = new();

    [NotMapped]// Không lưu vào SQL
    public int RecordTotal => ContractDetails.Count; // Không lưu vào SQL
}

[Table("Contracts")]
public class Contract
{
    [Key]
    public string ctId { get; set; } = Guid.NewGuid().ToString();

    public string numberCar { get; set; } = string.Empty;
    public string ctKey { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ctAmount { get; set; }
    public string ctDefaultDistance { get; set; } = string.Empty;
    public string ctOverDistance { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ctSurcharge { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ctPromotion { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? totalPrice { get; set; }

    public string userId { get; set; } = string.Empty;
    public DateTime? createdAt { get; set; }

    // Khóa ngoại tới bảng "Danh sách lên ca"
    public string? shiftworkId { get; set; }
    [ForeignKey(nameof(shiftworkId))]
    public ShiftWork? ShiftWork { get; set; } //n-1
}
