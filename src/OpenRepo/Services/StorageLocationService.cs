using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ToolBox.Platform;

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

    public static class OS
    {
        public static bool IsWin() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMac() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsGnu() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static string GetCurrent()
        {
            return
            (IsWin() ? "win" : null) ??
            (IsMac() ? "mac" : null) ??
            (IsGnu() ? "gnu" : null);
        }
    }

}
