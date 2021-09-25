using System;
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
        public int BookId { get; private set; }

        /// <summary>
        /// Gets or sets the file name of the de-DRMed e-book file.
        /// </summary>
        public string? DrmStrippedFileName { get; set; }

        public EbookFormat EbookFormat { get; set; }

        public EbookSource EbookSource { get; set; }

        /// <summary>
        /// Gets or sets the file name of the e-book file.
        /// </summary>
        public string? FileName { get; set; }

        // This GUID may be trashed later, but it's simpler to make it
        // assigned now rather than deal with the pain of making it
        // nullable.
        public Guid Folder { get; set; } = Guid.NewGuid();

        public string? PublisherISBN13 { get; set; }

        /// <summary>
        /// Gets or sets the version string provided by the publisher, if any.
        /// </summary>
        public string? PublisherVersion { get; set; }

        public string? VendorBookIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the version string provided by the vendor, if any.
        /// </summary>
        public string? VendorVersion { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder(this.EbookFormat.ToString());
            if (string.IsNullOrWhiteSpace(this.DrmStrippedFileName))
            {
                if (this.Account != null)
                {
                    result.Append(" (");
                    result.Append(this.Account.DisplayName);
                    result.Append(")");
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
            result.Append(" ");
            result.Append(this.FileName);
            return result.ToString();
        }
    }
}
