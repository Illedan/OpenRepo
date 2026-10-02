using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OpenRepo.Desktop.MacOS;
using OpenRepo.Providers;
using OpenRepo.Services;

namespace OpenRepo.Desktop
{
    public partial class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (OS.IsMac()) LoginShellEnvironment.Load();

                var viewModel = new LauncherViewModel(() => Task.Run(() =>
                    ProviderContainer.GetItems(ConfigurationService.GetConfig(), new IgnoredProviderFactory("Snake"))));
                var window = new MainWindow(viewModel);
                desktop.MainWindow = window;
                ActionSelectionService.Handler = item => viewModel.ShowActions(item);

                if (this.TryGetFeature<IActivatableLifetime>() is { } activatable)
                {
                    // Clicking the Dock icon while the app runs in the background.
                    activatable.Activated += (_, e) =>
                    {
                        if (e.Kind == ActivationKind.Reopen) window.ShowLauncher();
                    };
                }

                if (OS.IsMac())
                {
                    var error = GlobalHotKey.Register('\'', window.ToggleLauncher);
                    window.HotKeyText = error ?? "⌘' opens OpenRepo from anywhere";
                }

                window.ShowLauncher();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
