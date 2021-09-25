using System;
using System.Threading;
using System.Xml.Linq;

namespace EbookArchiver.Models
{
    public static class ILibraryExtensions
    {
        /// <summary>
        /// Converts this Library to a SpreadsheetML XML document.
        /// </summary>
        public static XDocument ToXmlSpreadsheet(this ILibrary library)
        {
            // SpreadsheetML references:
            //  http://blogs.msdn.com/b/bethmassi/archive/2007/10/30/quickly-import-and-export-excel-data-with-linq-to-xml.aspx
            //  http://en.wikipedia.org/wiki/Microsoft_Office_XML_formats#Excel_XML_Spreadsheet_example
            // Linq to XML reference: http://stackoverflow.com/questions/8768762/use-xml-literals-in-c
            XNamespace nsSpreadsheet = "urn:schemas-microsoft-com:office:spreadsheet";
            XNamespace nsOffice = "urn:schemas-microsoft-com:office:office";
            XDocument doc = new XDocument(
                                new XDeclaration("1.0", "utf-8", "no"),
                                new XElement(nsSpreadsheet + "Workbook",
                                    new XElement(nsOffice + "DocumentProperties",
                                        new XElement("Author", Thread.CurrentPrincipal?.Identity?.Name),
                                        new XElement("Created", DateTime.UtcNow.ToString("O"))
                                    ),
                                    new XElement("Worksheet",
                                        new XElement("Table",
                                            new XElement("Row",
                                                new XElement("Cell",
                                                    new XElement("Data",
                                                        new XAttribute(nsSpreadsheet + "Type", "String"),
                                                        "this is a test"
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            );

            return doc;
        }
    }
}
