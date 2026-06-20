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
        if (__instance.FuelMoleCount.Matches(2.0)
            && __instance.OxidiserMoleCount.Matches(1.0)
            && __instance.Outputs.Length == 2
            && __instance.Outputs[0].Matches(GasType.Pollutant, 3.0)
            && __instance.Outputs[1].Matches(GasType.CarbonDioxide, 6.0))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)} and {nameof(GasType.CarbonDioxide)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(__instance, new MoleQuantity(1.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(__instance, new MoleQuantity(2.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(__instance, new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(__instance, __instance.OxidiserMoleCount / __instance.FuelMoleCount);
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(__instance, __instance.FuelMoleCount / __instance.OxidiserMoleCount);
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with {GasType.CarbonDioxide} and {GasType.Steam}");
        }
        else if (__instance.FuelMoleCount.Matches(3.0)
                 && __instance.OxidiserMoleCount.Matches(2.0)
                 && __instance.Outputs.Length == 3
                 && __instance.Outputs[0].Matches(GasType.Pollutant, 3.0)
                 && __instance.Outputs[1].Matches(GasType.CarbonDioxide, 6.0)
                 && __instance.Outputs[2].Matches(GasType.Steam, 1.0))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)}, {nameof(GasType.CarbonDioxide)} and {nameof(GasType.Steam)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(__instance, new MoleQuantity(3.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(__instance, new MoleQuantity(4.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(__instance, new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(__instance, __instance.OxidiserMoleCount / __instance.FuelMoleCount);
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(__instance, __instance.FuelMoleCount / __instance.OxidiserMoleCount);
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with {GasType.CarbonDioxide} and {GasType.Steam}");
        }
    }
}
