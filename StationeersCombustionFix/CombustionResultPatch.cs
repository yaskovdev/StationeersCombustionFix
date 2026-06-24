namespace StationeersCombustionFix;

using System;
using System.Collections.Generic;
using Assets.Scripts.Atmospherics;
using HarmonyLib;
using static Assets.Scripts.Atmospherics.Chemistry;

[HarmonyPatch(typeof(CombustionResult), MethodType.Constructor, typeof(double), typeof(double), typeof(CombustionValue[]))]
internal static class CombustionResultPatch
{
    /// <summary>
    /// Returns whether the methane + ozone combustion reaction should also be patched. Wired to the BepInEx
    /// configuration in <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchMethaneOzoneReaction = () => false;

    // ReSharper disable once InconsistentNaming, since Harmony relies on the argument name to access the original instance 
    internal static void Postfix(CombustionResult __instance)
    {
        Plugin.Logger?.LogInfo($"Constructed instance: {__instance}");
        if (__instance.FuelMoleCount.Is(2.0)
            && __instance.OxidiserMoleCount.Is(1.0)
            && __instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) }))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)} and {nameof(GasType.CarbonDioxide)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            Patch(__instance, new MoleQuantity(1.0), new MoleQuantity(2.0), new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with 1 {GasType.CarbonDioxide} and 2 {GasType.Steam}");
        }
        else if (PatchMethaneOzoneReaction()
                 && __instance.FuelMoleCount.Is(3.0)
                 && __instance.OxidiserMoleCount.Is(2.0)
                 && __instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) }))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)}, {nameof(GasType.CarbonDioxide)} and {nameof(GasType.Steam)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            Patch(__instance, new MoleQuantity(3.0), new MoleQuantity(4.0), new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with 3 {GasType.CarbonDioxide} and 6 {GasType.Steam}");
        }
    }

    private static void Patch(CombustionResult instance, MoleQuantity fuelMoleCount, MoleQuantity oxidiserMoleCount, CombustionValue[] outputs)
    {
        var fieldValues = new List<(string Field, object Value)>
        {
            (nameof(CombustionResult.FuelMoleCount), fuelMoleCount),
            (nameof(CombustionResult.OxidiserMoleCount), oxidiserMoleCount),
            (nameof(CombustionResult.OxidiserRatio), oxidiserMoleCount / fuelMoleCount),
            (nameof(CombustionResult.FuelRatio), fuelMoleCount / oxidiserMoleCount),
            (nameof(CombustionResult.Outputs), outputs)
        };
        fieldValues.ForEach(it => AccessTools.Field(typeof(CombustionResult), it.Field).SetValue(instance, it.Value));
    }
}
