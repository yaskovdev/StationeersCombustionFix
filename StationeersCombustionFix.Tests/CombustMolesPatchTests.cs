namespace StationeersCombustionFix.Tests;

using Assets.Scripts.Atmospherics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static Assets.Scripts.Atmospherics.Chemistry;

[TestClass]
public class CombustMolesPatchTests
{
    // (GasType.Methane, GasType.Oxygen) => new CombustionResult(2, 1, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) })
    // Running combustion with fuel (Methane, 0.0702003868704067, 444.242810054923), oxidiser (Oxygen, 0.0351001934352034, 950.411240069048), ratio 0.99
    // Run combustion, got 19876.537538487, 0.0694983830017026, 1
    [TestMethod]
    public void Should2To1()
    {
        var combustionResult = new CombustionResult(2, 1, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) });
        var mixture = combustionResult.RunCombustion(new Mole(GasType.Methane, new MoleQuantity(0.0064401038814301), new MoleEnergy(40.126981302237)), new Mole(GasType.Oxygen, new MoleQuantity(0.00322005194071505), new MoleEnergy(20.7519437616961)), 1, out var combustionEnergy, out var burnedFuel, out var cleanBurnRatio);
        // combustionEnergy.ToDouble().ShouldBe(19876.537538487, 0.001);
        // burnedFuel.ToDouble().ShouldBe(0.0694983830017026, 0.001);
        // cleanBurnRatio.ShouldBe(1);
        mixture.Methane.Quantity.ToDouble().ShouldBe(0);
    }
    
    // (GasType.Methane, GasType.Oxygen) => new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) })
    // Running combustion with fuel (Methane, 0.163484713199987, 5437.41808821058), oxidiser (Oxygen, 0.0817423565999935, 1281.14921361891), ratio 0.0510212775974559
    // Run combustion, got 596.395723860865, 0.00208529973377925, 0.25
    [TestMethod]
    public void Should1To2()
    {
        var combustionResult = new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) });
        var mixture = combustionResult.RunCombustion(new Mole(GasType.Methane, new MoleQuantity(0.00127029939280172), new MoleEnergy(8.02727986811946)), new Mole(GasType.Oxygen, new MoleQuantity(0.00254059878560344), new MoleEnergy(16.3830661455039)), 1, out var combustionEnergy, out var burnedFuel, out var cleanBurnRatio);
        mixture.Methane.Quantity.ToDouble().ShouldBe(0);
    }

    [TestMethod]
    public void Should3()
    {
        var fuel = GasMixtureHelper.Create();
        fuel.Add(new Mole(GasType.Methane, new MoleQuantity(1), new MoleEnergy(8)));
        fuel.Add(new Mole(GasType.Oxygen, new MoleQuantity(2), new MoleEnergy(16)));
    }
}
