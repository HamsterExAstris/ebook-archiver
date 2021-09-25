using System;

namespace EbookArchiver.Models
{
    [Flags]
    public enum EbookFormat
    {
        None = 0,
        Palm = 1,
        MicrosoftReader = 2,
        Mobipocket = 4,
        KindleFormat7 = Mobipocket,
        Epub = 8,
        Topaz = 16,
        KindleFormat8 = 32,
        KindleFormat7And8 = KindleFormat7 | KindleFormat8,
        Hardcover = 64,
        TradePaperback = 128,
        MassMarketPaperback = 256,
        PortableDocumentFormat = 512
    }
}
