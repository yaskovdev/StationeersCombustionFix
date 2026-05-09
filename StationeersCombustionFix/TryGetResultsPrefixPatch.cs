using static Assets.Scripts.Atmospherics.Chemistry;

namespace StationeersCombustionFix;

using System.Collections.Generic;
using Assets.Scripts.Atmospherics;
using HarmonyLib;

[HarmonyPatch(typeof(Combustion), nameof(Combustion.TryGetResults))]
internal class TryGetResultsPrefixPatch
{
    private static bool Prefix(GasType product, out List<CombustionResult> results, ref bool __result)
    {
        Plugin.Logger?.LogInfo($"Calling {nameof(Combustion.TryGetResults)}");
        results = (List<CombustionResult>)null;
        CombustionResult[,] data = Shared.PatchedData;
        int upperBound1 = data.GetUpperBound(0);
        int upperBound2 = data.GetUpperBound(1);
        for (int lowerBound1 = data.GetLowerBound(0); lowerBound1 <= upperBound1; ++lowerBound1)
        {
            for (int lowerBound2 = data.GetLowerBound(1); lowerBound2 <= upperBound2; ++lowerBound2)
            {
                CombustionResult combustionResult = data[lowerBound1, lowerBound2];
                if (combustionResult != null)
                {
                    foreach (CombustionValue output in combustionResult.Outputs)
                    {
                        if (output.GasType == product)
                        {
                            if (results == null)
                                results = new List<CombustionResult>(6);
                            results.Add(combustionResult);
                        }
                    }
                }
            }
        }
        __result = results != null;
        return false;
    }
}
