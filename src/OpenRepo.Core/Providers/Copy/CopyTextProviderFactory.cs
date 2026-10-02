using OpenRepo.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Illedan.OpenRepo.Providers.Copy
{
    public class CopyTextProviderFactory : IProviderFactory
    {
        public string Id => "Clipboard";

        public IProvider GetProvider(string configuration)
        {
            return new CopyTextProvider(configuration);
        }
    }
}
