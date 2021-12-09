using System.ComponentModel.DataAnnotations;

namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about a series.
    /// </summary>
    public class Series
    {
        public int SeriesId { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        public string? PublisherId { get; set; }

        public override string ToString() => DisplayName;
    }
}
