using com.github.luckofthelefty.LethalEvents.Helpers;
using com.github.luckofthelefty.LethalEvents.Server;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

// Bracken (Flowerman) — grabs and drags player to kill
[HarmonyPatch(typeof(FlowermanAI))]
internal static class BrackenPatch
{
    [HarmonyPatch(nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    [HarmonyPostfix]
    private static void KillPlayerAnimationClientRpcPatch(FlowermanAI __instance, int playerObjectId)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerObjectId);

        EventServer.SendEvent("bracken_grab", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Jester — kills player
[HarmonyPatch(typeof(JesterAI))]
internal static class JesterPatch
{
    [HarmonyPatch(nameof(JesterAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void KillPlayerClientRpcPatch(JesterAI __instance, int playerId)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerId);

        EventServer.SendEvent("jester_kill", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Ghost Girl (Dress Girl) — haunts a specific player
[HarmonyPatch(typeof(DressGirlAI))]
internal static class GhostGirlPatch
{
    [HarmonyPatch(nameof(DressGirlAI.ChooseNewHauntingPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ChooseNewHauntingPlayerClientRpcPatch(DressGirlAI __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = "Unknown";
        if (__instance.hauntingPlayer != null)
            playerName = PlayerUtils.GetPlayerName(__instance.hauntingPlayer);

        EventServer.SendEvent("ghost_girl_haunt", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Coilhead (Spring Man) — moves when not watched
[HarmonyPatch(typeof(SpringManAI))]
internal static class CoilheadPatch
{
    [HarmonyPatch(nameof(SpringManAI.SetAnimationGoClientRpc))]
    [HarmonyPostfix]
    private static void SetAnimationGoClientRpcPatch(SpringManAI __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        EventServer.SendEvent("coilhead_moving", new Dictionary<string, object>());
    }

    [HarmonyPatch(nameof(SpringManAI.SetAnimationStopClientRpc))]
    [HarmonyPostfix]
    private static void SetAnimationStopClientRpcPatch(SpringManAI __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        EventServer.SendEvent("coilhead_stopped", new Dictionary<string, object>());
    }
}

// Masked (Mimic) — creates a mimic of a player
[HarmonyPatch(typeof(MaskedPlayerEnemy))]
internal static class MaskedPatch
{
    [HarmonyPatch(nameof(MaskedPlayerEnemy.CreateMimicClientRpc))]
    [HarmonyPostfix]
    private static void CreateMimicClientRpcPatch(MaskedPlayerEnemy __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string mimicking = __instance.mimickingPlayer != null
            ? PlayerUtils.GetPlayerName(__instance.mimickingPlayer)
            : "Unknown";

        EventServer.SendEvent("masked_mimic", new Dictionary<string, object>
        {
            { "mimicking", mimicking }
        });
    }
}

// Nutcracker — fires shotgun
[HarmonyPatch(typeof(NutcrackerEnemyAI))]
internal static class NutcrackerPatch
{
    [HarmonyPatch(nameof(NutcrackerEnemyAI.FireGunClientRpc))]
    [HarmonyPostfix]
    private static void FireGunClientRpcPatch(NutcrackerEnemyAI __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        EventServer.SendEvent("nutcracker_shot", new Dictionary<string, object>());
    }
}

// Forest Giant — grabs and eats player
[HarmonyPatch(typeof(ForestGiantAI))]
internal static class ForestGiantPatch
{
    [HarmonyPatch(nameof(ForestGiantAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GrabPlayerClientRpcPatch(ForestGiantAI __instance, int playerId)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerId);

        EventServer.SendEvent("giant_grab", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Snare Flea (Centipede) — clings to player's head
[HarmonyPatch(typeof(CentipedeAI))]
internal static class SnareFleaPatch
{
    [HarmonyPatch(nameof(CentipedeAI.ClingToPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ClingToPlayerClientRpcPatch(CentipedeAI __instance)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = "Unknown";
        if (__instance.clingingToPlayer != null)
            playerName = PlayerUtils.GetPlayerName(__instance.clingingToPlayer);

        EventServer.SendEvent("snare_flea_cling", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Spider — player trips a web
[HarmonyPatch(typeof(SandSpiderAI))]
internal static class SpiderPatch
{
    [HarmonyPatch(nameof(SandSpiderAI.PlayerTripWebClientRpc))]
    [HarmonyPostfix]
    private static void PlayerTripWebClientRpcPatch(SandSpiderAI __instance, int playerNum)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerNum);

        EventServer.SendEvent("spider_web_trip", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Hygrodere (Blob) — kills player
[HarmonyPatch(typeof(BlobAI))]
internal static class BlobPatch
{
    [HarmonyPatch(nameof(BlobAI.SlimeKillPlayerEffectClientRpc))]
    [HarmonyPostfix]
    private static void SlimeKillPlayerEffectClientRpcPatch(BlobAI __instance, int playerKilled)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerKilled);

        EventServer.SendEvent("blob_kill", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Thumper (Crawler) — hits player
[HarmonyPatch(typeof(CrawlerAI))]
internal static class ThumperPatch
{
    [HarmonyPatch(nameof(CrawlerAI.HitPlayerClientRpc))]
    [HarmonyPostfix]
    private static void HitPlayerClientRpcPatch(CrawlerAI __instance, int playerId)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerId);

        EventServer.SendEvent("thumper_hit", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Eyeless Dog (MouthDog) — kills player
[HarmonyPatch(typeof(MouthDogAI))]
internal static class EyelessDogPatch
{
    [HarmonyPatch(nameof(MouthDogAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void KillPlayerClientRpcPatch(MouthDogAI __instance, int playerId)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerId);

        EventServer.SendEvent("eyeless_dog_kill", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}

// Baboon Hawk — stabs player
[HarmonyPatch(typeof(BaboonBirdAI))]
internal static class BaboonHawkPatch
{
    [HarmonyPatch(nameof(BaboonBirdAI.StabPlayerDeathAnimClientRpc))]
    [HarmonyPostfix]
    private static void StabPlayerDeathAnimClientRpcPatch(BaboonBirdAI __instance, int playerObject)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;

        string playerName = PlayerUtils.GetPlayerName(playerObject);

        EventServer.SendEvent("baboon_hawk_stab", new Dictionary<string, object>
        {
            { "player", playerName }
        });
    }
}
