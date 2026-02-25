using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class ItemPatch
{
    [HarmonyPatch(nameof(PlayerControllerB.GrabObjectClientRpc))]
    [HarmonyPostfix]
    private static void GrabObjectClientRpcPatch(PlayerControllerB __instance, NetworkObjectReference grabbedObject)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;
        if (!PlayerUtils.ShouldTrackPlayer(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(__instance);
        string itemName = "Unknown";
        int scrapValue = 0;

        #pragma warning disable Harmony003
        if (grabbedObject.TryGet(out NetworkObject networkObject))
        #pragma warning restore Harmony003
        {
            if (networkObject.TryGetComponent(out GrabbableObject grabbableObject))
            {
                itemName = grabbableObject.itemProperties?.itemName ?? "Unknown";
                scrapValue = grabbableObject.scrapValue;
            }
        }

        EventServer.SendEvent("item_grabbed", new Dictionary<string, object>
        {
            { "player", playerName },
            { "item", itemName },
            { "scrapValue", scrapValue }
        });
    }

    [HarmonyPatch(nameof(PlayerControllerB.ThrowObjectClientRpc))]
    [HarmonyPostfix]
    private static void ThrowObjectClientRpcPatch(PlayerControllerB __instance, bool droppedInElevator, bool droppedInShipRoom)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;
        if (!PlayerUtils.ShouldTrackPlayer(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(__instance);

        EventServer.SendEvent("item_dropped", new Dictionary<string, object>
        {
            { "player", playerName },
            { "inShip", droppedInElevator || droppedInShipRoom }
        });
    }
}

[HarmonyPatch(typeof(LungProp))]
internal static class ApparatusPatch
{
    // DisconnectFromMachinery is NOT a ClientRpc — no dedup guard needed
    [HarmonyPatch(nameof(LungProp.DisconnectFromMachinery))]
    [HarmonyPostfix]
    private static void DisconnectFromMachineryPatch(LungProp __instance)
    {
        // Try to find who pulled it
        string playerName = "Unknown";
        if (__instance.isHeld && __instance.playerHeldBy != null)
        {
            if (!PlayerUtils.ShouldTrackPlayer(__instance.playerHeldBy)) return;
            playerName = PlayerUtils.GetPlayerName(__instance.playerHeldBy);
        }

        EventServer.SendEvent("apparatus_pulled", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}
