# Stride3D F# Agent Skill

Use the [Stride3D](https://www.stride3d.net/) game engine from **F#** via a code-only workflow, powered by the [stride-community-toolkit](https://github.com/stride3d/stride-community-toolkit). No Stride Game Studio required.

## Modules

| Module | Purpose |
|--------|---------|
| `StrideHelpers.fs` | Pipeable F# wrappers for C# extension methods (game setup, primitives, materials) |
| `SceneBuilder.fs` | Declarative scene construction with pure data records |
| `InputHandler.fs` | Functional input mapping: keyboard state -> DU actions -> normalised movement vectors |
| `PhysicsHelper.fs` | Bepu physics: impulses, raycasting, body queries |
| `UIHelper.fs` | Text overlays, HUD panels |
| `Program.fs` | Demo composing all modules |

## Quick Start

```bash
# Tests (no GPU required) — 73 tests
dotnet test Stride3dFSharp.Tests -p:StrideCompilerSkipBuild=true

# REPL (no GPU required) — 10 interactive tests
dotnet fsi repl-test.fsx

# Game (requires GPU + display + audio)
./run.sh
```

### Linux Prerequisites

```bash
# Arch Linux
sudo pacman -S freetype2 ttf-liberation mesa libgl sdl2 openal

# Ubuntu/Debian
sudo apt install libfreetype6 fonts-liberation libgl1-mesa-glx libsdl2-2.0-0 libopenal1
```

## Key F# Interop Notes

- **Vector3 `+` operator** doesn't work directly from F# (Stride uses `inref<>` params). Use the provided `addVec3`/`scaleVec3` helpers or `Vector3.Add(&a, &b, &result)`.
- **Extension methods** work when their namespaces are opened (`Stride.CommunityToolkit.Engine`, `.Bepu`, `.Skyboxes`).
- **`Nullable<T>`** parameters need `System.Nullable<Vector3>(v)`.
- **`Thickness`** needs 4 explicit floats or `Thickness.UniformRectangle(f)`.
- **Asset compiler** can be skipped for tests/REPL with `-p:StrideCompilerSkipBuild=true`.

## NuGet Packages

| Package | Purpose |
|---------|---------|
| `Stride.CommunityToolkit` | Core helpers, GameExtensions, materials |
| `Stride.CommunityToolkit.Bepu` | Bepu physics, Create3DPrimitive, raycasting |
| `Stride.CommunityToolkit.Skyboxes` | Skybox support |
| `Stride.CommunityToolkit.Linux` | Linux runtime support |

## Example: Declarative Scene

```fsharp
open Stride3dFSharp.SceneBuilder
open Stride3dFSharp.StrideHelpers
open Stride.CommunityToolkit.Rendering.ProceduralModels

let myScene =
    defaultScene
    |> withProfiler
    |> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 8f, 0f))
    |> addColouredPrimitive PrimitiveModelType.Cube (Vector3(-3f, 5f, 0f)) Color.Orange
    |> addDecorativePrimitive PrimitiveModelType.Cube (Vector3(2f, 0.5f, 0f)) Color.Gold

// In game start callback:
let entities = realise game scene myScene
```

## License

MIT
