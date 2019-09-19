using System;

namespace OpenRepo.Contracts
{
    public class SelectableItem
    {
        public SelectableItem(string title, Func<SelectableAction[]> actionsFactory)
        {
            Title = title;
            ActionsFactory = actionsFactory;
        }

        //TODO: Create ID to use in Machine learning to count usages?

        public string Title { get; }
        public Func<SelectableAction[]> ActionsFactory { get; }
    }
}
