namespace StationeersCombustionFix;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource? Logger;

    private void Awake()
    {
        Logger = base.Logger;

        var patchMethaneNitrousReaction = Config.Bind(
            "General",
            "PatchMethaneNitrousReaction",
            false,
            "Patch the methane + nitrous oxide combustion reaction. Corrects CH4 + N2O -> 2 CO2 + 2 N2 to CH4 + 4 N2O -> CO2 + 2 H2O + 4 N2. The methane + oxygen patch is always applied.");
        CombustionResultPatch.PatchMethaneNitrousReaction = () => patchMethaneNitrousReaction.Value;

        var patchMethaneOzoneReaction = Config.Bind(
            "General",
            "PatchMethaneOzoneReaction",
            false,
            "Patch the methane + ozone combustion reaction. Corrects 3 CH4 + 2 O3 -> 6 CO2 + 3 Pol + H2O to 3 CH4 + 4 O3 -> 3 CO2 + 6 H2O. The methane + oxygen patch is always applied.");
        CombustionResultPatch.PatchMethaneOzoneReaction = () => patchMethaneOzoneReaction.Value;

        var harmony = new Harmony("com.yaskovdev.stationeerscombustionfix");
        harmony.PatchAll(typeof(Plugin).Assembly);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded! Methane + nitrous oxide patch enabled: {patchMethaneNitrousReaction.Value}, Methane + ozone patch enabled: {patchMethaneOzoneReaction.Value}");
    }
}
