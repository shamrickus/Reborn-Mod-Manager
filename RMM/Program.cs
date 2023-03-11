using System;
using System.Collections.Generic;
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
            [Option('l', Default = null, HelpText = "Override location of Dota.")]
            public string DotaLocation { get; set; }

            [Option(Default = false, HelpText = "Try to find VPK executable in Source SDK 2013.")]
            public bool TrySDK { get; set; }

            [Option('a', Default = false, HelpText = "Install all mods without interaction.")]
            public bool All { get; set; }

            [Option('r', Default = false, HelpText = "Remove all mods without interaction.")]
            public bool Uninstall { get; set; }
        }

        private static Steam LoadSteam()
        {
            var steam = SteamInstance.Get();
            if (!steam.LocateSteam(null))
                throw new Exception("Unable to find steam");
            steam.LoadAllSteamLibraries();
            steam.IndexAllGames();
            return steam;
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

                    var modPack = Utilities.ReadModConfig();

                    if (option.Uninstall)
                    {
                        var steam = LoadSteam();
                        var dota = steam.GetGame(AppIDs.DOTA2_ID);
                        if (dota == null || !dota.Validate())
                            throw new Exception("Unable to find dota");
                        try
                        {
                            dota.Uninstall(modPack.Name);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Validating Dota 2 files through Steam will also uninstall the mods.");
                            throw;
                        }

                        Console.WriteLine("Successfully removed dota 2 mods");
                        return;
                    }

                    var modNames = modPack.Mods.Select(mod => mod.DisplayName).Prepend(ALL);
                    HashSet<string> selectedMods;
                    if (option.All)
                        selectedMods = new HashSet<string> { ALL };
                    else
                        selectedMods = Prompt.MultiSelect("Select mods to install:", modNames).ToHashSet();
                    if (selectedMods.Contains(ALL))
                    {
                        foreach (var m in modPack.Mods)
                        {
                            m.Selected = true;
                        }
                    }
                    else
                    {
                        foreach (var mod in modPack.Mods.Where(mod => selectedMods.Contains(mod.DisplayName)))
                        {
                            mod.Selected = true;
                        }
                    }

                    if (option.DotaLocation == null)
                    {
                        var steam = LoadSteam();
                        var dota = steam.GetGame(AppIDs.DOTA2_ID);
                        if (dota == null || !dota.Validate())
                            throw new Exception("Unable to find dota");
                        if (option.TrySDK)
                        {
                            if (!dota.SDKInstall(modPack, steam))
                                throw new Exception($"Unable to install mods using source sdk");
                        }
                        else
                        {
                            var msg = dota.Install(modPack);
                            if (!string.IsNullOrEmpty(msg))
                                throw new Exception(
                                    $"Unable to install mods at \"{steam.LocateGame(AppIDs.DOTA2_ID)}\": {msg}");
                        }
                    }
                    else
                    {
                        if (option.TrySDK)
                            throw new Exception("Unable to use sdk and custom location at the same time!");
                        var dota = new Dota(option.DotaLocation);
                        if (!dota.Validate())
                            throw new Exception($"Unable to find dota at \"{option.DotaLocation}\"");
                        if (!string.IsNullOrEmpty(dota.Install(modPack)))
                            throw new Exception($"Unable to install mods at \"{option.DotaLocation}\"");
                    }
                });
        }
    }
}
