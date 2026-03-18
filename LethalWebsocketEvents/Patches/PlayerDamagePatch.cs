using com.github.luckofthelefty.LethalWebsocketEvents.Helpers;
using com.github.luckofthelefty.LethalWebsocketEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalWebsocketEvents.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerDamagePatch
{
    [HarmonyPatch(nameof(PlayerControllerB.DamagePlayerClientRpc))]
    [HarmonyPostfix]
    private static void DamagePlayerClientRpcPatch(PlayerControllerB __instance, int damageNumber)
    {
        if (!NetworkUtils.ShouldProcess($"damage_{__instance.GetInstanceID()}_{damageNumber}")) return;
        if (!PlayerUtils.ShouldTrackPlayer(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(__instance);

        // Negative damage is healing (e.g. -100 from revive/heal)
        if (damageNumber < 0)
        {
            EventServer.SendEvent("player_healed", new Dictionary<string, object>
            {
                { "player", playerName },
                { "amount", -damageNumber },
                { "health", __instance.health }
            });
            return;
        }

        string enemyName = EnemyUtils.FindAttackingEnemy(__instance);

        var data = new Dictionary<string, object>
        {
            { "player", playerName },
            { "damage", damageNumber },
            { "health", __instance.health },
            { "critical", __instance.health <= 0 }
        };

        if (enemyName != null)
        {
            data["enemy"] = enemyName;
        }

        EventServer.SendEvent("player_damage", data);
    }
}
