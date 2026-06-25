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
        var methaneOxygen = NewMethaneOxygen();
        methaneOxygen.ShouldBeEquivalentTo(Combustion.ResultMethaneOxygen);
        CombustionResultPatch.PatchReactions(methaneOxygen, NewMethaneOzone());
        methaneOxygen.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        methaneOxygen.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        methaneOxygen.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        methaneOxygen.FuelRatio.ShouldBe(new MoleQuantity(0.5));
        methaneOxygen.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
    }

    [TestMethod]
    public void ShouldPatchMethaneOzoneReactionWhenEnabled()
    {
        CombustionResultPatch.PatchMethaneOzoneReaction = () => true;
        var methaneOzone = NewMethaneOzone();
        methaneOzone.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        CombustionResultPatch.PatchReactions(NewMethaneOxygen(), methaneOzone);
        methaneOzone.FuelMoleCount.ShouldBe(new MoleQuantity(3.0));
        methaneOzone.OxidiserMoleCount.ShouldBe(new MoleQuantity(4.0));
        methaneOzone.OxidiserRatio.ToDouble().ShouldBe(new MoleQuantity(1.3333).ToDouble(), 0.0001);
        methaneOzone.FuelRatio.ShouldBe(new MoleQuantity(0.75));
        methaneOzone.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
    }

    [TestMethod]
    public void ShouldNotPatchMethaneOzoneReactionWhenDisabled()
    {
        var methaneOzone = NewMethaneOzone();
        methaneOzone.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        var originalFuelMoleCount = methaneOzone.FuelMoleCount;
        var originalOxidiserMoleCount = methaneOzone.OxidiserMoleCount;
        var originalOutputs = methaneOzone.Outputs;
        var originalOxidiserRatio = methaneOzone.OxidiserRatio;
        var originalFuelRatio = methaneOzone.FuelRatio;
        CombustionResultPatch.PatchReactions(NewMethaneOxygen(), methaneOzone);
        methaneOzone.FuelMoleCount.ShouldBe(originalFuelMoleCount);
        methaneOzone.OxidiserMoleCount.ShouldBe(originalOxidiserMoleCount);
        methaneOzone.Outputs.ShouldBe(originalOutputs);
        methaneOzone.OxidiserRatio.ShouldBe(originalOxidiserRatio);
        methaneOzone.FuelRatio.ShouldBe(originalFuelRatio);
    }

    private static CombustionResult NewMethaneOxygen() =>
        new(2.0, 1.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) });

    private static CombustionResult NewMethaneOzone() =>
        new(3.0, 2.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) });
}
