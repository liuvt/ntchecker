using System.Text.Json.Serialization;

namespace TaxiNT.Data.Models
{
    public class VideoMetaSeo
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public int Duration { get; set; } = 0;

        [JsonPropertyName("tags")]
        public string[]? Tags { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("uploader")]
        public string? Uploader { get; set; }

        [JsonPropertyName("view_count")]
        public int View_Count { get; set; } = 0;

        [JsonPropertyName("publication_date")]
        public DateTime Publication_Date { get; set; } = DateTime.MinValue;
        [JsonPropertyName("locationVideoShow")]
        public string locationVideoShow { get; set; } = string.Empty;

    }
}
