using BepInEx;
using HarmonyLib;
using LessZdoCorruption.Fixes;
using Cfg = LessZdoCorruption.Config;

namespace LessZdoCorruption;

[BepInPlugin(ModId, PluginName, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string ModId = "LessZdoCorruption";
    public const string PluginName = "Less ZDO Corruption";
    public const string Version = "0.0.1";

    private void Awake()
    {
        Log.Logger = Logger;
        Cfg.Init(Config);

        var harmony = new Harmony(ModId);

        if (Cfg.SpawnSystem_LessRecords.Value)
        {
            try
            {
                LessSpawnSystemRecords.EnableFix(harmony);
            }
            catch (Exception e)
            {
                Log.Warning($"Failed to apply fix '{nameof(LessSpawnSystemRecords)}'. Skipping fix.", e);
            }
        }
    }
}
