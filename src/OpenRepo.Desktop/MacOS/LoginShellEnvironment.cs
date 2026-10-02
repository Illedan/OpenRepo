using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRepo.Desktop.MacOS
{
    /// <summary>
    /// Apps opened from the Dock or Finder get a minimal PATH, without Homebrew or other tools that
    /// scripts in the config may use. This copies PATH from the user's shell, as a new terminal would get it.
    /// </summary>
    internal static class LoginShellEnvironment
    {
        private const string Marker = "__OPENREPO_PATH__=";

        public static Task Loaded { get; private set; } = Task.CompletedTask;

        public static void Load() => Loaded = Task.Run(LoadPath);

        private static void LoadPath()
        {
            try
            {
                var startInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("SHELL") ?? "/bin/zsh")
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                foreach (var argument in new[] { "-l", "-i", "-c", $"printf '\\n{Marker}%s\\n' \"$PATH\"" })
                {
                    startInfo.ArgumentList.Add(argument);
                }

                using var shell = Process.Start(startInfo);
                shell.StandardInput.Close();
                shell.ErrorDataReceived += (_, _) => { };
                shell.BeginErrorReadLine();
                var output = shell.StandardOutput.ReadToEndAsync();
                if (!shell.WaitForExit(5000) || !output.Wait(1000))
                {
                    shell.Kill(true);
                    return;
                }

                var path = output.Result.Split('\n').FirstOrDefault(l => l.StartsWith(Marker))?.Substring(Marker.Length).Trim();
                if (!string.IsNullOrEmpty(path)) Environment.SetEnvironmentVariable("PATH", path);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Could not read PATH from the login shell: " + e.Message);
            }
        }
    }
}
