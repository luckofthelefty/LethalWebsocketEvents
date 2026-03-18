# LethalEvents

A BepInEx mod for Lethal Company that streams in-game events over WebSocket for Streamerbot integration.

Forked from [ZehsTeam/Lethal-Company-StreamOverlays](https://github.com/ZehsTeam/Lethal-Company-StreamOverlays) — stripped down to focus purely on discrete event broadcasting rather than overlay stats.

## How It Works

The mod runs a WebSocket server (default port `8765`) that broadcasts JSON events as they happen in-game. Connect Streamerbot (or any WebSocket client) to trigger custom alerts, sounds, scene changes, and more.

**WebSocket URL:** `ws://localhost:8765/events`

## Event Format

All events follow this structure:

```json
{
  "event": "player_death",
  "timestamp": "2026-02-24T15:30:00.0000000Z",
  "data": {
    "player": "Nick",
    "causeOfDeath": "Mauling"
  }
}
```

## Supported Events

### Player Events
| Event | Description | Data Fields |
|---|---|---|
| `player_death` | A player died | `player`, `causeOfDeath`, `playerId` |
| `player_damage` | A player took damage | `player`, `damage`, `health`, `critical` |
| `player_healed` | A player was healed | `player`, `health` |
| `player_joined` | A player connected | `player`, `playerCount` |
| `player_left` | A player disconnected | `player`, `playerCount` |
| `player_emote` | A player performed an emote | `player`, `emoteId` |

### Round Events
| Event | Description | Data Fields |
|---|---|---|
| `round_start` | Game started (ship departing) | `moon`, `weather` |
| `round_end` | Round ended | — |
| `ship_landed` | Ship landed on moon | `moon` |
| `ship_leaving` | Ship is leaving the moon | — |
| `day_changed` | Day/level info updated | `moon`, `weather` |
| `moon_changed` | Route changed to new moon | `moon`, `weather` |
| `quota_fulfilled` | Profit quota met | `newQuota`, `quotaIndex` |
| `players_revived` | Dead players revived (new day) | — |
| `vote_to_leave` | Someone voted to leave early | — |

### Enemy Events
| Event | Description | Data Fields |
|---|---|---|
| `enemy_spawned` | An enemy spawned | `enemyType`, `location` (indoor/outdoor) |
| `enemy_killed` | An enemy was killed | `enemyType` |
| `enemy_state_changed` | Enemy changed behavior state | `enemyType`, `stateIndex` |
| `enemy_hit` | An enemy was hit | `enemyType`, `force`, `player` |

### Item Events
| Event | Description | Data Fields |
|---|---|---|
| `item_grabbed` | A player picked up an item | `player`, `item`, `scrapValue` |
| `item_dropped` | A player dropped an item | `player`, `inShip` |
| `apparatus_pulled` | Apparatus disconnected | `player` |

### Facility Events
| Event | Description | Data Fields |
|---|---|---|
| `landmine_exploded` | A landmine exploded | — |
| `turret_mode_changed` | Turret changed mode | `mode`, `modeIndex` |
| `teleporter_used` | Teleporter activated | `isInverse` |

### Monster-Specific Events
| Event | Description | Data Fields |
|---|---|---|
| `bracken_grab` | Bracken grabbed a player | `player` |
| `jester_kill` | Jester killed a player | `player` |
| `ghost_girl_haunt` | Ghost Girl haunting a player | `player` |
| `coilhead_moving` | Coilhead started moving | — |
| `coilhead_stopped` | Coilhead stopped | — |
| `masked_mimic` | Masked mimicking a player | `player` |
| `nutcracker_shot` | Nutcracker fired | — |
| `giant_grab` | Forest Giant grabbed a player | `player` |
| `snare_flea_cling` | Snare Flea clung to a player | `player` |
| `spider_web_trip` | Player tripped a spider web | `player` |
| `blob_kill` | Blob killed a player | `player` |
| `thumper_hit` | Thumper hit a player | `player` |
| `eyeless_dog_kill` | Eyeless Dog killed a player | `player` |
| `baboon_hawk_stab` | Baboon Hawk stabbed a player | `player` |

### Connection Events
| Event | Description | Data Fields |
|---|---|---|
| `connected` | Client connected to WebSocket | `modVersion`, `modName` |

## Cause of Death Values

The `causeOfDeath` field in `player_death` events can be:
`Unknown`, `Bludgeoning`, `Gravity`, `Blast`, `Strangulation`, `Suffocation`, `Mauling`, `Gunshots`, `Crushing`, `Drowning`, `Abandoned`, `Electrocution`, `Kicking`, `Burning`, `Stabbing`, `Fan`

## Configuration

Config file is created at:
```
%localappdata%\..\LocalLow\ZeekerssRBLX\Lethal Company\LethalEvents\global.cfg
```

| Setting | Default | Description |
|---|---|---|
| `Server.AutoStart` | `true` | Start WebSocket server on game launch |
| `Server.WebSocketPort` | `8765` | WebSocket server port |
| `General.ExtendedLogging` | `false` | Log all broadcasted events to BepInEx console |
| `Filter.LocalPlayerOnly` | `false` | Only fire player-specific events for the local player |
| `Filter.PlayerName` | *(empty)* | Only fire player-specific events for this player name |

## Streamerbot Setup

1. In Streamerbot, go to **Servers/Clients** > **WebSocket Clients**
2. Add a new client: `ws://localhost:8765/events`
3. Create actions triggered by the `event` field in received messages
4. Example: On `player_death` where `causeOfDeath` = `Mauling` → play jumpscare sound

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx) for Lethal Company
2. Place `com.github.luckofthelefty.LethalEvents.dll` in `BepInEx/plugins/`
3. Launch the game

## Building

```bash
dotnet restore
dotnet build
```

Output DLL: `LethalEvents/bin/Debug/netstandard2.1/com.github.luckofthelefty.LethalEvents.dll`

## Support

This mod is provided as-is. **Support is limited to none.** Feel free to fork and modify for your own use.

## Credits

Based on [StreamOverlays](https://github.com/ZehsTeam/Lethal-Company-StreamOverlays) by Zehs.

## License

MIT
