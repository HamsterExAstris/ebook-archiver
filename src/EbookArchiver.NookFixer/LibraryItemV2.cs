using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookArchiver.NookFixer
{
    [Table("LibraryItemV2")]
    internal class LibraryItemV2
    {
        [Column("ean")]
        [Key]
        public string Ean { get; set; } = null!;

        [Column("isSampleEan")]
        public bool IsSampleEan { get; set; }

        [ForeignKey(nameof(Ean))]
        public LibraryProductV2 Product { get; set; } = null!;
    }
}
