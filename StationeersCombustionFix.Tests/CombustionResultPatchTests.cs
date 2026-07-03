namespace StationeersCombustionFix.Tests;

using System.Collections.Immutable;
using Assets.Scripts.Atmospherics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static Assets.Scripts.Atmospherics.Chemistry;

[TestClass]
public class CombustionResultPatchTests
{
    [TestInitialize]
    public void ResetConfig()
    {
        CombustionResultPatch.PatchMethaneNitrousReaction = () => false;
        CombustionResultPatch.PatchMethaneOzoneReaction = () => false;
        CombustionResultPatch.PatchHydrogenOxygenReaction = () => false;
        CombustionResultPatch.PatchHydrogenOzoneReaction = () => false;
        CombustionResultPatch.PatchAlcoholOxygenReaction = () => false;
        CombustionResultPatch.PatchAlcoholNitrousReaction = () => false;
        CombustionResultPatch.PatchAlcoholOzoneReaction = () => false;
    }

    [TestMethod]
    public void ShouldPatchMethaneOxygenResult()
    {
        var result = new CombustionResult(2.0, 1.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneOxygen);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        result.FuelRatio.ShouldBe(new MoleQuantity(0.5));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
    }

    [TestMethod]
    public void ShouldPatchMethaneNitrousResultWhenEnabled()
    {
        CombustionResultPatch.PatchMethaneNitrousReaction = () => true;
        var result = new CombustionResult(1.0, 1.0, new CombustionValue[] { new(GasType.CarbonDioxide, 2.0), new(GasType.Nitrogen, 2.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneNitrous);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(4.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(4.0));
        result.FuelRatio.ShouldBe(new MoleQuantity(0.25));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0), new(GasType.Nitrogen, 4.0) });
    }

    [TestMethod]
    public void ShouldNotPatchMethaneNitrousResultWhenDisabled()
    {
        var result = new CombustionResult(1.0, 1.0, new CombustionValue[] { new(GasType.CarbonDioxide, 2.0), new(GasType.Nitrogen, 2.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneNitrous);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldPatchMethaneOzoneResultWhenEnabled()
    {
        CombustionResultPatch.PatchMethaneOzoneReaction = () => true;
        var result = new CombustionResult(3.0, 2.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(3.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(4.0));
        result.OxidiserRatio.ToDouble().ShouldBe(new MoleQuantity(1.3333).ToDouble(), 0.0001);
        result.FuelRatio.ShouldBe(new MoleQuantity(0.75));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
    }

    [TestMethod]
    public void ShouldNotPatchMethaneOzoneResultWhenDisabled()
    {
        var result = new CombustionResult(3.0, 2.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldPatchHydrogenOxygenResultWhenEnabled()
    {
        CombustionResultPatch.PatchHydrogenOxygenReaction = () => true;
        var result = new CombustionResult(2.0, 1.0, new CombustionValue[] { new(GasType.Steam, 3.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultHydrogenOxygen);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(2.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(0.5));
        result.FuelRatio.ShouldBe(new MoleQuantity(2.0));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.Steam, 2.0) });
    }

    [TestMethod]
    public void ShouldNotPatchHydrogenOxygenResultWhenDisabled()
    {
        var result = new CombustionResult(2.0, 1.0, new CombustionValue[] { new(GasType.Steam, 3.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultHydrogenOxygen);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldPatchHydrogenOzoneResultWhenEnabled()
    {
        CombustionResultPatch.PatchHydrogenOzoneReaction = () => true;
        var result = new CombustionResult(3.0, 1.0, new CombustionValue[] { new(GasType.Steam, 4.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultHydrogenOzone);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(3.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserRatio.ToDouble().ShouldBe(new MoleQuantity(0.3333).ToDouble(), 0.0001);
        result.FuelRatio.ShouldBe(new MoleQuantity(3.0));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.Steam, 3.0) });
    }

    [TestMethod]
    public void ShouldNotPatchHydrogenOzoneResultWhenDisabled()
    {
        var result = new CombustionResult(3.0, 1.0, new CombustionValue[] { new(GasType.Steam, 4.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultHydrogenOzone);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldPatchAlcoholOxygenResultWhenEnabled()
    {
        CombustionResultPatch.PatchAlcoholOxygenReaction = () => true;
        var result = new CombustionResult(1.0, 3.0, new CombustionValue[] { new(GasType.CarbonDioxide, 8.0), new(GasType.Steam, 2.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultAlcoholOxygen);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(3.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(3.0));
        result.FuelRatio.ToDouble().ShouldBe(new MoleQuantity(0.3333).ToDouble(), 0.0001);
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 2.0), new(GasType.Steam, 3.0) });
    }

    [TestMethod]
    public void ShouldNotPatchAlcoholOxygenResultWhenDisabled()
    {
        var result = new CombustionResult(1.0, 3.0, new CombustionValue[] { new(GasType.CarbonDioxide, 8.0), new(GasType.Steam, 2.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultAlcoholOxygen);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldPatchAlcoholNitrousResultWhenEnabled()
    {
        CombustionResultPatch.PatchAlcoholNitrousReaction = () => true;
        var result = new CombustionResult(1.0, 2.0, new CombustionValue[] { new(GasType.Nitrogen, 4.0), new(GasType.Steam, 2.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultAlcoholNitrous);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(6.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(6.0));
        result.FuelRatio.ToDouble().ShouldBe(new MoleQuantity(0.1667).ToDouble(), 0.0001);
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 2.0), new(GasType.Steam, 3.0), new(GasType.Nitrogen, 6.0) });
    }

    [TestMethod]
    public void ShouldNotPatchAlcoholNitrousResultWhenDisabled()
    {
        var result = new CombustionResult(1.0, 2.0, new CombustionValue[] { new(GasType.Nitrogen, 4.0), new(GasType.Steam, 2.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultAlcoholNitrous);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldPatchAlcoholOzoneResultWhenEnabled()
    {
        CombustionResultPatch.PatchAlcoholOzoneReaction = () => true;
        var result = new CombustionResult(1.0, 2.0, new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 3.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultAlcoholOzone);
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        result.FuelRatio.ShouldBe(new MoleQuantity(0.5));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 2.0), new(GasType.Steam, 3.0) });
    }

    [TestMethod]
    public void ShouldNotPatchAlcoholOzoneResultWhenDisabled()
    {
        var result = new CombustionResult(1.0, 2.0, new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 3.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultAlcoholOzone);
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.Postfix(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldNotPatchOtherResults()
    {
        CombustionResultPatch.PatchMethaneNitrousReaction = () => true;
        CombustionResultPatch.PatchMethaneOzoneReaction = () => true;
        CombustionResultPatch.PatchHydrogenOxygenReaction = () => true;
        CombustionResultPatch.PatchHydrogenOzoneReaction = () => true;
        CombustionResultPatch.PatchAlcoholOxygenReaction = () => true;
        CombustionResultPatch.PatchAlcoholNitrousReaction = () => true;
        CombustionResultPatch.PatchAlcoholOzoneReaction = () => true;
        ImmutableList
            .Create(Combustion.ResultHydrogenNitrous, Combustion.ResultHydrazine)
            .ForEach(result =>
            {
                var originalFuelMoleCount = result.FuelMoleCount;
                var originalOxidiserMoleCount = result.OxidiserMoleCount;
                var originalOutputs = result.Outputs;
                var originalOxidiserRatio = result.OxidiserRatio;
                var originalFuelRatio = result.FuelRatio;
                CombustionResultPatch.Postfix(result);
                result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
                result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
                result.Outputs.ShouldBe(originalOutputs);
                result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
                result.FuelRatio.ShouldBe(originalFuelRatio);
            });
    }
}
