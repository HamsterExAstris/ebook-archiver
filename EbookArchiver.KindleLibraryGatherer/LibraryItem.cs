namespace EbookArchiver.KindleLibraryGatherer
{
    internal class LibraryItem
    {
        public string ASIN { get; set; } = null!;

        public string[] Authors { get; set; } = Array.Empty<string>();

        public string ProductUrl { get; set; } = null!;

        public string Title { get; set; } = null!;

        public bool MangaAsin { get; set; }

        public string ResourceType { get; set; } = null!;

        public decimal PercentageRead { get; set; }
    }
}
