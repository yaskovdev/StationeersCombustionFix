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

        var patchMethaneOzoneReaction = Config.Bind(
            "General",
            "PatchMethaneOzoneReaction",
            false,
            "Also patch the methane + ozone combustion reaction. Disabled by default; the methane + oxygen patch is always applied.");
        CombustionResultPatch.PatchMethaneOzoneReaction = () => patchMethaneOzoneReaction.Value;

        var harmony = new Harmony("com.yaskovdev.stationeerscombustionfix");
        harmony.PatchAll(typeof(Plugin).Assembly);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded! Methane + ozone patch enabled: {patchMethaneOzoneReaction.Value}");
    }
}
