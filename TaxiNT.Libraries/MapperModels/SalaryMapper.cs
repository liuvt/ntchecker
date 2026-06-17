
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.MapperModels;

public static class SalaryMapperGetByUser
{
    public static SalaryResponse ToSalaryResponse(Salary salary)
    {
        if (salary == null)
            return new SalaryResponse();

        return new SalaryResponse
        {
            Id = salary.Id,
            userId = salary.userId,
            revenue = salary.revenue,
            tripsTotal = salary.tripsTotal,
            salaryType = salary.salaryType,
            businessDays = salary.businessDays,
            salaryBase = salary.salaryBase,
            deductTotal = salary.deductTotal,
            salaryNet = salary.salaryNet,
            noteDeductOrder = salary.noteDeductOrder,
            salaryDate = salary.salaryDate,
            area = salary.area,
            createdAt = salary.createdAt
        };
    }

    public static SalaryResponseDto ToSalaryResponseDto(
        Salary salary,
        List<SalaryDetails> details,
        List<SalaryDeductDetailResponse> deductDetails)
    {
        return new SalaryResponseDto
        {
            Salary = ToSalaryResponse(salary),
            Details = details ?? new List<SalaryDetails>(),
            DeductDetails = deductDetails ?? new List<SalaryDeductDetailResponse>()
        };
    }
}

public static class SalaryMapperUpsert
{
    public static void MapUpdateSalary(
        Salary target,
        SalaryUpsertDto source,
        string userId,
        string salaryDate)
    {
        target.userId = userId;
        target.revenue = source.revenue;
        target.tripsTotal = source.tripsTotal;
        target.salaryType = source.salaryType;
        target.businessDays = source.businessDays;
        target.salaryBase = source.salaryBase;
        target.deductTotal = source.deductTotal;
        target.salaryNet = source.salaryNet;
        target.noteDeductOrder = source.noteDeductOrder;
        target.salaryDate = salaryDate;
        target.area = source.area;
    }

    public static Salary MapCreateSalary(
        SalaryUpsertDto source,
        string userId,
        string salaryDate,
        DateTime now)
    {
        return new Salary
        {
            Id = Guid.NewGuid().ToString(),
            userId = userId,
            revenue = source.revenue,
            tripsTotal = source.tripsTotal,
            salaryType = source.salaryType,
            businessDays = source.businessDays,
            salaryBase = source.salaryBase,
            deductTotal = source.deductTotal,
            salaryNet = source.salaryNet,
            noteDeductOrder = source.noteDeductOrder,
            salaryDate = salaryDate,
            area = source.area,
            createdAt = now
        };
    }

    public static List<SalaryDetails> MapSalaryDetails(
        List<SalaryDetailUpsertDto>? detailsInput,
        Salary targetSalary,
        DateTime now)
    {
        if (detailsInput == null || detailsInput.Count == 0)
            return new List<SalaryDetails>();

        return detailsInput.Select(detail => new SalaryDetails
        {
            Id = Guid.NewGuid().ToString(),

            userId = targetSalary.userId,
            salaryId = targetSalary.Id,
            salaryDate = targetSalary.salaryDate,
            area = targetSalary.area,
            createdAt = now,

            revenue = detail.revenue,
            revenueAC = detail.revenueAC,
            type = detail.type,
            salaryBase = detail.salaryBase,
            daterevenues = detail.daterevenues
        }).ToList();
    }

    public static List<SalaryDeductDetail> MapSalaryDeductDetails(
        List<SalaryDeductDetailUpsertDto>? deductDetailsInput,
        List<DeductCategory> deductCategories,
        Salary targetSalary,
        DateTime now)
    {
        if (deductDetailsInput == null || deductDetailsInput.Count == 0)
            return new List<SalaryDeductDetail>();

        var result = new List<SalaryDeductDetail>();

        foreach (var deduct in deductDetailsInput)
        {
            var code = deduct.Code?.Trim() ?? "";

            var category = deductCategories.First(x =>
                x.Code.Trim().Equals(code, StringComparison.OrdinalIgnoreCase));

            result.Add(new SalaryDeductDetail
            {
                Id = Guid.NewGuid().ToString(),
                SalaryId = targetSalary.Id,
                DeductCategoryId = category.Id,
                Amount = deduct.Amount,
                Note = deduct.Note,
                CreatedAt = now,

                Salary = null,
                DeductCategory = null
            });
        }

        return result;
    }

    public static decimal CalculateDeductTotal(List<SalaryDeductDetail> deductDetails)
    {
        return deductDetails?.Sum(x => x.Amount) ?? 0;
    }

    public static void ApplySalaryNet(Salary salary, decimal deductTotal)
    {
        salary.deductTotal = deductTotal;
        salary.salaryNet = (salary.salaryBase ?? 0) - deductTotal;
    }
}