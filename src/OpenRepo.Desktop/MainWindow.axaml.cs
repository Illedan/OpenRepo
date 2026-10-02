using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenRepo.Desktop.MacOS;

namespace OpenRepo.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly TextBox m_search;
        private readonly TextBlock m_hotKeyHint;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            m_search = this.Get<TextBox>("Search");
            m_hotKeyHint = this.Get<TextBlock>("HotKeyHint");
            var results = this.Get<ListBox>("Results");

            AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
            AddHandler(TextInputEvent, OnPreviewTextInput, RoutingStrategies.Tunnel);
            results.Tapped += OnResultTapped;
            results.DoubleTapped += OnResultDoubleTapped;
            Activated += (_, _) => FocusSearch();
        }

        public MainWindow(LauncherViewModel viewModel) : this()
        {
            DataContext = viewModel;
            viewModel.HideRequested += HideLauncher;
        }

        private LauncherViewModel ViewModel => (LauncherViewModel)DataContext;

        /// <summary>
        /// Text in the footer about the global shortcut.
        /// </summary>
        public string HotKeyText
        {
            set
            {
                m_hotKeyHint.Text = value;
                m_hotKeyHint.IsVisible = !string.IsNullOrEmpty(value);
            }
        }

        public void ShowLauncher()
        {
            ViewModel.Reset();
            _ = ViewModel.Reload(); // Picks up new folders and config changes.
            Show();
            Application.Current?.TryGetFeature<IActivatableLifetime>()?.TryLeaveBackground();
            Activate();
            FocusSearch();
        }

        public void HideLauncher()
        {
            // On macOS this hides the whole app, which gives focus back to the app used before.
            if (Application.Current?.TryGetFeature<IActivatableLifetime>()?.TryEnterBackground() != true)
            {
                Hide();
            }
        }

        public void ToggleLauncher()
        {
            if (IsActive)
            {
                HideLauncher();
            }
            else
            {
                ShowLauncher();
            }
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            // Closing the window keeps the app running so the global shortcut still works. Quit with ⌘Q.
            if (e.CloseReason == WindowCloseReason.WindowClosing && !e.IsProgrammatic)
            {
                e.Cancel = true;
                HideLauncher();
            }

            base.OnClosing(e);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    ViewModel.MoveSelection(1);
                    break;
                case Key.Up:
                    ViewModel.MoveSelection(-1);
                    break;
                case Key.Enter:
                    ActivateSelected();
                    break;
                case Key.Escape:
                    ViewModel.Back();
                    break;
                case Key.Back when ViewModel.IsChoosingAction:
                    ViewModel.Back();
                    break;
                case Key.Tab:
                    _ = ViewModel.Reload();
                    break;
                case Key.W when e.KeyModifiers == KeyModifiers.Meta:
                    HideLauncher();
                    break;
                default:
                    return;
            }

            e.Handled = true;
        }

        private void OnPreviewTextInput(object sender, TextInputEventArgs e)
        {
            // While choosing an action, typing picks one by its shortcut instead of searching.
            if (!ViewModel.IsChoosingAction) return;
            e.Handled = true;
            if (!string.IsNullOrEmpty(e.Text)) RunShortcut(e.Text[0]);
        }

        private void OnResultTapped(object sender, TappedEventArgs e)
        {
            if (ViewModel.IsChoosingAction) ActivateSelected();
            FocusSearch();
        }

        private void OnResultDoubleTapped(object sender, TappedEventArgs e)
        {
            if (!ViewModel.IsChoosingAction) ActivateSelected();
        }

        private async void ActivateSelected()
        {
            await LoginShellEnvironment.Loaded;
            ViewModel.Activate();
        }

        private async void RunShortcut(char key)
        {
            await LoginShellEnvironment.Loaded;
            ViewModel.TryRunShortcut(key);
        }

        private void FocusSearch()
        {
            m_search.Focus();
            m_search.CaretIndex = m_search.Text?.Length ?? 0;
        }
    }
}
