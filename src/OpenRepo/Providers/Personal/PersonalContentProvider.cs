using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenRepo.Contracts;

namespace OpenRepo.Providers.Personal
{
    public class PersonalContentProvider : IProvider
    {
        public PersonalContentProvider()
        {
        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            throw new NotImplementedException();
        }
    }
}
