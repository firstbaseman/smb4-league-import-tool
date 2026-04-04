using System;
using System.Collections.Generic;
using System.Text;

namespace SMB4LeagueImportTool.Models
{
    public sealed class SaveFolderValidationResult
    {
        public bool IsValid { get; init; }

        public string Message { get; init; } = string.Empty;

        public string StatusText { get; init; } = string.Empty;

        public static SaveFolderValidationResult Valid() =>
            new()
            {
                IsValid = true,
                Message = string.Empty,
                StatusText = string.Empty
            };

        public static SaveFolderValidationResult Invalid(
            string message,
            string statusText) =>
            new()
            {
                IsValid = false,
                Message = message,
                StatusText = statusText
            };
    }
}