using System.ComponentModel.DataAnnotations;

namespace EbookArchiver.Models
{
    public enum EbookFormat
    {
        None = 0,
        Palm = 1,
        [Display(Name = "Microsoft Reader")]
        MicrosoftReader = 2,
        [Display(Name = "Mobipocket/Kindle Format 7")]
        Mobipocket = 4,
        Epub = 8,
        Topaz = 16,
        [Display(Name = "Kindle Format 8")]
        KindleFormat8 = 32,
        [Display(Name = "Kindle Format 7 + 8")]
        KindleFormat7And8 = Mobipocket | KindleFormat8,
        [Display(Name = "CBZ")]
        ComicBookZip = 64,
        [Display(Name = "CBR")]
        ComicBookRar = 128,
        [Display(Name = "PDF")]
        PortableDocumentFormat = 512,
        [Display(Name = "Kindle KFX")]
        KindleKfx = 1024
    }
}
