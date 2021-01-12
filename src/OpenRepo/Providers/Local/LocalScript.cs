using System;
namespace Illedan.OpenRepo.Providers.Local
{
    public class LocalScript
    {
        public LocalScript(string line)
        {
            var splitterIndex = line.IndexOf(':');
            if (splitterIndex == -1)
            {
                Key = line;
                Value = "";
                return;
            }

            Key = line.Substring(0, splitterIndex);
            Value = line.Substring(splitterIndex + 1);
        }

        public string Key { get; }
        public string Value { get; }
    }
}
