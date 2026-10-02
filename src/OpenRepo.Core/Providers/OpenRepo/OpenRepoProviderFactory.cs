using OpenRepo.Contracts;

namespace OpenRepo.Providers.OpenRepo
{
    public class OpenRepoProviderFactory : IProviderFactory
    {
        public string Id => "OpenRepo";

        public IProvider GetProvider(string configuration)
        {
            return new OpenRepoProvider();
        }
    }
}
