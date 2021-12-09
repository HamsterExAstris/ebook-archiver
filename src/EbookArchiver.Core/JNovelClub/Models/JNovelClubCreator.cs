namespace EbookArchiver.JNovelClub.Models
{
    internal class JNovelClubCreator
    {
        //[JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        //[JsonPropertyName("role")]
        public string? Role { get; set; }
        //[JsonPropertyName("originalName")]
        public string? OriginalName { get; set; }
    }
}
