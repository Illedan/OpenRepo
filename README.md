# OpenRepo
Utility tool to navigate between local repositories

This is currently an alpha version.

To run the alpha version, do:

- :star:
- Clone
- `cd` into src
- Run `dotnet publish`
- Create Configuration.yaml in the output folder. Check subsection about config further down.
- `cd` into OpenRepo/bin/Debug/netcoreapp2.1/publish/
- `dotnet run location/OpenRepo.dll`

## Configuration.yaml
```
providerId:
    config1
    config2
```

ProviderId is the id set in the `IProviderFactory` and config1/config2/etc.. is not depending on each other. Each line is sent to a fresh new provider of that type.


## Providers

### Local

Finds all folders inside a folder.

Possible Parameters:
- `prefix:customname` Applies `customname` in front of all folder from this source. Needed if you have many equal names. (Recommended to add a divider like / at the end)
- `pt:programtype` Adds Actions to start extensions of programtype. Example is `pt:sln` which finds all solutions inside the repo. Logs if there is none, starts the first if one and gives a list to select if there are multiple.

Actions:
- `Open` Opens the folder in explorer/finder
- `Terminal` Opens the terminal at the target location 


## Contribute

Anything, anywhere, anyhow, anytime. Just do it! :heart:


## Thanks

Thanks to [Simon](https://github.com/simonkaspersen) for Mac OS terminal assistance.
