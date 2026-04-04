using SMB4LeagueImportTool.Models;

namespace SMB4LeagueImportTool.Core
{
    public static class Smb4SaveFolderValidator
    {
        public static SaveFolderValidationResult Validate(string? savesFolderPath)
        {
            if (string.IsNullOrWhiteSpace(savesFolderPath))
            {
                return SaveFolderValidationResult.Invalid(
                    "Please select a valid SMB4 saves folder first.",
                    "No saves folder selected.");
            }

            if (!Directory.Exists(savesFolderPath))
            {
                return SaveFolderValidationResult.Invalid(
                    "The selected saves folder no longer exists.\n\nPlease re-select the folder.",
                    "Saves folder not found.");
            }

            string masterSavPath = Path.Combine(
                savesFolderPath,
                Smb4SaveConstants.MasterSaveFileName);

            if (!File.Exists(masterSavPath))
            {
                return SaveFolderValidationResult.Invalid(
                    "master.sav was not found in the selected folder.\n\n" +
                    "Please select the folder that contains your SMB4 save files.",
                    "master.sav not found.");
            }

            return SaveFolderValidationResult.Valid();
        }

        public static bool IsValid(string? savesFolderPath) =>
            Validate(savesFolderPath).IsValid;

        public static string GetMasterSavPath(string savesFolderPath) =>
            Path.Combine(savesFolderPath, Smb4SaveConstants.MasterSaveFileName);
    }
}