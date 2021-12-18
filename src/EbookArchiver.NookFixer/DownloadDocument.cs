using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookArchiver.NookFixer
{
    [Table("download_doc")]
    internal class DownloadDocument
    {
        [Column("ean")]
        [Key]
        public string Ean { get; set; } = null!;

        [Column("license")]
        public string License { get; set; } = string.Empty;

        [Column("savedFileName")]
        public string SavedFileName { get; set; } = null!;
    }
}
