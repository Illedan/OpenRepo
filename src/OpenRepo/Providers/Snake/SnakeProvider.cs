using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.View;

namespace OpenRepo.Providers.Snake
{
    public class SnakeProvider : IProvider
    {
        private readonly int m_time;

        public SnakeProvider(int time)
        {
            m_time = time;
        }

        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            var bag = new ConcurrentBag<SelectableItem>();
            bag.Add(new SelectableItem("Snake", () => new SelectableAction[] { new SelectableAction("Play", () => Viewer.Push(new SnakeViewModel(m_time))) }));
            return Task.FromResult(bag);
        }
    }
}
