using static Assets.Scripts.Atmospherics.Chemistry;

namespace StationeersCombustionFix;

using System;
using Assets.Scripts.Atmospherics;
using HarmonyLib;

[HarmonyPatch(typeof(Combustion), nameof(Combustion.GetResult))]
internal class GetResultPrefixPatch
{
    private static bool Prefix(GasType fuelType, GasType oxidiserType, ref CombustionResult __result)
    {
        Plugin.Logger?.LogInfo($"Calling {nameof(Combustion.GetResult)}");
        var combustionResult = Shared.PatchedData[Combustion.FuelIndex(fuelType), Combustion.OxidiserIndex(oxidiserType)];
        __result = combustionResult.IsValid() ? combustionResult : throw new NotImplementedException(string.Format("Combustion result for {0} {1} is not implemented", (object)fuelType, (object)oxidiserType));
        return false;
    }
}
