# OpenRepo
Terminal based utility tool to navigate between local repositories.
Blog about it: https://kvanli.com/stories/openrepo


![badge](https://action-badges.now.sh/Illedan/Openrepo)

![Nuget](https://img.shields.io/nuget/v/Illedan.openrepo?color=%2300aa00&label=Illedan.OpenRepo)

## Installation

Requires the [.NET SDK](https://dotnet.microsoft.com/download) 8.0 or newer.

- Run `dotnet tool install --global Illedan.OpenRepo`
- Use the command `openrepo`
- Press Enter to edit config for your needs. For example add:
```
Local:
    /Users/username/Projects/
```
- Save and go back to the terminal to continue usage.

## Run from project

- Clone
- From the repository root, run `dotnet run --project src/OpenRepo`

## Desktop app (macOS)

A launcher window with the same search, actions and config as the terminal version, that you can keep in the Dock and open from anywhere with `⌘'`.

- From the repository root, run `./build-mac-app.sh --install`. It builds `OpenRepo.app`, copies it to `/Applications` and opens it.
- Press `⌘'` in any app to show or hide OpenRepo. It uses the key that types `'` on your keyboard layout.
- Right-click the Dock icon and choose Options → Keep in Dock, and Options → Open at Login so the shortcut works after a restart.

Closing the window keeps OpenRepo running in the background for the shortcut, quit it with `⌘Q`. Keys: arrows to select, enter to open, the highlighted letter to pick an action, esc to go back or hide, tab to reload the config. Snake is only in the terminal version.

The app is signed for use on the Mac that builds it, not notarized for sharing. To run it without building the app: `dotnet run --project src/OpenRepo.Desktop`

## Usages

Write anything to filter your list of choices (only availiable at the start screen).
Use arrows up/down to find your wanted choice, then press enter.

You will then get a list of possible actions, which is given by the provider returning this choice, use arrows to select and enter to use this action on the current choice. See Providers futher down.

## Configuration.yaml
Example:
```
# Welcome to OpenRepo
# Created by https://github.com/Illedan 
# Add this path. Where C:/Repos/ is replaced with your repo location
Local:
    C:/Repos/ pt:sln pt:bat

Personal:
    nuget https://www.nuget.org
```

| Paths with spaces are only supported if you wrap the whole path in quotes! |
| --- |


## Providers

### Local

Example:
```
Local:
    C:/repos prefix:repos/ pt:sln ptt:md
    /Users/Illedan/Projects/ prefix:projects/
```

Finds all folders inside a folder.

Possible Parameters:
- `prefix:customname` Applies `customname` in front of all folder from this source. Needed if you have many equal names. (Recommended to add a divider like / at the end)
- `pt:programtype` Adds Actions to start extensions of programtype. Example is `pt:sln` which finds all solutions inside the repo. Logs if there is none, starts the first if one and gives a list to select if there are multiple.
- `ptt:programtype` Adds Actions to start extensions of programtype. Example is `ptt:md` which finds all solutions inside the repo on the root level. Logs if there is none, starts the first if one and gives a list to select if there are multiple.

Actions:
- `Open` Opens the folder in explorer/finder
- `Terminal` Opens the terminal at the target location
- `Web` Opens a browser to the remote location of the repository. Only shows if this is a git repository.

### Personal

Here you can add a key + a value to link to. As the example here:
```
Personal:
    nuget https://www.nuget.org
    cool_user https://www.github.com/Illedan
```
Nuget would send the user to nuget.org and cool_user to Illedan's github profile.
This could also be folders in your file system, script, whatever you like :) 

### Settings

Setup configuration for open repo. For now the only possible action is to ignore folders when finding files of certain types. An example is to ignore `node_modules` as it is a very big folder.
```
Settings:
    ignore bin
    ignore node_modules
```

### Clipboard

Copies the last phrase of the text to your clipboard.
```
Clipboard:
    DIS_ID 12345
```

## Running on Windows with AutoHotkey
One approach to working with openrepo on a windows machine is to use an AutoHotkey script to fire it and if it is already running, just bring it in to view.

First, you will need to install AutoHotkey https://www.autohotkey.com/.

Go to your desktop and right click, new, AutoHotkey Script.

```
F1::
IfWinExist C:\Users\<USER>\.dotnet\tools\OpenRepo.exe
{
  WinActivate, C:\Users\<USER>\.dotnet\tools\OpenRepo.exe
}
else
{
    run, OpenRepo
    WinActivate, C:\Users\<USER>I\.dotnet\tools\OpenRepo.exe
}
```
This script will trigger when the ```F1``` key is pressed

Finally run the AutoHotkey script by double clicking it. 

Openrepo should now come in to view when pressing F1


## Contribute

Anything, anywhere, anyhow, anytime. Just do it! :heart:


## Thanks

Thanks to [Simon](https://github.com/simonkaspersen) for Mac OS terminal assistance.
