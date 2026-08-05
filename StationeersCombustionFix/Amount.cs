namespace StationeersCombustionFix;

internal enum AmountUnit
{
    Moles,
    Litres
}

internal sealed class Amount
{
    internal float Value { get; }

    internal AmountUnit Unit { get; }

    internal bool IsPositiveFinite => Value > 0 && !float.IsInfinity(Value);

    internal Amount(float value, AmountUnit unit)
    {
        Value = value;
        Unit = unit;
    }
}
