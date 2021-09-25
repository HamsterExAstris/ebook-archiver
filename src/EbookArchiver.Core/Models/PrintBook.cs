using System;

namespace EbookArchiver.Models
{
    /// <summary>
    /// Information about a book.
    /// </summary>
    public class PrintBook
    {
        /// <summary>
        /// Gets or sets the foreign key for this book.
        /// </summary>
        public int BookId { get; internal set; }

        /// <summary>
        /// Gets or sets the book's format.
        /// </summary>
        public EbookFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the book's ISBN-13.
        /// </summary>
        public string? ISBN13 { get; set; }

        /// <summary>
        /// Gets or sets the book's ISBN-10.
        /// </summary>
        public void SetISBN10(string value)
        {
            this.ISBN13 = "978" + value.Substring(0, 9);

            // Convert the string to an array of digits.
            var digits = new byte[12];
            byte nextIndex = 0;
            foreach (var digit in this.ISBN13)
            {
                digits[nextIndex] = Convert.ToByte(digit);
                nextIndex++;
            }

            // Sum up the digits.
            int sum = 0;
            for (int x = 0; x < 6; x++)
            {
                sum += digits[2 * x] + (3 * digits[(2 * x) + 1]);
            }

            // Calculate the check digit and append it to the ISBN.
            var checkDigit = (10 - (sum % 10)) % 10;
            this.ISBN13 += checkDigit.ToString();
        }
    }
}
