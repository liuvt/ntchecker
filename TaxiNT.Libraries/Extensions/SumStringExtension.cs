namespace TaxiNT.Libraries.Extensions;
// Hàm này xữ lý kiểu list<string> trả về tổng các giá trị của một cột trong list và trả về string
public static class SumStringExtension
{
    // Truyền thẳng tên cột cần cộng
    public static string ltvSumFieldValues<T>(this List<T> value, string namefields)
    {
        // Sum the values of all contracts, assuming each object has a 'Price' property.
        var totalList = value.Sum(e =>
        {
            // Lấy tên cột
            var fieldsNameToSum = e.GetType().GetProperty(namefields)?.GetValue(e)?.ToString() ?? "0";

            // Sau khi định dang tiền là hàng nghìn (.), thì decimal hiểu đó là phần thập phân (,) dẫn đến mất các số 0 phía sau dấu (.)
            // Do đó cần thay đổi dâu (.) thành dấu (,) để decimal hiểu được đâu là phần thập phân, đâu là hàng nghìn 
            // Trim(): Loại bỏ khoảng trắng thừa
            fieldsNameToSum = fieldsNameToSum?.Replace(".", "").Replace(",", "").Trim();
            // Try to parse the price string to decimal
            if (decimal.TryParse(fieldsNameToSum, out decimal data))
            {
                return data; // If parsing is successful, return the price
            }
            else
            {
                return 0; // If parsing fails or Price is invalid, return 0
            }
        });

        // Trả về tiền tệ, định dạng theo hàm mở rộng
        return totalList.ToString();
    }

    // Gọi thông qua selector với delegate
    public static string ltvSumFieldValues<T>(this List<T> list, Func<T, string> selector)
    {
        if (list == null || selector == null) return "0";

        decimal total = 0;

        foreach (var item in list)
        {
            var value = selector(item) ?? "0";

            // Loại bỏ định dạng tiền tệ và khoảng trắng
            var normalized = value.Replace(".", "").Replace(",", "").Trim();

            if (decimal.TryParse(normalized, out decimal number))
            {
                total += number;
            }
        }

        return total.ToString();
    }
}
