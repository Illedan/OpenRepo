using System.Diagnostics;
using System.IO;

namespace OpenRepo.Services
{
    public static class FolderService
    {
        public static void OpenLocation(string path)
        {
            var startInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            };

            Process.Start(startInfo);
        }

        public static string[] GetFolders(string path) => Directory.GetDirectories(path);
    }
}
