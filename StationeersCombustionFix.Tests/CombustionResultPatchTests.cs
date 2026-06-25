namespace StationeersCombustionFix.Tests;

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
    public void ShouldPatchMethaneOxygenReaction()
    {
        CombustionResultPatch.PatchReactions();
        var result = Combustion.ResultMethaneOxygen;
        result.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        result.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        result.FuelRatio.ShouldBe(new MoleQuantity(0.5));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
    }

    [TestMethod]
    public void ShouldPatchMethaneOzoneReactionWhenEnabled()
    {
        CombustionResultPatch.PatchMethaneOzoneReaction = () => true;
        CombustionResultPatch.PatchReactions();
        var result = Combustion.ResultMethaneOzone;
        result.FuelMoleCount.ShouldBe(new MoleQuantity(3.0));
        result.OxidiserMoleCount.ShouldBe(new MoleQuantity(4.0));
        result.OxidiserRatio.ToDouble().ShouldBe(new MoleQuantity(1.3333).ToDouble(), 0.0001);
        result.FuelRatio.ShouldBe(new MoleQuantity(0.75));
        result.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
    }

    [TestMethod]
    public void ShouldNotPatchMethaneOzoneReactionWhenDisabled()
    {
        var result = Combustion.ResultMethaneOzone;
        var originalFuelMoleCount = result.FuelMoleCount;
        var originalOxidiserMoleCount = result.OxidiserMoleCount;
        var originalOutputs = result.Outputs;
        var originalOxidiserRatio = result.OxidiserRatio;
        var originalFuelRatio = result.FuelRatio;
        CombustionResultPatch.PatchReactions();
        result.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        result.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        result.Outputs.ShouldBe(originalOutputs);
        result.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        result.FuelRatio.ShouldBe(originalFuelRatio);
    }
}
