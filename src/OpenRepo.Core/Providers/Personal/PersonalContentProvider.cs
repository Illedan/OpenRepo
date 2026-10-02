using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace OpenRepo.Providers.Personal
{
    public class PersonalContentProvider : IProvider
    {
        private string m_key, m_value;

        public PersonalContentProvider(string configuration)
        {
            var items = configuration.SplitPath();
            if(items.Length < 2)
            {
                throw new Exception("Personal should have 2 properties with space between: name value");
            }
            m_key = items[0];
            m_value = items[1];

        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            var bag = new ConcurrentBag<SelectableItem>();
            bag.Add(new SelectableItem(m_key, () => new SelectableAction[] { new SelectableAction("Open", () => StartProgramService.StartProgram(m_value)) }));
            return Task.FromResult(bag);
        }
    }
}
