# GamePush Unity Parity Test Runbook

Этот документ нужен тестировщику, чтобы:

1. поднять локальный Unity WebGL build;
2. открыть sandbox reference;
3. прогнать `Channels` и `Multiplayer`;
4. сравнить поведение `sandbox == Unity`;
5. не тратить время на уже известные `blocked`-кейсы.

## Что именно проверяем

- `Channels` parity против reference sandbox.
- `Multiplayer` parity против reference sandbox и локального parity bridge.
- Совпадение:
  - публичных вызовов;
  - payload/result;
  - `error:*` событий;
  - realtime-событий.

Базовые артефакты для сверки:

- [DoD summary](output/dod_status_summary.md)
- [Channels checklist](output/channels_parity_checklist.md)
- [Multiplayer checklist](output/multiplayer_parity_checklist.md)

## Требования

- Unity `6000.0.24f1` или совместимая `6000.x`
- установлен модуль `WebGL Build Support`
- Node.js `18+`
- `npm`
- браузер с DevTools
- возможность открыть две отдельные сессии браузера
  - лучше обычное окно + incognito
  - или два разных профиля

## Что важно заранее

### 1. Локальный origin должен быть разрешён в GamePush project

Если тест идёт не на project `4`, у проекта должны быть разрешены:

- `http://localhost:8123`
- `http://127.0.0.1:8123`

Иначе `Multiplayer.connect(...)` может падать с `origin_not_allowed`.

### 2. Проект и token

Parity build умеет брать `projectId` и `publicToken` из query string.

Формат URL:

```text
http://localhost:8123/?projectId=4&publicToken=xT3RpsJMXpKWHPrTWkv3VBeHJKvCBccT
```

Если используется другой проект, просто замените оба значения.

### 3. Sandbox reference

Reference sandbox:

- [https://s3.gamepush.com/games/4/](https://s3.gamepush.com/games/4/)

Если нужно принудительно подставить проект в sandbox, в DevTools Console:

```js
sandbox.setProject(4, 'xT3RpsJMXpKWHPrTWkv3VBeHJKvCBccT')
```

## Быстрый старт

### Вариант A. Просто проверить текущий snapshot

Если вы не меняли JS bridge и просто хотите проверить текущее состояние репозитория:

1. Откройте Unity project из папки:
   - `<repo-root>/Demo`
2. Дождитесь импортов.
3. В Unity запустите:
   - `Tools -> GamePush -> Parity WebGL`
4. После сборки поднимите локальный сервер:

```bash
cd <repo-root>
npm run serve:webgl:demo
```

5. Откройте локальную страницу:

```text
http://localhost:8123/?projectId=4&publicToken=xT3RpsJMXpKWHPrTWkv3VBeHJKvCBccT
```

6. Откройте sandbox:
   - [https://s3.gamepush.com/games/4/](https://s3.gamepush.com/games/4/)

### Вариант B. Если перед тестом менялся JS bridge

Перед Unity build пересоберите bundle:

```bash
cd <repo-root>/NodeJs
npm install
npm run build
```

Потом снова выполните шаги из варианта A.

## CLI-сборка без Unity UI

Если удобнее собирать через terminal:

```bash
"<unity-editor-path>/Unity" \
  -batchmode \
  -quit \
  -projectPath "<repo-root>/Demo" \
  -executeMethod GamePush.BuildTools.GP_ParityBuild.CI_BuildParityWebGL \
  -logFile -
```

Где:

- `<repo-root>`: корень этого репозитория
- `<unity-editor-path>`: папка `.../Unity.app/Contents/MacOS`

Если у вас другой patch/minor релиз Unity `6000.x`, просто замените путь к editor binary.

## Где что лежит

- Unity project:
  - `Demo`
- parity build output:
  - `Demo/Build/WebGLParity`
- local WebGL launcher:
  - `scripts/serve-webgl.mjs`
- parity bridge:
  - `Demo/Assets/GP_Examples/Parity/GP_ParityBridge.cs`
- WebGL build script:
  - `Demo/Assets/Editor/GamePush/GP_ParityBuild.cs`

## Режимы проверки

Есть два практических режима:

1. `UI mode`
   - подходит в первую очередь для `Channels`
   - используется demo scene `ExamplesScene`
2. `Parity bridge mode`
   - нужен для точечных вызовов и для `Multiplayer`
   - вызывается из browser console через `unityInstance.SendMessage(...)`

## Почему используется именно этот локальный server

Unity build в этом репозитории использует Brotli-сжатые файлы (`*.br`).

Обычные статические серверы иногда отдают их без корректного:

- `Content-Encoding: br`
- `Content-Type`

В этом случае браузер пытается читать сжатый JS как обычный JavaScript и страница падает с:

```text
SyntaxError: Invalid or unexpected token
```

Скрипт `npm run serve:webgl:demo` уже учитывает это и раздаёт build правильно.

Если вместо Unity loader открывается `Index of /`, значит сервер поднят не из build root или build собран не полностью.

## UI Mode: как проверять Channels

После открытия локальной страницы:

1. Дождитесь загрузки `MAIN MENU`.
2. Используйте экраны:
   - `CHANNELS`
   - `CHATS`
3. Все действия логируются в demo console внутри страницы.
4. Те же действия повторяйте в sandbox и сравнивайте:
   - success/error
   - payload
   - realtime события

Минимальный smoke checklist:

1. `createChannel`
2. `fetchChannel`
3. `fetchChannels`
4. `sendMessage`
5. `fetchMessages`
6. `updateChannel`
7. `deleteChannel`
8. `openChat`
9. `openFeed`
10. invite / request / moderation flows

Подробный статус уже зафиксирован в:

- [channels_parity_checklist.md](output/channels_parity_checklist.md)

## UI Mode: как проверять Multiplayer Demo

`Multiplayer` сейчас тестируется не через auto-builder, а через вручную сохранённую сцену.

Что это значит:

- источник истины для layout:
  - `Demo/Assets/GP_Examples/Scenes/ExamplesScene.unity`
- пункт меню `GamePush -> Examples -> Build Multiplayer Scene` отключён намеренно
- тестировщику не нужно и нельзя пытаться пересобирать layout этого экрана

### Где открыть экран

1. Откройте локальный WebGL build.
2. На `MAIN MENU` выберите:
   - `MULTIPLAYER`

### Как сгруппирован экран

Экран разбит на смысловые блоки.

#### 1. Session

Используется для подключения и режима тиков:

- `CHANNEL ID`
- `CONNECT`
- `DISCONNECT`
- `FAST`
- `SMOOTH`

Связи:

- `CONNECT` использует `CHANNEL ID`
- `DISCONNECT` использует `CHANNEL ID`
- `FAST` и `SMOOTH` не используют инпуты

#### 2. Schema & Initializer

Используется для подготовки host-side state sync:

- `SCHEMA JSON`
- `DEFINE SCHEMA`
- `INITIALIZER JSON`
- `INIT SYNC`
- `INIT ASYNC`
- `CLEAR INIT`

Связи:

- `DEFINE SCHEMA` использует `SCHEMA JSON`
- `INIT SYNC` и `INIT ASYNC` используют `INITIALIZER JSON`
- `CLEAR INIT` просто сбрасывает initializer

#### 3. Player State

- `STATE JSON`
- `SET STATE`

Связи:

- `SET STATE` использует `STATE JSON`

#### 4. Messaging

- `EVENT NAME`
- `MESSAGE JSON`
- `TARGET`
- `ECHO`
- `SEND MESSAGE`
- `OFF MESSAGE`

Связи:

- `SEND MESSAGE` использует все 4 поля
- `OFF MESSAGE` не использует input fields и отключает только `onMessage`
- `OFF MESSAGE` не отключает `customEvent`, это ожидаемое поведение

#### 5. Runtime Info

Readonly-кнопки:

- `READ TICK`
- `OFF TICK`
- `READ CONNECTED`
- `READ HOST`
- `READ MY STATE`
- `READ PLAYERS`
- `READ PEERS`
- `READ NETWORK`

Они не используют input fields и только читают текущее состояние SDK.

#### 6. Console

Нижний логовый блок.

В него пишутся:

- результаты методов
- ошибки
- realtime-события
- `onMessage`
- `onTick`
- `error:connect`
- `error:disconnect`

### Что проверить глазами

Перед функциональным тестом убедитесь:

- блоки визуально разделены и не наезжают друг на друга
- кнопки находятся внутри своих карточек
- нижний `Console` не перекрывает карточки
- `MAIN MENU` остаётся поверх экрана и работает

### Multiplayer UI smoke test

#### Сценарий 1. Один клиент

1. Введите `CHANNEL ID`
2. Нажмите `CONNECT`
3. Проверьте лог:
   - `MULTIPLAYER: CONNECT`
   - либо `connect`, либо `error:connect`
4. Нажмите:
   - `READ CONNECTED`
   - `READ HOST`
   - `READ TICK`
5. Нажмите `FAST`, потом `READ TICK`
6. Нажмите `SMOOTH`, потом `READ TICK`
7. Нажмите `DISCONNECT`

#### Сценарий 2. OFF MESSAGE / OFF TICK

1. Подключитесь к реальному `channelId`
2. Для проверки `OFF MESSAGE`:
   - либо включите `ECHO=true` и отправляйте сообщение самому себе
   - либо используйте второй клиент
3. До нажатия `OFF MESSAGE` убедитесь, что в консоли появляется:
   - `MULTIPLAYER: EVENT onMessage`
4. Нажмите `OFF MESSAGE`
5. Отправьте сообщение повторно
6. Ожидается:
   - `MULTIPLAYER: EVENT onMessage` больше не появляется
   - `MULTIPLAYER: EVENT customEvent` может продолжать появляться, это нормально
   - в момент клика логируется `MULTIPLAYER: OFF MESSAGE: listener disabled; customEvent stays active`
7. Для проверки `OFF TICK` дождитесь логов `tick`
8. Нажмите `OFF TICK`
9. Ожидается:
   - новые `MULTIPLAYER: EVENT tick` перестают приходить
   - повторный клик пишет `MULTIPLAYER: OFF TICK: already disabled`

#### Сценарий 3. Host + Peer

Нужны две сессии браузера:

- `Host`
- `Peer`

Порядок:

1. Через `Channels` создайте или используйте готовый `channelId`
2. Убедитесь, что `Peer` состоит в этом канале
3. На `Host`:
   - `INIT SYNC` или `INIT ASYNC`
4. На `Host` и `Peer`:
   - `CONNECT`
5. На `Peer`:
   - `SET STATE`
6. На `Host`:
   - `READ PLAYERS`

Ожидается:

- `Host` получает `playersUpdated`
- `READ PLAYERS` показывает state peer

Важно:

- без `INIT SYNC` или `INIT ASYNC` host-side sync может не появиться

#### Сценарий 4. error:disconnect

Цель: убедиться, что demo подписан на `error:disconnect` и выводит его в `Console`.

Шаги:

1. Откройте `MULTIPLAYER`
2. Введите `CHANNEL ID = 0`
3. Нажмите `DISCONNECT`

Ожидается:

- появляется `MULTIPLAYER: DISCONNECT: channelId=0`
- появляется `MULTIPLAYER: EVENT error:disconnect`
- payload ошибки содержит код из runtime, например `empty_channel_id`

Допустимо также:

- помимо realtime-события может появиться `MULTIPLAYER: DISCONNECT EXCEPTION`
- это не отдельный баг: demo одновременно логирует event `error:disconnect` и exception из отклонённого `await disconnect(...)`

Важно:

- пустой или нечисловой `CHANNEL ID` не подходит для этого теста
- в таком случае demo остановится раньше и напишет только `MULTIPLAYER: DISCONNECT: invalid channelId`
- это expected behavior, а не новый баг

#### Сценарий 5. Messaging

1. На обоих клиентах выполнить `CONNECT`
2. На одном клиенте заполнить:
   - `EVENT NAME`
   - `MESSAGE JSON`
   - `TARGET`
   - `ECHO`
3. Нажать `SEND MESSAGE`
4. Проверить:
   - `customEvent`
   - `onMessage`
   - payload в `Console`

### Что НЕ делать тестировщику

- не запускать `GamePush -> Examples -> Build Multiplayer Scene`
- не перестраивать `Multiplayer` layout через editor-builder
- не считать отсутствие `peer -> host` sync без initializer новым багом

## Parity Bridge Mode: подготовка console helper'ов

Откройте DevTools Console на локальной Unity WebGL странице и вставьте:

```js
window.parityAll = () => (window.GPParity?.events ?? []).map((x) => JSON.parse(x));

window.parityByRequest = (requestId) =>
  parityAll().filter((x) => x.requestId === requestId);

window.parityLast = () => parityAll().at(-1);

window.paritySend = (module, member, ...args) => {
  const requestId = `${module}:${member}:${Date.now()}:${Math.random().toString(16).slice(2)}`;
  window.unityInstance.SendMessage(
    'GP_ParityBridge',
    'RunCommand',
    JSON.stringify({ requestId, module, member, args })
  );
  return requestId;
};

window.parityGet = (module, member) => {
  const requestId = `${module}:${member}:${Date.now()}:${Math.random().toString(16).slice(2)}`;
  window.unityInstance.SendMessage(
    'GP_ParityBridge',
    'RunCommand',
    JSON.stringify({ requestId, module, member, kind: 'get', args: [] })
  );
  return requestId;
};

window.parityPing = (requestId = `ping:${Date.now()}`) => {
  window.unityInstance.SendMessage('GP_ParityBridge', 'Ping', requestId);
  return requestId;
};
```

Проверка, что bridge жив:

```js
const req = parityPing();
setTimeout(() => parityByRequest(req), 300);
```

Ожидается событие вида:

```json
{ "kind": "pong", "module": "system", "member": "ping", "payload": "ok" }
```

## Формат parity-команд

`RunCommand` принимает JSON:

```json
{
  "requestId": "optional-id",
  "module": "channels | multiplayer | parity",
  "member": "methodOrPropertyName",
  "kind": "get",
  "args": []
}
```

Правила:

- для методов используйте `args`
- для properties используйте `kind: "get"`
- результаты и события прилетают в `window.GPParity.events`

## Примеры parity-команд

### Channels

Получить property:

```js
const req = parityGet('channels', 'canBeOnline');
setTimeout(() => parityByRequest(req), 300);
```

Создать канал:

```js
const req = paritySend(
  'channels',
  'createChannel',
  {
    title: 'Parity Room',
    template: '1',
    private: false,
    visible: true,
    tags: [],
    messageTags: []
  }
);
```

Получить канал:

```js
const req = paritySend('channels', 'fetchChannel', { channelId: 12345 });
```

Ошибка на невалидном канале:

```js
const req = paritySend('channels', 'fetchChannel', { channelId: 0 });
```

### Multiplayer

Получить property:

```js
const req = parityGet('multiplayer', 'isConnected');
```

Включить default initializer на host:

```js
const req = paritySend('parity', 'enableDefaultPlayerInitializer');
```

Подключиться:

```js
const req = paritySend('multiplayer', 'connect', { channelId: 12345 });
```

Отключиться:

```js
const req = paritySend('multiplayer', 'disconnect', { channelId: 12345 });
```

Установить state:

```js
const req = paritySend('multiplayer', 'setPlayerState', { score: 1, ready: true });
```

Отправить custom event:

```js
const req = paritySend(
  'multiplayer',
  'sendMessage',
  'demo-event',
  { foo: 'bar' },
  { target: 'all', echo: true }
);
```

Включить probes:

```js
paritySend('parity', 'resetProbes');
paritySend('parity', 'enableMessageProbe');
paritySend('parity', 'enableTickProbe');
```

Снять probe state:

```js
const req = paritySend('parity', 'getProbeState');
```

## Как проверять Multiplayer вручную

### Рекомендуемый сценарий

Нужны две отдельные браузерные сессии:

- `Host`
- `Peer`

Обе открывают один и тот же локальный URL с одним и тем же `projectId/publicToken`.

### Сценарий 1. connect / disconnect

1. На `Host` создать канал через `Channels` UI или через `paritySend('channels', 'createChannel', ...)`
2. На `Peer` войти в канал:
   - `paritySend('channels', 'join', { channelId })`
3. На `Host`:
   - `paritySend('multiplayer', 'connect', { channelId })`
4. На `Peer`:
   - `paritySend('multiplayer', 'connect', { channelId })`
5. Сравнить:
   - `connect`
   - `playerJoined`
   - `connectedPlayers`
   - `isHost`
6. Выполнить `disconnect` на одной стороне и проверить:
   - `disconnect`
   - `playerLeft`
   - `playersUpdated`

### Сценарий 2. peer -> host state sync

Важно: перед проверкой на host обязательно включить initializer:

```js
paritySend('parity', 'enableDefaultPlayerInitializer');
```

Потом:

1. `Host` подключается к каналу
2. `Peer` подключается к тому же каналу
3. `Peer` вызывает:

```js
paritySend('multiplayer', 'setPlayerState', { score: 10, ready: true });
```

4. На `Host` проверить:
   - событие `playersUpdated`
   - property `playersState`

Если initializer не включён, `peer -> host` sync может не появиться. Это не баг Unity wrapper, а подтверждённое поведение reference core.

### Сценарий 3. sendMessage / onMessage / onTick

1. После `connect` включить probes:

```js
paritySend('parity', 'resetProbes');
paritySend('parity', 'enableMessageProbe');
paritySend('parity', 'enableTickProbe');
```

2. Отправить сообщение:

```js
paritySend(
  'multiplayer',
  'sendMessage',
  'smoke',
  { ok: true },
  { target: 'all', echo: true }
);
```

3. Проверить `getProbeState`

Ожидается:

- `messageProbeHits > 0`
- `tickProbeHits > 0`

## Как сравнивать с sandbox

Используйте тот же сценарий на reference sandbox:

- [https://s3.gamepush.com/games/4/](https://s3.gamepush.com/games/4/)

Сравнивать нужно:

- был ли success / error
- точный код ошибки
- какие realtime-события пришли
- порядок событий
- форму payload

Если поведение отличается, результат нужно сверять с готовыми чеклистами:

- [channels_parity_checklist.md](output/channels_parity_checklist.md)
- [multiplayer_parity_checklist.md](output/multiplayer_parity_checklist.md)

## Что сейчас уже НЕ считать новым багом

### Channels

- `setValue/addValue` success-path на project `4`
  - blocked
  - у доступных templates нет `fields`
- `closeChat`
  - blocked/manual overlay case

### Multiplayer

- `peer -> host setPlayerState` без initializer
  - expected behavior
  - host-side sync требует `setPlayerInitializer(...)`
- `becamePeer`
  - пока blocked / live-unverified
  - не удалось стабильно воспроизвести real host migration

## Что прикладывать к баг-репорту

Минимум:

1. шаги воспроизведения
2. локальный URL
3. sandbox URL
4. `projectId`
5. `channelId`, если кейс связан с конкретным каналом
6. console output локального build
7. console output sandbox
8. если использовался parity bridge:
   - сам вызов
   - `requestId`
   - результат `parityByRequest(requestId)`

## Рекомендуемый порядок прогона

1. `Channels` happy-path
2. `Channels error:*`
3. `Channels realtime`
4. `Multiplayer connect/disconnect`
5. `Multiplayer sendMessage`
6. `Multiplayer peer state sync with initializer`
7. только потом blocked/advanced cases

## Финальная сверка

После прогона обязательно сопоставьте результат с:

- [DoD summary](output/dod_status_summary.md)
- [Channels checklist](output/channels_parity_checklist.md)
- [Multiplayer checklist](output/multiplayer_parity_checklist.md)

Если кейс уже отмечен там как `pass`, а у тестировщика он ломается, это уже хороший кандидат на regression.
