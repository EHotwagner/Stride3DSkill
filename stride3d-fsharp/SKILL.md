---
name: stride3d-fsharp
description: >-
  Use when the user asks to "create a Stride3D game in F#", "use Stride game engine with F#",
  "make a 3D game in F#", "add Stride3D to my F# project", "set up Stride code-only",
  "create 3D primitives in F#", "add physics to F# game", "Stride Bepu physics F#",
  or needs to build a game or 3D application using Stride3D from F#.
version: 1.0.0
---

# Stride3D Game Engine — F# Code-Only Workflow

Build games and 3D applications with [Stride3D](https://www.stride3d.net/) from F# using the
[stride-community-toolkit](https://github.com/stride3d/stride-community-toolkit) code-only approach.
No Stride Game Studio installation required.

## Reference Implementation

The complete working implementation is at: `/home/developer/tools/Stride3DSkill/`

When assisting the user, use those files as authoritative reference for:
- F# module patterns (`StrideHelpers.fs`, `SceneBuilder.fs`, `InputHandler.fs`, `PhysicsHelper.fs`, `UIHelper.fs`)
- Test patterns (`Stride3dFSharp.Tests/`)
- REPL usage (`repl-test.fsx`)

## Project Setup

### 1. Create the project

```bash
dotnet new console --language F# --framework net10.0 --name MyGame
cd MyGame
dotnet add package Stride.CommunityToolkit --prerelease
dotnet add package Stride.CommunityToolkit.Bepu --prerelease
dotnet add package Stride.CommunityToolkit.Skyboxes --prerelease
# Linux only:
dotnet add package Stride.CommunityToolkit.Linux --prerelease
```

Add `<RuntimeIdentifier>linux-x64</RuntimeIdentifier>` to the `.fsproj` PropertyGroup on Linux.

### 2. Required namespaces

```fsharp
open Stride.CommunityToolkit.Engine              // GameExtensions (AddGraphicsCompositor, Add3DCamera, etc.)
open Stride.CommunityToolkit.Bepu                // Create3DPrimitive, Raycast, Bepu3DPhysicsOptions
open Stride.CommunityToolkit.Skyboxes            // AddSkybox
open Stride.CommunityToolkit.Rendering.Compositing       // AddCleanUIStage
open Stride.CommunityToolkit.Rendering.ProceduralModels  // PrimitiveModelType
open Stride.CommunityToolkit.Helpers             // VectorHelper
open Stride.Core.Mathematics                     // Vector3, Color, Quaternion, Matrix
open Stride.Engine                               // Entity, Scene, CameraComponent, TransformComponent
open Stride.Games                                // Game, GameTime
open Stride.Input                                // InputManager, Keys, MouseButton
open Stride.BepuPhysics                          // BodyComponent, HitInfo, BepuSimulation
```

### 3. Minimal game entry point

```fsharp
open Stride.CommunityToolkit.Engine
open Stride.CommunityToolkit.Bepu
open Stride.CommunityToolkit.Skyboxes
open Stride.CommunityToolkit.Rendering.Compositing
open Stride.CommunityToolkit.Rendering.ProceduralModels
open Stride.Core.Mathematics
open Stride.Engine
open Stride.Games

let game = new Game()

let start (scene: Scene) =
    game.AddGraphicsCompositor().AddCleanUIStage() |> ignore
    game.Add3DCamera().Add3DCameraController() |> ignore
    game.AddDirectionalLight() |> ignore
    game.Add3DGround() |> ignore
    game.AddSkybox() |> ignore

    let capsule = game.Create3DPrimitive(PrimitiveModelType.Capsule)
    capsule.Transform.Position <- Vector3(0f, 8f, 0f)
    capsule.Scene <- scene

let update (scene: Scene) (time: GameTime) =
    ()

[<EntryPoint>]
let main _ =
    game.Run(
        start = System.Action<Scene>(start),
        update = System.Action<Scene, GameTime>(update))
    0
```

## Critical F# Interop Patterns

### Vector3 arithmetic

Stride's `Vector3` uses `inref<>` operator overloads that **do not work** with F#'s `+` or `*`.

```fsharp
// WRONG — won't compile:
let c = a + b

// CORRECT — use static methods:
let mutable result = Unchecked.defaultof<Vector3>
Vector3.Add(&a, &b, &result)

// Or use the helper from StrideHelpers:
let c = addVec3 a b
let s = scaleVec3 2f v
```

### Nullable value type parameters

Some C# extension methods take `Nullable<Vector3>`. Construct explicitly:

```fsharp
Stride.CommunityToolkit.Engine.GameExtensions.AddGroundGizmo(
    game, System.Nullable<Vector3>(Vector3(-5f, 0.1f, -5f)), showAxisName = true)
```

### Thickness constructor

`Stride.UI.Thickness` requires 4 explicit floats (no single-param overload):

```fsharp
Thickness(5f, 5f, 5f, 5f)            // left, top, right, bottom
Thickness.UniformRectangle(5f)        // uniform helper
```

### Extension methods

C# extension methods work when their namespace is opened. Call as static methods if F# doesn't resolve them:

```fsharp
// Usually works:
game.AddGraphicsCompositor()

// If not resolved, use fully qualified static call:
Stride.CommunityToolkit.Engine.GameExtensions.AddGroundGizmo(game, ...)
```

## Available Extension Methods (by package)

### Stride.CommunityToolkit (GameExtensions)
`Run`, `SetupBase2D`, `SetupBase3D`, `AddGraphicsCompositor`, `Add2DCamera`, `Add3DCamera`,
`AddDirectionalLight`, `AddAllDirectionLighting`, `AddGroundGizmo`, `AddProfiler`,
`CreateMaterial`, `CreateFlatMaterial`, `Create3DPrimitive`, `Create2DPrimitive`

### Stride.CommunityToolkit.Bepu (GameExtensions)
`SetupBase2DScene`, `SetupBase3DScene`, `Add2DGround`, `Add3DGround`,
`Create2DPrimitive`, `Create3DPrimitive`, `CreateGround`

### Stride.CommunityToolkit.Bepu (CameraComponentExtensions)
`Raycast`, `RaycastMouse`

### Stride.CommunityToolkit.Skyboxes
`AddSkybox`

### GraphicsCompositorExtensions
`AddCleanUIStage`, `AddUIStage`, `AddSceneRenderer`, `AddRootRenderFeature`

### EntityExtensions
`Add3DCameraController`, `Add2DCameraController`, `AddGizmo`, `AddLightDirectionalGizmo`

## Building & Testing

```bash
# Build (skip asset compiler for development/CI):
dotnet build -p:StrideCompilerSkipBuild=true

# Run tests (no GPU required):
dotnet test -p:StrideCompilerSkipBuild=true

# Run game (requires GPU + display + audio):
dotnet build -p:StrideCompilerSkipBuild=true
dotnet bin/Debug/net10.0/linux-x64/MyGame.dll
```

### Linux prerequisites

```bash
sudo pacman -S freetype2 ttf-liberation mesa libgl sdl2 openal   # Arch
sudo apt install libfreetype6 fonts-liberation libgl1-mesa-glx libsdl2-2.0-0 libopenal1  # Debian/Ubuntu
```

## NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `Stride.CommunityToolkit` | 1.0.0-preview.62 | Core helpers, materials, game extensions |
| `Stride.CommunityToolkit.Bepu` | 1.0.0-preview.62 | Bepu physics, 3D primitives, raycasting |
| `Stride.CommunityToolkit.Skyboxes` | 1.0.0-preview.62 | Skybox rendering |
| `Stride.CommunityToolkit.Linux` | 1.0.0-preview.62 | Linux runtime (asset compiler, native deps) |
