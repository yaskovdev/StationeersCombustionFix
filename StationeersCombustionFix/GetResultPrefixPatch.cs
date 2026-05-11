using static Assets.Scripts.Atmospherics.Chemistry;

namespace StationeersCombustionFix;

using System;
using Assets.Scripts.Atmospherics;
using HarmonyLib;

[HarmonyPatch(typeof(Combustion), nameof(Combustion.GetResult))]
internal class GetResultPrefixPatch
{
    // ReSharper disable once InconsistentNaming, since Harmony relies on the argument name to change the result of the original method
    private static bool Prefix(GasType fuelType, GasType oxidiserType, ref CombustionResult __result)
    {
        var combustionResult = Shared.DataPatch[Combustion.FuelIndex(fuelType), Combustion.OxidiserIndex(oxidiserType)];
        __result = combustionResult.IsValid() ? combustionResult : throw new NotImplementedException($"Combustion result for {fuelType} {oxidiserType} is not implemented");
        return false;
    }
}
