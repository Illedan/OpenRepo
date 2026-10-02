using System;
using System.Collections.Generic;
using System.Linq;
using OpenRepo.Contracts;

namespace OpenRepo.Services
{
    public static class ItemFilterService
    {
        /// <summary>
        /// Keeps the items whose title contains every space separated word of the search text, ignoring case.
        /// </summary>
        public static List<SelectableItem> Filter(List<SelectableItem> items, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return items;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return items.Where(i => words.All(w => i.Title.Contains(w, StringComparison.OrdinalIgnoreCase))).ToList();
        }
    }
}
