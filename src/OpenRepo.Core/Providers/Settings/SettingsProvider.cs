using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace Illedan.OpenRepo.Providers.Settings
{
    public class SettingsProvider : IProvider
    {
        public SettingsProvider(string configuration)
        {
            var items = configuration.SplitPath();
            if (items.Length < 2)
            {
                throw new Exception("Settings should have 2 properties with space between: name value");
            }

            var key = items[0];
            var value = items[1];
            if (key.Equals("ignore", StringComparison.InvariantCultureIgnoreCase)) StartProgramService.IgnoredFolders.Add(value);
        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            return Task.FromResult(new ConcurrentBag<SelectableItem>());
        }
    }
}
