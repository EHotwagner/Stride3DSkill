# Stride3D F# Agent Skill

## Overview

This agent skill enables using the **Stride3D game engine** from **F#** via a code-only workflow powered by the [stride-community-toolkit](https://github.com/stride3d/stride-community-toolkit). No Stride Game Studio installation required.

## Architecture

```
Stride3dFSharp/
├── StrideHelpers.fs      # F# wrappers for C# extension methods (Game setup, primitives, materials)
├── SceneBuilder.fs       # Declarative scene construction with pure data descriptors
├── InputHandler.fs       # Functional input mapping (keyboard → actions → movement vectors)
├── PhysicsHelper.fs      # Bepu physics helpers (impulses, raycasting, body queries)
├── UIHelper.fs           # UI construction (text overlays, HUD panels)
└── Program.fs            # Demo entry point composing all modules

Stride3dFSharp.Tests/
├── MathTests.fs          # Vector3, Color, Matrix, Quaternion interop
├── InputTests.fs         # Input mapping and movement computation
├── SceneBuilderTests.fs  # Declarative scene builder
├── PhysicsTests.fs       # Physics helpers (no GPU needed)
├── UITests.fs            # UI type construction
└── IntegrationTests.fs   # Cross-module integration, entity hierarchy, complex scenes
```

## Key F# Patterns for Stride3D

### Vector3 Addition Workaround
Stride's `Vector3` uses `inref<>` operators that don't work directly with F#'s `+` operator.
Use the provided helpers:
```fsharp
let result = addVec3 a b         // Instead of a + b
let scaled = scaleVec3 2f v      // Instead of v * 2f
```

### Extension Methods
C# extension methods need their namespace opened:
```fsharp
open Stride.CommunityToolkit.Engine      // GameExtensions
open Stride.CommunityToolkit.Bepu        // Create3DPrimitive, Raycast
open Stride.CommunityToolkit.Skyboxes    // AddSkybox
open Stride.CommunityToolkit.Rendering.Compositing  // AddCleanUIStage
```

### Nullable Parameters
Some Stride APIs take `Nullable<T>`. Use explicit construction:
```fsharp
System.Nullable<Vector3>(Vector3(1f, 2f, 3f))
```

### Thickness Constructor
`Thickness` requires 4 floats (left, top, right, bottom):
```fsharp
Thickness(5f, 5f, 5f, 5f)           // explicit
Thickness.UniformRectangle(5f)       // uniform helper
```

## Running

### Tests (no GPU required)
```bash
cd Stride3dFSharp.Tests
dotnet test -p:StrideCompilerSkipBuild=true
```

### REPL (no GPU required)
```bash
dotnet fsi repl-test.fsx
```

### Game (requires GPU + display + audio)
```bash
# Prerequisites: freetype2 ttf-liberation mesa libgl sdl2 openal
./run.sh                    # foreground
./run.sh --background       # background (returns PID)
./run.sh --build-assets     # with full asset compilation
```

## NuGet Packages

| Package | Purpose |
|---------|---------|
| `Stride.CommunityToolkit` | Core helpers, GameExtensions, materials |
| `Stride.CommunityToolkit.Bepu` | Bepu physics, Create3DPrimitive, raycasting |
| `Stride.CommunityToolkit.Skyboxes` | Skybox support |
| `Stride.CommunityToolkit.Linux` | Linux runtime support |

## API Quick Reference

### Game Setup (pipeline style)
```fsharp
game |> setupGraphicsCompositor |> addCamera |> addLight |> addGround |> addSkybox
// or: setupDefault3DScene game
```

### Primitive Creation
```fsharp
createPrimitive game scene PrimitiveModelType.Capsule (Vector3(0f, 8f, 0f))
createColouredPrimitive game scene PrimitiveModelType.Cube pos Color.Red
createNonPhysicalPrimitive game scene PrimitiveModelType.Sphere pos Color.Gold
```

### Declarative Scene Builder
```fsharp
defaultScene
|> withProfiler
|> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 5f, 0f))
|> addColouredPrimitive PrimitiveModelType.Cube (Vector3(3f, 1f, 0f)) Color.Red
|> realise game scene
```

### Physics
```fsharp
applyImpulse (Vector3(0f, 10f, 0f)) entity
applyJump 5f entity
applyRandomImpulse 20f entity
raycastFromCamera camera mousePos 100f  // returns Entity option
```

### Input Handling
```fsharp
let actions = pollActions game.Input
let direction = computeMovement actions
applyTransformMovement speed deltaTime direction entity
```
