namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using HarmonyLib;
using static Assets.Scripts.Atmospherics.Chemistry;

[HarmonyPatch(typeof(CombustionResult), MethodType.Constructor, typeof(double), typeof(double), typeof(CombustionValue[]))]
internal static class CombustionResultPatch
{
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
        else if (__instance.FuelMoleCount.Is(3.0)
                 && __instance.OxidiserMoleCount.Is(2.0)
                 && __instance.Outputs.Is(new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) }))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)}, {nameof(GasType.CarbonDioxide)} and {nameof(GasType.Steam)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            Patch(__instance, new MoleQuantity(3.0), new MoleQuantity(4.0), new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with 3 {GasType.CarbonDioxide} and 6 {GasType.Steam}");
        }
    }

    private static void Patch(CombustionResult combustionResult, MoleQuantity fuelMoleCount, MoleQuantity oxidiserMoleCount, CombustionValue[] outputs)
    {
        AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(combustionResult, fuelMoleCount);
        AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(combustionResult, oxidiserMoleCount);
        AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(combustionResult, combustionResult.OxidiserMoleCount / combustionResult.FuelMoleCount);
        AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(combustionResult, combustionResult.FuelMoleCount / combustionResult.OxidiserMoleCount);
        AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(combustionResult, outputs);
    }
}
