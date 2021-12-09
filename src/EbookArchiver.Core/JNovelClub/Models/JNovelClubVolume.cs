using System;

namespace EbookArchiver.JNovelClub.Models
{
    internal class JNovelClubVolume
    {
        //[JsonPropertyName("legacyId")]
        public string? LegacyId { get; set; }
        //[JsonPropertyName("title")]
        public string Title { get; set; } = null!;
        //[JsonPropertyName("originalTitle")]
        public string? OriginalTitle { get; set; }
        //[JsonPropertyName("slug")]
        public string? Slug { get; set; }
        //[JsonPropertyName("number")]
        public int? Number { get; set; }
        //[JsonPropertyName("originalPublisher")]
        public string? OriginalPublisher { get; set; }
        //[JsonPropertyName("label")]
        public string? Label { get; set; }
        //[JsonPropertyName("creators")]
        public JNovelClubCreator[] Creators { get; set; } = Array.Empty<JNovelClubCreator>();
        //[JsonPropertyName("hidden")]
        public bool Hidden { get; set; }
        //[JsonPropertyName("forumTopicId")]
        public int? ForumTopicId { get; set; }
        //[JsonPropertyName("created")]
        public DateTime? Created { get; set; }
        //[JsonPropertyName("publishing")]
        public DateTime? Publishing { get; set; }
        //[JsonPropertyName("description")]
        public string? Description { get; set; }
        //[JsonPropertyName("shortDescription")]
        public string? ShortDescription { get; set; }
        //[JsonPropertyName("cover")]
        public JNovelClubCover? Cover { get; set; }
        //[JsonPropertyName("owned")]
        public bool Owned { get; set; }
    }
}
