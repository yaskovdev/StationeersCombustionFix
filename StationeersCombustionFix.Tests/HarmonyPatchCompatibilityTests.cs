namespace StationeersCombustionFix.Tests;

using System.Reflection;
using Assets.Scripts;
using Assets.Scripts.Atmospherics;
using HarmonyLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

[TestClass]
public class HarmonyPatchCompatibilityTests
{
    [TestMethod]
    public void ShouldResolveCombustionResultPatchTarget()
    {
        var target = ResolvePatchTarget(typeof(CombustionResultPatch));
        var expectedTarget = AccessTools.DeclaredConstructor(
            typeof(CombustionResult),
            new[] { typeof(double), typeof(double), typeof(CombustionValue[]) });
        expectedTarget.ShouldNotBeNull("The CombustionResult constructor changed in a game update.");

        target.ShouldBe(
            expectedTarget,
            "Harmony can no longer resolve the annotated CombustionResult constructor.");
    }

    [TestMethod]
    public void ShouldResolveStartingFuelMixturePatchTarget()
    {
        var target = ResolvePatchTarget(typeof(StartingFuelMixturePatch));
        var expectedTarget = AccessTools.DeclaredMethod(
            typeof(SpawnData),
            nameof(SpawnData.Initialize),
            new[] { typeof(ModAbout) });
        expectedTarget.ShouldNotBeNull("The SpawnData.Initialize signature changed in a game update.");

        target.ShouldBe(
            expectedTarget,
            "Harmony can no longer resolve the annotated SpawnData.Initialize method.");
    }

    private static MethodBase? ResolvePatchTarget(Type patchType)
    {
        var annotation = HarmonyMethodExtensions.GetMergedFromType(patchType);
        if (annotation.declaringType is null)
        {
            return null;
        }

        var methodType = annotation.methodType ?? MethodType.Normal;
        return methodType switch
        {
            MethodType.Constructor => AccessTools.DeclaredConstructor(annotation.declaringType, annotation.argumentTypes),
            MethodType.Normal when annotation.methodName is not null =>
                AccessTools.DeclaredMethod(annotation.declaringType, annotation.methodName, annotation.argumentTypes),
            _ => null
        };
    }
}
