namespace StationeersCombustionFix;

using System;
using System.Collections.Generic;
using Assets.Scripts.Atmospherics;
using HarmonyLib;
using static Assets.Scripts.Atmospherics.Chemistry;

internal static class CombustionResultPatch
{
    /// <summary>
    /// Returns whether the methane + ozone combustion reaction should also be patched. Wired to the BepInEx
    /// configuration in <see cref="Plugin.Awake"/> and defaults to false.
    /// </summary>
    internal static Func<bool> PatchMethaneOzoneReaction = () => false;

    internal static void PatchReactions() =>
        // Reading these fields triggers Combustion's static constructor, so the instances are guaranteed to exist.
        PatchReactions(Combustion.ResultMethaneOxygen, Combustion.ResultMethaneOzone);

    internal static void PatchReactions(CombustionResult methaneOxygen, CombustionResult methaneOzone)
    {
        // CombustionResult is a reference type, so mutating the shared, named Combustion.Result* instances propagates
        // the fix everywhere the game uses them, without having to patch every method that consumes the reactions.
        Patch(methaneOxygen, new MoleQuantity(1.0), new MoleQuantity(2.0), new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
        if (PatchMethaneOzoneReaction())
        {
            Patch(methaneOzone, new MoleQuantity(3.0), new MoleQuantity(4.0), new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
        }
    }

    private static void Patch(CombustionResult instance, MoleQuantity fuelMoleCount, MoleQuantity oxidiserMoleCount, CombustionValue[] outputs)
    {
        Plugin.Logger?.LogInfo($"Patching reaction: {instance.Format()}");
        var fieldValues = new List<(string Field, object Value)>
        {
            (nameof(CombustionResult.FuelMoleCount), fuelMoleCount),
            (nameof(CombustionResult.OxidiserMoleCount), oxidiserMoleCount),
            (nameof(CombustionResult.OxidiserRatio), oxidiserMoleCount / fuelMoleCount),
            (nameof(CombustionResult.FuelRatio), fuelMoleCount / oxidiserMoleCount),
            (nameof(CombustionResult.Outputs), outputs)
        };
        fieldValues.ForEach(it => AccessTools.Field(typeof(CombustionResult), it.Field).SetValue(instance, it.Value));
        Plugin.Logger?.LogInfo($"Patched reaction: {instance.Format()}");
    }
}
