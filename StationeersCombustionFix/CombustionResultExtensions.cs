namespace StationeersCombustionFix;

using System.Linq;
using Assets.Scripts.Atmospherics;

public static class CombustionResultExtensions
{
    public static string Format(this CombustionResult result) =>
        $"(FuelMoleCount: {result.FuelMoleCount}, OxidiserMoleCount: {result.OxidiserMoleCount}, Outputs: [{string.Join(", ", result.Outputs.Select(Format))}])";

    private static string Format(CombustionValue value) =>
        $"{value.GasType}: {value.Quantity}";
}
