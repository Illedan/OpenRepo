using System;
using System.IO;
using System.Threading.Tasks;
using OpenRepo.View;
using OpenRepo.ViewModels;

namespace OpenRepo
{
    class Program
    {
        private const string ConfigLocation = "Configuration.yaml";
        static async Task Main()
        {
            Console.CursorVisible = false;
            if (!File.Exists(ConfigLocation))
            {
                //TODO: Needs to be generated if not found to prevent bugz.
                Console.WriteLine("Please create a Configuration.yaml in the directory of your program.");
                return;
            }

            Console.WriteLine("Loading..."); //TODO: Simulate
            var viewModel = new MainViewModel(File.ReadAllText(ConfigLocation));
            await viewModel.Initialize();
            Viewer.Push(viewModel);
            Viewer.Start();
        }
    }
}
