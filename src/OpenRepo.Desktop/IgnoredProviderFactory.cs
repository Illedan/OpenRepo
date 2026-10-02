using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenRepo.Contracts;

namespace OpenRepo.Desktop
{
    /// <summary>
    /// Accepts a config section that only the terminal version supports, so it is not reported as an error.
    /// </summary>
    public class IgnoredProviderFactory : IProviderFactory, IProvider
    {
        public IgnoredProviderFactory(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public IProvider GetProvider(string configuration) => this;

        public Task<ConcurrentBag<SelectableItem>> GetItems() => Task.FromResult(new ConcurrentBag<SelectableItem>());
    }
}
