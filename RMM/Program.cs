using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Core;
using Sharprompt;

namespace CLI
{
    class Program
    {
        private const string ALL = "All";

        public class Options
        {
            [Option('l', Default = null, HelpText="Override location of Dota.")]
            public string DotaLocation { get; set; }
            
            [Option(Default = false, HelpText="Try to find VPK executable in Source SDK 2013.")]
            public bool TrySDK { get; set; }
            
            [Option('a', Default=false, HelpText="Install all mods without interaction.")]
            public bool All { get; set; }
        }
        
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(option =>
                {
                    var updater = new Updater();
                    if (updater.CheckForUpdate())
                    {
                        Console.WriteLine("Update available!");
                        Console.WriteLine(updater.GetChangeLog());
                    }
                    var modPack = Utilities.ReadModConfig(Path.Join(Utilities.AssemblyDirectory(), "config.xml"));
                    var modNames = modPack.Mods.Select(mod => mod.DisplayName).Prepend(ALL);
                    HashSet<string> selectedMods;
                    if (option.All)
                        selectedMods = new HashSet<string>(new[] {ALL});
                    else
                        selectedMods = Prompt.MultiSelect("Select mods to install:", modNames).ToHashSet();
                    if (selectedMods.Contains(ALL)) {
                        foreach (var m in modPack.Mods) { 
                            m.Selected = true;
                        }
                    }
                    else {
                        foreach (var mod in modPack.Mods.Where(mod => selectedMods.Contains(mod.DisplayName))) {
                            mod.Selected = true;
                        }
                    }
                    if (option.DotaLocation == null)
                    {
                        var steam = SteamInstance.Get();

                        steam.IndexAllGames();
                        var dota = new Dota(steam.LocateGame(AppIDs.DOTA2_ID));
                        if (!dota.Validate())
                            throw new Exception("Unable to find dota");
                        if (option.TrySDK) {
                            if (!dota.SDKInstall(modPack, steam))
                                throw new Exception($"Unable to install mods using source sdk");
                        }
                        else {
                            if (!dota.Install(modPack))
                                throw new Exception($"Unable to install mods at \"{steam.LocateGame(AppIDs.DOTA2_ID)}\"");
                        }
                    }
                    else
                    {
                        if (option.TrySDK) 
                            throw new Exception("Unable to use sdk and custom location at the same time!");
                        var dota = new Dota(option.DotaLocation);
                        if (!dota.Validate())
                            throw new Exception($"Unable to find dota at \"{option.DotaLocation}\"");
                        if (!dota.Install(modPack))
                            throw new Exception($"Unable to install mods at \"{option.DotaLocation}\"");
                    }
                });
        }
    }
}
