using BepInEx.Configuration;

namespace com.github.luckofthelefty.LethalEvents.Managers;

internal static class ConfigManager
{
    public static ConfigFile ConfigFile { get; private set; }

    public static ConfigEntry<bool> ExtendedLogging { get; private set; }
    public static ConfigEntry<bool> Server_AutoStart { get; private set; }
    public static ConfigEntry<int> Server_WebSocketPort { get; private set; }
    public static ConfigEntry<bool> Filter_LocalPlayerOnly { get; private set; }
    public static ConfigEntry<string> Filter_PlayerName { get; private set; }

    public static void Initialize(ConfigFile configFile)
    {
        ConfigFile = configFile;

        ExtendedLogging = configFile.Bind(
            "General", "ExtendedLogging", false,
            "Enable extended logging for debugging.");

        Server_AutoStart = configFile.Bind(
            "Server", "AutoStart", true,
            "If enabled, the WebSocket server will automatically start when the game launches.");

        Server_WebSocketPort = configFile.Bind(
            "Server", "WebSocketPort", 8765,
            "The WebSocket port for the event server.");

        Filter_LocalPlayerOnly = configFile.Bind(
            "Filter", "LocalPlayerOnly", false,
            "If enabled, player-specific events (death, damage, items) only fire for the local player. " +
            "Global events (enemy spawns, round start, etc.) always fire regardless of this setting.");

        Filter_PlayerName = configFile.Bind(
            "Filter", "PlayerName", "",
            "If set, player-specific events only fire for this player name. " +
            "Leave empty to use LocalPlayerOnly logic instead. " +
            "Useful if you want to track a specific player who isn't running the mod.");
    }
}
