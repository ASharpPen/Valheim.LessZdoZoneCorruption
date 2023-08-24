using BepInEx.Configuration;

namespace LessZdoCorruption;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal static class Config
{
    private static ConfigFile? File { get; set; }

    internal static void Init(ConfigFile file)
    {
        File = file;

        SpawnSystem_LessRecords = file.Bind(
            "SpawnSystem", 
            "Toggle Less Records", 
            true, 
            "Toggles fix for SpawnSystem (the handler for natural- and raid spawning) that moves \n" +
            "storing timestamps in the zone's ZDO to after check for spawning allowed.\n" +
            "This is intended for avoiding storage of a large number of timestamps for things \n" +
            "that has no chance of spawning, and could otherwise overload the max 256 entry\n" +
            "limit in modded games.\n" +
            "Restart required to take effect.");
    }

    public static ConfigEntry<bool> SpawnSystem_LessRecords { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
