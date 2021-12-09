namespace EbookArchiver.JNovelClub.Models
{
    internal class Pagination
    {
        //[JsonPropertyName("limit")]
        public int Limit { get; set; }
        //[JsonPropertyName("skip")]
        public int Skip { get; set; }
        //[JsonPropertyName("lastPage")]
        public bool LastPage { get; set; }
    }
}
