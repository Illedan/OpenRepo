using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Illedan.OpenRepo.Providers.Copy;
using Illedan.OpenRepo.Providers.Settings;
using OpenRepo.Contracts;
using OpenRepo.Providers.Local;
using OpenRepo.Providers.OpenRepo;
using OpenRepo.Providers.Personal;
using OpenRepo.Services;

namespace OpenRepo.Providers
{
    public static class ProviderContainer
    {
        private static IProviderFactory[] m_factories =
        {
            new LocalFactory(),
            new PersonalContentProviderFactory(),
            new CopyTextProviderFactory(),
            new SettingsProviderFactory()
        };

        /// <summary>
        /// Loads the items of every provider in the configuration, sorted by title.
        /// </summary>
        /// <param name="extraFactories">Providers only one of the user interfaces supports, like the terminal's Snake.</param>
        public static async Task<List<SelectableItem>> GetItems(string configuration, params IProviderFactory[] extraFactories)
        {
            var tasks = GetProviders(configuration, extraFactories).Select(p => p.GetItems()).ToArray();
            await Task.WhenAll(tasks);
            return tasks.SelectMany(t => t.Result).OrderBy(i => i.Title).ToList();
        }

        public static List<IProvider> GetProviders(string configuration, params IProviderFactory[] extraFactories)
        {
            var factories = m_factories.Concat(extraFactories).ToArray();
            var providers = new List<IProvider> { new OpenRepoProviderFactory().GetProvider(string.Empty) };
            var lines = configuration.Split("\n");
            IProviderFactory currentProviderFactory = null;

            foreach(var line in lines)
            {
                try
                {
                    if (line.Trim().StartsWith('#') || string.IsNullOrEmpty(line.Trim()))
                    {
                        continue; // Lines starting with # is a comment.
                    }

                    if(line.StartsWith(' ') || line.StartsWith('\t'))
                    {
                        if (currentProviderFactory == null)
                        {
                            LogService.Log($"Please provide a provider before you add configuration.");
                            continue;
                        }

                        providers.Add(currentProviderFactory.GetProvider(line.Trim()));
                    }
                    else if(!string.IsNullOrEmpty(line.Trim()))
                    {
                        var providerId = line.Replace(":", " ").Trim();
                        currentProviderFactory = factories.FirstOrDefault(f => f.Id == providerId);
                        if (currentProviderFactory == null)
                        {
                            LogService.Log($"Can't find provider with id {providerId}");
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("Config Error: <<" + line + ">> " + e.Message);
                }
            }

            return providers;
        }
    }
}
