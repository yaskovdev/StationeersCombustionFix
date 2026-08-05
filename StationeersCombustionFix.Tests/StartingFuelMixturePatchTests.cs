namespace StationeersCombustionFix.Tests;

using System.Linq;
using Assets.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Trading;
using static Assets.Scripts.Atmospherics.Chemistry;

[TestClass]
public class StartingFuelMixturePatchTests
{
    [TestInitialize]
    public void ResetConfig()
    {
        StartingFuelMixturePatch.PatchStartingFuelMixtures = () => false;
    }

    [TestMethod]
    public void ShouldCorrectLegacyMoleMixture()
    {
        var actions = new ActionData[]
        {
            new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
            new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
        };

        var correctedActions = StartingFuelMixture.CreateCorrectedActions(actions);

        correctedActions.ShouldNotBeNull();
        correctedActions.ShouldNotBeSameAs(actions);
        correctedActions.Gas(GasType.Oxygen).Moles.ShouldBe(100f);
        correctedActions.Gas(GasType.Methane).Moles.ShouldBe(50f);
        actions.Gas(GasType.Oxygen).Moles.ShouldBe(50f);
        actions.Gas(GasType.Methane).Moles.ShouldBe(100f);
    }

    [TestMethod]
    public void ShouldNotCorrectLitreMixture()
    {
        var actions = new ActionData[]
        {
            new GasAction { Type = GasType.Methane, Litres = 10f, Kelvin = 293.15f },
            new GasAction { Type = GasType.Oxygen, Litres = 5f, Kelvin = 293.15f }
        };

        var correctedActions = StartingFuelMixture.CreateCorrectedActions(actions);

        correctedActions.ShouldBeNull();
    }

    [TestMethod]
    public void ShouldNotCorrectUnrecognizedMixtures()
    {
        var mixtures = new[]
        {
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 100f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 50f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 60f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 21f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f },
                new GasAction { Type = GasType.Nitrogen, Moles = 1f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Litres = 100f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Litres = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Kelvin = 293.15f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f, Energy = 1f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f, Kelvin = 293.15f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new ActionData[]
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            }
        };

        mixtures.ShouldAllBe(it => StartingFuelMixture.CreateCorrectedActions(it) == null);
    }

    [TestMethod]
    public void ShouldTemporarilyCorrectMixtureInsideStartingSpawnScope()
    {
        var originalActions = new ActionData[]
        {
            new GasAction { Type = GasType.Oxygen, Moles = 500f, Celsius = 20f },
            new GasAction { Type = GasType.Methane, Moles = 1000f, Celsius = 20f }
        }.ToList();
        var spawnData = new DynamicSpawnData
        {
            PrefabId = "DynamicGasTankAdvanced",
            Actions = originalActions
        };
        StartingFuelMixturePatch.PatchStartingFuelMixtures = () => true;

        StartingSpawnScope.Enter();
        try
        {
            StartingFuelMixturePatch.Prefix(spawnData, out var state);

            spawnData.Actions.ShouldNotBeSameAs(originalActions);
            spawnData.Actions.Gas(GasType.Oxygen).Moles.ShouldBe(1000f);
            spawnData.Actions.Gas(GasType.Methane).Moles.ShouldBe(500f);

            StartingFuelMixturePatch.Finalizer(spawnData, null, state).ShouldBeNull();
            spawnData.Actions.ShouldBeSameAs(originalActions);
        }
        finally
        {
            StartingSpawnScope.Exit();
        }
    }

    [TestMethod]
    public void ShouldNotCorrectMixtureOutsideStartingSpawnScope()
    {
        var originalActions = new ActionData[]
        {
            new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
            new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
        }.ToList();
        var spawnData = new DynamicSpawnData { Actions = originalActions };
        StartingFuelMixturePatch.PatchStartingFuelMixtures = () => true;

        StartingFuelMixturePatch.Prefix(spawnData, out var state);

        state.ShouldBeNull();
        spawnData.Actions.ShouldBeSameAs(originalActions);
    }

    [TestMethod]
    public void ShouldOnlyEnterScopeForStartingEventsAndFallbackKits()
    {
        StartingSpawnScope.ShouldEnter(new SpawnData { EventType = SpawnEvent.NewWorld }).ShouldBeTrue();
        StartingSpawnScope.ShouldEnter(new SpawnData { EventType = SpawnEvent.RespawnPlayerKit }).ShouldBeTrue();
        StartingSpawnScope.ShouldEnter(new SpawnData { Id = "DefaultNewPlayer" }).ShouldBeTrue();
        StartingSpawnScope.ShouldEnter(new SpawnData { Id = "DefaultRespawnPlayer" }).ShouldBeTrue();
        StartingSpawnScope.ShouldEnter(new SpawnData { Id = "GasPowerPackage" }).ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldKeepScopeActiveAcrossNestedNonEntrySpawns()
    {
        var outerSpawn = new SpawnData { EventType = SpawnEvent.NewPlayerKit };
        var nestedSpawn = new SpawnData { Id = "DefaultToolbelt" };

        StartingSpawnScopePatch.Prefix(outerSpawn, out var outerState);
        try
        {
            StartingSpawnScopePatch.Prefix(nestedSpawn, out var nestedState);
            try
            {
                nestedState.ShouldBeFalse();
                StartingSpawnScope.IsActive.ShouldBeTrue();
            }
            finally
            {
                StartingSpawnScopePatch.Finalizer(null, nestedState);
            }
            StartingSpawnScope.IsActive.ShouldBeTrue();
        }
        finally
        {
            StartingSpawnScopePatch.Finalizer(null, outerState);
        }
        StartingSpawnScope.IsActive.ShouldBeFalse();
    }

}

internal static class StartingFuelMixtureTestExtensions
{
    internal static GasAction Gas(this IEnumerable<ActionData> actions, GasType gasType)
    {
        return actions.OfType<GasAction>().Single(it => it.Type == gasType);
    }
}
