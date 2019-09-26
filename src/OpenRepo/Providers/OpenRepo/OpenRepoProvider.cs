using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Web;
using OpenRepo.Contracts;
using OpenRepo.Services;

namespace OpenRepo.Providers.OpenRepo
{
    public class OpenRepoProvider : IProvider
    {
        public Task<ConcurrentBag<SelectableItem>> GetItems()
        {
            return Task.FromResult(new ConcurrentBag<SelectableItem>
            {
                new SelectableItem("OpenRepo - Edit config",
                    () => new SelectableAction[]
                    {
                        new SelectableAction("Edit", () => StartProgramService.StartProgram(ConfigurationService.ConfigurationPath))
                    }),
                new SelectableItem("OpenRepo - Create issue",
                    () => new SelectableAction[]
                    {
                        new SelectableAction("Open", () => StartProgramService.StartProgram("https://github.com/Illedan/openrepo/issues/new?title=Unhandled+Exception+in+Version:+"+
                            OpenRepoVersionService.GetCurrentVersion()+
                            "&body="+HttpUtility.UrlEncode(LogService.Message)))
                    }),
				new SelectableItem("OpenRepo - Update",
					() => new SelectableAction[]
					{
						new SelectableAction("Open", () => TerminalService.Instance.Term("dotnet tool update openrepo -g", ToolBox.Bridge.Output.Hidden))
					}),
			});
        }
    }
}
