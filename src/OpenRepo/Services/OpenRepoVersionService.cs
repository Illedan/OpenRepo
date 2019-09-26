using System;
using System.Linq;
namespace OpenRepo.Services
{
    public static class OpenRepoVersionService
    {
        public static string GetCurrentVersion()
        {
            var tools = TerminalService.Instance.Term("dotnet tool list -g", ToolBox.Bridge.Output.Hidden).stdout.Split("\n");
            var openRepoTool = tools.FirstOrDefault(t => t.ToLower().StartsWith("openrepo", StringComparison.CurrentCulture));

            if (openRepoTool == null) return "N/A";

            return openRepoTool.Split().FirstOrDefault(t => t.Any(l => char.IsNumber(l))) ?? "N/A";
        }
    }
}
