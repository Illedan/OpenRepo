using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Illedan.OpenRepo.Providers.Local;
using OpenRepo.Contracts;
using OpenRepo.View;
using OpenRepo.ViewModels;

namespace OpenRepo.Services
{
    public static class StartProgramService
    {
        public static HashSet<string> IgnoredFolders = new HashSet<string>();
        public static void StartProgramOfType(string programType, string path, bool includeAll)
        {
            var programs = GetFiles(path, programType, includeAll ? 999 : 1, IgnoredFolders).ToArray();
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

        private static List<string> GetFiles(string path, string programType, int maxLevel, HashSet<string> ignored)
        {
            var res = new List<string>();
            TraverseLevel(path, programType, ignored, res, maxLevel);
            void TraverseLevel(string path, string programType, HashSet<string> ignoredFolders, List<string> files, int maxLevel, int level = 0)
            {
                if (level >= maxLevel) return;
                files.AddRange(Directory.GetFiles(path, $"*.{programType}", SearchOption.TopDirectoryOnly));
                var newFolders = Directory.GetDirectories(path);
                foreach (var folder in newFolders)
                {
                    var dirName = Path.GetFileName(folder);
                    if (ignoredFolders.Contains(dirName)) continue;
                    TraverseLevel(folder, programType, ignoredFolders, files, maxLevel, level + 1);
                }
            }

            return res;
        }

        public static void StartProgram(string path)
        {
            var startInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true,
            };

            Process.Start(startInfo);
        }

        public static void RunScript(LocalScript script, string path)
        {
            var toRun = script.Value.Replace("{{path}}", path);
            TerminalService.Instance.Term(toRun);
        }
    }
}
