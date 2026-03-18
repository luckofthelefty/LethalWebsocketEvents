using com.github.luckofthelefty.LethalWebsocketEvents.Events;
using com.github.luckofthelefty.LethalWebsocketEvents.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using WebSocketSharp.Server;

namespace com.github.luckofthelefty.LethalWebsocketEvents.Server;

internal static class EventServer
{
    public static int WebSocketPort => ConfigManager.Server_WebSocketPort.Value;
    public static bool IsRunning { get; private set; }

    private static WebSocketServer _webSocketServer;
    private static readonly object _broadcastLock = new object();

    public static void Initialize()
    {
        Application.quitting += () =>
        {
            Stop();
        };

        if (ConfigManager.Server_AutoStart.Value)
        {
            Start();
        }
    }

    public static void Start()
    {
        if (IsRunning)
        {
            Logger.LogWarning("Event server is already running!");
            return;
        }

        try
        {
            IsRunning = true;

            _webSocketServer = new WebSocketServer($"ws://{System.Net.IPAddress.Any}:{WebSocketPort}");
            _webSocketServer.AddWebSocketService<EventBehavior>("/events");
            _webSocketServer.Start();

            if (!_webSocketServer.IsListening)
            {
                Logger.LogError("Failed to start WebSocket server. The port might already be in use.");
                Stop();
                return;
            }

            Logger.LogInfo($"WebSocket event server started on ws://localhost:{WebSocketPort}/events");
        }
        catch (SocketException ex)
        {
            Logger.LogError($"Failed to start WebSocket server. {ex.Message}");
            Stop();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to start WebSocket server. {ex}");
            Stop();
        }
    }

    public static void Stop()
    {
        if (!IsRunning) return;

        IsRunning = false;

        _webSocketServer?.Stop();
        _webSocketServer = null;

        Logger.LogInfo("Event server stopped.");
    }

    public static void SendEvent(GameEvent gameEvent)
    {
        if (_webSocketServer == null || !_webSocketServer.IsListening)
        {
            return;
        }

        lock (_broadcastLock)
        {
            try
            {
                var json = JsonConvert.SerializeObject(gameEvent);

                if (ConfigManager.ExtendedLogging.Value)
                {
                    Logger.LogInfo($"Broadcasting event: {json}");
                }

                foreach (var path in _webSocketServer.WebSocketServices.Paths)
                {
                    var serviceHost = _webSocketServer.WebSocketServices[path];
                    serviceHost.Sessions.Broadcast(json);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to broadcast event. {ex}");
            }
        }
    }

    public static void SendEvent(string eventName, Dictionary<string, object> data = null)
    {
        SendEvent(GameEvent.Create(eventName, data));
    }
}
