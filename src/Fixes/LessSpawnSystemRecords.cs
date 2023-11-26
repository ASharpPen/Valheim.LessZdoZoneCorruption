//#define Verbose

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LessZdoCorruption.Extensions;

namespace LessZdoCorruption.Fixes;

/// <summary>
/// SpawnSystem currently stores a timestamp for all natural spawns as well as event
/// spawns that are enabled and matches the biome.
/// For modded games, this can surpass the built in limit of 256 records.
/// 
/// This fix will attempt to reduce the number of records added, by moving the logic
/// that adds the timestamp to slightly later in the workflow, so that
/// only entries that actually can spawn will record the time.
/// </summary>
[Obsolete("Deprecated. Feature is too simple and ends up causing more spawn chance checks than expected.")]
internal static class LessSpawnSystemRecords
{
    private static MethodInfo Method_ZDO_Set_IntLong = AccessTools.Method(typeof(ZDO), nameof(ZDO.Set), new[] {typeof(int), typeof(long) });
    private static MethodInfo Method_SpawnSystem_FindBaseSpawnPoint = AccessTools.Method(typeof(SpawnSystem), nameof(SpawnSystem.FindBaseSpawnPoint));
    private static MethodInfo Method_SpawnSystem_UpdateSpawnList = AccessTools.Method(typeof(SpawnSystem), nameof(SpawnSystem.UpdateSpawnList));
    private static MethodInfo Method_GetStableHash = AccessTools.Method(typeof(StringExtensionMethods), nameof(StringExtensionMethods.GetStableHashCode));

    private static MethodInfo Method_LessSpawnSystemRecords_MoveSpawnTimeCode = AccessTools.Method(typeof(LessSpawnSystemRecords), nameof(MoveSpawnTimeCode));
    private static MethodInfo Method_LessSpawnSystemRecords_SetTimestamp = AccessTools.Method(typeof(LessSpawnSystemRecords), nameof(SetTimestamp));

    private static SpawnSystem? CurrentInstance { get; set; }
    private static DateTime? CurrentTime { get; set; }

    internal static void Enable(Harmony harmony)
    {
        /*
        harmony.Patch(
            Method_SpawnSystem_UpdateSpawnList,
            prefix: new HarmonyMethod(AccessTools.Method(typeof(LessSpawnSystemRecords), nameof(Init))));

        harmony.Patch(
            Method_SpawnSystem_UpdateSpawnList,
            transpiler: new HarmonyMethod(Method_LessSpawnSystemRecords_MoveSpawnTimeCode));
        */

#if DEBUG && Verbose
        harmony.Patch(
            Method_SpawnSystem_UpdateSpawnList,
            transpiler: new HarmonyMethod(AccessTools.Method(typeof(LessSpawnSystemRecords), nameof(DebugTranspiler))));
#endif
    }

    /// <summary>
    /// Grab initial input parameters for UpdateSpawnList.
    /// 
    /// Note: We could have loaded these in the transpiler, but this way its less volatile to 
    /// code changes.
    /// </summary>
    private static void Init(SpawnSystem __instance, DateTime currentTime)
    {
        CurrentInstance = __instance;
        CurrentTime = currentTime;
    }

    private static IEnumerable<CodeInstruction> MoveSpawnTimeCode(this IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        return new CodeMatcher(instructions, generator)
            // Find register of current spawn stableHashCode for later reference
            .MatchForward(true, 
                new CodeMatch(OpCodes.Call, Method_GetStableHash),
                    new CodeMatch(OpCodes.Stloc_S))
            .GetInstruction(out var stableHashCode)
            // Move to timestamp store
            .MatchForward(true, 
            new CodeMatch(OpCodes.Ldarga_S),
                new CodeMatch(OpCodes.Call),
                new CodeMatch(OpCodes.Callvirt, Method_ZDO_Set_IntLong))
            // Remove the timestamp store
            .RemoveInstruction()
            // Clean up the stack
            .InsertAndAdvance(OpCodes.Pop) // Pop ticks
            .InsertAndAdvance(OpCodes.Pop) // Pop currentTime
            .InsertAndAdvance(OpCodes.Pop) // Pop zdo           
            // Move to before we start finding a spawn position
            .MatchForward(true, new CodeMatch(OpCodes.Call, Method_SpawnSystem_FindBaseSpawnPoint))
            // Add the timestamp assignment here instead.
            .InsertAndAdvance(stableHashCode.GetLdlocFromStLoc()) // Load "stableHashCode"
            .InsertAndAdvance(Transpilers.EmitDelegate(SetTimestamp))
            .InstructionEnumeration();
    }

    private static void SetTimestamp(int stableHashCode)
    {
        try
        {
            if (CurrentInstance != false && CurrentInstance != null &&
                CurrentTime is not null)
            {
                CurrentInstance.m_nview.GetZDO().Set(stableHashCode, CurrentTime.Value.Ticks);

#if DEBUG && Verbose
                Log.Warning("Stored time: " + CurrentTime);
#endif
            }
        }
        catch (Exception e)
        {
            Log.Error("Error while attempting to record time that entity spawned.", e);
        }
    }

#if DEBUG
    private static IEnumerable<CodeInstruction> DebugTranspiler(IEnumerable<CodeInstruction> instructions)
        => new CodeMatcher(instructions)
        // Find register of current spawn stableHashCode for later reference
        .MatchForward(true,
            new CodeMatch(OpCodes.Call, Method_GetStableHash),
                new CodeMatch(OpCodes.Stloc_S))
        .GetInstruction(out var stableHashCode)
        .Advance(1)
        .InsertAndAdvance(stableHashCode.GetLdlocFromStLoc()) // Load "stableHashCode"
        .InsertAndAdvance(OpCodes.Ldloc_3)
        .InsertAndAdvance(Transpilers.EmitDelegate(Debug))
        .InstructionEnumeration();

    private static void Debug(int stableHashCode, SpawnSystem.SpawnData entry)
    {
        if (entry is not null)
        {
            var ticks = CurrentInstance!.m_nview.GetZDO().GetLong(stableHashCode, 0L);

            if (ticks == 0)
            {
                Log.Warning($"{entry.m_prefab.name}: N/A");
            }
            else
            {
                DateTime d = new DateTime(ticks);
                Log.Warning($"{entry.m_prefab.name}: " + d);
            }
        }
    }
#endif
}
