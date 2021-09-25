using System.ComponentModel.DataAnnotations;

namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about an acount.
    /// </summary>
    public class Account
    {
        public int AccountId { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        public override string ToString() => DisplayName;
    }
}
