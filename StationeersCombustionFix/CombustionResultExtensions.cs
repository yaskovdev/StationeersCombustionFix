namespace StationeersCombustionFix;

using System.Globalization;
using System.Linq;
using Assets.Scripts.Atmospherics;

public static class CombustionResultExtensions
{
    public static string Format(this CombustionResult result) =>
        $"(FuelMoleCount: {Format(result.FuelMoleCount)}, OxidiserMoleCount: {Format(result.OxidiserMoleCount)}, Outputs: [{string.Join(", ", result.Outputs.Select(Format))}])";

    private static string Format(CombustionValue value) =>
        $"{value.GasType}: {Format(value.Quantity)}";

    private static string Format(MoleQuantity quantity) =>
        quantity.ToDouble().ToString(CultureInfo.InvariantCulture);
}
