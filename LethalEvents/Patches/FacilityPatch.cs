using com.github.luckofthelefty.LethalEvents.Server;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(Landmine))]
internal static class LandminePatch
{
    [HarmonyPatch(nameof(Landmine.ExplodeMineClientRpc))]
    [HarmonyPostfix]
    private static void ExplodeMineClientRpcPatch()
    {
        EventServer.SendEvent("landmine_exploded", new Dictionary<string, object>());
    }
}

[HarmonyPatch(typeof(Turret))]
internal static class TurretPatch
{
    [HarmonyPatch(nameof(Turret.SetToModeClientRpc))]
    [HarmonyPostfix]
    #pragma warning disable Harmony003
    private static void SetToModeClientRpcPatch(int modeIndex)
    #pragma warning restore Harmony003
    {
        // TurretMode: 0 = Detection, 1 = Charging, 2 = Firing, 3 = Berserk
        string[] modeNames = { "Detection", "Charging", "Firing", "Berserk" };
        string mode = modeIndex >= 0 && modeIndex < modeNames.Length ? modeNames[modeIndex] : "Unknown";

        EventServer.SendEvent("turret_mode_changed", new Dictionary<string, object>
        {
            { "mode", mode },
            { "modeIndex", modeIndex }
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
        EventServer.SendEvent("teleporter_used", new Dictionary<string, object>
        {
            { "isInverse", __instance.isInverseTeleporter }
        });
    }
}
