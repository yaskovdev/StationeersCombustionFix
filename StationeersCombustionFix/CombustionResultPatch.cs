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
        if (MoleQuantityExtensions.Equals(__instance.FuelMoleCount, 2.0)
            && MoleQuantityExtensions.Equals(__instance.OxidiserMoleCount, 1.0)
            && CombustionValueExtensions.Equals(__instance.Outputs, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) }))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)} and {nameof(GasType.CarbonDioxide)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(__instance, new MoleQuantity(1.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(__instance, new MoleQuantity(2.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(__instance, new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(__instance, __instance.OxidiserMoleCount / __instance.FuelMoleCount);
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(__instance, __instance.FuelMoleCount / __instance.OxidiserMoleCount);
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with 1 {GasType.CarbonDioxide} and 2 {GasType.Steam}");
        }
        else if (MoleQuantityExtensions.Equals(__instance.FuelMoleCount, 3.0)
                 && MoleQuantityExtensions.Equals(__instance.OxidiserMoleCount, 2.0)
                 && CombustionValueExtensions.Equals(__instance.Outputs, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) }))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)}, {nameof(GasType.CarbonDioxide)} and {nameof(GasType.Steam)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(__instance, new MoleQuantity(3.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(__instance, new MoleQuantity(4.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(__instance, new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(__instance, __instance.OxidiserMoleCount / __instance.FuelMoleCount);
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(__instance, __instance.FuelMoleCount / __instance.OxidiserMoleCount);
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with 3 {GasType.CarbonDioxide} and 6 {GasType.Steam}");
        }
    }
}
