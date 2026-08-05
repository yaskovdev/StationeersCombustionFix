# Stationeers Combustion Fix

Fixes incorrect methane combustion reactions to be chemically accurate.

![Stationeers Combustion Fix](About/Preview.png)

## Methane + Oxygen

Replaces the incorrect reaction `2 CH₄ + O₂ → 6 CO₂ + 3 Pol` with the correct reaction `CH₄ + 2 O₂ → CO₂ + 2 H₂O`.

Recommended fuel mixture: 33% methane, 67% oxygen.

Always enabled.

## Methane + Nitrous Oxide

Replaces the incorrect reaction `CH₄ + N₂O → 2 CO₂ + 2 N₂` with the correct reaction `CH₄ + 4 N₂O → CO₂ + 2 H₂O + 4 N₂`.

Recommended fuel mixture: 20% methane, 80% nitrous oxide.

Enabled by default. Can be disabled in the StationeersLaunchPad configuration window or in `<GameDir>\BepInEx\config\StationeersCombustionFix.cfg` (where `<GameDir>` is the game folder, e.g. `C:\Program Files (x86)\Steam\steamapps\common\Stationeers`).

## Methane + Ozone

Replaces the incorrect reaction `3 CH₄ + 2 O₃ → 6 CO₂ + 3 Pol + H₂O` with the correct reaction `3 CH₄ + 4 O₃ → 3 CO₂ + 6 H₂O`.

Recommended fuel mixture: 42% methane, 58% ozone.

Enabled by default. Can be disabled in the StationeersLaunchPad configuration window or in `<GameDir>\BepInEx\config\StationeersCombustionFix.cfg` (where `<GameDir>` is the game folder, e.g. `C:\Program Files (x86)\Steam\steamapps\common\Stationeers`).

## Optional Starting Fuel Mixture Fix

The game's data-driven starting equipment still uses the old fuel ratio: 67% methane and 33% oxygen. With the corrected combustion reaction, this leaves excess methane in the starting welder and can ignite oxygen-rich surroundings.

When `PatchStartingFuelMixtures` is enabled, the mod recognizes the exact legacy 2:1 methane/oxygen mixture in new-world, new-player, and respawn spawn data and changes it to 1:2 while preserving the total quantity and pressure. This covers the built-in welder canister and portable fuel tanks, plus custom start conditions that use the same spawn system. A custom jetpack fuel canister is covered too; the vanilla jetpack currently starts with nitrogen.

Disabled by default. The setting only affects newly spawned equipment; existing tanks and saved contents are not changed.

## Notes

* The mod does not modify save files, so it can be safely enabled or disabled at any time.
* Applies corrections at the reaction-definition level, so fuel allocation, consumption, and exhaust calculations use consistent stoichiometry. A correctly mixed welder therefore leaves no unburned fuel.
* Tested with the Gas Fuel Generator (GFG). Combustion reaches the expected 90% ratio.

## Requirements

This mod is a BepInEx plugin. It requires BepInEx with the [StationeersLaunchPad](https://github.com/StationeersLaunchPad/StationeersLaunchPad) plugin to be installed. See the StationeersLaunchPad repository for the detailed install guide.

BepInEx only loads plugins from its own `BepInEx/plugins` folder, while subscribed Workshop mods live in Steam's workshop content folder. StationeersLaunchPad is the loader that bridges the two: it discovers the subscribed mod and hands its assembly to BepInEx. Without a loader, a subscribed-only mod is downloaded but never loaded.

## Installing the Mod (for Players)

1. Install BepInEx with the StationeersLaunchPad plugin (see the [StationeersLaunchPad](https://github.com/StationeersLaunchPad/StationeersLaunchPad) guide).
2. Subscribe to the mod on the Steam Workshop.
3. Launch the game. StationeersLaunchPad loads the mod automatically; enable it in the loader window at the bottom of the loading screen if needed.

Alternatively, without a loader, install BepInEx and copy `StationeersCombustionFix.dll` into `BepInEx/plugins` manually.

## Installing the Mod (for Developers)

Before building, make sure there are no conflicting copies of the mod:

1. Unsubscribe from the mod in Steam Workshop (if subscribed).
2. Verify there is no `StationeersCombustionFix.dll` in `<GameDir>\BepInEx\plugins\` (where `<GameDir>` is your Stationeers installation path, e.g. `C:\Program Files (x86)\Steam\steamapps\common\Stationeers`).

Then run the build script:

```powershell
.\Build-Plugin.ps1
```

This builds the plugin in Release configuration and deploys it (along with the `About` folder) to `Documents\My Games\Stationeers\mods\StationeersCombustionFix\`.

Launch the game. StationeersLaunchPad will pick up the mod automatically.

## Configuration

The mod exposes the following BepInEx settings (section `General`):

* `PatchMethaneNitrousReaction` (default `true`): when enabled, also patches the methane + nitrous oxide combustion reaction.
* `PatchMethaneOzoneReaction` (default `true`): when enabled, also patches the methane + ozone combustion reaction.
* `PatchStartingFuelMixtures` (default `false`): corrects exact legacy 2:1 methane/oxygen mixtures in newly spawned starting and respawn equipment to 1:2.

The methane + oxygen patch is always applied and cannot be disabled. You can toggle the optional patches in the StationeersLaunchPad configuration window at startup, or by editing the generated `<GameDir>\BepInEx\config\StationeersCombustionFix.cfg` file.

## Setting Up the Project

The project requires a reference to `Assembly-CSharp.dll` from your local Stationeers installation. This file is not included in the repository.

Running unit tests additionally requires `UnityEngine.dll` and `UnityEngine.CoreModule.dll` from your local Stationeers installation.

1. Copy `Directory.Build.props.example` to `Directory.Build.props` (in the repository root):
   ```
   cp Directory.Build.props.example Directory.Build.props
   ```
2. Open `Directory.Build.props` and set `GameDir` to your Stationeers installation path:
   * **Windows:** `c:\Program Files (x86)\Steam\steamapps\common\Stationeers`
   * **macOS:** `/Users/yaskovdev/Library/Application Support/Steam/steamapps/common/Stationeers`

   `Directory.Build.props` is ignored in Git, so this change stays local to your machine.
3. Run `dotnet clean` and `dotnet build` to build the project.

## Publishing to Steam Workshop

The same steps apply for both the initial publish and subsequent updates.

1. Update the version in both `StationeersCombustionFix/StationeersCombustionFix.csproj` and `About/About.xml`. Keep the two values identical.
2. Run `.\Build-Plugin.ps1` to build and deploy the mod locally.
3. Launch Stationeers, then go to Workshop. You'll see the mod and the Publish/Update button.
