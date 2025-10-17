using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiNT.Libraries.Models;

public class TripGroup
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [NotMapped]// Không lưu vào SQL
    public List<Trip> TripDetails { get; set; } = new();

    [NotMapped]// Không lưu vào SQL
    public int RecordTotal => TripDetails.Count; 
}

[Table("Trips")]
public class Trip
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string NumberCar { get; set; } = string.Empty;
    public DateTime? tpTimeStart { get; set; }
    public DateTime? tpTimeEnd { get; set; }
    public double? tpDistance { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? tpPrice { get; set; }
    public string tpPickUp { get; set; } = string.Empty;
    public string tpDropOut { get; set; } = string.Empty;
    public string tpType { get; set; } = string.Empty;
    public string userId { get; set; } = string.Empty;
    public DateTime? createdAt { get; set; }

    // Khóa ngoại tới bảng "Danh sách lên ca"
    public string? shiftworkId { get; set; }
    [ForeignKey(nameof(shiftworkId))]
    public ShiftWork? ShiftWork { get; set; } //n-1
}
