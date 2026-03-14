<div align="center">

[🇬🇧 English](#lone-brawler--english) &nbsp;|&nbsp; [🇷🇺 Русский](#lone-brawler--русский)

</div>

---

# Lone Brawler &nbsp;·&nbsp; English

> A 3D action game built in Unity. The codebase covers the full production pipeline: gameplay systems, infrastructure, UI, data management, editor tooling, and an in-house math library - 442 C# files.

<br>

## Tech Stack

| | |
|---|---|
| **Engine** | Unity (URP, Addressables, Input System, NavMesh) |
| **Language** | C# 9 |
| **DI** | Zenjex (custom, [open-source](https://github.com/antonprv/Zenjex)) |
| **Async** | UniTask + R3 (Reactive Extensions) |
| **Animation** | LeanTween |
| **Math** | FMath (custom) + Unity.Mathematics + Burst |
| **Serialisation** | Newtonsoft Json.NET |
| **Testing** | NUnit + NSubstitute + Unity Performance Testing |
| **Platform** | WebGL, Android |
| **SDKs** | Yandex Games SDK (ads, IAP, cloud saves, auth) · Yandex Ads SDK (cross-store monetisation) |
| **Input** | Unity Input System (code-generated bindings) |

---

## Table of Contents

- [Architecture](#architecture)
- [Platform & SDKs](#platform--sdks)
- [Gameplay Systems](#gameplay-systems)
- [Data Management & Save System](#data-management--save-system)
- [Audio System](#audio-system)
- [UI & Frontend](#ui--frontend)
- [Editor Tooling](#editor-tooling)
- [Custom Libraries & Extensions](#custom-libraries--extensions)
- [Testing](#testing)

---

## Architecture

### Game State Machine

The application lifecycle is driven by a generic, payload-capable state machine. Each state is a self-contained class resolved through a `StateFactory`, which keeps the machine itself lean and focused on transitions only.

```
BootstrapperState → LoadProgressState → MainMenuState → LoadLevelState → GameLoopState
```

States support two signatures - `Enter()` for simple transitions and `Enter<TPayload>(payload)` for cases that require data (for example, passing a level key to `LoadLevelState`).

<!-- 📸 Screenshot: State machine flow diagram or GameInstance inspector -->

---

### Dependency Injection - Zenjex

**Zenjex** is a custom DI framework authored for this project and released as a [standalone open-source library](https://github.com/antonprv/Zenjex). It wraps the **Reflex** container with a Zenject-inspired fluent API and Unity-specific injection mechanics.

Dependencies are declared with `[Zenjex]` on private readonly fields - no constructor boilerplate, no manual `Inject()` calls:

```csharp
[Zenjex] private readonly ISaveLoadService _saveLoad;
[Zenjex] private readonly IStaticDataService _staticData;
```

Injection runs in three passes (`OnContainerReady` at execution order -280, `OnGameLaunched` after async setup, `sceneLoaded` for additive scenes). `ZenjexRunner` deduplicates by `GetInstanceID()` so nothing is processed twice. `ZenjexBehaviour` is the strictest path - fields are filled before `OnAwake()` runs. `IInitializable` provides a Zenject-compatible post-injection entry point.

The fluent binding API covers singletons, transients, scoped, prefab instantiation, `WithArguments`, `BindInterfacesAndSelf`, `CopyIntoDirectSubContainers`, and eager vs. lazy resolution. `ZenjexSceneContext` exposes per-scene sub-containers to global services without MonoBehaviour coupling. A built-in editor debugger shows all injection records per pass and surfaces `ZNX-LATE` warnings.

<!-- 📸 Screenshot: Zenjex debugger window or [Zenjex] attribute on a field in inspector -->

---

### Addressables & Asset Management

`AssetLoader` wraps Unity Addressables with a handle-caching layer - a completed handle is stored by GUID on first load and reused on subsequent requests. Instantiated objects are tracked separately; `Cleanup()` releases all handles in one pass at scene teardown.

<!-- 📸 Screenshot: Addressables groups window -->

---

## Platform & SDKs

The game targets **WebGL** (Yandex Games) and **Android** (Google Play, RuStore, and other Android storefronts), with two Yandex SDKs integrated to cover both distribution channels.

### Yandex Games SDK

Handles the full Yandex Games platform stack: interstitial and rewarded ads, in-app purchases with server-side consumption, and **cloud saves** that sync `GameProgress` across devices. When a saved game is found in the cloud, it takes precedence over the local `PlayerPrefs` copy.

### Authentication & Player Profile

The main menu has a **Login** button that initiates Yandex OAuth. On a successful response the game fetches the player's profile from the Yandex server and displays their **name and avatar** in the UI. Unauthenticated sessions fall back gracefully to guest mode with local saves only.

<!-- 📸 Screenshot: Login button in main menu and player avatar/name displayed after auth -->

### Yandex Ads SDK

For builds distributed outside the Yandex Games ecosystem (Google Play, RuStore, etc.), the Yandex Ads SDK is used to serve banner, interstitial, and rewarded ads. The active SDK is toggled at build time via the `UseAddSdk` flag in `GameBuildData`, so the same codebase produces clean platform-specific builds without conditional compilation scattered across gameplay code.

<!-- 📸 Screenshot: Rewarded ad flow or build config toggling SDK flags -->

---

## Gameplay Systems

### Player

The player character is assembled from discrete, interface-driven components: `PlayerMove`, `PlayerHealth`, `PlayerDeath`, `PlayerAnimator`, `PlayerBuffConsumer`. Each component reads its initial values from `PlayerStats` stored in `GameProgress`, keeping the save system and gameplay layer cleanly separated.

<!-- 📸 Screenshot: Player in-game or player component hierarchy in inspector -->

---

### Enemy AI

`AggroSystem` tracks proximity and transitions the enemy into an attack-ready state. `EnemyMovement` drives NavMesh navigation once aggro fires. Attack behaviour is decoupled behind `IAttackBehaviour` and supports melee and projectile variants, configured per-enemy via `EnemyStaticData` ScriptableObjects. Object pooling covers both projectiles and VFX.

<!-- 📸 Screenshot: Enemy aggro radius gizmo in scene view or enemy attack in action -->

---

### Buff System

Buffs are plain C# classes inheriting `BuffBase`. Three activation modes: **Burst** (one-shot), **Constant** (permanent stat delta, survives saves), **Duration** (coroutine tick, tears down automatically). `BuffTrackerService` serialises live buff states - including remaining duration - on save and restores them on load. Visual effects are loaded via Addressables inside `SpawnEffectAsync()` with stacking prevention.

<!-- 📸 Screenshot: Buff icons in hotbar or buff VFX on player character -->

---

### Inventory & Hotbar

Two flat slot lists (main inventory + hotbar) serialised as part of `GameProgress`. `InventorySlotView` implements the full drag-and-drop event chain and renders icons asynchronously from Addressables. The hotbar maps numeric keys to `TryUseBuff()` each frame via `IInputService`.

<!-- 📸 Screenshot: Inventory window with drag-and-drop in action -->

---

### Souls System & Level Teleportation

`SoulsTrackerService` exposes the counter as `ReadOnlyReactiveProperty<int>` for zero-polling UI subscriptions. `TrySpendSouls` is an atomic check-and-deduct.

`LevelTeleportTrigger` stamps the progress object with a teleport name and UTC timestamp on contact, forces a save, then hands off to `LoadLevelState`. The timestamp lets the game reconstruct the exact spawn point on the next scene load.

<!-- 📸 Screenshot: Souls counter in HUD and teleport trigger in scene view -->

---

## Data Management & Save System

### GameProgress

All runtime mutable state lives in one serialisable root object:

| Domain | Contents |
|---|---|
| Player | `PLayerState` (health), `PlayerStats` (speed, damage, range) |
| World | `WorldData` (last transform, teleport name + UTC time) |
| Enemies | `EnemiesKilled` (cleared spawner IDs as `HashSetData<string>`) |
| Economy | `SoulsCollected` (amount + spawner positions) |
| Buffs | `BuffsRegistry` (class, activation type, state, remaining duration) |
| Inventory | `InventorySaveData` (slot arrays with buff class + count) |

Each domain section exposes `IsValid()` to reject corrupted or default-constructed data.

<!-- 📸 Screenshot: Save data in browser dev tools -->

---

### Live Progress Sync & Cloud Saves

`LiveProgressSync` autosaves every 5 seconds and responds to the Yandex Games `OnQuitGame` event for a synchronous final save on tab close. When Yandex cloud saves are enabled, `GameProgress` is additionally pushed to the Yandex cloud and pulled on next session start; the cloud copy wins over local `PlayerPrefs` on conflict.

<!-- 📸 Screenshot: Console log of autosave events -->

---

### Serialisable Collections

`DictionaryData<TKey, TValue>` and `HashSetData<T>` are custom drop-in replacements for Unity's unserialisable `Dictionary` and `HashSet`. Both implement `ISerializationCallbackReceiver` to keep backing lists in sync and ship `PropertyDrawer` implementations for correct Inspector rendering.

<!-- 📸 Screenshot: DictionaryData displayed in inspector -->

---

## Audio System

### Music Player

`MusicPlayer` is a fully async multi-track system built around two `AudioSource` slots that swap on every crossfade. Three paths chosen automatically: empty playlist (no-op), single track (native `AudioSource.loop`, no crossfade overhead), multi-track (next clip pre-loaded via Addressables during playback). Fisher-Yates shuffle per loop cycle. Fade and crossfade durations configured via `MusicPlayerConfig`. Volume reacts to `ISoundService` reactive properties in real time.

<!-- 📸 Screenshot: MusicPlayer component in inspector -->

---

### Sound Effects

`SoundList` maps `SoundType` enum keys to `AudioClipGroup` arrays in `DictionaryData`. A random clip is picked per playback via `IRandomService`. `MenuButtonSound` adds hover/click sounds to UI buttons via R3 observables with debounce guards against stacking.

<!-- 📸 Screenshot: SoundList component in inspector -->

---

## UI & Frontend

### Window System & Dev Console

Windows are loaded on demand via Addressables through `WindowService` + `IUIFactory`. The developer console follows a layered **MVVM** split - `ConsoleState` / `CommandHistory` / `MobileKeyboard` as Model, `ConsoleViewModel` as ViewModel, `ConsoleRenderer` + `ConsoleStyles` + `ScrollDragHandler` as View (IMGUI, touch-scroll on mobile). Platform detection via `PlatformService` adapts layout per device.

Available commands: `clear`, `help`, `filter`, `toggle_unity_logs`, `export_logs`, `log_stats`, `set_fps`, `stat_fps`, `add_souls`, `load_level`, `quit_to_menu`, `pause_game`, `reset_game`, `warp_player`.

<!-- 📸 Screenshot: Dev console open in-game -->

---

### Inventory Drag & Drop, Health Bars, Popups

`InventorySlotView` handles the full EventSystem interface chain, async Addressables icon loading, tooltip via `ITooltipProvider`, and colour-animated selection. `DragDropService` + `DragIconProvider` manage the floating drag icon and slot transfer context. World-space `HealthBar` components billboard toward the camera; `TextPopup` renders floating damage numbers. `HideButtonsOnPC` removes mobile overlays on desktop.

<!-- 📸 Screenshot: Drag-and-drop between inventory slots and enemy health bar -->

---

## Editor Tooling

- **Scene Switcher Overlay** - `[Overlay]`-registered SceneView toolbar dropdown for instant scene switching.
- **Quick Look** - dockable `EditorWindow` with a drag-and-drop shortlist of prefabs and ScriptableObjects, persisted to a ScriptableObject asset, auto-reflowing button grid.
- **Level Static Data Editor** - one-click **Collect All Data** button scans the active scene and auto-populates enemy spawners, teleports, and player start position.
- **Scene Data Selector** - toolbar button that jumps the Inspector to the StaticData asset for the currently open scene.
- **Manifests & Typed Dropdowns** - `ManifestEditorBase` + `SceneDropdownKeyDrawer` / `EnumDropdownKeyDrawer` replace raw string keys with validated dropdowns across all manifest ScriptableObjects.
- **Build Configuration** - `GameBuildData` ScriptableObject centralises `DebugConfiguration`, `TargetPlatform`, cloud save toggle, and ad SDK toggle. `FilteredEnumAttribute` removes the `None` sentinel from enum dropdowns.
- **Naming Convention Doc** - ScriptableObject-based style guide co-located with the project, documenting prefab prefixes (P\_, PA\_, PP\_, PAI\_, PUI\_) and UI element suffixes (\_CNT, \_BTN, \_BG, \_TXT, \_IMG).

<!-- 📸 Screenshot: Scene Switcher overlay, Quick Look window, LevelStaticData inspector with Collect button -->

---

## Custom Libraries & Extensions

- **FastMath** - two backends: `FMath` (managed) implements the Quake III Fast Inverse Square Root (`0x5f3759df` + Newton-Raphson), `FastSqrt`, `FastNormalize`, `FastLength`, `FastDistance` via unsafe pointer casts; `BurstMath` wraps `Unity.Mathematics` with `[BurstCompile]` + `[AggressiveInlining]` for hot paths.
- **Serialisable Vector Types** - `Vector3Data`, `QuatData`, `TransformData`, `Coordinates` with JSON support, `PropertyDrawer` implementations, and Unity conversion extensions.
- **UniTask Extensions** - `GetAwaiter()` overloads for `UniTask?` / `UniTask<T>?` enabling safe null-conditional `await`.
- **Functional Extensions** - `With<T>` overloads for fluent object initialisation with optional conditional application.
- **GameLogger** - structured logger prepending `[ClassName.MethodName]` via `StackFrame` reflection; compiled out entirely in Shipping builds.

<!-- 📸 Screenshot: Performance test results -->

---

## Testing

**50 test files** across two assemblies.

**Edit Mode** covers: SaveData (GameProgress, PlayerStats, BuffsRegistry, and edge cases), Inventory, custom collections, vector types, FastMath, the full DevConsole command set, and core services.

**Play Mode** includes smoke tests (all critical MonoBehaviours attach without exceptions via `ZenjexTestBootstrap`), integration tests (aggro, health, death, save triggers), and performance benchmarks (TakeDamage at 100 calls/measurement, frame-time stability) using Unity Performance Testing Package.

<!-- 📸 Screenshot: Test Runner with all tests passing -->

---
---

<div align="center">

[🇬🇧 English](#lone-brawler--english) &nbsp;|&nbsp; [🇷🇺 Русский](#lone-brawler--русский)

</div>

---

# Lone Brawler &nbsp;·&nbsp; Русский

> 3D экшн на Unity. Кодовая база покрывает полный производственный цикл: геймплейные системы, инфраструктура, UI, управление данными, редакторный инструментарий и собственная математическая библиотека - 442 файла на C#.

<br>

## Стек технологий

| | |
|---|---|
| **Движок** | Unity (URP, Addressables, Input System, NavMesh) |
| **Язык** | C# 9 |
| **DI** | Zenjex (собственный, [open-source](https://github.com/antonprv/Zenjex)) |
| **Async** | UniTask + R3 (Reactive Extensions) |
| **Анимации** | LeanTween |
| **Математика** | FMath (собственная) + Unity.Mathematics + Burst |
| **Сериализация** | Newtonsoft Json.NET |
| **Тестирование** | NUnit + NSubstitute + Unity Performance Testing |
| **Платформа** | WebGL, Android |
| **SDK** | Yandex Games SDK (реклама, IAP, облачные сохранения, авторизация) · Yandex Ads SDK (монетизация вне экосистемы Яндекса) |
| **Ввод** | Unity Input System (кодогенерированные биндинги) |

---

## Содержание

- [Архитектура](#архитектура)
- [Платформа и SDK](#платформа-и-sdk)
- [Геймплейные системы](#геймплейные-системы)
- [Данные и система сохранений](#данные-и-система-сохранений)
- [Аудиосистема](#аудиосистема)
- [UI и фронтенд](#ui-и-фронтенд)
- [Редакторный инструментарий](#редакторный-инструментарий)
- [Собственные библиотеки и расширения](#собственные-библиотеки-и-расширения)
- [Тестирование](#тестирование)

---

## Архитектура

### Game State Machine

Жизненный цикл приложения управляется generic-машиной состояний с поддержкой payload. Каждое состояние - самодостаточный класс, создаваемый через `StateFactory`.

```
BootstrapperState → LoadProgressState → MainMenuState → LoadLevelState → GameLoopState
```

Поддерживаются две сигнатуры входа: `Enter()` для простых переходов и `Enter<TPayload>(payload)` для передачи данных (например, ключа уровня в `LoadLevelState`).

<!-- 📸 Скриншот: диаграмма переходов или инспектор GameInstance -->

---

### Dependency Injection - Zenjex

**Zenjex** - собственный DI-фреймворк, написанный для этого проекта и выпущенный как [самостоятельная open-source библиотека](https://github.com/antonprv/Zenjex). Оборачивает контейнер **Reflex** Zenject-подобным fluent API и Unity-специфичными механиками инъекции.

Зависимости объявляются атрибутом `[Zenjex]` на приватных readonly-полях - никакого конструкторного бойлерплейта:

```csharp
[Zenjex] private readonly ISaveLoadService _saveLoad;
[Zenjex] private readonly IStaticDataService _staticData;
```

Инъекция выполняется в три прохода (`OnContainerReady` с порядком выполнения -280, `OnGameLaunched` после асинхронной настройки, `sceneLoaded` для аддитивных сцен). `ZenjexRunner` дедуплицирует по `GetInstanceID()`. `ZenjexBehaviour` - строжайший путь с гарантией заполнения полей до `OnAwake()`. `IInitializable` - Zenject-совместимый хук после инъекции.

Fluent binding API покрывает синглтоны, транзиенты, scoped, инстанцирование из префабов, `WithArguments`, `BindInterfacesAndSelf`, `CopyIntoDirectSubContainers`, eager/lazy resolution. `ZenjexSceneContext` предоставляет per-scene sub-контейнеры глобальным сервисам без MonoBehaviour-связанности. Встроенный редакторный дебаггер показывает все записи инъекций по проходам и `ZNX-LATE`-предупреждения.

<!-- 📸 Скриншот: окно Zenjex Debugger или атрибут [Zenjex] на поле в инспекторе -->

---

### Addressables и управление ассетами

`AssetLoader` оборачивает Unity Addressables слоем кеширования хэндлов - завершённый хэндл сохраняется по GUID при первой загрузке и переиспользуется в последующих запросах. `Cleanup()` освобождает все хэндлы за один проход при выгрузке сцены.

<!-- 📸 Скриншот: окно Addressables Groups -->

---

## Платформа и SDK

Игра выходит на **WebGL** (Яндекс Игры) и **Android** (Google Play, RuStore и другие Android-сторы). Для покрытия обоих каналов дистрибуции интегрированы два SDK от Яндекса.

### Yandex Games SDK

Закрывает полный стек платформы Яндекс Игр: показ рекламы (interstitial и rewarded), внутриигровые покупки с серверным консумированием, и **облачные сохранения**, которые синхронизируют `GameProgress` между устройствами. При наличии облачного сейва он имеет приоритет над локальным `PlayerPrefs`.

### Авторизация и профиль игрока

В главном меню есть кнопка **Login**, запускающая Yandex OAuth. При успешном ответе игра выполняет запрос к серверу Яндекса и отображает **имя и аватар** игрока в UI. Неавторизованные сессии корректно откатываются в гостевой режим с локальными сохранениями.

<!-- 📸 Скриншот: кнопка Login в главном меню и имя/аватар игрока после авторизации -->

### Yandex Ads SDK

Для сборок, распространяемых вне экосистемы Яндекс Игр (Google Play, RuStore и т.д.), монетизация реализована через Yandex Ads SDK - баннеры, interstitial и rewarded реклама. Активный SDK переключается на этапе сборки флагом `UseAddSdk` в `GameBuildData`, поэтому одна кодовая база даёт чистые платформоспецифичные билды без условной компиляции в геймплейном коде.

<!-- 📸 Скриншот: флоу rewarded-рекламы или конфиг сборки с переключателями SDK -->

---

## Геймплейные системы

### Персонаж игрока

Персонаж собран из дискретных, интерфейс-ориентированных компонентов: `PlayerMove`, `PlayerHealth`, `PlayerDeath`, `PlayerAnimator`, `PlayerBuffConsumer`. Каждый компонент читает начальные значения из `PlayerStats` в `GameProgress`, обеспечивая чистое разделение между системой сохранений и геймплейным слоем.

<!-- 📸 Скриншот: персонаж в игре или иерархия компонентов в инспекторе -->

---

### ИИ противников

`AggroSystem` отслеживает близость и переводит врага в боеготовное состояние. `EnemyMovement` запускает NavMesh-навигацию после срабатывания аггро. Поведение атаки вынесено за интерфейс `IAttackBehaviour` и поддерживает ближний бой и снаряды, конфигурируемые через `EnemyStaticData`. Пул объектов покрывает снаряды и VFX.

<!-- 📸 Скриншот: гизмо радиуса аггро или атака врага в динамике -->

---

### Система баффов

Баффы - чистые C#-классы, наследующие `BuffBase`. Три режима активации: **Burst** (одноразовый), **Constant** (постоянная дельта стата, сохраняется), **Duration** (корутин-тик, самоуничтожается). `BuffTrackerService` сериализует живые состояния баффов, включая остаток времени, при сохранении и восстанавливает их при загрузке. VFX загружаются через Addressables в `SpawnEffectAsync()` с защитой от стакинга.

<!-- 📸 Скриншот: иконки баффов в хотбаре или VFX-эффект на персонаже -->

---

### Инвентарь и хотбар

Два плоских списка слотов (основной инвентарь + хотбар), сериализуемых как часть `GameProgress`. `InventorySlotView` реализует полную цепочку drag-and-drop событий и загружает иконки асинхронно через Addressables. Хотбар маппит цифровые клавиши на `TryUseBuff()` каждый кадр через `IInputService`.

<!-- 📸 Скриншот: окно инвентаря с drag-and-drop в действии -->

---

### Система душ и телепортация

`SoulsTrackerService` хранит счётчик как `ReadOnlyReactiveProperty<int>` для подписки UI без поллинга. `TrySpendSouls` - атомарная проверка и списание.

`LevelTeleportTrigger` при контакте записывает имя телепорта и UTC-метку в объект прогресса, форсирует сохранение и передаёт управление `LoadLevelState`. Метка позволяет точно восстановить точку спауна при загрузке следующей сцены.

<!-- 📸 Скриншот: счётчик душ в HUD и триггер телепорта во вью сцены -->

---

## Данные и система сохранений

### GameProgress

Всё изменяемое состояние хранится в одном сериализуемом корневом объекте:

| Домен | Содержимое |
|---|---|
| Игрок | `PLayerState` (здоровье), `PlayerStats` (скорость, урон, дальность) |
| Мир | `WorldData` (последний трансформ, имя телепорта + UTC-время) |
| Враги | `EnemiesKilled` (ID очищенных спаунеров в `HashSetData<string>`) |
| Экономика | `SoulsCollected` (количество + позиции спаунеров) |
| Баффы | `BuffsRegistry` (класс, тип активации, состояние, остаток времени) |
| Инвентарь | `InventorySaveData` (массивы слотов с классом баффа и количеством) |

Каждая секция предоставляет `IsValid()` для отбраковки повреждённых данных.

<!-- 📸 Скриншот: данные сохранения в dev tools браузера -->

---

### Live Sync и облачные сохранения

`LiveProgressSync` автосохраняется каждые 5 секунд и отвечает на событие `OnQuitGame` от Yandex Games SDK для синхронного финального сохранения при закрытии вкладки. При включённых облачных сохранениях `GameProgress` дополнительно пушится в облако Яндекса и загружается при следующем старте сессии; при конфликте облачная копия имеет приоритет.

<!-- 📸 Скриншот: лог автосохранений в консоли -->

---

### Сериализуемые коллекции

`DictionaryData<TKey, TValue>` и `HashSetData<T>` - кастомные замены несериализуемых `Dictionary` и `HashSet`. Оба реализуют `ISerializationCallbackReceiver` для синхронизации backing-листов и поставляются с `PropertyDrawer` для корректного отображения в инспекторе.

<!-- 📸 Скриншот: DictionaryData в инспекторе -->

---

## Аудиосистема

### Music Player

`MusicPlayer` - полностью асинхронная многотрековая система на двух `AudioSource`-слотах, меняющихся ролями при кросфейде. Три пути: пустой плейлист (no-op), один трек (нативный `AudioSource.loop`, без кросфейда), несколько треков (следующий клип предзагружается через Addressables во время воспроизведения). Тасование по Фишеру-Йетсу за цикл. Параметры fade и crossfade настраиваются через `MusicPlayerConfig`. Громкость реагирует на реактивные свойства `ISoundService` в реальном времени.

<!-- 📸 Скриншот: компонент MusicPlayer в инспекторе -->

---

### Звуковые эффекты

`SoundList` хранит маппинг `SoundType` → `AudioClipGroup[]` в `DictionaryData`. Случайный клип выбирается через `IRandomService`. `MenuButtonSound` добавляет hover/click-звуки для UI-кнопок через R3-обзерверы с debounce-защитой от стакинга.

<!-- 📸 Скриншот: компонент SoundList в инспекторе -->

---

## UI и фронтенд

### Оконная система и Dev Console

Окна загружаются по требованию через Addressables via `WindowService` + `IUIFactory`. Консоль разработчика реализована в **MVVM**: `ConsoleState` / `CommandHistory` / `MobileKeyboard` как Model, `ConsoleViewModel` как ViewModel, `ConsoleRenderer` + `ConsoleStyles` + `ScrollDragHandler` как View (IMGUI, тач-скролл на мобильных). `PlatformService` адаптирует раскладку под устройство.

Команды: `clear`, `help`, `filter`, `toggle_unity_logs`, `export_logs`, `log_stats`, `set_fps`, `stat_fps`, `add_souls`, `load_level`, `quit_to_menu`, `pause_game`, `reset_game`, `warp_player`.

<!-- 📸 Скриншот: дев-консоль открыта в игре -->

---

### Drag & Drop инвентаря, шкалы здоровья, попапы

`InventorySlotView` реализует полную цепочку EventSystem, асинхронную загрузку иконок через Addressables, тултип через `ITooltipProvider` и цветовую анимацию выбора. `DragDropService` + `DragIconProvider` управляют плавающей иконкой и контекстом переноса между слотами. `HealthBar` в мировом пространстве биллбордится к камере; `TextPopup` рендерит плавающие числа урона. `HideButtonsOnPC` скрывает мобильные оверлеи на десктопе.

<!-- 📸 Скриншот: drag-and-drop между слотами и полоска здоровья врага -->

---

## Редакторный инструментарий

- **Scene Switcher Overlay** - `[Overlay]`-зарегистрированный дропдаун в тулбаре SceneView для мгновенного переключения сцен.
- **Quick Look** - стыкуемый `EditorWindow` с drag-and-drop шортлистом префабов и ScriptableObject'ов, сохраняемым в ScriptableObject-ассет, с авторефлоингом сетки кнопок.
- **Level Static Data Editor** - кнопка **Collect All Data** сканирует активную сцену и автозаполняет спаунеры врагов, телепорты и стартовую позицию игрока.
- **Scene Data Selector** - кнопка в тулбаре, переводящая инспектор к StaticData-ассету текущей сцены.
- **Манифесты и типизированные дропдауны** - `ManifestEditorBase` + `SceneDropdownKeyDrawer` / `EnumDropdownKeyDrawer` заменяют строковые ключи валидируемыми дропдаунами во всех манифест-ScriptableObject'ах.
- **Build Configuration** - `GameBuildData` централизует `DebugConfiguration`, `TargetPlatform`, переключатели облачных сохранений и рекламного SDK. `FilteredEnumAttribute` убирает сентинел `None` из enum-дропдаунов.
- **Документация соглашений** - ScriptableObject-based стайл-гайд прямо в проекте с префиксами (P\_, PA\_, PP\_, PAI\_, PUI\_) и суффиксами UI-элементов (\_CNT, \_BTN, \_BG, \_TXT, \_IMG).

<!-- 📸 Скриншот: Scene Switcher, Quick Look, инспектор LevelStaticData с кнопкой Collect -->

---

## Собственные библиотеки и расширения

- **FastMath** - два бэкенда: `FMath` (managed) реализует Fast Inverse Square Root из Quake III (`0x5f3759df` + Ньютон-Рафсон), `FastSqrt`, `FastNormalize`, `FastLength`, `FastDistance` через unsafe pointer casts; `BurstMath` оборачивает `Unity.Mathematics` с `[BurstCompile]` + `[AggressiveInlining]` для горячих путей.
- **Сериализуемые векторные типы** - `Vector3Data`, `QuatData`, `TransformData`, `Coordinates` с JSON-поддержкой, `PropertyDrawer` и конвертацией в Unity-типы.
- **UniTask Extensions** - `GetAwaiter()` для `UniTask?` / `UniTask<T>?`, enabling null-conditional `await`.
- **Functional Extensions** - `With<T>` для fluent-инициализации с опциональным условным применением.
- **GameLogger** - структурированный логгер с `[ClassName.MethodName]` через `StackFrame`; полностью вырезается в Shipping-сборках.

<!-- 📸 Скриншот: результаты перфоманс-тестов -->

---

## Тестирование

**50 тестовых файлов** в двух сборках.

**Edit Mode** покрывает: SaveData (GameProgress, PlayerStats, BuffsRegistry и граничные случаи), инвентарь, кастомные коллекции, векторные типы, FastMath, полный набор команд DevConsole и ключевые сервисы.

**Play Mode** включает smoke-тесты (все критические MonoBehaviour-компоненты подключаются без исключений через `ZenjexTestBootstrap`), интеграционные тесты (аггро, здоровье, смерть, триггеры сохранения) и перфоманс-бенчмарки (TakeDamage при 100 вызовах/измерение, стабильность frametime) через Unity Performance Testing Package.

<!-- 📸 Скриншот: Test Runner со всеми пройденными тестами -->