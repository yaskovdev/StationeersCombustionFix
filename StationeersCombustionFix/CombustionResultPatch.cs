namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using HarmonyLib;
using static Assets.Scripts.Atmospherics.Chemistry;

[HarmonyPatch(typeof(CombustionResult), MethodType.Constructor, typeof(double), typeof(double), typeof(CombustionValue[]))]
internal static class CombustionResultPatch
{
    private static void Postfix(CombustionResult __instance)
    {
        Plugin.Logger?.LogInfo($"Called constructor with instance: {__instance}");
        if (__instance.Outputs.Length == 2 && __instance.Outputs[0].GasType == GasType.Pollutant && __instance.Outputs[1].GasType == GasType.CarbonDioxide)
        {
            Plugin.Logger?.LogInfo("Called constructor with the instance we need to replace");
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(__instance, new MoleQuantity(1));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(__instance, new MoleQuantity(2));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(__instance, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) });
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(__instance, new MoleQuantity(2));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(__instance, new MoleQuantity(0.5));
            Plugin.Logger?.LogInfo($"DONE, {__instance.Outputs[0].GasType}");
        }
    }
}
