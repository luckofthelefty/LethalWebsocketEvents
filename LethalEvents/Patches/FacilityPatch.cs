using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(Landmine))]
internal static class LandminePatch
{
    [HarmonyPatch(nameof(Landmine.ExplodeMineClientRpc))]
    [HarmonyPostfix]
    private static void ExplodeMineClientRpcPatch(Landmine __instance)
    {
        if (!NetworkUtils.ShouldProcess($"landmine_{__instance.GetInstanceID()}")) return;

        EventServer.SendEvent("landmine_exploded", new Dictionary<string, object>());
    }
}

[HarmonyPatch(typeof(Turret))]
internal static class TurretPatch
{
    [HarmonyPatch(nameof(Turret.SetToModeClientRpc))]
    [HarmonyPostfix]
    private static void SetToModeClientRpcPatch(Turret __instance, int mode)
    {
        if (!NetworkUtils.ShouldProcess($"turret_{__instance.GetInstanceID()}_{mode}")) return;

        // TurretMode: 0 = Detection, 1 = Charging, 2 = Firing, 3 = Berserk
        string[] modeNames = { "Detection", "Charging", "Firing", "Berserk" };
        string modeName = mode >= 0 && mode < modeNames.Length ? modeNames[mode] : "Unknown";

        EventServer.SendEvent("turret_mode_changed", new Dictionary<string, object>
        {
            { "mode", modeName },
            { "modeIndex", mode }
        });
    }
}

[HarmonyPatch(typeof(ShipTeleporter))]
internal static class TeleporterPatch
{
    [HarmonyPatch(nameof(ShipTeleporter.PressTeleportButtonClientRpc))]
    [HarmonyPostfix]
    private static void PressTeleportButtonClientRpcPatch(ShipTeleporter __instance)
    {
        if (!NetworkUtils.ShouldProcess($"teleporter_{__instance.GetInstanceID()}")) return;

        EventServer.SendEvent("teleporter_used", new Dictionary<string, object>
        {
            { "isInverse", __instance.isInverseTeleporter }
        });
    }
}
