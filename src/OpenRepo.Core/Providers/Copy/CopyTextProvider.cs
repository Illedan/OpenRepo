using Illedan.OpenRepo.Services;
using OpenRepo.Contracts;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Illedan.OpenRepo.Providers.Copy
{
    public class CopyTextProvider : IProvider
    {
        private string m_key, m_value;

        public CopyTextProvider(string configuration)
        {
            var items = configuration.Split();
            if (items.Length < 2)
            {
                throw new Exception("Clipboard should have 2 values. Value is whatever is behind the first space: name value");
            }

            m_key = items[0];
            m_value = string.Join(" ", items.Skip(1));
        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            var bag = new ConcurrentBag<SelectableItem>();
            bag.Add(new SelectableItem(m_key, () => new SelectableAction[] { new SelectableAction("Copy", () => CopyToClipboardService.AddTextToClipboard(m_value)) }));
            return Task.FromResult(bag);
        }
    }
}
