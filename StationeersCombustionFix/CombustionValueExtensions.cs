namespace StationeersCombustionFix;

using System.Linq;
using Assets.Scripts.Atmospherics;

public static class CombustionValueExtensions
{
    public static bool Equals(this CombustionValue[] values, CombustionValue[] expectedValues) =>
        values.Length == expectedValues.Length
        && values.Zip(expectedValues, Equals).All(x => x);

    private static bool Equals(this CombustionValue value, CombustionValue expectedValue) =>
        value.GasType == expectedValue.GasType && MoleQuantityExtensions.Equals(value.Quantity, expectedValue.Quantity);
}
