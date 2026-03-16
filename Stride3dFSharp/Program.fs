/// Stride3D F# Demo – showcases the helper modules.
/// Run with: dotnet run
module Stride3dFSharp.Program

open Stride.Core.Mathematics
open Stride.Engine
open Stride.Games
open Stride.CommunityToolkit.Rendering.ProceduralModels
open Stride.CommunityToolkit.Helpers
open Stride3dFSharp.StrideHelpers
open Stride3dFSharp.SceneBuilder
open Stride3dFSharp.InputHandler
open Stride3dFSharp.PhysicsHelper

let game = new Game()

// Mutable state for the demo
let mutable playerEntity: Entity option = None

let start (scene: Scene) =
    // Build the scene declaratively
    let setup =
        defaultScene
        |> withProfiler
        |> addPrimitive PrimitiveModelType.Capsule (Vector3(0f, 8f, 0f))
        |> addColouredPrimitive PrimitiveModelType.Cube (Vector3(-3f, 5f, 0f)) Color.Orange
        |> addDecorativePrimitive PrimitiveModelType.Cube (Vector3(2f, 0.5f, 0f)) Color.Gold

    let entities = realise game scene setup
    playerEntity <- entities |> List.tryHead

    Stride.CommunityToolkit.Engine.GameExtensions.AddGroundGizmo(
        game, System.Nullable<Vector3>(Vector3(-5f, 0.1f, -5f)), showAxisName = true)

let update (scene: Scene) (time: GameTime) =
    let dt = float32 time.Elapsed.TotalSeconds
    let actions = pollActions game.Input

    // Move the player capsule via transform
    match playerEntity with
    | Some entity ->
        let dir = computeMovement actions
        applyTransformMovement 3f dt dir entity
    | None -> ()

    // Spawn cubes when Q is held
    if hasAction SpawnCube actions then
        let entity =
            createColouredPrimitive
                game scene PrimitiveModelType.Cube
                (VectorHelper.RandomVector3([| -3f; 3f |], [| 10f; 13f |], [| -3f; 3f |]))
                Color.Green
        // Random scatter
        applyRandomImpulse 5f entity

[<EntryPoint>]
let main _argv =
    runGame game start update
