using HarmonyLib;
using UnityEngine;

namespace LessZdoCorruption.Fixes;

/// <summary>
/// On serialization, ZDO's will store a byte telling how many
/// records are in the list its about to write. However, nothing
/// is checking if there are more records in that list than a byte
/// can express.
/// 
/// This can obviously cause issues because it means the entire ZDO
/// becomes mis-aligned.
/// 
/// This fix will instead block any attempt at setting values that
/// would make it overflow this limit, and report it as an error.
/// It sure isn't a great solution, but the alternative is random corruption.
/// </summary>
internal static class ZdoBlockOverflow
{
    public static void Enable(Harmony harmony)
    {
        var patchedType = typeof(ZDOExtraData);

        // Patch Update
        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Update), new[] { typeof(ZDOID), typeof(int), typeof(Vector3) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Vector3))));

        // Patch Add methods
        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(float) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Float))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(string) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_String))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(Vector3) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Vector3))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(Quaternion) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Quaternion))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(int) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Int))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(long) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Long))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Add), new[] { typeof(ZDOID), typeof(int), typeof(byte[]) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_ByteArray))));

        // Patch Set methods
        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(float) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Float))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(string) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_String))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(Vector3) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Vector3))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(Quaternion) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Quaternion))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(int) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Int))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(long) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_Long))));

        harmony.Patch(
            AccessTools.Method(patchedType, nameof(ZDOExtraData.Set), new[] { typeof(ZDOID), typeof(int), typeof(byte[]) }),
            new HarmonyMethod(AccessTools.Method(typeof(ZdoBlockOverflow), nameof(BlockOverflow_Int_ByteArray))));
    }

    private static bool CheckSpace<T, K>(
        Dictionary<ZDOID, BinarySearchDictionary<T, K>> storage,
        ZDOID zid)
        where T : IComparable<T>
    {
        if (storage is null)
        {
            return true;
        }

        if (storage.TryGetValue(zid, out var data) &&
            data.Count >= 255)
        {
            try
            {
                var zdo = ZDOMan.instance.GetZDO(zid);

                if (zdo is not null)
                {
                    var prefab = ZNetScene.instance.GetPrefab(zdo.m_prefab);

                    if (prefab != false)
                    {
                        Log.Error($"[ZdoBlockOverflow]: Attempted to add too much '{typeof(K).Name}' data to ZDO '{prefab.name}'. Blocking data from being stored.");
                    }
                    else
                    {
                        PrintFallbackError(zid);
                    }
                }
                else
                {
                    PrintFallbackError(zid);
                }
            }
            catch
            {
                PrintFallbackError(zid);
            }

            return false;
        }

        return true;

        static void PrintFallbackError(ZDOID zid)
        {
            Log.Error($"[ZdoBlockOverflow]: Attempted to add too much '{typeof(K).Name}' data to ZDOID '{zid.ID}'. Blocking data from being stored.");
        }
    }

    private static bool BlockOverflow_Int_Float(ZDOID zid) => CheckSpace(ZDOExtraData.s_floats, zid);

    private static bool BlockOverflow_Int_String(ZDOID zid) => CheckSpace(ZDOExtraData.s_strings, zid);

    private static bool BlockOverflow_Int_Vector3(ZDOID zid) => CheckSpace(ZDOExtraData.s_vec3, zid);

    private static bool BlockOverflow_Int_Quaternion(ZDOID zid) => CheckSpace(ZDOExtraData.s_quats, zid);

    private static bool BlockOverflow_Int_Int(ZDOID zid) => CheckSpace(ZDOExtraData.s_ints, zid);

    private static bool BlockOverflow_Int_Long(ZDOID zid) => CheckSpace(ZDOExtraData.s_longs, zid);

    private static bool BlockOverflow_Int_ByteArray(ZDOID zid) => CheckSpace(ZDOExtraData.s_byteArrays, zid);
}
