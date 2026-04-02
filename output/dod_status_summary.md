# DoD Status Summary

Updated: 2026-04-01

## Overall

- Status: `near-done`
- Large implementation gaps are closed.
- Remaining work is mostly `blocked runtime/project cases` plus final strict parity cleanup.

## Channels

- Functional parity: `near-done`
- Strict API parity: `near-done`

### Done

- Main create/fetch/update/delete flows
- Messages, invites, join requests, moderation
- Most `error:*` cases
- Object-style public API
- Most async `Task` / `Task<T>` paths

### Blocked

- `setValue/addValue` success path on project `4`
  - available templates do not expose `fields`
- `closeChat`
  - live overlay/manual case is still not reproducible in a stable automated way

### Cleanup Only

- Final literal `EventEmitter` semantics
- Final DTO shape polish

## Multiplayer

- Functional parity: `near-done`
- Strict API parity: `near-done`

### Done

- `connect`, `disconnect`
- `tickRate`, `setMode`, `isConnected`, `isHost`
- `connectedPlayers`, `networkStats`, `myState`, `playersState`
- `sendMessage`, `onMessage/offMessage`, `onTick/offTick`
- `becameHost`, `hostMigrated`
- `error:connect`, `error:disconnect`, `error:sendState`

### Confirmed Runtime Note

- `peer -> host setPlayerState` requires `setPlayerInitializer(...)`
- Without initializer, the reference core ignores incoming peer state because the peer is missing in host-side `playersState`

### Blocked

- `becamePeer`
  - not reproduced in live runtime
  - current setup did not trigger a real host migration while the old host stayed connected
- `setPlayerInitializer(...)` through the Unity parity helper
  - partially blocked by the next WebGL rebuild path

### Cleanup Only

- Final Unity-specific convenience API cleanup
- Final typed surface review against the reference core

## Practical Next Step

- Treat current state as baseline.
- Finish remaining strict source cleanup.
- Move blocked runtime cases into separate focused follow-up work instead of mixing them with main parity delivery.
