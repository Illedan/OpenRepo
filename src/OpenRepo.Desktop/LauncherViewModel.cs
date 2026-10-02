using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace OpenRepo.Desktop
{
    /// <summary>
    /// The desktop version of the terminal's MainViewModel and ActionSelectionViewModel:
    /// a filtered list of items, and the actions of one item once it is opened.
    /// </summary>
    public class LauncherViewModel : INotifyPropertyChanged
    {
        private readonly Func<Task<List<SelectableItem>>> m_loadItems;
        private List<SelectableItem> m_items = new List<SelectableItem>();
        private List<SelectableItem> m_filteredItems = new List<SelectableItem>();
        private SelectableItem m_actionItem;
        private SelectableAction[] m_actions;
        private Dictionary<char, SelectableAction> m_shortcuts;
        private IReadOnlyList<LauncherRow> m_rows = Array.Empty<LauncherRow>();
        private string m_searchText = string.Empty;
        private int m_selectedIndex;
        private bool m_isLoading;
        private int m_loadVersion;
        private bool m_actionsShownWhileRunning;

        public LauncherViewModel(Func<Task<List<SelectableItem>>> loadItems)
        {
            m_loadItems = loadItems;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when an action finished without errors, or the user pressed escape with nothing left to go back from.
        /// </summary>
        public event Action HideRequested;

        public IReadOnlyList<LauncherRow> Rows
        {
            get => m_rows;
            private set
            {
                m_rows = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(EmptyText));
            }
        }

        public string SearchText
        {
            get => m_searchText;
            set
            {
                value ??= string.Empty;
                if (m_searchText == value) return;
                m_searchText = value;
                OnPropertyChanged();
                if (!IsChoosingAction) ApplyFilter();
            }
        }

        public int SelectedIndex
        {
            get => m_selectedIndex;
            set
            {
                if (m_selectedIndex == value) return;
                m_selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => m_isLoading;
            private set
            {
                m_isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EmptyText));
            }
        }

        public bool IsChoosingAction => m_actionItem != null;

        public string ActionTitle => m_actionItem?.Title;

        public string Message => LogService.Message.Trim();

        public bool HasMessage => !string.IsNullOrEmpty(Message);

        public bool IsEmpty => Rows.Count == 0;

        public string EmptyText =>
            IsLoading ? "Loading…" :
            m_items.Count == 0 ? "Nothing to show yet. Edit your config, then press Tab to reload." :
            "No matches";

        public async Task Reload()
        {
            var version = ++m_loadVersion;
            IsLoading = true;
            LogService.Clear();
            List<SelectableItem> items;
            try
            {
                items = await m_loadItems();
            }
            catch (Exception e)
            {
                LogService.Log(e.Message);
                items = new List<SelectableItem>();
            }

            if (version != m_loadVersion) return; // A newer reload has started.
            m_items = items;
            IsLoading = false;
            if (!IsChoosingAction) ApplyFilter();
            OnMessageChanged();
        }

        /// <summary>
        /// Goes back to an empty search, as when the launcher was first opened.
        /// </summary>
        public void Reset()
        {
            CloseActions();
            SearchText = string.Empty;
            SelectedIndex = 0;
        }

        public void MoveSelection(int delta)
        {
            if (Rows.Count == 0) return;
            SelectedIndex = ((Math.Max(0, SelectedIndex) + delta) % Rows.Count + Rows.Count) % Rows.Count;
        }

        public void Activate()
        {
            if (SelectedIndex < 0 || SelectedIndex >= Rows.Count) return;
            if (IsChoosingAction)
            {
                RunAction(m_actions[SelectedIndex]);
                return;
            }

            var item = m_filteredItems[SelectedIndex];
            SelectableAction[] actions;
            try
            {
                actions = item.ActionsFactory();
            }
            catch (Exception e)
            {
                LogService.Log(e.Message);
                OnMessageChanged();
                return;
            }

            if (actions.Length == 1)
            {
                RunAction(actions[0]);
            }
            else if (actions.Length > 1)
            {
                ShowActions(item, actions);
            }
        }

        /// <summary>
        /// Runs the action with this shortcut key, when choosing an action.
        /// </summary>
        public bool TryRunShortcut(char key)
        {
            if (!IsChoosingAction || !m_shortcuts.TryGetValue(char.ToLower(key), out var action)) return false;
            RunAction(action);
            return true;
        }

        public void Back()
        {
            if (IsChoosingAction)
            {
                CloseActions();
            }
            else if (SearchText.Length > 0 || HasMessage)
            {
                SearchText = string.Empty;
                LogService.Clear();
                OnMessageChanged();
            }
            else
            {
                HideRequested?.Invoke();
            }
        }

        public void ShowActions(SelectableItem item, SelectableAction[] actions = null)
        {
            m_actionsShownWhileRunning = true;
            m_actionItem = item;
            m_actions = actions ?? item.ActionsFactory();
            m_shortcuts = ActionShortcutService.GetShortcuts(m_actions);
            Rows = m_actions.Select(a => new LauncherRow(a.Title, m_shortcuts.First(s => s.Value == a).Key.ToString().ToUpper())).ToList();
            SelectedIndex = 0;
            OnPropertyChanged(nameof(IsChoosingAction));
            OnPropertyChanged(nameof(ActionTitle));
        }

        private void RunAction(SelectableAction action)
        {
            LogService.Clear();
            m_actionsShownWhileRunning = false;
            action.Action();
            OnMessageChanged();
            if (m_actionsShownWhileRunning) return; // The action asked to choose between more actions.

            CloseActions();
            SearchText = string.Empty; // Keeps the item selected, to retry if the action failed.
            if (!HasMessage) HideRequested?.Invoke();
        }

        private void CloseActions()
        {
            if (!IsChoosingAction) return;
            var item = m_actionItem;
            m_actionItem = null;
            m_actions = null;
            m_shortcuts = null;
            OnPropertyChanged(nameof(IsChoosingAction));
            OnPropertyChanged(nameof(ActionTitle));
            ApplyFilter(item);
        }

        private void ApplyFilter(SelectableItem selectItem = null)
        {
            var previous = selectItem ??
                (SelectedIndex >= 0 && SelectedIndex < m_filteredItems.Count ? m_filteredItems[SelectedIndex] : null);
            m_filteredItems = ItemFilterService.Filter(m_items, SearchText);
            Rows = m_filteredItems.Select(i => new LauncherRow(i.Title)).ToList();
            SelectedIndex = Math.Max(0, m_filteredItems.IndexOf(previous));
        }

        private void OnMessageChanged()
        {
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(HasMessage));
        }

        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
