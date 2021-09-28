﻿namespace EbookArchiver.OneDrive
{
    public class GraphConstants
    {
        // Defines the permission scopes used by the app
        public static readonly string[] DefaultScopes =
        {
            UserRead,
            FilesReadWriteAppFolder
        };

        // Default page size for collections
        public const int PageSize = 25;

        // User
        public const string UserRead = "User.Read";
        public const string UserReadBasicAll = "User.ReadBasic.All";
        public const string UserReadAll = "User.Read.All";
        public const string UserReadWrite = "User.ReadWrite";
        public const string UserReadWriteAll = "User.ReadWrite.All";

        // Files
        public const string FilesReadWriteAppFolder = "Files.ReadWrite.AppFolder";

        // Errors
        public const string ItemNotFound = "ErrorItemNotFound";
        public const string RequestDenied = "Authorization_RequestDenied";
        public const string RequestResourceNotFound = "Request_ResourceNotFound";
        public const string ResourceNotFound = "ResourceNotFound";
    }
}
