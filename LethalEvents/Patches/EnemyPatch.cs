using com.github.luckofthelefty.LethalEvents.Server;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(EnemyAI))]
internal static class EnemyPatch
{
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
        if (__instance.enemyType == null) return;

        string enemyName = __instance.enemyType.enemyName ?? __instance.GetType().Name;

        EventServer.SendEvent("enemy_killed", new Dictionary<string, object>
        {
            { "enemyType", enemyName }
        });
    }
}
