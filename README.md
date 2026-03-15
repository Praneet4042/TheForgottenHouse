# 🏚️The Forgotten House

<div align="center">

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Horror ](https://img.shields.io/badge/Genre-Horror-red?style=for-the-badge)
![SURVIVAL ](https://img.shields.io/badge/Genre-SURVIVAL-red?style=for-the-badge)

![Game Jam](https://img.shields.io/badge/IITK-Game%20Jam-orange?style=for-the-badge)

**A first-person horror escape game where your lantern is your only weapon — and your greatest curse.**

*Built in Unity 2022.3.62f3 | Theme: Only One + The More You Use It The Worse It Gets + Cliche*

[🎮 Getting Started](#getting-started) • [📖 Gameplay](#gameplay) • [🛠 Technical](#technical-implementation) • [🎨 Assets](#assets--credits)

</div>

---

## 📑 Table of Contents

- [About The Project](#about-the-project)
- [Theme Connection](#theme-connection)
- [Horror Clichés](#horror-clichés----embraced-and-subverted)
- [Built With](#built-with)
- [Getting Started](#getting-started)
- [Gameplay](#gameplay)
- [Minigames](#minigames)
- [Progression System](#progression-system)
- [Technical Implementation](#technical-implementation)
- [AI Generated Assets](#ai-generated-assets)
- [Creativity and Innovation](#creativity-and-innovation)
- [Timeline](#timeline)
- [Team](#team)
- [Assets and Credits](#assets--credits)
- [Acknowledgments](#acknowledgments)

---

## 📌 About The Project

**The Forgotten House** is a first-person psychological horror escape game built during the IITK Game Development Club Game Jam. The player is trapped inside a haunted house and must complete 8 unique minigames spread across 4 areas — Living Room, Bedroom, Hall, and Yard — to unlock the exit and escape.

The game features a persistent ghost that hunts the player using the lantern light as a trigger. Every second counts as health drains constantly, and using the lantern — your only tool to interact with the world — also puts you in danger.

### 🎯 Key Features

- **8 Unique Minigames** — Each with distinct mechanics, difficulty, and horror elements
- **Dynamic Ghost AI** — NavMesh-powered ghost that chases only when lantern is ON
- **Constant Health Drain** — Creates urgency and tension throughout gameplay
- **Progressive Door System** — Complete tasks to unlock new areas of the house
- **Atmospheric Horror** — Dark lighting, fog, ambient audio, and jumpscares
- **Tripo AI 3D Assets** — Custom horror models generated using AI
- **Procedural Audio** — Phone call audio generated entirely at runtime in C#


---

## 🎭 Theme Connection


### Jam Themes: **"Only One"** + **"The More You Use It The Worse It Gets"** + **"Cliché"**

The game is built entirely around all three themes through a single mechanic — the lantern.

**Only One:**
- The player has **only one lantern** — the single tool for survival and interaction
- There is **only one exit** — the player must find and unlock it
- The player has **only one life** — health drains to zero and it is game over
- There is **only one ghost** — but it is everywhere you go

**The More You Use It The Worse It Gets:**
- The lantern is **essential** — without it you cannot see, interact with objects, or complete any minigame
- But **every time you turn it on**, the ghost immediately starts chasing you
- The more tasks you complete, the longer you spend with the lantern on

- Using the lantern more = more danger = worse situation

**Cliché:**
- The entire game is built on **classic horror clichés** — haunted house, ghost, creepy dolls, Ouija Board, mysterious phone calls, spider jumpscares, and dark atmosphere
- But every cliché is **deliberately subverted** — the haunted house trope of "light = safety" is flipped into "light = danger"
- The Ouija Board is not just decoration — it has **real health consequences** based on player choices
- The Cup Game Level 3 subverts the cliché of "games are winnable" — there is **no coin**, jumpscare is inevitable
- Players who rely on horror cliché knowledge to "play smart" are punished — the Ouija Board **penalizes clever answers**
- The ghost cliché of "always hunting" is subverted — **the player controls when the ghost chases** by toggling the lantern

This creates a perfect tension loop: you **need** the lantern to progress, but using it **endangers** you. The clichés make the world feel familiar — but the subversions keep the player constantly off-balance and genuinely scared.

---


## 🛠 Built With

<div align="center">

| Technology | Purpose |
|------------|---------|
| **Unity 2022.3.62f3** | Game Engine |
| **Built-In Render Pipeline** | Rendering |
| **C#** | Programming Language |
| **Unity NavMesh** | Ghost AI Pathfinding |
| **TextMeshPro** | UI Text Rendering |
| **Quick Outline** | Object Glow Effects |
| **Tripo AI** | 3D Model Generation |
| **Git + GitHub Desktop** | Version Control |
| **ScriptableObjects** | Timing Bar Level Data |
| **glTFast** | GLB model import |
| **Floating Text** | Prompts Face Player From Any Angle |


</div>

---

## ⚡ Getting Started

### ✅ Prerequisites

- **Unity 2022.3.62f3 ** (Built-In Render Pipeline)
- **Git** installed on your system
- **TextMeshPro** package (auto-imported)
- **Quick Outline,** package from Unity Asset Store 

### 🔧 Installation

1. **Clone the repository:**
```bash
   git clone https://github.com/[your-repo-link]
   cd ForgottenHouse
```

2. **Open in Unity Hub**
   - Select Unity 2022.3.62f3 
   - Open the project folder

3. **Import Required Packages:**
   - TextMeshPro (Window → TextMeshPro → Import TMP Essential Resources)
   - Quick Outline (Unity Asset Store — free)

4. **Open Main Scene:**
   - Assets → Scenes → Main scene

5. **Press Play to test or build:**
   - File → Build Settings → Add scenes in order:
     - Index 0: StartScene
     - Index 1: Main scene
     - Index 2: EndScene

---

## 🎮 Gameplay

### Core Loop
```
Start Game
    ↓
Explore the house with lantern OFF (safe but not able to interact)
    ↓
Turn lantern ON to see objects and interact
    ↓
Ghost immediately starts chasing
    ↓
Complete minigame to proceed to next stage
    ↓
Turn lantern OFF to avoid ghost
    ↓
Repeat — complete all 8 tasks
    ↓
Grab Pills to recover 50% of HP
    ↓
Unlock exit gate and escape!
```

### Controls

| Key | Action |
|-----|--------|
| **WASD** | Move |
| **Left Shift** | Sprint |
| **Mouse** | Look Around |
| **X** | Toggle Lantern ON/OFF |
| **F** | Interact with all minigame objects |
| **C** | Search objects (Keycard Hunt only) |
| **SPACE** | Stop the ball (Timing Bar only) |
| **ESC** | Pause Menu |

### Survival Mechanics

- **Health Drain** — Health decreases constantly at 2 HP/second
- **Sprint System** — player can only sprint for 3 seconds and there is a cooldown of 7 seconds
- **Ghost Proximity Drain** — Ghost drains health faster when within 30 units
- **Kill Radius** — Ghost instantly kills player if within 5 units
- **Lantern Toggle** — T key turns lantern ON (dangerous) or OFF (safe)
- **Invincibility** — Health drain pauses during all minigames
- **Pill Boxes** — Restore 50% health, appear near unlocked doors

---

## 🎯 Minigames

### 1. 🗝️ Keycard Hunt
**Location:** Living Room + Bedroom 1
**Key:** C to search

The keycard is randomly hidden in one of 6 searchable objects — a bookshelf, dresser, coat rack, chest, closet, and drawer. Objects glow golden when lantern is ON and player is in range. Search wrong objects and receive eerie messages. Find the keycard to complete the task.

**Mechanics:**
- Random keycard placement every playthrough
- Golden outline glow on nearby objects (lantern required)
- Typewriter-style eerie search messages on empty objects
- Searched objects permanently disabled
- Can only hold one keycard search at a time
- Keycard found message dispaly on screen

---

### 2. 🏆 Cup Shell Game
**Location:** Living Room
**Key:** F to start

A classic 3-level shell game where a coin is hidden under one of three cups. Watch carefully as the cups shuffle — then pick the correct one!

**Mechanics:**
- Level 1 — Slow shuffle, 4 swaps (easy)
- Level 2 — Medium speed, 6 swaps (medium)
- Level 3 — Blazing fast, 10 swaps (the coin doesn't exist!)
- Wrong pick on Level 1/2 → spider jumpscare + retry
- Level 3 always triggers spider jumpscare regardless of choice
- Coin always returned to original position after shuffle (fair on L1/L2)
- Perspective scaling creates fake 3D depth effect
- Cup lift and slam animations with matching audio

---

### 3. 🎭 Scary Maze
**Location:** Hall / Bedroom 2
**Key:** F to start

A cursor-controlled maze game with 3 increasingly difficult levels. Guide your cursor through the maze without touching the walls.

**Mechanics:**
- Place cursor on blue dot to begin each level
- Level 1 — Wide corridors, forgiving layout
- Level 2 — Narrower paths, more complex turns
- Level 3 — Nearly impossible narrow path
- Hit wall on Level 1 or 2 → silent restart from Level 1
- Hit wall on Level 3 OR complete Level 3 → forced jumpscare
- Task counted on reaching Level 3 (win or lose)
- Mouse movement tracking using RectTransformUtility

---

### 4. 📞 Spooky Phone Call
**Location:** Hall
**Key:** F to answer

A ringing telephone plays when player enters range. Answer it to hear a haunting voice giving you a code. Then decode and enter it on the keypad.

**Mechanics:**
- Procedurally generated ghostly voice
- Phone ringing generated using sine wave tone synthesis
- Full subtitle system shows dialogue during call
- Code hint: "One four [beep] one four" — beep = **77**
- Correct code: **147714**
- Wrong code → red feedback + display shake animation + retry
- Press R to clear input, press F to replay the full hint
- Procedural static ambience using white noise + 60Hz hum

---

### 5. ⚡ Fuse Box
**Location:** Yard
**Key:** F to interact

A Simon Says style sequence memory game. Watch the fuse buttons light up in sequence then repeat it exactly.

**Mechanics:**
- 6 colored fuse buttons (Red, Blue, Green, Yellow, Purple, Orange)
- 9-step sequence to memorize and repeat
- Sequence shown with color highlight flashes
- Wrong button → "The sequence fails. Try again." + new sequence generated
- Complete sequence → "Fuses aligned. Something stirs."
- Task counted on success, cannot replay after completion

---

### 6. 🔮 Ouija Board
**Location:** Yard
**Key:** F to interact

A fully animated Ouija Board with a planchette that glides smoothly to spell out questions letter by letter. Answer YES or NO — every choice has real health consequences.

**Mechanics:**
- Planchette smoothly glides to each letter with wobble + settle animation
- Typewriter flicker effect as letters appear on screen
- Decision tree with 4 possible outcomes based on YES/NO answers:

| Path | Fate | Health Effect | Screen Effect |
|------|------|--------------|---------------|
| YES → YES | PRANK | Drops to 1 HP then restores | Red flash → Green flash |
| YES → NO | CURSE | Health cut in half | Large red flash |
| NO → YES | SPARED | Healed +20 HP | Green flash |
| NO → NO | JUMPSCARE | -10 HP | White flash → Red flash |

- Screen flashes red/green to clearly signal outcomes
- Task counted regardless of outcome
- Board cannot be reopened after completion
- Planchette flies off screen dramatically on Curse outcome

---

### 7. 🎯 Timing Bar
**Location:** Yard
**Key:** F to start, SPACE to stop

A bouncing ball moves left and right across a bar. Stop it in the green zone to advance. 4 levels of increasing speed and shrinking green zone.

**Mechanics:**
- The Ball should be stopped exactly at half or towards the right half
- Ball bounces faster each level
- Green zone position and width configurable per level
- Level data stored in ScriptableObjects (easily tunable by designers)
- Miss → "Missed! Restarting..." + restart from Level 1
- Hit green → "Nice! Next level..."
- Complete all 4 levels → task counted
- Cursor locked during play (keyboard only — SPACE to stop)

---

### 8. 🔥 Doll Bonfire
**Location:** Yard
**Key:** K at bonfire to start, M to pick up doll, O at bonfire to throw

Find 8 horror dolls scattered across the yard and burn them one by one at the bonfire. Each doll destroyed restores health.

**Mechanics:**
- Press K at bonfire to learn the task (requires lantern ON)
- All 8 dolls glow with orange outline when lantern is ON and task started
- Can only carry one doll at a time — must return to bonfire after each
- Return to bonfire and press O to throw it in
- Each doll burned → +5 HP restored to player
- Progress counter displayed: "X/8 DOLLS DESTROYED"
- "It is done..." world space message on completion
- Task counted when all 8 are burned
- FloatingText system makes all prompts face player from any angle

---

## 🚪 Progression System
```
START
│
├── Living Room + Bedroom 1
│   ├── Keycard Hunt ✓
│   └── Cup Shell Game ✓
│   Total Tasks Completed 2 → Hall door unlocks (90° rotation)
│                           → PillBox 1 appears (+50% health)
│
├── Hall + Bedroom 2
│   ├── Scary Maze ✓
│   └── Spooky Phone Call ✓
│   Total Tasks Completed 4 → Yard double doors unlock
│                           → PillBox 2 appears (+50% health)
│
└── Yard
    ├── Fuse Box ✓
    ├── Ouija Board ✓
    ├── Timing Bar ✓
    └── Doll Bonfire ✓
    Total Tasks Completed 8 → CompletionUI shows
                            → Exit gate glows golden
                            → Press F at gate → End Scene
```

---

## 🔧 Technical Implementation

### Ghost AI System
- Unity **NavMesh Agent** for pathfinding across all rooms and yard
- Ghost activates chase only when `LanternToggle.instance.isOn == true`
- Kill radius check every frame — instant death if within 5 units
- `GhostAudioController` manages idle/chase audio loops with hysteresis and 4 second cooldown to prevent rapid switching
- Ghost **frozen** during all minigames via `GhostAI.SetActive(false)`
- NavMesh rebaked after all room geometry finalized

### Minigame Architecture
- **MinigameManager** singleton controls all minigame state globally
- `StartMinigame(panel, showCursor)` — disables HorrorFPPController, sets invincible, shows panel
- `EndMinigame(panel)` — re-enables controller, resumes health drain, locks cursor
- All minigames call `TaskManager.Instance.TaskCompleted()` on success
- Teammates built minigames in isolated scenes using dummy MinigameManager
- Prefabs handed to Integration Manager for main scene assembly

### Task + Door System
- **TaskManager** singleton tracks global completion count (0–8)
- Door thresholds configurable in Inspector: 2 → Door1, 4 → Door2, 8 → Exit
- `DoorUnlock.cs` supports configurable rotation axis (X/Y/Z), angle, and speed
- Double door support — left opens +90°, right opens -90°
- PillBox objects start disabled, `Appear()` called by TaskManager at threshold
- Exit gate uses `ExitGate.cs` with Quick Outline golden glow on activation
- CompletionUI appears after 4 second delay to avoid overlap with task messages

### Player Health System
- Constant drain: 1 HP/second via `Time.deltaTime
- `SetInvincible(bool)` pauses all drain during minigames
- Health bar with color gradient —  red
- `TakeDamage(float)` and `Heal(float)` public methods used by all systems

### World Space Prompts
- All interactable objects use **3D TextMeshPro** world space text
- `FloatingText.cs` using `LateUpdate` with `Quaternion.LookRotation` + `Slerp` for smooth camera-facing
- Prompts only visible when lantern ON + player within interact range
- Quick Outline golden glow on all interactable assets (lantern required)
- Outline component on asset object, text on trigger object — connected via Inspector reference

### Procedural Audio (Spooky Phone Call)
- Phone ring: two 480Hz sine wave tones generated at runtime
- Scary Phone Call voice: wavering frequency (baseFreq + sin waver) + tremolo + breath noise
- Static ambience: white noise + 60Hz electrical hum
- All audio generated using `AudioClip.Create()` and `SetData()`
- Zero external audio files required for entire phone call sequence
- Eleven Labs Used to customize the voice
### Keycard Randomization Fix
- `KeycardManager.Awake()` assigns keycard before `SearchObject.Start()` runs
- Prevents race condition that caused keycard to always appear in first object
- `SetHasKeycard(false)` removed from `Start()` to preserve `Awake()` assignment
- Random index selection using `Random.Range(0, searchObjects.Length)`

### Version Control Strategy
- Unity-specific `.gitignore` to exclude Library and Temp folders
- Scene independence enforced — only Integration Manager commits to main scene
- Git conflict resolution using Cursor AI/ VS CODE STUDIO for merge marker cleanup

---

## 🤖 AI Generated Assets

All 3D models generated using **Tripo AI** (https://www.tripo3d.ai):

| Asset | Usage in Game |
|-------|---------------|
| **Ghost Model** | Main ghost character that chases player |
| **Horror Doll x8** | 8 collectible dolls in yard |
| **Dead Trees** | Yard atmosphere decoration |
| **Vintage Radio**  |Used For StopTheBall Minigame  |
| **Ouija Board**  |Used For Ouija Board Minigame  |
| **Rusted Metal Cups** |Used For CupShellGame Minigame  |
| **Arcade Machine** | Used for Scary Maze Minigame |
| **Antiqe Telephone** |Used for Spooky Call Minigame  |
| **Power Transmition Tower** |Used for FuseBox Minigame  |
| **Rest of the Household Assets** |Bed, Antique Wardrobe, Old Wooden Door Panel, Crates, Port Holder,Clock|

**Import Process:**
- Exported as FBX or GLB from Tripo AI
- GLB files imported via glTFast Unity package
- No Texture issues resolved using extracting textures from assets 
- Mesh Colliders added manually post-import
- Scale adjusted to match Unity world scale

---

## 💡 Creativity and Innovation

### Unique Lantern Mechanic
Unlike typical horror games where light = safety, in Forgotten House **light = danger**. This single design decision inverts the player's survival instinct and creates constant psychological tension. Every interaction requires risk assessment — is this task worth turning the lantern on right now?

### Procedural Horror Audio
The Spooky Phone Call minigame generates all audio procedurally at runtime using mathematical wave synthesis — no audio files needed. All sounds are either custom made or used from freesound.org

### Ouija Board Decision Tree
The Ouija Board features a branching narrative with 4 distinct outcomes, each with real health consequences. Players who think they're "smart" by choosing YES/YES get punished — a meta commentary on player behavior that rewards genuine engagement over optimization.

### Cup Game Level 3 Deception
Level 3 of the Cup Shell Game has **no coin** — it is impossible to win. This deliberately subverts game design expectations and shocks the player with a jumpscare regardless of their choice. The player only discovers this the first time they reach Level 3.

### Singleton Architecture
A clean singleton-based MinigameManager allows any minigame to integrate with just 3 lines of code. This enabled true parallel development — teammates built minigames in complete isolation and handed off prefabs to the Integration Manager.

### FloatingText System
All world space prompts use a LateUpdate-based LookAt system that smoothly faces the player camera from any direction. Combined with Quick Outline glow, this creates a polished interaction system that works consistently across all 8 minigames.

### Theme Dual Interpretation
The game interprets both jam themes simultaneously through a single mechanic (the lantern), making the design cohesive and elegant rather than forcing two separate systems to justify the themes.

---

## ⏳ Timeline

| Day | Milestone |
|-----|-----------|
| **Day 1** | Ideation, Core architecture — LanternToggle, GhostAI, PlayerHealth, GameOverManager, HealthBarUI |
| **Day 2** | FuseBox minigame, MinigameManager, TaskManager, GhostAudioController |
| **Day 3** | Keycard Hunt, world space prompts, Quick Outline, scene lighting and atmosphere |
| **Day 4** | Timing Bar integration, Scary Maze integration, Ouija Board integration |
| **Day 5** | Spooky Phone Call, Cup Shell Game, Doll Bonfire system |
| **Day 6** | Door system, PillBox, ExitGate, CompletionUI, Pause Menu |
| **Day 7** | World prompts + lantern checks for all 8 minigames, F key standardization, final testing and build |

---

## 👥 Team

| Name | Role |
|------|------|
| **Praneet Ayush Lakra** | Integration Manager — Main scene, Ghost AI, Health System, Door System, Keycard Hunt, Doll Bonfire, all script integration, NavMesh, lighting, Assets Generation, Ideation |
| **Ayush Kumar** | Timing Bar minigame, Spooky call minigame, Terrain Building, Sound FX, Sound Manager, Ideation|
| **Pious Kujur** | Mini Game Ideation, Scary Maze minigame, Ouija Board minigame, ShellGame Minigame, Sound FX, Sound Manager, Ideation, Main Game Core Ideation |
| **Mayank Kapse** | Start Screen, End Screen, Pause Menu, Typewriter Mechanic, SoundFX, Instruction,  Ideation |
| **Mayank Agarwal** | Assets Generation from Tripo AI, Ideation |

---

## 🎨 Assets & Credits

### Unity Asset Store

| Asset | Usage |
|------ |-------|
| **Quick Outline** | Object glow/outline effects on all interactables |
| **BK Alchemist House** | Interior furniture, walls, floors, doors |
| **Cemetery Pack** |  Yard environment, fence, gates |
| **Mountain Terrain Rocks** |  Outdoor terrain decoration |
| **First Aid Jar** |  PillBox health pickup model |
| **Campfire Asset** |  Bonfire in yard |
| **ALP Terrain** |  Main outdoor terrain |


### Audio
- Procedurally generated: phone call voice, ringing, static (pure C#)
- Ghost audio: sourced from free SFX libraries
- maze: sourced from free SFX libraries
- CupShellGame: good/bad outcome sounds from free SFX libraries
- Used freesound.org
---

## 🙌 Acknowledgments

- **Unity Technologies** — Game Engine and documentation
- **Tripo AI** (tripo3d.ai) — AI 3D model generation
- **Unity Asset Store** — Assets for main scene
- **IITK Game Development Club** — Game Jam organization and support
- **Unity Forums** — Technical support and troubleshooting
- **TextMeshPro** — Advanced text rendering system
- **Claude (Anthropic)** — AI assistance during development
- **ChatGPT (Anthropic)** — AI assistance during development

---

<div align="center">

**Made with 🕯️ and fear at IITK Game Jam 2026**

*"Only one lantern. The more you use it, the worse it gets."*


</div>