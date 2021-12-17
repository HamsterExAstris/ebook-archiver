namespace EbookArchiver.KindleLibraryGatherer
{
    internal class LibraryResult
    {
        public LibraryItem[] ItemsList { get; set; } = Array.Empty<LibraryItem>();

        public string? PaginationToken { get; set; }
    }
}
