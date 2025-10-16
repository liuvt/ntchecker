using System.Text.Json.Serialization;

namespace TaxiNT.Libraries.Models.Webhooks
{
    public class UserMessageLocation
    {
        [JsonPropertyName("msg_id")]
        public string MsgId { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("attachments")]
        public List<Attachment> Attachments { get; set; } = new();
    }

    public class Attachment
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("payload")]
        public Payload Payload { get; set; } = new();
    }

    public class Payload
    {
        [JsonPropertyName("coordinates")]
        public Coordinates Coordinates { get; set; } = new();
    }

    public class Coordinates
    {
        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = string.Empty;

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = string.Empty;
    }
}
