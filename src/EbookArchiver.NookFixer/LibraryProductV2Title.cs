using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EbookArchiver.NookFixer
{
    [Table("LibraryProductV2Titles")]
    [Keyless]
    internal class LibraryProductV2Title
    {
        [Column("lpv2_ean")]
        public string Ean { get; set; } = null!;

        [Column("title")]
        public string Title { get; set; } = null!;
    }
}
