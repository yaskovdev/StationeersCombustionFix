namespace StationeersCombustionFix;

using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Atmospherics;
using BepInEx.Logging;
using HarmonyLib;
using static Assets.Scripts.Atmospherics.Chemistry;

public class CombustionResultPatch
{
    private static readonly FieldInfo[] CombustionResultFields =
        typeof(CombustionResult).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    /// <summary>
    /// Maps the name of a <c>Combustion.Result*</c> field to the corrected reaction to overwrite it with. Built from
    /// the config in the constructor: always-on reactions are added unconditionally, opt-in ones only when enabled.
    /// </summary>
    private readonly Dictionary<string, CombustionResult> _reactions;

    private readonly ManualLogSource _logger;

    public CombustionResultPatch(bool patchMethaneOzoneReaction, ManualLogSource logger)
    {
        _reactions = new Dictionary<string, CombustionResult>
        {
            [nameof(Combustion.ResultMethaneOxygen)] =
                new(1.0, 2.0, new CombustionValue[] { new(GasType.CarbonDioxide, 1.0), new(GasType.Steam, 2.0) }),
        };
        if (patchMethaneOzoneReaction)
        {
            _reactions[nameof(Combustion.ResultMethaneOzone)] =
                new CombustionResult(3.0, 4.0, new CombustionValue[] { new(GasType.CarbonDioxide, 3.0), new(GasType.Steam, 6.0) });
        }
        _logger = logger;
    }

    public void PatchReactions() => PatchReactions(typeof(Combustion), null);

    /// <summary>
    /// Overwrites the configured <see cref="CombustionResult"/> fields declared on <paramref name="containerType"/>
    /// with their corrected reactions. Pass a type with a null <paramref name="container"/> for static fields (as
    /// <c>Combustion</c> declares them), or a type plus an instance for instance fields (used by the tests).
    /// </summary>
    internal void PatchReactions(Type containerType, object? container)
    {
        foreach (var reaction in _reactions)
        {
            var field = AccessTools.Field(containerType, reaction.Key);
            if (field == null)
            {
                _logger.LogWarning($"Reaction field {reaction.Key} not found on {containerType.Name}, skipping");
                continue;
            }

            var target = (CombustionResult)field.GetValue(container);
            _logger.LogInfo($"Patching reaction {reaction.Key}: {target.Format()}");
            Overwrite(reaction.Value, target);
            _logger.LogInfo($"Patched reaction {reaction.Key}: {target.Format()}");
        }
    }

    /// <summary>
    /// <see cref="CombustionResult"/> is a reference type, so copying the corrected fields onto the shared, named instance propagates
    /// the fix everywhere the game uses it, without having to patch every method that consumes the reaction.
    /// </summary>
    private static void Overwrite(CombustionResult source, CombustionResult target)
    {
        foreach (var field in CombustionResultFields)
        {
            field.SetValue(target, field.GetValue(source));
        }
    }
}
