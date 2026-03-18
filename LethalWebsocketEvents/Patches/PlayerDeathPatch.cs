using com.github.luckofthelefty.LethalWebsocketEvents.Helpers;
using com.github.luckofthelefty.LethalWebsocketEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace com.github.luckofthelefty.LethalWebsocketEvents.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerDeathPatch
{
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void KillPlayerClientRpcPatch(PlayerControllerB __instance, int playerId, int causeOfDeath)
    {
        if (!NetworkUtils.ShouldProcess($"death_{playerId}")) return;

        var playerScript = StartOfRound.Instance?.allPlayerScripts != null && playerId >= 0 && playerId < StartOfRound.Instance.allPlayerScripts.Length
            ? StartOfRound.Instance.allPlayerScripts[playerId]
            : __instance;

        if (!PlayerUtils.ShouldTrackPlayer(playerScript)) return;

        string playerName = PlayerUtils.GetPlayerName(playerScript);
        string cause = ((CauseOfDeath)causeOfDeath).ToString();
        string enemyName = EnemyUtils.FindAttackingEnemy(playerScript);

        var data = new Dictionary<string, object>
        {
            { "player", playerName },
            { "causeOfDeath", cause },
            { "playerId", playerId }
        };

        if (enemyName != null)
        {
            data["enemy"] = enemyName;
        }

        EventServer.SendEvent("player_death", data);
    }
}
