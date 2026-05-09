namespace StationeersCombustionFix;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource? Logger;

    private void Awake()
    {
        // Plugin startup logic
        var harmony = new Harmony("com.yaskovdev.stationeerscombustionfix");
        harmony.PatchAll(typeof(Plugin).Assembly);
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
