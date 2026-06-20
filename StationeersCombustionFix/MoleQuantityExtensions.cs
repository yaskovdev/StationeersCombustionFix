namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;

public static class MoleQuantityExtensions
{
    public static bool Is(this MoleQuantity value, MoleQuantity quantity) => value.Is(quantity.ToDouble());

    public static bool Is(this MoleQuantity value, double quantity) => value.ToDouble().Equals(quantity);
}
