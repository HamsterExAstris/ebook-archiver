namespace EbookArchiver.JNovelClub.Models
{
    internal class ResponseBase
    {
        /// <summary>
        /// Hidden constructor to prevent this class from being instantiated.
        /// </summary>
        protected ResponseBase()
        {
        }

        //[JsonPropertyName("pagination")]
        public Pagination? Pagination { get; set; }
    }
}
