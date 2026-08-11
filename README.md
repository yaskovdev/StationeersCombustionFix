# Stationeers Combustion Fix [![Daily game compatibility](https://github.com/yaskovdev/StationeersCombustionFix/actions/workflows/daily-game-compatibility.yml/badge.svg)](https://github.com/yaskovdev/StationeersCombustionFix/actions/workflows/daily-game-compatibility.yml)

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

The vanilla game's data-driven spawn recipes use a fuel ratio of 67% methane and 33% oxygen. With the corrected combustion reaction, this leaves excess methane in the starting welder and can ignite oxygen-rich surroundings.

When `PatchStartingFuelMixtures` is enabled, the mod recognizes every exact 2:1 methane/oxygen mixture while spawn data is loaded and changes it to 1:2 while preserving the total quantity and pressure. This covers the built-in fuel tank, fuel packages, starting and tutorial welders, creative spawn-menu entries, and compatible custom spawn data. A custom jetpack fuel canister is covered too; the vanilla jetpack currently starts with nitrogen.

Enabled by default. It can be disabled in the mod configuration. The corrected recipes affect only equipment spawned afterward, so existing tanks and saved contents are not changed. Restart the game after changing this setting.

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
* `PatchHydrogenOxygenReaction` (default `false`): patches the hydrogen + oxygen combustion reaction.
* `PatchHydrogenOzoneReaction` (default `false`): patches the hydrogen + ozone combustion reaction.
* `PatchAlcoholOxygenReaction` (default `false`): patches the alcohol + oxygen combustion reaction, treating alcohol as ethanol.
* `PatchAlcoholNitrousReaction` (default `false`): patches the alcohol + nitrous oxide combustion reaction, treating alcohol as ethanol.
* `PatchAlcoholOzoneReaction` (default `false`): patches the alcohol + ozone combustion reaction, treating alcohol as ethanol.
* `PatchStartingFuelMixtures` (default `true`): corrects exact 2:1 methane/oxygen mixtures in loaded spawn data to 1:2.

The methane + oxygen patch is always applied and cannot be disabled. You can toggle the optional patches in the StationeersLaunchPad configuration window at startup, or by editing the generated `<GameDir>\BepInEx\config\StationeersCombustionFix.cfg` file.

## Setting Up the Project

The project requires a reference to `Assembly-CSharp.dll` from your local Stationeers installation. This file is not included in the repository.

Running unit tests additionally requires `UnityEngine.dll` and `UnityEngine.CoreModule.dll` from your local Stationeers installation.

GitHub Actions also runs the tests every three hours against the current Windows dedicated-server assemblies and start-condition data downloaded anonymously from Steam. The game files remain ephemeral and are not committed or uploaded as workflow artifacts.

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

## Compatibility and Dependency Maintenance

* `.github/workflows/daily-game-compatibility.yml` runs every three hours (and on pushes to `master` or manually), downloads only the required files from the public Stationeers dedicated-server depot, and tests against the current game data. Downloaded game files are temporary.
* NuGet versions are pinned by the committed `packages.lock.json` files. To update dependencies, edit the relevant `PackageReference` if needed, run `dotnet restore StationeersCombustionFix.sln --force-evaluate`, test, and commit the resulting project and lock-file changes together. CI uses `--locked-mode`, so stale lock files fail the build.
* GitHub disables scheduled workflows in public repositories after 60 days without repository activity. If this happens, re-enable this workflow from the Actions tab or run `gh workflow enable daily-game-compatibility.yml`.
