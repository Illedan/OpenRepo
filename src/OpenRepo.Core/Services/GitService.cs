using System;
using ToolBox.Bridge;
using ToolBox.Platform;

namespace OpenRepo.Services
{
    public static class GitService
    {
        public static bool TryGetRemoteGitLocation(string path, out string uri)
        {
            var response = TerminalService.Instance.Term("git config --get remote.origin.url", Output.Hidden, path);
            var output = response.stdout.Trim();
            Uri createdUri = null;
            var isValid = !string.IsNullOrEmpty(output) && Uri.TryCreate(output, UriKind.RelativeOrAbsolute, out createdUri);
            uri = string.Empty;

            if (isValid) uri = createdUri.AbsoluteUri;
            return isValid;
        }
    }
}
