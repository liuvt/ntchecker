
namespace TaxiNT.Libraries.Extensions
{
    public static class Helper
    {
        // Chuyển đổi view count thành định dạng ngắn gọn
        public static string ltvFormatViewCount(this int views)
        {
            if (views >= 1_000_000)
                return $"{views / 1_000_000.0:F1}M".Replace(".0", "");
            else if (views >= 1_000)
                return $"{views / 1_000.0:F1}K".Replace(".0", "");
            else
                return views.ToString();
        }

    }
}
