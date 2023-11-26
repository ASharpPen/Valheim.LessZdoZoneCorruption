using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LessZdoCorruption.Fixes;
using Cfg = LessZdoCorruption.Config;

namespace LessZdoCorruption;

[BepInPlugin(ModId, PluginName, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string ModId = "LessZdoCorruption";
    public const string PluginName = "Less ZDO Corruption";
    public const string Version = "1.0.1";

    private void Awake()
    {
        Log.Logger = Logger;
        Cfg.Init(Config);

        var harmony = new Harmony(ModId);

        Toggle(Cfg.RandEvent_Cleanup, RandEventSpawnSystemCleanup.Enable, nameof(RandEventSpawnSystemCleanup));
        Toggle(Cfg.Zdo_BlockOverflow, ZdoBlockOverflow.Enable, nameof(ZdoBlockOverflow));
        Toggle(Cfg.ZnetScene_RemoveObject_Cleanup, ZNetSceneRemoveObjectsCleanup.Enable, nameof(ZNetSceneRemoveObjectsCleanup));

        void Toggle(ConfigEntry<bool> cfg, Action<Harmony> fix, string name)
        {
            if (cfg.Value)
            {
                try
                {
                    fix(harmony);
                }
                catch (Exception e)
                {
                    Log.Warning($"Failed to apply fix '{name}'. Skipping fix.", e);
                }
            }
        }
    }
}
