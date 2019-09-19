using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRepo.Contracts
{
    public interface IProvider
    {
        /// <summary>
        /// A list of all items.
        /// Could return an empty list populated with data once fetched.
        /// As the list would be requerries every time the search is reset.
        /// </summary>
        /// <returns></returns>
        Task<ConcurrentBag<SelectableItem>> GetItems();
    }
}
