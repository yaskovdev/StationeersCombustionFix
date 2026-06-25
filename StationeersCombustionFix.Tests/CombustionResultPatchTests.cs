namespace StationeersCombustionFix.Tests;

using Assets.Scripts.Atmospherics;
using BepInEx.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static Assets.Scripts.Atmospherics.Chemistry;

[TestClass]
public class CombustionResultPatchTests
{
    [TestMethod]
    public void ShouldPatchMethaneOxygenReaction()
    {
        var reactions = new FakeReactions();
        reactions.ResultMethaneOxygen.ShouldBeEquivalentTo(Combustion.ResultMethaneOxygen);
        new CombustionResultPatch(patchMethaneOzoneReaction: false, new ManualLogSource(nameof(CombustionResultPatchTests))).PatchReactions(typeof(FakeReactions), reactions);
        reactions.ResultMethaneOxygen.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        reactions.ResultMethaneOxygen.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        reactions.ResultMethaneOxygen.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        reactions.ResultMethaneOxygen.FuelRatio.ShouldBe(new MoleQuantity(0.5));
        reactions.ResultMethaneOxygen.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
    }

    [TestMethod]
    public void ShouldPatchMethaneOzoneReactionWhenEnabled()
    {
        var reactions = new FakeReactions();
        reactions.ResultMethaneOzone.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        new CombustionResultPatch(patchMethaneOzoneReaction: true, new ManualLogSource(nameof(CombustionResultPatchTests))).PatchReactions(typeof(FakeReactions), reactions);
        reactions.ResultMethaneOzone.FuelMoleCount.ShouldBe(new MoleQuantity(3.0));
        reactions.ResultMethaneOzone.OxidiserMoleCount.ShouldBe(new MoleQuantity(4.0));
        reactions.ResultMethaneOzone.OxidiserRatio.ToDouble().ShouldBe(new MoleQuantity(1.3333).ToDouble(), 0.0001);
        reactions.ResultMethaneOzone.FuelRatio.ShouldBe(new MoleQuantity(0.75));
        reactions.ResultMethaneOzone.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
    }

    [TestMethod]
    public void ShouldNotPatchMethaneOzoneReactionWhenDisabled()
    {
        var reactions = new FakeReactions();
        reactions.ResultMethaneOzone.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
        new CombustionResultPatch(patchMethaneOzoneReaction: false, new ManualLogSource(nameof(CombustionResultPatchTests))).PatchReactions(typeof(FakeReactions), reactions);
        reactions.ResultMethaneOzone.ShouldBeEquivalentTo(Combustion.ResultMethaneOzone);
    }

    /// <summary>
    /// Stands in for the static Combustion class: its field names match the keys in CombustionResultPatch, but the
    /// instances are private to the test so patching them never mutates the real game statics.
    /// </summary>
    private class FakeReactions
    {
        public readonly CombustionResult ResultMethaneOxygen =
            new(2.0, 1.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0) });

        public readonly CombustionResult ResultMethaneOzone =
            new(3.0, 2.0, new CombustionValue[] { new(GasType.Pollutant, 3.0), new(GasType.CarbonDioxide, 6.0), new(GasType.Steam, 1.0) });
    }
}
