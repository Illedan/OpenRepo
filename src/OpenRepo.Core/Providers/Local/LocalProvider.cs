using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Illedan.OpenRepo.Providers.Local;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace OpenRepo.Providers.Local
{
    public class LocalProvider : IProvider
    {
        private readonly string m_path;
        private readonly string m_prefix;
        private readonly string[] m_programTypesToStart;
        private readonly string[] m_programTypesTopFolderToStart;
        private readonly LocalScript[] m_scripts;

        public LocalProvider(string configuration)
        {
            var splittedConfig = configuration.Trim().SplitPath();
            m_path = splittedConfig[0];
            if (!Directory.Exists(m_path))
            {
                throw new Exception("Folder not found: " + m_path);
            }

            var splitters = splittedConfig.Skip(1).Select(s => new LocalScript(s)).ToList();

            m_prefix = splitters
                .FirstOrDefault(s => s.Key == "prefix")
                ?.Value ?? string.Empty;


            m_programTypesToStart = splitters
                .Where(s => s.Key == "pt")
                .Select(s => s.Value) 
                .ToArray();

            m_programTypesTopFolderToStart = splitters
                .Where(s => s.Key=="ptt")
                .Select(s => s.Value) 
                .ToArray();

            splitters.RemoveAll(s => s.Key == "pt" || s.Key == "ptt" || s.Key == "prefix");

            m_scripts = splitters.ToArray();
        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            var folders = FolderService.GetFolders(m_path);
            var items = folders.Select(f => new SelectableItem(m_prefix + FileService.GetFileName(f), () => GetActions(f))).ToList();
            return Task.FromResult(new ConcurrentBag<SelectableItem>(items));
        }

        private SelectableAction[] GetActions(string path)
        {
            var actions = new List<SelectableAction>
            {
                new SelectableAction("Open", () => FolderService.OpenLocation(path)),
            };

            //if (false) // Disable until I can figure it out
            //{
            //    actions.Add(new SelectableAction("Terminal", () => TerminalService.OpenTerminal(path)));
            //}

            var hasGit = GitService.TryGetRemoteGitLocation(path, out string uri);
            if (hasGit)
            {
                actions.Add(new SelectableAction("Web", () => StartProgramService.StartProgram(uri)));
            }

            foreach (var type in m_programTypesToStart)
            {
                actions.Add(new SelectableAction(type, () => StartProgramService.StartProgramOfType(type, path, true)));
            }

            foreach (var type in m_programTypesTopFolderToStart)
            {
                actions.Add(new SelectableAction(type, () => StartProgramService.StartProgramOfType(type, path, false)));
            }

            foreach(var script in m_scripts)
            {
                actions.Add(new SelectableAction(script.Key, () => StartProgramService.RunScript(script, path)));
            }

            return actions.ToArray();
        }
    }
}
