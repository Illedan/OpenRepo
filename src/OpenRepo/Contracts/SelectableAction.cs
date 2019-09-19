using System;
namespace OpenRepo.Contracts
{
    public class SelectableAction
    {
        public SelectableAction(string title, Action action)
        {
            Title = title;
            Action = action;
        }

        public Action Action { get; }
        public string Title { get; }
    }
}
