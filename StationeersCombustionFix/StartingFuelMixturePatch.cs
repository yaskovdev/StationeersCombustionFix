namespace StationeersCombustionFix;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Entities;
using HarmonyLib;
using Trading;

internal static class StartingSpawnScope
{
    [ThreadStatic] private static int _depth;

    internal static bool IsActive => _depth > 0;

    internal static bool ShouldEnter(SpawnData spawnData) =>
        spawnData.EventType != SpawnEvent.None
        || string.Equals(spawnData.Id, "DefaultNewPlayer", StringComparison.Ordinal)
        || string.Equals(spawnData.Id, "DefaultRespawnPlayer", StringComparison.Ordinal);

    internal static void Enter()
    {
        _depth++;
    }

    internal static void Exit()
    {
        if (_depth <= 0)
        {
            Plugin.Logger?.LogError("Starting spawn scope was exited without a matching entry");
            _depth = 0;
            return;
        }
        _depth--;
    }
}

[HarmonyPatch(typeof(SpawnData), nameof(SpawnData.Execute), typeof(Thing), typeof(Human))]
internal static class StartingSpawnScopePatch
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony relies on the argument names")]
    internal static void Prefix(SpawnData __instance, out bool __state)
    {
        __state = StartingSpawnScope.ShouldEnter(__instance);
        if (__state)
        {
            StartingSpawnScope.Enter();
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony relies on the argument names")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Harmony uses the return value to propagate the original exception")]
    internal static Exception? Finalizer(Exception? __exception, bool __state)
    {
        if (__state)
        {
            StartingSpawnScope.Exit();
        }
        return __exception;
    }
}

[HarmonyPatch(typeof(ThingSpawnData), nameof(ThingSpawnData.Execute), typeof(Thing), typeof(Human))]
internal static class StartingFuelMixturePatch
{
    /// <summary>
    /// Returns whether legacy starting fuel mixtures should be corrected. Wired to the BepInEx configuration in
    /// <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchStartingFuelMixtures = () => false;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony relies on the argument names")]
    internal static void Prefix(ThingSpawnData __instance, out List<ActionData>? __state)
    {
        __state = null;
        if (!PatchStartingFuelMixtures() || !StartingSpawnScope.IsActive)
        {
            return;
        }

        var correctedActions = StartingFuelMixture.CreateCorrectedActions(__instance.Actions);
        if (correctedActions == null)
        {
            return;
        }

        __state = __instance.Actions;
        __instance.Actions = correctedActions;
        Plugin.Logger?.LogInfo($"Corrected legacy methane + oxygen mixture for starting spawn '{__instance.PrefabId}'.");
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony relies on the argument names")]
    internal static Exception? Finalizer(ThingSpawnData __instance, Exception? __exception, List<ActionData>? __state)
    {
        if (__state != null)
        {
            __instance.Actions = __state;
        }
        return __exception;
    }
}
