using System.ComponentModel.DataAnnotations.Schema;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.Entities;

/// <summary> 
public class SalaryUpsertRequestV1
{
    /// <summary>
    /// Thông tin tổng hợp lương (bản ghi chính)
    /// </summary>
    public Salary Salary { get; set; } = new();

    /// <summary>
    /// Danh sách chi tiết lương theo ngày (daily breakdown)
    /// </summary>
    public List<SalaryDetails>? Details { get; set; } = new();
    /// <summary>
    /// Danh sách chi tiết Khoản trừ lương (deduction breakdown)
    /// </summary>
    public List<SalaryDeductDetailUpsertDto> DeductDetails { get; set; } = new();
}

// Request
public class SalaryUpsertRequest
{
    public SalaryUpsertDto Salary { get; set; } = new();

    public List<SalaryDetailUpsertDto> Details { get; set; } = new();

    public List<SalaryDeductDetailUpsertDto> DeductDetails { get; set; } = new();
}

//Salary
public class SalaryUpsertDto
{
    public string userId { get; set; } = string.Empty;

    public decimal? revenue { get; set; }

    public int tripsTotal { get; set; }

    public string salaryType { get; set; } = string.Empty;

    public int businessDays { get; set; }

    public decimal? salaryBase { get; set; }

    public string? noteDeductOrder { get; set; } = string.Empty;

    public string? salaryDate { get; set; } = string.Empty;

    public string? area { get; set; } = string.Empty;
}

//Detail
public class SalaryDetailUpsertDto
{
    public string userId { get; set; } = string.Empty;

    public decimal? revenue { get; set; }

    public decimal? revenueAC { get; set; }

    public string? type { get; set; } = string.Empty;

    public decimal? salaryBase { get; set; }

    public string? daterevenues { get; set; } = string.Empty;

    public string? salaryDate { get; set; } = string.Empty;

    public string? area { get; set; } = string.Empty;
}

//Deduct
public class SalaryDeductDetailUpsertDto
{
    public string Code { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } = 0;

    public string? Note { get; set; } = string.Empty;
}

// Trả trạng thái Upsert
public class SalaryUpsertResult
{
    public bool Success { get; set; }

    public string? SalaryId { get; set; }

    public string Message { get; set; } = string.Empty;

    public int DetailsCount { get; set; }

    public int DeductDetailsCount { get; set; }

    public List<string> Errors { get; set; } = new();
}