using GameNetcodeStuff;

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

    public static string GetPlayerName(PlayerControllerB playerScript)
    {
        if (playerScript == null)
            return "Unknown";

        return playerScript.playerUsername ?? "Unknown";
    }
}
