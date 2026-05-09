using static Assets.Scripts.Atmospherics.Chemistry;

namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using HarmonyLib;
using BepInEx;

internal class CombustionPatch
{
    private static readonly CombustionResult[,] PatchedData = new CombustionResult[4, 4]
    {
        {
            new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) }),
            Combustion.ResultMethaneNitrous,
            Combustion.ResultMethaneOzone,
            null
        },
        {
            Combustion.ResultHydrogenOxygen,
            Combustion.ResultHydrogenNitrous,
            Combustion.ResultHydrogenOzone,
            null
        },
        {
            Combustion.ResultAlcoholOxygen,
            Combustion.ResultAlcoholNitrous,
            Combustion.ResultAlcoholOzone,
            null
        },
        {
            null,
            null,
            null,
            Combustion.ResultHydrazine
        }
    };

    // ReSharper disable once InconsistentNaming, since Harmony relies on the argument name to change the result of the original method
    [HarmonyPatch(typeof(Combustion), nameof(Combustion.CombustMoles))]
    private static bool CombustMolesPrefix(Mole fuel, Mole oxidiser, double combustionRatio, out MoleEnergy combustionEnergy, out MoleQuantity combustedFuel, out float cleanBurnRatio, ref GasMixture __result)
    {
        var combustionResult = (fuel.Type, oxidiser.Type) switch
        {
            (GasType.Methane, GasType.Oxygen) => new CombustionResult(1, 2, new CombustionValue[] { new(GasType.CarbonDioxide, 1), new(GasType.Steam, 2) }),
            _ => CombustionResult.Invalid
        };
        if (combustionResult.IsValid())
        {
            Plugin.Logger?.LogInfo($"Running combustion with fuel ({fuel.Type}, {fuel.Quantity.ToDouble()}, {fuel.Energy.ToDouble()}), oxidiser ({oxidiser.Type}, {oxidiser.Quantity.ToDouble()}, {oxidiser.Energy.ToDouble()}), ratio {combustionRatio}");
            __result = combustionResult.RunCombustion(fuel, oxidiser, combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio);
            Plugin.Logger?.LogInfo($"Run combustion, got {combustionEnergy.ToDouble()}, {combustedFuel.ToDouble()}, {cleanBurnRatio}");
            return false;
        }
        combustionEnergy = MoleEnergy.Zero;
        combustedFuel = MoleQuantity.Zero;
        cleanBurnRatio = 1f;
        return true;
    }
}
