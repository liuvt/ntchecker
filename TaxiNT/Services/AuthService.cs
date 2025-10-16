using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using TaxiNT.Libraries.Models.GGSheets;
using TaxiNT.Libraries.Entities;
using TaxiNT.Extensions;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services;

public class AuthService : IAuthService
{
    #region Constructor 
    //For Connection to Spread
    private SheetsService sheetsService;
    private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private readonly string CredentialGGSheetService = "ggsheetaccount.json";
    private readonly string AppName = "NTBL Taxi";
    private readonly string SpreadSheetId = "1i0ZV-0ZBUF0j5QWW1ag3boWHjFWM6XJ3Zn-NKriGeFo";

    // For Sheet
    private readonly string sheetTAI_KHOAN = "TAI_KHOAN";

    public AuthService()
    {
        //File xác thực google tài khoản
        GoogleCredential credential;
        using (var stream = new FileStream(CredentialGGSheetService, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
        }

        // Đăng ký service
        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = AppName,
        });
    }
    #endregion

    #region TAI_KHOAN
    // Lấy toàn bộ danh sách
    private async Task<List<GGSUser>> GetsGGSUser()
    {
        var dts = new List<GGSUser>();
        var range = $"{sheetTAI_KHOAN}!A2:J";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {

            dts.Add(new GGSUser
            {
                id = item.ltvGetValueString(0),
                msnv = item.ltvGetValueString(1),
                hoten_msnv = item.ltvGetValueString(2),
                mat_khau = item.ltvGetValueString(3),
                so_tai = item.ltvGetValueString(4),
                khu_vuc = item.ltvGetValueString(5),
                trangthai_lamviec = item.ltvGetValueString(6),
                chon_ngayxem_phieuchecker_tx = item.ltvGetValueString(7),
                email = item.ltvGetValueString(8),
                loai_banglai = item.ltvGetValueString(9),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã msnv 
    public async Task<GGSUser> GetGGSUser(string msnv)
    {
        var dts = await GetsGGSUser() ?? new List<GGSUser>();
        return dts.FirstOrDefault(e => e.msnv.Equals(msnv, StringComparison.OrdinalIgnoreCase)) ?? new GGSUser();
    }
    #endregion

    #region LOGIN
    public async Task<GGSUser> GGSLogin(GGSUserLoginDto model)
    {
        var listUsers = await GetsGGSUser() ?? new List<GGSUser>();

        var user = listUsers.FirstOrDefault(u =>
            u.msnv.Equals(model.Username, StringComparison.OrdinalIgnoreCase));

        if (user == null || user.mat_khau != model.Password)
        {
            throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        return user;
    }

    #endregion

}
