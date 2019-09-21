using System.IO;
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
                if(m_instance == null)
                {
                    File.WriteAllText(Path.Combine(AssemblyLocationService.AssemblyDirectory, "cmd.bat"), CmdBat);
                    File.WriteAllText(Path.Combine(AssemblyLocationService.AssemblyDirectory, "cmd.sh"), CmdSh);
                    var bridgeSystem = OS.GetCurrent() == "win" ? BridgeSystem.Bat : BridgeSystem.Bash;
                    m_instance = new ShellConfigurator(bridgeSystem);
                }

                return m_instance;
            }
        }

        public static void OpenTerminal(string path)
        {
            Instance.Term("cd " + path, Output.External);
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
