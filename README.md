<p align="center">
  <img src="assets/logo.png" alt="HonkAPI Logo" width="300"/>
</p>

HonkAPI is a C# modding framework and runtime abstraction layer for *Untitled Goose Game* (v1.1.4). Built for BepInEx 5 and Harmony, it provides high-level APIs to interact with game systems, player entities, human NPCs, physics objects, audio, and user interfaces without requiring direct bytecode manipulation or manual patches.

---

## Architecture Overview

HonkAPI abstracts internal game assemblies into decoupled modules exposed through the static `Honk` facade:

- **HonkAPI.Core**: Mod discovery, lifecycle management (`IHonkMod`, `HonkContext`), persistent key-value storage (`HonkSaveAPI`), and native main menu integration.
- **HonkAPI.GooseApi**: Direct manipulation of the active goose instance (physics, transformations, animations, voice, and inventory).
- **HonkAPI.NPC**: Querying and commanding human AI entities (`Brain`) and their NavMesh navigation states.
- **HonkAPI.Props**: Procedural instantiation, cloning, and physics manipulation of interactive objects (`Prop`).
- **HonkAPI.Events**: Unified event bus providing hooks for entity lifecycle, actions, and game state transitions.
- **HonkAPI.Audio**: Invocation of native FMOD events and custom runtime audio playback.
- **HonkAPI.World**: Environment queries, time scaling, and camera control.
- **HonkAPI.UI**: Immediate-mode GUI runtime (`HonkWindow`) and native menu hooks.

---

## Core API Reference

### 1. Goose Control (`Honk.Goose`)

The `Honk.Goose` subsystem manages active goose instances:

```csharp
// Retrieve the active goose instance
Goose activeGoose = Honk.Goose.Current;

// Movement and transformation
Honk.Goose.SetSpeedMultiplier(1.5f);
Honk.Goose.SetScale(1.2f);
Honk.Goose.Teleport(new Vector3(10f, 0f, 5f));

// Actions and state
Honk.Goose.HonkSound(1.0f);
Honk.Goose.FlapWings();
Honk.Goose.DuckHead(0.8f);

// Item manipulation
Prop heldItem = Honk.Goose.GetHeldProp();
Honk.Goose.DropHeldProp();
Honk.Goose.GrabProp(targetProp);
```

### 2. Human NPC Management (`Honk.NPCs`)

The `Honk.NPCs` subsystem queries and directs human characters:

```csharp
// Entity queries
List<Brain> allNpcs = Honk.NPCs.GetAll();
Brain nearest = Honk.NPCs.GetClosest(goosePosition);

// Behavioral shortcuts
Honk.NPCs.Scare(nearest);
Honk.NPCs.Alert(nearest);
Honk.NPCs.Confuse(nearest);
Honk.NPCs.MakeGrumpy(nearest);
Honk.NPCs.Calm(nearest);

// Navigation and interaction
Honk.NPCs.WalkTo(nearest, destinationCoordinates);
Honk.NPCs.MakeDropHeld(nearest);
```

#### Brain state machine (`Honk.NPCs.States`)

Every NPC runs a priority-ordered state machine with 62 states, exposed as the `HumanState` enum. The enum value is the state's priority: lower values win. Each frame the game selects the first state that is both desired and still valid.

```csharp
// Read
HumanState? current = Honk.NPCs.States.Current(npc);
bool chasing = Honk.NPCs.States.IsIn(npc, HumanState.Chase);
List<HumanState> wanted = Honk.NPCs.States.Desired(npc);

// Nudge: mark a state desired and let normal priority decide
Honk.NPCs.States.Request(npc, HumanState.Search);

// Override: switch immediately and hold, ignoring priority
Honk.NPCs.States.Force(npc, HumanState.Chase, 10f);   // 0 = until Release
Honk.NPCs.States.Release(npc);

// Remove states from consideration, per NPC or everywhere
Honk.NPCs.States.Suppress(npc, HumanState.ReactionStartled);
Honk.NPCs.States.SuppressEverywhere(HumanState.Shoo);

// Reactions and the real state objects
Honk.NPCs.States.Trigger(npc, ReactionName.ReactionTheftShock);
ScaredState scared = Honk.NPCs.States.Get<ScaredState>(npc, HumanState.Scared);

// React to any NPC changing state
Honk.NPCs.States.OnChanged += (npc, from, to) => { };
```

#### Brain control (`Honk.NPCs.Control`)

Movement targets, look targets, speeds, behavior permissions, goose awareness and reaction tuning:

```csharp
Honk.NPCs.Control.SetCasualSpeed(npc, 1.2f);
Honk.NPCs.Control.SetAllowVisiting(npc, false);
bool seen = Honk.NPCs.Control.CanSeeGoose(npc);
Honk.NPCs.Control.SetAssumedGoosePosition(npc, decoyPosition);
Honk.NPCs.Control.ForgetGoose(npc);
```

### 3. Interactive Props (`Honk.Props`)

The `Honk.Props` subsystem handles interactive objects and physics:

```csharp
// Procedural prop creation via builder
Prop box = Honk.Props.Create("CustomBox")
    .FromPrimitive(PrimitiveType.Cube)
    .WithScale(new Vector3(0.5f, 0.5f, 0.5f))
    .WithMass(2.0f)
    .MakeWettable(true)
    .Build();

// Cloning existing scene items
Prop duplicateBell = Honk.Props.CloneProp("Bell", spawnPosition);

// Physics application
Honk.Props.Launch(duplicateBell, new Vector3(0f, 5f, 10f));
Honk.Props.MakeWet(duplicateBell);
```

### 4. Audio Playback (`Honk.Audio`)

The `Honk.Audio` subsystem interfaces with FMOD sound events and runtime audio clips:

```csharp
// Invoke native FMOD audio events
Honk.Audio.Play("event:/Goose/Honk", goosePosition);

// Play runtime AudioClip assets
Honk.Audio.PlayCustom(audioClip, goosePosition, volume: 1.0f);
```

### 5. World and Environment (`Honk.World`)

The `Honk.World` subsystem interfaces with the global environment:

```csharp
string currentArea = Honk.World.CurrentArea;
Honk.World.SetSlowMotion(0.5f);
Honk.World.SetCameraDistance(15f);
Honk.World.Pause(true);
```

### 6. Event System (`Honk.Events`)

Static event delegates to subscribe to gameplay and engine hooks:

```csharp
Honk.Events.OnGooseSpawn += (goose) => {
    // Invoked when a goose entity initializes in the scene
};

Honk.Events.OnHonk += (goose, isGlobal) => {
    // Invoked when a honk action is triggered
};

Honk.Events.OnPropGrabbed += (goose, prop) => {
    // Invoked when an item is picked up by the beak
};

Honk.Events.OnUpdate += () => {
    // Invoked once per frame
};
```

### 7. Other Subsystems

```csharp
// Camera: distance, offset, rotation, follow target, freeze, shake. Reset() restores everything.
Honk.Camera.SetDistances(30f, 20f, 60f);
Honk.Camera.Shake(0.6f, 0.15f);

// Weather: lighting and atmosphere. ResetAtmosphere() restores the original values.
Honk.Weather.SetTimeOfDay(18.3f);
Honk.Weather.SetSun(Color.red, 1.2f);
Honk.Weather.ScaleAllLights(0.5f);

// Audio: pitched playback, FMOD bus volume
Honk.Audio.PlayPitched("event:/...", position, 1.5f);
Honk.Audio.SetBusVolume("bus:/", 0.5f);

// Goals: per-area queries and progress
int done, total;
Honk.Goals.GetProgress(GoalListArea.Garden, out done, out total);

// World: ground height, NavMesh sampling, area spawns
float y;
Honk.World.TryGetGroundHeight(position, out y);

// Physics: shockwaves that also move geese and scare NPCs
Honk.Physics.Shockwave(origin, 12f, 8f);

// Input: the game's own Rewired bindings, chords
Honk.Input.RegisterChord(KeyCode.LeftControl, KeyCode.K, () => { });

// Events
Honk.Events.OnAreaChange += (area) => { };
Honk.Events.OnNPCStateChanged += (npc, from, to) => { };
```

---

## Developing a Mod with HonkAPI

Mods implement the `IHonkMod` interface. Compiled mod assemblies placed in the plugins directory are automatically detected, instantiated, and initialized:

```csharp
using System;
using UnityEngine;
using HonkAPI;
using HonkAPI.Core;

namespace MyFirstMod
{
    public class MyFirstModPlugin : IHonkMod
    {
        public string ModId { get { return "com.author.myfirstmod"; } }
        public string ModName { get { return "My First Mod"; } }
        public string Version { get { return "1.0.0"; } }

        public void OnInitialize(HonkContext context)
        {
            context.Logger.LogInfo("MyFirstMod initialized.");

            Honk.Events.OnHonk += HandleHonk;
            Honk.Events.OnUpdate += HandleUpdate;
        }

        private void HandleHonk(Goose goose, bool isGlobal)
        {
            // Custom logic executed upon honking
            Honk.Props.CloneProp("Apple", goose.transform.position + Vector3.up);
        }

        private void HandleUpdate()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Honk.Goose.HonkSound(1.2f);
            }
        }
    }
}
```

---

## Compilation and Distribution

### References

Mod projects must reference:
- `HonkAPI.dll`
- `Assembly-CSharp.dll` (from `Untitled_Data/Managed/`)
- `UnityEngine.dll` and core modules (from `Untitled_Data/Managed/`)
- `BepInEx.dll` (from `BepInEx/core/`)
- `0Harmony.dll` (from `BepInEx/core/`)

### Installation

1. Place `HonkAPI.dll` into the `BepInEx/plugins/` directory.
2. Place any compiled mod assemblies implementing `IHonkMod` into `BepInEx/plugins/` or a `mods/` subfolder.
3. HonkAPI will load the mods at startup and register them in the native in-game Mods menu.

---

## Runtime Requirements

- **Game Version**: *Untitled Goose Game* v1.1.4 (x64 Unity 2018.4 runtime).
- **Mod Loader**: BepInEx 5.4.x (64-bit).
- **Target Framework**: .NET Framework 4.7.2 / .NET Standard 2.0.
