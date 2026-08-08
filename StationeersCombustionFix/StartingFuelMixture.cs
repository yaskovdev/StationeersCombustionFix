namespace StationeersCombustionFix;

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Trading;
using static Assets.Scripts.Atmospherics.Chemistry;

internal static class StartingFuelMixture
{
    /// <summary>
    /// Permanently corrects matching recipes throughout a deserialized spawn-data tree before the game initializes
    /// them. For vanilla data, this is equivalent to changing
    /// <c>Stationeers\rocketstation_Data\StreamingAssets\Data\startconditions.xml</c> in memory, so subsequent tooltips
    /// and spawned contents are both derived from the corrected recipes.
    /// </summary>
    internal static int CorrectSpawnTree(SpawnData spawnData) =>
        spawnData.Items.Sum(CorrectThingTree)
        + spawnData.DynamicThings.Sum(CorrectThingTree)
        + spawnData.Structures.Sum(CorrectThingTree)
        + spawnData.Spawns.Sum(CorrectSpawnTree);

    private static int CorrectThingTree(ThingSpawnData spawnData) =>
        CorrectActions(spawnData)
        + spawnData.Items.Sum(CorrectThingTree)
        + spawnData.DynamicThings.Sum(CorrectThingTree)
        + spawnData.Spawns.Sum(CorrectSpawnTree);

    private static int CorrectActions(ThingSpawnData spawnData)
    {
        if (CreateCorrectedActions(spawnData.Actions) is { } correctedActions)
        {
            spawnData.Actions = correctedActions;
            return 1;
        }
        return 0;
    }

    private static List<ActionData>? CreateCorrectedActions(IReadOnlyList<ActionData> actions)
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

        if (methaneAmount is not null
            && oxygenAmount is not null
            && methaneTemperature is not null
            && oxygenTemperature is not null
            // Energy is total, so preserving it while changing the amount would change the resulting temperature.
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
