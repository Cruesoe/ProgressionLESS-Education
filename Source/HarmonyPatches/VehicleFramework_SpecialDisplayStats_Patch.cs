using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionEducation;

[HarmonyPatch]
public static class VehicleFramework_SpecialDisplayStats_Patch
{
    private static Type listType;
    private static MethodInfo addMethod;
    private static ConstructorInfo entryConstructor;

    public static bool Prepare() => ModsConfig.IsActive("SmashPhil.VehicleFramework");

    public static MethodBase TargetMethod()
    {
        var vehicleDefType = AccessTools.TypeByName("Vehicles.VehicleDef");
        var vehiclePawnType = AccessTools.TypeByName("Vehicles.VehiclePawn");
        var statDrawEntryType = AccessTools.TypeByName("Vehicles.VehicleStatDrawEntry");
        listType = typeof(List<>).MakeGenericType(statDrawEntryType);
        addMethod = listType.GetMethod("Add");
        entryConstructor = AccessTools.Constructor(statDrawEntryType, new[] {
            typeof(StatCategoryDef),
            typeof(string),
            typeof(string),
            typeof(string),
            typeof(int),
            typeof(string),
            typeof(IEnumerable<Dialog_InfoCard.Hyperlink>),
            typeof(bool)
        });
        return AccessTools.Method(vehicleDefType, "SpecialDisplayStats", new[] { vehiclePawnType });
    }

    public static object Postfix(object __result, object __instance)
    {
        if (!EducationMod.settings.enableProficiencySystem || !EducationMod.settings.enableVehicleProficiency)
        {
            return __result;
        }

        var reqTier = ProficiencyUtility.GetRequiredVehicleTierFromDef(__instance);
        if (reqTier == null || entryConstructor == null)
        {
            return __result;
        }

        var list = Activator.CreateInstance(listType);
        if (__result is IEnumerable originalEnumerable)
        {
            foreach (var item in originalEnumerable)
            {
                addMethod.Invoke(list, new[] { item });
            }
        }

        var newEntry = entryConstructor.Invoke(new object[] {
            StatCategoryDefOf.Basics,
            "PE_RequiredProficiencyStat".Translate().ToString(),
            reqTier.label.CapitalizeFirst(),
            "PE_RequiredProficiencyStatVehicleDesc".Translate().ToString(),
            5000,
            null,
            null,
            false
        });
        addMethod.Invoke(list, new[] { newEntry });
        return list;
    }
}
