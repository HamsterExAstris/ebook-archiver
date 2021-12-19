using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookArchiver.NookFixer
{
    [Table("LibraryProductV2")]
    internal class LibraryProductV2
    {
        [Column("ean")]
        [Key]
        public string Ean { get; set; } = null!;

        [Column("isbn")]
        public string? Isbn { get; set; }

        [Column("seriesTitle")]
        public string? SeriesTitle { get; set; }

        [Column("seriesNumber")]
        public string? SeriesNumber { get; set; }

        [Column("publisher")]
        public string? Publisher { get; set; }

        public LibraryItemV2 Item { get; set; } = null!;
    }
}
