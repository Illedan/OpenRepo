using System;
namespace OpenRepo.Contracts
{
    public interface IProviderFactory
    {
        string Id { get; }

        IProvider GetProvider(string configuration);
    }
}
