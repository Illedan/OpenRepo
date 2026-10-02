using System.Collections.Generic;
using System.Linq;
using OpenRepo.Contracts;

namespace OpenRepo.Services
{
    public static class ActionShortcutService
    {
        /// <summary>
        /// Gives each action a key: the first letter of its title not already taken, otherwise its position (1-based).
        /// </summary>
        public static Dictionary<char, SelectableAction> GetShortcuts(SelectableAction[] actions)
        {
            var shortcuts = new Dictionary<char, SelectableAction>();
            for (var i = 0; i < actions.Length; i++)
            {
                var action = actions[i];
                var targetLetter = action.Title.ToLower().FirstOrDefault(letter => !shortcuts.ContainsKey(letter));
                shortcuts.Add(targetLetter == 0 ? (char)('0' + i + 1) : targetLetter, action);
            }

            return shortcuts;
        }
    }
}
