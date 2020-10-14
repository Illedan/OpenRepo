using System;
using System.IO;
namespace OpenRepo.Services
{
    public static class StoragelocationService
    {
        public static string StorageDirectory
        {
            get
            {
                if(OS.IsMac())
                {
                    var username = Environment.GetEnvironmentVariable("USERNAME") ??
                        Environment.GetEnvironmentVariable("USER");
                    return $"/Users/{username}/Library/Application Support/OpenRepo/";
                }

                if (OS.IsGnu())
                {
                    return "/etc/OpenRepo/";
                }

                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenRepo");
            }
        }
    }
}
