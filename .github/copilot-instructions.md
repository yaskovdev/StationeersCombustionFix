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

The mod patches combustion reactions via a Harmony **postfix** on the `CombustionResult` constructor. The flow is:

1. **`Plugin.cs`** — BepInEx entry point. Reads config, wires up `Func<bool>` delegates for optional reactions, and calls `harmony.PatchAll()`.
2. **`CombustionResultPatch.cs`** — The Harmony `[HarmonyPatch]`. The `Postfix` method inspects each newly constructed `CombustionResult` and, if it matches the game's incorrect reaction values, replaces them with correct stoichiometry using reflection (`AccessTools.Field`).
3. **Extension methods** (`MoleQuantityExtensions.cs`, `CombustionValueExtensions.cs`, `CombustionResultExtensions.cs`) — Provide `.Is()` comparisons for `MoleQuantity` and `CombustionValue[]` (used for exact matching) and `.Format()` for logging.

## Key Conventions

- **Exact match detection:** The postfix patch intentionally uses exact (non-fuzzy, order-sensitive) matching of mole counts and output arrays. This is by design — if the game developers change these values, the mod should silently stop patching rather than apply corrections to an unrecognized reaction. Never introduce fuzzy or approximate matching.
- **Methane + oxygen is always patched; other reactions are opt-in:** The methane + oxygen reaction is always corrected. Methane + nitrous oxide and methane + ozone are each gated behind their own `Func<bool>` config setting and are **disabled by default**. Default-off is deliberate: it preserves existing users' behavior on update (least surprise) and the additional reactions are less-tested. New or potentially-surprising patches should default off.
- **Reaction order follows `Combustion.cs`:** The if/else-if chain in `CombustionResultPatch.Postfix` and the declarations in Plugin/README/About follow the same order as the game's `Combustion` class (oxygen, nitrous oxide, ozone).
- **Config via `Func<bool>`:** Each optional reaction has a `Func<bool>` delegate rather than a static bool. `Plugin.Awake` binds the BepInEx `ConfigEntry` (kept private) and wires the delegate to read `.Value` **live** on each call — always current, no caching or `SettingChanged` needed. The delegate also lets tests override the setting without any BepInEx infrastructure.
- **Reflection for readonly fields:** `CombustionResult` fields are readonly, so the patch uses `AccessTools.Field(...).SetValue(...)` to overwrite them post-construction.
- **`.Is()` not `.Equals()`:** The comparison extensions are named `.Is(...)` rather than `Equals` on purpose — an extension method named `Equals` is unreachable via instance-call syntax (the instance/`object.Equals` always wins) and creates method-group ambiguity.
- **`using` directives go inside the namespace** (per `.editorconfig`).

## Distribution & Loading

- **The mod is a BepInEx plugin and needs a loader to run from the Workshop.** Subscribing on the Steam Workshop alone does nothing: BepInEx only scans `BepInEx/plugins`, while subscribed mods live in Steam's workshop content folder. **StationeersLaunchPad** is the loader that bridges the two — it discovers the subscribed mod and hands its assembly to BepInEx, which then invokes the `[BepInPlugin]` entry point. Requirements are therefore BepInEx + StationeersLaunchPad. BepInEx must be installed first; StationeersLaunchPad is then extracted into `BepInEx/plugins`. Without a loader, the mod is downloaded but never loaded, and combustion is unchanged with no error.
- **Do not link against StationeersLaunchPad code** — its API is explicitly unstable. The mod stays loader-agnostic (works under StationeersLaunchPad or the older StationeersMods) by exposing only a standard BepInEx entry point.
- **`About/About.xml` `WorkshopHandle` ties updates to the existing item.** It must stay set to the published file ID (`3724908136`) so publishing updates the existing Workshop page (preserving comments/ratings/stats) instead of creating a duplicate. SLP-style requirement notes and the `StationeersLaunchPad` Workshop tag are set at publish time, not in `About.xml`.
