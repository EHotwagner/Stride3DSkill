/// Declarative scene construction using an F# computation-expression style builder.
module Stride3dFSharp.SceneBuilder

open Stride.Core.Mathematics
open Stride.Engine
open Stride.Games
open Stride.CommunityToolkit.Rendering.ProceduralModels
open Stride3dFSharp.StrideHelpers

// ---------------------------------------------------------------------------
// Scene element descriptors – pure data, no side-effects
// ---------------------------------------------------------------------------

type PrimitiveSpec =
    { PrimitiveType: PrimitiveModelType
      Position: Vector3
      Color: Color option
      HasCollider: bool }

type SceneSetup =
    { UseCompositor: bool
      UseCamera: bool
      UseLight: bool
      UseGround: bool
      UseSkybox: bool
      UseProfiler: bool
      Primitives: PrimitiveSpec list }

let defaultScene =
    { UseCompositor = true
      UseCamera = true
      UseLight = true
      UseGround = true
      UseSkybox = true
      UseProfiler = false
      Primitives = [] }

// ---------------------------------------------------------------------------
// Builder functions – compose scene descriptions
// ---------------------------------------------------------------------------

let withProfiler setup = { setup with UseProfiler = true }
let withoutSkybox setup = { setup with UseSkybox = false }
let withoutGround setup = { setup with UseGround = false }

let addPrimitive primitiveType position setup =
    let spec =
        { PrimitiveType = primitiveType
          Position = position
          Color = None
          HasCollider = true }
    { setup with Primitives = setup.Primitives @ [ spec ] }

let addColouredPrimitive primitiveType position color setup =
    let spec =
        { PrimitiveType = primitiveType
          Position = position
          Color = Some color
          HasCollider = true }
    { setup with Primitives = setup.Primitives @ [ spec ] }

let addDecorativePrimitive primitiveType position color setup =
    let spec =
        { PrimitiveType = primitiveType
          Position = position
          Color = Some color
          HasCollider = false }
    { setup with Primitives = setup.Primitives @ [ spec ] }

// ---------------------------------------------------------------------------
// Scene realisation – applies the description to a running game
// ---------------------------------------------------------------------------

/// Materialise a SceneSetup into actual Stride entities.
let realise (game: Game) (scene: Scene) (setup: SceneSetup) =
    if setup.UseCompositor then setupGraphicsCompositor game |> ignore
    if setup.UseCamera then addCamera game |> ignore
    if setup.UseLight then addLight game |> ignore
    if setup.UseGround then addGround game |> ignore
    if setup.UseSkybox then addSkybox game |> ignore
    if setup.UseProfiler then addProfiler game |> ignore

    setup.Primitives
    |> List.map (fun spec ->
        match spec.Color, spec.HasCollider with
        | None, _ ->
            createPrimitive game scene spec.PrimitiveType spec.Position
        | Some c, true ->
            createColouredPrimitive game scene spec.PrimitiveType spec.Position c
        | Some c, false ->
            createNonPhysicalPrimitive game scene spec.PrimitiveType spec.Position c)
