using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(EnemyAI))]
internal static class EnemyPatch
{
    // Start is NOT a ClientRpc — no dedup guard needed
    [HarmonyPatch(nameof(EnemyAI.Start))]
    [HarmonyPostfix]
    private static void StartPatch(EnemyAI __instance)
    {
        if (__instance.enemyType == null) return;

        string enemyName = __instance.enemyType.enemyName ?? __instance.GetType().Name;
        bool isOutside = __instance.isOutside;

        EventServer.SendEvent("enemy_spawned", new Dictionary<string, object>
        {
            { "enemyType", enemyName },
            { "location", isOutside ? "outdoor" : "indoor" }
        });
    }

    [HarmonyPatch(nameof(EnemyAI.KillEnemyClientRpc))]
    [HarmonyPostfix]
    private static void KillEnemyClientRpcPatch(EnemyAI __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;
        if (__instance.enemyType == null) return;

        string enemyName = __instance.enemyType.enemyName ?? __instance.GetType().Name;

        EventServer.SendEvent("enemy_killed", new Dictionary<string, object>
        {
            { "enemyType", enemyName }
        });
    }

    // Fires for ALL enemies (vanilla + modded) when they change behavior state.
    // Common states: 0 = idle/roaming, 1 = alert/chasing, 2 = attacking (varies by enemy)
    [HarmonyPatch(nameof(EnemyAI.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void SwitchToBehaviourClientRpcPatch(EnemyAI __instance, int stateIndex)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;
        if (__instance.enemyType == null) return;

        string enemyName = __instance.enemyType.enemyName ?? __instance.GetType().Name;

        EventServer.SendEvent("enemy_state_changed", new Dictionary<string, object>
        {
            { "enemyType", enemyName },
            { "state", stateIndex }
        });
    }

    // Fires for ALL enemies when hit by a player
    [HarmonyPatch(nameof(EnemyAI.HitEnemyClientRpc))]
    [HarmonyPostfix]
    private static void HitEnemyClientRpcPatch(EnemyAI __instance, int force, int playerWhoHit, bool playHitSFX, int hitID)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;
        if (__instance.enemyType == null) return;

        string enemyName = __instance.enemyType.enemyName ?? __instance.GetType().Name;
        string playerName = PlayerUtils.GetPlayerName(playerWhoHit);

        EventServer.SendEvent("enemy_hit", new Dictionary<string, object>
        {
            { "enemyType", enemyName },
            { "player", playerName },
            { "force", force }
        });
    }
}
