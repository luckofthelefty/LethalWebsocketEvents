using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class PlayerDamagePatch
{
    [HarmonyPatch(nameof(PlayerControllerB.DamagePlayerClientRpc))]
    [HarmonyPostfix]
    private static void DamagePlayerClientRpcPatch(PlayerControllerB __instance, int damageNumber)
    {
        string playerName = PlayerUtils.GetPlayerName(__instance);

        EventServer.SendEvent("player_damage", new Dictionary<string, object>
        {
            { "player", playerName },
            { "damage", damageNumber },
            { "health", __instance.health },
            { "critical", __instance.health <= 0 }
        });
    }
}
