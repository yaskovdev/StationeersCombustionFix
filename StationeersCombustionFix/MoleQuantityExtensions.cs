namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;

public static class MoleQuantityExtensions
{
    public static bool Matches(this MoleQuantity value, double quantity) => new MoleQuantity(quantity).Equals(value);
}
