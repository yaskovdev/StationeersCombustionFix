namespace StationeersCombustionFix;

using Trading;

internal static class GasActionExtensions
{
    internal static Amount? GetAmount(this GasAction source)
    {
        var hasMoles = !float.IsNaN(source.Moles);
        var hasLitres = !float.IsNaN(source.Litres);
        if (hasMoles == hasLitres)
        {
            return null;
        }
        return hasMoles ? new Amount(source.Moles, AmountUnit.Moles) : new Amount(source.Litres, AmountUnit.Litres);
    }

    internal static Temperature? GetTemperature(this GasAction source)
    {
        var hasCelsius = !float.IsNaN(source.Celsius);
        var hasKelvin = !float.IsNaN(source.Kelvin);
        if (hasCelsius == hasKelvin)
        {
            return null;
        }
        return hasCelsius ? new Temperature(source.Celsius, TemperatureUnit.Celsius) : new Temperature(source.Kelvin, TemperatureUnit.Kelvin);
    }

    internal static GasAction CloneWithAmount(this GasAction source, Amount amount) =>
        new()
        {
            Type = source.Type,
            Moles = amount.Unit == AmountUnit.Moles ? amount.Value : float.NaN,
            Litres = amount.Unit == AmountUnit.Litres ? amount.Value : float.NaN,
            Celsius = source.Celsius,
            Kelvin = source.Kelvin,
            Energy = source.Energy
        };
}
