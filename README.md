# Stationeers Combustion Fix

Replaces the incorrect reaction `2 CH4 + O2 → 6 CO2 + 3 POL` with the chemically correct methane combustion reaction `CH4 + 2 O2 → CO2 + 2 H2O`.

The plugin replaces the `Combustion.ResultMethaneOxygen` value with a chemically accurate `CombustionResult`.

If welding releases methane into the atmosphere, make sure the oxygen proportion in the fuel mixture is at least twice the methane proportion. A mixture of 33% methane and 67% oxygen should work.
