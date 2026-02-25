using System.Reflection;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalEvents.Helpers;

internal static class NetworkUtils
{
    public static bool IsConnected => NetworkManager.Singleton?.IsConnectedClient ?? false;
    public static bool IsServer => NetworkManager.Singleton?.IsServer ?? false;

    // Cached reflection for __rpc_exec_stage (internal field in NetworkBehaviour)
    private static readonly FieldInfo _rpcExecStageField =
        typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);

    public static ulong GetLocalClientId()
    {
        return NetworkManager.Singleton?.LocalClientId ?? 0;
    }

    public static bool IsLocalClientId(ulong clientId)
    {
        return clientId == GetLocalClientId();
    }

    /// <summary>
    /// Returns true if a ClientRpc is in the actual client execution stage.
    /// On the host, ClientRpc methods are called twice:
    ///   1. Initial call (__rpc_exec_stage = None) — serializes and dispatches the RPC, returns early
    ///   2. Client execution (__rpc_exec_stage = Client) — actually runs the method body
    /// Harmony postfix fires after BOTH calls, causing duplicate events.
    /// Use this guard at the top of every ClientRpc postfix to skip the dispatch call.
    /// </summary>
    public static bool IsClientRpcExecution(NetworkBehaviour instance)
    {
        if (instance == null || _rpcExecStageField == null) return true;

        // __RpcExecStage: None = 0, Server = 1, Client = 2
        int stage = (int)_rpcExecStageField.GetValue(instance);
        return stage == 2; // Client
    }
}
