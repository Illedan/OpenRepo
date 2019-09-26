using System;
using System.Linq;
namespace OpenRepo.Services
{
    public static class OpenRepoVersionService
    {
        private static string m_cachedVersion = GetCurrentVersionInternal();
        public static string GetCurrentVersion()
        {
            return m_cachedVersion;
        }

        private static string GetCurrentVersionInternal()
        {
            var tools = TerminalService.Instance.Term("dotnet tool list -g", ToolBox.Bridge.Output.Hidden).stdout.Split("\n");
            var openRepoTool = tools.FirstOrDefault(t => t.ToLower().StartsWith("openrepo", StringComparison.CurrentCulture));

            if (openRepoTool == null) return "N/A";

            return openRepoTool.Split().FirstOrDefault(t => t.Any(l => char.IsNumber(l))) ?? "N/A";
        }
    }
}
