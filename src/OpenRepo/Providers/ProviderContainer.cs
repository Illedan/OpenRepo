using System;
using System.Collections.Generic;
using System.Linq;
using OpenRepo.Contracts;
using OpenRepo.Providers.EditConfiguration;
using OpenRepo.Providers.Local;
using OpenRepo.Services;

namespace OpenRepo.Providers
{
    public static class ProviderContainer
    {
        private static IProviderFactory[] m_factories =
        {
            new LocalFactory(),
            new EditConfigurationProviderFactory()
        };

        public static List<IProvider> GetProviders(string configuration)
        {
            var providers = new List<IProvider>();
            var lines = configuration.Split("\n");
            IProviderFactory currentProviderFactory = null;

            foreach(var line in lines)
            {
                try
                {
                    if (line.Trim().StartsWith('#'))
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
                        currentProviderFactory = m_factories.FirstOrDefault(f => f.Id == providerId);
                        if (currentProviderFactory == null)
                        {
                            LogService.Log($"Can't find provider with id {providerId}");
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.Log("Line failed: " + line + " " + e.Message);
                }
            }

            return providers;
        }
    }
}
