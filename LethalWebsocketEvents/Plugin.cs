using BepInEx;
using com.github.luckofthelefty.LethalWebsocketEvents.Managers;
using com.github.luckofthelefty.LethalWebsocketEvents.Patches;
using com.github.luckofthelefty.LethalWebsocketEvents.Server;
using HarmonyLib;
using System.Threading.Tasks;

namespace com.github.luckofthelefty.LethalWebsocketEvents;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    private readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static Plugin Instance { get; private set; }

    #pragma warning disable IDE0051
    private void Awake()
    #pragma warning restore IDE0051
    {
        Instance = this;

        LethalWebsocketEvents.Logger.Initialize(BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID));
        LethalWebsocketEvents.Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded!");

        ConfigManager.Initialize(Config);

        // Player patches
        _harmony.PatchAll(typeof(PlayerDeathPatch));
        _harmony.PatchAll(typeof(PlayerDamagePatch));
        _harmony.PatchAll(typeof(EmotePatch));

        // Round patches
        _harmony.PatchAll(typeof(RoundPatch));
        _harmony.PatchAll(typeof(QuotaPatch));

        // Enemy patches
        _harmony.PatchAll(typeof(EnemyPatch));

        // Item patches
        _harmony.PatchAll(typeof(ItemPatch));
        _harmony.PatchAll(typeof(ApparatusPatch));

        // Facility patches
        _harmony.PatchAll(typeof(LandminePatch));
        _harmony.PatchAll(typeof(TurretPatch));
        _harmony.PatchAll(typeof(TeleporterPatch));

        // Monster-specific patches
        _harmony.PatchAll(typeof(BrackenPatch));
        _harmony.PatchAll(typeof(JesterPatch));
        _harmony.PatchAll(typeof(GhostGirlPatch));
        _harmony.PatchAll(typeof(CoilheadPatch));
        _harmony.PatchAll(typeof(MaskedPatch));
        _harmony.PatchAll(typeof(NutcrackerPatch));
        _harmony.PatchAll(typeof(ForestGiantPatch));
        _harmony.PatchAll(typeof(SnareFleaPatch));
        _harmony.PatchAll(typeof(SpiderPatch));
        _harmony.PatchAll(typeof(BlobPatch));
        _harmony.PatchAll(typeof(ThumperPatch));
        _harmony.PatchAll(typeof(EyelessDogPatch));
        _harmony.PatchAll(typeof(BaboonHawkPatch));

        // Connection/misc patches
        _harmony.PatchAll(typeof(ConnectionPatch));
        _harmony.PatchAll(typeof(DisconnectPatch));
        _harmony.PatchAll(typeof(VoteToLeavePatch));

        Task.Run(EventServer.Initialize);
    }
}
