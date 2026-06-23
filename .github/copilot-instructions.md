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

1. **`Plugin.cs`** — BepInEx entry point. Reads config, wires up `PatchMethaneOzoneReaction` as a `Func<bool>`, and calls `harmony.PatchAll()`.
2. **`CombustionResultPatch.cs`** — The Harmony `[HarmonyPatch]`. The `Postfix` method inspects each newly constructed `CombustionResult` and, if it matches the game's incorrect reaction values, replaces them with correct stoichiometry using reflection (`AccessTools.Field`).
3. **Extension methods** (`MoleQuantityExtensions.cs`, `CombustionValueExtensions.cs`) — Provide `.Is()` comparisons for `MoleQuantity` and `CombustionValue[]`, used for exact matching.

## Key Conventions

- **Exact match detection:** The postfix patch intentionally uses exact (non-fuzzy, order-sensitive) matching of mole counts and output arrays. This is by design — if the game developers change these values, the mod should silently stop patching rather than apply corrections to an unrecognized reaction. Never introduce fuzzy or approximate matching.
- **Reflection for readonly fields:** `CombustionResult` fields are readonly, so the patch uses `AccessTools.Field(...).SetValue(...)` to overwrite them post-construction.
- **Config via `Func<bool>`:** `CombustionResultPatch.PatchMethaneOzoneReaction` is a `Func<bool>` delegate rather than a static bool, allowing tests to override it without BepInEx infrastructure.
- **`using` directives go inside the namespace** (per `.editorconfig`).
