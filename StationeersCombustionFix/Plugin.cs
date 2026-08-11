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
            true,
            "Patch the methane + nitrous oxide combustion reaction. Corrects CH4 + N2O -> 2 CO2 + 2 N2 to CH4 + 4 N2O -> CO2 + 2 H2O + 4 N2. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchMethaneNitrousReaction = () => patchMethaneNitrousReaction.Value;

        var patchMethaneOzoneReaction = Config.Bind(
            "General",
            "PatchMethaneOzoneReaction",
            true,
            "Patch the methane + ozone combustion reaction. Corrects 3 CH4 + 2 O3 -> 6 CO2 + 3 Pol + H2O to 3 CH4 + 4 O3 -> 3 CO2 + 6 H2O. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchMethaneOzoneReaction = () => patchMethaneOzoneReaction.Value;

        var patchHydrogenOxygenReaction = Config.Bind(
            "General",
            "PatchHydrogenOxygenReaction",
            false,
            "Patch the hydrogen + oxygen combustion reaction. Corrects 2 H2 + O2 -> 3 H2O to 2 H2 + O2 -> 2 H2O. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchHydrogenOxygenReaction = () => patchHydrogenOxygenReaction.Value;

        var patchHydrogenOzoneReaction = Config.Bind(
            "General",
            "PatchHydrogenOzoneReaction",
            false,
            "Patch the hydrogen + ozone combustion reaction. Corrects 3 H2 + O3 -> 4 H2O to 3 H2 + O3 -> 3 H2O. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchHydrogenOzoneReaction = () => patchHydrogenOzoneReaction.Value;

        var patchAlcoholOxygenReaction = Config.Bind(
            "General",
            "PatchAlcoholOxygenReaction",
            false,
            "Patch the alcohol + oxygen combustion reaction, treating alcohol as ethanol (C2H6O). Corrects Alc + 3 O2 -> 8 CO2 + 2 H2O to C2H6O + 3 O2 -> 2 CO2 + 3 H2O. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchAlcoholOxygenReaction = () => patchAlcoholOxygenReaction.Value;

        var patchAlcoholNitrousReaction = Config.Bind(
            "General",
            "PatchAlcoholNitrousReaction",
            false,
            "Patch the alcohol + nitrous oxide combustion reaction, treating alcohol as ethanol (C2H6O). Corrects Alc + 2 N2O -> 4 N2 + 2 H2O to C2H6O + 6 N2O -> 2 CO2 + 3 H2O + 6 N2. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchAlcoholNitrousReaction = () => patchAlcoholNitrousReaction.Value;

        var patchAlcoholOzoneReaction = Config.Bind(
            "General",
            "PatchAlcoholOzoneReaction",
            false,
            "Patch the alcohol + ozone combustion reaction, treating alcohol as ethanol (C2H6O). Corrects Alc + 2 O3 -> CO2 + 3 H2O to C2H6O + 2 O3 -> 2 CO2 + 3 H2O. The methane + oxygen patch is always applied and has no toggle.");
        CombustionResultPatch.PatchAlcoholOzoneReaction = () => patchAlcoholOzoneReaction.Value;

        var patchStartingFuelMixtures = Config.Bind(
            "General",
            "PatchStartingFuelMixtures",
            true,
            "Correct exact 2:1 methane + oxygen mixtures in loaded spawn data to 1:2 while preserving total quantity. Covers vanilla starts, respawns, tutorials, creative spawn-menu entries, and compatible custom spawn data. Existing equipment is unchanged. Restart after changing this setting.");
        StartingFuelMixturePatch.PatchStartingFuelMixtures = () => patchStartingFuelMixtures.Value;

        var harmony = new Harmony("com.yaskovdev.stationeerscombustionfix");
        harmony.PatchAll(typeof(Plugin).Assembly);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded! Methane + nitrous oxide patch enabled: {patchMethaneNitrousReaction.Value}, methane + ozone patch enabled: {patchMethaneOzoneReaction.Value}, starting fuel mixture patch enabled: {patchStartingFuelMixtures.Value}");
    }
}
