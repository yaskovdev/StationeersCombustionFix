namespace StationeersCombustionFix.Tests;

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Trading;

[TestClass]
public class GasActionExtensionsTests
{
    [TestMethod]
    public void ShouldCloneAllFieldsExceptAmount()
    {
        var source = new GasAction
        {
            Moles = 100f,
            Litres = 10f
        };
        var copiedFields = typeof(GasAction)
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(field => field.Name != nameof(GasAction.Moles) && field.Name != nameof(GasAction.Litres))
            .ToArray();

        for (var index = 0; index < copiedFields.Length; index++)
        {
            copiedFields[index].SetValue(source, CreateNonDefaultValue(copiedFields[index].FieldType, index));
        }

        var clone = source.CloneWithAmount(new Amount(50f, AmountUnit.Moles));

        clone.ShouldNotBeSameAs(source);
        clone.Moles.ShouldBe(50f);
        clone.Litres.ShouldBe(float.NaN);

        foreach (var field in copiedFields)
        {
            field.GetValue(clone).ShouldBe(field.GetValue(source), $"Field {field.Name} was not copied.");
        }
    }

    private static object CreateNonDefaultValue(Type type, int index)
    {
        if (type.IsEnum)
        {
            return Enum.GetValues(type)
                .Cast<object>()
                .First(value => Convert.ToUInt64(value) != 0);
        }
        return type == typeof(float) ? index + 1f : throw new AssertFailedException($"Add a non-default test value for the new GasAction field type {type.FullName}");
    }
}
