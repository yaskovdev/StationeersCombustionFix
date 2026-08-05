namespace StationeersCombustionFix;

using System.Collections.Generic;
using System.Linq;
using Trading;
using static Assets.Scripts.Atmospherics.Chemistry;

internal static class StartingFuelMixture
{
    internal static List<ActionData>? CreateCorrectedActions(IReadOnlyList<ActionData> actions)
    {
        var mixture = FindMethaneMixture(actions);

        if (mixture == default)
        {
            return null;
        }

        var methaneAmount = mixture.Methane.GetAmount();
        var oxygenAmount = mixture.Oxygen.GetAmount();
        var methaneTemperature = mixture.Methane.GetTemperature();
        var oxygenTemperature = mixture.Oxygen.GetTemperature();

        // Energy is total, so preserving it while changing the amount would change the resulting temperature.
        if (methaneAmount is not null
            && oxygenAmount is not null
            && methaneTemperature is not null
            && oxygenTemperature is not null
            && float.IsNaN(mixture.Methane.Energy)
            && float.IsNaN(mixture.Oxygen.Energy)
            && methaneAmount.Unit == AmountUnit.Moles
            && oxygenAmount.Unit == AmountUnit.Moles
            && methaneAmount.IsPositiveFinite
            && oxygenAmount.IsPositiveFinite
            && methaneAmount.Value.Equals(oxygenAmount.Value * 2)
            && methaneTemperature.Matches(oxygenTemperature))
        {
            return actions.Select(action => action switch
            {
                _ when ReferenceEquals(action, mixture.Methane) => mixture.Methane.CloneWithAmount(oxygenAmount),
                _ when ReferenceEquals(action, mixture.Oxygen) => mixture.Oxygen.CloneWithAmount(methaneAmount),
                _ => action
            }).ToList();
        }
        return null;
    }

    private static (GasAction Methane, GasAction Oxygen) FindMethaneMixture(IReadOnlyList<ActionData> actions)
    {
        var gasActions = actions.OfType<GasAction>().ToArray();
        var methane = gasActions.FirstOrDefault(it => it.Type == GasType.Methane);
        var oxygen = gasActions.FirstOrDefault(it => it.Type == GasType.Oxygen);
        return gasActions.Length != 2 || methane is null || oxygen is null ? default : (methane, oxygen);
    }
}
