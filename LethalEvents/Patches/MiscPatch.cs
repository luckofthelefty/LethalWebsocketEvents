using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class ConnectionPatch
{
    // Use SendNewPlayerValuesClientRpc instead of OnPlayerConnectedClientRpc
    // because player names aren't synced yet when OnPlayerConnected fires.
    // This RPC fires after the player's username/cosmetics are broadcast to all clients.
    [HarmonyPatch(nameof(PlayerControllerB.SendNewPlayerValuesClientRpc))]
    [HarmonyPostfix]
    private static void SendNewPlayerValuesClientRpcPatch(PlayerControllerB __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(__instance);
        int playerCount = PlayerUtils.GetConnectedPlayerCount();

        EventServer.SendEvent("player_joined", new Dictionary<string, object>
        {
            { "player", playerName },
            { "playerCount", playerCount }
        });
    }
}

[HarmonyPatch(typeof(StartOfRound))]
internal static class DisconnectPatch
{
    [HarmonyPatch(nameof(StartOfRound.OnPlayerDC))]
    [HarmonyPostfix]
    private static void OnPlayerDCPatch(int playerObjectNumber)
    {
        string playerName = "Unknown";
        if (StartOfRound.Instance?.allPlayerScripts != null &&
            playerObjectNumber >= 0 &&
            playerObjectNumber < StartOfRound.Instance.allPlayerScripts.Length)
        {
            playerName = PlayerUtils.GetPlayerName(StartOfRound.Instance.allPlayerScripts[playerObjectNumber]);
        }

        int playerCount = PlayerUtils.GetConnectedPlayerCount();

        EventServer.SendEvent("player_left", new Dictionary<string, object>
        {
            { "player", playerName },
            { "playerCount", playerCount }
        });
    }
}

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class EmotePatch
{
    [HarmonyPatch(nameof(PlayerControllerB.PerformEmote))]
    [HarmonyPostfix]
    private static void PerformEmotePatch(PlayerControllerB __instance, int emoteID)
    {
        if (!PlayerUtils.ShouldTrackPlayer(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(__instance);

        EventServer.SendEvent("player_emote", new Dictionary<string, object>
        {
            { "player", playerName },
            { "emoteId", emoteID }
        });
    }
}

[HarmonyPatch(typeof(TimeOfDay))]
internal static class VoteToLeavePatch
{
    [HarmonyPatch(nameof(TimeOfDay.VoteShipToLeaveEarly))]
    [HarmonyPostfix]
    private static void VoteShipToLeaveEarlyPatch()
    {
        EventServer.SendEvent("vote_to_leave", new Dictionary<string, object>());
    }
}
