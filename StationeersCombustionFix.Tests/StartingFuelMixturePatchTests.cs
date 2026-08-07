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
    public void ShouldCorrectVanillaMoleMixture()
    {
        var actions = new List<ActionData>
        {
            new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
            new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
        };

        var correctedActions = CorrectActions(actions);

        correctedActions.ShouldNotBeNull();
        correctedActions.ShouldNotBeSameAs(actions);
        correctedActions.Gas(GasType.Oxygen).Moles.ShouldBe(100f);
        correctedActions.Gas(GasType.Methane).Moles.ShouldBe(50f);
        actions.Gas(GasType.Oxygen).Moles.ShouldBe(50f);
        actions.Gas(GasType.Methane).Moles.ShouldBe(100f);
    }

    [TestMethod]
    public void ShouldCorrectMixtureWhenMethaneActionComesFirst()
    {
        var actions = new List<ActionData>
        {
            new GasAction { Type = GasType.Methane, Moles = 1000f, Celsius = 20f },
            new GasAction { Type = GasType.Oxygen, Moles = 500f, Celsius = 20f }
        };

        var correctedActions = CorrectActions(actions);

        correctedActions.ShouldNotBeNull();
        correctedActions.OfType<GasAction>().Select(it => it.Type)
            .ShouldBe(new[] { GasType.Methane, GasType.Oxygen });
        correctedActions.Gas(GasType.Methane).Moles.ShouldBe(500f);
        correctedActions.Gas(GasType.Oxygen).Moles.ShouldBe(1000f);
    }

    [TestMethod]
    public void ShouldNotCorrectLitreMixture()
    {
        var actions = new List<ActionData>
        {
            new GasAction { Type = GasType.Methane, Litres = 10f, Kelvin = 293.15f },
            new GasAction { Type = GasType.Oxygen, Litres = 5f, Kelvin = 293.15f }
        };

        var correctedActions = CorrectActions(actions);

        correctedActions.ShouldBeNull();
    }

    [TestMethod]
    public void ShouldNotCorrectUnrecognizedMixtures()
    {
        var mixtures = new[]
        {
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 100f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 50f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 60f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 21f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f },
                new GasAction { Type = GasType.Nitrogen, Moles = 1f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Litres = 100f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Litres = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Kelvin = 293.15f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f, Energy = 1f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f, Celsius = 20f, Kelvin = 293.15f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            },
            new List<ActionData>
            {
                new GasAction { Type = GasType.Oxygen, Moles = 50f },
                new GasAction { Type = GasType.Methane, Moles = 100f, Celsius = 20f }
            }
        };

        mixtures.ShouldAllBe(it => CorrectActions(it) == null);
    }

    [TestMethod]
    public void ShouldCorrectEveryExactMixtureInSpawnDataTree()
    {
        var tank = FuelThing("CustomFuelTank", 500f, 1000f);
        var tankActions = tank.Actions;
        var canister = FuelThing("CustomWelderCanister", 50f, 100f);
        var canisterActions = canister.Actions;
        var tool = new DynamicSpawnData { PrefabId = "CustomWelder" };
        tool.Items.Add(canister);
        var containedDynamicTank = FuelThing("ContainedDynamicFuelTank", 500f, 1000f);
        var containedDynamicTankActions = containedDynamicTank.Actions;
        tool.DynamicThings.Add(containedDynamicTank);
        var containedSpawnTank = FuelThing("ContainedSpawnFuelTank", 500f, 1000f);
        var containedSpawnTankActions = containedSpawnTank.Actions;
        var containedSpawn = new SpawnData { Id = "ContainedCustomSpawn" };
        containedSpawn.DynamicThings.Add(containedSpawnTank);
        tool.Spawns.Add(containedSpawn);
        var structure = new StructureSpawnData
        {
            PrefabId = "CustomStructure",
            Actions = FuelActions(50f, 100f)
        };
        var structureActions = structure.Actions;
        var nestedTank = FuelThing("NestedCustomFuelTank", 500f, 1000f);
        var nestedTankActions = nestedTank.Actions;
        var nestedSpawn = new SpawnData { Id = "NestedCustomSpawn" };
        nestedSpawn.DynamicThings.Add(nestedTank);
        var spawnData = new SpawnData { Id = "CustomStartData" };
        spawnData.DynamicThings.Add(tank);
        spawnData.Items.Add(tool);
        spawnData.Structures.Add(structure);
        spawnData.Spawns.Add(nestedSpawn);

        StartingFuelMixture.CorrectSpawnTree(spawnData).ShouldBe(6);

        tank.Actions.ShouldNotBeSameAs(tankActions);
        tank.Actions.Gas(GasType.Oxygen).Moles.ShouldBe(1000f);
        tank.Actions.Gas(GasType.Methane).Moles.ShouldBe(500f);
        canister.Actions.ShouldNotBeSameAs(canisterActions);
        canister.Actions.Gas(GasType.Oxygen).Moles.ShouldBe(100f);
        canister.Actions.Gas(GasType.Methane).Moles.ShouldBe(50f);
        containedDynamicTank.Actions.ShouldNotBeSameAs(containedDynamicTankActions);
        containedSpawnTank.Actions.ShouldNotBeSameAs(containedSpawnTankActions);
        structure.Actions.ShouldNotBeSameAs(structureActions);
        nestedTank.Actions.ShouldNotBeSameAs(nestedTankActions);

        StartingFuelMixture.CorrectSpawnTree(spawnData).ShouldBe(0);
    }

    [TestMethod]
    public void ShouldApplyCorrectionWhenSpawnDataIsInitializedAndSettingEnabled()
    {
        var target = FuelThing("CustomFuelTank", 500f, 1000f);
        var originalActions = target.Actions;
        var spawnData = new SpawnData { Id = "CustomSpawnData" };
        spawnData.DynamicThings.Add(target);
        StartingFuelMixturePatch.PatchStartingFuelMixtures = () => true;

        StartingFuelMixturePatch.Prefix(spawnData);

        target.Actions.ShouldNotBeSameAs(originalActions);
        target.Actions.Gas(GasType.Oxygen).Moles.ShouldBe(1000f);
        target.Actions.Gas(GasType.Methane).Moles.ShouldBe(500f);
    }

    [TestMethod]
    public void ShouldLeaveSpawnDataUnchangedWhenSettingDisabled()
    {
        var target = FuelThing("CustomFuelTank", 500f, 1000f);
        var originalActions = target.Actions;
        var spawnData = new SpawnData { Id = "CustomSpawnData" };
        spawnData.DynamicThings.Add(target);

        StartingFuelMixturePatch.Prefix(spawnData);

        target.Actions.ShouldBeSameAs(originalActions);
    }

    private static List<ActionData>? CorrectActions(List<ActionData> actions)
    {
        var target = new DynamicSpawnData { Actions = actions };
        var spawnData = new SpawnData();
        spawnData.DynamicThings.Add(target);

        return StartingFuelMixture.CorrectSpawnTree(spawnData) == 0 ? null : target.Actions;
    }

    private static DynamicSpawnData FuelThing(string prefabId, float oxygenMoles, float methaneMoles) =>
        new()
        {
            PrefabId = prefabId,
            Actions = FuelActions(oxygenMoles, methaneMoles)
        };

    private static List<ActionData> FuelActions(float oxygenMoles, float methaneMoles) =>
        new()
        {
            new GasAction { Type = GasType.Oxygen, Moles = oxygenMoles, Celsius = 20f },
            new GasAction { Type = GasType.Methane, Moles = methaneMoles, Celsius = 20f }
        };
}

internal static class StartingFuelMixtureTestExtensions
{
    internal static GasAction Gas(this IEnumerable<ActionData> actions, GasType gasType)
    {
        return actions.OfType<GasAction>().Single(it => it.Type == gasType);
    }
}
