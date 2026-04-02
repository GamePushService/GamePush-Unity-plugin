# Channels Parity Checklist

Refs:
- `/Users/pprudnikov/Projects/CodeX/GP/game-score-sdk-master/src/core/Channels/Channels.ts`
- `/Users/pprudnikov/Projects/CodeX/GP-Unity-Plugin/Demo/Assets/Plugins/GamePush/Runtime/Modules/GP_Channels.cs`

Legend:
- `pass`: source-level or live parity подтверждено
- `mismatch`: найдено расхождение с эталоном
- `unverified`: ещё не прогнано живым сценарием
- `blocked`: нельзя честно проверить без второго клиента или ручного overlay-сценария

## Current Summary

Source-level blockers, которые ещё мешают закрыть strict DoD:

1. Публичный Unity API для `Channels` всё ещё не `1:1` по сигнатурам, но async surface уже сильно подтянут.
   `setValue/addValue`, `create/update/deleteChannel`, `fetchChannel/fetchPersonalChannel/fetchFeedChannel`, `fetchChannels/fetchMoreChannels`, `fetchMembers/fetchInvites/fetchJoinRequests/fetchMessages` и их `fetchMore*`-варианты, а также message/invite/member actions уже переведены на `Task`/`Task<T>`. Оставшийся gap теперь уже не в отсутствии async surface, а в буквальном ecosystem-паритете с Promise/emitter контрактом эталона.

2. Unity публично всё ещё торчит шире эталона, но surface уже заметно сужен.
   Message-ветка, invite/request/member flows и `delete/fetch/getters` уже переведены на object-style public path, но в `GP_Channels.cs` ещё остаются convenience-overloads и неидеальный state/result-contract.

3. `on/off` всё ещё не `1:1` по emitter-контракту, но gap стал уже.
   Публичный alias `message` уже добавлен и маппится на realtime `event:message`, а simple success-events теперь тоже можно слушать через `UnityAction<GP_Data>` с `null` payload. Оставшийся gap — отсутствие literal typed `EventEmitter` semantics эталона.

4. State accessors всё ещё не идеально совпадают по контракту, но стали ближе к эталону.
   `getLocalChannelState` и `getChannelField` в Unity теперь отдают typed models вместо `GP_Data`, но их shape всё ещё Unity-specific (`Dictionary<string, object>` / serializable DTO) вместо точного runtime класса эталона.

5. `setValue` / `addValue` всё ещё не закрыты полностью, но result-contract уже выровнен по форме.
   Input-контракт в Unity переведён на query-object `{ channelId, key, value, version? }`, а публичный API теперь async `Task<{ success, value }>` через `ChannelStateMutationResultData`. Оставшийся gap — success-path нельзя честно прогнать на project `4`, а realtime/event side всё ещё живёт отдельным typed payload.

Runtime status на сейчас:

- Live-confirmed `sandbox vs Unity`: `createChannel`, `updateChannel`, `deleteChannel`, `fetchChannel`, `fetchPersonalChannel`, `fetchFeedChannel`, `fetchChannels`, `fetchMoreChannels`
- Live-confirmed `sandbox vs Unity`: `event:updateChannel`, `event:deleteChannel`
- Live-confirmed `sandbox vs Unity`: `sendMessage`, `sendPersonalMessage`, `sendFeedMessage`, `fetchMessages`, `fetchMoreMessages`
- Live-confirmed `sandbox vs Unity`: `fetchPersonalMessages`, `fetchMorePersonalMessages`, `fetchFeedMessages`, `fetchMoreFeedMessages`
- Live-confirmed `sandbox vs Unity`: message realtime/edit flows `event:message`, `editMessage`, `deleteMessage`, `event:editMessage`, `event:deleteMessage`, `error:editMessage`, `error:deleteMessage`
- Live-confirmed `sandbox vs Unity`: message error-paths `error:sendMessage`, `error:fetchMessages`, `error:fetchMoreMessages` on invalid `channelId` -> `empty_channel_id`
- Live-confirmed `sandbox vs Unity`: `canBeOnline`, `isMainChatEnabled`, `mainChatId`
- Live-confirmed `sandbox vs Unity`: `openChat`, `openPersonalChat`, `openFeed`, `openChat` event
- Live-confirmed `sandbox vs Unity`: invite/request flows `cancelInvite`, `rejectInvite`, `fetchMoreInvites`, `fetchChannelInvites`, `fetchMoreChannelInvites`, `fetchMoreSentInvites`, `fetchMoreJoinRequests`, `fetchMoreSentJoinRequests`
- Live-confirmed `sandbox vs Unity`: moderation/member flows `leave`, `cancelJoin`, `rejectJoinRequest`, `kick`, `mute`, `unmute`, `event:leave`, `event:cancelJoin`, `event:rejectJoinRequest`, `event:mute`, `event:unmute`
- Live-confirmed on the same empty-state channel: `getLocalChannelState`, `getChannelField`, `getChannelValue`
- Live-confirmed only on empty-field channels: `setValue/addValue` дают expected error-path `fields_not_found`; extra Unity WebGL alert убран
- Частично live-confirmed по коду и поведению: `error:openFeed` separation, `authorId` path у `fetchFeedMessages`, ACL shape для `createChannel`
- Negative-case note: `openFeed({ playerId: 0 })` не даёт отдельный `error:openFeed`; и sandbox, и Unity уходят в `error:fetchChannel(empty_channel_id)`. При `playerId=999999999` обе стороны дают и `error:fetchFeedChannel(player_not_found)`, и целевой `error:openFeed(player_not_found)`.
- Message/feed note: `fetchPersonalMessages`, `fetchMorePersonalMessages`, `fetchFeedMessages`, `fetchMoreFeedMessages` на `playerId=999999999` не эмитят `error:*`; и sandbox, и Unity возвращают пустой `{ items: [], canLoadMore: false }`.
- Project config note: на project `4` templates `1` и `2` существуют, но возвращают `fields: []`; templates `3+` отсутствуют (`channel_template_not_found`). Это блокирует честный success-path для `setValue/addValue`.
- Остальное по `Channels` ещё требует системного прогона `sandbox vs Unity`

## Properties

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `canBeOnline` | pass | pass | Sandbox и Unity отдают `true` |
| `isMainChatEnabled` | pass | pass | Sandbox и Unity отдают `false` |
| `mainChatId` | pass | pass | Sandbox и Unity отдают `17` |

## Open / Overlay

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `openChat` | mismatch | pass | Runtime event-flow совпал на object-style `openChat({ id, tags: [] })`; strict signature всё ещё не `1:1` |
| `openPersonalChat` | mismatch | pass | Runtime flow совпал на `openPersonalChat({ playerId, tags: [] })`: success идёт через `openChat`, затем в обоих runtime приходит trailing `error:openChat(Cancel last request)` |
| `openFeed` | mismatch | pass | Runtime flow совпал на `openFeed({ playerId, tags: [] })`: success идёт через `openChat`, затем в обоих runtime приходит trailing `error:openChat(Cancel last request)`; первый sandbox-run дал transient backend `WriteConflict` |
| `openChat` event | pass | pass | `openChat` приходит и в sandbox, и в Unity; Unity дополнительно даёт `fetchChannel` перед `openChat` |
| `closeChat` event | pass | blocked | Unity эмитит `closeChat` |
| `error:openChat` | pass | pass | Live-confirmed на `openPersonalChat/openFeed`: в обоих runtime приходит `Cancel last request` |
| `error:openFeed` | pass | pass | Live-confirmed на `playerId=999999999`: обе стороны дают `player_not_found`; при `playerId=0` обе стороны вместо этого уходят в `error:fetchChannel(empty_channel_id)` |

## Membership / Moderation

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `join` | mismatch | pass | Live-confirmed в private-channel request flow: action success совпал, public API в Unity теперь async `Task`, но result/emitter ecosystem всё ещё не буквально идентичен эталону |
| `leave` | mismatch | pass | Live-confirmed на joined-member channel: action success совпал, а обе стороны получают `event:leave` с `reason:\"leave\"`; public API в Unity теперь async `Task`, но strict parity всё ещё не `1:1` |
| `cancelJoin` | mismatch | pass | Live-confirmed на pending request: requester action success совпал, owner получает `event:cancelJoin`, а `fetchJoinRequests` после этого пустой |
| `acceptJoinRequest` | mismatch | pass | Live-confirmed owner-side в request flow: action success совпал; public API в Unity теперь async `Task`, но bridge всё ещё не даёт typed payload как в эталоне |
| `rejectJoinRequest` | mismatch | pass | Live-confirmed owner-side на pending request: action success совпал, requester получает `event:rejectJoinRequest`; public API в Unity теперь async `Task`, но strict parity всё ещё не `1:1` |
| `kick` | mismatch | pass | Live-confirmed на joined-member channel: action success совпал, а side-effect в обоих runtime идёт через `event:leave` с `reason:\"kick\"`; public API в Unity теперь async `Task`, но strict parity всё ещё не `1:1` |
| `mute` | mismatch | pass | Live-confirmed owner-side на joined-member channel: action success совпал, а обе стороны получают `event:mute` с одинаковым `unmuteAt`; public API в Unity теперь async `Task`, но strict parity всё ещё не `1:1` |
| `unmute` | mismatch | pass | Live-confirmed owner-side на joined-member channel: action success совпал, а обе стороны получают `event:unmute`; public API в Unity теперь async `Task`, но strict parity всё ещё не `1:1` |
| `fetchMembers` | mismatch | pass | Runtime payload совпал; backend на обеих сторонах отдаёт `canLoadMore:true` даже при одном member и `limit=1`. Public API в Unity теперь async `Task<FetchMembersResultData>` |
| `fetchMoreMembers` | mismatch | pass | Runtime payload совпал; после первого page `fetchMoreMembers(limit=1)` отдаёт пустой список и `canLoadMore:false`. Public API в Unity теперь async `Task<FetchMembersResultData>` |
| `event:join` | pass | pass | Live-confirmed в invite accept flow: receiver join event совпал по payload shape |
| `event:leave` | pass | pass | Live-confirmed на `leave` и `kick`: обе стороны получают одинаковый payload; для `kick` reason=`kick` |
| `event:cancelJoin` | pass | pass | Live-confirmed owner-side на `cancelJoin`: payload `{ channelId, playerId }` совпал |
| `event:joinRequest` | pass | pass | Live-confirmed owner-side: payload shape совпал |
| `event:rejectJoinRequest` | pass | pass | Live-confirmed owner/requester-side: payload `{ channelId, playerId }` совпал |
| `event:mute` | pass | pass | Live-confirmed на owner/requester-side: payload `{ channelId, playerId, unmuteAt }` совпал |
| `event:unmute` | pass | pass | Live-confirmed на owner/requester-side: payload `{ channelId, playerId }` совпал |
| `error:join` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:leave` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:cancelJoin` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:acceptJoinRequest` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:rejectJoinRequest` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:kick` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:mute` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:unmute` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:fetchMembers` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:fetchMoreMembers` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |

## Invites / Requests

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `sendInvite` | mismatch | pass | Live-confirmed в two-client flow: sender-side invite создаётся и payload совпадает по полям `channelId/playerToId/playerFromId/date`; public API в Unity теперь async `Task`, но strict signature всё ещё не `1:1` |
| `cancelInvite` | mismatch | pass | Live-confirmed sender-side: action success совпал, receiver получает `event:cancelInvite`, а invite исчезает из `fetchInvites/fetchSentInvites`; public API в Unity теперь async `Task`, но strict signature всё ещё не `1:1` |
| `acceptInvite` | mismatch | pass | Live-confirmed в two-client flow: receiver-side accept проходит и post-state совпадает; public API в Unity теперь async `Task`, но strict signature всё ещё не `1:1` |
| `rejectInvite` | mismatch | pass | Live-confirmed receiver-side: action success совпал, sender получает `event:rejectInvite`, а invite исчезает из `fetchSentInvites`; public API в Unity теперь async `Task`, но strict signature всё ещё не `1:1` |
| `fetchInvites` | mismatch | pass | Live-confirmed в invite-state: runtime payload совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchInvitesResultData>` |
| `fetchMoreInvites` | mismatch | pass | Live-confirmed на receiver-side с двумя pending invites: второй page совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchInvitesResultData>` |
| `fetchChannelInvites` | mismatch | pass | Live-confirmed owner-side: runtime payload совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchInvitesResultData>` |
| `fetchMoreChannelInvites` | mismatch | pass | Live-confirmed owner-side после `limit=1`: обе стороны отдают пустой page и `canLoadMore:false`. Public API в Unity теперь async `Task<FetchInvitesResultData>` |
| `fetchSentInvites` | mismatch | pass | Live-confirmed в invite-state: runtime payload совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchInvitesResultData>` |
| `fetchMoreSentInvites` | mismatch | pass | Live-confirmed sender-side с двумя pending invites: второй page совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchInvitesResultData>` |
| `fetchJoinRequests` | mismatch | pass | Live-confirmed в request-state: runtime payload совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchJoinRequestsResultData>` |
| `fetchMoreJoinRequests` | mismatch | pass | Live-confirmed owner-side после `limit=1`: обе стороны отдают пустой page и `canLoadMore:false`. Public API в Unity теперь async `Task<FetchJoinRequestsResultData>` |
| `fetchSentJoinRequests` | mismatch | pass | Live-confirmed в request-state: runtime payload совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchJoinRequestsResultData>` |
| `fetchMoreSentJoinRequests` | mismatch | pass | Live-confirmed requester-side с двумя pending requests: второй page совпал (`items/canLoadMore`). Public API в Unity теперь async `Task<FetchJoinRequestsResultData>` |
| `event:invite` | pass | pass | Live-confirmed на receiver-side; payload shape совпал |
| `event:cancelInvite` | pass | pass | Live-confirmed на sender/receiver-side: payload `{ channelId, playerToId, playerFromId }` совпал |
| `event:acceptInvite` | pass | blocked | Event name заведен |
| `event:rejectInvite` | pass | pass | Live-confirmed на sender/receiver-side: payload `{ channelId, playerToId, playerFromId }` совпал |
| `error:sendInvite` | pass | blocked | Event name заведен |
| `error:cancelInvite` | pass | blocked | Event name заведен |
| `error:acceptInvite` | pass | blocked | Event name заведен |
| `error:rejectInvite` | pass | blocked | Event name заведен |
| `error:fetchInvites` | pass | blocked | Event name заведен |
| `error:fetchMoreInvites` | pass | blocked | Event name заведен |
| `error:fetchChannelInvites` | pass | blocked | Event name заведен |
| `error:fetchMoreChannelInvites` | pass | blocked | Event name заведен |
| `error:fetchSentInvites` | pass | blocked | Event name заведен |
| `error:fetchMoreSentInvites` | pass | blocked | Event name заведен |
| `error:fetchJoinRequests` | pass | blocked | Event name заведен |
| `error:fetchMoreJoinRequests` | pass | blocked | Event name заведен |
| `error:fetchSentJoinRequests` | pass | blocked | Event name заведен |
| `error:fetchMoreSentJoinRequests` | pass | blocked | Event name заведен |

## Messages

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `sendMessage` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<ChannelMessageResultData>`, но typed shape/emitter contract всё ещё не буквально как в эталоне |
| `sendPersonalMessage` | mismatch | pass | Runtime payload совпал; в Unity наружу это тот же `sendMessage` event-path |
| `sendFeedMessage` | mismatch | pass | Runtime payload совпал; в Unity наружу это тот же `sendMessage` event-path |
| `editMessage` | mismatch | pass | Live-confirmed owner-side: action success совпал, а receiver получает `event:editMessage` с тем же payload shape; public API в Unity теперь async `Task<ChannelMessageResultData>`, но strict parity всё ещё не `1:1` |
| `deleteMessage` | mismatch | pass | Live-confirmed owner-side: action success совпал, а receiver получает `event:deleteMessage` с тем же payload shape; public API в Unity теперь async `Task`, но strict parity всё ещё не `1:1` |
| `fetchMessages` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<FetchMessagesResultData>`, но strict signature всё ещё не 1:1 |
| `fetchMoreMessages` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<FetchMessagesResultData>`, но strict signature всё ещё не 1:1 |
| `fetchPersonalMessages` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<FetchMessagesResultData>`, но strict signature всё ещё не 1:1 |
| `fetchMorePersonalMessages` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<FetchMessagesResultData>`, но strict signature всё ещё не 1:1 |
| `fetchFeedMessages` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<FetchMessagesResultData>`, но strict signature всё ещё не 1:1 |
| `fetchMoreFeedMessages` | mismatch | pass | Runtime payload совпал; public API в Unity теперь async `Task<FetchMessagesResultData>`, но strict signature всё ещё не 1:1 |
| `message` | pass | unverified | Public alias `message` добавлен и маппится на realtime `event:message`, но payload наружу всё ещё `GP_Data`, а не typed model эталона |
| `event:message` | pass | pass | Live-confirmed on receiver-side: realtime payload shape совпал |
| `event:editMessage` | pass | pass | Live-confirmed on receiver-side: payload shape совпал, включая `player:null` |
| `event:deleteMessage` | pass | pass | Live-confirmed on receiver-side: payload shape совпал, включая `player:null` |
| `error:sendMessage` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:editMessage` | pass | pass | Live-confirmed on invalid `messageId`: обе стороны дают `channel_message_not_found` |
| `error:deleteMessage` | pass | pass | Live-confirmed on invalid `messageId`: обе стороны дают `channel_message_not_found` |
| `error:fetchMessages` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:fetchMoreMessages` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:fetchPersonalMessages` | pass | pass | Negative-case `playerId=999999999` не вызывает error; и sandbox, и Unity возвращают пустой `{ items: [], canLoadMore: false }` через success-path |
| `error:fetchMorePersonalMessages` | pass | pass | Negative-case `playerId=999999999` не вызывает error; и sandbox, и Unity возвращают пустой `{ items: [], canLoadMore: false }` через success-path |
| `error:fetchFeedMessages` | pass | pass | Negative-case `playerId=999999999` не вызывает error; и sandbox, и Unity возвращают пустой `{ items: [], canLoadMore: false }` через success-path |
| `error:fetchMoreFeedMessages` | pass | pass | Negative-case `playerId=999999999` не вызывает error; и sandbox, и Unity возвращают пустой `{ items: [], canLoadMore: false }` через success-path |

## Channels / Discovery

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `fetchChannel` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<FetchChannelData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `fetchPersonalChannel` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<FetchChannelData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `fetchFeedChannel` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<FetchChannelData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `fetchChannels` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<FetchChannelsResultData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `fetchMoreChannels` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<FetchChannelsResultData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `createChannel` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<CreateChannelData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `updateChannel` | mismatch | pass | Runtime payload совпал; public API в Unity уже возвращает `Task<UpdateChannelData>`, но остальной ecosystem вокруг метода всё ещё не буквально совпадает с эталоном |
| `deleteChannel` | mismatch | pass | Runtime payload совпал; public scalar overload убран, а public API в Unity уже async `Task`, но result shape всё ещё не буквальный эквивалент эталона |
| `event:updateChannel` | pass | pass | Runtime payload совпал, включая ACL flags `canSetVariable/canAddValue/canSubtractValue=false` |
| `event:deleteChannel` | pass | pass | Runtime payload совпал (`{ id }`) |
| `event:connect` | pass | unverified | Event name заведен |
| `error:fetchChannel` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:fetchPersonalChannel` | pass | pass | Live-confirmed on invalid `playerId=0`: обе стороны дают `player_not_found` |
| `error:fetchFeedChannel` | pass | pass | Live-confirmed on invalid `playerId=0`: обе стороны дают `player_not_found` |
| `error:fetchChannels` | pass | unverified | Event name заведен |
| `error:fetchMoreChannels` | pass | unverified | Event name заведен |
| `error:createChannel` | pass | pass | Live-confirmed on invalid `template=999999`: обе стороны дают `channel_template_not_found` |
| `error:updateChannel` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |
| `error:deleteChannel` | pass | pass | Live-confirmed on invalid `channelId=0`: обе стороны дают `empty_channel_id` |

## State

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `getLocalChannelState` | mismatch | pass | Runtime shape совпал на empty-state channel; public return в Unity уже typed и ближе к эталону: `ChannelLocalStateData` теперь имеет `getField/get/_set`, а bridge добавляет version keys в `state` как `${key}:ver` при сохранении legacy `versions`, но это всё ещё DTO, а не буквальный runtime класс web SDK |
| `getChannelField` | mismatch | pass | Runtime совпал на missing field (`null`); public return в Unity уже typed и ближе к эталону: `ChannelModelFieldData` теперь даёт alias `default`, но shape всё ещё DTO-based |
| `getChannelValue` | mismatch | pass | Runtime `undefined` на missing key теперь совпадает; public query-wrapper добавлен, но strict type parity всё ещё не `1:1` |
| `setValue` | mismatch | blocked | Public input в Unity уже ближе к эталону: query-object `{ channelId, key, value, version? }`, а public result теперь совпадает по форме с эталоном: async `Task<{ success, value }>` через `ChannelStateMutationResultData`. На project `4` templates `1/2` существуют, но возвращают `fields: []`; templates `3+` отсутствуют. Error-path совпал (`fields_not_found`), но success-path честно не проверить |
| `addValue` | mismatch | blocked | Public input в Unity уже ближе к эталону: query-object `{ channelId, key, value, version? }`, а public result теперь совпадает по форме с эталоном: async `Task<{ success, value }>` через `ChannelStateMutationResultData`. На project `4` templates `1/2` существуют, но возвращают `fields: []`; templates `3+` отсутствуют. Error-path совпал (`fields_not_found`), но success-path честно не проверить |
| `event:changeValue` | pass | unverified | Event name заведен |
| `setValue` event | pass | unverified | Success event не проверить без channel fields |
| `error:setValue` | pass | pass | Empty-field error-path совпал: Unity эмитит `fields_not_found` без extra browser alert |

## on/off Event Surface

| Item | Source parity | Live parity | Notes |
| --- | --- | --- | --- |
| `on(eventName, callback)` | mismatch | unverified | Public surface стал заметно ближе к эталону: even simple events слушаются через единый callback, Any-like payload в DTO больше не завязан на `JToken`, и добавлен generic `on<T>` для typed subscriptions. Оставшийся gap — всё ещё нет literal `EventEmitter` semantics web SDK |
| `off(eventName, callback)` | mismatch | unverified | Та же проблема, но теперь есть symmetric generic `off<T>` и examples уже переведены на единый data-path |
| generic `event` | pass | unverified | В Unity есть |

## Next Execution Order

1. Добить source-level `Channels` API gaps, которые не требуют backend:
   - продолжить вычищать оставшиеся public convenience-overloads
   - довести typed DTO/result shape ближе к literal web runtime
   - решить остаточный `Promise`/async mismatch

2. После этого прогнать live single-client matrix:
   - `openChat/openFeed`
   - `fetchChannels/fetchMoreChannels`
   - `fetchMessages/fetchMoreMessages`
   - `setValue/addValue`

3. Затем двухклиентные сценарии:
   - invites
   - join requests
   - realtime message / moderation events
