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
        static async Task Main()
        {
            Viewer.Start();
            Reset();
            await new TaskCompletionSource<object>().Task;
        }

        public static async void Reset()
        {
            try
            {
                LogService.Clear();
                Console.WriteLine("Loading..."); // Poor mans loading bar \o/
                var viewModel = new MainViewModel(ConfigurationService.GetConfig());
                await viewModel.Initialize();
                Viewer.Reset(viewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                LogService.Log(e.Message);
            }
        }
    }
}
