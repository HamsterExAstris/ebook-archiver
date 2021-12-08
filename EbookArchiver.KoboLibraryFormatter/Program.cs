// See https://aka.ms/new-console-template for more information
using EbookArchiver.KoboLibraryFormatter;

Console.WriteLine("Hello, World!");

using var reader = new StreamReader("input.txt");
using var writer = new StreamWriter("output.csv");
string? line;
LibraryItem? item = null;
do
{
    line = await reader.ReadLineAsync();
    if (item == null)
    {
        item = new();
    }

    if (line == null)
    {
        // Do nothing.
    }
    else if (line.Contains("/20"))
    {
        // End of the row.
        int lastSlash = line.LastIndexOf('/', line.LastIndexOf('/') - 1);
        string? date = line.Substring(lastSlash - 2);
        if (!date.StartsWith('1'))
        {
            date = date.Substring(1);
        }
        item.Status = line.Substring(0, line.Length - date.Length);
        item.DateAdded = date;

        OutputObject(item, writer);
        item = null;
    }
    else if (item.Title == null)
    {
        item.Title = line;
    }
    else if (item.Author == null)
    {
        item.Author = line;
    }
    else if (item.Genre == null)
    {
        item.Genre = line;
    }
    else if (item.Series == null)
    {
        item.Series = item.Genre;
        item.Genre = line;
    }
    else
    {
        throw new InvalidOperationException($"Too many lines: [{item.Title}], {line}");
    }
}
while (line != null);

static void OutputObject(LibraryItem item, StreamWriter writer)
{
    writer.Write('"');
    writer.Write(item.Title);
    writer.Write("\",\"");
    writer.Write(item.Author);
    writer.Write("\",\"");
    writer.Write(item.Series);
    writer.Write("\",\"");
    writer.Write(item.Genre);
    writer.Write("\",\"");
    writer.Write(item.Status);
    writer.Write("\",\"");
    writer.Write(item.DateAdded);
    writer.WriteLine('"');
}
