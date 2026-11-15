using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace ProgressionEducation;

[HarmonyPatch]
[HotSwappable]
public static class VehicleFramework_GetFloatMenuOptions_Patch
{
    private static FieldInfo handlersField;

    public static bool Prepare() => ModsConfig.IsActive("SmashPhil.VehicleFramework");

    public static MethodBase TargetMethod()
    {
        var vehiclePawnType = AccessTools.TypeByName("Vehicles.VehiclePawn");
        handlersField = AccessTools.Field(vehiclePawnType, "handlers");
        return AccessTools.Method(vehiclePawnType, "GetFloatMenuOptions", new[] { typeof(Pawn) });
    }

    public static void Postfix(object __instance, Pawn selPawn, ref IEnumerable<FloatMenuOption> __result)
    {
        if (!EducationMod.settings.enableProficiencySystem || !EducationMod.settings.enableVehicleProficiency)
        {
            return;
        }

        if (handlersField?.GetValue(__instance) is not IEnumerable<object> handlers)
        {
            return;
        }

        var list = new List<FloatMenuOption>(__result);
        var idx = 0;
        foreach (var handler in handlers)
        {
            if (idx >= list.Count) break;
            var (canOperate, failReason) = ProficiencyUtility.CheckVehicleOperation(selPawn, handler);
            if (!canOperate && !string.IsNullOrEmpty(failReason))
            {
                var opt = list[idx];
                opt.Label = opt.Label.Replace("VF_BoardFailureNonCombatant".Translate(selPawn.LabelShort), failReason);
            }
            idx++;
        }
        __result = list;
    }
}
