namespace StationeersCombustionFix;

using System.Linq;
using Assets.Scripts.Atmospherics;

public static class CombustionValueExtensions
{
    public static bool Is(this CombustionValue[] values, CombustionValue[] expectedValues) =>
        values.Length == expectedValues.Length
        && values.Zip(expectedValues, Is).All(x => x);

    private static bool Is(this CombustionValue value, CombustionValue expectedValue) =>
        value.GasType == expectedValue.GasType && value.Quantity.Is(expectedValue.Quantity);
}
