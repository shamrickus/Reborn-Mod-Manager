#Reborn-Mod-Manager

## Prereqs
* Windows 10+/Linux
* .NET 7.0 (semi-optional, see Installation)
* Steam & Dota 2

## Installation

* Download the latest version from the [releases page](https://github.com/shamrickus/Reborn-Mod-Manager/releases)
  * Some versions contain both self-contained and regular binaries. The self-contained version packages the .NET 7 
  runtime so that it does not need to be installed on the host system. These versions are usually much larger as a result.
    * self-contained versions end with `{RELEASE}_Big`
  * ### Windows 10+ x64
    * Reborn Mod Manager (GUI)
    * RMM (CLI)
  * ### Linux x64
    * RMM (CLI)
    
* Run RMM.
* Select the mods you want and install.
* Launch dota and the sound should be installed. If there are mods you would like to see but aren't available, open an [Issue](https://github.com/shamrickus/Reborn-Mod-Manager/issues)

## Note
* Each dota 2 patch _can_ wipe out all of the mods installed, depending on what files are updated. Therefore it may be required to rerun the tool on specific updates.


## Dev Notes
* .NET 7 SDK

# Building 
- Run `dotnet restore`
- Run `dotnet build`
- Run `dotnet run -project {PROJECT}`

