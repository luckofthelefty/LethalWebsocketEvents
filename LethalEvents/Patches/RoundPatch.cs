using com.github.luckofthelefty.LethalEvents.Server;
using HarmonyLib;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class RoundPatch
{
    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    [HarmonyPostfix]
    private static void StartGamePatch()
    {
        string moon = StartOfRound.Instance?.currentLevel?.PlanetName ?? "Unknown";
        string weather = StartOfRound.Instance?.currentLevel?.currentWeather.ToString() ?? "None";

        EventServer.SendEvent("round_start", new Dictionary<string, object>
        {
            { "moon", moon },
            { "weather", weather }
        });
    }

    [HarmonyPatch(nameof(StartOfRound.EndOfGame))]
    [HarmonyPrefix]
    private static void EndOfGamePatch()
    {
        EventServer.SendEvent("round_end", new Dictionary<string, object>());
    }

    [HarmonyPatch(nameof(StartOfRound.ShipLeave))]
    [HarmonyPostfix]
    private static void ShipLeavePatch()
    {
        EventServer.SendEvent("ship_leaving", new Dictionary<string, object>());
    }

    [HarmonyPatch(nameof(StartOfRound.ReviveDeadPlayers))]
    [HarmonyPostfix]
    private static void ReviveDeadPlayersPatch()
    {
        EventServer.SendEvent("players_revived", new Dictionary<string, object>());
    }

    [HarmonyPatch(nameof(StartOfRound.SetMapScreenInfoToCurrentLevel))]
    [HarmonyPostfix]
    private static void SetMapScreenInfoToCurrentLevelPatch()
    {
        string moon = StartOfRound.Instance?.currentLevel?.PlanetName ?? "Unknown";
        string weather = StartOfRound.Instance?.currentLevel?.currentWeather.ToString() ?? "None";

        EventServer.SendEvent("day_changed", new Dictionary<string, object>
        {
            { "moon", moon },
            { "weather", weather }
        });
    }

    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    [HarmonyPostfix]
    private static void ChangeLevelClientRpcPatch()
    {
        string moon = StartOfRound.Instance?.currentLevel?.PlanetName ?? "Unknown";
        string weather = StartOfRound.Instance?.currentLevel?.currentWeather.ToString() ?? "None";

        EventServer.SendEvent("moon_changed", new Dictionary<string, object>
        {
            { "moon", moon },
            { "weather", weather }
        });
    }

    [HarmonyPatch(nameof(StartOfRound.openingDoorsSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void OpeningDoorsSequencePatch()
    {
        string moon = StartOfRound.Instance?.currentLevel?.PlanetName ?? "Unknown";

        EventServer.SendEvent("ship_landed", new Dictionary<string, object>
        {
            { "moon", moon }
        });
    }
}

[HarmonyPatch(typeof(TimeOfDay))]
internal static class QuotaPatch
{
    [HarmonyPatch(nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
    [HarmonyPostfix]
    private static void SyncNewProfitQuotaClientRpcPatch()
    {
        int newQuota = TimeOfDay.Instance?.profitQuota ?? 0;
        int quotaIndex = (TimeOfDay.Instance?.timesFulfilledQuota ?? 0) + 1;

        EventServer.SendEvent("quota_fulfilled", new Dictionary<string, object>
        {
            { "newQuota", newQuota },
            { "quotaIndex", quotaIndex }
        });
    }
}
