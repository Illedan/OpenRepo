namespace OpenRepo.Desktop
{
    public class LauncherRow
    {
        public LauncherRow(string title, string shortcut = null)
        {
            Title = title;
            Shortcut = shortcut;
        }

        public string Title { get; }

        /// <summary>
        /// The key that runs this row directly, only set when choosing an action.
        /// </summary>
        public string Shortcut { get; }

        public bool HasShortcut => Shortcut != null;
    }
}
