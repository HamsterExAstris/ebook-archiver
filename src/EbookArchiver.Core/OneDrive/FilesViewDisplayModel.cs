using System;
using System.Collections.Generic;
using Microsoft.Graph;

namespace EbookArchiver.OneDrive
{
    public class FilesViewDisplayModel
    {
        // List of all child folders in current view
        public IList<DriveItem> Folders { get; set; } = Array.Empty<DriveItem>();

        // List of all child files in current view
        public IList<DriveItem> Files { get; set; } = Array.Empty<DriveItem>();

        // Currently selected folder
        public DriveItem? SelectedFolder { get; set; }

        // URL to next page of results
        public string? NextPageUrl { get; set; }
    }
}
