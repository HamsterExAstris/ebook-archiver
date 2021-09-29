using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about a specific version of an e-book.
    /// </summary>
    public class Ebook
    {
        public int EbookId { get; set; }

        public Account? Account { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to this ebook's <see cref="Account"/>.
        /// </summary>
        public int? AccountId { get; set; }

        public Book? Book { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to this ebook's <see cref="Book"/>.
        /// </summary>
        [Display(Name = "Book")]
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the file name of the de-DRMed e-book file.
        /// </summary>
        [Display(Name = "DRM-Free File")]
        public string? DrmStrippedFileName { get; set; }

        [Display(Name = "Format")]
        public EbookFormat EbookFormat { get; set; }

        public string? DrmStrippedFileId { get; set; }

        public string? EbookFileId { get; set; }

        [Display(Name = "Source")]
        public EbookSource EbookSource { get; set; }

        /// <summary>
        /// Gets or sets the file name of the e-book file.
        /// </summary>
        [Display(Name = "Original File")]
        public string? FileName { get; set; }

        [Display(Name = "Ebook ISBN-13")]
        public string? PublisherISBN13 { get; set; }

        /// <summary>
        /// Gets or sets the version string provided by the publisher, if any.
        /// </summary>
        [Display(Name = "Publisher Version")]
        public string? PublisherVersion { get; set; }

        [Display(Name = "Vendor Book Identifier")]
        public string? VendorBookIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the version string provided by the vendor, if any.
        /// </summary>
        [Display(Name = "Vendor Version")]
        public string? VendorVersion { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder(EbookFormat.ToString());
            if (string.IsNullOrWhiteSpace(DrmStrippedFileName))
            {
                if (Account != null)
                {
                    result.Append(" (");
                    result.Append(Account.DisplayName);
                    result.Append(')');
                }
                else
                {
                    result.Append(" (DRM free)");
                }
            }
            else
            {
                result.Append(" (de-DRMed)");
            }
            result.Append(' ');
            result.Append(FileName);
            return result.ToString();
        }
    }
}
