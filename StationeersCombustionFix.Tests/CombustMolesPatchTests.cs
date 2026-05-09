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
        combustionResult.RunCombustion(new Mole(GasType.Methane, new MoleQuantity(0.0702003868704067), new MoleEnergy(444.242810054923)), new Mole(GasType.Oxygen, new MoleQuantity(0.0351001934352034), new MoleEnergy(950.411240069048)), 0.99, out var combustionEnergy, out var burnedFuel, out var cleanBurnRatio);
        combustionEnergy.ToDouble().ShouldBe(19876.537538487, 0.001);
        burnedFuel.ToDouble().ShouldBe(0.0694983830017026, 0.001);
        cleanBurnRatio.ShouldBe(1);
    }
    
    // (GasType.Methane, GasType.Oxygen) => new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) })
    // Running combustion with fuel (Methane, 0.163484713199987, 5437.41808821058), oxidiser (Oxygen, 0.0817423565999935, 1281.14921361891), ratio 0.0510212775974559
    // Run combustion, got 596.395723860865, 0.00208529973377925, 0.25
    [TestMethod]
    public void Should1To2()
    {
        var combustionResult = new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) });
        combustionResult.RunCombustion(new Mole(GasType.Methane, new MoleQuantity(0.163484713199987), new MoleEnergy(5437.41808821058)), new Mole(GasType.Oxygen, new MoleQuantity(0.0817423565999935), new MoleEnergy(1281.14921361891)), 0.0510212775974559, out var combustionEnergy, out var burnedFuel, out var cleanBurnRatio);
        combustionEnergy.ToDouble().ShouldBe(596.395723860865, 0.001);
        burnedFuel.ToDouble().ShouldBe(0.00208529973377925, 0.001);
        cleanBurnRatio.ShouldBe(0.25f);
    }
}
