using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.Entities;

/// <summary>
/// Response DTO cho API GetFullSalary - trả về đầy đủ Salary + SalaryDetails
/// </summary>
public class SalaryFullResponseDto
{
    /// <summary>
    /// Thông tin tổng hợp lương (bản ghi chính)
    /// </summary>
    public Salary Salary { get; set; } = new();

    /// <summary>
    /// Danh sách chi tiết lương theo ngày (daily breakdown)
    /// </summary>
    public List<SalaryDetails> Details { get; set; } = new();
}

/// <summary>
/// Response DTO cho API GetFullSalary - trả về đầy đủ Salary + SalaryDetails + DeductDetails
/// </summary>

public class SalaryResponseDto
{
    public SalaryResponse Salary { get; set; } = new();

    public List<SalaryDetails> Details { get; set; } = new();

    public List<SalaryDeductDetailResponse> DeductDetails { get; set; } = new();
}

public class SalaryResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string userId { get; set; } = string.Empty; //Mã tài xế
    public decimal? revenue { get; set; } //Doanh thu
    public int tripsTotal { get; set; } //Số cuốc
    public string salaryType { get; set; } = string.Empty; // Loại hình kinh doanh LƯƠNG NGÀY - LƯƠNG THÁNG
    public int businessDays { get; set; } //Số ngày KD
    public decimal? salaryBase { get; set; } //Sau mức ăn chia || Lương cơ bản
    public decimal? deductTotal { get; set; } //Tổng trừ
    public decimal? salaryNet { get; set; } //Lương thực nhận
    public string? noteDeductOrder { get; set; } = string.Empty; //14. Ghi chú trừ khác
    public string? salaryDate { get; set; } = string.Empty; // Ví dụ: 05/2026
    public string? area { get; set; } = string.Empty; // Khu vực
    public DateTime createdAt { get; set; } = DateTime.Now;
}

public class SalaryDeductDetailResponse
{
    public string Id { get; set; } = string.Empty;

    public string SalaryId { get; set; } = string.Empty;

    public int DeductCategoryId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
}