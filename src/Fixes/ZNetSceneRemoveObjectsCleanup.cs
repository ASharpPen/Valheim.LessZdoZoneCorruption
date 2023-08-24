using HarmonyLib;

namespace LessZdoCorruption.Fixes;

internal static class ZNetSceneRemoveObjectsCleanup
{
    public static void Enable(Harmony harmony)
    {
        harmony.Patch(
            AccessTools.Method(typeof(ZNetScene), nameof(ZNetScene.RemoveObjects)),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(ZNetSceneRemoveObjectsCleanup), nameof(VerifyAndCleanInput))));
    }

    private static void Log(string message)
    {
        if (Config.ZnetScene_RemoveObject_Verbose.Value)
        {
            LessZdoCorruption.Log.Warning("[ZnetScene RemoveObjects] " + message);
        }
    }

    public static void VerifyAndCleanInput(ZNetScene __instance, ref List<ZDO> currentNearObjects, ref List<ZDO> currentDistantObjects)
    {
        if (currentNearObjects is null)
        {
            Log($"'{nameof(currentNearObjects)}' input is null. Fixing.");
            currentNearObjects = new List<ZDO>(0);
        }
        else
        {
            int nullsFixed = 0;

            for (int i = 0; i < currentNearObjects.Count; ++i)
            {
                if (currentNearObjects[i] is null)
                {
                    ++nullsFixed;

                    currentNearObjects.RemoveAt(i);
                    --i;
                }
            }

            if (nullsFixed > 0)
            {
                Log($"null values in '{nameof(currentNearObjects)}'. Cleaning {nullsFixed} nulls.");
            }
        }

        if (currentDistantObjects is null)
        {
            Log($"'{nameof(currentDistantObjects)}' input as null. Fixing.");
            currentDistantObjects = new List<ZDO>(0);
        }
        else
        {
            int nullsFixed = 0;

            for (int i = 0; i < currentDistantObjects.Count; ++i)
            {
                if (currentDistantObjects[i] is null)
                {
                    ++nullsFixed;

                    currentDistantObjects.RemoveAt(i);
                    --i;
                }
            }

            if (nullsFixed > 0)
            {
                Log($"null values in '{nameof(currentDistantObjects)}'. Cleaning {nullsFixed} nulls.");
            }
        }

        List<ZDO> brokenKeys = new List<ZDO>();
        List<KeyValuePair<ZDO, ZNetView>> brokenPairs = new List<KeyValuePair<ZDO, ZNetView>>();
        foreach (var pair in __instance.m_instances)
        {
            if (pair.Value is null)
            {
                brokenKeys.Add(pair.Key);
            }
            else if (pair.Value.GetZDO() is null)
            {
                brokenPairs.Add(pair);
            }
        }

        if (brokenKeys.Count > 0)
        {
            Log($"ZNetScene.m_instances had '{brokenKeys.Count}' zdo keys with null ZNetView values. Removing keys (Prefab, Pos):");

            var group = brokenKeys.GroupBy(x => x.GetPrefab());

            foreach (var key in brokenKeys)
            {
                try
                {
                    var prefab = ZNetScene.instance.GetPrefab(key.GetPrefab());
                    if (!prefab || prefab is null)
                    {
                        Log($"\t({key.GetPrefab()}, {key.GetPosition()})");
                    }
                    else
                    {
                        Log($"\t({prefab.name}, {key.GetPosition()})");
                    }
                }
                catch (Exception e)
                {
                    Log($"\t{key.GetPrefab()}: Error during identification of broken prefab. Key will still be removed: " + e);
                }

                __instance.m_instances.Remove(key);
            }
        }
        if (brokenPairs.Count > 0)
        {
            Log($"ZNetScene.m_instances had {brokenPairs.Count} zdo keys with znetviews that had null ZDO. Removing keys (Prefab, Pos):");

            foreach (var pair in brokenPairs)
            {
                try
                {
                    var prefab = ZNetScene.instance.GetPrefab(pair.Key.GetPrefab());
                    if (prefab == false ||
                        prefab is null)
                    {
                        Log($"\t({pair.Key.GetPrefab()}, {pair.Key.GetPosition()}");
                    }
                    else
                    {
                        Log($"\t({prefab.name}, {pair.Key.GetPosition()}");
                    }
                }
                catch
                {
                }

                __instance.m_instances.Remove(pair.Key);
            }
        }
    }
}
