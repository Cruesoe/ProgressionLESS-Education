using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace ProgressionEducation;

[HarmonyPatch(typeof(TraitSet), "TraitsSorted", MethodType.Getter)]
public static class TraitSet_TraitsSorted_Patch
{
    // Scratch list reused on every call, as the vanilla getter does
    private static readonly List<Trait> filtered = [];

    public static void Postfix(ref List<Trait> __result)
    {
        if (__result == null)
        {
            return;
        }

        var anyHidden = false;
        for (var i = 0; i < __result.Count; i++)
        {
            if (ShouldHide(__result[i]))
            {
                anyHidden = true;
                break;
            }
        }

        if (!anyHidden)
        {
            return;
        }

        filtered.Clear();
        for (var i = 0; i < __result.Count; i++)
        {
            if (!ShouldHide(__result[i]))
            {
                filtered.Add(__result[i]);
            }
        }

        __result = filtered;
    }

    private static bool ShouldHide(Trait trait)
    {
        var track = ProficiencyUtility.TrackFor(trait.def);
        if (track == null)
        {
            return false;
        }

        if (!EducationMod.settings.enableProficiencySystem || EducationMod.settings.enableKnowledgePanel)
        {
            return true;
        }

        return !ProficiencyUtility.IsTrackEnabled(track);
    }
}
