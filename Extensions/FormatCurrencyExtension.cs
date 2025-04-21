using System.Globalization;

namespace ntchecker.Extensions;
public static class FormatCurrencyExtension
{
    // Convert string to string VND
    private static readonly string zero = "0";
    private static readonly CultureInfo viCulture = new CultureInfo("vi-VN");
    public static string ltvVNDCurrency(this object input)
    {
        try
        {
            if (input == null) return zero;

            if (input is decimal dec)
            {
                return dec.ToString("N0", viCulture);
            }

            if (input is string str)
            {
                // Định dạng lại chuỗi vào
                var clean = str.Replace(",", "").Replace(".", "").Trim();

                if (decimal.TryParse(clean, out decimal parsed))
                {
                    return parsed.ToString("N0", viCulture);
                }
            }

            return zero;
        }
        catch
        {
            return zero;
        }
    }
}
