using com.github.luckofthelefty.LethalWebsocketEvents.Events;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace com.github.luckofthelefty.LethalWebsocketEvents.Server;

public class EventBehavior : WebSocketBehavior
{
    protected override void OnOpen()
    {
        var connectEvent = GameEvent.Create("connected", new Dictionary<string, object>
        {
            { "modVersion", MyPluginInfo.PLUGIN_VERSION },
            { "modName", MyPluginInfo.PLUGIN_NAME }
        });

        Send(JsonConvert.SerializeObject(connectEvent));
    }
}
