using System;
using OpenRepo.Contracts;

namespace OpenRepo.Services
{
    /// <summary>
    /// Lets shared code ask the active UI (terminal or desktop) to show a list of actions to choose from.
    /// </summary>
    public static class ActionSelectionService
    {
        public static Action<SelectableItem> Handler { get; set; }

        public static void Show(SelectableItem item) => Handler?.Invoke(item);
    }
}
