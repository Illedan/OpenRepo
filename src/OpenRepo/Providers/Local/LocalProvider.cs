using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace OpenRepo.Providers.Local
{
    public class LocalProvider : IProvider
    {
        private readonly string m_path;

        public LocalProvider(string path)
        {
            m_path = path;
        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            var folders = FolderService.GetFolders(m_path);
            var items = folders.Select(f => new SelectableItem(FileService.GetFileName(f), () => GetActions(f))).ToList();
            return Task.FromResult(new ConcurrentBag<SelectableItem>(items));
        }

        private SelectableAction[] GetActions(string path)
        {
            var actions = new List<SelectableAction>
            {
                new SelectableAction("Open", () => FolderService.OpenLocation(path)),
                new SelectableAction("Solution", () => StartProgramService.StartProgramOfType("sln", path, true)), // TODO: Subconfig
                new SelectableAction("Terminal", () => TerminalService.OpenTerminal(path)),
            };

            var hasGit = GitService.TryGetRemoteGitLocation(out string uri);
            if (hasGit)
            {
                actions.Add(new SelectableAction("Web", () => Process.Start(uri)));
            }

            return actions.ToArray();
        }
    }
}
