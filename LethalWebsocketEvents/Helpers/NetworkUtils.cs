using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace com.github.luckofthelefty.LethalWebsocketEvents.Helpers;

internal static class NetworkUtils
{
    public static bool IsConnected => NetworkManager.Singleton?.IsConnectedClient ?? false;
    public static bool IsServer => NetworkManager.Singleton?.IsServer ?? false;

    // Frame-based deduplication: track which events already fired this frame.
    // On a host, Harmony postfixes on ClientRpc methods fire twice per frame
    // (once for the server dispatch, once for the client execution).
    // This replaces the old __rpc_exec_stage reflection which was broken on solo host.
    private static readonly Dictionary<string, int> _lastProcessedFrame = new();

    public static ulong GetLocalClientId()
    {
        return NetworkManager.Singleton?.LocalClientId ?? 0;
    }

    public static bool IsLocalClientId(ulong clientId)
    {
        return clientId == GetLocalClientId();
    }

    /// <summary>
    /// Returns true the first time this eventKey is seen in the current frame,
    /// false on subsequent calls within the same frame. Use at the top of every
    /// ClientRpc postfix to prevent duplicate events on the host.
    /// </summary>
    public static bool ShouldProcess(string eventKey)
    {
        int frame = Time.frameCount;
        if (_lastProcessedFrame.TryGetValue(eventKey, out int lastFrame) && lastFrame == frame)
            return false;
        _lastProcessedFrame[eventKey] = frame;
        return true;
    }
}
