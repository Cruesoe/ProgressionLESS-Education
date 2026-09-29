using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionEducation;

// Character Development only switches on its Education support for ferny's package ID; switch it on for this fork
[StaticConstructorOnStartup]
public static class CharacterDevelopmentCompat
{
    static CharacterDevelopmentCompat()
    {
        if (!ModsConfig.IsActive("ferny.characterdevelopment")) return;
        var compat = AccessTools.TypeByName("WantsAndQuirks.ProgressionEducationCompat");
        if (compat == null) return;
        RuntimeHelpers.RunClassConstructor(compat.TypeHandle);
        if (AccessTools.Field(compat, "Active")?.GetValue(null) is true) return;

        var values = new (string field, object value)[]
        {
            ("proficiencyDefType", typeof(ProficiencyDef)),
            ("settingsField", AccessTools.Field(typeof(EducationMod), nameof(EducationMod.settings))),
            ("enableProficiencySystemField", AccessTools.Field(typeof(EducationSettings), nameof(EducationSettings.enableProficiencySystem))),
            ("isProficiencyTraitMethod", AccessTools.Method(typeof(ProficiencyUtility), nameof(ProficiencyUtility.IsProficiencyTrait), new[] { typeof(TraitDef) })),
            ("isTrackEnabledMethod", AccessTools.Method(typeof(ProficiencyUtility), nameof(ProficiencyUtility.IsTrackEnabled))),
            ("tiersField", AccessTools.Field(typeof(ProficiencyDef), nameof(ProficiencyDef.tiers))),
            ("tierTraitDefField", AccessTools.Field(typeof(ProficiencyTierDef), nameof(ProficiencyTierDef.traitDef))),
            ("Active", true),
        };
        foreach (var (field, _) in values)
        {
            if (AccessTools.Field(compat, field) == null)
            {
                Log.Warning($"[ProgressionLESS: Education] Character Development support not enabled: {compat.FullName}.{field} not found.");
                return;
            }
        }
        try
        {
            foreach (var (field, value) in values)
            {
                AccessTools.Field(compat, field).SetValue(null, value);
            }
        }
        catch (Exception e)
        {
            Log.Warning($"[ProgressionLESS: Education] Character Development support not enabled: {e.Message}");
        }
    }
}
