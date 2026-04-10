# Multiplayer Demo Handoff

## Scope

Фокус только на tester-facing demo экране для `Multiplayer` внутри Unity examples.

Речь не про дальнейшую parity-реализацию SDK, а про готовый экран в `GP_Examples`, который:

- открывается из `ExamplesScene`
- выглядит в стиле остальных demo-модулей
- вызывает методы `GP_Multiplayer`
- пишет результаты, события и ошибки в `ConsoleUI`

Другие demo-модули сейчас не трогаем без новых вводных.

## Goal

Собрать полноценный экран `MULTIPLAYER` в стиле текущих examples:

- кнопки под основные методы
- инпуты под аргументы
- логирование вызовов и событий
- usable для ручного тестирования без дополнительного кода

## Files Added

- `Demo/Assets/GP_Examples/Modules/Multiplayer/Multiplayer.cs`
- `Demo/Assets/GP_Examples/Editor/MultiplayerSceneBuilder.cs`
- `Demo/Assets/GP_Examples/Editor/ExamplesSceneTools.cs`
- `Demo/Assets/GP_Examples/Editor/GamePush.Examples.Editor.asmdef`

## Files Modified

- `Demo/Assets/GP_Examples/Scenes/ExamplesScene.unity`

## What Is Already Implemented

### Runtime script

`Demo/Assets/GP_Examples/Modules/Multiplayer/Multiplayer.cs`

Экран уже умеет:

- `connect`
- `disconnect`
- `setMode(Fast/Smooth)`
- `definePlayerSchema`
- `setPlayerInitializer` sync
- `setPlayerInitializer` async
- `disable initializer`
- `setPlayerState`
- `sendMessage`
- чтение:
  - `tickRate`
  - `isConnected`
  - `isHost`
  - `myState`
  - `playersState`
  - `connectedPlayers`
  - `networkStats`

Подписки на события уже добавлены:

- `connect`
- `disconnect`
- `error:connect`
- `error:disconnect`
- `error:sendState`
- `playerJoined`
- `playerLeft`
- `playersUpdated`
- `customEvent`
- `hostMigrated`
- `becameHost`
- `becamePeer`
- `onMessage`
- `onTick`

Логирование идёт через `ConsoleUI.Instance.Log(...)`.

### Scene builder

`Demo/Assets/GP_Examples/Editor/MultiplayerSceneBuilder.cs`

Builder делает следующее:

- открывает `ExamplesScene`
- дублирует template-модуль `Variables`
- дублирует кнопку `VARIABLES`
- переименовывает их в `Multiplayer` и `MULTIPLAYER`
- меняет target у menu button
- вешает `Examples.Multiplayer.Multiplayer`
- перестраивает содержимое экрана

### Editor-only assembly

`Demo/Assets/GP_Examples/Editor/GamePush.Examples.Editor.asmdef`

Добавлен потому, что без него editor-скрипты попадали в player build и ломали сборку на `UnityEditor`.

В него уже добавлена ссылка на:

- `GamePush.Examples`
- `Unity.TextMeshPro`

## Important Current Problem

Первый вариант builder-а собрал экран плохо.

Что пошло не так:

- для инпутов был взят слишком сложный template из существующего модуля
- вместе с ним в `Multiplayer` перетащились чужие внутренние элементы
- визуально это дало мусор:
  - `PLATFORM OPTIONS`
  - `KEY`
  - `VALUE`
  - наложение текста и полей

Поэтому builder был переписан ещё раз:

- теперь он больше не должен клонировать старый composite field-group целиком
- вместо этого он создаёт простую структуру `Label + TMP_InputField`

Но этот новый вариант ещё надо повторно прогнать в Unity и глазами проверить.

## Required Rebuild Step

После любых правок builder-а нужно:

1. дождаться перекомпиляции в Unity
2. запустить:
   - `GamePush -> Examples -> Build Multiplayer Scene`
3. заново открыть:
   - `Demo/Assets/GP_Examples/Scenes/ExamplesScene.unity`
4. только потом оценивать визуальный результат

Если этого не сделать, на сцене останется старая сломанная версия.

## Current Visual Requirements

Экран должен выглядеть как остальные demo-модули:

- читаемый заголовок `MULTIPLAYER`
- сетка кнопок без наложений
- чистые labels у полей
- никаких чужих текстов от других модулей
- `Console` снизу и не перекрывает форму
- `MAIN MENU` возвращает обратно

## Expected Inputs On Screen

Минимальный usable набор:

- `channelId`
- `schema json`
- `initializer json`
- `state json`
- `event name`
- `message json`
- `target`
- `echo`

## Expected Buttons On Screen

- `CONNECT`
- `DISCONNECT`
- `FAST`
- `SMOOTH`
- `DEFINE SCHEMA`
- `INIT SYNC`
- `INIT ASYNC`
- `CLEAR INIT`
- `SET STATE`
- `SEND MESSAGE`
- `READ TICK`
- `READ CONNECTED`
- `READ HOST`
- `READ MY STATE`
- `READ PLAYERS`
- `READ PEERS`
- `READ NETWORK`

## Known Worktree Noise

В worktree есть не только целевые изменения по `Multiplayer`, но и Unity noise.

Сейчас среди лишнего/побочного есть:

- `Demo/.vscode/*`
- `Demo/Packages/manifest.json`
- `Demo/ProjectSettings/*`
- `Demo/Demo.slnx`
- часть auto-changes в TMP/Graphics/UnityConnect assets

Новый агент не должен автоматически считать это частью целевой задачи.

## What To Touch Carefully

Целевые файлы по задаче:

- `Demo/Assets/GP_Examples/Modules/Multiplayer/Multiplayer.cs`
- `Demo/Assets/GP_Examples/Editor/MultiplayerSceneBuilder.cs`
- `Demo/Assets/GP_Examples/Editor/GamePush.Examples.Editor.asmdef`
- `Demo/Assets/GP_Examples/Scenes/ExamplesScene.unity`

Нежелательно без нужды трогать:

- другие demo-модули
- `Channels`
- общий SDK runtime
- unrelated `ProjectSettings`

## Recommended Next Step For Another Agent

1. Открыть проект в Unity.
2. Убедиться, что compile errors нет.
3. Запустить:
   - `GamePush -> Examples -> Build Multiplayer Scene`
4. Открыть `ExamplesScene`.
5. Визуально проверить `MULTIPLAYER`.
6. Если layout всё ещё плохой, править только builder, а не сцену вручную.
7. После того как экран станет визуально нормальным, прогнать functional smoke test:
   - `connect`
   - `disconnect`
   - `setMode`
   - `setPlayerInitializer`
   - `setPlayerState`
   - `sendMessage`
   - `read*`
   - события в `Console`

## Functional Smoke Test After Layout Fix

1. Открыть `MULTIPLAYER`.
2. Ввести `channelId`.
3. Нажать `CONNECT`.
4. Проверить лог:
   - `connect`
   - `playerJoined`
   - `tick`
5. Нажать:
   - `READ CONNECTED`
   - `READ HOST`
   - `READ MY STATE`
6. Нажать `DISCONNECT`.

Two-client smoke:

1. На host включить `INIT SYNC` или `INIT ASYNC`.
2. Подключить peer в тот же канал.
3. На peer нажать `SET STATE`.
4. На host проверить:
   - `playersUpdated`
   - `READ PLAYERS`

## Related Docs

- `output/multiplayer_parity_checklist.md`
- `output/channels_parity_checklist.md`
- `output/dod_status_summary.md`

## Commit Policy

По прямому указанию пользователя:

- ничего не коммитить и не пушить без отдельного запроса

