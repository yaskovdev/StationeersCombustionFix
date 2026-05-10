namespace StationeersCombustionFix;

using Assets.Scripts.Objects.Items;
using HarmonyLib;

[HarmonyPatch(typeof(WeldingTorch), nameof(WeldingTorch.OnAtmosphericTick))]
internal class OnAtmosphericTickPatch
{
    private static bool Prefix(ref WeldingTorch __instance)
    {
        Plugin.Logger?.LogInfo($"Welding torch internal atmosphere is {__instance.InternalAtmosphere.GasMixture.DebugPrint()}");
        return true;
    }
}
