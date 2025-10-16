
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiNT.Libraries.Models;

public class Contract
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [NotMapped]// Không lưu vào SQL

    public List<ContractDetail> ContractDetails { get; set; } = new();

    [NotMapped]// Không lưu vào SQL
    public int RecordTotal => ContractDetails.Count; // Không lưu vào SQL
}

[Table("Contracts")]
public class ContractDetail
{
    [Key]
    public string ctId { get; set; } = Guid.NewGuid().ToString();

    public string numberCar { get; set; } = string.Empty;
    public string ctKey { get; set; } = string.Empty;
    public decimal? ctAmount { get; set; }
    public string ctDefaultDistance { get; set; } = string.Empty;
    public string ctOverDistance { get; set; } = string.Empty;
    public decimal? ctSurcharge { get; set; }
    public decimal? ctPromotion { get; set; } 
    public decimal? totalPrice { get; set; }

    public string userId { get; set; } = string.Empty;
    public DateTime? createdAt { get; set; }

    // Khóa ngoại tới bảng "Danh sách lên ca"
    public string? shiftworkId { get; set; }
    [ForeignKey(nameof(shiftworkId))]
    public ShiftWork? ShiftWork { get; set; } //n-1
}
