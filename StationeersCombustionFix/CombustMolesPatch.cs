namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using HarmonyLib;

[HarmonyPatch(typeof(Combustion), nameof(Combustion.CombustMoles))]
internal class CombustMolesPatch
{
    // ReSharper disable once InconsistentNaming, since Harmony relies on the argument name to change the result of the original method
    private static bool Prefix(Mole fuel, Mole oxidiser, double combustionRatio, out MoleEnergy combustionEnergy, out MoleQuantity combustedFuel, out float cleanBurnRatio, ref GasMixture __result)
    {
        Plugin.Logger?.LogInfo("Running combustion");
        CombustionResult result = !fuel.IsValid || !oxidiser.IsValid ? CombustionResult.Invalid : Shared.DataPatch[Combustion.FuelIndex(fuel.Type), Combustion.OxidiserIndex(oxidiser.Type)];
        if (CombustionResult.IsValid(result))
        {
            Plugin.Logger?.LogInfo($"Running combustion with fuel ({fuel.Type}, {fuel.Quantity.ToDouble()}, {fuel.Energy.ToDouble()}), oxidiser ({oxidiser.Type}, {oxidiser.Quantity.ToDouble()}, {oxidiser.Energy.ToDouble()}), combustion ratio {combustionRatio}");
            if (combustionRatio < 1)
            {
                Plugin.Logger?.LogInfo($"Combustion ratio was {combustionRatio}, forcing 1");
            }
            var mixture = result.RunCombustion(fuel, oxidiser, combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio);
            __result = mixture;
            Plugin.Logger?.LogInfo($"Run combustion, got mixture: {mixture.DebugPrint()}, combustion energy: {combustionEnergy.ToDouble()}, combusted fuel: {combustedFuel.ToDouble()}, clean burn ratio: {cleanBurnRatio}");
            return false;
        }
        GasMixture gasMixture = GasMixtureHelper.Create();
        gasMixture.Add(fuel);
        gasMixture.Add(oxidiser);
        combustionEnergy = MoleEnergy.Zero;
        combustedFuel = MoleQuantity.Zero;
        cleanBurnRatio = 1f;
        __result = gasMixture;
        return false;
    }
}
