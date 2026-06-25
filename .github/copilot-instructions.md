# Copilot Instructions

## Project Overview

A BepInEx plugin (mod) for the game [Stationeers](https://store.steampowered.com/app/544550/Stationeers/) that fixes incorrect methane combustion reactions to be chemically accurate. At startup it directly mutates the game's named `Combustion.Result*` reaction objects, replacing the game's incorrect stoichiometry with correct values.

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

1. **`Plugin.cs`** — BepInEx entry point. Reads config, then `new CombustionResultPatch(patchMethaneOzoneReaction.Value, Logger).PatchReactions()`, passing its BepInEx `Logger` into the patch.
2. **`CombustionResultPatch.cs`** — A public instance class configured at construction with the config flags and the logger. Its constructor builds a `_reactions` dictionary mapping each `Combustion.Result*` field name to its corrected `CombustionResult` template (always-on reactions added unconditionally, opt-in ones only when their flag is set). `PatchReactions()` iterates the dictionary and copies each template's fields onto the live shared instance via reflection (`AccessTools.Field` + `FieldInfo.SetValue`). Because `CombustionResult` is a reference type, mutating these shared instances propagates the fix everywhere the game uses them. Reading a field first triggers `Combustion`'s static constructor, so the instances are guaranteed to exist. **Adding a new always-on reaction is a one-line dictionary entry.**
3. **`CombustionResultExtensions.cs`** — Provides a `Format()` helper used for logging.

> **Why not a Harmony patch?** An earlier approach used a Harmony postfix on `Combustion`'s static constructor, but Harmony triggers the static constructor while gathering metadata to apply the patch — so the `.cctor` runs (once) *before* the postfix is attached, and the postfix never fires. See the [static constructors edge case](https://harmony.pardeike.net/articles/patching-edgecases.html#static-constructors). Direct mutation sidesteps this entirely; HarmonyLib is still referenced only for `AccessTools`.

## Key Conventions

- **Reactions live in a dictionary built at construction:** Each reaction to patch is one entry in the instance's `_reactions` dictionary: `field name → corrected CombustionResult template`. The constructor takes the config flags and adds always-on reactions unconditionally and opt-in reactions only when their flag is set. The corrected template is built by calling the `CombustionResult` constructor (which computes the ratios), and patching copies *all* of its instance fields onto the live instance.
- **Patch named fields directly:** Reactions are keyed by `Combustion.Result*` field name (via `nameof`), so the patch knows exactly which reaction it edits. It does not inspect or value-match reactions to identify them.
- **Methane + oxygen is always patched; methane + ozone is opt-in:** The methane + oxygen reaction is always corrected. The methane + ozone reaction is gated behind the `PatchMethaneOzoneReaction` config setting and is **disabled by default**. Default-off is deliberate: it preserves existing users' behavior on update (least surprise) and the ozone reaction is the less-tested branch. New or potentially-surprising patches should default off.
- **Reflection for readonly fields:** `CombustionResult` fields are readonly, so the patch uses `FieldInfo.SetValue(...)` to overwrite them.
- **Config read once at construction:** Config flags are passed to the `CombustionResultPatch` constructor and the `_reactions` dictionary is fixed from then on. Changing the config mid-game has no effect (the patch already ran in `Awake`); this matches the previous behavior and keeps the class free of any BepInEx config dependency, which keeps tests simple.
- **Logger injected via constructor:** `CombustionResultPatch` takes a non-nullable `ManualLogSource` in its constructor (production passes the plugin's `Logger`; tests pass a `new ManualLogSource(...)`). The class no longer reaches for a static `Plugin.Logger`.
- **Public surface vs test-only seam:** The `CombustionResultPatch` class, its constructor, and the parameterless `PatchReactions()` are `public` (the production API). The `PatchReactions(Type containerType, object? container = null)` overload stays `internal` to signal it exists only for testing (the test project sees it via `InternalsVisibleTo`).
- **Container-agnostic seam for tests:** `PatchReactions(Type containerType, object? container = null)` resolves each field on `containerType` (static when `container` is null, instance otherwise). Production calls the parameterless `PatchReactions()` → `PatchReactions(typeof(Combustion))`. Tests construct the patch with a logger and explicit flags and pass a private `FakeReactions` **instance** class whose field names match the dictionary keys — so patching mutates a throwaway per-test object, never the real game statics. Each test also asserts (read-only) that the fake's instance `ShouldBeEquivalentTo` the corresponding real static, pinning the assumed source recipe.
- **`using` directives go inside the namespace** (per `.editorconfig`).

## Distribution & Loading

- **The mod is a BepInEx plugin and needs a loader to run from the Workshop.** Subscribing on the Steam Workshop alone does nothing: BepInEx only scans `BepInEx/plugins`, while subscribed mods live in Steam's workshop content folder. **StationeersLaunchPad** is the loader that bridges the two — it discovers the subscribed mod and hands its assembly to BepInEx, which then invokes the `[BepInPlugin]` entry point. Requirements are therefore BepInEx + StationeersLaunchPad. BepInEx must be installed first; StationeersLaunchPad is then extracted into `BepInEx/plugins`. Without a loader, the mod is downloaded but never loaded, and combustion is unchanged with no error.
- **Do not link against StationeersLaunchPad code** — its API is explicitly unstable. The mod stays loader-agnostic (works under StationeersLaunchPad or the older StationeersMods) by exposing only a standard BepInEx entry point.
- **`About/About.xml` `WorkshopHandle` ties updates to the existing item.** It must stay set to the published file ID (`3724908136`) so publishing updates the existing Workshop page (preserving comments/ratings/stats) instead of creating a duplicate. SLP-style requirement notes and the `StationeersLaunchPad` Workshop tag are set at publish time, not in `About.xml`.
