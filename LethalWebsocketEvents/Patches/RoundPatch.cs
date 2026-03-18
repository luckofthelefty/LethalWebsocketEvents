using com.github.luckofthelefty.LethalWebsocketEvents.Helpers;
using com.github.luckofthelefty.LethalWebsocketEvents.Server;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalWebsocketEvents.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal static class RoundPatch
{
    private static DateTime _lastDayChangedTime = DateTime.MinValue;
    private static string _lastDayChangedKey = "";
    private static readonly TimeSpan EventDebounce = TimeSpan.FromSeconds(2);

    private static DateTime _lastShipLandedTime = DateTime.MinValue;

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
        string key = $"{moon}|{weather}";

        // Debounce — this method fires multiple times during player joins and level loads
        var now = DateTime.UtcNow;
        if (now - _lastDayChangedTime < EventDebounce && _lastDayChangedKey == key)
            return;

        _lastDayChangedTime = now;
        _lastDayChangedKey = key;

        EventServer.SendEvent("day_changed", new Dictionary<string, object>
        {
            { "moon", moon },
            { "weather", weather }
        });
    }

    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    [HarmonyPostfix]
    private static void ChangeLevelClientRpcPatch(StartOfRound __instance)
    {
        if (!NetworkUtils.ShouldProcess("moon_changed")) return;

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
        // Debounce — coroutine enumerator fires on every iteration
        var now = DateTime.UtcNow;
        if (now - _lastShipLandedTime < EventDebounce)
            return;

        _lastShipLandedTime = now;

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
    private static void SyncNewProfitQuotaClientRpcPatch(TimeOfDay __instance)
    {
        if (!NetworkUtils.ShouldProcess("quota_fulfilled")) return;

        int newQuota = TimeOfDay.Instance?.profitQuota ?? 0;
        int quotaIndex = (TimeOfDay.Instance?.timesFulfilledQuota ?? 0) + 1;

        EventServer.SendEvent("quota_fulfilled", new Dictionary<string, object>
        {
            { "newQuota", newQuota },
            { "quotaIndex", quotaIndex }
        });
    }
}
