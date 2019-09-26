using System;
using System.Linq;

namespace OpenRepo.Services
{
    public static class LineSplitterService
    {
        private const char Splitter = (char)8; // Not possible to write "backspace".
        /// <summary>
        /// Splits a line on spaces and creates sections if any " exists
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string[] SplitPath(this string line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) return new string[0];
  
            if (line.Count(l => l == '"') % 2 == 1) throw new Exception("Invalid number of quotes in the paths. If you need to have an odd number, please create an issue and explain why.");

            var chars = line.ToCharArray();
            var betweenQuotes = false;
            for(var i = 0; i < line.Length; i++)
            {
                if(line[i] == '"')
                {
                    betweenQuotes = !betweenQuotes;
                    continue;
                }
                if (line[i] != ' ' || !betweenQuotes) continue;

                chars[i] = Splitter;
            }

            line = string.Join("", chars).Replace("\"", "");
            return line.Split().Select(l => l.Replace(Splitter, ' ').Trim()).Where(l => !string.IsNullOrEmpty(l)).ToArray();
        }
    }
}
