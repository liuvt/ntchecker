using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using TaxiNT.Extensions;
using TaxiNT.Services.Interfaces;
using System.Text.Json;
using TaxiNT.Libraries.Models.Webhooks;

namespace TaxiNT.Services
{
    public class ZaloService : IZaloService
    {
        #region Constructor 
        //For Connection to Spread
        private SheetsService sheetsService;
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string CredentialGGSheetService = "ggsheetaccount.json";
        private readonly string AppName = "Zalo OA";
        private readonly string SpreadSheetId = "1MZBZHRlMLnU-q5_9_kY9jzb6FVyufARTONWwQ_omruk";

        // For Sheet
        private readonly string sheetuser_send_text = "user_send_text";
        private readonly string sheetuser_send_location = "user_send_location";
        private readonly string sheetuser_text_location = "user_text_location";
        


        public ZaloService()
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
        public async Task<bool> AppendUserMessageTextAsync(UserMessageText model)
        {
            try
            {
                var now = DateTime.Now;
                var values = new List<IList<object>>
                {
                    new List<object>
                    {
                        model.MsgId,
                        model.Text,
                        now.ToString("dd/MM/yyyy HH:mm:ss") 
                    }
                };

                var valueRange = new ValueRange { Values = values };

                // Đặt range linh hoạt
                string range = $"{sheetuser_send_text}!A2:C";
                await sheetsService.ltvAppendSheetValuesAsync(SpreadSheetId, range, valueRange);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [Connection GoogleSheet Error] {ex.Message}");
            }
        }
        #endregion

        #region User Send Location
        //Ghi log vào Google Sheet
        public async Task<bool> AppendUserSendLocationAsync(UserMessageLocation model)
        {
            try
            {
                var now = DateTime.Now;

                var values = new List<IList<object>>
                {
                    new List<object>
                    {
                        model.MsgId,
                        model.Attachments[0].Type,
                        model.Attachments[0].Payload.Coordinates.Latitude,
                        model.Attachments[0].Payload.Coordinates.Longitude,
                        now.ToString("dd/MM/yyyy HH:mm:ss")
                    }
                };

                var valueRange = new ValueRange
                {
                    Values = values
                };

                string range = $"{sheetuser_send_location}!A2:E";
                await sheetsService.ltvAppendSheetValuesAsync(SpreadSheetId, range, valueRange);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [Connection GoogleSheet Error] {ex.Message}");
            }
        }
        #endregion

        #region User Send Text - Location
        //Ghi log vào Google Sheet
        public async Task<bool> AppendUserTextLocationAsync(ZaloWebhookEvent model)
        {
            try
            {
                if (model == null)
                    throw new Exception("Không có dữ liệu tin nhắn hợp lệ.");

                UserMessageLocation? locationMsg = null;
                UserMessageText? textMsg = null;

                switch (model.EventName)
                {
                    case "user_send_location":
                        locationMsg = model.Message?.Deserialize<UserMessageLocation>();
                        break;

                    case "user_send_text":
                        textMsg = model.Message?.Deserialize<UserMessageText>();
                        break;

                    default:
                        Console.WriteLine("Sự kiện chưa xử lý: {0}", model.EventName);
                        break;
                }

                if (locationMsg == null && textMsg == null)
                    throw new Exception("Không có dữ liệu tin nhắn hợp lệ.");

                //Format validate
                var now = DateTime.Now;

                var msgId = textMsg?.MsgId ?? locationMsg?.MsgId ?? string.Empty;
                var text = textMsg?.Text ?? locationMsg?.Text ?? string.Empty;
                var type = locationMsg?.Attachments?.FirstOrDefault()?.Type ?? string.Empty;
                var lat = locationMsg?.Attachments?.FirstOrDefault()?.Payload?.Coordinates?.Latitude ?? string.Empty;
                var lng = locationMsg?.Attachments?.FirstOrDefault()?.Payload?.Coordinates?.Longitude ?? string.Empty;

                // Set data for Google Sheets
                var values = new List<IList<object>>
                {
                    new List<object>
                    {
                        model.Sender.Id,   // User ID
                        model.EventName,   // Event Name
                        model.Timestamp,   // Thời gian từ Zalo
                        msgId,
                        text,
                        type,
                        lat,
                        lng,
                        now.ToString("dd/MM/yyyy HH:mm:ss")
                    }
                };

                var valueRange = new ValueRange
                {
                    Values = values
                };

                string range = $"{sheetuser_text_location}!A2:E";
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
