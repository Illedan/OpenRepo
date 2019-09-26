using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public LocalProvider(string configuration)
        {
            var splittedConfig = configuration.Trim().SplitPath();
            m_path = splittedConfig[0];
            if (!Directory.Exists(m_path))
            {
                throw new Exception("Folder not found: " + m_path);
            }

            m_prefix = splittedConfig
                .FirstOrDefault(s => s.StartsWith("prefix:", StringComparison.CurrentCultureIgnoreCase))
                ?.Split(':').Last() ?? string.Empty;

            m_programTypesToStart = splittedConfig
                .Where(s => s.StartsWith("pt:", StringComparison.CurrentCultureIgnoreCase))
                .Select(s => s.Split(':').Last()) 
                .ToArray();

            m_programTypesTopFolderToStart = splittedConfig
                .Where(s => s.StartsWith("ptt:", StringComparison.CurrentCultureIgnoreCase))
                .Select(s => s.Split(':').Last()) 
                .ToArray();
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
                new SelectableAction("Terminal", () => TerminalService.OpenTerminal(path)),
            };

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
                actions.Add(new SelectableAction(type, () => StartProgramService.StartProgramOfType(type, path, true)));
            }

            return actions.ToArray();
        }
    }
}
