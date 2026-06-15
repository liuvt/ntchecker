namespace TaxiNT.Libraries.Entities;

public class SalaryDeleteRequest
{
    public string userId { get; set; } = string.Empty;

    public string salaryDate { get; set; } = string.Empty;
}

public class SalaryDeleteByAreaRequest
{
    public string area { get; set; } = string.Empty;

    public string salaryDate { get; set; } = string.Empty;
}

public class SalaryDeleteResult
{
    public bool Success { get; set; }

    public string SalaryId { get; set; } = string.Empty;

    public string userId { get; set; } = string.Empty;

    public string salaryDate { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = new();
}