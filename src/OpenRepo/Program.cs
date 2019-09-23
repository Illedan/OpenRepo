using System;
using System.IO;
using System.Threading.Tasks;
using OpenRepo.Services;
using OpenRepo.View;
using OpenRepo.ViewModels;

namespace OpenRepo
{
    class Program
    {
        private const string DefaultConfig = "# Welcome to OpenRepo\n# Created by https://github.com/Illedan \n# Below you can view a sample configuration if you remove the #s. \n#Local:\n#    c:myPath/\n\n#Personal:\n#    nuget https://www.nuget.org";
        private static DateTime m_lastUpdated = DateTime.MinValue;
        private static string m_configLocation;
        static Task Main()
        {
            m_configLocation = ConfigurationService.ConfigurationPath;
            CheckChanges();
            Viewer.Start();
            return new TaskCompletionSource<object>().Task;
        }

        public static async Task Reset()
        {
            LogService.Clear();
            Console.WriteLine("Loading..."); // Poor mans loading bar \o/
            var viewModel = new MainViewModel(File.ReadAllText(m_configLocation));
            await viewModel.Initialize();
            Viewer.Reset(viewModel);
        }

        private static async void CheckChanges()
        {
            while (true)
            {
                try
                {
                    if (!File.Exists(m_configLocation))
                    {
                        var dir = Path.GetDirectoryName(m_configLocation);
                        Directory.CreateDirectory(dir);
                        File.WriteAllText(m_configLocation, DefaultConfig);
                    }

                    var time = File.GetLastWriteTimeUtc(ConfigurationService.ConfigurationPath);
                    if(time != m_lastUpdated)
                    {
                        m_lastUpdated = time;
                        await Reset();
                    }

                    await Task.Delay(500);
                }
                catch(Exception e)
                {
                    LogService.Log(e.Message);
                }
            }
        }
    }
}
