namespace StationeersCombustionFix;

using Assets.Scripts.Atmospherics;
using Assets.Scripts.Util;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(GasMixture), nameof(GasMixture.Combust))]
public class CombustPatch
{
    private static bool Prefix(double combustionRatio, out MoleQuantity burnedFuel, out float cleanBurnRatio, ref GasMixture __instance, ref MoleEnergy __result)
    {
        Plugin.Logger?.LogInfo($"Calling {nameof(GasMixture.Combust)}");
        Plugin.Logger?.LogInfo($"Methane before combustion: {__instance.Methane.Quantity.ToDouble()}");
        Plugin.Logger?.LogInfo($"Oxygen before combustion: {__instance.Oxygen.Quantity.ToDouble()}");
        burnedFuel = MoleQuantity.Zero;
        cleanBurnRatio = 1f;
        MoleEnergy zero = MoleEnergy.Zero;
        MoleQuantity totalFuel = __instance.TotalFuel;
        MoleQuantity totalOxidiser = __instance.TotalOxidiser;
        MoleQuantity totalHypergolics = __instance.TotalHypergolics;
        if (totalHypergolics <= MoleQuantity.Zero && (totalFuel <= MoleQuantity.Zero || totalOxidiser <= MoleQuantity.Zero))
        {
            __result = zero;
            return false;
        }
        GasMixture newGasMix = GasMixtureHelper.Create();
        if (totalHypergolics > MoleQuantity.Zero)
        {
            MoleQuantity quantity1 = __instance.Hydrazine.Quantity;
            if (quantity1 > MoleQuantity.Zero)
            {
                MoleEnergy combustionEnergy;
                MoleQuantity combustedFuel;
                float cleanBurnRatio1;
                newGasMix.Add(Combustion.CombustMoles(__instance.Hydrazine.Remove(quantity1 / 2.0), __instance.Hydrazine.Remove(quantity1 / 2.0), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio1));
                zero += combustionEnergy;
                burnedFuel += combustedFuel;
                cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio1, 1f);
            }
            MoleQuantity quantity2 = __instance.LiquidHydrazine.Quantity;
            if (quantity2 > MoleQuantity.Zero)
            {
                MoleEnergy combustionEnergy;
                MoleQuantity combustedFuel;
                float cleanBurnRatio2;
                newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrazine.Remove(quantity2 / 2.0), __instance.LiquidHydrazine.Remove(quantity2 / 2.0), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio2));
                zero += combustionEnergy;
                burnedFuel += combustedFuel;
                cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio2, 1f);
            }
        }
        if (totalOxidiser > MoleQuantity.Zero && totalFuel > MoleQuantity.Zero)
        {
            MoleQuantity quantity3 = __instance.Methane.Quantity;
            MoleQuantity quantity4 = __instance.LiquidMethane.Quantity;
            MoleQuantity quantity5 = __instance.Hydrogen.Quantity;
            MoleQuantity quantity6 = __instance.LiquidHydrogen.Quantity;
            MoleQuantity quantity7 = __instance.LiquidAlcohol.Quantity;
            MoleQuantity quantity8 = __instance.Oxygen.Quantity;
            MoleQuantity quantity9 = __instance.LiquidOxygen.Quantity;
            MoleQuantity quantity10 = __instance.NitrousOxide.Quantity;
            MoleQuantity quantity11 = __instance.LiquidNitrousOxide.Quantity;
            MoleQuantity quantity12 = __instance.Ozone.Quantity;
            MoleQuantity quantity13 = __instance.LiquidOzone.Quantity;
            double num1 = (quantity3 / totalFuel).ToDouble();
            double num2 = (quantity4 / totalFuel).ToDouble();
            double num3 = (quantity5 / totalFuel).ToDouble();
            double num4 = (quantity6 / totalFuel).ToDouble();
            double num5 = (quantity7 / totalFuel).ToDouble();
            double num6 = (quantity8 / totalOxidiser).ToDouble();
            MoleQuantity moleQuantity1 = quantity9 / totalOxidiser;
            double num7 = moleQuantity1.ToDouble();
            moleQuantity1 = quantity10 / totalOxidiser;
            double num8 = moleQuantity1.ToDouble();
            moleQuantity1 = quantity11 / totalOxidiser;
            double num9 = moleQuantity1.ToDouble();
            double num10 = (quantity12 / totalOxidiser).ToDouble();
            double num11 = (quantity13 / totalOxidiser).ToDouble();
            MoleQuantity moleQuantity2 = quantity3 * Shared.ResultMethaneOxygenPatch.OxidiserRatio;
            MoleQuantity moleQuantity3 = quantity4 * Shared.ResultMethaneOxygenPatch.OxidiserRatio;
            MoleQuantity moleQuantity4 = quantity5 * Combustion.ResultHydrogenOxygen.OxidiserRatio;
            MoleQuantity moleQuantity5 = quantity6 * Combustion.ResultHydrogenOxygen.OxidiserRatio;
            MoleQuantity moleQuantity6 = quantity7 * Combustion.ResultAlcoholOxygen.OxidiserRatio;
            MoleQuantity moleQuantity7 = moleQuantity2 + moleQuantity3 + moleQuantity4 + moleQuantity5 + moleQuantity6;
            MoleQuantity moleQuantity8 = moleQuantity7 > MoleQuantity.Zero ? RocketMath.Min(quantity8 / moleQuantity7, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles1 = moleQuantity2 * moleQuantity8;
            MoleQuantity removedMoles2 = moleQuantity3 * moleQuantity8;
            MoleQuantity removedMoles3 = moleQuantity4 * moleQuantity8;
            MoleQuantity removedMoles4 = moleQuantity5 * moleQuantity8;
            MoleQuantity removedMoles5 = moleQuantity6 * moleQuantity8;
            MoleQuantity moleQuantity9 = quantity3 * Shared.ResultMethaneOxygenPatch.OxidiserRatio;
            MoleQuantity moleQuantity10 = quantity4 * Shared.ResultMethaneOxygenPatch.OxidiserRatio;
            MoleQuantity moleQuantity11 = quantity5 * Combustion.ResultHydrogenOxygen.OxidiserRatio;
            MoleQuantity moleQuantity12 = quantity6 * Combustion.ResultHydrogenOxygen.OxidiserRatio;
            MoleQuantity moleQuantity13 = quantity7 * Combustion.ResultAlcoholOxygen.OxidiserRatio;
            MoleQuantity moleQuantity14 = moleQuantity9 + moleQuantity10 + moleQuantity11 + moleQuantity12 + moleQuantity13;
            MoleQuantity moleQuantity15 = moleQuantity14 > MoleQuantity.Zero ? RocketMath.Min(quantity9 / moleQuantity14, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles6 = moleQuantity9 * moleQuantity15;
            MoleQuantity removedMoles7 = moleQuantity10 * moleQuantity15;
            MoleQuantity removedMoles8 = moleQuantity11 * moleQuantity15;
            MoleQuantity removedMoles9 = moleQuantity12 * moleQuantity15;
            MoleQuantity removedMoles10 = moleQuantity13 * moleQuantity15;
            MoleQuantity moleQuantity16 = quantity3 * Combustion.ResultMethaneNitrous.OxidiserRatio;
            MoleQuantity moleQuantity17 = quantity4 * Combustion.ResultMethaneNitrous.OxidiserRatio;
            MoleQuantity moleQuantity18 = quantity5 * Combustion.ResultHydrogenNitrous.OxidiserRatio;
            MoleQuantity moleQuantity19 = quantity6 * Combustion.ResultHydrogenNitrous.OxidiserRatio;
            MoleQuantity moleQuantity20 = quantity7 * Combustion.ResultAlcoholNitrous.OxidiserRatio;
            MoleQuantity moleQuantity21 = moleQuantity16 + moleQuantity17 + moleQuantity18 + moleQuantity19 + moleQuantity20;
            MoleQuantity moleQuantity22 = moleQuantity21 > MoleQuantity.Zero ? RocketMath.Min(quantity10 / moleQuantity21, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles11 = moleQuantity16 * moleQuantity22;
            MoleQuantity removedMoles12 = moleQuantity17 * moleQuantity22;
            MoleQuantity removedMoles13 = moleQuantity18 * moleQuantity22;
            MoleQuantity removedMoles14 = moleQuantity19 * moleQuantity22;
            MoleQuantity removedMoles15 = moleQuantity20 * moleQuantity22;
            MoleQuantity moleQuantity23 = quantity3 * Combustion.ResultMethaneNitrous.OxidiserRatio;
            MoleQuantity moleQuantity24 = quantity4 * Combustion.ResultMethaneNitrous.OxidiserRatio;
            MoleQuantity moleQuantity25 = quantity5 * Combustion.ResultHydrogenNitrous.OxidiserRatio;
            MoleQuantity moleQuantity26 = quantity6 * Combustion.ResultHydrogenNitrous.OxidiserRatio;
            MoleQuantity moleQuantity27 = quantity7 * Combustion.ResultAlcoholNitrous.OxidiserRatio;
            MoleQuantity moleQuantity28 = moleQuantity23 + moleQuantity24 + moleQuantity25 + moleQuantity26 + moleQuantity27;
            MoleQuantity moleQuantity29 = moleQuantity28 > MoleQuantity.Zero ? RocketMath.Min(quantity11 / moleQuantity28, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles16 = moleQuantity23 * moleQuantity29;
            MoleQuantity removedMoles17 = moleQuantity24 * moleQuantity29;
            MoleQuantity removedMoles18 = moleQuantity25 * moleQuantity29;
            MoleQuantity removedMoles19 = moleQuantity26 * moleQuantity29;
            MoleQuantity removedMoles20 = moleQuantity27 * moleQuantity29;
            MoleQuantity moleQuantity30 = quantity3 * Combustion.ResultMethaneOzone.OxidiserRatio;
            MoleQuantity moleQuantity31 = quantity4 * Combustion.ResultMethaneOzone.OxidiserRatio;
            MoleQuantity moleQuantity32 = quantity5 * Combustion.ResultHydrogenOzone.OxidiserRatio;
            MoleQuantity moleQuantity33 = quantity6 * Combustion.ResultHydrogenOzone.OxidiserRatio;
            MoleQuantity moleQuantity34 = quantity7 * Combustion.ResultAlcoholOzone.OxidiserRatio;
            MoleQuantity moleQuantity35 = moleQuantity30 + moleQuantity31 + moleQuantity32 + moleQuantity33 + moleQuantity34;
            MoleQuantity moleQuantity36 = moleQuantity35 > MoleQuantity.Zero ? RocketMath.Min(quantity12 / moleQuantity35, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles21 = moleQuantity30 * moleQuantity36;
            MoleQuantity removedMoles22 = moleQuantity31 * moleQuantity36;
            MoleQuantity removedMoles23 = moleQuantity32 * moleQuantity36;
            MoleQuantity removedMoles24 = moleQuantity33 * moleQuantity36;
            MoleQuantity removedMoles25 = moleQuantity34 * moleQuantity36;
            MoleQuantity moleQuantity37 = quantity3 * Combustion.ResultMethaneOzone.OxidiserRatio;
            MoleQuantity moleQuantity38 = quantity4 * Combustion.ResultMethaneOzone.OxidiserRatio;
            MoleQuantity moleQuantity39 = quantity5 * Combustion.ResultHydrogenOzone.OxidiserRatio;
            MoleQuantity moleQuantity40 = quantity6 * Combustion.ResultHydrogenOzone.OxidiserRatio;
            MoleQuantity moleQuantity41 = quantity7 * Combustion.ResultAlcoholOzone.OxidiserRatio;
            MoleQuantity moleQuantity42 = moleQuantity37 + moleQuantity38 + moleQuantity39 + moleQuantity40 + moleQuantity41;
            MoleQuantity moleQuantity43 = moleQuantity42 > MoleQuantity.Zero ? RocketMath.Min(quantity13 / moleQuantity42, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles26 = moleQuantity37 * moleQuantity43;
            MoleQuantity removedMoles27 = moleQuantity38 * moleQuantity43;
            MoleQuantity removedMoles28 = moleQuantity39 * moleQuantity43;
            MoleQuantity removedMoles29 = moleQuantity40 * moleQuantity43;
            MoleQuantity removedMoles30 = moleQuantity41 * moleQuantity43;
            MoleQuantity moleQuantity44 = removedMoles1 * Shared.ResultMethaneOxygenPatch.FuelRatio;
            MoleQuantity moleQuantity45 = removedMoles6 * Shared.ResultMethaneOxygenPatch.FuelRatio;
            MoleQuantity moleQuantity46 = removedMoles11 * Combustion.ResultMethaneNitrous.FuelRatio;
            MoleQuantity moleQuantity47 = removedMoles16 * Combustion.ResultMethaneNitrous.FuelRatio;
            MoleQuantity moleQuantity48 = removedMoles21 * Combustion.ResultMethaneOzone.FuelRatio;
            MoleQuantity moleQuantity49 = removedMoles26 * Combustion.ResultMethaneOzone.FuelRatio;
            MoleQuantity moleQuantity50 = moleQuantity44 + moleQuantity45 + moleQuantity46 + moleQuantity47 + moleQuantity48 + moleQuantity49;
            MoleQuantity moleQuantity51 = moleQuantity50 > MoleQuantity.Zero ? RocketMath.Min(quantity3 / moleQuantity50, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles31 = moleQuantity44 * moleQuantity51;
            MoleQuantity removedMoles32 = moleQuantity45 * moleQuantity51;
            MoleQuantity removedMoles33 = moleQuantity46 * moleQuantity51;
            MoleQuantity removedMoles34 = moleQuantity47 * moleQuantity51;
            MoleQuantity removedMoles35 = moleQuantity48 * moleQuantity51;
            MoleQuantity removedMoles36 = moleQuantity49 * moleQuantity51;
            MoleQuantity moleQuantity52 = removedMoles2 * Shared.ResultMethaneOxygenPatch.FuelRatio;
            MoleQuantity moleQuantity53 = removedMoles7 * Shared.ResultMethaneOxygenPatch.FuelRatio;
            MoleQuantity moleQuantity54 = removedMoles12 * Combustion.ResultMethaneNitrous.FuelRatio;
            MoleQuantity moleQuantity55 = removedMoles17 * Combustion.ResultMethaneNitrous.FuelRatio;
            MoleQuantity moleQuantity56 = removedMoles22 * Combustion.ResultMethaneOzone.FuelRatio;
            MoleQuantity moleQuantity57 = removedMoles27 * Combustion.ResultMethaneOzone.FuelRatio;
            MoleQuantity moleQuantity58 = moleQuantity52 + moleQuantity53 + moleQuantity54 + moleQuantity55 + moleQuantity56 + moleQuantity57;
            MoleQuantity moleQuantity59 = moleQuantity58 > MoleQuantity.Zero ? RocketMath.Min(quantity4 / moleQuantity58, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles37 = moleQuantity52 * moleQuantity59;
            MoleQuantity removedMoles38 = moleQuantity53 * moleQuantity59;
            MoleQuantity removedMoles39 = moleQuantity54 * moleQuantity59;
            MoleQuantity removedMoles40 = moleQuantity55 * moleQuantity59;
            MoleQuantity removedMoles41 = moleQuantity56 * moleQuantity59;
            MoleQuantity removedMoles42 = moleQuantity57 * moleQuantity59;
            MoleQuantity moleQuantity60 = removedMoles3 * Combustion.ResultHydrogenOxygen.FuelRatio;
            MoleQuantity moleQuantity61 = removedMoles8 * Combustion.ResultHydrogenOxygen.FuelRatio;
            MoleQuantity moleQuantity62 = removedMoles13 * Combustion.ResultHydrogenNitrous.FuelRatio;
            MoleQuantity moleQuantity63 = removedMoles18 * Combustion.ResultHydrogenNitrous.FuelRatio;
            MoleQuantity moleQuantity64 = removedMoles23 * Combustion.ResultHydrogenOzone.FuelRatio;
            MoleQuantity moleQuantity65 = removedMoles28 * Combustion.ResultHydrogenOzone.FuelRatio;
            MoleQuantity moleQuantity66 = moleQuantity60 + moleQuantity61 + moleQuantity62 + moleQuantity63 + moleQuantity64 + moleQuantity65;
            MoleQuantity moleQuantity67 = moleQuantity66 > MoleQuantity.Zero ? RocketMath.Min(quantity5 / moleQuantity66, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles43 = moleQuantity60 * moleQuantity67;
            MoleQuantity removedMoles44 = moleQuantity61 * moleQuantity67;
            MoleQuantity removedMoles45 = moleQuantity62 * moleQuantity67;
            MoleQuantity removedMoles46 = moleQuantity63 * moleQuantity67;
            MoleQuantity removedMoles47 = moleQuantity64 * moleQuantity67;
            MoleQuantity removedMoles48 = moleQuantity65 * moleQuantity67;
            MoleQuantity moleQuantity68 = removedMoles4 * Combustion.ResultHydrogenOxygen.FuelRatio;
            MoleQuantity moleQuantity69 = removedMoles9 * Combustion.ResultHydrogenOxygen.FuelRatio;
            MoleQuantity moleQuantity70 = removedMoles14 * Combustion.ResultHydrogenNitrous.FuelRatio;
            MoleQuantity moleQuantity71 = removedMoles19 * Combustion.ResultHydrogenNitrous.FuelRatio;
            MoleQuantity moleQuantity72 = removedMoles24 * Combustion.ResultHydrogenOzone.FuelRatio;
            MoleQuantity moleQuantity73 = removedMoles29 * Combustion.ResultHydrogenOzone.FuelRatio;
            MoleQuantity moleQuantity74 = moleQuantity68 + moleQuantity69 + moleQuantity70 + moleQuantity71 + moleQuantity72 + moleQuantity73;
            MoleQuantity moleQuantity75 = moleQuantity74 > MoleQuantity.Zero ? RocketMath.Min(quantity6 / moleQuantity74, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles49 = moleQuantity68 * moleQuantity75;
            MoleQuantity removedMoles50 = moleQuantity69 * moleQuantity75;
            MoleQuantity removedMoles51 = moleQuantity70 * moleQuantity75;
            MoleQuantity removedMoles52 = moleQuantity71 * moleQuantity75;
            MoleQuantity removedMoles53 = moleQuantity72 * moleQuantity75;
            MoleQuantity removedMoles54 = moleQuantity73 * moleQuantity75;
            MoleQuantity moleQuantity76 = removedMoles5 * Combustion.ResultAlcoholOxygen.FuelRatio;
            MoleQuantity moleQuantity77 = removedMoles10 * Combustion.ResultAlcoholOxygen.FuelRatio;
            MoleQuantity moleQuantity78 = removedMoles15 * Combustion.ResultAlcoholNitrous.FuelRatio;
            MoleQuantity moleQuantity79 = removedMoles20 * Combustion.ResultAlcoholNitrous.FuelRatio;
            MoleQuantity moleQuantity80 = removedMoles25 * Combustion.ResultAlcoholOzone.FuelRatio;
            MoleQuantity moleQuantity81 = removedMoles30 * Combustion.ResultAlcoholOzone.FuelRatio;
            MoleQuantity moleQuantity82 = moleQuantity76 + moleQuantity77 + moleQuantity78 + moleQuantity79 + moleQuantity80 + moleQuantity81;
            MoleQuantity moleQuantity83 = moleQuantity82 > MoleQuantity.Zero ? RocketMath.Min(quantity7 / moleQuantity82, MoleQuantity.One) : MoleQuantity.One;
            MoleQuantity removedMoles55 = moleQuantity76 * moleQuantity83;
            MoleQuantity removedMoles56 = moleQuantity77 * moleQuantity83;
            MoleQuantity removedMoles57 = moleQuantity78 * moleQuantity83;
            MoleQuantity removedMoles58 = moleQuantity79 * moleQuantity83;
            MoleQuantity removedMoles59 = moleQuantity80 * moleQuantity83;
            MoleQuantity removedMoles60 = moleQuantity81 * moleQuantity83;
            if (quantity3 > MoleQuantity.Zero)
            {
                if (quantity8 > MoleQuantity.Zero)
                {
                    // TODO: __instance.Methane.Remove(removedMoles31) most likely doesn't remove methane fully, therefore it leaks to the atmosphere
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio3;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Methane.Remove(removedMoles31), __instance.Oxygen.Remove(removedMoles1), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio3));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio3, (float)(num6 * num1));
                }
                if (quantity9 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio4;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Methane.Remove(removedMoles32), __instance.LiquidOxygen.Remove(removedMoles6), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio4));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio4, (float)(num7 * num1));
                }
                if (quantity10 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio5;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Methane.Remove(removedMoles33), __instance.NitrousOxide.Remove(removedMoles11), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio5));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio5, (float)(num8 * num1));
                }
                if (quantity11 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio6;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Methane.Remove(removedMoles34), __instance.LiquidNitrousOxide.Remove(removedMoles16), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio6));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio6, (float)(num9 * num1));
                }
                if (quantity12 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio7;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Methane.Remove(removedMoles35), __instance.Ozone.Remove(removedMoles21), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio7));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio7, (float)(num10 * num1));
                }
                if (quantity13 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio8;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Methane.Remove(removedMoles36), __instance.LiquidOzone.Remove(removedMoles26), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio8));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio8, (float)(num11 * num1));
                }
            }
            if (quantity4 > MoleQuantity.Zero)
            {
                if (quantity8 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio9;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidMethane.Remove(removedMoles37), __instance.Oxygen.Remove(removedMoles2), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio9));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio9, (float)(num6 * num2));
                }
                if (quantity9 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio10;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidMethane.Remove(removedMoles38), __instance.LiquidOxygen.Remove(removedMoles7), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio10));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio10, (float)(num7 * num2));
                }
                if (quantity10 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio11;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidMethane.Remove(removedMoles39), __instance.NitrousOxide.Remove(removedMoles12), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio11));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio11, (float)(num8 * num2));
                }
                if (quantity11 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio12;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidMethane.Remove(removedMoles40), __instance.LiquidNitrousOxide.Remove(removedMoles17), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio12));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio12, (float)(num9 * num2));
                }
                if (quantity12 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio13;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidMethane.Remove(removedMoles41), __instance.Ozone.Remove(removedMoles22), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio13));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio13, (float)(num10 * num2));
                }
                if (quantity13 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio14;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidMethane.Remove(removedMoles42), __instance.LiquidOzone.Remove(removedMoles27), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio14));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio14, (float)(num11 * num2));
                }
            }
            if (quantity5 > MoleQuantity.Zero)
            {
                if (quantity8 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio15;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Hydrogen.Remove(removedMoles43), __instance.Oxygen.Remove(removedMoles3), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio15));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio15, (float)(num6 * num3));
                }
                if (quantity9 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio16;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Hydrogen.Remove(removedMoles44), __instance.LiquidOxygen.Remove(removedMoles8), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio16));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio16, (float)(num7 * num3));
                }
                if (quantity10 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio17;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Hydrogen.Remove(removedMoles45), __instance.NitrousOxide.Remove(removedMoles13), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio17));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio17, (float)(num8 * num3));
                }
                if (quantity11 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio18;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Hydrogen.Remove(removedMoles46), __instance.LiquidNitrousOxide.Remove(removedMoles18), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio18));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio18, (float)(num9 * num3));
                }
                if (quantity12 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio19;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Hydrogen.Remove(removedMoles47), __instance.Ozone.Remove(removedMoles23), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio19));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio19, (float)(num10 * num3));
                }
                if (quantity13 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio20;
                    newGasMix.Add(Combustion.CombustMoles(__instance.Hydrogen.Remove(removedMoles48), __instance.LiquidOzone.Remove(removedMoles28), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio20));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio20, (float)(num11 * num3));
                }
            }
            if (quantity6 > MoleQuantity.Zero)
            {
                if (quantity8 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio21;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrogen.Remove(removedMoles49), __instance.Oxygen.Remove(removedMoles4), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio21));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio21, (float)(num6 * num4));
                }
                if (quantity9 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio22;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrogen.Remove(removedMoles50), __instance.LiquidOxygen.Remove(removedMoles9), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio22));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio22, (float)(num7 * num4));
                }
                if (quantity10 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio23;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrogen.Remove(removedMoles51), __instance.NitrousOxide.Remove(removedMoles14), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio23));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio23, (float)(num8 * num4));
                }
                if (quantity11 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio24;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrogen.Remove(removedMoles52), __instance.LiquidNitrousOxide.Remove(removedMoles19), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio24));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio24, (float)(num9 * num4));
                }
                if (quantity12 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio25;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrogen.Remove(removedMoles53), __instance.Ozone.Remove(removedMoles24), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio25));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio25, (float)(num10 * num4));
                }
                if (quantity13 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio26;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidHydrogen.Remove(removedMoles54), __instance.LiquidOzone.Remove(removedMoles29), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio26));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio26, (float)(num11 * num4));
                }
            }
            if (quantity7 > MoleQuantity.Zero)
            {
                if (quantity8 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio27;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidAlcohol.Remove(removedMoles55), __instance.Oxygen.Remove(removedMoles5), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio27));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio27, (float)(num6 * num5));
                }
                if (quantity9 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio28;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidAlcohol.Remove(removedMoles56), __instance.LiquidOxygen.Remove(removedMoles10), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio28));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio28, (float)(num7 * num5));
                }
                if (quantity10 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio29;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidAlcohol.Remove(removedMoles57), __instance.NitrousOxide.Remove(removedMoles15), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio29));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio29, (float)(num8 * num5));
                }
                if (quantity11 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio30;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidAlcohol.Remove(removedMoles58), __instance.LiquidNitrousOxide.Remove(removedMoles20), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio30));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio30, (float)(num9 * num5));
                }
                if (quantity12 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio31;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidAlcohol.Remove(removedMoles59), __instance.Ozone.Remove(removedMoles25), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio31));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio31, (float)(num10 * num5));
                }
                if (quantity13 > MoleQuantity.Zero)
                {
                    MoleEnergy combustionEnergy;
                    MoleQuantity combustedFuel;
                    float cleanBurnRatio32;
                    newGasMix.Add(Combustion.CombustMoles(__instance.LiquidAlcohol.Remove(removedMoles60), __instance.LiquidOzone.Remove(removedMoles30), combustionRatio, out combustionEnergy, out combustedFuel, out cleanBurnRatio32));
                    zero += combustionEnergy;
                    burnedFuel += combustedFuel;
                    cleanBurnRatio = Mathf.Lerp(cleanBurnRatio, cleanBurnRatio32, (float)(num11 * num5));
                }
            }
        }
        __instance.Add(newGasMix);
        Plugin.Logger?.LogInfo($"Methane after combustion: {__instance.Methane.Quantity.ToDouble()}");
        Plugin.Logger?.LogInfo($"Oxygen after combustion: {__instance.Oxygen.Quantity.ToDouble()}");
        __result = zero;
        return false;
    }
}
