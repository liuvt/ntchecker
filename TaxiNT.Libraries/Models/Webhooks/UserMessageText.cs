using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaxiNT.Libraries.Models.Webhooks
{
    public class UserMessageText
    {
        [JsonPropertyName("msg_id")]
        public string MsgId { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
