using SafeAuthenticator.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(VaultConnectionFileManager))]

namespace SafeAuthenticator.Services
{
    public class VaultConnectionFileManager
    {
        private static string _defaultVaultConnectionFileName = "vault_connection_info.config";

        internal void AddNewVaultConnectionConfigFile(string friendlyFileName, string fileLocation)
        {
            // Todo: Copy the file, add entry in the list and give it a random name and store in app Directory.
        }

        internal void DeleteVaultConnectionConfigFile(string friendlyFileName)
        {
            // Todo: Delete file from the app directory and remove from list as well.
        }

        internal void SetAsActiveConnectionConfigFile(string friendlyFileName)
        {
            // Todo: Replace existing vault file with the given file.
        }

        internal void GetAllFileEntries()
        {
            // Todo: Read file list from the memory and return the same.
        }

        internal void DeleteAllFiles()
        {
            // Todo: Delete all config files and clear the list in memory.
        }
    }
}
