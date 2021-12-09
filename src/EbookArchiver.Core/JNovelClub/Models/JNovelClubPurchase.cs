using System;
using System.Text.Json.Serialization;

namespace EbookArchiver.JNovelClub.Models
{
    internal class JNovelClubPurchase
    {
        //[JsonPropertyName("legacyId")]
        public string? LegacyId { get; set; }
        //[JsonPropertyName("volume")]
        public JNovelClubVolume Volume { get; set; } = null!;
        [JsonPropertyName("serie")]
        public JNovelClubSeries? Series { get; set; }
        //[JsonPropertyName("purchased")]
        public DateTime purchased { get; set; }
        //[JsonPropertyName("downloads")]
        public JNovelClubDownload[] Downloads { get; set; } = Array.Empty<JNovelClubDownload>();
        //[JsonPropertyName("status")]
        public string? Status { get; set; }
        //[JsonPropertyName("lastDownload")]
        public DateTime? LastDownload { get; set; }
        //[JsonPropertyName("lastUpdated")]
        public object? LastUpdated { get; set; }
    }
}
