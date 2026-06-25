namespace StationeersCombustionFix;

using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        var patchMethaneOzoneReaction = Config.Bind(
            "General",
            "PatchMethaneOzoneReaction",
            false,
            "Also patch the methane + ozone combustion reaction. Disabled by default; the methane + oxygen patch is always applied.");

        // TODO: test that changes in config in SLP are applied
        new CombustionResultPatch(patchMethaneOzoneReaction.Value, Logger).PatchReactions();
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded! Methane + ozone patch enabled: {patchMethaneOzoneReaction.Value}");
    }
}
