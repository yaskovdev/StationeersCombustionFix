# Copilot Instructions

## Project Overview

A BepInEx plugin (mod) for the game [Stationeers](https://store.steampowered.com/app/544550/Stationeers/) that fixes incorrect methane combustion reactions to be chemically accurate. It uses Harmony to patch `CombustionResult` constructor calls at runtime, replacing the game's incorrect stoichiometry with correct values.

## Build & Test

**Prerequisites:** Copy `Directory.Build.props.example` to `Directory.Build.props` and set `GameDir` to your local Stationeers installation path. This file is git-ignored. The project references `Assembly-CSharp.dll` (and for tests, `UnityEngine.dll` and `UnityEngine.CoreModule.dll`) from the game's managed assemblies.

```powershell
dotnet build                         # Build entire solution
dotnet test                          # Run all tests
dotnet test --filter "FullyQualifiedName~ShouldPatchMethaneOxygenResult"  # Run a single test
.\Build-Plugin.ps1                   # Build Release + deploy to local mods folder
```

The plugin targets `netstandard2.0`; the test project targets `net8.0` (MSTest + Shouldly).

## Architecture

The mod patches combustion reactions by **directly mutating** the game's named `Combustion.Result*` reaction objects at plugin startup. The flow is:

1. **`Plugin.cs`** — BepInEx entry point. Reads config, wires up `PatchMethaneOzoneReaction` as a `Func<bool>`, and calls `CombustionResultPatch.PatchReactions()`.
2. **`CombustionResultPatch.cs`** — `PatchReactions()` mutates the named, shared `Combustion.Result*` instances directly (`Combustion.ResultMethaneOxygen` always, `Combustion.ResultMethaneOzone` when opted in), overwriting their readonly fields with correct stoichiometry via reflection (`AccessTools.Field`). Because `CombustionResult` is a reference type, mutating these shared instances propagates the fix everywhere the game uses them. Reading a field first triggers `Combustion`'s static constructor, so the instances are guaranteed to exist.
3. **`CombustionResultExtensions.cs`** — Provides a `Format()` helper used for logging.

> **Why not a Harmony patch?** An earlier approach used a Harmony postfix on `Combustion`'s static constructor, but Harmony triggers the static constructor while gathering metadata to apply the patch — so the `.cctor` runs (once) *before* the postfix is attached, and the postfix never fires. See the [static constructors edge case](https://harmony.pardeike.net/articles/patching-edgecases.html#static-constructors). Direct mutation sidesteps this entirely; HarmonyLib is still referenced only for `AccessTools`.

## Key Conventions

- **Patch named fields directly:** `PatchReactions()` targets the named `Combustion.Result*` fields by name (no value matching), so it knows exactly which reaction it is editing. It does not inspect or match reaction values to identify them.
- **Methane + oxygen is always patched; methane + ozone is opt-in:** The methane + oxygen reaction is always corrected. The methane + ozone reaction is gated behind the `PatchMethaneOzoneReaction` config setting and is **disabled by default**. Default-off is deliberate: it preserves existing users' behavior on update (least surprise) and the ozone reaction is the less-tested branch. New or potentially-surprising patches should default off.
- **Reflection for readonly fields:** `CombustionResult` fields are readonly, so the patch uses `AccessTools.Field(...).SetValue(...)` to overwrite them.
- **Config via `Func<bool>`:** `CombustionResultPatch.PatchMethaneOzoneReaction` is a `Func<bool>` delegate rather than a static bool. `Plugin.Awake` binds the BepInEx `ConfigEntry` (kept private) and wires this delegate to read `.Value` **live** on each call — always current, no caching or `SettingChanged` needed. The delegate also lets tests override the setting without any BepInEx infrastructure.
- **Testable `Patch` helper:** The shared `Patch(instance, fuel, oxidiser, outputs)` helper applies one correction (sets the readonly fields and logs before/after). `PatchReactions()` calls it once for methane + oxygen and, when opted in, once for methane + ozone. Tests exercise `PatchReactions()` against the real `Combustion.Result*` statics; the corrections are idempotent (absolute values), so the tests are order-independent.
- **`using` directives go inside the namespace** (per `.editorconfig`).

## Distribution & Loading

- **The mod is a BepInEx plugin and needs a loader to run from the Workshop.** Subscribing on the Steam Workshop alone does nothing: BepInEx only scans `BepInEx/plugins`, while subscribed mods live in Steam's workshop content folder. **StationeersLaunchPad** is the loader that bridges the two — it discovers the subscribed mod and hands its assembly to BepInEx, which then invokes the `[BepInPlugin]` entry point. Requirements are therefore BepInEx + StationeersLaunchPad. BepInEx must be installed first; StationeersLaunchPad is then extracted into `BepInEx/plugins`. Without a loader, the mod is downloaded but never loaded, and combustion is unchanged with no error.
- **Do not link against StationeersLaunchPad code** — its API is explicitly unstable. The mod stays loader-agnostic (works under StationeersLaunchPad or the older StationeersMods) by exposing only a standard BepInEx entry point.
- **`About/About.xml` `WorkshopHandle` ties updates to the existing item.** It must stay set to the published file ID (`3724908136`) so publishing updates the existing Workshop page (preserving comments/ratings/stats) instead of creating a duplicate. SLP-style requirement notes and the `StationeersLaunchPad` Workshop tag are set at publish time, not in `About.xml`.
