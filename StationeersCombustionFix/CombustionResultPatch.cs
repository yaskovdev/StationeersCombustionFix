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
    /// Returns whether the methane + nitrous oxide combustion reaction should also be patched. Wired to the BepInEx
    /// configuration in <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchMethaneNitrousReaction = () => false;

    /// <summary>
    /// Returns whether the methane + ozone combustion reaction should also be patched. Wired to the BepInEx
    /// configuration in <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchMethaneOzoneReaction = () => false;

    // ReSharper disable once InconsistentNaming, since Harmony relies on the argument name to access the original instance 
    internal static void Postfix(CombustionResult __instance)
    {
        Plugin.Logger?.LogInfo($"Constructed instance: {__instance.Format()}");
        if (__instance.FuelMoleCount.Is(2.0)
            && __instance.OxidiserMoleCount.Is(1.0)
            && __instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) }))
        {
            Plugin.Logger?.LogInfo($"Matched methane + oxygen, replacing {__instance.Format()}");
            Patch(__instance, new MoleQuantity(1.0), new MoleQuantity(2.0), new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
            Plugin.Logger?.LogInfo($"Replaced with {__instance.Format()}");
        }
        else if (PatchMethaneNitrousReaction()
                 && __instance.FuelMoleCount.Is(1.0)
                 && __instance.OxidiserMoleCount.Is(1.0)
                 && __instance.Outputs.Is(new CombustionValue[] { new(GasType.CarbonDioxide, 2.0), new(GasType.Nitrogen, 2.0) }))
        {
            Plugin.Logger?.LogInfo($"Matched methane + nitrous oxide, replacing {__instance.Format()}");
            Patch(__instance, new MoleQuantity(1.0), new MoleQuantity(4.0), new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0), new(GasType.Nitrogen, 4.0) });
            Plugin.Logger?.LogInfo($"Replaced with {__instance.Format()}");
        }
        else if (PatchMethaneOzoneReaction()
                 && __instance.FuelMoleCount.Is(3.0)
                 && __instance.OxidiserMoleCount.Is(2.0)
                 && __instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) }))
        {
            Plugin.Logger?.LogInfo($"Matched methane + ozone, replacing {__instance.Format()}");
            Patch(__instance, new MoleQuantity(3.0), new MoleQuantity(4.0), new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
            Plugin.Logger?.LogInfo($"Replaced with {__instance.Format()}");
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
