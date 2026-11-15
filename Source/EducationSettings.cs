using UnityEngine;
using Verse;

namespace ProgressionEducation;

public class EducationSettings : ModSettings
{
    public float daycareClassesLearningSpeedModifier = 1f;
    public bool debugMode;
    public bool enableProficiencySystem = true;
    public float globalLearningSpeedModifier = 1f;
    public float proficiencyClassesLearningSpeedModifier = 1f;
    public float skillClassesLearningSpeedModifier = 1f;
    public bool enableKnowledgePanel = true;
    public bool bestowProficiencyToAll = false;
    public bool enableWeaponProficiency = true;
    public bool enableVehicleProficiency = true;
    public bool enableSpeechProficiency = true;
    public bool showClassSuspendedAlert = false;

    public void DoSettingsWindowContents(Rect inRect)
    {
        Listing_Standard listing = new();
        listing.Begin(inRect);
        listing.Label("PE_GlobalLearningSpeed".Translate()
                      + ": "
                      + (globalLearningSpeedModifier * 100).ToString("F0")
                      + "%");
        globalLearningSpeedModifier =
            listing.Slider(globalLearningSpeedModifier, 0.1f, 10f);
        listing.Label("PE_SkillClassesLearningSpeed".Translate()
                      + ": "
                      + (skillClassesLearningSpeedModifier * 100).ToString("F0")
                      + "%");
        skillClassesLearningSpeedModifier =
            listing.Slider(skillClassesLearningSpeedModifier, 0.1f, 10f);
        listing.Label("PE_ProficiencyClassesLearningSpeed".Translate()
                      + ": "
                      + (proficiencyClassesLearningSpeedModifier * 100).ToString("F0")
                      + "%");
        proficiencyClassesLearningSpeedModifier =
            listing.Slider(proficiencyClassesLearningSpeedModifier, 0.1f, 10f);
        listing.Label("PE_DaycareClassesLearningSpeed".Translate()
                      + ": "
                      + (daycareClassesLearningSpeedModifier * 100).ToString("F0")
                      + "%");
        daycareClassesLearningSpeedModifier =
            listing.Slider(daycareClassesLearningSpeedModifier, 0.1f, 10f);
        listing.CheckboxLabeled("PE_EnableProficiencySystem".Translate(),
            ref enableProficiencySystem);

        listing.Gap();
        var proficiencyOff = enableProficiencySystem ? null : "PE_RequiresProficiencySystem".Translate().ToString();
        DependentCheckbox(listing, "PE_EnableKnowledgePanel".Translate(), ref enableKnowledgePanel, proficiencyOff);
        DependentCheckbox(listing, "PE_BestowProficiencyToAll".Translate(), ref bestowProficiencyToAll, proficiencyOff);
        DependentCheckbox(listing, "PE_EnableWeaponProficiency".Translate(), ref enableWeaponProficiency, proficiencyOff);
        DependentCheckbox(listing, "PE_EnableVehicleProficiency".Translate(), ref enableVehicleProficiency,
            proficiencyOff ?? (ProficiencyUtility.AreVehicleModsActive ? null : "PE_RequiresVehicleMod".Translate().ToString()));
        DependentCheckbox(listing, "PE_EnableSpeechProficiency".Translate(), ref enableSpeechProficiency, proficiencyOff);
        listing.Gap();
        listing.CheckboxLabeled("PE_ShowClassSuspendedAlert".Translate(), ref showClassSuspendedAlert);
        listing.Gap();
        listing.CheckboxLabeled("PE_EnableDebugMode".Translate(), ref debugMode);
        listing.End();
    }

    // Draws a checkbox greyed out and inert, with the reason as its tooltip, when disabledReason is set
    private static void DependentCheckbox(Listing_Standard listing, string label, ref bool value, string disabledReason)
    {
        var rect = listing.GetRect(Text.LineHeight);
        Widgets.CheckboxLabeled(rect, label, ref value, disabledReason != null);
        if (disabledReason != null)
        {
            TooltipHandler.TipRegion(rect, disabledReason);
        }

        listing.Gap(listing.verticalSpacing);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref globalLearningSpeedModifier,
            "globalLearningSpeedModifier", 1f);
        Scribe_Values.Look(ref skillClassesLearningSpeedModifier,
            "skillClassesLearningSpeedModifier", 1f);
        Scribe_Values.Look(ref proficiencyClassesLearningSpeedModifier,
            "proficiencyClassesLearningSpeedModifier", 1f);
        Scribe_Values.Look(ref daycareClassesLearningSpeedModifier,
            "daycareClassesLearningSpeedModifier", 1f);
        Scribe_Values.Look(ref enableProficiencySystem, "enableProficiencySystem",
            true);
        Scribe_Values.Look(ref debugMode, "debugMode");
        Scribe_Values.Look(ref enableKnowledgePanel, "enableKnowledgePanel", true);
        Scribe_Values.Look(ref bestowProficiencyToAll, "bestowProficiencyToAll", false);
        Scribe_Values.Look(ref enableWeaponProficiency, "enableWeaponProficiency", true);
        Scribe_Values.Look(ref enableVehicleProficiency, "enableVehicleProficiency", true);
        Scribe_Values.Look(ref enableSpeechProficiency, "enableSpeechProficiency", true);
        Scribe_Values.Look(ref showClassSuspendedAlert, "showClassSuspendedAlert", false);
    }
}
