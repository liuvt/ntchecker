using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ntchecker.Data.Models.GGSheetModels;
using ntchecker.Extensions;
using ntchecker.Services.Interfaces;

namespace ntchecker.Components.Pages.Bases;
public class OrderBase : ComponentBase
{
    [Parameter]
    public string userId { get; set; }
    [Inject]
    protected IJSRuntime Js { get; set; }
    [Inject]
    protected NavigationManager nav { get; set; }
    [Inject]
    protected IOrderService orderService { get; set; }

    protected Revenue revenue { get; set; } = new Revenue();
    protected Contract _contract { get; set; } = new Contract();
    protected Timepiece _timepiece { get; set; } = new Timepiece();

    protected string totalAmount { get; set; } = string.Empty; // Tổng hợp đồng và cuốc lẻ
    protected bool isLoaded = false;
    protected string dateBill { get; set; } = string.Empty;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                userId = Uri.UnescapeDataString(userId); // Chuyển chuỗi URL sang Tên - Mã Nhân Viên

                // Thông tin Doanh thu và QR chuyển khoản
                revenue = await GetRevenue(userId);
                Console.WriteLine("revenue done!");

                if (!string.IsNullOrEmpty(revenue.createdAt)) dateBill = "Ngày: " + revenue.createdAt;

                if (revenue == null || revenue.typeCar == "Khoán điện") isLoaded = false; // Không tìm thấy ID hoặc xe khoán điện không xem được phiếu
                else // Xe điện được xem phiếu
                {
                    // Thống tin sheet Hợp đồng
                    _contract = await GetContract(userId);
                    Console.WriteLine("_contract done!");
                    // Thông tin sheet Đồng hồ
                    _timepiece = await _GetTimepiece(userId);
                    Console.WriteLine("_timepiece done!");

                    totalAmount = new List<string>
                    {
                        _contract.TotalPrice, _timepiece.TotalPrice,
                    }.ltvSumFieldValues(e => e).ltvVNDCurrency();

                    isLoaded = true; //Xe lên ca
                }
                StateHasChanged(); // Cập nhật lại giao diện
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                isLoaded = false;
            }
        }
    }

    private async Task<Revenue> GetRevenue(string _userId)
    {
        var rs = await orderService.GetRevenue(_userId);
        StateHasChanged();
        return rs;
    }

    private async Task<Contract> GetContract(string _userId)
    {
        var rs = await orderService.GetContract(_userId);
        StateHasChanged();
        return rs;
    }

    private async Task<Timepiece> _GetTimepiece(string _userId)
    {
        var rs = await orderService.GetTimepiece(_userId);
        StateHasChanged();
        return rs;
    }

    // Chuyển đổi bảng điều khiển sang chế độ máy tính
    protected bool isDesktopView { get; set; } = false;
    protected async Task forceDesktopView()
    {
        isDesktopView = !isDesktopView;
        await Js.InvokeVoidAsync("forceDesktopView");
        StateHasChanged(); // Cập nhật lại giao diện
    }
}
