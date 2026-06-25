namespace StationeersCombustionFix;

using System;
using System.Collections.Generic;
using Assets.Scripts.Atmospherics;
using HarmonyLib;
using static Assets.Scripts.Atmospherics.Chemistry;

[HarmonyPatch(typeof(Combustion), MethodType.StaticConstructor)]
internal static class CombustionResultPatch
{
    /// <summary>
    /// Returns whether the methane + ozone combustion reaction should also be patched. Wired to the BepInEx
    /// configuration in <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchMethaneOzoneReaction = () => false;

    internal static void Postfix()
    {
        // Combustion.Result* are static readonly references to CombustionResult instances. Since CombustionResult
        // is a reference type, mutating the fields of these shared instances propagates the fix everywhere the game
        // uses them, without having to patch every method that consumes the reactions.
        PatchIfMatches(Combustion.ResultMethaneOxygen);
        PatchIfMatches(Combustion.ResultMethaneOzone);
    }

    internal static void PatchIfMatches(CombustionResult instance)
    {
        Plugin.Logger?.LogInfo($"Inspecting reaction: {instance.Format()}");
        if (instance.FuelMoleCount.Is(2.0)
            && instance.OxidiserMoleCount.Is(1.0)
            && instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) }))
        {
            Plugin.Logger?.LogInfo($"Matched methane + oxygen, replacing {instance.Format()}");
            Patch(instance, new MoleQuantity(1.0), new MoleQuantity(2.0), new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
            Plugin.Logger?.LogInfo($"Replaced with {instance.Format()}");
        }
        else if (PatchMethaneOzoneReaction()
                 && instance.FuelMoleCount.Is(3.0)
                 && instance.OxidiserMoleCount.Is(2.0)
                 && instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) }))
        {
            Plugin.Logger?.LogInfo($"Matched methane + ozone, replacing {instance.Format()}");
            Patch(instance, new MoleQuantity(3.0), new MoleQuantity(4.0), new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
            Plugin.Logger?.LogInfo($"Replaced with {instance.Format()}");
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
