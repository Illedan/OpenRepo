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

                   // File.WriteAllText(Path.Combine(AssemblyDirectory, "cmd.bat"), CmdBat);
                   // var shPath = Path.Combine(AssemblyDirectory, "cmd.sh");
                   // File.WriteAllText(shPath, CmdSh);
                   // m_instance.Term("chmod +x " + shPath, Output.Hidden);
                }

                return m_instance;
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static void OpenTerminal(string path)
        {
            Instance.Term("cd " + path, Output.External);
        }

        private static string GetFileName()
        {
            if (OS.IsWin()) return "cmd.exe";
            return "/bin/bash";
        }

        private const string CmdBat = @"
echo off

set cmd=%1
set dir=%2

if defined dir (
    cd /d %dir%\
)
start cmd.exe /K %cmd%
cls

exit 0
";

        private const string CmdSh = @"
#!/bin/bash
cmd=""$1"";
dir=""$2"";

if [ -n ""$dir"" ]; then

osascript <<EOF
    tell application ""Terminal"" to do script ""cd $dir; $cmd""
EOF

else

osascript <<EOF
    tell application ""Terminal"" to do script ""$cmd""
EOF

fi
clear

exit 0
";
    }
}
