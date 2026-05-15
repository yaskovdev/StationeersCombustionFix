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
        if (IsMatch(__instance.FuelMoleCount, 2.0)
            && IsMatch(__instance.OxidiserMoleCount, 1.0)
            && __instance.Outputs.Length == 2
            && IsMatch(__instance.Outputs[0], GasType.Pollutant, 3.0)
            && IsMatch(__instance.Outputs[1], GasType.CarbonDioxide, 6.0))
        {
            Plugin.Logger?.LogInfo($"{nameof(CombustionResult)} is {nameof(GasType.Pollutant)} and {nameof(GasType.CarbonDioxide)}, replacing it with {GasType.CarbonDioxide} and {GasType.Steam}");
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelMoleCount)).SetValue(__instance, new MoleQuantity(1.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserMoleCount)).SetValue(__instance, new MoleQuantity(2.0));
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.Outputs)).SetValue(__instance, new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.OxidiserRatio)).SetValue(__instance, __instance.OxidiserMoleCount / __instance.FuelMoleCount);
            AccessTools.Field(typeof(CombustionResult), nameof(CombustionResult.FuelRatio)).SetValue(__instance, __instance.FuelMoleCount / __instance.OxidiserMoleCount);
            Plugin.Logger?.LogInfo($"Replaced {nameof(CombustionResult)} with {GasType.CarbonDioxide} and {GasType.Steam}");
        }
    }

    private static bool IsMatch(CombustionValue value, GasType gasType, double quantity) => value.GasType == gasType && IsMatch(value.Quantity, quantity);

    private static bool IsMatch(MoleQuantity value, double quantity) => new MoleQuantity(quantity).Equals(value);
}
