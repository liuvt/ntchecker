using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiNT.Libraries.Models;

[Table("Salaries")]
public class Salary
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string userId { get; set; } = string.Empty; //Mã tài xế
    [Column(TypeName = "decimal(18,2)")]
    public decimal? revenue { get; set; } //Doanh thu
    public int tripsTotal { get; set; } //Số cuốc
    public string salaryType { get; set; } = string.Empty; // Loại hình kinh doanh LƯƠNG NGÀY - LƯƠNG THÁNG
    public int businessDays { get; set; } //Số ngày KD

    [Column(TypeName = "decimal(18,2)")]
    public decimal? salaryBase { get; set; } //Sau mức ăn chia || Lương cơ bản

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductTotal { get; set; } //Tổng trừ

    [Column(TypeName = "decimal(18,2)")]
    public decimal? salaryNet { get; set; } //Lương thực nhận

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForDeposit { get; set; } //1. Trừ ký quỹ

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForSalaryAdvance { get; set; } //2. Trừ lương ứng

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForNegativeSalary { get; set; } //3. Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương (công ty)

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForViolationReport { get; set; } //4. Trừ vi phạm biên bản  

    [Column(TypeName = "decimal(18,2)")]
    public decimal? no_sua_chua { get; set; } //5. Nợ sửa chữa

    [Column(TypeName = "decimal(18,2)")]
    public decimal? haomon_voxe { get; set; } //6. Hao mòn vỏ xe

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForCharging { get; set; } //7. Sat pin

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForChargingPenalty { get; set; } //8. phạt sạt: Charging Penalty

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForTollPayment { get; set; } //9. Trừ tiền qua trạm : Deduction for Toll Payment

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForSocialInsurance { get; set; } //10. Trừ BHXH

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForNegativeSalaryPartner { get; set; } //11. Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương (Thương quyền)

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForPIT { get; set; } //12. Trừ TNCN - Personal Income Tax Deduction 

    [Column(TypeName = "decimal(18,2)")]
    public decimal? deductForOrder { get; set; } //13. Trừ khác

    public string? noteDeductOrder { get; set; } = string.Empty; //14. Ghi chú trừ khác

    public string? salaryDate { get; set; } = string.Empty; // Ví dụ: 05/2026
    public string? area { get; set; } = string.Empty; // Khu vực

    public DateTime createdAt { get; set; } = DateTime.Now;

    // Liên kết 1-n với chi tiết lương
    public List<SalaryDetails>? Details { get; set; }
    public List<SalaryDeductDetail>? DeductDetails { get; set; } = new();
}

[Table("DeductCategories")]
[Index(nameof(Code), IsUnique = true)]
public class DeductCategory
{
    [Key]
    public int Id { get; set; }

    public int SortOrder { get; set; } // STT
    // Ví dụ: phiThuongHieu, kyQuyLaiXe, tienSacPin
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    // Ví dụ: phí thương hiệu, ký quỹ lái xe

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<SalaryDeductDetail> SalaryDeductDetails { get; set; } = new();
}

[Table("SalaryDeductDetails")]
public class SalaryDeductDetail
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string SalaryId { get; set; } = string.Empty;

    public int DeductCategoryId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } = 0;

    public string? Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey(nameof(SalaryId))]
    public Salary? Salary { get; set; }

    [ForeignKey(nameof(DeductCategoryId))]
    public DeductCategory? DeductCategory { get; set; }
}


public class DeductionItem
{
    public string Name { get; set; } = string.Empty;
    public string NameAlias { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
}

[Table("SalaryDetails")]
public class SalaryDetails
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string userId { get; set; } = string.Empty; // Mã tài xế

    [Column(TypeName = "decimal(18,2)")]
    public decimal? revenue { get; set; } // Doanh thu

    [Column(TypeName = "decimal(18,2)")]
    public decimal? revenueAC { get; set; } // Doanh thu AC giảm 5% GTGT

    public string type { get; set; } = string.Empty; // Loại hình kinh doanh LƯƠNG NGÀY - LƯƠNG THÁNG

    [Column(TypeName = "decimal(18,2)")]
    public decimal? salaryBase { get; set; } // Sau mức ăn chia

    public string daterevenues { get; set; } = string.Empty; // Ngày doanh thu chi tiết

    // Foreign key đến bảng Salaries
    public string? salaryId { get; set; }

    [ForeignKey(nameof(salaryId))]
    public Salary? Salary { get; set; }
    public string? salaryDate { get; set; } = string.Empty; //Tháng/năm
    public string? area { get; set; } = string.Empty; // Khu vực

    public DateTime createdAt { get; set; }
}


