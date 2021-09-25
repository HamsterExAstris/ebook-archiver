namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about a series.
    /// </summary>
    public class Series
    {
        public int SeriesId { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public override string ToString() => DisplayName;
    }
}
