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
    public void ResetConfig() => CombustionResultPatch.PatchMethaneOzoneReaction = () => false;

    [TestMethod]
    public void ShouldPatchMethaneOxygenResult()
    {
        var result = new CombustionResult(2.0, 1.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneOxygen);
        CombustionResultPatch.PatchIfMatches(result);
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        result.FuelRatio.ShouldBe(new MoleQuantity(0.5));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
    }

    [TestMethod]
    public void ShouldPatchMethaneOzoneResultWhenEnabled()
    {
        CombustionResultPatch.PatchMethaneOzoneReaction = () => true;
        var result = new CombustionResult(3.0, 2.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) });
        result.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        CombustionResultPatch.PatchIfMatches(result);
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
        CombustionResultPatch.PatchIfMatches(result);
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }

    [TestMethod]
    public void ShouldNotPatchOtherResults()
    {
        CombustionResultPatch.PatchMethaneOzoneReaction = () => true;
        ImmutableList
            .Create(Combustion.ResultMethaneNitrous, Combustion.ResultHydrogenOxygen, Combustion.ResultHydrogenNitrous, Combustion.ResultHydrogenOzone, Combustion.ResultAlcoholOxygen, Combustion.ResultAlcoholNitrous, Combustion.ResultAlcoholOzone, Combustion.ResultHydrazine)
            .ForEach(result =>
            {
                var originalFuelMoleCount = result.FuelMoleCount;
                var originalOxidiserMoleCount = result.OxidiserMoleCount;
                var originalOutputs = result.Outputs;
                var originalOxidiserRatio = result.OxidiserRatio;
                var originalFuelRatio = result.FuelRatio;
                CombustionResultPatch.PatchIfMatches(result);
                result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
                result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
                result.Outputs.ShouldBe(originalOutputs);
                result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
                result.FuelRatio.ShouldBe(originalFuelRatio);
            });
    }
}
