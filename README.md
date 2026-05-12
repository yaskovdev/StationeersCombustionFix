# Stationeers Combustion Fix

Replaces the incorrect methane combustion reaction `2 CH4 + O2 → 6 CO2 + 3 POL` with the chemically correct reaction `CH4 + 2 O2 → CO2 + 2 H2O`.

![Stationeers Combustion Fix](Image.png)

The plugin replaces the `Combustion.ResultMethaneOxygen` value with a chemically accurate `CombustionResult`.

If welding releases methane into the atmosphere, make sure the oxygen proportion in the fuel mixture is at least twice the methane proportion. A mixture of 33% methane and 67% oxygen should work.

## Setting Up the Project

The project requires a reference to `Assembly-CSharp.dll` from your local Stationeers installation. This file is not included in the repository.

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

1. Create the `$env:USERPROFILE\Documents\My Games\Stationeers\mods\StationeersCombustionFix` folder.
2. Create an empty `GameData` folder in the new folder.
3. Copy the `About` folder to the new folder.
4. Copy the `StationeersCombustionFix.dll` to the new folder.
5. Run Stationeers, then go to Workshop. You'll see the mod and the Publish button.
