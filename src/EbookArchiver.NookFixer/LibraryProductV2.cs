using System.ComponentModel.DataAnnotations.Schema;

namespace EbookArchiver.NookFixer
{
    internal class LibraryProductV2
    {
        [Column("ean")]
        public string Ean { get; set; } = null!;
    }
}
