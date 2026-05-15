namespace StationeersCombustionFix.Tests;

using Assets.Scripts.Atmospherics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static Assets.Scripts.Atmospherics.Chemistry;

[TestClass]
public class CombustionResultPatchTests
{
    [TestMethod]
    public void ShouldPatchMethaneOxygenResult()
    {
        CombustionResultPatch.Postfix(Combustion.ResultMethaneOxygen);
        Combustion.ResultMethaneOxygen.FuelMoleCount.ShouldBe(new MoleQuantity(1.0));
        Combustion.ResultMethaneOxygen.OxidiserMoleCount.ShouldBe(new MoleQuantity(2.0));
        Combustion.ResultMethaneOxygen.Outputs.ShouldBe(new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) });
        Combustion.ResultMethaneOxygen.OxidiserRatio.ShouldBe(new MoleQuantity(2.0));
        Combustion.ResultMethaneOxygen.FuelRatio.ShouldBe(new MoleQuantity(0.5));
    }
}
