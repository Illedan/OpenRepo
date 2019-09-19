using System.Diagnostics;
using System.IO;
using System.Linq;
using OpenRepo.Contracts;
using OpenRepo.View;
using OpenRepo.ViewModels;

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

            if(programs.Length > 1)
            {
                var item = new SelectableItem(path + "*." + programType, () => programs.Select(p => new SelectableAction(FileService.GetFileName(p), () => StartProgram(p))).ToArray());
                Viewer.Push(new ActionSelectionViewModel(item));
            }
            else
            {
                StartProgram(programs.First());
            }
        }

        public static void StartProgram(string path)
        {
            var startInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            };

            Process.Start(startInfo);
        }
    }
}
