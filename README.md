# Reborn-Mod-Manager

## Prereqs
* .NET framework 4.6.1
* Windows 10 (Windows 7/8 may be possible, but these have not been tested. If you would like to use these, feel free to open an [Issue!](https://github.com/shamrickus/Reborn-Mod-Manager/issues))
* Dota 2

## Installation/Usage
* Download the latest `.msi` file from the [releases page](https://github.com/shamrickus/Reborn-Mod-Manager/releases)
* Run the installer, it should check that the prereqs are installed properly.
* Run DotaInstaller, it will try to find your dota 2 location; if it cannot then it will prompt you to locate it. 
* Select the mods you want and install.
* Launch dota and the sound should be installed. If there are mods you would like to see but aren't available, open an [Issue](https://github.com/shamrickus/Reborn-Mod-Manager/issues)

## Note
* Each dota 2 patch _can_ wipe out all of the mods installed, depending on what files are updated. Therefore it may be required to rerun the tool on specific updates.



## Dev Prereqs
* Visual Studio 15+
* Python 3 (for the `Tools` proejct)
