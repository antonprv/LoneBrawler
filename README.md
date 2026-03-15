<div align="center">

[🇬🇧 English](#lone-brawler--english) &nbsp;|&nbsp; [🇷🇺 Русский](#lone-brawler--русский)

</div>

---

# Lone Brawler &nbsp;·&nbsp; English

> A 3D action game built in Unity. The codebase covers the full production pipeline: gameplay systems, infrastructure, UI, data management, editor tooling, and an in-house math library - 442 C# files and over 200 000 lines of code. Check out project's **[documentation](https://antonprv.github.io/lonebrawler-website/)** for details.

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

The application lifecycle runs through a generic state machine with payload support. Each state is a self-contained class resolved via `StateFactory`, so the machine itself stays thin and handles nothing but transitions.

```
BootstrapperState → LoadProgressState → MainMenuState → LoadLevelState → GameLoopState
```

States come in two forms: `Enter()` for plain transitions and `Enter<TPayload>(payload)` when data needs to travel with the transition - for example, passing a level key into `LoadLevelState`.

<!-- 📸 Screenshot: State machine flow diagram or GameInstance inspector -->

---

### Dependency Injection - Zenjex

**Zenjex** is a custom DI framework built for this project, later released as a [standalone open-source library](https://github.com/antonprv/Zenjex). It wraps the **Reflex** container with a Zenject-style fluent API and Unity-specific injection mechanics.

Dependencies are declared with `[Zenjex]` on private readonly fields - no constructor boilerplate, no manual `Inject()` calls:

```csharp
[Zenjex] private readonly ISaveLoadService _saveLoad;
[Zenjex] private readonly IStaticDataService _staticData;
```

Injection runs in three passes: `OnContainerReady` at execution order -280, `OnGameLaunched` after async setup, and `sceneLoaded` for additive scenes. `ZenjexRunner` deduplicates by `GetInstanceID()` so no object gets processed twice. `ZenjexBehaviour` is the strictest path - fields are filled before `OnAwake()` runs. `IInitializable` gives services a clean post-injection entry point, similar to Zenject's own interface.

The fluent binding API covers singletons, transients, scoped bindings, prefab instantiation, `WithArguments`, `BindInterfacesAndSelf`, `CopyIntoDirectSubContainers`, and eager vs. lazy resolution. `ZenjexSceneContext` gives global services access to per-scene sub-containers without any MonoBehaviour coupling. The built-in editor debugger lists all injection records per pass and highlights `ZNX-LATE` warnings.

<!-- 📸 Screenshot: Zenjex debugger window or [Zenjex] attribute on a field in inspector -->

---

### Addressables & Asset Management

`AssetLoader` wraps Unity Addressables with a handle-caching layer. On first load a completed handle is stored by GUID; repeat requests return the cached result directly. Instantiated objects are tracked in a separate list, so `Cleanup()` releases every handle in a single pass when the scene tears down.

<!-- 📸 Screenshot: Addressables groups window -->

---

## Platform & SDKs

The game ships on **WebGL** (Yandex Games) and **Android** (Google Play, RuStore, and other storefronts). Two Yandex SDKs cover both distribution channels.

### Yandex Games SDK

Covers the full Yandex Games platform stack: interstitial and rewarded ads, in-app purchases with server-side consumption, and **cloud saves** that sync `GameProgress` across devices. Cloud data takes precedence over the local `PlayerPrefs` copy when both are present.

### Authentication & Player Profile

The main menu has a **Login** button that kicks off Yandex OAuth. After a successful response the game fetches the player's profile from the Yandex server and shows their **name and avatar** in the UI. Sessions without login fall back to guest mode with local saves only.

<!-- 📸 Screenshot: Login button in main menu and player avatar/name displayed after auth -->

### Yandex Ads SDK

For builds outside the Yandex Games ecosystem (Google Play, RuStore, etc.), the Yandex Ads SDK handles banner, interstitial, and rewarded ads. The active SDK is switched at build time via the `UseAddSdk` flag in `GameBuildData`, so one codebase produces clean platform-specific builds without conditional compilation spread across gameplay code.

<!-- 📸 Screenshot: Rewarded ad flow or build config toggling SDK flags -->

---

## Gameplay Systems

### Player

The player character is built from discrete, interface-driven components: `PlayerMove`, `PlayerHealth`, `PlayerDeath`, `PlayerAnimator`, `PlayerBuffConsumer`. Each one reads its starting values from `PlayerStats` in `GameProgress`, which keeps the save system and the gameplay layer independent of each other.

<!-- 📸 Screenshot: Player in-game or player component hierarchy in inspector -->

---

### Enemy AI

`AggroSystem` tracks proximity and moves the enemy into an attack-ready state. `EnemyMovement` then drives NavMesh navigation toward the player. Attack behaviour sits behind `IAttackBehaviour` and comes in melee and projectile variants, each configured per enemy type in `EnemyStaticData` ScriptableObjects. Both projectiles and VFX run through object pools.

<!-- 📸 Screenshot: Enemy aggro radius gizmo in scene view or enemy attack in action -->

---

### Buff System

Buffs are plain C# classes that inherit `BuffBase`. Three activation modes: **Burst** (fires once), **Constant** (permanent stat change, written into the save), **Duration** (coroutine tick loop, cleans itself up when time runs out). `BuffTrackerService` snapshots live buff state - including remaining duration - on save and restores everything on load. VFX are loaded via Addressables inside `SpawnEffectAsync()` with a guard against effect stacking.

<!-- 📸 Screenshot: Buff icons in hotbar or buff VFX on player character -->

---

### Inventory & Hotbar

Two flat slot lists (main inventory and hotbar) serialised inside `GameProgress`. `InventorySlotView` handles the full drag-and-drop event chain and fetches icons from Addressables asynchronously on each refresh. The hotbar reads numeric key input every frame through `IInputService` and calls `TryUseBuff()` on the selected slot.

<!-- 📸 Screenshot: Inventory window with drag-and-drop in action -->

---

### Souls System & Level Teleportation

`SoulsTrackerService` exposes the souls counter as `ReadOnlyReactiveProperty<int>`, so UI elements subscribe rather than poll. `TrySpendSouls` checks and deducts in one atomic call.

When the player touches a `LevelTeleportTrigger`, the component writes a teleport name and UTC timestamp into the progress object, forces a save, then hands off to `LoadLevelState`. The timestamp and name let the next scene restore the correct spawn point.

<!-- 📸 Screenshot: Souls counter in HUD and teleport trigger in scene view -->

---

## Data Management & Save System

### GameProgress

All mutable runtime state lives in one serialisable root object:

| Domain | Contents |
|---|---|
| Player | `PLayerState` (health), `PlayerStats` (speed, damage, range) |
| World | `WorldData` (last transform, teleport name + UTC time) |
| Enemies | `EnemiesKilled` (cleared spawner IDs as `HashSetData<string>`) |
| Economy | `SoulsCollected` (amount + spawner positions) |
| Buffs | `BuffsRegistry` (class, activation type, state, remaining duration) |
| Inventory | `InventorySaveData` (slot arrays with buff class + count) |

Each section has an `IsValid()` guard that rejects corrupted or default-constructed data.

<!-- 📸 Screenshot: Save data in browser dev tools -->

---

### Live Sync & Cloud Saves

`LiveProgressSync` autosaves every 5 seconds and responds to the `OnQuitGame` event from the Yandex Games SDK with a synchronous final save when the browser tab closes. With cloud saves on, `GameProgress` is also pushed to Yandex cloud storage and pulled on the next session start - the cloud copy wins on conflict.

<!-- 📸 Screenshot: Console log of autosave events -->

---

### Serialisable Collections

`DictionaryData<TKey, TValue>` and `HashSetData<T>` are custom replacements for Unity's unserialisable `Dictionary` and `HashSet`. Both implement `ISerializationCallbackReceiver` to keep backing lists in sync, and both ship with `PropertyDrawer` implementations for correct Inspector display.

<!-- 📸 Screenshot: DictionaryData displayed in inspector -->

---

## Audio System

### Music Player

`MusicPlayer` is a fully async multi-track system built on two `AudioSource` slots that swap roles on every crossfade. Three paths are picked automatically: empty playlist is a no-op, a single track delegates to native `AudioSource.loop` with no crossfade overhead, and multiple tracks get Addressables pre-loading plus auto-advance. Shuffle uses Fisher-Yates per loop cycle. Fade and crossfade timings come from `MusicPlayerConfig`. Volume reacts to `ISoundService` reactive properties in real time.

<!-- 📸 Screenshot: MusicPlayer component in inspector -->

---

### Sound Effects

`SoundList` maps `SoundType` enum keys to `AudioClipGroup` arrays inside a `DictionaryData`. On each playback a random clip is picked through `IRandomService`. `MenuButtonSound` attaches hover and click sounds to UI buttons via R3 observables with debounce guards to stop sounds from stacking on rapid input.

<!-- 📸 Screenshot: SoundList component in inspector -->

---

## UI & Frontend

### Window System & Dev Console

Windows load on demand from Addressables through `WindowService` and `IUIFactory`. The developer console follows a strict **MVVM** split - `ConsoleState`, `CommandHistory`, and `MobileKeyboard` are the Model; `ConsoleViewModel` is the ViewModel; `ConsoleRenderer`, `ConsoleStyles`, and `ScrollDragHandler` form the View (IMGUI, touch-scroll on mobile). `PlatformService` adapts the layout per device.

Commands: `clear`, `help`, `filter`, `toggle_unity_logs`, `export_logs`, `log_stats`, `set_fps`, `stat_fps`, `add_souls`, `load_level`, `quit_to_menu`, `pause_game`, `reset_game`, `warp_player`.

<!-- 📸 Screenshot: Dev console open in-game -->

---

### Inventory Drag & Drop, Health Bars, Popups

`InventorySlotView` runs the full EventSystem interface chain, fetches icons from Addressables asynchronously, shows tooltips through `ITooltipProvider` on hover, and animates the selection colour. `DragDropService` and `DragIconProvider` manage the floating drag icon and the transfer context between slots. World-space `HealthBar` components billboard toward the camera; `TextPopup` renders floating damage numbers. `HideButtonsOnPC` removes mobile overlays on desktop.

<!-- 📸 Screenshot: Drag-and-drop between inventory slots and enemy health bar -->

---

## Editor Tooling

- **Scene Switcher Overlay** - a `[Overlay]`-registered SceneView toolbar dropdown for instant scene switching.
- **Quick Look** - a dockable `EditorWindow` with a drag-and-drop shortlist of prefabs and ScriptableObjects, persisted to a ScriptableObject asset, with an auto-reflowing button grid.
- **Level Static Data Editor** - the **Collect All Data** button scans the active scene and fills in enemy spawners, teleports, and the player start position.
- **Scene Data Selector** - a toolbar button that jumps the Inspector to the StaticData asset for the currently open scene.
- **Manifests & Typed Dropdowns** - `ManifestEditorBase` with `SceneDropdownKeyDrawer` and `EnumDropdownKeyDrawer` replaces raw string keys with validated dropdowns across all manifest ScriptableObjects.
- **Build Configuration** - `GameBuildData` centralises `DebugConfiguration`, `TargetPlatform`, and the cloud save and ad SDK toggles. `FilteredEnumAttribute` strips the `None` sentinel from enum dropdowns.
- **Naming Convention Doc** - a ScriptableObject style guide inside the project, covering prefab prefixes (P\_, PA\_, PP\_, PAI\_, PUI\_) and UI element suffixes (\_CNT, \_BTN, \_BG, \_TXT, \_IMG).

<!-- 📸 Screenshot: Scene Switcher, Quick Look, LevelStaticData inspector with Collect button -->

---

## Custom Libraries & Extensions

- **FastMath** - two backends: `FMath` (managed) implements the Quake III Fast Inverse Square Root (`0x5f3759df` + Newton-Raphson), `FastSqrt`, `FastNormalize`, `FastLength`, and `FastDistance` via unsafe pointer casts; `BurstMath` wraps `Unity.Mathematics` with `[BurstCompile]` and `[AggressiveInlining]` for hot paths.
- **Serialisable vector types** - `Vector3Data`, `QuatData`, `TransformData`, `Coordinates` with JSON support, `PropertyDrawer` implementations, and Unity conversion helpers.
- **UniTask Extensions** - `GetAwaiter()` overloads for `UniTask?` and `UniTask<T>?`, making null-conditional `await` work cleanly.
- **Functional Extensions** - `With<T>` overloads for fluent object setup with optional conditional branching.
- **GameLogger** - a structured logger that prepends `[ClassName.MethodName]` via `StackFrame`; the entire body compiles out in Shipping builds.

<!-- 📸 Screenshot: Performance test results -->

---

## Testing

**50 test files** across two assemblies.

**Edit Mode** covers SaveData (GameProgress, PlayerStats, BuffsRegistry, and edge cases), inventory, custom collections, vector types, FastMath, the full DevConsole command set, and core services.

**Play Mode** has smoke tests (all critical MonoBehaviour components attach without exceptions, using `ZenjexTestBootstrap`), integration tests (aggro, health, death, save triggers), and performance benchmarks (TakeDamage at 100 calls per measurement, frame-time stability) through Unity Performance Testing Package.

<!-- 📸 Screenshot: Test Runner with all tests passing -->

---
---

<div align="center">

[🇬🇧 English](#lone-brawler--english) &nbsp;|&nbsp; [🇷🇺 Русский](#lone-brawler--русский)

</div>

---

# Lone Brawler &nbsp;·&nbsp; Русский

> 3D экшн на Unity. Кодовая база охватывает полный производственный цикл: геймплейные системы, инфраструктура, UI, управление данными, редакторный инструментарий и собственная математическая библиотека - 442 файла на C#, 200 тысяч строк кода. Все детали в **[документации проекта](https://antonprv.github.io/lonebrawler-website/)**.

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

Жизненный цикл приложения проходит через generic-машину состояний с поддержкой payload. Каждое состояние - самодостаточный класс, создаваемый через `StateFactory`, поэтому сама машина остаётся тонкой и занимается только переходами.

```
BootstrapperState → LoadProgressState → MainMenuState → LoadLevelState → GameLoopState
```

Состояния бывают двух видов: `Enter()` для простых переходов и `Enter<TPayload>(payload)` когда с переходом нужно передать данные - например, ключ уровня в `LoadLevelState`.

<!-- 📸 Скриншот: диаграмма переходов или инспектор GameInstance -->

---

### Dependency Injection - Zenjex

**Zenjex** - собственный DI-фреймворк, написанный для этого проекта и выпущенный как [отдельная open-source библиотека](https://github.com/antonprv/Zenjex). Оборачивает контейнер **Reflex** Zenject-подобным fluent API и Unity-специфичными механиками инъекции.

Зависимости объявляются атрибутом `[Zenjex]` на приватных readonly-полях - никакого конструкторного бойлерплейта, никаких ручных вызовов `Inject()`:

```csharp
[Zenjex] private readonly ISaveLoadService _saveLoad;
[Zenjex] private readonly IStaticDataService _staticData;
```

Инъекция выполняется в три прохода: `OnContainerReady` с порядком выполнения -280, `OnGameLaunched` после асинхронной настройки, `sceneLoaded` для аддитивных сцен. `ZenjexRunner` дедуплицирует по `GetInstanceID()` - ни один объект не обрабатывается дважды. `ZenjexBehaviour` - строжайший путь, где поля заполняются до `OnAwake()`. `IInitializable` даёт сервисам чистую точку входа после инъекции, аналог одноимённого интерфейса в Zenject.

Fluent binding API покрывает синглтоны, транзиенты, scoped биндинги, инстанцирование из префабов, `WithArguments`, `BindInterfacesAndSelf`, `CopyIntoDirectSubContainers`, eager и lazy resolution. `ZenjexSceneContext` открывает глобальным сервисам доступ к per-scene sub-контейнерам без привязки к MonoBehaviour. Встроенный редакторный дебаггер показывает все записи инъекций по проходам и подсвечивает `ZNX-LATE` предупреждения.

<!-- 📸 Скриншот: окно Zenjex Debugger или атрибут [Zenjex] на поле в инспекторе -->

---

### Addressables и управление ассетами

`AssetLoader` оборачивает Unity Addressables слоем кеширования хэндлов. При первой загрузке завершённый хэндл сохраняется по GUID; повторные запросы сразу возвращают кешированный результат. Инстанциированные объекты хранятся отдельным списком, поэтому `Cleanup()` освобождает все хэндлы за один проход при выгрузке сцены.

<!-- 📸 Скриншот: окно Addressables Groups -->

---

## Платформа и SDK

Игра выходит на **WebGL** (Яндекс Игры) и **Android** (Google Play, RuStore и другие сторы). Два SDK от Яндекса покрывают оба канала дистрибуции.

### Yandex Games SDK

Закрывает полный стек платформы: показ рекламы (interstitial и rewarded), внутриигровые покупки с серверным консумированием и **облачные сохранения**, синхронизирующие `GameProgress` между устройствами. При наличии облачного сейва он имеет приоритет над локальным `PlayerPrefs`.

### Авторизация и профиль игрока

В главном меню есть кнопка **Login**, запускающая Yandex OAuth. После успешного ответа игра запрашивает профиль с сервера Яндекса и отображает **имя и аватар** игрока в UI. Без авторизации сессия корректно переходит в гостевой режим с локальными сохранениями.

<!-- 📸 Скриншот: кнопка Login в главном меню и имя/аватар игрока после авторизации -->

### Yandex Ads SDK

Для сборок вне экосистемы Яндекс Игр (Google Play, RuStore и т.д.) монетизация идёт через Yandex Ads SDK - баннеры, interstitial и rewarded реклама. Активный SDK переключается флагом `UseAddSdk` в `GameBuildData` на этапе сборки, поэтому одна кодовая база даёт чистые платформоспецифичные билды без условной компиляции в геймплейном коде.

<!-- 📸 Скриншот: флоу rewarded-рекламы или конфиг сборки с переключателями SDK -->

---

## Геймплейные системы

### Персонаж игрока

Персонаж собран из дискретных, интерфейс-ориентированных компонентов: `PlayerMove`, `PlayerHealth`, `PlayerDeath`, `PlayerAnimator`, `PlayerBuffConsumer`. Каждый читает начальные значения из `PlayerStats` в `GameProgress`, что держит систему сохранений и геймплейный слой независимыми друг от друга.

<!-- 📸 Скриншот: персонаж в игре или иерархия компонентов в инспекторе -->

---

### ИИ противников

`AggroSystem` отслеживает близость и переводит врага в боеготовное состояние. `EnemyMovement` запускает NavMesh-навигацию к игроку. Поведение атаки вынесено за интерфейс `IAttackBehaviour` и бывает ближнего боя или снарядным, конфигурируется per-тип врага в `EnemyStaticData`. Снаряды и VFX работают через пулы объектов.

<!-- 📸 Скриншот: гизмо радиуса аггро или атака врага в динамике -->

---

### Система баффов

Баффы - чистые C#-классы, наследующие `BuffBase`. Три режима активации: **Burst** (срабатывает один раз), **Constant** (постоянное изменение стата, записывается в сейв), **Duration** (корутин-тик, сам завершается по истечении времени). `BuffTrackerService` снапшотит живые состояния баффов - в том числе остаток времени - при сохранении и восстанавливает при загрузке. VFX загружаются через Addressables в `SpawnEffectAsync()` с защитой от стакинга.

<!-- 📸 Скриншот: иконки баффов в хотбаре или VFX на персонаже -->

---

### Инвентарь и хотбар

Два плоских списка слотов (основной инвентарь и хотбар), сериализуемых внутри `GameProgress`. `InventorySlotView` обрабатывает полную цепочку drag-and-drop событий и загружает иконки из Addressables асинхронно при каждом обновлении. Хотбар читает нажатия цифровых клавиш каждый кадр через `IInputService` и вызывает `TryUseBuff()` на выбранном слоте.

<!-- 📸 Скриншот: окно инвентаря с drag-and-drop в действии -->

---

### Система душ и телепортация

`SoulsTrackerService` хранит счётчик душ как `ReadOnlyReactiveProperty<int>` - UI-элементы подписываются, а не опрашивают. `TrySpendSouls` проверяет баланс и списывает за один атомарный вызов.

При контакте с `LevelTeleportTrigger` компонент записывает имя телепорта и UTC-метку в объект прогресса, форсирует сохранение и передаёт управление `LoadLevelState`. Метка и имя позволяют следующей сцене восстановить точную точку спауна.

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

У каждой секции есть метод `IsValid()`, отклоняющий повреждённые или дефолтно-сконструированные данные.

<!-- 📸 Скриншот: данные сохранения в dev tools браузера -->

---

### Live Sync и облачные сохранения

`LiveProgressSync` автосохраняется каждые 5 секунд и реагирует на событие `OnQuitGame` от Yandex Games SDK синхронным финальным сохранением при закрытии вкладки. При включённых облачных сохранениях `GameProgress` параллельно пушится в облако Яндекса и загружается при следующем старте - при конфликте побеждает облачная копия.

<!-- 📸 Скриншот: лог автосохранений в консоли -->

---

### Сериализуемые коллекции

`DictionaryData<TKey, TValue>` и `HashSetData<T>` - замены несериализуемых `Dictionary` и `HashSet` от Unity. Оба реализуют `ISerializationCallbackReceiver` для синхронизации backing-листов и поставляются с `PropertyDrawer` для корректного отображения в инспекторе.

<!-- 📸 Скриншот: DictionaryData в инспекторе -->

---

## Аудиосистема

### Music Player

`MusicPlayer` - полностью асинхронная многотрековая система на двух `AudioSource`-слотах, меняющихся ролями при кросфейде. Три пути выбираются автоматически: пустой плейлист - no-op, один трек делегирует нативному `AudioSource.loop` без кросфейда, несколько треков - предзагрузка через Addressables и авто-переход. Тасование по Фишеру-Йетсу за цикл. Параметры fade и crossfade берутся из `MusicPlayerConfig`. Громкость реагирует на реактивные свойства `ISoundService` в реальном времени.

<!-- 📸 Скриншот: компонент MusicPlayer в инспекторе -->

---

### Звуковые эффекты

`SoundList` хранит маппинг `SoundType` → `AudioClipGroup[]` в `DictionaryData`. Случайный клип выбирается через `IRandomService`. `MenuButtonSound` добавляет hover и click-звуки к UI-кнопкам через R3-обзерверы с debounce-защитой от стакинга при быстрых нажатиях.

<!-- 📸 Скриншот: компонент SoundList в инспекторе -->

---

## UI и фронтенд

### Оконная система и Dev Console

Окна загружаются по требованию через Addressables через `WindowService` и `IUIFactory`. Консоль разработчика выстроена по **MVVM**: `ConsoleState`, `CommandHistory` и `MobileKeyboard` как Model; `ConsoleViewModel` как ViewModel; `ConsoleRenderer`, `ConsoleStyles` и `ScrollDragHandler` как View (IMGUI, тач-скролл на мобильных). `PlatformService` адаптирует раскладку под устройство.

Команды: `clear`, `help`, `filter`, `toggle_unity_logs`, `export_logs`, `log_stats`, `set_fps`, `stat_fps`, `add_souls`, `load_level`, `quit_to_menu`, `pause_game`, `reset_game`, `warp_player`.

<!-- 📸 Скриншот: дев-консоль открыта в игре -->

---

### Drag & Drop инвентаря, шкалы здоровья, попапы

`InventorySlotView` обрабатывает полную цепочку EventSystem, загружает иконки из Addressables асинхронно, показывает тултип через `ITooltipProvider` и анимирует цвет выбора. `DragDropService` и `DragIconProvider` ведут плавающую иконку и контекст переноса между слотами. `HealthBar` в мировом пространстве поворачивается к камере; `TextPopup` показывает плавающие числа урона. `HideButtonsOnPC` убирает мобильные оверлеи на десктопе.

<!-- 📸 Скриншот: drag-and-drop между слотами и полоска здоровья врага -->

---

## Редакторный инструментарий

- **Scene Switcher Overlay** - дропдаун в тулбаре SceneView для мгновенного переключения между сценами, зарегистрированный через `[Overlay]`.
- **Quick Look** - стыкуемый `EditorWindow` с drag-and-drop шортлистом префабов и ScriptableObjects, сохраняемым в ассет проекта, с авторефлоингом сетки кнопок.
- **Level Static Data Editor** - кнопка **Collect All Data** сканирует открытую сцену и заполняет спаунеры врагов, телепорты и стартовую позицию игрока.
- **Scene Data Selector** - кнопка в тулбаре, переводящая инспектор к StaticData-ассету текущей сцены.
- **Манифесты и типизированные дропдауны** - `ManifestEditorBase` с `SceneDropdownKeyDrawer` и `EnumDropdownKeyDrawer` заменяет строковые ключи валидируемыми дропдаунами во всех манифест-ScriptableObjects.
- **Build Configuration** - `GameBuildData` хранит `DebugConfiguration`, `TargetPlatform` и переключатели облачных сохранений и рекламного SDK в одном месте. `FilteredEnumAttribute` убирает сентинел `None` из enum-дропдаунов.
- **Документация соглашений** - стайл-гайд в виде ScriptableObject прямо внутри проекта: префиксы префабов (P\_, PA\_, PP\_, PAI\_, PUI\_) и суффиксы UI-элементов (\_CNT, \_BTN, \_BG, \_TXT, \_IMG).

<!-- 📸 Скриншот: Scene Switcher, Quick Look, инспектор LevelStaticData с кнопкой Collect -->

---

## Собственные библиотеки и расширения

- **FastMath** - два бэкенда: `FMath` (managed) реализует Fast Inverse Square Root из Quake III (`0x5f3759df` + Ньютон-Рафсон), `FastSqrt`, `FastNormalize`, `FastLength` и `FastDistance` через unsafe pointer casts; `BurstMath` оборачивает `Unity.Mathematics` с `[BurstCompile]` и `[AggressiveInlining]` для горячих путей.
- **Сериализуемые векторные типы** - `Vector3Data`, `QuatData`, `TransformData`, `Coordinates` с JSON-поддержкой, `PropertyDrawer` и хелперами конвертации в Unity-типы.
- **UniTask Extensions** - `GetAwaiter()` для `UniTask?` и `UniTask<T>?`, дающие null-conditional `await`.
- **Functional Extensions** - `With<T>` для fluent-инициализации объектов с опциональным условным применением.
- **GameLogger** - структурированный логгер с `[ClassName.MethodName]` через `StackFrame`; всё тело метода компилируется в ничто в Shipping-сборках.

<!-- 📸 Скриншот: результаты перфоманс-тестов -->

---

## Тестирование

**50 тестовых файлов** в двух сборках.

**Edit Mode** покрывает SaveData (GameProgress, PlayerStats, BuffsRegistry, граничные случаи), инвентарь, кастомные коллекции, векторные типы, FastMath, полный набор команд DevConsole и ключевые сервисы.

**Play Mode** включает smoke-тесты (все критические MonoBehaviour-компоненты подключаются без исключений через `ZenjexTestBootstrap`), интеграционные тесты (аггро, здоровье, смерть, триггеры сохранения) и перфоманс-бенчмарки (TakeDamage при 100 вызовах на измерение, стабильность frametime) через Unity Performance Testing Package.

<!-- 📸 Скриншот: Test Runner со всеми пройденными тестами -->
