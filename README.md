# OpenRepo
Utility tool to navigate between local repositories


![nuget](https://img.shields.io/nuget/v/OpenRepo)

## Installation

- Run `dotnet tool install --global OpenRepo`
- Use the command `openrepo`
- Press Enter to edit config for your needs. For example add:
```
Local:
    /Users/username/Projects/
```
- Save and go back to the terminal to continue usage.

## Run from project

- Clone
- `cd` into src
- Run `dotnet publish`
- `cd` into OpenRepo/bin/Debug/netcoreapp2.1/publish/
- `dotnet run OpenRepo.dll`

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

ProviderId is the id set in the `IProviderFactory` and config1/config2/etc.. is not depending on each other. Each line is sent to a fresh new provider of that type.


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
- `ptt:programtype` Adds Actions to start extensions of programtype. Example is `pt:sln` which finds all solutions inside the repo on the root level. Logs if there is none, starts the first if one and gives a list to select if there are multiple.

Actions:
- `Open` Opens the folder in explorer/finder
- `Terminal` Opens the terminal at the target location 
_ `Web` Opens a browser to the remote location of the repository. Only shows if this is a git repository.

### Personal

Here you can add a key + a value to link to. As the example here:
```
Personal:
    nuget https://www.nuget.org
    cool_user https://www.github.com/Illedan
```
Nuget would send the user to nuget.org and cool_user to Illedan's github profile.
This could also be folders in your file system, script, whatever you like :) 


## Contribute

Anything, anywhere, anyhow, anytime. Just do it! :heart:


## Thanks

Thanks to [Simon](https://github.com/simonkaspersen) for Mac OS terminal assistance.
