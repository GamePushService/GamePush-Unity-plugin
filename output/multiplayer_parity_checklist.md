# Multiplayer Parity Checklist

Updated: 2026-04-01

## Live Confirmed

- `connect({ channelId })`: `pass`
- `disconnect({ channelId })`: `pass`
- `tickRate`: `pass`
- `setMode("fast"|"smooth")`: `pass`
- `isConnected`: `pass`
- `isHost`: `pass`
- `connectedPlayers`: `pass`
- `networkStats`: `pass`
- `sendMessage(eventName, data, { target, echo })`: `pass`
- `onMessage/offMessage`: `pass`
- `onTick/offTick`: `pass`
- `becameHost`: `pass`
- `hostMigrated`: `pass`
- `error:connect`: `pass`
- `error:disconnect`: `pass`
- `error:sendState`: `pass`

## Important Runtime Note

- `peer -> host setPlayerState`: `pass with initializer`
  - Without `setPlayerInitializer`, host-side `playersState` does not contain peer entries, so incoming `PEER_STATE` is ignored by the reference core.
  - After `setPlayerInitializer(...)`, host receives `playersUpdated` and host-side `playersState` is updated correctly.

## Confirmed Behavior Details

- `playersState` in the reference web runtime is a `Map`, not a plain object.
- Unity getter parity currently serializes `playersState` as a JSON object keyed by `playerId`.
- For `error:sendState`, a synchronous serialization failure (for example circular state in raw JS runtime) emits the event repeatedly while invalid local state remains active.
- `peer -> host setPlayerState` was re-checked after `setPlayerInitializer(...)` and confirmed both in raw runtime and through the Unity getter path.

## Blocked / Not Fully Proven Yet

- `becamePeer`: `blocked`
  - The event is emitted only on real host migration while the previous host stays connected.
  - Simple `disconnect -> reconnect` does not trigger `becamePeer`; it only creates a new peer connection.
  - Current local setup did not yet produce a live migration path that satisfies `HostSelector` conditions without disconnecting the old host.
  - Artificial mutation of exposed `connectedPlayers` metrics on the current host session did not trigger migration either, so this path remains unproven in live runtime.
  - Additional 68s live wait after forced degradation of the current host on the peer observer side also produced no `hostMigrated` and no `becamePeer`.

- `setPlayerInitializer(...)` through Unity parity harness: `partially verified`
  - Raw runtime behavior is confirmed live.
  - Browser-side parity helper for initializer was added locally but the next WebGL rebuild was blocked by Unity batchmode `case sensitive file system`.

## Remaining DoD Work

- Reproduce and confirm `becamePeer` with a deterministic migration scenario.
- Re-run `setPlayerInitializer(...)` through the Unity parity harness after the next successful WebGL rebuild.
- Finalize strict API parity review for any remaining Unity-specific convenience surface in `GP_Multiplayer`.
