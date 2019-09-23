using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace OpenRepo.Providers.EditConfiguration
{
    public class EditConfigurationProvider : IProvider
    {
        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            return Task.FromResult(new ConcurrentBag<SelectableItem>
            {
                new SelectableItem("Edit config",
                    () => new SelectableAction[]
                    {
                        new SelectableAction("Edit", () => StartProgramService.StartProgram(ConfigurationService.ConfigurationPath))
                    })
            });
        }
    }
}
