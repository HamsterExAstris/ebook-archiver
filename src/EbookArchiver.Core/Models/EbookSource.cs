using System.ComponentModel.DataAnnotations;

namespace EbookArchiver.Models
{
    public enum EbookSource
    {
        None = 0,
        [Display(Name = "Digital Original")]
        DigitalOriginal = 1,
        Scanned = 2,
        [Display(Name = "Digital Recreation")]
        DigitalRecreation = 4
    }
}
