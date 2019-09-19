using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OpenRepo.Services
{
    public static class StartProgramService
    {
        public static void StartProgramOfType(string programType, string path, bool includeAll)
        {
            var programs = Directory.GetFiles(path, $"*.{programType}", includeAll ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if (programs.Length == 0)
            {
                LogService.Log($"No type of {programType} found in {path}.");
                return;
            }

            //TODO: Create a subviewer to select the one you want for more than 1.
            // Only a caser if a set of directories contains a lot of *.programType.
            var startInfo = new ProcessStartInfo(programs.First())
            {
                UseShellExecute = true,
            };

            Process.Start(startInfo);
        }
    }
}
