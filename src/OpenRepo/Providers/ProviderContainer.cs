using System;
using System.Collections.Generic;
using System.Linq;
using OpenRepo.Contracts;
using OpenRepo.Providers.Local;

namespace OpenRepo.Providers
{
    public static class ProviderContainer
    {
        private static IProviderFactory[] m_factories =
        {
            new LocalFactory()
        };

        public static List<IProvider> GetProviders(string configuration)
        {
            var providers = new List<IProvider>();
            var lines = configuration.Split("\n");
            IProviderFactory currentProviderFactory = null;
            foreach(var line in lines)
            {
                if (line.StartsWith('#')) continue; //Comment.
                if(line.StartsWith(' ') || line.StartsWith('\t'))
                {
                    if (currentProviderFactory == null) continue; // TODO: Throw, atleast print to user?
                    providers.Add(currentProviderFactory.GetProvider(line.Trim()));
                }
                else if(!string.IsNullOrEmpty(line.Trim()))
                {
                    var providerId = line.Replace(":", " ").Trim();
                    currentProviderFactory = m_factories.FirstOrDefault(f => f.Id == providerId);
                }
            }

            return providers;
        }
    }
}
