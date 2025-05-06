using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;

namespace ntchecker.Extensions;
public static class GGSheetExtension
{
    /// <summary>
    /// GetSheetValuesAsync
    /// UpdateSheetValuesAsync
    /// ClearSheetValuesAsync
    /// RemoveRowBySheetNameAsync
    /// AppendSheetValuesAsync
    /// GetValue
    /// </summary>

    /// <summary>
    /// Lấy toàn bộ dữ liệu trong range từ Google Sheets
    /// </summary>
    public static async Task<IList<IList<object>>> ltvGetSheetValuesAsync(this SheetsService service, string spreadsheetId, string range)
    {
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync();
        return response.Values ?? new List<IList<object>>();
    }

    /// <summary>
    /// Cập nhật dữ liệu vào vùng chỉ định trong Google Sheets
    /// </summary>
    public static async Task<IList<IList<object>>> UpdateSheetValuesAsync(this SheetsService service, string spreadsheetId, string range, ValueRange valueRange)
    {
        var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var response = await updateRequest.ExecuteAsync();
        return response.UpdatedData?.Values ?? new List<IList<object>>();
    }

    /// <summary>
    /// Xóa (clear) toàn bộ dữ liệu trong một vùng chỉ định
    /// </summary>
    public static async Task ClearSheetValuesAsync(this SheetsService service, string spreadsheetId, string range)
    {
        var clearRequest = service.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadsheetId, range);
        await clearRequest.ExecuteAsync();
    }

    /// <summary>
    /// Xóa 1 dòng khỏi Google Sheet theo tên sheet và index (0-based, bỏ qua dòng tiêu đề)
    /// </summary>
    public static async Task RemoveRowBySheetNameAsync(this SheetsService service, string spreadsheetId, string sheetName, int rowIndex)
    {
        // Không xóa dòng tiêu đề
        if (rowIndex <= 0) return;

        // Lấy thông tin các sheet trong bảng tính
        var spreadsheet = await service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();

        // Tìm sheet theo tên
        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == sheetName);
        if (sheet == null)
        {
            throw new Exception($"Sheet với tên {sheetName} không tồn tại.");
        }

        var sheetId = sheet.Properties.SheetId;

        // Tạo yêu cầu xóa dòng
        var deleteRequest = new Request
        {
            DeleteDimension = new DeleteDimensionRequest
            {
                Range = new DimensionRange
                {
                    SheetId = sheetId,
                    Dimension = "ROWS",
                    StartIndex = rowIndex,
                    EndIndex = rowIndex + 1
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = new[] { deleteRequest }
        };

        var request = service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId);
        await request.ExecuteAsync();
    }

    /// <summary>
    /// Thêm dữ liệu mới vào cuối bảng
    /// </summary>
    public static async Task AppendSheetValuesAsync(this SheetsService service, string spreadsheetId, string range, ValueRange valueRange)
    {
        var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
        await appendRequest.ExecuteAsync();
    }

    /// <summary>
    /// Trích xuất giá trị theo index, trả về chuỗi rỗng nếu không có giá trị
    /// </summary>
    public static string ltvGetValue(this IList<object> item, int index, string defaultValue = "") 
        => ((item.Count > index && item[index] != null)
            ? item[index].ToString().Trim()
            : defaultValue);

    /// <summary>
    /// Trích xuất giá trị theo index, trả về giá trị mặc định (0.0m) nếu không có giá trị hoặc không thể chuyển đổi.
    /// </summary>
    public static decimal ltvGetValueDecimal(this IList<object> item, int index, decimal defaultValue = 0.0m)
    {
        if (item.Count > index && item[index] != null)
        {
            // Kiểm tra và chuyển đổi giá trị thành decimal
            if (decimal.TryParse(item[index].ToString().Trim(), out decimal result))
            {
                return result;
            }
        }
        return defaultValue; // Trả về giá trị mặc định nếu không chuyển đổi được
    }
}
