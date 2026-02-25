using com.github.luckofthelefty.LethalEvents.Managers;
using GameNetcodeStuff;
using System;

namespace com.github.luckofthelefty.LethalEvents.Helpers;

internal static class PlayerUtils
{
    public static PlayerControllerB GetLocalPlayerScript()
    {
        if (GameNetworkManager.Instance == null)
            return null;

        return GameNetworkManager.Instance.localPlayerController;
    }

    public static bool IsLocalPlayer(PlayerControllerB playerScript)
    {
        return playerScript == GetLocalPlayerScript();
    }

    public static bool IsLocalPlayerSpawned()
    {
        return GetLocalPlayerScript() != null;
    }

    public static PlayerControllerB GetPlayerScriptByClientId(ulong clientId)
    {
        if (StartOfRound.Instance == null)
            return null;

        foreach (var playerScript in StartOfRound.Instance.allPlayerScripts)
        {
            if (playerScript.actualClientId == clientId)
            {
                return playerScript;
            }
        }

        return null;
    }

    /// <summary>
    /// Counts players that are actually connected (controlledByClient + not dead).
    /// connectedPlayersAmount doesn't include the local player, so we add 1.
    /// </summary>
    public static int GetConnectedPlayerCount()
    {
        if (StartOfRound.Instance == null)
            return 1;

        return StartOfRound.Instance.connectedPlayersAmount + 1;
    }

    public static string GetPlayerName(PlayerControllerB playerScript)
    {
        if (playerScript == null)
            return "Unknown";

        return playerScript.playerUsername ?? "Unknown";
    }

    /// <summary>
    /// Gets player name from a player object index (used by many monster ClientRpc methods).
    /// </summary>
    public static string GetPlayerName(int playerObjectIndex)
    {
        if (StartOfRound.Instance?.allPlayerScripts == null ||
            playerObjectIndex < 0 || playerObjectIndex >= StartOfRound.Instance.allPlayerScripts.Length)
            return "Unknown";

        return GetPlayerName(StartOfRound.Instance.allPlayerScripts[playerObjectIndex]);
    }

    /// <summary>
    /// Returns true if the event should fire for this player based on filter settings.
    /// If no filter is active, always returns true (all players tracked).
    /// </summary>
    public static bool ShouldTrackPlayer(PlayerControllerB playerScript)
    {
        // If a specific player name is set, filter by that
        string filterName = ConfigManager.Filter_PlayerName?.Value;
        if (!string.IsNullOrEmpty(filterName))
        {
            string playerName = GetPlayerName(playerScript);
            return string.Equals(playerName, filterName, StringComparison.OrdinalIgnoreCase);
        }

        // If local-only mode is enabled, only track the local player
        if (ConfigManager.Filter_LocalPlayerOnly?.Value == true)
        {
            return IsLocalPlayer(playerScript);
        }

        // No filter — track all players
        return true;
    }

    /// <summary>
    /// Overload for when you only have a player name string.
    /// </summary>
    public static bool ShouldTrackPlayer(string playerName)
    {
        string filterName = ConfigManager.Filter_PlayerName?.Value;
        if (!string.IsNullOrEmpty(filterName))
        {
            return string.Equals(playerName, filterName, StringComparison.OrdinalIgnoreCase);
        }

        if (ConfigManager.Filter_LocalPlayerOnly?.Value == true)
        {
            var localPlayer = GetLocalPlayerScript();
            return localPlayer != null && string.Equals(GetPlayerName(localPlayer), playerName, StringComparison.OrdinalIgnoreCase);
        }

        return true;
    }

    /// <summary>
    /// Overload for when you have a playerId index into allPlayerScripts.
    /// </summary>
    public static bool ShouldTrackPlayer(int playerId)
    {
        if (StartOfRound.Instance?.allPlayerScripts == null ||
            playerId < 0 || playerId >= StartOfRound.Instance.allPlayerScripts.Length)
            return true; // Can't determine, let it through

        return ShouldTrackPlayer(StartOfRound.Instance.allPlayerScripts[playerId]);
    }
}
