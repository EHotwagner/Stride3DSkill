/// Common helpers and type aliases for Stride3D F# development.
/// Wraps C# extension methods into idiomatic F# functions.
module Stride3dFSharp.StrideHelpers

open Stride.Core.Mathematics
open Stride.Engine
open Stride.Games
open Stride.CommunityToolkit.Engine
open Stride.CommunityToolkit.Bepu
open Stride.CommunityToolkit.Skyboxes
open Stride.CommunityToolkit.Rendering.Compositing
open Stride.CommunityToolkit.Rendering.ProceduralModels

// ---------------------------------------------------------------------------
// Game bootstrap helpers – thin F# wrappers around the C# extension methods
// ---------------------------------------------------------------------------

/// Set up the default 3D graphics compositor with a clean UI stage.
let setupGraphicsCompositor (game: Game) =
    game.AddGraphicsCompositor().AddCleanUIStage() |> ignore
    game

/// Add a 3D camera with default mouse-look controller.
let addCamera (game: Game) =
    game.Add3DCamera().Add3DCameraController() |> ignore
    game

/// Add a single directional light.
let addLight (game: Game) =
    game.AddDirectionalLight() |> ignore
    game

/// Add a ground plane with Bepu physics.
let addGround (game: Game) =
    game.Add3DGround() |> ignore
    game

/// Add a skybox.
let addSkybox (game: Game) =
    game.AddSkybox() |> ignore
    game

/// Add the profiler overlay (Shift+Ctrl+P to toggle).
let addProfiler (game: Game) =
    game.AddProfiler() |> ignore
    game

/// Full default 3D scene: compositor, camera, light, ground, skybox.
let setupDefault3DScene (game: Game) =
    game
    |> setupGraphicsCompositor
    |> addCamera
    |> addLight
    |> addGround
    |> addSkybox

// ---------------------------------------------------------------------------
// Entity creation helpers
// ---------------------------------------------------------------------------

/// Create a 3D primitive with Bepu physics and add it to the scene.
let createPrimitive (game: Game) (scene: Scene) (primitiveType: PrimitiveModelType) (position: Vector3) =
    let entity = game.Create3DPrimitive(primitiveType)
    entity.Transform.Position <- position
    entity.Scene <- scene
    entity

/// Create a 3D primitive with custom Bepu physics options.
let createPrimitiveWithOptions
    (game: Game)
    (scene: Scene)
    (primitiveType: PrimitiveModelType)
    (position: Vector3)
    (options: Bepu3DPhysicsOptions)
    =
    let entity = game.Create3DPrimitive(primitiveType, options)
    entity.Transform.Position <- position
    entity.Scene <- scene
    entity

/// Create a coloured primitive (convenience wrapper).
let createColouredPrimitive
    (game: Game)
    (scene: Scene)
    (primitiveType: PrimitiveModelType)
    (position: Vector3)
    (color: Color)
    =
    let options = Bepu3DPhysicsOptions(Material = game.CreateMaterial(color))
    createPrimitiveWithOptions game scene primitiveType position options

/// Create a primitive without a collider.
let createNonPhysicalPrimitive
    (game: Game)
    (scene: Scene)
    (primitiveType: PrimitiveModelType)
    (position: Vector3)
    (color: Color)
    =
    let options =
        Bepu3DPhysicsOptions(
            Material = game.CreateMaterial(color),
            IncludeCollider = false
        )
    createPrimitiveWithOptions game scene primitiveType position options

// ---------------------------------------------------------------------------
// Material helpers
// ---------------------------------------------------------------------------

/// Create a standard material with the given colour.
let createMaterial (game: IGame) (color: Color) =
    game.CreateMaterial(color)

/// Create a flat (unlit) material.
let createFlatMaterial (game: IGame) (color: Color) =
    game.CreateFlatMaterial(System.Nullable<Color>(color))

// ---------------------------------------------------------------------------
// Game.Run wrapper
// ---------------------------------------------------------------------------

/// Run the game with start and update callbacks, returning the exit code.
let runGame (game: Game) (start: Scene -> unit) (update: Scene -> GameTime -> unit) =
    game.Run(
        start = System.Action<Scene>(start),
        update = System.Action<Scene, GameTime>(update)
    )
    0
