namespace StationeersCombustionFix.Tests;

using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Atmospherics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static Assets.Scripts.Atmospherics.Chemistry;

[TestClass]
public class GameCompatibilityTests
{
    [TestMethod]
    public void ShouldRecognizeEveryCombustionResult()
    {
        var expectedNames = new[]
            {
                nameof(Combustion.ResultMethaneOxygen),
                nameof(Combustion.ResultMethaneNitrous),
                nameof(Combustion.ResultMethaneOzone),
                nameof(Combustion.ResultHydrogenOxygen),
                nameof(Combustion.ResultHydrogenNitrous),
                nameof(Combustion.ResultHydrogenOzone),
                nameof(Combustion.ResultAlcoholOxygen),
                nameof(Combustion.ResultAlcoholNitrous),
                nameof(Combustion.ResultAlcoholOzone),
                nameof(Combustion.ResultHydrazine)
            }
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var actualNames = typeof(Combustion)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(CombustionResult))
            .Select(field => field.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        actualNames.ShouldBe(expectedNames, "The game's combustion result catalog changed; review whether reactions need to be added, removed, or patched.");
    }

    [TestMethod]
    public void ShouldFindExpectedVanillaFuelMixtures()
    {
        var startConditionsPath = Path.Combine(AppContext.BaseDirectory, "startconditions.xml");
        if (!File.Exists(startConditionsPath))
        {
            if (string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail($"The game data file was not copied to {startConditionsPath}.");
            }
            Assert.Inconclusive("A full Stationeers installation is required to run the start-conditions compatibility check.");
            return;
        }

        var methaneXmlName = GetXmlName(GasType.Methane);
        var oxygenXmlName = GetXmlName(GasType.Oxygen);
        var matchingMixtures = XDocument.Load(startConditionsPath)
            .Descendants()
            .Where(element => IsMatchingFuelMixture(element, methaneXmlName, oxygenXmlName))
            .Select(element => $"{element.Name.LocalName}:{element.Attribute("Id")?.Value ?? "<missing>"}")
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var expectedMixtures = new[]
        {
            "DynamicThing:DynamicGasTankAdvanced",
            "Item:ItemGasCanisterEmpty",
            "Item:ItemGasCanisterEmpty",
            "Item:ItemGasCanisterEmpty",
            "Item:ItemGasCanisterEmpty",
            "Item:ItemGasCanisterEmpty"
        };

        matchingMixtures.ShouldBe(expectedMixtures, "The vanilla methane/oxygen start recipes changed; review the data and the starting-fuel patch.");
    }

    private static bool IsMatchingFuelMixture(XElement element, string methaneXmlName, string oxygenXmlName)
    {
        var gases = element.Elements("Gas").ToArray();
        if (gases.Length != 2)
        {
            return false;
        }

        var methane = gases.Where(gas => gas.Attribute("Type")?.Value == methaneXmlName).ToArray();
        var oxygen = gases.Where(gas => gas.Attribute("Type")?.Value == oxygenXmlName).ToArray();
        return methane.Length == 1
               && oxygen.Length == 1
               && TryGetMoles(methane[0], out var methaneMoles)
               && TryGetMoles(oxygen[0], out var oxygenMoles)
               && methaneMoles.Equals(oxygenMoles * 2)
               && HasMatchingTemperature(methane[0], oxygen[0])
               && methane[0].Attribute("Energy") is null
               && oxygen[0].Attribute("Energy") is null;
    }

    private static bool TryGetMoles(XElement gas, out float moles)
    {
        moles = 0;
        return gas.Attribute("Litres") is null
               && float.TryParse(gas.Attribute("Moles")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out moles)
               && moles > 0
               && float.IsFinite(moles);
    }

    private static bool HasMatchingTemperature(XElement first, XElement second)
    {
        var temperatureAttributes = new[] { "Celsius", "Kelvin" };
        var firstUnits = temperatureAttributes.Where(name => first.Attribute(name) is not null).ToArray();
        var secondUnits = temperatureAttributes.Where(name => second.Attribute(name) is not null).ToArray();
        if (firstUnits.Length != 1 || secondUnits.Length != 1 || firstUnits[0] != secondUnits[0])
        {
            return false;
        }

        return float.TryParse(first.Attribute(firstUnits[0])?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var firstTemperature)
               && float.TryParse(second.Attribute(secondUnits[0])?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var secondTemperature)
               && firstTemperature.Equals(secondTemperature);
    }

    private static string GetXmlName(GasType gasType)
    {
        var enumField = typeof(GasType).GetField(gasType.ToString());
        return enumField?.GetCustomAttribute<XmlEnumAttribute>()?.Name ?? gasType.ToString();
    }
}
