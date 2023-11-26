using BepInEx.Configuration;

namespace LessZdoCorruption;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal static class Config
{
    private static ConfigFile? File { get; set; }

    internal static void Init(ConfigFile file)
    {
        File = file;

        RandEvent_Cleanup = file.Bind(
            "RandEventSystem - Cleanup",
            "Enabled",
            true,
            "Toggles fix for events (eg., raids and boss fights) that tells the\n" +
            "current zone-owner to clean up the data stored in the zone about that raid\n" +
            "when that raid ends.\n" +
            "Intended for avoiding storage of too many timestamps of things spawned.\n" +
            "Restart required to take effect.");

        Zdo_BlockOverflow = file.Bind(
            "Zdo - Block Overflow",
            "Enabled",
            true,
            "Toggles blocking of adding too much data to ZDO's.\n" +
            "For instance, if you had 400 creatures attempting to spawn, this would\n" +
            "currently cause world corruption. This fix denies the game from storing\n" +
            "data in ZDO's when above 255 entries.\n" +
            "Note that this probably has a performance cost, and the blocking\n" +
            "itself might cause unknown side-effects due to entries not being stored.\n" +
            "Restart required to take effect.");

        ZnetScene_RemoveObject_Cleanup = file.Bind(
            "ZNetScene.RemoveObjects",
            "Enabled",
            true,
            "Toggles cleanup code for the dreaded \"ZNetScene.RemoveObjects\" error.\n" +
            "This fix will attempt to identify and handle the error so that it doesn't\n" +
            "end up spamming endlessly in the logs.\n" +
            "Restart required to take effect.");

        ZnetScene_RemoveObject_Verbose = file.Bind(
            "ZNetScene.RemoveObjects",
            "Verbose",
            true,
            "Toggles logging of debugging info when problems are detected.\n" +
            "Attempts will be made to help identify the cause.\n" +
            "Restart not necessary.");
    }

    public static ConfigEntry<bool> RandEvent_Cleanup { get; set; }

    public static ConfigEntry<bool> Zdo_BlockOverflow { get; set; }

    public static ConfigEntry<bool> ZnetScene_RemoveObject_Cleanup { get; set; }

    public static ConfigEntry<bool> ZnetScene_RemoveObject_Verbose { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
