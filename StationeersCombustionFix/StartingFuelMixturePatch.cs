namespace StationeersCombustionFix;

using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts;
using HarmonyLib;

[HarmonyPatch(typeof(SpawnData), nameof(SpawnData.Initialize), typeof(ModAbout))]
internal static class StartingFuelMixturePatch
{
    /// <summary>
    /// Returns whether vanilla spawn-data fuel mixtures should be corrected. Wired to the BepInEx configuration in
    /// <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchStartingFuelMixtures = () => false;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony relies on the argument names")]
    internal static void Prefix(SpawnData __instance)
    {
        if (!PatchStartingFuelMixtures())
        {
            return;
        }

        var correctedCount = StartingFuelMixture.CorrectSpawnTree(__instance);
        if (correctedCount > 0)
        {
            Plugin.Logger?.LogInfo($"Corrected {correctedCount} methane + oxygen mixture(s) in spawn data '{__instance.Id}'");
        }
    }
}
