using BepInEx.Configuration;

namespace com.github.luckofthelefty.LethalEvents.Managers;

internal static class ConfigManager
{
    public static ConfigFile ConfigFile { get; private set; }

    public static ConfigEntry<bool> ExtendedLogging { get; private set; }
    public static ConfigEntry<bool> Server_AutoStart { get; private set; }
    public static ConfigEntry<int> Server_WebSocketPort { get; private set; }

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
    }
}
