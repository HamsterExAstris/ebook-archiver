using System.Text;

namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about a book.
    /// </summary>
    public class Book
    {
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the book's title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        public int? AuthorId { get; set; }

        public Author? Author { get; set; }

        public bool IsNotOwned { get; set; }

        public int? SeriesId { get; set; }

        public string? SeriesIndex { get; set; }

        public Series? Series { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();
            if (this.Author != null)
            {
                result.Append(this.Author.DisplayName);
                result.Append(" - ");
            }
            if (this.Series != null)
            {
                result.Append(this.Series.DisplayName);
                if (!string.IsNullOrWhiteSpace(this.SeriesIndex))
                {
                    result.Append(" #");
                    result.Append(this.SeriesIndex);
                }
                result.Append(" - ");
            }
            result.Append(this.Title);
            return result.ToString();
        }
    }
}
