using static Assets.Scripts.Atmospherics.Chemistry;

namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using HarmonyLib;

[HarmonyPatch(typeof(Combustion), nameof(Combustion.TryGetResult), new[] { typeof(GasType), typeof(GasType), typeof(CombustionResult) }, new[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
internal class TryGetResultPrefixPatch
{
    private static bool Prefix(GasType fuelType, GasType oxidiserType, out CombustionResult result, ref bool __result)
    {
        Plugin.Logger?.LogInfo($"Calling {nameof(Combustion.TryGetResult)}");
        result = CombustionResult.Invalid;
        int num1 = Combustion.FuelIndex(fuelType);
        int num2 = Combustion.OxidiserIndex(oxidiserType);
        if (num1 == -1 || num2 == -1)
        {
            __result = false;
            return false;
        }
        result = Shared.PatchedData[Combustion.FuelIndex(fuelType), Combustion.OxidiserIndex(oxidiserType)];
        __result = result.IsValid();
        return false;
    }
}
