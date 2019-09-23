using System;
using OpenRepo.Contracts;

namespace OpenRepo.Providers.Personal
{
    public class PersonalContentProviderFactory : IProviderFactory
    {

        public string Id => "Personal";

        public IProvider GetProvider(string configuration)
        {
            return new PersonalContentProvider(configuration);
        }
    }
}
