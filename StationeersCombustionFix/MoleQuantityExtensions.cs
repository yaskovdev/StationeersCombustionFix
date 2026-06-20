namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;

public static class MoleQuantityExtensions
{
    public static bool Equals(this MoleQuantity value, MoleQuantity quantity) => Equals(value, quantity.ToDouble());

    public static bool Equals(this MoleQuantity value, double quantity) => value.ToDouble().Equals(quantity);
}
