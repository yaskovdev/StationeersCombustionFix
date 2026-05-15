namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using static Assets.Scripts.Atmospherics.Chemistry;

public static class CombustionValueExtensions
{
    public static bool Matches(this CombustionValue value, GasType gasType, double quantity) =>
        value.GasType == gasType && value.Quantity.Matches(quantity);
}
