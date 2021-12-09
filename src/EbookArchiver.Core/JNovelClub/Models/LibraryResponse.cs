using System;

namespace EbookArchiver.JNovelClub.Models
{
    internal class LibraryResponse : ResponseBase
    {
        //[JsonPropertyName("books")]
        public JNovelClubPurchase[] Books { get; set; } = Array.Empty<JNovelClubPurchase>();
    }
}
