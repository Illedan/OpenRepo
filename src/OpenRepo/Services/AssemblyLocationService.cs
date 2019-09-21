using System;
using System.IO;
using System.Reflection;
using ToolBox.Platform;

namespace OpenRepo.Services
{
    public static class AssemblyLocationService
    {
        public static string AssemblyDirectory
        {
            get
            {
                if(OS.GetCurrent() != "win")
                {
                    var username = Environment.GetEnvironmentVariable("USERNAME") ??
                        Environment.GetEnvironmentVariable("USER");
                    return @"/Users/"+ username +"/Library/Application Support/OpenRepo/";
                }
                else
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }
               // string codeBase = Assembly.GetExecutingAssembly().CodeBase;
               // UriBuilder uri = new UriBuilder(codeBase);
               // string path = Uri.UnescapeDataString(uri.Path);
               // return Path.GetDirectoryName(path);
            }
        }
    }
}
