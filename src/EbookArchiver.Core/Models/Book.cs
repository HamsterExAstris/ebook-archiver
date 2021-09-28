using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Author(s)")]
        public int AuthorId { get; set; }

        public Author? Author { get; set; }

        [Display(Name = "Is this book stolen?")]
        public bool IsNotOwned { get; set; }

        [Display(Name = "Series")]
        public int? SeriesId { get; set; }

        [Display(Name = "Series Index")]
        public string? SeriesIndex { get; set; }

        public Series? Series { get; set; }

        public string? FolderId { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();
            if (Author != null)
            {
                result.Append(Author.DisplayName);
                result.Append(" - ");
            }
            if (Series != null)
            {
                result.Append(Series.DisplayName);
                if (!string.IsNullOrWhiteSpace(SeriesIndex))
                {
                    result.Append(" #");
                    result.Append(SeriesIndex);
                }
                result.Append(" - ");
            }
            result.Append(Title);
            return result.ToString();
        }
    }
}
