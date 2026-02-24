using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerDeathPatch
{
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void KillPlayerClientRpcPatch(PlayerControllerB __instance, int playerId, int causeOfDeath)
    {
        var playerScript = StartOfRound.Instance?.allPlayerScripts != null && playerId >= 0 && playerId < StartOfRound.Instance.allPlayerScripts.Length
            ? StartOfRound.Instance.allPlayerScripts[playerId]
            : __instance;

        string playerName = PlayerUtils.GetPlayerName(playerScript);
        string cause = ((CauseOfDeath)causeOfDeath).ToString();

        // Try to determine what enemy killed the player
        string enemyName = null;
        if (playerScript.causeOfDeath == CauseOfDeath.Mauling ||
            playerScript.causeOfDeath == CauseOfDeath.Strangulation ||
            playerScript.causeOfDeath == CauseOfDeath.Crushing ||
            playerScript.causeOfDeath == CauseOfDeath.Bludgeoning)
        {
            // Check if there's a reference to the killer enemy via the body
            if (playerScript.deadBody != null && playerScript.deadBody.causeOfDeath != CauseOfDeath.Unknown)
            {
                // The enemy type is harder to get directly, but we can check nearby enemies
            }
        }

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
