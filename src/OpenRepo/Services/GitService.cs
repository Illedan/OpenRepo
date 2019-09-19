namespace OpenRepo.Services
{
    public static class GitService
    {
        public static bool TryGetRemoteGitLocation(out string uri)
        {
            // TODO: Use hidden terminal to check if git status works? Atleast: git config --get remote.origin.url
            uri = string.Empty;
            return false;
        }
    }
}
