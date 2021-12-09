using System;

namespace EbookArchiver.JNovelClub.Models
{
    internal class JNovelClubSeries
    {
        //[JsonPropertyName("legacyId")]
        public string? LegacyId { get; set; }
        //[JsonPropertyName("type")]
        public string? type { get; set; }
        //[JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        //[JsonPropertyName("shortTitle")]
        public string? ShortTitle { get; set; }
        //[JsonPropertyName("originalTitle")]
        public string? OriginalTitle { get; set; }
        //[JsonPropertyName("slug")]
        public string? Slug { get; set; }
        //[JsonPropertyName("hidden")]
        public bool Hidden { get; set; }
        //[JsonPropertyName("created")]
        public DateTime? Created { get; set; }
        //[JsonPropertyName("description")]
        public string? Description { get; set; }
        //[JsonPropertyName("shortDescription")]
        public string? ShortDescription { get; set; }
        //[JsonPropertyName("tags")]
        //public string[] Tags { get; set; } = Array.Empty<string>();
        //[JsonPropertyName("cover")]
        public JNovelClubCover? Cover { get; set; }
        //[JsonPropertyName("following")]
        //public bool Following { get; set; }
        //[JsonPropertyName("catchup")]
        //public bool Catchup { get; set; }
    }
}
