using System;
using OpenRepo.Contracts;

namespace OpenRepo.Providers.Snake
{
    public class SnakeProviderFactory : IProviderFactory
    {
        public string Id => "Snake";

        public IProvider GetProvider(string configuration)
        {
            var canParse = int.TryParse(configuration.Trim(), out int time);
            if (!canParse || time < 5 || time > 1000) throw new Exception("Configuration of snake needs to be a number between 5 and 1000");
            return new SnakeProvider(time);
        }
    }
}
