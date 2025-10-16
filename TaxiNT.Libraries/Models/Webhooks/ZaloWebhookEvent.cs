using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaxiNT.Libraries.Models.Webhooks
{
    public class ZaloWebhookEvent
    {
        [JsonPropertyName("app_id")]
        public string AppId { get; set; } = string.Empty;

        [JsonPropertyName("user_id_by_app")]
        public string UserIdByApp { get; set; } = string.Empty;

        [JsonPropertyName("event_name")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonPropertyName("sender")]
        public Sender Sender { get; set; } = new();

        [JsonPropertyName("recipient")]
        public Recipient Recipient { get; set; } = new();

        [JsonPropertyName("message")]
        public JsonElement? Message { get; set; }
    }

    public class Sender
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class Recipient
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
