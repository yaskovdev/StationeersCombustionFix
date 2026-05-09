using static Assets.Scripts.Atmospherics.Chemistry;

namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using HarmonyLib;

[HarmonyPatch(typeof(Combustion), nameof(Combustion.CombustMoles))]
internal class CombustionPatch
{
    private static bool Prefix(Mole fuel, Mole oxidiser, double combustionRatio, out MoleEnergy combustionEnergy, out MoleQuantity combustedFuel, out float cleanBurnRatio, ref GasMixture __result)
    {
        var combustionResult = (fuel.Type, oxidiser.Type) switch
        {
            (GasType.Methane, GasType.Oxygen) => new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) }),
            _ => CombustionResult.Invalid
        };
        if (combustionResult.IsValid())
        {
            __result = combustionResult.RunCombustion(fuel, oxidiser, combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio);
            return false;
        }
        combustionEnergy = MoleEnergy.Zero;
        combustedFuel = MoleQuantity.Zero;
        cleanBurnRatio = 1f;
        return true;
    }
}
