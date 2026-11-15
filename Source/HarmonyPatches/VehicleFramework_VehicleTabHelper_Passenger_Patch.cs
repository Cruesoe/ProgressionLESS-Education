using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ProgressionEducation;

[HarmonyPatch]
public static class VehicleFramework_VehicleTabHelper_Passenger_Patch
{
    private static FieldInfo draggedPawnField;
    private static FieldInfo transferToHolderField;

    public static bool Prepare() => ModsConfig.IsActive("SmashPhil.VehicleFramework");

    public static MethodBase TargetMethod()
    {
        var helperType = AccessTools.TypeByName("Vehicles.VehicleTabHelper_Passenger");
        draggedPawnField = AccessTools.Field(helperType, "draggedPawn");
        transferToHolderField = AccessTools.Field(helperType, "transferToHolder");
        return AccessTools.Method(helperType, "HandleDragEvent");
    }

    public static bool Prefix()
    {
        if (!EducationMod.settings.enableProficiencySystem || !EducationMod.settings.enableVehicleProficiency
            || draggedPawnField == null || transferToHolderField == null)
        {
            return true;
        }

        var draggedPawn = draggedPawnField.GetValue(null) as Pawn;
        var transferToHolder = transferToHolderField.GetValue(null);
        if (draggedPawn == null || transferToHolder == null) return true;

        var (canOperate, failReason) = ProficiencyUtility.CheckVehicleOperation(draggedPawn, transferToHolder);
        if (!canOperate && !string.IsNullOrEmpty(failReason))
        {
            Messages.Message(failReason, MessageTypeDefOf.RejectInput);
            draggedPawnField.SetValue(null, null);
            transferToHolderField.SetValue(null, null);
            return false;
        }
        return true;
    }
}
