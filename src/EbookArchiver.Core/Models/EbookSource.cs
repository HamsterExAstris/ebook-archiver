using System;

namespace EbookArchiver.Models
{
    [Flags]
    public enum EbookSource
    {
        None = 0,
        DigitalOriginal = 1,
        Scanned = 2,
        DigitalRecreation = 4
    }
}
