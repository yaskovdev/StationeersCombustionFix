namespace StationeersCombustionFix;

internal enum TemperatureUnit
{
    Celsius,
    Kelvin
}

internal sealed class Temperature
{
    private float Value { get; }

    private TemperatureUnit Unit { get; }

    internal Temperature(float value, TemperatureUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    internal bool Matches(Temperature other) => Unit == other.Unit && Value.Equals(other.Value);
}
