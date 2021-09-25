using System.ComponentModel.DataAnnotations;

namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about an author.
    /// </summary>
    public class Author
    {
        public int AuthorId { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        public override string ToString() => DisplayName;
    }
}
