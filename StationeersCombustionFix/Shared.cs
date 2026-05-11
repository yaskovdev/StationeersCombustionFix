namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using static Assets.Scripts.Atmospherics.Chemistry;

public static class Shared
{
    public static readonly CombustionResult ResultMethaneOxygenPatch = new(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) });

    public static readonly CombustionResult?[,] DataPatch =
    {
        {
            ResultMethaneOxygenPatch,
            Combustion.ResultMethaneNitrous,
            Combustion.ResultMethaneOzone,
            null
        },
        {
            Combustion.ResultHydrogenOxygen,
            Combustion.ResultHydrogenNitrous,
            Combustion.ResultHydrogenOzone,
            null
        },
        {
            Combustion.ResultAlcoholOxygen,
            Combustion.ResultAlcoholNitrous,
            Combustion.ResultAlcoholOzone,
            null
        },
        {
            null,
            null,
            null,
            Combustion.ResultHydrazine
        }
    };
}
