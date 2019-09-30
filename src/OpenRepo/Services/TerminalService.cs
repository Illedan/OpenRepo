using System;
using System.IO;
using System.Reflection;
using ToolBox.Bridge;
using ToolBox.Platform;

namespace OpenRepo.Services
{
    public static class TerminalService
    {
        private static ShellConfigurator m_instance;
        public static ShellConfigurator Instance
        {
            get
            {
                if (m_instance == null)
                {
                    var bridgeSystem = OS.IsWin() ? BridgeSystem.Bat : BridgeSystem.Bash;
                    m_instance = new ShellConfigurator(bridgeSystem);
                }

                return m_instance;
            }
        }

        public static void OpenTerminal(string path)
        {
            var response = Instance.Term("cd " + path, Output.External);
            LogService.Log(response.stderr);
        }
    }
}
