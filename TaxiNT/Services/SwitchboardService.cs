using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Models.Webhooks;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services
{
    public class SwitchboardService : ISwitchboardService
    {
        #region Constructor 
        //For Connection to Spread
        private SheetsService sheetsService;
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string CredentialGGSheetService = "ggsheetaccount.json";
        private readonly string AppName = "Switchboard API";
        private readonly string SpreadSheetId = "1MZBZHRlMLnU-q5_9_kY9jzb6FVyufARTONWwQ_omruk";

        // For Sheet
        private readonly string sheetswitchboard_hook = "switchboard_hook";
        


        public SwitchboardService()
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

        #region User Message Text
        //Ghi log vào Google Sheet
        public async Task<bool> AppendCallLogAsync(CallLog model)
        {
            try
            {
                var values = new List<IList<object>>
                {
                    new List<object>
                    {
                        model.Id,
                        model.Action,
                        model.Source,
                        model.Destination,
                        model.UniqueId,
                        model.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")
                    }
                };

                var valueRange = new ValueRange { Values = values };

                // Đặt range linh hoạt
                string range = $"{sheetswitchboard_hook}!A2:F";
                await sheetsService.ltvAppendSheetValuesAsync(SpreadSheetId, range, valueRange);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [Connection GoogleSheet Error] {ex.Message}");
            }
        }
        #endregion

    }
}
