using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class ConnectionPatch
{
    [HarmonyPatch(nameof(StartOfRound.OnPlayerConnectedClientRpc))]
    [HarmonyPostfix]
    private static void OnPlayerConnectedClientRpcPatch(ulong clientId)
    {
        var playerScript = PlayerUtils.GetPlayerScriptByClientId(clientId);
        string playerName = PlayerUtils.GetPlayerName(playerScript);

        int playerCount = GameNetworkManager.Instance?.connectedPlayers
            ?? (StartOfRound.Instance?.connectedPlayersAmount + 1)
            ?? 1;

        EventServer.SendEvent("player_joined", new Dictionary<string, object>
        {
            { "player", playerName },
            { "playerCount", playerCount }
        });
    }

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

        int playerCount = GameNetworkManager.Instance?.connectedPlayers
            ?? (StartOfRound.Instance?.connectedPlayersAmount + 1)
            ?? 0;

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
