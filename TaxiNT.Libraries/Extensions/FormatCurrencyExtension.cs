using System.Globalization;

namespace TaxiNT.Libraries.Extensions;
public static class FormatCurrencyExtension
{
    // Convert string to string VND
    private static readonly string zero = "0";
    // Culture info for Vietnamese currency formatting
    private static readonly CultureInfo viCulture = new CultureInfo("vi-VN");

    // Phương thức trả về giá trị string (hỗ trợ decimal? và object)
    public static string ltvVNDCurrency(this object? input)
    {
        try
        {
            if (input == null) return zero;

            if (input is decimal dec)
                return dec.ToString("N0", viCulture);

            if (input is decimal dec2)
                return dec2.ToString("N0", viCulture);

            if (input is string str)
            {
                var clean = str.Replace(",", "").Replace(".", "").Trim();
                if (decimal.TryParse(clean, out decimal parsed))
                    return parsed.ToString("N0", viCulture);
            }

            return zero;
        }
        catch
        {
            return zero;
        }
    }

    // Overload tiện lợi cho decimal?
    public static string ltvVNDCurrency(this decimal? input)
        => input.HasValue ? input.Value.ToString("N0", viCulture) : zero;

    // Phương thức trả về giá trị decimal
    public static decimal ltvVNDCurrencyToDecimal(this object input)
    {
        try
        {
            if (input == null)
                return 0;

            if (input is decimal dec)
                return dec;

            if (input is string str)
            {
                // Xoá đơn vị tiền tệ nếu có
                str = str.Replace("VND", "", StringComparison.OrdinalIgnoreCase)
                         .Replace("₫", "")
                         .Replace(".", "")
                         .Replace(",", "")
                         .Trim();

                if (decimal.TryParse(str, out decimal parsed))
                    return parsed;
            }

            if (input is IConvertible)
                return Convert.ToDecimal(input);

            return 0;
        }
        catch
        {
            return 0;
        }
    }
}
