using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalEvents.Events;

internal class GameEvent
{
    [JsonProperty("event")]
    public string Event { get; set; }

    [JsonProperty("timestamp")]
    public string Timestamp { get; set; }

    [JsonProperty("data")]
    public Dictionary<string, object> Data { get; set; }

    public GameEvent(string eventName, Dictionary<string, object> data = null)
    {
        Event = eventName;
        Timestamp = DateTime.UtcNow.ToString("o");
        Data = data ?? new Dictionary<string, object>();
    }

    public static GameEvent Create(string eventName, Dictionary<string, object> data = null)
    {
        return new GameEvent(eventName, data);
    }
}
