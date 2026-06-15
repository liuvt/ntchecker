using System.Globalization;

namespace TaxiNT.Libraries.Extensions;

public static class FormatMonth
{

    //Expected: yyyy-MM, MM-yyyy hoặc MM/yyyy => Output: MM/yyyy
    public static string? FormatSalaryMonth(string? date)
    {
        if (string.IsNullOrWhiteSpace(date))
            return null;

        var value = date.Trim();

        if (value.Length > 7)
            value = value[^7..];

        var formats = new[]
        {
        "yyyy-MM",
        "MM-yyyy",
        "MM/yyyy"
    };

        if (DateTime.TryParseExact(
            value,
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate))
        {
            return parsedDate.ToString("MM/yyyy");
        }

        return null;
    }
}
